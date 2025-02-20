using ClumsyWizard.Utilities;
using System.Collections;
using UnityEngine;

public enum UpgradeType
{
    HullIntegrity,
    NewCargoContainer,
    CargoCapacity,
    NormalWorkerCapacity,
    ArmedWorkerCapacity,
    Damage
}

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade Data")]
public class UpgradeDataSO : ScriptableObject
{
    [field:SerializeField] public Sprite Icon { get; private set; }
    [field:SerializeField] public string Name { get; private set; }
    [field:SerializeField, TextArea(5, 5)] public string Description { get; private set; }

    [field: Header("Cost")]
    [field:SerializeField] public int BulletCost { get; private set; }
    [field:SerializeField] public int MetalCost { get; private set; }
    [field:SerializeField] public int ScrewCost { get; private set; }

    [field: Header("Stat")]
    [field: SerializeField] public CW_Dictionary<UpgradeType, int> Upgrades { get; private set; }
}