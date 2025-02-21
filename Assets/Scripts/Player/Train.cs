using ClumsyWizard.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using Random = UnityEngine.Random;

public class PassengerTraveDetails
{
    public int Count;
    public int StationsLeft;
    public int Fee;

    public PassengerTraveDetails(int count, int stationsLeft, int fee)
    {
        Count = count;
        StationsLeft = stationsLeft;
        Fee = fee;
    }
}

public enum RepairType
{
    Hull,
    Engine,
    CargoArea,
}

public class RepairData
{
    public string Name;
    public Sprite Icon;
    public int MetalCost;
    public int ScrewCost;

    public RepairData(string name, Sprite icon, int metalCost, int screwCost)
    {
        Name = name;
        Icon = icon;
        MetalCost = metalCost;
        ScrewCost = screwCost;
    }
}

public class Train : MonoBehaviour
{
    public WeaponSystem WeaponSystem { get; private set; }

    [Header("Upgrade")]
    [SerializeField] private CargoCompartment cargoCompartmentPrefab;
    [SerializeField] private float newCompartmentSpace;

    [Header("Compartments")]
    [SerializeField] private EngineCompartment engineCompartment ;
    [SerializeField] private WorkerCompartment workerCompartment;
    [SerializeField] private List<CargoCompartment> cargoCompartments;

    [Header("Stats")]
    [SerializeField] private int hullIntegrity;
    private int currentHullIntegrity;
    private int cargoSpaceBonus;

    [Header("Repair")]
    [SerializeField] private Vector2Int repairMetalCostRange;
    [SerializeField] private Vector2Int repairScrewCostRange;
    [SerializeField] private CW_Dictionary<RepairType, Sprite> repairIcons;
    private float repairCheckRange;
    public List<RepairData> RequiredRepairs { get; private set; } = new List<RepairData>();

    //Stats and cargo
    private CW_Dictionary<StatType, int> passengerState = new CW_Dictionary<StatType, int>();
    private List<PassengerTraveDetails> passengerDetails = new List<PassengerTraveDetails>();
    private CW_Dictionary<CargoType, int> cargo = new CW_Dictionary<CargoType, int>();

    //Bounds
    private Bounds bound;

    public void Initialize()
    {
        WeaponSystem= GetComponentInChildren<WeaponSystem>();

        engineCompartment.Initialize(this);
        workerCompartment.Initialize(this);
        for (int i = 0; i < cargoCompartments.Count; i++)
        {
            cargoCompartments[i].Initialize(this);
            if (i == cargoCompartments.Count - 1)
                cargoCompartments[i].SetEnd(true);
            else
                cargoCompartments[i].SetEnd(false);
        }
        CreateBounds();

        AddPassengerStat(StatType.Infection, 0);
        AddPassengerStat(StatType.Anger, 0);

        transform.position = new Vector3((transform.childCount - 1.0f) * 2, transform.position.y, 0.0f);

        currentHullIntegrity = hullIntegrity;
        CalculateRepairCheckRange();

        HealthModified();
        cargoSpaceBonus = 0;

        GameManager.Instance.OnStateChange += (GameState state) =>
        {
            if (state == GameState.Station)
                ProcessPassengerDetails();
        };
    }

    public void ProcessPassengerDetails()
    {
        for (int i = passengerDetails.Count - 1; i >= 0; i--)
        {
            PassengerTraveDetails details = passengerDetails[i];
            details.StationsLeft--;

            if(details.StationsLeft <= 0)
            {
                RemoveCargo(CargoType.Passenger, details.Count);
                PlayerDataManager.Instance.AddBullets(details.Count * details.Fee);

                UpdatePassengerStatUI();

                passengerDetails.RemoveAt(i);
            }
        }
    }

    //Health
    public void RepairIssue(int index)
    {
        Heal((int)((hullIntegrity - currentHullIntegrity) / (float)RequiredRepairs.Count));
        RemovePassengerStat(StatType.Anger, 5); // 5% decrease in anger for every issue fixed
        RequiredRepairs.RemoveAt(index);
    }
    public void Damage(int amount)
    {
        currentHullIntegrity -= amount;
        if (currentHullIntegrity < 0)
        {
            currentHullIntegrity = 0;
            Dead();
        }

        if(currentHullIntegrity < repairCheckRange)
        {
            RepairType type = (RepairType)Random.Range(0, Enum.GetValues(typeof(RepairType)).Length);
            RequiredRepairs.Add(new RepairData(type.ToString(), repairIcons[type], Random.Range(repairMetalCostRange.x, repairMetalCostRange.y + 1), Random.Range(repairScrewCostRange.x, repairScrewCostRange.y + 1)));

            AddPassengerStat(StatType.Anger, 5); // 5% increase in anger for every repair required

            CalculateRepairCheckRange();
        }

        HealthModified();
    }

    public void Heal(int amount)
    {
        currentHullIntegrity += amount;
        if (currentHullIntegrity > hullIntegrity)
            currentHullIntegrity = hullIntegrity;

        CalculateRepairCheckRange();
        HealthModified();
    }

    private void Dead()
    {
        GameManager.Instance.GameOver("Your train was destroyed and your enterprise has reached its last station forever.");
    }

    private void CalculateRepairCheckRange()
    {
        repairCheckRange = currentHullIntegrity * 0.75f; //Every 25% decrease in health will add a new repair data
    }

    //Stats
    public void AddPassengerStat(StatType type, int amount)
    {
        if (amount > GetCargoCount(CargoType.Passenger))
            return;

        if (passengerState.ContainsKey(type))
        {
            passengerState[type] += amount;
            if (passengerState[type] >= 100)
            {
                if(type == StatType.Anger)
                {
                    GameManager.Instance.GameOver("Your passengers had enough of your management style. They took over your train and hanged you from the end of your own train. You ended up being the final decoration on your train.");
                }
                else
                {
                    GameManager.Instance.GameOver("Your train got run over from within. As the horde of infected passengers pounded at your door, you used the last bullet to end things on your own terms.");
                }
            }
        }
        else
        {
            passengerState.Add(type, amount);
        }

        UpdatePassengerStatUI();
    }
    public void RemovePassengerStat(StatType type, int amount)
    {
        if (!passengerState.ContainsKey(type))
            return;

        passengerState[type] -= amount;
        if (passengerState[type] < 0)
            passengerState[type] = 0;

        UpdatePassengerStatUI();
    }

    //Worker
    public void AddWorker(WorkerType type, int amount)
    { 
        workerCompartment.AddWorker(type, amount);
    }

    //Cargo Usage
    public void AddPassengerDetials(int count, int stations, int fee)
    {
        passengerDetails.Add(new PassengerTraveDetails(count, stations, fee));
        GameManager.Instance.PassengersCarried += count;
        AddCargo(CargoType.Passenger, count);
    }
    public bool AddCargo(CargoType type, int amount)
    {
        if (amount > (type == CargoType.Passenger ? GetPassengerCapacity() : GetResourceCapacity()))
            return false;

        if (cargo.ContainsKey(type))
        {
            if (cargo[type] + amount > (type == CargoType.Passenger ? GetPassengerCapacity() : GetResourceCapacity()))
                return false;

            cargo[type] += amount;
        }
        else
        {
            cargo.Add(type, amount); 
        }

        if (type == CargoType.Passenger)
            UpdatePassengerStatUI();

        PlayerDataManager.Instance.UpdateCargoUI();
        return true;
    }
    public bool RemoveCargo(CargoType type, int amount)
    {
        if (!cargo.ContainsKey(type) || cargo[type] - amount < 0)
            return false;

        cargo[type] -= amount;
        if (type == CargoType.Passenger)
            UpdatePassengerStatUI();

        PlayerDataManager.Instance.UpdateCargoUI();
        return true;
    }
    public bool HasEnoughCargo(CargoType type, int amount)
    {
        if (!cargo.ContainsKey(type))
            return false;

        return cargo[type] - amount >= 0;
    }

    //Cargo Getters
    public int GetCargoCount(CargoType type)
    {
        return cargo[type];
    }
    public int GetPassengerCapacity()
    {
        int total = cargoSpaceBonus;
        for (int i = 0; i < cargoCompartments.Count; i++)
        {
            total += cargoCompartments[i].Capacity;
        }

        return total - (cargo[CargoType.Metal] + cargo[CargoType.Screw]);
    }

    public int GetResourceCapacity()
    {
        int total = cargoSpaceBonus;
        for (int i = 0; i < cargoCompartments.Count; i++)
        {
            total += cargoCompartments[i].Capacity;
        }

        return total - cargo[CargoType.Passenger];
    }

    //Upgrade
    public void IncreaseMaxhullIntegrity(int amount)
    {
        hullIntegrity += amount;
        currentHullIntegrity += amount;
        HealthModified();
    }
    public void IncreaseCargoSpace(int amount)
    {
        cargoSpaceBonus += amount;
        PlayerDataManager.Instance.UpdateCargoUI();
    }
    public void AddCargoCompartment()
    {
        CargoCompartment compartment = Instantiate(cargoCompartmentPrefab, transform);
        compartment.transform.position = new Vector3(cargoCompartments[cargoCompartments.Count - 1].transform.position.x - newCompartmentSpace, transform.position.y, transform.position.z);
        compartment.Initialize(this);
        cargoCompartments[cargoCompartments.Count - 1].SetEnd(false);
        compartment.SetEnd(true);
        cargoCompartments.Add(compartment);
        transform.position = new Vector3((transform.childCount - 1.0f) * 2, transform.position.y, 0.0f);
        PlayerDataManager.Instance.UpdateCargoUI();
        CreateBounds();
    }
    public void IncreaseWorkerCapacity(WorkerType type, int amount)
    {
        workerCompartment.IncreaseWorkerCapacity(type, amount);
    }
    public void IncreaseDamage(int amount)
    {
        WeaponSystem.IncreaseDamage(amount);
    }

    //UI
    public void HealthModified()
    {
        PlayerDataManager.Instance.UpdateStatUI(StatType.HullIntegrity, currentHullIntegrity, hullIntegrity);
    }
    public void UpdatePassengerStatUI()
    {
        PlayerDataManager.Instance.UpdateStatUI(StatType.Infection, passengerState[StatType.Infection], 100);
        PlayerDataManager.Instance.UpdateStatUI(StatType.Anger, passengerState[StatType.Anger], 100);
    }

    //Helper functions
    private void CreateBounds()
    {
        bound = new Bounds();
        bound.Encapsulate(engineCompartment.transform.position);
        bound.Encapsulate(workerCompartment.transform.position);
        for (int i = 0; i < cargoCompartments.Count; i++)
        {
            bound.Encapsulate(cargoCompartments[i].transform.position);
        }
    }
    public Vector2 GetRandomPointOnBody()
    {
        return new Vector2(Random.Range(bound.min.x, bound.max.x), Random.Range(bound.min.y, bound.max.y));
    }
}
