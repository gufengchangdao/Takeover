namespace GameFramework.Hot
{
    public class BehaviorTree
    {
        private BehaviorNode root;
        public bool HaveRoot => root != null;

        public bool IsTerminated => IsSuccess || IsFailure;//是否运行结束
        public bool IsSuccess => root.IsSuccess;//是否成功
        public bool IsFailure => root.IsFailure;//是否失败
        public bool IsRunning => root.IsRunning;//是否正在运行

        public BehaviorTree(BehaviorNode root)
        {
            this.root = root;
        }

        public void Tick()
        {
            root.Tick();
        }

        public void SetRoot(BehaviorNode root)
        {
            this.root = root;
        }
    }
}