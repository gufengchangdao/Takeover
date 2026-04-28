namespace GameFramework.Hot
{
    /// <summary>
    /// 修饰节点
    /// </summary>
    public abstract class BTDecoratorNode : BehaviorNode
    {
        protected BehaviorNode child;

        public virtual void SetChild(BehaviorNode child)
        {
            child.Parent = this;
            this.child = child;
        }

        public override void Reset()
        {
            base.Reset();
            child?.Reset();
        }
    }
}