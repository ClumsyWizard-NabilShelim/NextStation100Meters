using ClumsyWizard.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGenerator : CW_Singleton<WorldGenerator>
{
    [Header("Track")]
    [SerializeField] private Transform trackHolder;
    [SerializeField] private float trackSize;
    [SerializeField] private float distanceBeforeReorder;

    private Transform lastTrack;

    [Header("Crate Spawn")]
    [SerializeField] private float crateSpawnRate;
    [SerializeField] private GameObject cratePrefab;

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

        GameManager.Instance.OnStateChange += OnGameStateChange;

        SpawnNewProps();
    }

    private void OnGameStateChange(GameState state)
    {
        if (state == GameState.Station)
        {
            CancelInvoke("SpawnNewProps");
        }
        else if (state == GameState.Travelling || state == GameState.UnderAttack)
        {
            CancelInvoke("SpawnNewProps");
            Invoke("SpawnNewProps", timeBetweenSpawn);
        }
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
            if(Random.Range(0,  101) <= crateSpawnRate)
                Instantiate(cratePrefab, currentSpawnPoints[0], Quaternion.identity);
            else
                Instantiate(props[Random.Range(0, props.Length)], currentSpawnPoints[0], Quaternion.identity);
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
