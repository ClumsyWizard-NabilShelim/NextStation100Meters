using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMoveObject : MonoBehaviour
{
    private void Update()
    {
        transform.Translate(-transform.right * GameManager.Instance.CurrentWorldSpeed * Time.deltaTime);
    }
}
