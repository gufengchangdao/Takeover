using GameFramework.Hot;

namespace Takeover
{
    public static class UnitStates
    {
        public class Idle : FsmState<Unit>
        {
            protected override void OnEnter(object userData)
            {
                base.OnEnter(userData);
                Owner.PlayAnimation("Idle");
            }
        }

        public class Move : FsmState<Unit>
        {
            protected override void OnEnter(object userData)
            {
                base.OnEnter(userData);
                Owner.PlayAnimation("Move");
            }

            protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
                base.OnUpdate(elapseSeconds, realElapseSeconds);
                Owner.UpdateStateGotoXY(elapseSeconds);
            }
        }
    }
}