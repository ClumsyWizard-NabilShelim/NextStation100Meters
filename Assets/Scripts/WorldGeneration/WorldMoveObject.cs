using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMoveObject : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Update()
    {
        transform.Translate(-transform.right * speed * Time.deltaTime);
    }
}
