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
        public List<Unit> Units { get; private set; } = new();

        public int UnitCount => Units.Count;

        public Vector2 MainUnitPosition
        {
            get
            {
                var unit = GetMainUnit();
                if (!unit)
                    return default;
                return unit.transform.position;
            }
        }

        public ArmyHealthBar HealthBar { get; private set; }

        public float HealthPercent
        {
            get
            {
                int totalMax = 0;
                int totalCur = 0;
                for (int i = 0; i < Units.Count; i++)
                {
                    var unit = Units[i];
                    totalMax += unit.Health.MaxHealth;
                    totalCur += unit.Health.CurHealth;
                }
                if (totalMax == 0) return 0;

                return totalCur * 1f / totalMax;
            }
        }

        public List<int> CurPathList { get; private set; }

        public bool WantToMove => CurPathList != null && CurPathList.Count > 0;

        private CooldownTimer behaviorCD = new(1);

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
            InitFormation();
            InitHealthBar();

            fsm = GFGlobal.Fsm.CreateFsm(this,
            new ArmyStates.Idle(),
            new ArmyStates.Move(),
            new ArmyStates.Attack());

            fsm.ChangeState<ArmyStates.Idle>();
        }

        protected override void OnDestroy()
        {
            if (CurCastle)
            {
                CurCastle.OnArmyExit(this);
                CurCastle = null;
            }

            GFGlobal.Fsm.DestroyFsm(fsm);
            fsm = null;

            foreach (var unit in Units)
                if (unit)
                    GameObject.Destroy(unit.gameObject);
            Units.Clear();
            base.OnDestroy();
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (behaviorCD.IsReady(true))
                BehaviorUpdate(behaviorCD.Interval);

        }

        // 更新行为，充当一个简单的行为树吧
        private void BehaviorUpdate(float dt)
        {
            if (!InCombat && WantToMove && !fsm.InState<ArmyStates.Move>())
            {
                fsm.ChangeState<ArmyStates.Move>();
            }
        }

        public override void OnLateUpdate(float dt)
        {
            HealthBar.UpdateHealthAndPosition(Units, HealthPercent, InCastle);
        }

        public Unit GetMainUnit()
        {
            if (Units.Count == 0) return null;

            for (int i = 0; i < Units.Count; i++)
            {
                if (!Units[i].IsDead)
                    return Units[i];
            }
            return Units[0]; //都死了
        }

        // 实例化士兵对象
        private void InitUnits()
        {
            var armyData = GFGlobal.Tables.TbArmyData[TableId];
            string assetPath = string.Format(GFGlobal.GlobalTableData.UnitPath, armyData.UnitType + Camp.ToString());
            GameObject unitPrefab = GFGlobal.Resource.LoadAssetSync<GameObject>(assetPath);
            Units.Capacity = armyData.Num;
            for (int i = 0; i < armyData.Num; i++)
            {
                var go = GameObject.Instantiate(unitPrefab, Global.LevelLogic.UnitTransform);
                var unit = go.GFGetOrAddComponent<Unit>();
                float speed = armyData.Speed / 100; //跑的太快了，给个统一缩放比例
                unit.Init(armyData.Health, speed);

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
        public void CommandGotoTarget(int targetNodeIndex)
        {
            if (InCastle)
            {
                if (Global.MapPath.GetNodeIndex(CurCastle) == targetNodeIndex)
                    return;

                ExitCastle();
            }

            CurPathList = Global.MapPath.GetPathNodeList(MainUnitPosition, Global.MapPath.GetNodePosition(targetNodeIndex));
            behaviorCD.SetDone(); //立即更新行为
        }
    }
}