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
    private Train train;

    [Header("Resources")]
    [SerializeField] private int startingBullets;
    private int currentBullets;

    [Header("Cargo")]
    [SerializeField] private int startingMetals;
    [SerializeField] private int startingScrews;
    [SerializeField] private int startingPassengers;

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
        train = GameObject.FindGameObjectWithTag("Train").GetComponent<Train>();
        train.Initialize();

        currentBullets = startingBullets;
        UpdateBulletUI();

        train.WorkerCompartment.AddWorker(WorkerType.Normal, startingNormalWorkers);
        train.WorkerCompartment.AddWorker(WorkerType.Armed, startingArmedWorkers);

        train.AddCargo(CargoType.Metal, startingMetals);
        train.AddCargo(CargoType.Screw, startingScrews);
        train.AddCargo(CargoType.Passenger, startingPassengers);
    }

    //Resource Management
    public void AddBullets(int amount)
    {
        currentBullets += amount;
        UpdateBulletUI();
    }
    public bool UseBullets(int amount)
    {
        if (currentBullets - amount < 0)
            return false;

        currentBullets -= amount;
        UpdateBulletUI();

        return true;
    }

    //UI
    public void UpdateBulletUI()
    {
        bulletUI.SetData(currentBullets.ToString());
    }
    public void UpdateStatUI(StatType type, int currentAmount, int maxAmount)
    {
        statBarUI[type].SetBarData(currentAmount / (float)maxAmount, $"{currentAmount} / {maxAmount}");
    }
    public void UpdateWorkerUI(WorkerType type, int currentAmount, int maxAmount)
    {
        workerAmountUI[type].SetData($"{currentAmount} / {maxAmount}");
    }
    public void UpdateCargoUI()
    {
        cargoAmountUI[CargoType.Screw].SetData($"{train.GetCargoCount(CargoType.Screw)}/{train.GetResourceCapacity()}");
        cargoAmountUI[CargoType.Metal].SetData($"{train.GetCargoCount(CargoType.Metal)}/{train.GetResourceCapacity()}");
        cargoAmountUI[CargoType.Passenger].SetData($"{train.GetCargoCount(CargoType.Passenger)}/{train.GetPassengerCapacity()}");
    }
}
