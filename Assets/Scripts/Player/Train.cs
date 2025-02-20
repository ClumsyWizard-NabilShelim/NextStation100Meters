using ClumsyWizard.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
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
    [Header("Upgrade")]
    [SerializeField] private CargoCompartment cargoCompartmentPrefab;
    [SerializeField] private float newCompartmentSpace;

    [Header("Compartments")]
    [SerializeField] private EngineCompartment engineCompartment ;
    [SerializeField] private WorkerCompartment workerCompartment;
    [SerializeField] private List<CargoCompartment> cargoCompartments;

    [Header("Stats")]
    [SerializeField] private int hullIntegrity;
    [SerializeField] private int damage;
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

    public void Initialize()
    {
        engineCompartment.Initialize(this);
        workerCompartment.Initialize(this);
        for (int i = 0; i < cargoCompartments.Count; i++)
        {
            cargoCompartments[i].Initialize(this);
        }

        AddPassengerStat(StatType.Infection, 0);
        AddPassengerStat(StatType.Anger, 0);

        transform.position = new Vector3(transform.childCount - 1.0f, transform.position.y, 0.0f);

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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Damage(25);
        }
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
        Debug.Log("Dead");
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
            if (passengerState[type] > GetCargoCount(CargoType.Passenger))
                passengerState[type] = GetCargoCount(CargoType.Passenger);
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
        compartment.transform.position = new Vector3(-(1 + cargoCompartments.Count) * newCompartmentSpace, transform.position.y, transform.position.z);
        compartment.Initialize(this);
        cargoCompartments.Add(compartment);
        PlayerDataManager.Instance.UpdateCargoUI();
    }
    public void IncreaseWorkerCapacity(WorkerType type, int amount)
    {
        workerCompartment.IncreaseWorkerCapacity(type, amount);
    }
    public void IncreaseDamage(int amount)
    {
        damage += amount;
    }

    //UI
    public void HealthModified()
    {
        PlayerDataManager.Instance.UpdateStatUI(StatType.HullIntegrity, currentHullIntegrity, hullIntegrity);
    }
    public void UpdatePassengerStatUI()
    {
        PlayerDataManager.Instance.UpdateStatUI(StatType.Infection, (passengerState[StatType.Infection] / GetPassengerCapacity()) * 100, 100);
        PlayerDataManager.Instance.UpdateStatUI(StatType.Anger, (passengerState[StatType.Anger] / GetPassengerCapacity()) * 100, 100);
    }
}
