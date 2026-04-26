using GameFramework.AOT;
using GameFramework.Hot;
using UnityEngine;

namespace Takeover
{
    /// <summary>
    /// 一个士兵作为一个单位
    /// </summary>
    [RequireComponent(typeof(UnitHealth))]
    public partial class Unit : UpdateableComponent
    {
        public UnitHealth Health { get; private set; }

        public bool IsDead => Health.IsDead;

        private Animator animator;
        private Fsm<Unit> fsm;

        private CooldownTimer behaviorCD = new(1);

        void Awake()
        {
            Health = GetComponent<UnitHealth>();
            animator = GetComponent<Animator>();
        }

        protected override void Start()
        {
            base.Start();

            fsm = GFGlobal.Fsm.CreateFsm(this,
                new UnitStates.Idle(),
                new UnitStates.Move()
            );
        }

        protected override void OnDestroy()
        {
            if (fsm != null)
                GFGlobal.Fsm.DestroyFsm(fsm);
            base.OnDestroy();
        }

        public void Init(int maxHealth, float speed)
        {
            Health.MaxHealth = maxHealth;
            Speed = speed;

            TargetPos = Position;
        }

        public void OnEnterCastle(Castle castle)
        {
            transform.position = castle.transform.position;
            gameObject.SetActive(false);
        }

        public void OnExitCastle()
        {
            if (!Health.IsDead)
                gameObject.SetActive(true);
        }

        public override void OnUpdate(float dt)
        {
            if (behaviorCD.IsReady(true) && !IsDead)
                BehaviorUpdate(behaviorCD.Interval);

            if (!IsDead)
                ApplyMoveImpulse();

            impulseDX *= 0.5f; //惯性减速
            impulseDY *= 0.5f;
        }

        private void BehaviorUpdate(float dt)
        {
            if (!fsm.InState<UnitStates.Move>() && !AtTarget)
            {
                fsm.ChangeState<UnitStates.Move>();
                return;
            }

            if (fsm.InState<UnitStates.Move>() && AtTarget)
            {
                fsm.ChangeState<UnitStates.Idle>();
                return;
            }
        }
    }
}