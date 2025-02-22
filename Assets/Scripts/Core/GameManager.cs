using ClumsyWizard.Core;
using ClumsyWizard.UI;
using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameState
{
    Init,
    MainMenu,
    Travelling,
    UnderAttack,
    Station,
    Over
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
    public float CurrentWorldSpeed { get; private set; }
    public GameState State { get; private set; }
    public Action<GameState> OnStateChange { get; set; }

    private float changeRate;
    private float currentTime;
    private Transform train;

    [Header("GamePlay")]
    [SerializeField] private float worldSpeed;
    [SerializeField] private Vector2Int travelDurationRange;
    [SerializeField] private Vector2Int timeBetweenCombatDurationRange;
    private float currentTravelTime;
    private float currentCombatDelayTime;

    [Header("UI")]
    [SerializeField] private Animator gameOverAnimator;
    [SerializeField] private TextMeshProUGUI gameOverReasonText;
    [SerializeField] private TextMeshProUGUI statValuesText;
    [SerializeField] private CW_Button retryButton;
    [SerializeField] private CW_Button mainMenuButton;

    //Stats record
    public int StationsReached { get; set;}
    public int PassengersCarried { get; set;}
    public int PassengersKilled { get; set;}
    public int PassengersInfected { get; set;}
    public int BulletsEarned { get; set; }
    public int BulletsSpent { get; set; }


    private void Start()
    {
        CurrentWorldSpeed = worldSpeed;
        train = GameObject.FindGameObjectWithTag("Train").transform;
        SetState(GameState.MainMenu);

        retryButton.SetClickEvent(() =>
        {
            CW_SceneManagement.Instance.Reload();
        });
        mainMenuButton.SetClickEvent(() =>
        {
            CW_SceneManagement.Instance.Load("Level 1");
        });
    }

    private void Update()
    {
        if (State == GameState.Over)
            return;

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

        if (State == GameState.Travelling)
        {
            if (currentTime <= changeRate)
            {
                CurrentWorldSpeed = Mathf.Lerp(0.0f, worldSpeed, currentTime / changeRate);
                currentTime += Time.deltaTime;
            }

            if (currentCombatDelayTime <= 0.0f)
            {
                EnemyManager.Instance.StartWave();
            }
            else
            {
                currentCombatDelayTime -= Time.deltaTime;
            }
        }

        if(State == GameState.Travelling || State == GameState.UnderAttack)
        {
            if (currentTravelTime <= 0.0f)
            {
                if (State != GameState.UnderAttack)
                {
                    SetState(GameState.Station);
                    return;
                }
            }
            else
            {
                currentTravelTime -= Time.deltaTime;
            }
        }
    }

    public void SetState(GameState state)
    {
        if (State == state || State == GameState.Over)
            return;

        GameState previousState = State;
        State = state;

        if(state == GameState.MainMenu)
        {
            CurrentWorldSpeed = 0.0f;
        }
        else if (state == GameState.Station)
        {
            StationManager.Instance.SpawnNewStation();
            changeRate = (2 * StationManager.Instance.CurrentStation.transform.position.x) / worldSpeed;
            currentTime = 0.0f;
        }
        else if (state == GameState.Travelling)
        {

            if(previousState != GameState.UnderAttack)
            {
                changeRate = 3.0f; // Arbitrary number
                currentTime = 0.0f;
                currentTravelTime = Random.Range(travelDurationRange.x, travelDurationRange.y);
            }

            currentCombatDelayTime = Random.Range(timeBetweenCombatDurationRange.x, timeBetweenCombatDurationRange.y);
        }
        else if(state == GameState.Over)
        {
            CurrentWorldSpeed = 0.0f;
        }

        OnStateChange?.Invoke(state);
    }

    public void GameOver(string reason)
    {
        gameOverAnimator.SetBool("Show", true);
        gameOverReasonText.text = reason;
        statValuesText.text = $"{StationsReached}\n\n{PassengersCarried}\n{PassengersKilled}\n\n{BulletsEarned}\n{BulletsSpent}";
        SetState(GameState.Over);
    }
}
