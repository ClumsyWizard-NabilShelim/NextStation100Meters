using ClumsyWizard.UI;
using ClumsyWizard.Utilities;
using System.Collections;
using TMPro;
using UnityEngine;

public class CargoCompartment : TrainCompartment
{
    [field: SerializeField] public int Capacity { get; private set; } = 0;
    [SerializeField] private Sprite endSprite;
    [SerializeField] private Sprite centerSprite;

    private KickPassengerMenu kickPassengerMenu;

    public override void Initialize(Train train)
    {
        base.Initialize(train);

        kickPassengerMenu = GetComponentInChildren<KickPassengerMenu>();
        kickPassengerMenu.Initialize(train);
    }

    public void SetEnd(bool isEnd)
    {
        if(isEnd)
            GetComponentInChildren<SpriteRenderer>().sprite = endSprite;
        else
            GetComponentInChildren<SpriteRenderer>().sprite = centerSprite;
    }
}