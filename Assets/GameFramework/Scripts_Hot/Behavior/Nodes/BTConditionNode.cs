using System;

namespace GameFramework.Hot
{
    public class BTConditionNode : BehaviorNode
    {
        private Func<bool> fn;

        public BTConditionNode(Func<bool> fn)
        {
            this.fn = fn;
        }

        protected override EBehaviorStatus OnUpdate()
        {
            if (fn())
                return EBehaviorStatus.Success;
            else
                return EBehaviorStatus.Failure;
        }
    }
}