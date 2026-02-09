using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class TopToolbar
{
    static TopToolbar()
    {
        ToolbarExtend.OnLeftToolbarGUI -= DrawLeftBtns;
        ToolbarExtend.OnLeftToolbarGUI += DrawLeftBtns;
    }

    private static void DrawLeftBtns()
    {
        if (GUILayout.Button(new GUIContent("运行游戏", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            // 如果有未保存的场景修改，先提示保存
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            // 打开启动场景（注意：这是编辑器打开场景，不是运行时LoadScene）
            EditorSceneManager.OpenScene("Assets/Scenes/Launch.unity");

            // 进入播放模式（真正启动游戏）
            if (!EditorApplication.isPlaying)
                EditorApplication.isPlaying = true;
        }
    }
}