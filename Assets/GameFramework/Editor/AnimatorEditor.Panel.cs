using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public partial class AnimatorEditor
{
    private void ShowPanelAnimationButton(Animator animator)
    {
        if (animator.transform is not RectTransform)
            return;

        if (animator.runtimeAnimatorController != null)
            return;

        if (!GUILayout.Button("创建UI动画"))
            return;

        string defaultName = animator.name;
        var message = string.Format("Create a new animator for the game object '{0}':", defaultName);
        string path = EditorUtility.SaveFilePanelInProject("New Animation Contoller", defaultName, "controller", message);

        if (string.IsNullOrEmpty(path))
            return;

        ClearFile(path);

        var controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        var directory = Path.GetDirectoryName(path);
        var stateIn = CreateClip(controller, directory, "PanelIn", false);
        var stateLoop = CreateClip(controller, directory, "PanelLoop", true);
        CreateClip(controller, directory, "PanelOut", false);
        TransitionState(stateIn, stateLoop);

        animator.runtimeAnimatorController = controller;

        AssetDatabase.Refresh();
    }

    private void ClearFile(string path)
    {
        if (File.Exists(path))
            AssetDatabase.DeleteAsset(path);

        // 确保目录存在
        var directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    private AnimatorState CreateClip(AnimatorController controller, string dir, string name, bool isLoop)
    {
        AnimationClip clip = new();
        clip.name = name;
        string path = Path.Combine(dir, $"{name}.anim");
        ClearFile(path);
        AssetDatabase.CreateAsset(clip, path);
        var clipSetting = AnimationUtility.GetAnimationClipSettings(clip);
        clipSetting.loopTime = isLoop;
        AnimationUtility.SetAnimationClipSettings(clip, clipSetting);

        return controller.AddMotion(clip);
    }

    private void TransitionState(AnimatorState source, AnimatorState destination)
    {
        var transition = source.AddTransition(destination);
        transition.exitTime = 1;
        transition.duration = 0;
        transition.hasExitTime = true;
    }
}