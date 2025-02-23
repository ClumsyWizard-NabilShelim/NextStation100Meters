using ClumsyWizard.Audio;
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
    Worker_Area,
    Engine,
    Cargo_Area,
}

public class RepairData
{
    public string ID;
    public RepairType Type;
    public Sprite Icon;
    public int MetalCost;
    public int ScrewCost;
    public float TimeRequired;

    public RepairData(string id, RepairType type, Sprite icon, int metalCost, int screwCost, float timeRequired)
    {
        ID = id;
        Type = type;
        Icon = icon;
        MetalCost = metalCost;
        ScrewCost = screwCost;
        TimeRequired = timeRequired;
    }
}

public class Train : MonoBehaviour
{
    private CW_AudioPlayer audioPlayer;
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
    [SerializeField] private Vector2Int repairMetalMaxCostRange;
    [SerializeField] private Vector2Int repairScrewCostRange;
    [SerializeField] private Vector2Int repairScrewMaxCostRange;
    [SerializeField] private int repairIncreaseThreshold;
    [SerializeField] private CW_Dictionary<RepairType, Sprite> repairIcons;
    private float repairCheckRange;
    public Dictionary<string, RepairData> RequiredRepairs { get; private set; } = new Dictionary<string, RepairData>();
    private int repairIDGenerationNumber;
    private int previousCostIncrementCount;

    [Header("Visuals")]
    [SerializeField] private float timeBetweenExplosion;
    private List<TrainExplosionVFX> explosionParticleSystems = new List<TrainExplosionVFX>();
    private bool canExplode;

    //Stats and cargo
    private CW_Dictionary<StatType, int> passengerState = new CW_Dictionary<StatType, int>();
    private List<PassengerTraveDetails> passengerDetails = new List<PassengerTraveDetails>();
    private CW_Dictionary<CargoType, int> cargo = new CW_Dictionary<CargoType, int>();

    //Bounds
    private Bounds bound;

    public void Initialize()
    {
        audioPlayer = GetComponent<CW_AudioPlayer>();
        WeaponSystem= GetComponentInChildren<WeaponSystem>();

        canExplode = true;

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
        GetExplosionParticleSytmes();
        CreateBounds();

        AddPassengerStat(StatType.Anger, 0);

        transform.position = new Vector3((transform.childCount - 1.0f) * 2, transform.position.y, 0.0f);

        currentHullIntegrity = hullIntegrity;
        CalculateRepairCheckRange();

        HealthModified();
        cargoSpaceBonus = 0;

        GameManager.Instance.OnStateChange += (GameState state) =>
        {
            if (state == GameState.Travelling)
                audioPlayer.Play("Train");
            else if (state == GameState.Station)
                ProcessPassengerDetails();
            else if(state == GameState.Over)
                audioPlayer.Stop("Train");
        };

        GameManager.Instance.OnStationReached += () =>
        {
            if (GameManager.Instance.StationsReached - previousCostIncrementCount >= repairIncreaseThreshold)
            {
                previousCostIncrementCount = GameManager.Instance.StationsReached;
                repairMetalCostRange = new Vector2Int(Mathf.Min(repairMetalCostRange.x + 1, repairMetalMaxCostRange.x), Mathf.Min(repairMetalCostRange.y + 1, repairMetalMaxCostRange.y));
                repairScrewCostRange = new Vector2Int(Mathf.Min(repairScrewCostRange.x + 1, repairScrewMaxCostRange.x), Mathf.Min(repairScrewCostRange.y + 1, repairScrewMaxCostRange.y));
            }
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
                UpdatePassengerStatUI();

                passengerDetails.RemoveAt(i);
            }
        }

        RemovePassengerStat(StatType.Anger, Mathf.CeilToInt(passengerState[StatType.Anger] * 0.05f)); // 5% decrease in total anger
    }

    public void RemovePassengerData(int amount)
    {
        if (passengerDetails.Count == 0)
            return;

        if(amount <= passengerDetails[0].Count)
        {
            passengerDetails[0].Count -= amount;
            RemoveCargo(CargoType.Passenger, amount);
            if (passengerDetails[0].Count <= 0)
                passengerDetails.RemoveAt(0);
        }
        else
        {
            int diff = amount - passengerDetails[0].Count;
            RemoveCargo(CargoType.Passenger, passengerDetails[0].Count);
            passengerDetails.RemoveAt(0);
            RemovePassengerData(diff);
        }
    }

    public Worker GetWorker()
    {
        return workerCompartment.GetWorker();
    }

    //Health
    public void RepairIssueAndChargeResource(string id)
    {
        PlayerDataManager.Instance.Train.RemoveCargo(CargoType.Metal, RequiredRepairs[id].MetalCost);
        PlayerDataManager.Instance.Train.RemoveCargo(CargoType.Screw, RequiredRepairs[id].ScrewCost);

        RepairType type = RequiredRepairs[id].Type;
        Heal((int)((hullIntegrity - currentHullIntegrity) / (float)RequiredRepairs.Count));
        RequiredRepairs.Remove(id);

        if (type == RepairType.Engine)
        {
            engineCompartment.Management.RemoveRepairPopUp(id);
        }
        if (type == RepairType.Worker_Area)
        {
            workerCompartment.Management.RemoveRepairPopUp(id);
        }
        else
        {
            cargoCompartments[0].Management.RemoveRepairPopUp(id);
        }
    }
    public bool CanRepair(string id)
    {
        return HasEnoughCargo(CargoType.Metal, RequiredRepairs[id].MetalCost) && HasEnoughCargo(CargoType.Screw, RequiredRepairs[id].ScrewCost);
    }
    private void AddRepair()
    {
        RepairType type = (RepairType)Random.Range(0, Enum.GetValues(typeof(RepairType)).Length);
        string id = $"{type}_{repairIDGenerationNumber}";
        float timeRequired = Random.Range(5.0f, 10.0f);
        RequiredRepairs.Add(id, new RepairData(id , type, repairIcons[type], Random.Range(repairMetalCostRange.x, repairMetalCostRange.y + 1), Random.Range(repairScrewCostRange.x, repairScrewCostRange.y + 1), timeRequired));
        repairIDGenerationNumber++;

        if (type == RepairType.Engine)
        {
            engineCompartment.Management.AddRepairPopUp(RequiredRepairs[id]);
        }
        else if (type == RepairType.Worker_Area)
        {
            workerCompartment.Management.AddRepairPopUp(RequiredRepairs[id]);
        }
        else
        {
            cargoCompartments[0].Management.AddRepairPopUp(RequiredRepairs[id]);
        }
    }

    public void Damage(int amount)
    {
        currentHullIntegrity -= amount;

        if (currentHullIntegrity < 0)
        {
            currentHullIntegrity = 0;
            if(canExplode)
            {
                StartCoroutine(Dead());
                canExplode = false;
            }
        }

        if(currentHullIntegrity < repairCheckRange)
        {
            AddRepair();
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

    private IEnumerator Dead()
    {
        GameManager.Instance.SetState(GameState.Over);
        for (int i = 0; i < explosionParticleSystems.Count; i++)
        {
            explosionParticleSystems[i].Explode();   
            yield return new WaitForSeconds(timeBetweenExplosion);
        }

        yield return new WaitForSeconds(2.0f);

        GameManager.Instance.GameOver("Your train was destroyed and your enterprise has reached its last station forever.");
    }

    private void CalculateRepairCheckRange()
    {
        repairCheckRange = currentHullIntegrity * 0.75f; //Every 25% decrease in health will add a new repair data
    }

    //Stats
    public void AddPassengerStat(StatType type, int amount)
    {
        if (passengerState.ContainsKey(type))
        {
            passengerState[type] += amount;
            StatPopUpManager.Instance.ShowStatPopUp(transform.position, $"+{amount}<sprite={(int)Icon.Anger}>", StatPopUpColor.Red);
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
        StatPopUpManager.Instance.ShowStatPopUp(transform.position, $"-{amount}<sprite={(int)Icon.Anger}>", StatPopUpColor.Red);
        if (passengerState[type] < 0)
            passengerState[type] = 0;

        UpdatePassengerStatUI();
    }

    //Worker
    public void AddWorker(int amount)
    { 
        workerCompartment.AddWorker(amount);
    }

    //Cargo Usage
    public void AddPassengerDetials(int count, int stations, int fee)
    {
        PassengerTraveDetails details = new PassengerTraveDetails(count, stations, fee);
        passengerDetails.Add(details);
        PlayerDataManager.Instance.AddBullets(details.Count * details.Fee);
        GameManager.Instance.PassengersCarried += count;
        AddCargo(CargoType.Passenger, count, false);
    }
    public bool AddCargo(CargoType type, int amount, bool addMaxPossible)
    {
        if (amount > (type == CargoType.Passenger ? GetPassengerCapacity() : GetResourceCapacity(type)))
        {
            if(addMaxPossible)
            {
                int amountToAdd = type == CargoType.Passenger ? GetPassengerCapacity() : GetResourceCapacity(type);
                StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"+{amountToAdd - cargo[type]}<sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Green);
                cargo[type] = amountToAdd;
            }
            else
            {
                StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"No Space to add <sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Red);StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"No Space to add <sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Red);
                return false;
            }
        }
        {
            if (cargo.ContainsKey(type))
            {
                if (type == CargoType.Passenger)
                {
                    if (cargo[type] + amount > GetPassengerCapacity())
                    {
                        if (addMaxPossible)
                        {
                            int amountToAdd = GetPassengerCapacity();
                            StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"+{amountToAdd - cargo[type]}<sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Green);
                            cargo[type] = amountToAdd;
                        }
                        else
                        {
                            StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"No Space to add <sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Red); StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"No Space to add <sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Red);
                            return false;
                        }
                    }
                    else
                    {
                        cargo[type] += amount;
                        StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"+{amount}<sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Green);
                    }
                }
                if (type == CargoType.Metal || type == CargoType.Screw)
                {
                    if (cargo[type] + amount > GetResourceCapacity(type))
                    {
                        if (addMaxPossible)
                        {
                            int amountToAdd = GetResourceCapacity(type);
                            StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"+{amountToAdd - cargo[type]}<sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Green);
                            cargo[type] = amountToAdd;
                        }
                        else
                        {
                            StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"No Space to add <sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Red); StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"No Space to add <sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Red);
                            return false;
                        }
                    }
                    else
                    {
                        cargo[type] += amount;
                        StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"+{amount}<sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Green);
                    }
                }     
            }
            else
            {
                cargo.Add(type, amount);
                StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"+{amount}<sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Green);
            }
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
        StatPopUpManager.Instance.ShowStatPopUp(engineCompartment.transform.position, $"-{amount}<sprite={(int)(type == CargoType.Passenger ? Icon.Passenger : type == CargoType.Metal ? Icon.Metal : Icon.Screw)}>", StatPopUpColor.Red);
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

    public int GetResourceCapacity(CargoType type)
    {
        int total = cargoSpaceBonus;
        for (int i = 0; i < cargoCompartments.Count; i++)
        {
            total += cargoCompartments[i].Capacity;
        }

        return total - cargo[CargoType.Passenger] - (type == CargoType.Metal ? cargo[CargoType.Screw] : cargo[CargoType.Metal]);
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

        for (int i = 0; i < compartment.TrainStats.ExplosionHolder.childCount; i++)
        {
            explosionParticleSystems.Add(compartment.TrainStats.ExplosionHolder.GetChild(i).GetComponent<TrainExplosionVFX>());
        }
    }
    public void IncreaseWorkerCapacity(int amount)
    {
        workerCompartment.IncreaseWorkerCapacity(amount);
    }
    public void ArmWorkers()
    {
        workerCompartment.ArmWorkers();
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
        PlayerDataManager.Instance.UpdateStatUI(StatType.Anger, passengerState[StatType.Anger], 100);
    }

    //Visuals
    private void GetExplosionParticleSytmes()
    {
        explosionParticleSystems = new List<TrainExplosionVFX>();

        for (int i = 0; i < engineCompartment.TrainStats.ExplosionHolder.childCount; i++)
        {
            explosionParticleSystems.Add(engineCompartment.TrainStats.ExplosionHolder.GetChild(i).GetComponent<TrainExplosionVFX>());
        }

        for (int i = 0; i < workerCompartment.TrainStats.ExplosionHolder.childCount; i++)
        {
            explosionParticleSystems.Add(workerCompartment.TrainStats.ExplosionHolder.GetChild(i).GetComponent<TrainExplosionVFX>());
        }

        for (int i = 0; i < cargoCompartments.Count; i++)
        {
            for (int j = 0; j < cargoCompartments[i].TrainStats.ExplosionHolder.childCount; j++)
            {
                explosionParticleSystems.Add(cargoCompartments[i].TrainStats.ExplosionHolder.GetChild(j).GetComponent<TrainExplosionVFX>());
            }
        }
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
        return new Vector2(Random.Range(bound.min.x, bound.max.x), Random.Range(-2.0f, -1.0f));
    }
}
