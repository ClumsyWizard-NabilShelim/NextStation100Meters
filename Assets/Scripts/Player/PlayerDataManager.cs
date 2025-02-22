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

public enum CargoType
{
    Metal,
    Screw,
    Passenger
}

public class PlayerDataManager : CW_Singleton<PlayerDataManager>
{
    [SerializeField] private Animator animator;
    public Train Train { get; private set; }

    [Header("Resources")]
    [SerializeField] private int startingBullets;
    private int currentBullets;

    [Header("Cargo")]
    [SerializeField] private int startingMetals;
    [SerializeField] private int startingScrews;

    [Header("Workers")]
    [SerializeField] private int startingNormalWorkers;

    [Header("UI")]
    [SerializeField] private StatAmount bulletUI;
    [SerializeField] private StatAmount workerAmountUI;
    [SerializeField] private CW_Dictionary<StatType, StatBar> statBarUI;
    [SerializeField] private CW_Dictionary<CargoType, StatAmount> cargoAmountUI;


    private void Start()
    {
        Train = GameObject.FindGameObjectWithTag("Train").GetComponent<Train>();
        Train.Initialize();

        currentBullets = startingBullets;
        UpdateBulletUI();

        Train.AddWorker(startingNormalWorkers);

        Train.AddCargo(CargoType.Metal, startingMetals, false);
        Train.AddCargo(CargoType.Screw, startingScrews, false);
        Train.AddCargo(CargoType.Passenger, 0, false);

        GameManager.Instance.OnStateChange += (GameState state) =>
        {
            if (state == GameState.Travelling)
                animator.SetTrigger("Show");
            else if(state == GameState.Over)
                animator.SetTrigger("Hide");
        };
    }

    //Resource Management
    public void AddBullets(int amount)
    {
        currentBullets += amount;
        StatPopUpManager.Instance.ShowStatPopUp(Train.transform.position, $"+{amount} <sprite={(int)Icon.Bullet}>", StatPopUpColor.Green);
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
    public void UpdateWorkerUI(int currentAmount)
    {
        workerAmountUI.SetData($"{currentAmount}");
    }
    public void UpdateCargoUI()
    {
        cargoAmountUI[CargoType.Screw].SetData($"{Train.GetCargoCount(CargoType.Screw)}/{Train.GetResourceCapacity(CargoType.Screw)}");
        cargoAmountUI[CargoType.Metal].SetData($"{Train.GetCargoCount(CargoType.Metal)}/{Train.GetResourceCapacity(CargoType.Metal)}");
        cargoAmountUI[CargoType.Passenger].SetData($"{Train.GetCargoCount(CargoType.Passenger)}/{Train.GetPassengerCapacity()}");
    }
}
