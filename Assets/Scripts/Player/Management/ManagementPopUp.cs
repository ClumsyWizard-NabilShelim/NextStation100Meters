using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManagementPopUp : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [field: SerializeField] public ToolTip ToolTip { get; private set; }

    [SerializeField] private Image iconImage;
    [SerializeField] private Image fillImage;
    private Action<string, float> startCallback;
    private Action<string> cancelCallback;
    private string id;
    private string info;
    private string label;
    private float requiredTime;
    private bool toggleCancel;

    public void Initialize(string id, float requiredTime, string label, string info, Sprite icon, Action<string, float> startCallback, Action<string> cancelCallback)
    {
        this.id = id;
        this.requiredTime = requiredTime;
        this.info = info;
        this.label = label;
        iconImage.sprite = icon;

        ToolTip.SetInfo(label, info);

        this.startCallback = startCallback;
        this.cancelCallback = cancelCallback;

        fillImage.fillAmount = 0;
        toggleCancel = false;
    }

    public void UpdateFill(float amount)
    {
        fillImage.fillAmount = amount;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(toggleCancel)
            cancelCallback?.Invoke(id);
        else
            startCallback?.Invoke(id, requiredTime);

        toggleCancel = !toggleCancel;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.SetInfo(label, info);
        ToolTip.Show(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTip.Show(true);
    }
}
