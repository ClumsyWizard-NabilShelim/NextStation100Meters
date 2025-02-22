using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeRider : MonoBehaviour, IDamageable
{
    private Animator animator;

    [SerializeField] private float jumpToTargetTime;
    [SerializeField] private int damage;
    [SerializeField] private float damageRadius;
    [SerializeField] private float exlpodeDelay;
    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private Collider2D col;

    private Vector2 startingPos;
    private Vector2 targetPos;
    private float currentTime;
    private bool canJump;


    public void JumpToTarget(Vector2 target)
    {
        col.enabled = true;
        float min = transform.position.x - (damageRadius / 2.0f);
        float max = transform.position.x + (damageRadius / 2.0f);
        target = new Vector2(Mathf.Clamp(target.x, min, max), target.y);

        transform.parent = null;
        startingPos = transform.position;
        targetPos = target;
        currentTime = 0.0f;
        canJump = true;

        GetComponentInChildren<YSort>().enabled = false;

        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        renderer.sortingLayerName ="Player";
        renderer.sortingOrder = 5;

        animator = GetComponent<Animator>();
        animator.SetTrigger("Jump");
    }

    private void Update()
    {
        if (!canJump)
            return;

        if (currentTime <= jumpToTargetTime)
        {
            transform.position = Vector2.Lerp(startingPos, targetPos, currentTime / jumpToTargetTime);
            currentTime += Time.deltaTime;

            if (currentTime > jumpToTargetTime)
            {
                animator.SetTrigger("ExplodeBlink");
                Invoke("Explode", exlpodeDelay);
            }
        }
    }

    public void Explode()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, damageRadius, damageLayer);

        if (cols != null && cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                IDamageable damageable = cols[i].GetComponent<IDamageable>();
                if (damageable == null)
                    continue;

                damageable.Damage(damage);
            }
        }
        CameraShake.Instance.ShakeObject(0.3f, ShakeMagnitude.Large);
        Destroy(Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation), 2.0f);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }

    public void Damage(int amount)
    {
        CancelInvoke("Explode");
        Destroy(Instantiate(bloodEffect, transform.position, bloodEffect.transform.rotation), 2.0f);
        Destroy(gameObject);
    }
}
