using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace ClumsyWizard.Core
{
    public interface ISceneLoadEvent
    {
        public void OnSceneLoadOver(Action onComplete);
        public void OnSceneLoadTriggered(Action onComplete);
    }

    public class CW_SceneManagement : CW_Persistant<CW_SceneManagement>, ISceneLoadEvent
    {
        public string CurrentLevel => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        public bool IsMenuScene => CurrentLevel.Contains("Menu");
        private bool loading;

        private CW_SceneLoadFunctionality sceneLoadFunctionality;

        //Level Load Cleanup
        private ISceneLoadEvent[] sceneLoadEvents;
        private int currentSceneLoadEventIndex;
        private string sceneToLoad = string.Empty;

        protected override void Awake()
        {
            base.Awake();
            sceneLoadFunctionality = GetComponent<CW_SceneLoadFunctionality>();
        }

        public void Reload()
        {
            Load(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        public void Load(string sceneName = "")
        {
            if (loading)
                return;

            loading = true;
            StartCoroutine(LoadScene(sceneName));
        }

        private IEnumerator LoadScene(string sceneName)
        {
            if (sceneName == "")
            {
                Debug.Log("Empty Scene Name");
            }
            else
            {
                sceneToLoad = sceneName;

                if(sceneLoadFunctionality != null)
                    sceneLoadFunctionality.OnLoadTriggered();

                yield return new WaitForSecondsRealtime(1.3f);
                CleanUpScene();
            }
        }

        //Scene load cleanup callbacks
        private void CleanUpScene()
        {
            if (sceneLoadEvents == null || sceneLoadEvents.Length == 0)
            {
                currentSceneLoadEventIndex = -1;
                sceneLoadEvents = FindObjectsOfType<MonoBehaviour>().OfType<ISceneLoadEvent>().ToArray();
            }
            else if(currentSceneLoadEventIndex >= sceneLoadEvents.Length - 1)
            {
                sceneLoadEvents = null;
                StartCoroutine(FinishLoadingScene());
                return;
            }

            currentSceneLoadEventIndex++;
            sceneLoadEvents[currentSceneLoadEventIndex].OnSceneLoadTriggered(CleanUpScene);
        }
        private IEnumerator FinishLoadingScene()
        {
            yield return null;

            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneToLoad);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                if (sceneLoadFunctionality != null)
                    sceneLoadFunctionality.LoadingProgress(Mathf.Clamp01(operation.progress / 0.9f));

                if (operation.progress >= 0.9f)
                {
                    yield return new WaitForEndOfFrame();

                    operation.allowSceneActivation = true;
                    InitializeScene();
                }

                yield return null;
            }
        }

        //Scene load compleete callbacks
        private void InitializeScene()
        {
            if (sceneLoadEvents == null || sceneLoadEvents.Length == 0)
            {
                currentSceneLoadEventIndex = -1;
                sceneLoadEvents = FindObjectsOfType<MonoBehaviour>().OfType<ISceneLoadEvent>().ToArray();
            }
            else if (currentSceneLoadEventIndex >= sceneLoadEvents.Length - 1)
            {
                sceneLoadEvents = null;
                loading = false;

                if (sceneLoadFunctionality != null)
                    sceneLoadFunctionality.OnLoadingOver();

                return;
            }

            currentSceneLoadEventIndex++;
            sceneLoadEvents[currentSceneLoadEventIndex].OnSceneLoadOver(InitializeScene);
        }

        //Clean up
        public void OnSceneLoadOver(Action onComplete)
        {
            onComplete?.Invoke();
        }

        public void OnSceneLoadTriggered(Action onComplete)
        {
            onComplete?.Invoke();
        }
    }
}