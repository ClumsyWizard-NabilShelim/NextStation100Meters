using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCompartment : MonoBehaviour
{
    protected Train train;

    [field: SerializeField] public int Health { get; private set; }
    public int CurrentHealth { get; private set; }

    public virtual void Initialize(Train train)
    {
        this.train = train;
        CurrentHealth = Health;
        train.HealthModified();
    }

    public void Damage(int amount)
    {
        CurrentHealth -= amount;
        train.HealthModified();
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
            Dead();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        train.HealthModified();
        if (CurrentHealth > Health)
            CurrentHealth = Health;
    }

    private void Dead()
    {
        Debug.Log("Dead");
    }
}
