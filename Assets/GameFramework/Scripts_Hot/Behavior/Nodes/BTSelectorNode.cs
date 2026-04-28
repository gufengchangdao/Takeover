namespace GameFramework.Hot
{
    /// <summary>
    /// 选择器
    /// </summary>
    public class BTSelectorNode : BTSequenceNode
    {
        protected override EBehaviorStatus OnUpdate()
        {
            if (children.Count == 0)
                return EBehaviorStatus.Failure;

            while (true)
            {
                var s = children[currentIndex].Tick();
                if (s != EBehaviorStatus.Failure)
                    return s;

                currentIndex++;
                if (currentIndex >= children.Count)
                    return EBehaviorStatus.Failure;
            }
        }
    }
}