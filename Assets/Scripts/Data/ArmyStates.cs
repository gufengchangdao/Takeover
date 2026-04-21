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

        }

        public class Attack : FsmState<Army>
        {

        }

        // 所有士兵死亡
        public class Dead : FsmState<Army>
        {

        }
    }
}