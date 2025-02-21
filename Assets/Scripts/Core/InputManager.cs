using ClumsyWizard.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : CW_Persistant<InputManager>, ISceneLoadEvent
{
    public Action OnPause;
    public Action OnShoot;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CW_HUDMenuManager.Instance.IsAnyMenuOpen)
                CW_HUDMenuManager.Instance.CloseLastMenu();
            else
                OnPause?.Invoke();
        }

        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            OnShoot?.Invoke();
    }

    //Clean up
    public void OnSceneLoadOver(Action onComplete)
    {
        onComplete?.Invoke();
    }

    public void OnSceneLoadTriggered(Action onComplete)
    {
        onComplete?.Invoke();
    }
}