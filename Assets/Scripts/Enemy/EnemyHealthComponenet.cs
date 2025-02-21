using System.Collections;
using UnityEngine;

public class EnemyHealthComponenet : MonoBehaviour, IDamageable
{
    private Enemy enemy;
    [SerializeField] private int health;
    private int currentHealth;

    public void Initialize(Enemy enemy)
    {
        this.enemy = enemy;
        currentHealth = health;
    }

    public void Damage(int amount)
    {
        if (currentHealth <= 0)
            return;

        currentHealth -= amount;

        if (currentHealth <= 0)
            Dead();
    }

    private void Dead()
    {
        enemy.Killed();
    }
}