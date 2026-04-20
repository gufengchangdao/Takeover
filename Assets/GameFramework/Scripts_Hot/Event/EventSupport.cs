using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.AOT;

namespace GameFramework.Hot
{
    // 事件回调
    public delegate void GameEventHandler<in TEvent>(object sender, TEvent e) where TEvent : GameEvent;

    public class EventSupport : IRecyclable
    {
        private readonly Dictionary<Type, IEventBucket> m_EventBuckets = new Dictionary<Type, IEventBucket>();

        public static EventSupport Create()
        {
            return TypeReferencePool.GetOrNew<EventSupport>();
        }

        public void Subscribe<TEvent>(GameEventHandler<TEvent> handler, int priority = 0) where TEvent : GameEvent
        {
            if (handler == null)
                return;

            GetOrCreateBucket<TEvent>().Subscribe(handler, priority);
        }

        private EventBucket<TEvent> GetOrCreateBucket<TEvent>() where TEvent : GameEvent
        {
            Type eventType = typeof(TEvent);
            if (m_EventBuckets.TryGetValue(eventType, out IEventBucket bucket))
                return (EventBucket<TEvent>)bucket;
            EventBucket<TEvent> newBucket = new EventBucket<TEvent>();
            m_EventBuckets.Add(eventType, newBucket);
            return newBucket;
        }

        public void Unsubscribe<TEvent>(GameEventHandler<TEvent> handler) where TEvent : GameEvent
        {
            if (handler == null)
                return;

            Type eventType = typeof(TEvent);
            if (!m_EventBuckets.TryGetValue(eventType, out IEventBucket bucket))
                return;

            ((EventBucket<TEvent>)bucket).Unsubscribe(handler);

            if (bucket.IsEmpty)
                m_EventBuckets.Remove(eventType);
        }

        public void Fire<TEvent>(object sender, TEvent e) where TEvent : GameEvent
        {
            Fire(sender, (GameEvent)e);
        }

        public void Fire(object sender, GameEvent e)
        {
            if (e == null)
                return;

            Type eventType = e.GetType();
            try
            {
                e.handled = false;
                if (m_EventBuckets.TryGetValue(eventType, out IEventBucket bucket))
                {
                    bucket.Dispatch(sender, e);
                    if (bucket.IsEmpty)
                        m_EventBuckets.Remove(eventType);
                }
            }
            finally
            {
                TypeReferencePool.Recycle(e);
            }
        }

        public void OnRecycle()
        {
            m_EventBuckets.Clear();
        }



        private interface IEventBucket
        {
            bool IsEmpty { get; }

            void Dispatch(object sender, GameEvent e);
        }

        private sealed class EventBucket<TEvent> : IEventBucket where TEvent : GameEvent
        {
            private readonly List<ListenerEntry> m_Listeners = new();
            private readonly Type m_EventType = typeof(TEvent);
            private int m_DispatchCount = 0;

            private readonly List<GameEventHandler<TEvent>> curHandlers = new();
            private readonly HashSet<GameEventHandler<TEvent>> removedHandlers = new();

            public bool IsEmpty
            {
                get { return m_Listeners.Count == 0; }
            }

            public void Subscribe(GameEventHandler<TEvent> handler, int priority)
            {
                int insertIndex = -1;
                for (int i = 0; i < m_Listeners.Count; i++)
                {
                    ListenerEntry entry = m_Listeners[i];
                    if (entry.Handler == handler)
                    {
                        Log.Warning("[Event] Event {0} already registered :{1}.{2}",
                                    m_EventType.Name,
                                    handler.Method.DeclaringType?.Name,
                                    handler.Method.Name);
                        return;
                    }

                    if (insertIndex == -1 && priority > entry.Priority)
                        insertIndex = i;
                }
                if (insertIndex == -1)
                    m_Listeners.Add(new ListenerEntry(handler, priority));
                else
                    m_Listeners.Insert(insertIndex, new ListenerEntry(handler, priority));
            }

            public void Unsubscribe(GameEventHandler<TEvent> handler)
            {
                for (int i = 0; i < m_Listeners.Count; i++)
                {
                    if (m_Listeners[i].Handler == handler)
                    {
                        m_Listeners.RemoveAt(i);
                        if (m_DispatchCount > 0) //执行回调期间
                            removedHandlers.Add(handler);
                        return;
                    }
                }
            }

            public void Dispatch(object sender, GameEvent e)
            {
                TEvent eventArgs = (TEvent)e;
                if (m_DispatchCount == 0)
                {
                    curHandlers.Clear();
                    for (int i = 0; i < m_Listeners.Count; i++)
                        curHandlers.Add(m_Listeners[i].Handler);
                }
                m_DispatchCount++;

                for (int i = 0; i < curHandlers.Count && !eventArgs.handled; i++)
                {
                    GameEventHandler<TEvent> handler = curHandlers[i];
                    if (removedHandlers.Contains(handler))
                        continue;
                    try
                    {
                        handler(sender, eventArgs);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("[Event] Event {0} error : {1}", m_EventType.Name, ex);
                    }
                }

                m_DispatchCount--;
                if (m_DispatchCount == 0)
                {
                    curHandlers.Clear();
                    removedHandlers.Clear();
                }
            }

            private sealed class ListenerEntry
            {
                public readonly GameEventHandler<TEvent> Handler;
                public readonly int Priority; //优先级越高越先执行

                public ListenerEntry(GameEventHandler<TEvent> handler, int priority)
                {
                    Handler = handler;
                    Priority = priority;
                }
            }
        }
    }
}
