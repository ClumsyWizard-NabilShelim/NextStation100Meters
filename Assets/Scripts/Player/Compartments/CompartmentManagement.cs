using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RepairingData
{
    public string ID;
    public ManagementPopUp PopUp;
    public float TargetTime;
    public float CurrentTime;
    public Worker AssignedWorker;

    public RepairingData(string id, ManagementPopUp popUp, float targetTime, Worker worker)
    {
        ID = id;
        PopUp = popUp;
        TargetTime = targetTime;
        CurrentTime = 0.0f;
        AssignedWorker = worker;
    }
}

public class CompartmentManagement : MonoBehaviour
{
    private Train train;
    [SerializeField] private Transform popUpContainer;
    [SerializeField] private ManagementPopUp popUpPrefab;
    [SerializeField] protected Sprite repairIcon;

    private Dictionary<string, ManagementPopUp> repairPopUps = new Dictionary<string, ManagementPopUp>();
    private List<RepairingData> repairingData = new List<RepairingData>();

    public void Initialize(Train train)
    {
        this.train = train;
    }

    public void AddRepairPopUp(RepairData data)
    {
        string info = $"{data.MetalCost}<sprite={(int)Icon.Metal}> {data.ScrewCost}<sprite={(int)Icon.Screw}>";
        repairPopUps.Add(data.ID, CreatePopUp(data.ID, data.TimeRequired, "Cost", info, repairIcon, StartRepair, CancelRepair));
    }
    public void RemoveRepairPopUp(string id)
    {
        if (repairPopUps.ContainsKey(id))
        {
            Destroy(repairPopUps[id].gameObject);
            repairPopUps.Remove(id);
        }
    }

    private void Update()
    {
        if (repairingData.Count > 0)
        {
            for (int i = repairingData.Count - 1; i >= 0; i--)
            {
                RepairingData data = repairingData[i];
                data.CurrentTime += Time.deltaTime * data.AssignedWorker.WorkSpeed;
                if (data.CurrentTime >= data.TargetTime)
                {
                    data.AssignedWorker.IsBusy = false;
                    train.RepairIssueAndChargeResource(data.ID);
                    repairingData.RemoveAt(i);
                }
                data.PopUp.UpdateFill(data.CurrentTime / data.TargetTime);
            }
        } 
    }

    private void StartRepair(string id, float requiredTime)
    {
        Worker worker = train.GetWorker();
        if (worker != null)
        {
            if (train.CanRepair(id))
            {
                worker.IsBusy = true;
                repairingData.Add(new RepairingData(id, repairPopUps[id], requiredTime, worker));
            }
            else
            {
                repairPopUps[id].ToolTip.SetInfo("Unable", "No enough resources for this action.");
            }
        }
        else
        {
            repairPopUps[id].ToolTip.SetInfo("Unable", "No workers are free. Cancel some other action or wait for it to be over.");
        }
    }

    private void CancelRepair(string id)
    {
        for (int i = 0; i < repairingData.Count; i++)
        {
            if (repairingData[i].ID == id)
            {
                repairingData[i].PopUp.UpdateFill(0.0f);
                repairingData.RemoveAt(i);
                break;
            }
        }
    }

    private ManagementPopUp CreatePopUp(string id, float requiredTime, string label, string info, Sprite icon, Action<string, float> startCallback, Action<string> cancelCallback)
    {
        ManagementPopUp popUp = Instantiate(popUpPrefab, popUpContainer);
        popUp.Initialize(id, requiredTime, label, info, icon, startCallback, cancelCallback);
        return popUp;
    }
}
