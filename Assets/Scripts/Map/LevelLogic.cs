using System.Collections.Generic;
using GameFramework.AOT;
using GameFramework.Hot;
using TableStructure;
using UnityEngine;

namespace Takeover
{
    public partial class LevelLogic : UpdateableComponent
    {
        private int _gameSpeed = 1;
        public int GameSpeed
        {
            get
            {
                return _gameSpeed;
            }
            set
            {
                if (_gameSpeed == value)
                    return;
                _gameSpeed = value;
                GFGlobal.Event.Fire(this, GameSpeedChangeEvent.Create(value));
            }
        }

        public List<Castle> Castles { get; private set; } = new();
        public List<Army> Armies { get; private set; } = new();
        public List<Building> Buildings { get; private set; } = new();
        public Dictionary<ECamp, CombotantData> Combotants { get; private set; } = new();

        private bool lockAI = false;
        public bool Playing { get; private set; } = true;
        private int lastSecond;
        public BindableProperty<float> LifeTime = new();

        private Transform _unitsTransform;
        public Transform UnitTransform
        {
            get
            {
                if (!_unitsTransform)
                    _unitsTransform = new GameObject("Units").transform;
                return _unitsTransform;
            }
        }

        private Transform _armyHealthTransform;
        public Transform ArmyHealthTransform
        {
            get
            {
                if (!_armyHealthTransform)
                    _armyHealthTransform = new GameObject("ArmyHealth").transform;
                return _armyHealthTransform;
            }
        }

        // 初始化关卡
        protected override void Start()
        {
            base.Start();

            foreach (var castle in FindObjectsByType<Castle>(FindObjectsSortMode.None))
            {
                Castles.Add(castle);

                if (!Combotants.ContainsKey(castle.Camp))
                    Combotants[castle.Camp] = new CombotantData(castle.Camp);
            }

            Combotants[Global.LevelData.Camp].IsPlayer = true;

            // 初始化资源
            foreach (var camp in Combotants.Keys)
                UpdateResSpeed(camp);

            GFGlobal.Event.Subscribe<OnCastleStateChange>(OnCastleStateChange);
            GFGlobal.Event.Subscribe<OnBuildingStateChange>(OnBuildingStateChange);
        }

        protected override void OnDestroy()
        {
            GFGlobal.Event.Unsubscribe<OnCastleStateChange>(OnCastleStateChange);
            GFGlobal.Event.Unsubscribe<OnBuildingStateChange>(OnBuildingStateChange);
            LifeTime.ClearAllEvents();
            base.OnDestroy();
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (!Playing)
                return; //游戏被暂停了

            dt *= GameSpeed;

            LifeTime.Value += dt;
            if (!(Mathf.FloorToInt(LifeTime.Value) > lastSecond))
                return;//每秒更新一次

            lastSecond = Mathf.FloorToInt(LifeTime.Value);

            // 增加资源
            foreach (var combotant in Combotants.Values)
                combotant.AddResCounters();

            if (!lockAI)
                UpdateCpuCombotantLogic(1);
        }

        void LateUpdate()
        {
            for (int i = 0; i < Armies.Count; i++)
            {
                Armies[i].LateUpdate();
            }
        }
    }
}