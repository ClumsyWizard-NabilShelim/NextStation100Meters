using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainDecal : MonoBehaviour
{
    [SerializeField] private GameObject damagedDecal;
    [SerializeField] private GameObject repairedDecal;
    [SerializeField] private ParticleSystem smokeParticleSystem;

    private void Start()
    {
        damagedDecal.SetActive(false);
        repairedDecal.SetActive(false);
    }

    public void ActivateDamagedDecal()
    {
        smokeParticleSystem.Play();
        damagedDecal.SetActive(true);
        repairedDecal.SetActive(false);
    }

    public void ActivateRepairedDecal()
    {
        smokeParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        damagedDecal.SetActive(false);
        repairedDecal.SetActive(true);
    }
}
