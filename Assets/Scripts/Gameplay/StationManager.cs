using ClumsyWizard.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationManager : CW_Singleton<StationManager>
{
    [SerializeField] private Station station;
    [SerializeField] private Transform stationSpawnPoint;
    public Station CurrentStation { get; private set; }
    private float currentTime;

    //Spawn Station
    public void SpawnNewStation()
    {
        GameManager.Instance.StationsReached++;
        CurrentStation = Instantiate(station, stationSpawnPoint.position, Quaternion.identity);
        CurrentStation.Initialize();
    }
}
