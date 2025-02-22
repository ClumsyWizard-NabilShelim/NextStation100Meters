using ClumsyWizard.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RepairSlot : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI infoText;

    private string id;
    private Action<string> onConfirm;

    public void Initialize(string id, Sprite icon, string info,Action<string> onConfirm)
    {
        this.id = id;
        this.onConfirm = onConfirm;

        iconImage.sprite = icon;
        infoText.text = info;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
            onConfirm?.Invoke(id);
    }
}
