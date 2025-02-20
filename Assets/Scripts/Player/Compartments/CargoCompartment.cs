using ClumsyWizard.Utilities;
using System.Collections;
using UnityEngine;

public class CargoCompartment : TrainCompartment
{
    [field: SerializeField] public int Capacity { get; private set; } = 0;
    [SerializeField] private Sprite endSprite;
    [SerializeField] private Sprite centerSprite;

    public void SetEnd(bool isEnd)
    {
        if(isEnd)
            GetComponentInChildren<SpriteRenderer>().sprite = endSprite;
        else
            GetComponentInChildren<SpriteRenderer>().sprite = centerSprite;
    }
}