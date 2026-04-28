using System;

namespace GameFramework.Hot
{
    public class BTActionNode : BehaviorNode
    {
        private Action action;

        public BTActionNode(Action action)
        {
            this.action = action;
        }

        protected override EBehaviorStatus OnUpdate()
        {
            action?.Invoke();
            return EBehaviorStatus.Success;
        }
    }
}