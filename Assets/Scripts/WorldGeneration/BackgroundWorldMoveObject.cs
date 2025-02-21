using System.Collections;
using UnityEngine;

public class BackgroundWorldMoveObject : MonoBehaviour
{
    [SerializeField] private float slowDownRate;
    private void Update()
    {
        transform.Translate(-transform.right * GameManager.Instance.CurrentWorldSpeed *slowDownRate * Time.deltaTime);
    }
}