using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClumsyWizard.Utilities
{
    public enum LerpType
    {
        Once,
        PingPong,
        OnceAndReturn
    }

    public abstract class CW_Lerp<TValue> : MonoBehaviour
    {
        [SerializeField] private LerpType lerpType;

        [SerializeField] private float lerpTime;
        private float currentTime;

        private TValue originalValue;
        private TValue startingValue;
        private TValue targetValue;

        private bool lerp;
        private bool goToOriginal;

        private Action overCallback;
        private Action<TValue> lerpCallback;

        private void Start()
        {
            originalValue = GetOriginalValue();
        }

        protected abstract TValue GetOriginalValue();

        public void Animate(TValue startingValue, TValue targetValue, Action<TValue> lerpCallback, Action overCallback)
        {
            this.startingValue = startingValue;
            this.targetValue = targetValue;

            currentTime = 0.0f;
            goToOriginal = false;

            this.overCallback = overCallback;
            this.lerpCallback = lerpCallback;

            lerp = true;
        }

        private void Update()
        {
            if (!lerp)
                return;

            if (currentTime > lerpTime)
            {
                lerp = false;

                if (lerpType == LerpType.PingPong)
                {
                    if (goToOriginal)
                        Animate(originalValue, targetValue, lerpCallback, null);
                    else
                        Animate(targetValue, originalValue, lerpCallback, null);

                    goToOriginal = !goToOriginal;
                }
                else if(lerpType == LerpType.OnceAndReturn)
                {
                    if (!goToOriginal)
                        Animate(targetValue, originalValue, lerpCallback, overCallback);
                    else
                        overCallback?.Invoke();

                    goToOriginal = true;
                }
                else
                {
                    overCallback?.Invoke();
                }

                return;
            }

            currentTime += Time.deltaTime;

            lerpCallback?.Invoke(LerpValue(startingValue, targetValue, currentTime / lerpTime));
        }

        protected abstract TValue LerpValue(TValue startingValue, TValue targetValue, float ratio);
    }
}