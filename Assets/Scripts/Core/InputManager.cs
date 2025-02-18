using ClumsyWizard.Core;
using System;
using System.Collections;
using UnityEngine;

public class InputManager : CW_Persistant<InputManager>, ISceneLoadEvent
{
    public Action OnPause;

    public Action OnJump;
    public Action OnSlideStart;
    public Action OnSlideEnd;
    public Action OnInteract;
    private float slideStartDelay = 0.15f;
    private bool wasSlide;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CW_HUDMenuManager.Instance.IsAnyMenuOpen)
                CW_HUDMenuManager.Instance.CloseLastMenu();
            else
                OnPause?.Invoke();
        }

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnJump?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            wasSlide = false;
            Invoke("StartSlide", slideStartDelay);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            CancelInvoke("StartSlide");

            if (!wasSlide)
                OnInteract?.Invoke();
            else
                OnSlideEnd?.Invoke();
        }
    }

    private void StartSlide()
    {
        wasSlide = true;
        OnSlideStart?.Invoke();
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