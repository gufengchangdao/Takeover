using GameFramework.Hot;

namespace Takeover
{
    public class OnBuildingStateChange : BaseGameEvent<OnBuildingStateChange>
    {
        public Building Building { get; private set; }
        public bool ToActive { get; private set; }

        public static OnBuildingStateChange Create(Building building, bool toActive)
        {
            var data = Create();
            data.Building = building;
            data.ToActive = toActive;
            return data;
        }

        public override void OnRecycle()
        {
            Building = null;
        }
    }
}