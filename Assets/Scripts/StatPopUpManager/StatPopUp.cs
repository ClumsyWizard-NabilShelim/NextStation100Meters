using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;

    public void SetData(string info, Color color)
    {
        infoText.color = color;
        infoText.text = info;
    }

    public void DestroyPopUp()
    {
        Destroy(gameObject);
    }
}
