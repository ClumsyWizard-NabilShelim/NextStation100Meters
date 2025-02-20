using ClumsyWizard.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerCompartment : TrainCompartment
{
    private Dictionary<WorkerType, int> workers = new Dictionary<WorkerType, int>();
    [SerializeField] private CW_Dictionary<WorkerType, int> workerCapacity;

    public bool AddWorker(WorkerType type, int amount)
    {
        if (amount > workerCapacity[type])
            return false;

        if (workers.ContainsKey(type))
        {
            if (workers[type] + amount > workerCapacity[type])
                return false;

            workers[type] += amount;
        }
        else
        {
            workers.Add(type, amount);
        }

        PlayerDataManager.Instance.UpdateWorkerUI(type, workers[type], workerCapacity[type]);
        return true;
    }
    public void RemoveWorker(WorkerType type, int amount)
    {
        if (!workers.ContainsKey(type))
            return;

        workers[type] -= amount;

        if (workers[type] <= 0)
            workers[type] = 0;

        PlayerDataManager.Instance.UpdateWorkerUI(type, workers[type], workerCapacity[type]);
    }

    public void IncreaseWorkerCapacity(WorkerType type, int amount)
    {
        workerCapacity[type] += amount;
        workers[type] += amount;
        PlayerDataManager.Instance.UpdateWorkerUI(type, workers[type], workerCapacity[type]);
    }
}