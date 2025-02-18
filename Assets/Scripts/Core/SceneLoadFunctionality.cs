using ClumsyWizard.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadFunctionality : CW_SceneLoadFunctionality
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnLoadTriggered()
    {
        animator.SetBool("Fade", true);
    }

    public override void LoadingProgress(float progress)
    {
    }

    public override void OnLoadingOver()
    {
        animator.SetBool("Fade", false);
    }
}
