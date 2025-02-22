using ClumsyWizard.Audio;
using ClumsyWizard.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private CW_AudioPlayer audioPlayer;
    [SerializeField] private CW_Button startButton;
    [SerializeField] private CW_Button tutorialButton;
    [SerializeField] private float focusTime;

    private Vector2 startingPosition;
    private Transform cameraHolder;
    private bool hasStarted;
    private float currentTime;

    private void Start()
    {
        audioPlayer = GetComponent<CW_AudioPlayer>();
        cameraHolder = Camera.main.transform.parent;
        startingPosition = cameraHolder.position;
        startButton.SetClickEvent(() =>
        {
            audioPlayer.Play("Click");
            GameManager.Instance.SetState(GameState.Travelling);
            hasStarted = true;
            currentTime = 0.0f;
        });

        tutorialButton.SetClickEvent(() =>
        {
            audioPlayer.Play("Click");
            TutorialManager.Instance.Open();
        });
    }

    private void Update()
    {
        if (!hasStarted)
            return;

        if (currentTime <= focusTime)
        {
            cameraHolder.position = Vector2.Lerp(startingPosition, Vector2.zero, currentTime / focusTime);
            currentTime += Time.deltaTime;
        }
        else
        {
            cameraHolder.position = Vector2.zero;
            hasStarted = false;
        }
    }
}
