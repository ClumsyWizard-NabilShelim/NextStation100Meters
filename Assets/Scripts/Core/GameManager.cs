using ClumsyWizard.Core;
using System;
using UnityEngine;

public enum GameState
{
    Initialize,
    Travelling,
    UnderAttack,
    Station
}

public enum Icon
{
    Infection,
    Anger,
    ArmedWorker,
    Bullet,
    Metal,
    NormalWorker,
    Passenger,
    Screw,
    Armor
}

public class GameManager : CW_Singleton<GameManager>
{
    [SerializeField] private float worldSpeed;
    public float CurrentWorldSpeed { get; private set; }
    public GameState State { get; private set; }
    public Action<GameState> OnStateChange { get; set; }

    private float changeRate;
    private float currentTime;
    private Transform train;

    private void Start()
    {
        CurrentWorldSpeed = worldSpeed;
        train = GameObject.FindGameObjectWithTag("Train").transform;
        SetState(GameState.Travelling);
    }

    private void Update()
    {
        if (State == GameState.Station)
        {
            if (currentTime <= changeRate)
            {
                CurrentWorldSpeed = Mathf.Lerp(worldSpeed, 0.0f, currentTime / changeRate);
                currentTime += Time.deltaTime;

                if (currentTime > changeRate)
                    PlayerManagementMenu.Instance.OpenMenu();
            }
        }
        else if (State == GameState.Travelling)
        {
            if (currentTime <= changeRate)
            {
                CurrentWorldSpeed = Mathf.Lerp(0.0f, worldSpeed, currentTime / changeRate);
                currentTime += Time.deltaTime;
            }
        }
    }

    public void SetState(GameState state)
    {
        if (State == state)
            return;

        State = state;

        if (state == GameState.Station)
        {
            changeRate = (2 * StationManager.Instance.CurrentStation.transform.position.x) / worldSpeed;
            currentTime = 0.0f;
        }
        else if (state == GameState.Travelling)
        {
            changeRate = 3.0f; // Arbitrary number
            currentTime = 0.0f;
        }

        OnStateChange?.Invoke(state);
    }
}
