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
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            GFGlobal.Event.Fire(this, SceneLoadBeginEvent.Create(sceneName, mode));
            SceneManager.LoadScene(sceneName, mode);
        }

        public void UnloadSceneAsync(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, Action<AsyncOperation> completed = null)
        {
            GFGlobal.Event.Fire(this, SceneLoadBeginEvent.Create(sceneName, mode));
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);
            if (completed != null)
                op.completed += completed;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GFGlobal.Event.Fire(this, SceneLoadEndEvent.Create(scene, mode));
        }
    }
}