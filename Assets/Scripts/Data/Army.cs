using GameFramework.Hot;
using TableStructure;

namespace Takeover
{
    /// <summary>
    /// 一组士兵
    /// </summary>
    public class Army
    {
        public ECamp Camp { get; private set; }
        public int Upkeep { get; private set; }
        /// <summary>
        /// 当前驻扎的城堡
        /// </summary>
        public Castle CurCastle { get; private set; }

        public bool AtCastle => CurCastle == null;

        private Fsm<Army> fsm;

        public Army()
        {
            fsm = Fsm<Army>.Create("Army", this,
            new ArmyStates.Idle(),
            new ArmyStates.Move(),
            new ArmyStates.Attack(),
            new ArmyStates.Dead());
        }

        /// <summary>
        /// 让士兵前往该目标
        /// </summary>
        public void CommandGotoTarget(Castle castle)
        {

        }
    }
}