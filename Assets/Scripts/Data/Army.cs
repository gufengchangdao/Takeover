using System.Collections.Generic;
using GameFramework.Hot;
using TableStructure;
using UnityEngine;

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

        public bool InCastle => CurCastle != null;

        /// <summary>
        /// 所有单位
        /// </summary>
        public List<Unit> Units { get; private set; }

        public ArmyHealthBar HealthBar { get; private set; }

        private Fsm<Army> fsm;

        public Army(string armyId, ECamp camp)
        {
            TableId = armyId;
            this.Camp = camp;
            InitUnits();
            InitHealthBar();

            fsm = Fsm<Army>.Create("Army", this,
            new ArmyStates.Idle(),
            new ArmyStates.Move(),
            new ArmyStates.Attack(),
            new ArmyStates.Dead());
        }

        public void OnDestroy()
        {
            if (CurCastle)
                ExitCastle();

            foreach (var unit in Units)
                GameObject.Destroy(unit.gameObject);
            GameObject.Destroy(HealthBar.gameObject);
        }

        // 因为这不是mono脚本，外部调用
        public void LateUpdate()
        {
            HealthBar.UpdateHealthAndPosition(Units, InCastle);
        }

        // 实例化士兵对象
        private void InitUnits()
        {
            GameObject unitPrefab = GFGlobal.Resource.LoadAssetSync<GameObject>($"Assets/Content/Sprites/Units/Prefab/{TableId}.prefab");
            var armyData = GFGlobal.Tables.TbArmyData[TableId];
            Units = new(armyData.Num);
            for (int i = 0; i < armyData.Num; i++)
            {
                var go = GameObject.Instantiate(unitPrefab, Global.LevelLogic.UnitTransform);
                var unit = go.GFGetOrAddComponent<Unit>();
                // 初始化单位
                unit.Init(armyData.Health);

                Units.Add(unit);
            }
        }

        private void InitHealthBar()
        {
            GameObject healthBarPrefab = GFGlobal.Resource.LoadAssetSync<GameObject>("Assets/Content/Sprites/Army/ArmyHealthBar.prefab");
            HealthBar = GameObject.Instantiate(healthBarPrefab, Global.LevelLogic.ArmyHealthTransform).GetComponent<ArmyHealthBar>();
            HealthBar.SetCamp(Camp);
            HealthBar.SetHealthPercent(1);
        }

        // 进入城堡
        public void EnterCastle(Castle castle)
        {
            if (CurCastle == castle)
                return;

            if (CurCastle)
                ExitCastle();

            CurCastle = castle;
            castle.OnArmyEnter(this);
        }

        // 离开城堡
        public void ExitCastle()
        {
            if (CurCastle)
            {
                CurCastle.OnArmyExit(this);
                CurCastle = null;
            }
        }

        /// <summary>
        /// 让士兵前往该目标
        /// </summary>
        public void CommandGotoTarget(Castle castle)
        {

        }
    }
}