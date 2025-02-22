using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crates : MonoBehaviour, IDamageable
{
    [SerializeField] private Vector2Int health;
    [SerializeField] private Vector2Int metalReward;
    [SerializeField] private Vector2Int screwReward;
    [SerializeField] private GameObject destroyEffect;
    private int currentHealth;

    private void Start()
    {
        currentHealth = Random.Range(health.x, health.y + 1);
    }

    public void Damage(int amount)
    {
        currentHealth-=amount;

        if (currentHealth <= 0)
        {
            PlayerDataManager.Instance.Train.AddCargo(CargoType.Metal, Random.Range(metalReward.x , metalReward.y + 1), true);
            PlayerDataManager.Instance.Train.AddCargo(CargoType.Screw, Random.Range(screwReward.x, screwReward.y + 1), true);
            GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2.0f);
            Destroy(gameObject);
        }
    }
}
