using GameFramework.AOT;
using GameFramework.Hot;

namespace Takeover
{
    public class ArmyStates
    {
        public class Idle : FsmState<Army>
        {

        }

        public class Move : FsmState<Army>
        {
            protected override void OnEnter(object userData)
            {
                Log.Debug("开始移动，移动节点{0}", string.Join(",", Owner.CurPathList));

                Owner.SendToNextNode();
            }

            protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
                //先移动主单位
                if (Owner.GetMainUnit().AtTarget)
                    Owner.SendToNextNode();

                //根据主单位计算其他单位站位
                Owner.RepairFormation();
            }
        }

        public class Attack : FsmState<Army>
        {

        }
    }
}