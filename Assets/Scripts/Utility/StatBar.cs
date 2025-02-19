using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI amountText;

    public void SetBarData(float fillAmount, string amount)
    {
        fillImage.fillAmount = fillAmount;
        amountText.text = amount;
    }
}