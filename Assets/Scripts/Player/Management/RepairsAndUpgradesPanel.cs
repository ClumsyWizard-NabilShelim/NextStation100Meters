using ClumsyWizard.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairsAndUpgradesPanel : ManagementPanel
{
    private CW_AudioPlayer audioPlayer;
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
        audioPlayer = GetComponent<CW_AudioPlayer>();
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

        foreach (RepairData data in PlayerDataManager.Instance.Train.RequiredRepairs.Values)
        {
            Instantiate(repairSlotPrefab, repairSlotHolder).Initialize(data.ID, data.Icon, $"Repair the {data.Type.ToString().Replace("_", " ")} for\n {data.MetalCost} <sprite={(int)Icon.Metal}>  {data.ScrewCost} <sprite={(int)Icon.Screw}>", OnRepair);
        }

        CreateUpgradeSlots();
    }

    private void CreateUpgradeSlots()
    {
        if(upgradeSlotHolder.childCount > 0)
        {
            for(int i = upgradeSlotHolder.childCount - 1; i >= 0; i--)
            {
                Destroy(upgradeSlotHolder.GetChild(i).gameObject);
            }
        }

        upgrades = new Dictionary<string, UpgradeDataSO>();
        upgradeSlots = new Dictionary<string, UpgradeSlot>();

        for (int i = 0; i < upgradeSos.Count; i++)
        {
            if (purchasedUpgrades.Contains(upgradeSos[i].Name))
                continue;

            upgrades.Add(upgradeSos[i].Name, upgradeSos[i]);
            string info = upgradeSos[i].Description + "\n";

            if (upgradeSos[i].BulletCost > 0)
                info += $"{upgradeSos[i].BulletCost} <sprite={(int)Icon.Bullet}> ";

            if (upgradeSos[i].BulletCost > 0)
                info += $" {upgradeSos[i].MetalCost} <sprite={(int)Icon.Metal}> ";

            if (upgradeSos[i].BulletCost > 0)
                info += $" {upgradeSos[i].ScrewCost} <sprite={(int)Icon.Screw}> ";

            UpgradeSlot slot = Instantiate(upgradeSlotPrefab, upgradeSlotHolder);
            bool canBuy = PlayerDataManager.Instance.HasEnougBullets(upgradeSos[i].BulletCost) && PlayerDataManager.Instance.Train.HasEnoughCargo(CargoType.Metal, upgradeSos[i].MetalCost) && PlayerDataManager.Instance.Train.HasEnoughCargo(CargoType.Screw, upgradeSos[i].ScrewCost);
            slot.Initialize(upgradeSos[i].Name, upgradeSos[i].Icon, upgradeSos[i].Name, info, canBuy, OnBuyUpgrade);
            upgradeSlots.Add(upgradeSos[i].Name, slot);
        }
    }

    private void OnRepair(string id)
    {
        RepairData data = PlayerDataManager.Instance.Train.RequiredRepairs[id];

        if (PlayerDataManager.Instance.Train.HasEnoughCargo(CargoType.Metal, data.MetalCost) && PlayerDataManager.Instance.Train.HasEnoughCargo(CargoType.Screw, data.ScrewCost))
        {
            audioPlayer.Play("Build");
            PlayerDataManager.Instance.Train.RepairIssueAndChargeResource(id);

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
            audioPlayer.Play("Coin");

            foreach (UpgradeType type in data.Upgrades.Keys)
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
                else if (type == UpgradeType.WorkerCapacity)
                {
                    PlayerDataManager.Instance.Train.IncreaseWorkerCapacity(data.Upgrades[type]);
                }
                else if (type == UpgradeType.ArmWorkers)
                {
                    PlayerDataManager.Instance.Train.ArmWorkers();
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
