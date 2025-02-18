using ClumsyWizard.Core;
using ClumsyWizard.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace ClumsyWizard.Audio
{
    public class AudioVolumeLerpData
    {
        public float startVolume;
        public float targetVolume;
        public float elapsedTime;
        public AudioSource audioSource;

        public AudioVolumeLerpData(float startVolume, float targetVolume, AudioSource audioSource)
        {
            this.startVolume = startVolume;
            this.targetVolume = targetVolume;
            this.audioSource = audioSource;
            elapsedTime = 0.0f;
        }
    }

    public class CW_AudioManager : CW_Persistant<CW_AudioManager>, ISceneLoadEvent
    {
        [field: SerializeField] public CW_Dictionary<SoundType, AudioMixerGroup> Mixers { get; private set; }

        //Fade out audio
        private List<AudioSource> backgroundAudioSources = new List<AudioSource>();
        private List<AudioVolumeLerpData> audiosToStop = new List<AudioVolumeLerpData>();
        private float audioStopDuration = 1.0f;

        private void Update()
        {
            if (audiosToStop.Count == 0)
                return;

            for (int i = audiosToStop.Count - 1; i >= 0; i--)
            {
                AudioVolumeLerpData lerpData = audiosToStop[i];

                if (lerpData.elapsedTime > audioStopDuration)
                {
                    lerpData.audioSource.Stop();
                    audiosToStop.RemoveAt(i);
                }
                else
                {
                    lerpData.elapsedTime += Time.deltaTime;
                    lerpData.audioSource.volume = Mathf.Lerp(lerpData.startVolume, lerpData.targetVolume, lerpData.elapsedTime / audioStopDuration);
                }
            }
        }

        public void Stop(AudioSource audioSource)
        {
            audiosToStop.Add(new AudioVolumeLerpData(audioSource.volume, 0.0f, audioSource));
        }

        public void AddBackgroundAudio(AudioSource audioSource)
        {
            backgroundAudioSources.Add(audioSource);
        }

        //Clean up
        public void OnSceneLoadTriggered(Action onComplete)
        {
            foreach (AudioSource audioSource in backgroundAudioSources)
            {
                Stop(audioSource);
            }

            backgroundAudioSources.Clear();

            if (audiosToStop.Count == 0)
                onComplete?.Invoke();
            else
                StartCoroutine(CompleteAudioStop(onComplete));
        }

        private IEnumerator CompleteAudioStop(Action onComplete)
        {
            yield return new WaitForSecondsRealtime(audioStopDuration);
            audiosToStop.Clear();
            onComplete?.Invoke();
        }

        public void OnSceneLoadOver(Action onComplete)
        {
            onComplete?.Invoke();
        }
    }
}