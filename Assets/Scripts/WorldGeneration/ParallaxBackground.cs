using System.Collections;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float backgroundSize;
    [SerializeField] private float distanceBeforeReorder;

    private Transform lastBackground;

    private void Start()
    {
        lastBackground = transform.GetChild(0);
    }

    private void Update()
    {
        if (lastBackground.localPosition.x < distanceBeforeReorder)
        {
            lastBackground.localPosition = transform.GetChild(2).localPosition + Vector3.right * backgroundSize;
            lastBackground.SetAsLastSibling();
            lastBackground = transform.GetChild(0);
        }
    }
}