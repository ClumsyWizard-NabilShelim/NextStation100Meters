using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMoveObject : MonoBehaviour
{
    [SerializeField] private float multiplier = 1.0f;

    private void Update()
    {
        transform.Translate(-transform.right * GameManager.Instance.CurrentWorldSpeed * multiplier * Time.deltaTime);
    }
}
