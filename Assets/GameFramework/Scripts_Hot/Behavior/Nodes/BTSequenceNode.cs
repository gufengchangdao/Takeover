namespace GameFramework.Hot
{
    /// <summary>
    /// 顺序器
    /// </summary>
    public class BTSequenceNode : BTCompositeNode
    {
        protected int currentIndex = -1;//当前运行的子节点

        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (children.Count > 0)
                currentIndex = 0;//从第一个子节点开始
        }

        protected override EBehaviorStatus OnUpdate()
        {
            if (children.Count == 0)
                return EBehaviorStatus.Success;

            while (true)
            {
                var s = children[currentIndex].Tick();
                if (s != EBehaviorStatus.Success)
                    return s;

                //如果运行成功，就换到下一个子节点
                currentIndex++;
                if (currentIndex >= children.Count) //如果全都成功运行完了，就返回「成功」
                    return EBehaviorStatus.Success;
            }
        }
    }
}