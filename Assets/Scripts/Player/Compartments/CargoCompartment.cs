using ClumsyWizard.Utilities;
using System.Collections;
using UnityEngine;

public class CargoCompartment : TrainCompartment
{
    [field: SerializeField] public int Capacity { get; private set; } = 0;
}