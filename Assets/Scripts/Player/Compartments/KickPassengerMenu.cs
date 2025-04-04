﻿using ClumsyWizard.Audio;
using ClumsyWizard.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class KickPassengerMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CW_AudioPlayer audioPlayer;

    [Header("Kick Passenger")]
    [SerializeField] private GameObject container;
    [SerializeField] private CW_Button decreaseButton;
    [SerializeField] private CW_Button increaseButton;
    [SerializeField] private CW_Button confirmButton;
    [SerializeField] private TextMeshProUGUI amountText;
    private int currentSelectedAmount;

    public void Initialize(Train train)
    {
        audioPlayer = GetComponent<CW_AudioPlayer>();
        container.SetActive(false);
        decreaseButton.SetClickEvent(() =>
        {
            audioPlayer.Play("Click");
            currentSelectedAmount--;
            if (currentSelectedAmount < 0)
                currentSelectedAmount = 0;

            amountText.text = currentSelectedAmount.ToString();
        });

        increaseButton.SetClickEvent(() =>
        {
            audioPlayer.Play("Click");
            currentSelectedAmount++;
            if (currentSelectedAmount > train.GetCargoCount(CargoType.Passenger))
                currentSelectedAmount = train.GetCargoCount(CargoType.Passenger);

            amountText.text = currentSelectedAmount.ToString();
        });

        confirmButton.SetClickEvent(() =>
        {
            if (currentSelectedAmount == 0)
                return;

            audioPlayer.Play("Click");
            train.AddPassengerStat(StatType.Anger, (int)((currentSelectedAmount / (float)train.GetCargoCount(CargoType.Passenger)*100)));
            train.RemovePassengerData(currentSelectedAmount);
            GameManager.Instance.PassengersKilled += currentSelectedAmount;
            StatPopUpManager.Instance.ShowStatPopUp(transform.position, $"-{currentSelectedAmount}<sprite={(int)Icon.Passenger}>", StatPopUpColor.Red);
            currentSelectedAmount = 0;
            amountText.text = currentSelectedAmount.ToString();
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