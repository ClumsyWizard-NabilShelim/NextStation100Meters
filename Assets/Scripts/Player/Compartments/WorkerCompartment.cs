using ClumsyWizard.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Worker
{
    public float WorkSpeed;
    public float KillSpeed;
    public bool IsBusy;

    public Worker(float workSpeed, float killSpeed, bool isBusy)
    {
        WorkSpeed = workSpeed;
        KillSpeed = killSpeed;
        IsBusy = isBusy;
    }
}

public class WorkerCompartment : TrainCompartment
{
    [SerializeField] private Worker baseNormalWorkerData;
    [SerializeField] private Worker baseArmedWorkerData;

    private bool isArmed;

    private List<Worker> workers = new List<Worker>();

    public bool AddWorker(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (isArmed)
                workers.Add(new Worker(baseArmedWorkerData.WorkSpeed, baseArmedWorkerData.KillSpeed, false));
            else
                workers.Add(new Worker(baseNormalWorkerData.WorkSpeed, baseNormalWorkerData.KillSpeed, false));
        }

        PlayerDataManager.Instance.UpdateWorkerUI(workers.Count);
        return true;
    }

    public Worker GetWorker()
    {
        for (int i = 0; i < workers.Count; i++)
        {
            if (!workers[i].IsBusy)
                return workers[i];
        }

        return null;
    }

    public void IncreaseWorkerCapacity(int amount)
    {
        AddWorker(amount);
        PlayerDataManager.Instance.UpdateWorkerUI(workers.Count);
    }
    public void ArmWorkers()
    {
        int currentAmount = workers.Count;
        workers.Clear();
        isArmed = true;
        AddWorker(currentAmount);
    }
}