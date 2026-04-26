using GameFramework.Hot;

namespace Takeover
{
    public static class UnitStates
    {
        public class Idle : FsmState<Unit>
        {

        }

        public class Move : FsmState<Unit>
        {
            protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
                base.OnUpdate(elapseSeconds, realElapseSeconds);
                Owner.UpdateStateGotoXY(elapseSeconds);
            }
        }
    }
}