using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainHealthComponenet : MonoBehaviour, IDamageable
{
    private Train train;
    [field: SerializeField] public Transform ExplosionHolder;

    public void Initialize(Train train)
    { 
        this.train = train; 
    }

    public void Damage(int amount)
    {
        train.Damage(amount);
    }
}
