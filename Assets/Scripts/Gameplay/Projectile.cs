using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damageRadius;
    [SerializeField] private LayerMask damageLayer;
    private int damage;

    public void Initialize(int damage)
    {
        this.damage = damage;
        GetComponent<Rigidbody2D>().AddForce(transform.right * speed, ForceMode2D.Impulse);
        Destroy(gameObject, 10.0f);
    }

    private void Update()
    {
        Collider2D col = Physics2D.OverlapCircle(transform.position, damageRadius, damageLayer);

        if (col == null)
            return;

        IDamageable damageable= col.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.Damage(damage);

        Destroy(gameObject);
    }

    //Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
