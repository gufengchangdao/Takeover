namespace GameFramework.Hot
{
    public partial class BehaviorTreeBuilder
    {
        public BehaviorTreeBuilder Sequence()
        {
            var tp = new BTSequenceNode();
            AddNode(tp);
            return this;
        }
        public BehaviorTreeBuilder Seletctor()
        {
            var tp = new BTSelectorNode();
            AddNode(tp);
            return this;
        }
        public BehaviorTreeBuilder Filter()
        {
            var tp = new BTFilterNode();
            AddNode(tp);
            return this;
        }
        public BehaviorTreeBuilder Parallel(BTParallelNode.Policy success, BTParallelNode.Policy failure)
        {
            var tp = new BTParallelNode(success, failure);
            AddNode(tp);
            return this;
        }
        public BehaviorTreeBuilder Monitor(BTParallelNode.Policy success, BTParallelNode.Policy failure)
        {
            var tp = new BTMonitorNode(success, failure);
            AddNode(tp);
            return this;
        }
        public BehaviorTreeBuilder ActiveSelector()
        {
            var tp = new BTActiveSelectorNode();
            AddNode(tp);
            return this;
        }
        public BehaviorTreeBuilder Repeat(int limit)
        {
            var tp = new BTRepeatNode(limit);
            AddNode(tp);
            return this;
        }
        public BehaviorTreeBuilder Inverter()
        {
            var tp = new BTInverterNode();
            AddNode(tp);
            return this;
        }

        public BehaviorTreeBuilder DebugNode(string word)
        {
            var node = new BTDebugNodeNode(word);
            AddNode(node);
            return this;
        }

        public BehaviorTreeBuilder PrioritySelector(float period = 1, bool noscatter = false)
        {
            AddNode(new BTPrioritySelectorNode(period, noscatter));
            return this;
        }
    }
}