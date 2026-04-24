using System.Collections.Generic;
using GameFramework.Hot;
using TableStructure;

namespace Takeover
{
    /// <summary>
    /// 一组士兵
    /// </summary>
    public partial class Army
    {
        public string TableId { get; private set; }

        public ECamp Camp { get; private set; }
        /// <summary>
        /// 当前驻扎的城堡
        /// </summary>
        public Castle CurCastle { get; private set; }

        public bool AtCastle => CurCastle == null;

        /// <summary>
        /// 所有单位
        /// </summary>
        public List<Unit> Units { get; private set; } = new();

        public ArmyHealthBar HealthBar { get; private set; }

        private Fsm<Army> fsm;

        public Army(ECamp camp)
        {
            this.Camp = camp;

            fsm = Fsm<Army>.Create("Army", this,
            new ArmyStates.Idle(),
            new ArmyStates.Move(),
            new ArmyStates.Attack(),
            new ArmyStates.Dead());
        }

        public void OnEnterCastle(Castle castle)
        {
            CurCastle = castle;
        }

        /// <summary>
        /// 让士兵前往该目标
        /// </summary>
        public void CommandGotoTarget(Castle castle)
        {

        }
    }
}