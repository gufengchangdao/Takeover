using System.Collections.Generic;
using GameFramework.AOT;

namespace GameFramework.Hot
{
    public partial class BehaviorTreeBuilder
    {
        private readonly Stack<BehaviorNode> nodeStack;//构建树结构用的栈
        private readonly BehaviorTree bhTree;//构建的树

        public BehaviorTreeBuilder()
        {
            bhTree = new BehaviorTree(null);//构造一个没有根的树
            nodeStack = new Stack<BehaviorNode>();//初始化构建栈
        }

        // 不直接调用这个，用封装的其他方法构建
        public BehaviorTreeBuilder AddNode(BehaviorNode behavior)
        {
            if (bhTree.HaveRoot)//有根节点时，加入构建栈
            {
                var node = nodeStack.Peek();
                if (node is BTCompositeNode compositeNode)
                    compositeNode.AddChild(behavior);
                else if (node is BTDecoratorNode decoratorNode)
                    decoratorNode.SetChild(behavior);
                else
                    Log.Error($"[BehaviorTree] {node}  does not support setting child nodes.");
            }
            else //当树没根时，新增得节点视为根节点
            {
                bhTree.SetRoot(behavior);
            }

            //只有组合节点和修饰节点需要进构建堆
            if (behavior is BTCompositeNode || behavior is BTDecoratorNode)
            {
                nodeStack.Push(behavior);
            }

            return this;
        }

        public BehaviorTreeBuilder Back()
        {
            nodeStack.Pop();
            return this;
        }

        public BehaviorTree End()
        {
            nodeStack.Clear();
            return bhTree;
        }
    }
}