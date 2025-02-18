using System.Collections;
using UnityEngine;

namespace ClumsyWizard.Utilities
{
    public class CW_LerpFloat : CW_Lerp<float>
    {
        [SerializeField] private float originalAmount;

        protected override float GetOriginalValue()
        {
            return originalAmount;
        }

        protected override float LerpValue(float startingValue, float targetValue, float ratio)
        {
            return Mathf.Lerp(startingValue, targetValue, ratio);
        }
    }
}