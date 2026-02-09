using System;

namespace GameFramework.Hot
{
    public class OnPanelOpenEvent : BaseGameEvent<OnPanelOpenEvent>
    {
        public Type ControlType { get; private set; }

        public static OnPanelOpenEvent Create(Type controlType)
        {
            var e = Create();
            e.ControlType = controlType;
            return e;
        }

        public override void OnRecycle()
        {
            ControlType = null;
        }
    }
}