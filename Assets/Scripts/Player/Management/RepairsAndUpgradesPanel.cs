using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairsAndUpgradesPanel : ManagementPanel
{
    [Header("Repairs")]
    [SerializeField] private RepairSlot repairSlotPrefab;
    [SerializeField] private Transform repairSlotHolder;

    [Header("Upgrades")]
    [SerializeField] private UpgradeSlot upgradeSlotPrefab;
    [SerializeField] private List<UpgradeDataSO> upgradeSos;
    [SerializeField] private Transform upgradeSlotHolder;
    private Dictionary<string, UpgradeSlot> upgradeSlots = new Dictionary<string, UpgradeSlot>();
    private Dictionary<string, UpgradeDataSO> upgrades = new Dictionary<string, UpgradeDataSO>();
    private List<string> purchasedUpgrades = new List<string>();

    private void Start()
    {
        for (int i = 0; i < upgradeSos.Count; i++)
        {
            upgrades.Add(upgradeSos[i].Name, upgradeSos[i]);
            string info = upgradeSos[i].Description + "\n";

            if (upgradeSos[i].BulletCost > 0)
                info += $"{upgradeSos[i].BulletCost} <sprite={(int)Icon.Bullet}> ";

            if (upgradeSos[i].BulletCost > 0)
                info += $" {upgradeSos[i].MetalCost} <sprite={(int)Icon.Metal}> ";

            if (upgradeSos[i].BulletCost > 0)
                info += $" {upgradeSos[i].ScrewCost} <sprite={(int)Icon.Screw}> ";

            UpgradeSlot slot = Instantiate(upgradeSlotPrefab, upgradeSlotHolder);
            slot.Initialize(upgradeSos[i].Name, upgradeSos[i].Icon, upgradeSos[i].Name, info, true, OnBuyUpgrade);
            upgradeSlots.Add(upgradeSos[i].Name, slot);
        }
    }

    public override void Open()
    {
        base.Open();

        foreach (string id in upgradeSlots.Keys)
        {
            if (purchasedUpgrades.Contains(id))
                upgradeSlots[id].SetAvailable(false);
        }

        //Repair ui
        for (int i = repairSlotHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(repairSlotHolder.GetChild(i).gameObject);
        }

        for(int i = 0; i < PlayerDataManager.Instance.Train.RequiredRepairs.Count; i++)
        {
            RepairData data = PlayerDataManager.Instance.Train.RequiredRepairs[i];
            Instantiate(repairSlotPrefab, repairSlotHolder).Initialize(i, data.Icon, $"Repair the {data.Name} for\n {data.MetalCost} <sprite={(int)Icon.Metal}>  {data.ScrewCost} <sprite={(int)Icon.Screw}>", OnRepair);
        }
    }

    private void OnRepair(int index)
    {
        RepairData data = PlayerDataManager.Instance.Train.RequiredRepairs[index];

        if (PlayerDataManager.Instance.Train.HasEnoughCargo(CargoType.Metal, data.MetalCost) && PlayerDataManager.Instance.Train.HasEnoughCargo(CargoType.Screw, data.ScrewCost))
        {
            PlayerDataManager.Instance.Train.RemoveCargo(CargoType.Metal, data.MetalCost);
            PlayerDataManager.Instance.Train.RemoveCargo(CargoType.Screw, data.ScrewCost);

            PlayerDataManager.Instance.Train.RepairIssue(index);

            Open();
        }
    }

    private void OnBuyUpgrade(string upgradeID)
    {
        UpgradeDataSO data = upgrades[upgradeID];
        
        if(PlayerDataManager.Instance.HasEnougBullets(data.BulletCost) &&
            PlayerDataManager.Instance.Train.HasEnoughCargo(CargoType.Metal, data.MetalCost) && 
            PlayerDataManager.Instance.Train.HasEnoughCargo(CargoType.Screw, data.ScrewCost))
        {
            PlayerDataManager.Instance.UseBullets(data.BulletCost);
            PlayerDataManager.Instance.Train.RemoveCargo(CargoType.Metal, data.MetalCost);
            PlayerDataManager.Instance.Train.RemoveCargo(CargoType.Screw, data.ScrewCost);

            purchasedUpgrades.Add(upgradeID);

            foreach(UpgradeType type in data.Upgrades.Keys)
            {
                if(type == UpgradeType.HullIntegrity)
                {
                    PlayerDataManager.Instance.Train.IncreaseMaxhullIntegrity(data.Upgrades[type]);
                }
                else if (type == UpgradeType.NewCargoContainer)
                {
                    PlayerDataManager.Instance.Train.AddCargoCompartment();
                }
                else if (type == UpgradeType.CargoCapacity)
                {
                    PlayerDataManager.Instance.Train.IncreaseCargoSpace(data.Upgrades[type]);
                }
                else if (type == UpgradeType.NormalWorkerCapacity)
                {
                    PlayerDataManager.Instance.Train.IncreaseWorkerCapacity(WorkerType.Normal, data.Upgrades[type]);
                }
                else if (type == UpgradeType.ArmedWorkerCapacity)
                {
                    PlayerDataManager.Instance.Train.IncreaseWorkerCapacity(WorkerType.Armed, data.Upgrades[type]);
                }
                else if (type == UpgradeType.Damage)
                {
                    PlayerDataManager.Instance.Train.IncreaseDamage(data.Upgrades[type]);
                }
            }
        }

        Open();
    }
}
