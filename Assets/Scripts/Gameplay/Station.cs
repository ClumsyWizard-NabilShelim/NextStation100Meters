using ClumsyWizard.Core;
using ClumsyWizard.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Station : MonoBehaviour
{
    [SerializeField] private CW_Dictionary<CargoType, Vector2Int> cargoAmountRange;
    [SerializeField] private CW_Dictionary<CargoType, Vector2Int> cargoCostRange;
    [SerializeField] private Vector2Int passengerTravelDistanceRange;

    public CW_Dictionary<CargoType, int> CargoStock { get; private set; }
    public CW_Dictionary<CargoType, int> CargoCost { get; private set; }
    public int PassengerTravelDistance { get; private set; }

    public void Initialize()
    {
        CargoStock = new CW_Dictionary<CargoType, int>();
        CargoCost = new CW_Dictionary<CargoType, int>();

        for (int i = 0; i < Enum.GetValues(typeof(CargoType)).Length; i++)
        {
            CargoType type = (CargoType)i;

            CargoStock.Add(type, Random.Range(cargoAmountRange[type].x, cargoAmountRange[type].y + 1));
            CargoCost.Add(type, Random.Range(cargoCostRange[type].x, cargoCostRange[type].y + 1));
        }

        PassengerTravelDistance = Random.Range(passengerTravelDistanceRange.x, passengerTravelDistanceRange.y);
    }

    public void AddCargo(CargoType type, int amount)
    {
        CargoStock[type] += amount;
    }

    public void RemoveCargo(CargoType type, int amount)
    {
        CargoStock[type] -= amount;
    }

    public int GetCargoCost(CargoType type)
    {
        return CargoCost[type];
    }
    public int GetCargoStock(CargoType type)
    {
        return CargoStock[type];
    }
}
