using System;

namespace GameFramework.Hot
{
    /// <summary>
    /// 事件节点
    /// 可以通知BTPrioritySelectorNode重新评估
    /// </summary>
    public class BTEventNode : BTDecoratorNode
    {
        private bool triggered;
        private readonly Action unsubscribe;

        /// var node = new BTEventNode(handler =>
        /// {
        ///     someObject.OnSomething += handler;
        ///     return () => someObject.OnSomething -= handler;
        /// });
        public BTEventNode(Func<Action, Action> subscribe)
        {
            unsubscribe = subscribe?.Invoke(OnEvent);
        }

        private void OnEvent()
        {
            if (Status == EBehaviorStatus.Running)
                child.Reset();

            triggered = true;

            DoToParents(ResetPriorityNode);
        }

        private void ResetPriorityNode(BehaviorNode node)
        {
            if (node is BTPrioritySelectorNode priorityNode)
                priorityNode.RequestReevaluation(); //重新评估节点
        }

        public override void Reset()
        {
            base.Reset();
            triggered = false;
        }

        public void Unsubscribe()
        {
            unsubscribe?.Invoke();
        }

        protected override EBehaviorStatus OnUpdate()
        {
            var s = Status;
            if (Status == EBehaviorStatus.Ready && triggered)
                s = EBehaviorStatus.Running;

            if (s == EBehaviorStatus.Running)
            {
                triggered = false;
                if (child != null)
                    return child.Tick();
                else
                    return EBehaviorStatus.Failure;
            }

            return Status;
        }
    }
}
