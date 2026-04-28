using GameFramework.AOT;

namespace GameFramework.Hot
{
    public class BTDebugNodeNode : BehaviorNode
    {
        private string word;
        public BTDebugNodeNode(string word)
        {
            this.word = word;
        }
        protected override EBehaviorStatus OnUpdate()
        {
            Log.Info(word);
            return EBehaviorStatus.Success;
        }
    }
}