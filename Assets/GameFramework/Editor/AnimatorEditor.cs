using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(Animator), true)]
public partial class AnimatorEditor : DecoratorEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var animator = (Animator)target;
        CreateAnimatorControl(animator);
        CreateAnimatorClip(animator);
        ShowPanelAnimationButton(animator);
    }

    private void CreateAnimatorControl(Animator animator)
    {
        if (animator.runtimeAnimatorController != null)
            return;

        if (!GUILayout.Button("创建动画控制器"))
            return;

        // 获取Animator所在的GameObject名称作为默认文件名
        string defaultName = animator.gameObject.name + "Controller";
        string message = "选择动画控制器的保存位置";

        // 打开保存文件对话框
        string path = EditorUtility.SaveFilePanelInProject("新建动画控制器", defaultName, "controller", message);

        // 如果用户取消了选择，直接返回
        if (string.IsNullOrEmpty(path))
            return;

        // 创建新的Animator Controller
        var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(path);

        // 将创建的控制器赋值给Animator组件
        animator.runtimeAnimatorController = controller;

        // 标记场景和资源为已修改
        EditorUtility.SetDirty(animator);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"动画控制器已创建: {path}");
    }

    private void CreateAnimatorClip(Animator animator)
    {
        if (animator.runtimeAnimatorController == null)
            return;

        if (!GUILayout.Button("创建Clip"))
            return;

        if (animator.runtimeAnimatorController is not AnimatorController controller)
        {
            Debug.LogWarning("当前 AnimatorController 不是可编辑的 AnimatorController，无法添加 Clip。");
            return;
        }

        string controllerPath = AssetDatabase.GetAssetPath(controller);
        string defaultDirectory = string.IsNullOrEmpty(controllerPath) ? "Assets" : Path.GetDirectoryName(controllerPath);
        string defaultName = animator.gameObject.name;
        string message = "选择动画 Clip 的保存位置";
        string path = EditorUtility.SaveFilePanelInProject("新建动画 Clip", defaultName, "anim", message, defaultDirectory);

        if (string.IsNullOrEmpty(path))
            return;

        AnimationClip clip = new();
        clip.name = Path.GetFileNameWithoutExtension(path);
        AssetDatabase.CreateAsset(clip, path);
        // controller.AddMotion(clip);
        var newState = controller.layers[0].stateMachine.AddState(clip.name);
        newState.motion = clip;

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"动画 Clip 已创建并添加到 Controller: {path}");
    }
}