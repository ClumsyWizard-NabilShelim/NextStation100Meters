using ClumsyWizard.Audio;
using ClumsyWizard.Core;
using ClumsyWizard.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : CW_Singleton<TutorialManager>
{
    private Animator animator;
    private CW_AudioPlayer audioPlayer;

    [SerializeField] private Transform tutorialHolder;
    [SerializeField] private CW_Button nextButton;
    [SerializeField] private CW_Button previousButton;
    [SerializeField] private CW_Button closeButton;
    private int currentIndex;

    private void Start()
    {
        audioPlayer = GetComponent<CW_AudioPlayer>();

        animator = GetComponent<Animator>();
        closeButton.gameObject.SetActive(false);
        currentIndex = 0;
        tutorialHolder.GetChild(currentIndex).gameObject.SetActive(true);

        nextButton.SetClickEvent(() =>
        {
            audioPlayer.Play("Click");
            tutorialHolder.GetChild(currentIndex).gameObject.SetActive(false);
            currentIndex++;
            if(currentIndex >= tutorialHolder.childCount)
                currentIndex = tutorialHolder.childCount - 1;
            
            if(currentIndex == tutorialHolder.childCount - 1)
                closeButton.gameObject.SetActive(true);
            else
                closeButton.gameObject.SetActive(false);

            tutorialHolder.GetChild(currentIndex).gameObject.SetActive(true);
        });

        previousButton.SetClickEvent(() =>
        {
            audioPlayer.Play("Click");
            tutorialHolder.GetChild(currentIndex).gameObject.SetActive(false);

            currentIndex--;
            if (currentIndex < 0)
                currentIndex = 0;

            closeButton.gameObject.SetActive(false);
            tutorialHolder.GetChild(currentIndex).gameObject.SetActive(true);
        });

        closeButton.SetClickEvent(() =>
        {
            audioPlayer.Play("Menu");
            animator.SetBool("Show", false);
        });
    }

    public void Open()
    {
        audioPlayer.Play("Menu");
        animator.SetBool("Show", true);

        tutorialHolder.GetChild(currentIndex).gameObject.SetActive(false);
        currentIndex = 0;
        tutorialHolder.GetChild(currentIndex).gameObject.SetActive(true);
    }
}
