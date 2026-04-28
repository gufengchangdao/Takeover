using System.Collections.Generic;
using GameFramework.AOT;
using GameFramework.Hot;
using TableStructure;
using UnityEngine;

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

        public bool IsAllUnitDead
        {
            get
            {
                for (int i = 0; i < Units.Count; i++)
                {
                    if (!Units[i].IsDead)
                        return false;
                }
                return true;
            }
        }

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

        public List<int> CurPathList { get; private set; } = new();

        public bool WantToMove => CurPathList.Count > 0;

        public float Radius { get; private set; }

        private BehaviorTree behaviorTree;

        void Awake()
        {
            HealthBar = GetComponent<ArmyHealthBar>();
        }

        public void Init(string armyId, ECamp camp)
        {
            TableId = armyId;
            this.Camp = camp;
            var armyData = GFGlobal.Tables.TbArmyData[TableId];
            AttackRange = armyData.AttackRange;
            Radius = armyData.Radius;

            InitUnits();
            InitFormation();

            behaviorTree = new BehaviorTreeBuilder()
                .Repeat(-1)
                    .Condition(() => !IsAllUnitDead)
                        .Seletctor()
                            .If(() => HasTarget, new BTArmyChaseAndAttackNode(this))
                            .If(() => WantToMove, new BTArmyMoveNode(this))
                            .Action(IdleAction)
                .End();
        }

        protected override void OnDestroy()
        {
            if (CurCastle)
            {
                CurCastle.OnArmyExit(this);
                CurCastle = null;
            }

            foreach (var unit in Units)
                if (unit)
                    GameObject.Destroy(unit.gameObject);
            Units.Clear();
            base.OnDestroy();
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            UpdateNearTargets();

            if (!behaviorTree.IsTerminated)
                behaviorTree.Tick();
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
                unit.OnArriveTarget += OnUnitArriveTarget;
                Units.Add(unit);
            }
        }

        private void OnUnitArriveTarget(Unit unit)
        {
            if (InCastle) //如果主单位已经进城堡了，那当前位置一定是最后的城堡节点
            {
                unit.OnEnterCastle(CurCastle);
            }
        }

        // 进入城堡
        public void EnterCastle(Castle castle, bool setAllUnitPos = false)
        {
            if (CurCastle == castle)
                return;

            if (CurCastle)
                ExitCastle();

            CurCastle = castle;

            if (setAllUnitPos)
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

            Global.MapPath.UpdatePathNodeList(MainUnitPosition, Global.MapPath.GetNodePosition(targetNodeIndex), CurPathList);
            // behaviorCD.SetDone(); //立即更新行为
        }
    }
}