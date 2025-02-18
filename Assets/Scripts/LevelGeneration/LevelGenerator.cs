using ClumsyWizard.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : CW_Singleton<LevelGenerator>
{
    [SerializeField] private List<Room> rooms;
    [SerializeField] private int chunkSize;
    private Vector2 nextRoomSpawnPosition;

    [Header("New Chunk Genertion")]
    [SerializeField] private Vector2 checkArea;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float triggerCheckOffset;
    private Collider2D playerCollider;
    private Vector2 checkPosition;

    protected override void Awake()
    {
        base.Awake();
        checkPosition = new Vector2(-1000, -1000);
    }

    public void GenerateRooms()
    {
        for (int i = 0; i < chunkSize; i++)
        {
            Room room = rooms[Random.Range(0, rooms.Count)];
            Instantiate(room, nextRoomSpawnPosition, Quaternion.identity).transform.SetParent(transform);

            nextRoomSpawnPosition = new Vector2(nextRoomSpawnPosition.x + room.RoomSize, nextRoomSpawnPosition.y + room.NextRoomYOffset);

            if (i == (int)(chunkSize * triggerCheckOffset))
                checkPosition = nextRoomSpawnPosition;
        }
    }

    private void Update()
    {
        //Collider2D col = Physics2D.OverlapBox(checkPosition, checkArea, 0.0f, playerLayer);

        //if (playerCollider == null)
        //{
        //    if(col != null)
        //    {
        //        playerCollider = col;
        //        GenerateRooms();
        //    }
        //}
        //else
        //{
        //    if(col == null)
        //    {
        //        playerCollider = col;
        //    }
        //}
    }

    //Debug
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(checkPosition, checkArea);
    }
}
