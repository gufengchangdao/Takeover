using UnityEngine;

public static class GFAnimatorExtension
{
    public static float GFGetClipLength(this Animator animator, string name)
    {
        var controller = animator.runtimeAnimatorController;
        if (controller == null)
            return 0;

        foreach (var clip in controller.animationClips)
            if (name == clip.name)
                return clip.length;

        return 0;
    }

    public static bool GFIsInState(this Animator animator, string name, int layer = 0)
    {
        var info = animator.GetCurrentAnimatorStateInfo(layer);
        return info.shortNameHash == Animator.StringToHash(name);
    }

    public static bool GFHasParameter(this Animator animator, string name)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
            if (param.name == name)
                return true;
        return false;
    }

    /// <summary>
    /// 播放动画，要求state的名字和clip名一致，只有view上有该动画时才会播放所有子节点动画
    /// </summary>
    /// <param name="anim"></param>
    /// <returns>view动画的时长</returns>
    public static float GFPlayUIAnimation(this Animator rootAnimator, string anim, bool includeChild)
    {
        if (rootAnimator.runtimeAnimatorController == null)
            return 0;

        float animLen = rootAnimator.GFGetClipLength(anim);
        if (Mathf.Approximately(animLen, 0))
            return 0;

        if (!includeChild)
        {
            rootAnimator.Play(anim, -1, 0);
            return animLen;
        }

        // 播放所有子节点动画
        foreach (var animator in rootAnimator.GetComponentsInChildren<Animator>())
        {
            var controller = animator.runtimeAnimatorController;
            if (controller == null)
                continue;

            foreach (var clip in controller.animationClips)
                if (clip.name == anim)
                    animator.Play(anim, -1, 0);
        }
        return animLen;
    }
}