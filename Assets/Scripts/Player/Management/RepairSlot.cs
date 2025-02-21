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

    private string id;
    private Action<string> onConfirm;

    public void Initialize(string id, Sprite icon, string info,Action<string> onConfirm)
    {
        this.id = id;
        this.onConfirm = onConfirm;

        iconImage.sprite = icon;
        infoText.text = info;
        confirmButton.SetClickEvent(() =>
        {
            onConfirm?.Invoke(id);
        });
    }
}
