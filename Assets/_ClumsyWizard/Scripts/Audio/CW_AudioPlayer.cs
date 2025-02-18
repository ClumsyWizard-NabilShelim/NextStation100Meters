using ClumsyWizard.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace ClumsyWizard.Audio
{
    [Serializable]
    public class AudioData
    {
        public CW_SoundDataSO Data;
        public bool Loop;
        public bool PlayOnAwake;
        public bool InstantStop;

        [Range(0.0f, 1.0f)] public float Multiplier = 1.0f;

        [HideInInspector] public AudioSource AudioSource;

        public AudioData(CW_SoundDataSO data, bool loop, bool playOnAwake, bool instantStop, float multiplier)
        {
            Data = data;
            Loop = loop;
            PlayOnAwake = playOnAwake;
            Multiplier = multiplier;
            InstantStop = instantStop;
        }
    }

    public class CW_AudioPlayer : MonoBehaviour
    {
        [SerializeField] protected CW_Dictionary<string, AudioData> audioDatas;

        private void Start()
        {
            foreach (string audioID in audioDatas.Keys)
            {
                if (audioDatas[audioID].Data == null)
                    continue;

                audioDatas[audioID].AudioSource = CreateAudioSource(audioID);

                if(audioDatas[audioID].Data.Type == SoundType.Background)
                    CW_AudioManager.Instance.AddBackgroundAudio(audioDatas[audioID].AudioSource);

                if (audioDatas[audioID].PlayOnAwake)
                    Play(audioID);
            }
        }

        public void Play(string id)
        {
            if (!audioDatas.ContainsKey(id) || audioDatas[id].Data == null)
                return;

            audioDatas[id].AudioSource.clip = audioDatas[id].Data.GetClip();
            audioDatas[id].AudioSource.Play();
        }
        public void Stop(string id)
        {
            if (!audioDatas.ContainsKey(id) || audioDatas[id].Data == null)
                return;

            if (audioDatas[id].InstantStop)
                audioDatas[id].AudioSource.Stop();
            else
                CW_AudioManager.Instance.Stop(audioDatas[id].AudioSource);
        }

        //Helper Functions
        private AudioSource CreateAudioSource(string audioID)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = CW_AudioManager.Instance.Mixers[audioDatas[audioID].Data.Type];

            source.volume = audioDatas[audioID].Data.Volume * (audioDatas[audioID].Multiplier == 0.0f ? 1.0f : audioDatas[audioID].Multiplier);

            source.loop = audioDatas[audioID].Loop;
            source.playOnAwake = audioDatas[audioID].PlayOnAwake;

            return source;
        }
    }
}