using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClumsyWizard.UI
{
    [CreateAssetMenu(menuName = "Clumsy Wizard/Utility/Canvas Layer Data")]
    public class CW_CanvasLayerDataSO : ScriptableObject
    {
        [field: SerializeField] public List<string> Layers { get; private set; }

        public int IndexOf(string layer)
        {
            if(Layers.Contains(layer)) 
                return Layers.IndexOf(layer);

            return 0;
        }

        public string ValueAt(int index)
        {
            if(index < Layers.Count)
                return Layers[index];

            return "No Value";
        }
    }
}