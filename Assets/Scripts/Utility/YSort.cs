using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSort : MonoBehaviour
{
    private void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer == null)
            renderer = GetComponentInChildren<SpriteRenderer>();

        if(renderer != null)
            renderer.sortingOrder = -(int)transform.position.y;
    }
}
