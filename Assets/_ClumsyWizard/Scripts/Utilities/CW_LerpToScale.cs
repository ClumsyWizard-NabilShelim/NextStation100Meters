using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClumsyWizard.Utilities
{
    public class CW_LerpToScale : CW_Lerp<Vector3>
    {
        protected override Vector3 LerpValue(Vector3 startingValue, Vector3 targetValue, float ratio)
        {
            return Vector3.Lerp(startingValue, targetValue, ratio);
        }

        protected override Vector3 GetOriginalValue()
        {
            return transform.localScale;
        }
    }
}