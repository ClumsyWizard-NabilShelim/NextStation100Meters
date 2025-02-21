using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSort : MonoBehaviour
{
    [SerializeField] private bool alwaysUpdate;
    [SerializeField] private SpriteRenderer childRenderer;
    private new SpriteRenderer renderer;

    private void Start()
    {
        Sort();
    }

    private void Update()
    {
        if (alwaysUpdate)
            Sort();
    }

    private void Sort()
    {
        if(renderer == null)
        {
            renderer = GetComponent<SpriteRenderer>();
            if (renderer == null)
                renderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (renderer != null)
        {
            float basePos = renderer.bounds.min.y;
            renderer.sortingOrder = (int)(basePos * -100);

            if(childRenderer != null)
                childRenderer.sortingOrder = renderer.sortingOrder - 1;
        }
    }
}
