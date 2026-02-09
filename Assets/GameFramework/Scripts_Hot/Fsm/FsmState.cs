using System;
using System.Collections.Generic;

namespace GameFramework.Hot
{
    public class FsmState<T> where T : class
    {
        protected Fsm<T> fsm;
        protected T Owner => fsm?.Owner;

        internal readonly List<FsmTimeline> m_Timelines = new();

        /// <summary>
        /// 该状态的标签。
        /// </summary>
        // public virtual int[] Tags => new int[0];
        public virtual int[] Tags { get; set; } = new int[0];

        /// <summary>
        /// 有限状态机状态初始化时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        protected internal virtual void OnInit(Fsm<T> fsm)
        {
            this.fsm = fsm;
        }

        /// <summary>
        /// 有限状态机状态进入时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        protected internal virtual void OnEnter(object userData)
        {
        }

        /// <summary>
        /// 有限状态机状态轮询时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        protected internal virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        /// <summary>
        /// 有限状态机状态离开时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        protected internal virtual void OnLeave()
        {
        }

        /// <summary>
        /// 有限状态机状态销毁时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        protected internal virtual void OnDestroy()
        {
            fsm = null;
            m_Timelines.Clear();
        }

        public void AddTimelines(params FsmTimeline[] timelines)
        {
            foreach (var timeline in timelines)
                m_Timelines.Add(timeline);
            m_Timelines.Sort();
        }

        public class FsmTimeline : IComparable<FsmTimeline>
        {
            public float time;

            public Action Fn;

            public FsmTimeline(float time, Action Fn)
            {
                this.time = time;
                this.Fn = Fn;
            }

            public int CompareTo(FsmTimeline other)
            {
                if (other == null) return 1;
                return time.CompareTo(other.time);
            }
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型。</typeparam>
        /// <param name="fsm">有限状态机引用。</param>
        protected void ChangeState<TState>(object userData = null) where TState : FsmState<T>
        {
            ChangeState(typeof(TState), userData);
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="stateType">要切换到的有限状态机状态类型。</param>
        protected void ChangeState(Type stateType, object userData = null)
        {
            fsm.ChangeState(stateType, userData);
        }
    }
}