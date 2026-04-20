using UnityEngine;

/// <summary>
/// 从给定的状态里随机选一个循环播放
/// </summary>
[RequireComponent(typeof(Animator))]
public class RandomAnimationPlay : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private string[] loopStates;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.keepAnimatorStateOnDisable = true;
    }

    void Start()
    {
        PlayRandomAnimation();
    }

    public void PlayRandomAnimation()
    {
        if (loopStates == null || loopStates.Length == 0)
            return;

        var state = loopStates[Random.Range(0, loopStates.Length)];
        animator.Play(state);
    }
}
