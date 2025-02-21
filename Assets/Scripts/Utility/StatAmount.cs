using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatAmount : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private ToolTip toolTip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.Show(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.Show(false);
    }

    public void SetData(string amount)
    {
        amountText.text = amount;
    }
}