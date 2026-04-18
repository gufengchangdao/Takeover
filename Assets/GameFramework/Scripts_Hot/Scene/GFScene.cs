using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFramework.Hot
{
    public class GFScene : GFBaseModule
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneAsync(string sceneName, Action<AsyncOperation> completed)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.completed += completed;
        }
    }
}