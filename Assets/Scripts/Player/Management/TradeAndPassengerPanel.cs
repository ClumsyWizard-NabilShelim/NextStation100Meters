using ClumsyWizard.Audio;
using ClumsyWizard.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TradeAndPassengerPanel : ManagementPanel
{
    private CW_AudioPlayer audioPlayer;

    [Header("Trade")]
    [SerializeField] private int metalSellCost;
    [SerializeField] private int screwSellCost;

    [SerializeField] private TradeSlot metalBuySlot;
    [SerializeField] private TradeSlot screwBuySlot;

    [SerializeField] private TradeSlot metalSellSlot;
    [SerializeField] private TradeSlot screwSellSlot;

    [Header("Passenger")]
    [SerializeField] private CW_Button decreasePassengerButton;
    [SerializeField] private CW_Button increasePassengerButton;
    [SerializeField] private CW_Button takePassengerButton;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI passengerCountText;
    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private TextMeshProUGUI takePassengerText;
    [SerializeField] private GameObject passengerContainer;
    [SerializeField] private GameObject noPassengerContainer;
    private int passengerCount;
    private int maxPassengerTakeable;
    private int passengerFee;
    private int currentPassengerCount;
    private bool newStation = true;

    private void Start()
    {
        audioPlayer = GetComponent<CW_AudioPlayer>();
        GameManager.Instance.OnStateChange += (GameState state) =>
        {
            if (state == GameState.Station)
                newStation = true;
        };
    }

    public override void Open()
    {
        base.Open();

        passengerContainer.SetActive(newStation);
        noPassengerContainer.SetActive(!newStation);

        //Trade
        metalBuySlot.Initialize(CargoType.Metal, "Buy", StationManager.Instance.CurrentStation.GetCargoStock(CargoType.Metal), StationManager.Instance.CurrentStation.GetCargoCost(CargoType.Metal), OnBuyCargo);
        screwBuySlot.Initialize(CargoType.Screw, "Buy", StationManager.Instance.CurrentStation.GetCargoStock(CargoType.Screw), StationManager.Instance.CurrentStation.GetCargoCost(CargoType.Screw), OnBuyCargo);

        metalSellSlot.Initialize(CargoType.Metal, "Sell", PlayerDataManager.Instance.Train.GetCargoCount(CargoType.Metal), metalSellCost, OnSellCargo);
        screwSellSlot.Initialize(CargoType.Screw, "Sell", PlayerDataManager.Instance.Train.GetCargoCount(CargoType.Metal), screwSellCost, OnSellCargo);

        //Passenger
        passengerCount = StationManager.Instance.CurrentStation.GetCargoStock(CargoType.Passenger);
        passengerFee = StationManager.Instance.CurrentStation.GetCargoCost(CargoType.Passenger);
        int openSpace = PlayerDataManager.Instance.Train.GetPassengerCapacity() - PlayerDataManager.Instance.Train.GetCargoCount(CargoType.Passenger);
        capacityText.text = $"Open Space: {openSpace}";
        maxPassengerTakeable = Mathf.Min(openSpace, passengerCount);
        currentPassengerCount = maxPassengerTakeable;

        decreasePassengerButton.SetClickEvent(() =>
        {
            audioPlayer.Play("Click");
            currentPassengerCount--;
            if (currentPassengerCount < 0)
                currentPassengerCount = 0;

            UpdatePassengerUI();
        });
        increasePassengerButton.SetClickEvent(() =>
        {
            audioPlayer.Play("Click");
            currentPassengerCount++;
            if (currentPassengerCount > maxPassengerTakeable)
                currentPassengerCount = maxPassengerTakeable;

            UpdatePassengerUI();
        });
        takePassengerButton.SetClickEvent(TakePassengers);

        UpdatePassengerUI();
    }

    //Trade
    private void OnBuyCargo(CargoType cargoType, int amount, int cost)
    {
        int totalCost = amount * cost;

        if(PlayerDataManager.Instance.UseBullets(totalCost))
        {
            if(PlayerDataManager.Instance.Train.AddCargo(cargoType, amount, false))
            {
                audioPlayer.Play("Coin");

                if (cargoType == CargoType.Metal)
                    metalBuySlot.ResetCurrentCount();
                else if(cargoType == CargoType.Screw)
                    screwBuySlot.ResetCurrentCount();

                StationManager.Instance.CurrentStation.RemoveCargo(cargoType, amount);
                Open(); //Refresh UI
            }
            else
            {
                //Show pop up panel
            }
        }
        else
        {
            //Show pop up panel
        }
    }

    private void OnSellCargo(CargoType cargoType, int amount, int cost)
    {
        int totalCost = amount * cost;
        PlayerDataManager.Instance.AddBullets(totalCost);
        PlayerDataManager.Instance.Train.RemoveCargo(cargoType, amount);
        StationManager.Instance.CurrentStation.AddCargo(cargoType, amount);

        if (cargoType == CargoType.Metal)
            metalSellSlot.ResetCurrentCount();
        else if (cargoType == CargoType.Screw)
            screwSellSlot.ResetCurrentCount();

        audioPlayer.Play("Coin");
        Open(); //Refresh UI
    }

    //Passenger
    private void TakePassengers()
    {
        audioPlayer.Play("Click");
        PlayerDataManager.Instance.Train.AddPassengerDetials(currentPassengerCount, StationManager.Instance.CurrentStation.PassengerTravelDistance, passengerFee);
        newStation = false;
        Open(); //Refresh UI
    }
    private void UpdatePassengerUI()
    {
        infoText.text = $"{passengerCount} passengers want to travel and stop after {StationManager.Instance.CurrentStation.PassengerTravelDistance} station(s). Total collectedable fee from passengers {passengerCount * passengerFee}<sprite={(int)Icon.Bullet}>.\n How many do you want to take>";
        passengerCountText.text = currentPassengerCount.ToString();

        takePassengerText.text = $"Take {currentPassengerCount} passengers\nProfit {currentPassengerCount * passengerFee}<sprite={(int)Icon.Bullet}>";
    }
}
