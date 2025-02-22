using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainDecalManager : MonoBehaviour
{
    [SerializeField] private Transform decalHolder;
    private List<TrainDecal> decals = new List<TrainDecal>();
    private List<TrainDecal> activeDecals = new List<TrainDecal>();

    private void Start()
    {
        for (int i = 0; i < decalHolder.childCount; i++)
        {
            decals.Add(decalHolder.GetChild(i).GetComponent<TrainDecal>());
        }
    }

    public void ActivateDamagedDecal()
    {
        if (decals.Count == 0)
            return;

        int index = Random.Range(0, decals.Count);
        activeDecals.Add(decals[index]);
        decals[index].ActivateDamagedDecal();
        decals.RemoveAt(index);
    }
    public void ActivateRepairedDecal()
    {
        if (activeDecals.Count == 0)
            return;

        int index = Random.Range(0, activeDecals.Count);
        activeDecals[index].ActivateRepairedDecal();
        activeDecals.RemoveAt(index);
    }
}
