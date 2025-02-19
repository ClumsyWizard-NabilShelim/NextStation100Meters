using System.Collections;
using TMPro;
using UnityEngine;

public class StatAmount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountText;

    public void SetData(string amount)
    {
        amountText.text = amount;
    }
}