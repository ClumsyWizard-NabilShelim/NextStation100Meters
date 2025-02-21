using ClumsyWizard.Core;
using ClumsyWizard.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    HullIntegrity,
    Infection,
    Anger
}

public enum WorkerType
{
    Normal,
    Armed
}

public enum CargoType
{
    Metal,
    Screw,
    Passenger
}

public class PlayerDataManager : CW_Singleton<PlayerDataManager>
{
    public Train Train { get; private set; }

    [Header("Resources")]
    [SerializeField] private int startingBullets;
    private int currentBullets;

    [Header("Cargo")]
    [SerializeField] private int startingMetals;
    [SerializeField] private int startingScrews;

    [Header("Workers")]
    [SerializeField] private int startingNormalWorkers;
    [SerializeField] private int startingArmedWorkers;

    [Header("UI")]
    [SerializeField] private StatAmount bulletUI;
    [SerializeField] private CW_Dictionary<StatType, StatBar> statBarUI;
    [SerializeField] private CW_Dictionary<WorkerType, StatAmount> workerAmountUI;
    [SerializeField] private CW_Dictionary<CargoType, StatAmount> cargoAmountUI;


    private void Start()
    {
        Train = GameObject.FindGameObjectWithTag("Train").GetComponent<Train>();
        Train.Initialize();

        currentBullets = startingBullets;
        UpdateBulletUI();

        Train.AddWorker(WorkerType.Normal, startingNormalWorkers);
        Train.AddWorker(WorkerType.Armed, startingArmedWorkers);

        Train.AddCargo(CargoType.Metal, startingMetals);
        Train.AddCargo(CargoType.Screw, startingScrews);
        Train.AddCargo(CargoType.Passenger, 0);
    }

    //Resource Management
    public void AddBullets(int amount)
    {
        currentBullets += amount;
        GameManager.Instance.BulletsEarned += amount;
        UpdateBulletUI();
    }
    public bool UseBullets(int amount)
    {
        if (currentBullets - amount < 0)
            return false;

        GameManager.Instance.BulletsSpent += amount;
        currentBullets -= amount;
        UpdateBulletUI();

        return true;
    }
    public bool HasEnougBullets(int amount)
    {
        return currentBullets - amount >= 0;
    }

    //UI
    public void UpdateBulletUI()
    {
        bulletUI.SetData(currentBullets.ToString());
    }
    public void UpdateStatUI(StatType type, int currentAmount, int maxAmount)
    {
        statBarUI[type].SetBarData(currentAmount / (float)maxAmount, $"{currentAmount}/{maxAmount}");
    }
    public void UpdateWorkerUI(WorkerType type, int currentAmount, int maxAmount)
    {
        workerAmountUI[type].SetData($"{currentAmount}/{maxAmount}");
    }
    public void UpdateCargoUI()
    {
        cargoAmountUI[CargoType.Screw].SetData($"{Train.GetCargoCount(CargoType.Screw)}/{Train.GetResourceCapacity()}");
        cargoAmountUI[CargoType.Metal].SetData($"{Train.GetCargoCount(CargoType.Metal)}/{Train.GetResourceCapacity()}");
        cargoAmountUI[CargoType.Passenger].SetData($"{Train.GetCargoCount(CargoType.Passenger)}/{Train.GetPassengerCapacity()}");
    }
}
