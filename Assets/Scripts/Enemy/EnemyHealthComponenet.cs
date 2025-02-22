using System.Collections;
using UnityEngine;

public class EnemyHealthComponenet : MonoBehaviour, IDamageable
{
    private Enemy enemy;
    [SerializeField] private int health;
    [SerializeField] private GameObject explosionEffect;
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
        DestroyEffect();
        enemy.Killed();
    }

    public void DestroyEffect()
    {
        Destroy(Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation), 2.0f);
        CameraShake.Instance.ShakeObject(0.1f, ShakeMagnitude.Small);
    }
}