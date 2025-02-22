using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRaiderEnemy : Enemy
{
    private Vector2 shootTarget;
    [SerializeField] private int fireRate;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float fireCoolDown;
    [SerializeField] private int damage;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Vector2 startShootDelayRange;
    private float currentTime;
    private int currentShotsFired;

    public override void Initialize(Vector2 targetPosition)
    {
        base.Initialize(targetPosition);

        shootTarget = PlayerDataManager.Instance.Train.GetRandomPointOnBody();

        currentShotsFired = 0;
        currentTime = Random.Range(startShootDelayRange.x, startShootDelayRange.y);
    }

    protected override void Combat()
    {
        if(currentTime <= 0.0f)
        {
            if (currentShotsFired >= fireRate)
            {
                currentShotsFired = 0;
                currentTime = fireCoolDown;
                animator.SetBool("Shoot", false);
                return;
            }

            animator.SetBool("Shoot", true);
            currentTime = timeBetweenShots;
            currentShotsFired++;
            audioPlayer.Play("Shoot");
            CameraShake.Instance.ShakeObject(0.1f, ShakeMagnitude.Medium);
            Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation).Initialize(damage);
        }

        currentTime -= Time.deltaTime;

        Vector2 diff = shootTarget - (Vector2)shootPoint.position;
        float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        shootPoint.eulerAngles = new Vector3(0.0f, 0.0f, rot);
    }
}
