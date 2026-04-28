namespace GameFramework.Hot
{
    public class BTIdleNode : BehaviorNode
    {
        protected override EBehaviorStatus OnUpdate()
        {
            return EBehaviorStatus.Running;
        }
    }
}