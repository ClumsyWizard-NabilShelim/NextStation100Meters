using ClumsyWizard.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClumsyWizard.UI
{
    public class CW_LoadTargetScene : MonoBehaviour
    {
        [SerializeField] private string targetLevel;

        private void Start()
        {
            GetComponent<CW_Button>().AddClickEvent(LoadLevel);
        }

        private void LoadLevel()
        {
            CW_SceneManagement.Instance.Load(targetLevel);
        }
    }
}