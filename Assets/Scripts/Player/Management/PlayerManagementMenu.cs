using ClumsyWizard.Core;
using ClumsyWizard.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagementMenu : CW_Singleton<PlayerManagementMenu>
{
    [SerializeField] private Animator animator;
    [SerializeField] private List<ManagementPanel> panels;
    [SerializeField] private CW_Button nextPanelButton;
    [SerializeField] private CW_Button previousPanelButton;
    [SerializeField] private CW_Button closePanelButton;

    private int panelIndex;

    private void Start()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].gameObject.SetActive(false);
        }

        panels[0].gameObject.SetActive(true);

        //Button setup
        nextPanelButton.SetClickEvent(() =>
        {
            int index = panelIndex + 1;

            if (index >= panels.Count)
                index = 0;

            ShowPanel(index);
        });

        previousPanelButton.SetClickEvent(() =>
        {
            int index = panelIndex - 1;

            if (index < 0)
                index = panels.Count - 1;

            ShowPanel(index);
        });

        closePanelButton.SetClickEvent(CloseMenu);
    }

    public void OpenMenu()
    {
        animator.SetBool("Show", true);
        ShowPanel(0);
    }

    private void ShowPanel(int index)
    {
        panels[panelIndex].gameObject.SetActive(false);
        panels[panelIndex].Close();
        panelIndex = index;
        panels[panelIndex].gameObject.SetActive(true);
        panels[panelIndex].Open();
    }

    public void CloseMenu()
    {
        animator.SetBool("Show", false);
        GameManager.Instance.SetState(GameState.Travelling);
    }
}
