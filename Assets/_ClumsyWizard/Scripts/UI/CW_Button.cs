using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClumsyWizard.UI
{
    public class CW_Button : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        private Action onClick;

        public void AddClickEvent(Action clickEvent)
        {
            onClick += clickEvent;
        }
        public void SetClickEvent(Action clickEvent)
        {
            onClick = clickEvent;
        }
        public void RemoveClickEvent(Action clickEvent)
        {
            onClick -= clickEvent;
        }
        public void ResetClickEvent()
        {
            onClick = null;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onClick?.Invoke();
        }
    }
}
