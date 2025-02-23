using System.Collections;
using System.Collections.Generic;
using ClumsyWizard.Audio;
using UnityEngine;

public class TrainExplosionVFX : MonoBehaviour
{
    [SerializeField] private CW_AudioPlayer audioPlayer;
    [SerializeField] private Animator animator;

    public void Explode()
    {
        audioPlayer.Play("Explosion");
        animator.SetTrigger("Explode");
    }
}
