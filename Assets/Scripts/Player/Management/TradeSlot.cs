using ClumsyWizard.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TradeSlot : MonoBehaviour
{
    private CargoType cargotType;
    [SerializeField] private CW_Button decreaseButton;
    [SerializeField] private CW_Button increaseButton;
    [SerializeField] private CW_Button confirmButton;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI confirmText;

    private string tradeType;
    private int stock;
    private int cost;
    private Action<CargoType, int, int> onConfirm;

    private int currentSelectedAmount;

    private void Start()
    {
        decreaseButton.SetClickEvent(() =>
        {
            currentSelectedAmount--;
            if (currentSelectedAmount < 0)
                currentSelectedAmount = 0;

            UpdateUI();
        });

        increaseButton.SetClickEvent(() =>
        {
            currentSelectedAmount++;
            if (currentSelectedAmount > stock)
                currentSelectedAmount = stock;

            UpdateUI();
        });

        confirmButton.SetClickEvent(() =>
        {
            onConfirm?.Invoke(cargotType, currentSelectedAmount, cost);
        });
    }

    public void Initialize(CargoType cargotType, string tradeType, int stock, int cost, Action<CargoType, int, int> onConfirm)
    {
        this.cargotType = cargotType;
        this.tradeType = tradeType;
        this.stock = stock;
        this.cost = cost;
        this.onConfirm = onConfirm;

        currentSelectedAmount = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        amountText.text = currentSelectedAmount.ToString();
        confirmText.text = $"{tradeType} for {currentSelectedAmount * cost}<sprite={(int)Icon.Bullet}>";
    }
}