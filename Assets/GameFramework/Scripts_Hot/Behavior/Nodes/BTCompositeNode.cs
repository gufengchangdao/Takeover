using System.Collections.Generic;

namespace GameFramework.Hot
{
    /// <summary>
    /// 组合节点
    /// </summary>
    public abstract class BTCompositeNode : BehaviorNode
    {
        protected List<BehaviorNode> children = new();

        //移除指定子节点
        public virtual void RemoveChild(BehaviorNode child)
        {
            if (child.Parent == this)
                child.Parent = null;
            children.Remove(child);
        }

        public void ClearChildren()//清空子节点列表
        {
            foreach (var child in children)
                child.Parent = null;
            children.Clear();
        }

        public virtual void AddChild(BehaviorNode child)//添加子节点
        {
            children.Add(child);
            child.Parent = this;
        }

        public override void Reset()
        {
            base.Reset();
            foreach (var node in children)
                node.Reset();
        }
    }
}