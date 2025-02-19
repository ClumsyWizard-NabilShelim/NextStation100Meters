using ClumsyWizard.Utilities;
using System.Collections;
using UnityEngine;

public class CargoCompartment : TrainCompartment
{
    protected CW_Dictionary<CargoType, int> cargo = new CW_Dictionary<CargoType, int>();
    [SerializeField] private int capacity;

    public override void Initialize(Train train)
    {
        base.Initialize(train);

        cargo = new CW_Dictionary<CargoType, int>()
        {
            {CargoType.Metal, 0},
            {CargoType.Screw, 0},
            {CargoType.Passenger, 0}
        };
    }

    public bool AddCargo(CargoType type, int amount)
    {
        if (amount > capacity)
            return false;

        if (cargo.ContainsKey(type))
        {
            if (cargo[type] + amount > (type == CargoType.Passenger ? PassengerCapacity() : ResourceCapacity()))
                return false;

            cargo[type] += amount;
        }
        else
        {
            cargo.Add(type, amount);
        }

        PlayerDataManager.Instance.UpdateCargoUI();
        return true;
    }

    public bool RemoveCargo(CargoType type, int amount)
    {
        if (!cargo.ContainsKey(type) || cargo[type] - amount < 0)
            return false;

        cargo[type] -= amount;

        PlayerDataManager.Instance.UpdateCargoUI();
        return true;
    }

    //Helper functions
    public int GetCargoCount(CargoType type)
    {
        return cargo[type];
    }
    public int PassengerCount()
    {
        return cargo[CargoType.Passenger];
    }
    public int ResourceCount()
    {
        return cargo[CargoType.Metal] + cargo[CargoType.Screw];
    }
    public int ResourceCapacity()
    {
        return capacity - PassengerCount();
    }
    public int PassengerCapacity()
    {
        return capacity - ResourceCount();
    }
}