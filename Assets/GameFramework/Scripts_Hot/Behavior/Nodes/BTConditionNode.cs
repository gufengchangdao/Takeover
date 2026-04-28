using System;

namespace GameFramework.Hot
{
    public class BTConditionNode : BehaviorNode
    {
        protected Func<bool> fn;

        protected BTConditionNode() { }

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