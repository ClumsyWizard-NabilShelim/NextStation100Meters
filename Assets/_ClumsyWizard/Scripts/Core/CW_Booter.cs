using UnityEngine;

namespace ClumsyWizard.Core
{
    public static class CW_Booter
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ExecuteAfter()
        {
            Object.Instantiate(Resources.Load("PersistantCore"));
        }

    }
}