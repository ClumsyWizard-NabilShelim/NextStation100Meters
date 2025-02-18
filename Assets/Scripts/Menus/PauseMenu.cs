using ClumsyWizard.Core;
using ClumsyWizard.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : CW_Persistant<PauseMenu>, IHUDMenu, ISceneLoadEvent
{
    private Animator animator;
    [SerializeField] private CW_Button resumeButton;
 
    private void Start()
    {
        animator = GetComponent<Animator>();

        resumeButton.AddClickEvent(Resume);

        InputManager.Instance.OnPause += Pause;
    }

    public void Pause()
    {
        if (CW_SceneManagement.Instance.IsMenuScene)
            return;

        CW_HUDMenuManager.Instance.AddOpenMenu(this);
        Time.timeScale = 0.0f;
        animator.SetBool("Pause", true);
    }

    private void Resume()
    {
        Time.timeScale = 0.0f;
        animator.SetBool("Pause", false);
    }

    //HUD Functionality
    public bool Close()
    {
        Resume();
        return true;
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
