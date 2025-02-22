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
        audioPlayer.Play("Jump");
        rider.JumpToTarget(PlayerDataManager.Instance.Train.GetRandomPointOnBody());
        animator.SetTrigger("Destroy");
        Invoke("DestroyBike", 1.05f);
    }

    private void DestroyBike()
    {
        enemyStats.DestroyEffect();
        base.Killed();
    }

    public override void Retreat()
    {
        base.Retreat();
        CancelInvoke("ThrowRider");
    }
}
