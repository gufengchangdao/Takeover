using GameFramework.Hot;

namespace Takeover
{
    /// <summary>
    /// 城堡状态变化全局事件
    /// </summary>
    public class OnCastleStateChange : BaseGameEvent<OnCastleStateChange>
    {
        public Castle Castle { get; private set; }
        public bool ToActive { get; private set; }

        public static OnCastleStateChange Create(Castle castle, bool toActive)
        {
            var data = Create();
            data.Castle = castle;
            data.ToActive = toActive;
            return data;
        }

        public override void OnRecycle()
        {
            Castle = null;
        }
    }
}