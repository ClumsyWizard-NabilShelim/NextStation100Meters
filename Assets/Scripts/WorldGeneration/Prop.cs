using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField] private float randomizeRadius;
    [SerializeField] private bool destroyOffScreen;

    private void Start()
    {
        Vector2 pos = Random.insideUnitCircle * randomizeRadius;
        pos.y /= 2.0f;
        transform.position += (Vector3)pos;
    }

    private void Update()
    {
        if(destroyOffScreen)
        {
            if(transform.position.x < -15.0f) //Some arbitrary number
            {
                Destroy(gameObject, 5.0f);
                destroyOffScreen = false;
            }    
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, randomizeRadius);
    }
}
