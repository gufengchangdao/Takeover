using System;

namespace GameFramework.Hot
{
    public abstract class BehaviorNode
    {
        public virtual string Name
        {
            get
            {
                return GetType().Name;
            }
        }
        public EBehaviorStatus Status { get; private set; }

        public bool IsTerminated => IsSuccess || IsFailure;//是否运行结束
        public bool IsSuccess => Status == EBehaviorStatus.Success;//是否成功
        public bool IsFailure => Status == EBehaviorStatus.Failure;//是否失败
        public bool IsRunning => Status == EBehaviorStatus.Running;//是否正在运行

        public BehaviorNode Parent { get; set; }

        public BehaviorNode()
        {
            Status = EBehaviorStatus.Ready;
        }

        //当进入该节点时才会执行一次的函数，类似FSM的OnEnter
        protected virtual void OnInitialize() { }

        //该节点的运行逻辑，会时时返回执行结果的状态，类似FSM的OnUpdate
        protected abstract EBehaviorStatus OnUpdate();

        //当运行结束时才会执行一次的函数，类似FSM的OnExit
        protected virtual void OnTerminate() { }

        public EBehaviorStatus Tick()
        {
            if (!IsRunning)
                OnInitialize();
            Status = OnUpdate();
            if (!IsRunning)
                OnTerminate();
            return Status;
        }

        public virtual void Reset()
        {
            Status = EBehaviorStatus.Ready;
        }

        //强行打断该节点的运作
        public void Abort()
        {
            OnTerminate();
            Status = EBehaviorStatus.Aborted;
        }

        /// <summary>
        /// 对父节点进行一些调用
        /// </summary>
        public void DoToParents(Action<BehaviorNode> fn)
        {
            if (Parent != null)
            {
                fn(Parent);
                Parent.DoToParents(fn);
            }
        }

        public BehaviorNode GetRoot()
        {
            var node = this;
            while (node.Parent != null)
                node = node.Parent;
            return node;
        }
    }
}