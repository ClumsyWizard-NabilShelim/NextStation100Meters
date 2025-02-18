using ClumsyWizard.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : CW_Singleton<CameraManager>
{
    private Transform target;
    [SerializeField] private float followSpeed;
    [SerializeField] private Vector3 followOffset;

    public void Initialize(Transform player)
    {
        target = player;
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        transform.position = Vector3.Lerp(transform.position, target.position + followOffset, followSpeed * Time.deltaTime);
    }
}
