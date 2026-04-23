using GameFramework.Hot;

namespace Takeover
{
    public class LevelControl : BaseControl
    {
        public void ChangeGameSpeed()
        {
            Global.LevelLogic.GameSpeed = Global.LevelLogic.GameSpeed == 1 ? 2 : 1;
        }
    }
}