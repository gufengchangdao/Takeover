using System;
using System.Collections.Generic;
using GameFramework.AOT;
using Sirenix.OdinInspector;

namespace GameFramework.Hot
{
    /// <summary>
    /// 有限状态机。
    /// 1. 可以临时存储Data，在切换到下一个状态时，调用完下一个状态的OnEnter方法后会清空数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [HideReferenceObjectPicker]
    public class Fsm<T> : FsmBase, IRecyclable where T : class
    {
        [ShowInInspector]
        public string TypeNamePair => typeof(T).Name + (string.IsNullOrEmpty(Name) ? "" : ("." + Name));

        private readonly Dictionary<string, object> m_Datas = new();

        public T Owner { get; private set; }
        public override Type OwnerType => typeof(T);

        private readonly Dictionary<Type, FsmState<T>> m_States = new();

        private FsmState<T> m_CurrentState;
        public FsmState<T> CurrentState
        {
            get
            {
                return m_CurrentState;
            }
            private set
            {
                m_CurrentState = value;
                CurrentStateTime = 0;
                m_CurrentTimelineIndex = 0;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态名称。
        /// </summary>
        [ShowInInspector]
        public string CurrentStateName => CurrentState?.GetType().FullName;

        /// <summary>
        /// 获取当前有限状态机状态持续时间。
        /// </summary>
        [ShowInInspector, ReadOnly]
        public float CurrentStateTime { get; private set; }

        private int m_CurrentTimelineIndex;

        /// <summary>
        /// 状态数量。
        /// </summary>
        public int StateCount => m_States.Count;

        /// <summary>
        /// 是否正在运行。
        /// </summary>
        public bool IsRunning => CurrentState != null;

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>创建的有限状态机。</returns>
        public static Fsm<T> Create(string name, T owner, params FsmState<T>[] states)
        {
            Fsm<T> fsm = TypeReferencePool.GetOrNew<Fsm<T>>();
            fsm.Name = name;
            fsm.Owner = owner;
            foreach (FsmState<T> state in states)
            {
                Type stateType = state.GetType();
                if (fsm.m_States.ContainsKey(stateType))
                {
                    Log.Error("[FSM] FSM '{0}.{1}' state '{2}' is already exist.", typeof(T), name, stateType.FullName);
                    continue;
                }

                fsm.m_States.Add(stateType, state);
                state.OnInit(fsm);
            }
            return fsm;
        }

        public static Fsm<T> Create(T owner, params FsmState<T>[] states)
        {
            return Create("", owner, states);
        }

        /// <summary>
        /// 关闭并清理有限状态机。
        /// </summary>
        internal override void Shutdown()
        {
            TypeReferencePool.Recycle(this);
        }

        /// <summary>
        /// 清理有限状态机，不过不应该直接调用，应该调用Shutdown。
        /// </summary>
        public void OnRecycle()
        {
            CurrentState?.OnLeave();
            foreach (var state in m_States.Values)
                state.OnDestroy();
            m_States.Clear();
            Name = null;
            Owner = null;

            m_Datas.Clear();

            CurrentState = null;
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <param name="stateType">要切换到的有限状态机状态类型。</param>
        public void ChangeState(Type stateType, object userData = null)
        {
            FsmState<T> state = GetState(stateType);
            if (state == null)
            {
                Log.Error("[FSM] FSM '{0}.{1}' can not start state '{2}' which is not exist.", typeof(T), Name, stateType.FullName);
                return;
            }

            // Type oldStateType = CurrentState.GetType();
            CurrentState?.OnLeave();

            CurrentState = state;
            CurrentState.OnEnter(userData);

            // 事件推送给实体自己
            // if (Owner is Entity entity)
            //     entity.Event.Fire(this, StateChangeEvent.Create(stateType, oldStateType));

            ClearData();
        }

        public void ChangeState<S>(object userData = null)
        {
            ChangeState(typeof(S), userData);
        }

        public S GetState<S>() where S : FsmState<T>
        {
            return GetState(typeof(S)) as S;
        }

        public FsmState<T> GetState(Type stateType)
        {
            return m_States.TryGetValue(stateType, out FsmState<T> state) ? state : null;
        }

        public bool TryGetData<D>(string name, out D data)
        {
            var d = GetData(name);
            if (d == null)
            {
                data = default;
                return false;
            }
            data = (D)d;
            return true;
        }

        public object GetData(string name)
        {
            return m_Datas.TryGetValue(name, out object value) ? value : null;
        }

        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetData(string name, object value)
        {
            m_Datas[name] = value;
        }

        public void ClearData()
        {
            m_Datas.Clear();
        }

        /// <summary>
        /// 设置有限状态机数据，并且会尝试把原来的值归还引用池中。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetDataWithRelease(string name, object value)
        {
            object oldValue = GetData(name);
            if (oldValue is IRecyclable reference)
                TypeReferencePool.Recycle(reference);
            SetData(name, value);
        }

        /// <summary>
        /// 移除有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>是否移除有限状态机数据成功。</returns>
        public bool RemoveData(string name)
        {
            return m_Datas.Remove(name);
        }

        public bool RemoveDataWithRelease(string name)
        {
            object oldValue = GetData(name);
            if (oldValue is IRecyclable reference)
                TypeReferencePool.Recycle(reference);
            return RemoveData(name);
        }

        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (CurrentState == null)
            {
                return;
            }

            CurrentStateTime += elapseSeconds;
            // 检查timeline
            var timelines = CurrentState.m_Timelines;
            if (m_CurrentTimelineIndex < timelines.Count)
            {
                FsmState<T>.FsmTimeline timeline = timelines[m_CurrentTimelineIndex];
                if (CurrentStateTime >= timeline.time)
                {
                    timeline.Fn();
                    m_CurrentTimelineIndex++;
                }
            }

            CurrentState.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 是否存在有限状态机状态。
        /// </summary>
        /// <param name="stateType">要检查的有限状态机状态类型。</param>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState(Type stateType)
        {
            return m_States.ContainsKey(stateType);
        }

        public bool HasState<TState>() where TState : FsmState<T>
        {
            return m_States.ContainsKey(typeof(TState));
        }

        public bool InState<TState>() where TState : FsmState<T>
        {
            return CurrentState is TState;
        }
    }
}