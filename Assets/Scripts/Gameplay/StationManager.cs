using ClumsyWizard.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationManager : CW_Singleton<StationManager>
{
    [SerializeField] private Station station;
    [SerializeField] private Transform stationSpawnPoint;
    [SerializeField] private Vector2 timeBetweenStation;
    public Station CurrentStation { get; private set; }
    private float currentTime;

    private void Start()
    {
        GameManager.Instance.OnStateChange += OnGameStateChange;
    }

    private void OnGameStateChange(GameState state)
    {
        if (state == GameState.Travelling)
        {
            SpawnNewStation();
        }
    }

    //Spawn Station
    public void SpawnNewStation()
    {
        currentTime = Random.Range(timeBetweenStation.x, timeBetweenStation.y);
        Invoke("CreateStation", currentTime);
    }

    private void CreateStation()
    {
        CurrentStation = Instantiate(station, stationSpawnPoint.position, Quaternion.identity);
        GameManager.Instance.SetState(GameState.Station);
    }
}
