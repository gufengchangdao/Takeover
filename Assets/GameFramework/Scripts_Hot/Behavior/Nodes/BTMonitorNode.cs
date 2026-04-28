namespace GameFramework.Hot
{
    /// <summary>
    /// 监视器
    /// </summary>
    public class BTMonitorNode : BTParallelNode
    {
        public BTMonitorNode(Policy mSuccessPolicy, Policy mFailurePolicy)
        : base(mSuccessPolicy, mFailurePolicy)
        {
        }
        public void AddCondition(BehaviorNode condition)
        {
            children.Insert(0, condition);
        }
        public void AddAction(BehaviorNode action)
        {
            children.Add(action);
        }
    }
}