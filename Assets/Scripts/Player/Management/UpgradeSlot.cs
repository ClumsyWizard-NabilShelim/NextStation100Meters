using ClumsyWizard.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private CW_Button confirmButton;
    [SerializeField] private GameObject unavailableTint;

    private string id;
    private Action<string> onConfirm;

    public void Initialize(string id, Sprite icon, string label, string info, bool available, Action<string> onConfirm)
    {
        this.id = id;
        this.onConfirm = onConfirm;

        iconImage.sprite = icon;
        labelText.text = label;
        infoText.text = info;
        confirmButton.SetClickEvent(() =>
        {
            onConfirm?.Invoke(id);
        });

        SetAvailable(available);
    }

    public void SetAvailable(bool available)
    {
        unavailableTint.SetActive(!available);
    }
}
