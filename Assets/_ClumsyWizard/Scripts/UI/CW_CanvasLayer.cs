using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClumsyWizard.UI
{
    public class CW_CanvasLayer : MonoBehaviour
    {
        [HideInInspector] public int Layer;
        [HideInInspector] public string LayerName;

        private void Start()
        {
            UpdateCanvas();
        }

        public void UpdateCanvas()
        {
            GetComponent<Canvas>().sortingOrder = Layer;
        }
    }
}