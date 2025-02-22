using ClumsyWizard.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CW_Singleton<EnemyManager>
{
    [SerializeField] private Transform spawnPointHolder;
    [SerializeField] private Transform targetPointHolderRestricted;
    [SerializeField] private Transform targetPointHolderNormal;
    private List<Transform> targetPointsRestricted = new List<Transform>();
    private List<Transform> targetPointsNormal = new List<Transform>();

    [Header("Difficulty")]
    [SerializeField] private Vector2Int waveDurationRange;
    [SerializeField] private Vector2Int waveSizeRange;
    [SerializeField] private List<Enemy> normalEnemies;
    [SerializeField] private List<Enemy> restrictedEnemies;

    private float currentWaveTime;

    private int waveSize;
    private List<Enemy> enemies = new List<Enemy>();

    private void Start()
    {
        GameManager.Instance.OnStateChange += (GameState state) =>
        {
            if (state == GameState.Over)
            {
                if (enemies.Count > 0)
                {
                    for (int i = enemies.Count - 1; i >= 0; i--)
                    {
                        enemies[i].Retreat();
                        RemoveEnemy(enemies[i]);
                    }
                }
            }
        };
    }

    public void StartWave()
    {
        waveSize = Random.Range(waveSizeRange.x, waveSizeRange.y + 1);
        GameManager.Instance.SetState(GameState.UnderAttack);
        enemies = new List<Enemy>();
        currentWaveTime = Random.Range(waveDurationRange.x, waveDurationRange.y);
        ReInitializeTargetPoints();

        for (int i = 0; i < waveSize; i++)
        {
            if (!CreateEnemy(i % 2 == 0))
                break;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.UnderAttack)
            return;

        if(currentWaveTime <= 0.0f)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Retreat();
                RemoveEnemy(enemies[i]);
            }
        }
        else
        {
            currentWaveTime -= Time.deltaTime;
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        if (enemies.Count == 0)
            GameManager.Instance.SetState(GameState.Travelling);
    }

    //Helper functions
    private bool CreateEnemy(bool restricted)
    {
        Enemy enemy = null;
        Transform target = GetTargetPoint(restricted);

        if (target == null)
            return false;

        if (restricted)
            enemy = Instantiate(restrictedEnemies[Random.Range(0, restrictedEnemies.Count)], spawnPointHolder.GetChild(Random.Range(0, spawnPointHolder.childCount)).position, Quaternion.identity);
        else
            enemy = Instantiate(normalEnemies[Random.Range(0, normalEnemies.Count)], spawnPointHolder.GetChild(Random.Range(0, spawnPointHolder.childCount)).position, Quaternion.identity);

        enemies.Add(enemy);
        enemy.Initialize(target.position);

        return true;
    }
    private Transform GetTargetPoint(bool restricted)
    {
        if (restricted)
        {
            if (targetPointsRestricted.Count == 0)
                return null;

            int index = Random.Range(0, targetPointsRestricted.Count);
            Transform point = targetPointsRestricted[index];
            targetPointsRestricted.RemoveAt(index);
            return point;
        }
        else
        {
            if (targetPointsNormal.Count == 0)
                return null;

            int index = Random.Range(0, targetPointsNormal.Count);
            Transform point = targetPointsNormal[index];
            targetPointsNormal.RemoveAt(index);
            return point;
        }
    }

    private void ReInitializeTargetPoints()
    {
        for (int i = 0; i < targetPointHolderRestricted.childCount; i++)
        {
            targetPointsRestricted.Add(targetPointHolderRestricted.GetChild(i));
        }
        for (int i = 0; i < targetPointHolderNormal.childCount; i++)
        {
            targetPointsNormal.Add(targetPointHolderNormal.GetChild(i));
        }
    }
}