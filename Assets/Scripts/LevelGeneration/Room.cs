using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [field: SerializeField] public int RoomSize { get; private set; }
    [field: SerializeField] public int NextRoomYOffset { get; private set; }

    [SerializeField] private float chanceToSpawnObject;
    [SerializeField] private Transform objectSpawnPointHolder;
    [SerializeField] private List<GameObject> spawnableObjects;

    private void Start()
    {
        if (spawnableObjects.Count == 0)
            return;

        CreateObject();
    }

    private void CreateObject()
    {
        if (objectSpawnPointHolder.childCount == 0)
            return;

        Transform spawnPoint = objectSpawnPointHolder.GetChild(Random.Range(0, objectSpawnPointHolder.childCount));
        spawnPoint.SetParent(null);

        if (Random.Range(0, 101) < chanceToSpawnObject)
            Instantiate(spawnableObjects[Random.Range(0, spawnableObjects.Count)], spawnPoint.position, Quaternion.identity);

        Destroy(spawnPoint.gameObject);
        CreateObject();
    }
}
