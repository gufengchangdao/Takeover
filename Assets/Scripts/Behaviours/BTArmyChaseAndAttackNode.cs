using GameFramework.Hot;

namespace Takeover
{
    public class BTArmyChaseAndAttackNode : BehaviorNode
    {
        private Army army;

        public BTArmyChaseAndAttackNode(Army army)
        {
            this.army = army;
        }

        protected override EBehaviorStatus OnUpdate()
        {
            return EBehaviorStatus.Failure;
        }
    }
}