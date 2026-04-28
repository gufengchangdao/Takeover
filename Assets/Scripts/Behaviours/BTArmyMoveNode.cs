using GameFramework.AOT;
using GameFramework.Hot;

namespace Takeover
{
    public class BTArmyMoveNode : BehaviorNode
    {
        private Army army;

        public BTArmyMoveNode(Army army)
        {
            this.army = army;
        }

        protected override void OnInitialize()
        {
            Log.Debug("开始移动，移动节点{0}", string.Join(",", army.CurPathList));
            army.SendToNextNode();
        }

        protected override EBehaviorStatus OnUpdate()
        {
            //先移动主单位
            if (army.GetMainUnit().AtTarget)
                army.SendToNextNode();

            //根据主单位计算其他单位站位
            army.RepairFormation();

            if (army.WantToMove)
                return EBehaviorStatus.Running;
            return EBehaviorStatus.Success;
        }
    }
}