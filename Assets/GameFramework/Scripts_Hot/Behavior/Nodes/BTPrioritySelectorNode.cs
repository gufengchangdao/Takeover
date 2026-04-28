using UnityEngine;

namespace GameFramework.Hot
{
    /// <summary>
    /// 带有周期评估机制的Select
    /// 从饥荒那扒过来的
    /// </summary>
    public class BTPrioritySelectorNode : BTCompositeNode
    {
        protected int currentIndex = -1;//当前运行的子节点
        private float period;
        private float lasttime;

        public BTPrioritySelectorNode(float period = 1, bool noscatter = false)
        {
            this.period = period;
            if (!noscatter)
                lasttime = Time.time + period * (0.5f + Random.Range(0f, 1f));
            else
                lasttime = -period;
        }

        public void RequestReevaluation()
        {
            lasttime = -period;
        }

        protected override EBehaviorStatus OnUpdate()
        {
            EBehaviorStatus res = EBehaviorStatus.Failure;
            float time = Time.time;
            bool doEval = lasttime + period < time;

            if (doEval)
            {
                // 重新评估
                lasttime = time;
                currentIndex = -1;
                for (int i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    if (child.Status == EBehaviorStatus.Success
                                              || child.Status == EBehaviorStatus.Failure
                                              || child.Status == EBehaviorStatus.Aborted)
                        child.Reset();

                    if (currentIndex == -1)
                    {
                        var cs = child.Tick();
                        if (cs == EBehaviorStatus.Success || cs == EBehaviorStatus.Running)
                        {
                            res = cs;
                            currentIndex = i;
                        }
                    }

                    if (currentIndex != i && child.Status == EBehaviorStatus.Running)
                        child.Abort();
                }

                if (currentIndex == -1)
                    return EBehaviorStatus.Failure;
            }
            else
            {
                if (currentIndex != -1)
                {
                    var node = children[currentIndex];
                    if (node.Status == EBehaviorStatus.Running)
                        node.Tick();
                    res = node.Status;
                    if (res != EBehaviorStatus.Running)
                        lasttime = -period; //当前行为一旦结束，下次父节点再访问时会立刻重新选优先级
                }
            }

            return res;
        }
    }
}