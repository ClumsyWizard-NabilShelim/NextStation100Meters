using ClumsyWizard.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private CW_Button confirmButton;

    private int index;
    private Action<int> onConfirm;

    public void Initialize(int index, Sprite icon, string info,Action<int> onConfirm)
    {
        this.index = index;
        this.onConfirm = onConfirm;

        iconImage.sprite = icon;
        infoText.text = info;
        confirmButton.SetClickEvent(() =>
        {
            onConfirm?.Invoke(index);
        });
    }
}
