using System.Collections;
using UnityEngine;

namespace ClumsyWizard.Core
{
    public abstract class CW_SceneLoadFunctionality : MonoBehaviour
    {
        public abstract void OnLoadTriggered();
        public abstract void LoadingProgress(float progress);
        public abstract void OnLoadingOver();
    }
}