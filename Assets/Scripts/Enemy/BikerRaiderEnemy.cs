using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BikerRaiderEnemy : Enemy
{
    [SerializeField] private BikeRider rider;
    [SerializeField] private Vector2 delayBeforeJumpRange;

    public override void Initialize(Vector2 targetPosition)
    {
        base.Initialize(targetPosition);

        Invoke("ThrowRider", Random.Range(delayBeforeJumpRange.x, delayBeforeJumpRange.y));
    }

    protected override void Combat()
    {
    }

    private void ThrowRider()
    {
        rider.JumpToTarget(PlayerDataManager.Instance.Train.GetRandomPointOnBody());
        animator.SetTrigger("Destroy");
        Invoke("DestroyBike", 0.5f);
    }

    private void DestroyBike()
    {
        base.Killed();
    }

    public override void Killed()
    {
        rider.Explode();
        base.Killed();
    }
}
