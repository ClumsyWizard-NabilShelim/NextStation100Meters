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

public class GameManager : CW_Singleton<GameManager>
{
    [SerializeField] private float worldSpeed;
    public float CurrentWorldSpeed { get; private set; }
    public GameState State { get; private set; }
    public Action<GameState> OnStateChange { get; set; }

    [SerializeField] private float slowDownRate;
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
        if(State == GameState.Station)
        {
            if(currentTime <= slowDownRate)
            {
                CurrentWorldSpeed = Mathf.Lerp(worldSpeed, 0.0f, currentTime / slowDownRate);
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
            slowDownRate = (2 * StationManager.Instance.CurrentStation.transform.position.x) / worldSpeed;
            currentTime = 0.0f;
        }

        OnStateChange?.Invoke(state);
    }
}
