using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFramework.Hot
{
    public class GFScene : GFBaseModule
    {
        void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            GFGlobal.Event.Fire(this, SceneLoadBeginEvent.Create(sceneName, mode));
            SceneManager.LoadScene(sceneName, mode);
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, Action<AsyncOperation> completed = null)
        {
            GFGlobal.Event.Fire(this, SceneLoadBeginEvent.Create(sceneName, mode));
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);
            if (completed != null)
                op.completed += completed;
        }

        public void LoadSceneByPackage(string assetPath)
        {
            GFGlobal.Resource.LoadSceneSync(assetPath);
        }

        public void UnloadSceneAsync(string sceneName)
        {
            GFGlobal.Event.Fire(this, SceneUnloadBeginEvent.Create(sceneName));
            SceneManager.UnloadSceneAsync(sceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GFGlobal.Event.Fire(this, SceneLoadEndEvent.Create(scene, mode));
        }

        private void OnSceneUnloaded(Scene scene)
        {
            GFGlobal.Event.Fire(this, SceneUnloadEndEvent.Create(scene));
        }
    }
}