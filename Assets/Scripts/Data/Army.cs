using System.Collections.Generic;
using GameFramework.AOT;
using GameFramework.Hot;
using TableStructure;
using UnityEngine;
using UnityEngine.U2D;

namespace Takeover
{
    /// <summary>
    /// 一组士兵，和血条是同一个预制件上的
    /// </summary>
    [RequireComponent(typeof(ArmyHealthBar))]
    public partial class Army : UpdateableComponent
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

        public Unit MainUnit
        {
            get
            {
                for (int i = 0; i < Units.Count; i++)
                {
                    if (Units[i].IsActive)
                        return Units[i];
                }
                return null;
            }
        }

        public ArmyHealthBar HealthBar { get; private set; }

        public float HealthPercent
        {
            get
            {
                if (Units.Count == 0)
                    return 0;

                int totalMax = 0;
                int totalCur = 0;
                for (int i = 0; i < Units.Count; i++)
                {
                    var unit = Units[i];
                    totalMax += unit.Health.MaxHealth;
                    totalCur += unit.Health.CurHealth;
                }
                return totalCur * 1f / totalMax;
            }
        }

        private Fsm<Army> fsm;

        void Awake()
        {
            HealthBar = GetComponent<ArmyHealthBar>();
        }

        public void Init(string armyId, ECamp camp)
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

        protected override void OnDestroy()
        {
            if (CurCastle)
                ExitCastle();

            foreach (var unit in Units)
                GameObject.Destroy(unit.gameObject);
            base.OnDestroy();
        }

        public override void OnLateUpdate(float dt)
        {
            HealthBar.UpdateHealthAndPosition(Units, HealthPercent, InCastle);
        }

        // 实例化士兵对象
        private void InitUnits()
        {
            var armyData = GFGlobal.Tables.TbArmyData[TableId];
            string assetPath = string.Format(GFGlobal.GlobalTableData.UnitPath, armyData.UnitType + Camp.ToString());
            GameObject unitPrefab = GFGlobal.Resource.LoadAssetSync<GameObject>(assetPath);
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
            var atlas = GFGlobal.Resource.LoadAssetSync<SpriteAtlas>(GFGlobal.Tables.TbGlobalSettingData.ArmyIconPath);
            HealthBar.Init(Camp, atlas.GetSprite(TableId));
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

            for (int i = 0; i < Units.Count; i++)
                Units[i].OnEnterCastle(castle);

            castle.OnArmyEnter(this);
        }

        // 离开城堡
        public void ExitCastle()
        {
            if (CurCastle)
            {
                for (int i = 0; i < Units.Count; i++)
                    Units[i].OnExitCastle();

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