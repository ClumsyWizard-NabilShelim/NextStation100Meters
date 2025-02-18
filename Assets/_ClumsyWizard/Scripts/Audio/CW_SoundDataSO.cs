using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ClumsyWizard.Audio
{
    public enum SoundType
    {
        Background,
        SFX,
    }

    [CreateAssetMenu(menuName = "New/Sound")]
    public class CW_SoundDataSO : ScriptableObject
    {
        [SerializeField] private AudioClip[] clip;
        [field: SerializeField, Range(0.0f, 1.0f)] public float Volume { get; private set; }
        [field: SerializeField] public SoundType Type { get; private set; }

        public AudioClip GetClip()
        {
            return clip[Random.Range(0, clip.Length)];
        }
    }
}