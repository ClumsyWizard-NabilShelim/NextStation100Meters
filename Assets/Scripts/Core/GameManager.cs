using ClumsyWizard.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Uninitialized,
    Start,
    Ongoing,
    Over
}

public class GameManager : CW_Singleton<GameManager>
{
    private GameState state;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private float killzoneY;
    private Transform player;

    private void Start()
    {
        SetState(GameState.Start);
    }

    private void Update()
    {
        if (player == null)
            return;

        if(player.position.y < killzoneY)
        {
            SetState(GameState.Over);
        }
    }

    //State change
    public void SetState(GameState state)
    {
        if (this.state == state)
            return;

        this.state = state;

        if(state == GameState.Start)
        {
            LevelGenerator.Instance.GenerateRooms();
            player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity).transform;
            CameraManager.Instance.Initialize(player);
            SetState(GameState.Ongoing);
        }

        if (state == GameState.Over)
        {
            CW_SceneManagement.Instance.Reload();
        }
    }
}
