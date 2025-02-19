using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    [field: SerializeField] public EngineCompartment EngineCompartment { get; private set; }
    [field: SerializeField] public WorkerCompartment WorkerCompartment { get; private set; }
    [field: SerializeField] public List<CargoCompartment> CargoCompartments { get; private set; }

    public void Initialize()
    {
        EngineCompartment.Initialize(this);
        WorkerCompartment.Initialize(this);
        for (int i = 0; i < CargoCompartments.Count; i++)
        {
            CargoCompartments[i].Initialize(this);
        }

        transform.position = new Vector3(transform.childCount - 1.0f, transform.position.y, 0.0f);
    }

    public int GetTotalHealth()
    {
        int health = EngineCompartment.Health + WorkerCompartment.Health;
        for (int i = 0; i < CargoCompartments.Count; i++)
        {
            health += CargoCompartments[i].Health;
        }

        return health;
    }
    public int GetTotalCurrentHealth()
    {
        int health = EngineCompartment.CurrentHealth + WorkerCompartment.CurrentHealth;
        for (int i = 0; i < CargoCompartments.Count; i++)
        {
            health += CargoCompartments[i].CurrentHealth;
        }

        return health;
    }

    //Cargo Usage
    public bool AddCargo(CargoType type, int amount)
    {
        for (int i = 0; i < CargoCompartments.Count; i++)
        {
            if (CargoCompartments[i].AddCargo(type, amount))
                return true;
        }

        return false;
    }
    public bool RemoveCargo(CargoType type, int amount)
    {
        for (int i = 0; i < CargoCompartments.Count; i++)
        {
            if (CargoCompartments[i].RemoveCargo(type, amount))
                return true;
        }

        return false;
    }

    //Cargo getters
    public int GetCargoCount(CargoType type)
    {
        int total = 0;
        for (int i = 0; i < CargoCompartments.Count; i++)
        {
            total += CargoCompartments[i].GetCargoCount(type);
        }

        return total;
    }
    public int GetPassengerCount()
    {
        int total = 0;
        for (int i = 0; i < CargoCompartments.Count; i++)
        {
            total += CargoCompartments[i].PassengerCount();
        }

        return total;
    }
    public int GetPassengerCapacity()
    {
        int total = 0;
        for (int i = 0; i < CargoCompartments.Count; i++)
        {
            total += CargoCompartments[i].PassengerCapacity();
        }

        return total;
    }

    public int GetResourceCount()
    {
        int total = 0;
        for (int i = 0; i < CargoCompartments.Count; i++)
        {
            total += CargoCompartments[i].ResourceCount();
        }

        return total;
    }
    public int GetResourceCapacity()
    {
        int total = 0;
        for (int i = 0; i < CargoCompartments.Count; i++)
        {
            total += CargoCompartments[i].ResourceCapacity();
        }

        return total;
    }

    //UI
    public void HealthModified()
    {
        PlayerDataManager.Instance.UpdateStatUI(StatType.HullIntegrity, GetTotalCurrentHealth(), GetTotalHealth());
    }
}
