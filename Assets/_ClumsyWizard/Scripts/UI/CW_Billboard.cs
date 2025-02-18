using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CW_Billboard : MonoBehaviour
{
    private Transform camTransform;

    private void Awake()
    {
        camTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + camTransform.forward);
    }
}
