using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Track")]
    [SerializeField] private Transform trackHolder;
    [SerializeField] private float trackSize;
    [SerializeField] private float distanceBeforeReorder;

    private Transform lastTrack;

    [Header("World Props")]
    [SerializeField] private GameObject station;
    [SerializeField] private Transform stationSpawnPoint;
    [SerializeField] private Vector2 timeBetweenStation;
    private float currentTime;

    [Header("World Props")]
    [SerializeField] private GameObject[] props;
    [SerializeField] private Transform spawnPointHolder;
    [SerializeField] private float propSpawnChance;
    [SerializeField] private float timeBetweenSpawn;
    private List<Vector3> originalSpawnPoints = new List<Vector3>();
    private List<Vector3> currentSpawnPoints = new List<Vector3>();

    private void Start()
    {
        lastTrack = trackHolder.GetChild(0);

        for (int i = 0; i < spawnPointHolder.childCount; i++)
        {
            originalSpawnPoints.Add(spawnPointHolder.GetChild(i).position);
        }

        SpawnNewProps();
        SpawnNewStation();
    }

    private void Update()
    {
        if(lastTrack.localPosition.x < distanceBeforeReorder)
        {
            lastTrack.localPosition = trackHolder.GetChild(2).localPosition + Vector3.right * trackSize;
            lastTrack.SetAsLastSibling();
            lastTrack = trackHolder.GetChild(0);
        }
    }

    //Spawn Station
    private void SpawnNewStation()
    {
        currentTime = Random.Range(timeBetweenStation.x, timeBetweenStation.y);
        Invoke("CreateStation", currentTime);
    }

    private void CreateStation()
    {
        CancelInvoke("SpawnNewProps");
        Instantiate(station, stationSpawnPoint.position, Quaternion.identity);
    }

    //Spawn Props
    private void CreateProps()
    {
        if (currentSpawnPoints.Count == 0)
        {
            Invoke("SpawnNewProps", timeBetweenSpawn);
            return;
        }

        if (Random.Range(0, 101) <= propSpawnChance)
        {
            GameObject prop =  Instantiate(props[Random.Range(0, props.Length)], currentSpawnPoints[0], Quaternion.identity);
            Destroy(prop, 15.0f);
        }

        currentSpawnPoints.RemoveAt(0);
        CreateProps();
    }

    private void SpawnNewProps()
    {
        currentSpawnPoints = originalSpawnPoints.ToList();
        CreateProps();
    }
}
