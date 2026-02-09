using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Animator), true)]
public partial class AnimatorEditor : DecoratorEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        var animator = (Animator)target;
        CreateAnimatiorControl(animator);
        ShowPanelAnimationButton(animator);
    }

    private void CreateAnimatiorControl(Animator animator)
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
}