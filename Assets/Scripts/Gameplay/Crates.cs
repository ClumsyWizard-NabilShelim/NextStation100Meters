using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crates : MonoBehaviour, IDamageable
{
    [SerializeField] private Vector2Int health;
    [SerializeField] private Vector2Int bulletsReward;
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
            int chance = Random.Range(0, 101);

            if(chance <= 30)
                PlayerDataManager.Instance.AddBullets(Random.Range(bulletsReward.x , bulletsReward.y + 1));
            else if(chance > 30 && chance <= 65)
                PlayerDataManager.Instance.Train.AddCargo(CargoType.Metal, Random.Range(metalReward.x , metalReward.y + 1), true);
            else if(chance > 65)
                PlayerDataManager.Instance.Train.AddCargo(CargoType.Screw, Random.Range(screwReward.x, screwReward.y + 1), true);

            GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2.0f);
            Destroy(gameObject);
        }
    }
}
