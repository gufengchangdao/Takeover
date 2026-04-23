using GameFramework.Hot;

namespace Takeover
{
    public class GameSpeedChangeEvent : BaseGameEvent<GameSpeedChangeEvent>
    {
        public int GameSpeed { get; private set; }

        public static GameSpeedChangeEvent Create(int speed)
        {
            var data = Create();
            data.GameSpeed = speed;
            return data;
        }

        public override void OnRecycle()
        {
        }
    }
}