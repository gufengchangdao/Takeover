namespace GameFramework.Hot
{
    /// <summary>
    /// 取反器
    /// </summary>
    public class BTInverterNode : BTDecoratorNode
    {
        protected override EBehaviorStatus OnUpdate()
        {
            child.Tick();
            if (child.IsFailure)
                return EBehaviorStatus.Success;
            if (child.IsSuccess)
                return EBehaviorStatus.Failure;
            return EBehaviorStatus.Running;
        }
    }
}