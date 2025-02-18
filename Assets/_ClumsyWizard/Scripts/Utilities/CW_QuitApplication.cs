using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClumsyWizard.UI
{
    public class CW_QuitApplication : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<CW_Button>().AddClickEvent(LoadLevel);
        }

        private void LoadLevel()
        {
            Application.Quit();
        }
    }
}