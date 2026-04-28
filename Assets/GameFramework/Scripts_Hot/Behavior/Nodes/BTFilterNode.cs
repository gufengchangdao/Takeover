namespace GameFramework.Hot
{
    /// <summary>
    /// 过滤器
    /// </summary>
    public class BTFilterNode : BTSequenceNode
    {
        public void AddCondition(BehaviorNode condition)//添加条件，就用头插入
        {
            children.Insert(0, condition);
        }
        public void AddAction(BehaviorNode action)//添加动作，就用尾插入
        {
            children.Add(action);
        }
    }
}