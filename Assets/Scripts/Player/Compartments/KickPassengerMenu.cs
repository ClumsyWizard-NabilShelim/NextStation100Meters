using ClumsyWizard.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class KickPassengerMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Kick Passenger")]
    [SerializeField] private GameObject container;
    [SerializeField] private CW_Button decreaseButton;
    [SerializeField] private CW_Button increaseButton;
    [SerializeField] private CW_Button confirmButton;
    [SerializeField] private TextMeshProUGUI amountText;
    private int currentSelectedAmount;

    public void Initialize(Train train)
    {
        container.SetActive(false);
        decreaseButton.SetClickEvent(() =>
        {
            currentSelectedAmount--;
            if (currentSelectedAmount < 0)
                currentSelectedAmount = 0;

            amountText.text = currentSelectedAmount.ToString();
        });

        increaseButton.SetClickEvent(() =>
        {
            currentSelectedAmount++;
            if (currentSelectedAmount > train.GetCargoCount(CargoType.Passenger))
                currentSelectedAmount = train.GetCargoCount(CargoType.Passenger);

            amountText.text = currentSelectedAmount.ToString();
        });

        confirmButton.SetClickEvent(() =>
        {
            if (currentSelectedAmount == 0)
                return;

            train.AddPassengerStat(StatType.Anger, (int)((currentSelectedAmount / (float)train.GetCargoCount(CargoType.Passenger)*100)));
            train.RemovePassengerData(currentSelectedAmount);
            StatPopUpManager.Instance.ShowStatPopUp(transform.position, $"-{currentSelectedAmount}<sprite={(int)Icon.Passenger}>", StatPopUpColor.Red);
            currentSelectedAmount = 0;
            container.SetActive(false);
        });

        currentSelectedAmount = 0;
        amountText.text = currentSelectedAmount.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        container.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        container.SetActive(false);
    }
}