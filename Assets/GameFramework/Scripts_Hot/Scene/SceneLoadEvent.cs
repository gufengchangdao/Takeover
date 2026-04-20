using UnityEngine.SceneManagement;

namespace GameFramework.Hot
{
    public class SceneLoadBeginEvent : BaseGameEvent<SceneLoadBeginEvent>
    {
        public string SceneName { get; private set; }
        public LoadSceneMode Mode { get; private set; }

        public static SceneLoadBeginEvent Create(string sceneName, LoadSceneMode mode)
        {
            var data = Create();
            data.SceneName = sceneName;
            data.Mode = mode;
            return data;
        }
        public override void OnRecycle()
        {
            SceneName = null;
            Mode = default;
        }
    }

    public class SceneLoadEndEvent : BaseGameEvent<SceneLoadEndEvent>
    {
        public Scene Scene { get; private set; }
        public LoadSceneMode Mode { get; private set; }

        public static SceneLoadEndEvent Create(Scene scene, LoadSceneMode mode)
        {
            var data = Create();
            data.Scene = scene;
            data.Mode = mode;
            return data;
        }
        public override void OnRecycle()
        {
            Scene = default;
            Mode = default;
        }
    }
}