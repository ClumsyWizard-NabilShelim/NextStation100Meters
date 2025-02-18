using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClumsyWizard.Gameplay
{
    [ExecuteInEditMode]
    public class CW_SameParticleSeedGenerator : MonoBehaviour
    {
        public ParticleSystem[] setSeedParticles;
        public int ParticleSeed;
        public bool forceNewSeed;

        private void Awake()
        {
            GenerateSeed();
            for (int i = 0; i < setSeedParticles.Length; i++)
            {
                setSeedParticles[i].Play();
            }

            Destroy(gameObject, setSeedParticles[0].main.duration);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (forceNewSeed)
            {
                GenerateSeed();
                forceNewSeed = false;
            }
        }
#endif

        private void GenerateSeed()
        {
            ParticleSeed = Random.Range(0, int.MaxValue);

            if (setSeedParticles.Length > 0)
            {
                for (int i = 0; i < setSeedParticles.Length; i++)
                {
                    setSeedParticles[i].randomSeed = (uint)ParticleSeed;
                }
            }
        }
    }
}