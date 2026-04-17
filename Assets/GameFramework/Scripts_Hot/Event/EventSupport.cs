using System;
using System.Collections.Generic;
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
            private readonly LinkedList<ListenerEntry> m_Listeners = new LinkedList<ListenerEntry>();
            private readonly Type m_EventType = typeof(TEvent);
            private int m_DispatchCount = 0;

            public bool IsEmpty
            {
                get { return m_Listeners.Count == 0; }
            }

            public void Subscribe(GameEventHandler<TEvent> handler, int priority)
            {
                LinkedListNode<ListenerEntry> current = m_Listeners.First;
                while (current != null)
                {
                    ListenerEntry entry = current.Value;
                    if (!entry.Removed && entry.Handler == handler)
                    {
                        Log.Warning(
                            "[Event] Event {0} already registered :{1}.{2}",
                            m_EventType.Name,
                            handler.Method.DeclaringType?.Name,
                            handler.Method.Name);
                        return;
                    }

                    if (priority > entry.Priority)
                    {
                        m_Listeners.AddBefore(current, new ListenerEntry(handler, priority));
                        return;
                    }

                    current = current.Next;
                }

                m_Listeners.AddLast(new ListenerEntry(handler, priority));
            }

            public void Unsubscribe(GameEventHandler<TEvent> handler)
            {
                LinkedListNode<ListenerEntry> current = m_Listeners.First;
                while (current != null)
                {
                    LinkedListNode<ListenerEntry> next = current.Next;
                    ListenerEntry entry = current.Value;

                    if (!entry.Removed && entry.Handler == handler)
                    {
                        if (m_DispatchCount > 0)
                            entry.Removed = true;
                        else
                            m_Listeners.Remove(current);

                        break;
                    }

                    current = next;
                }

                if (m_DispatchCount == 0)
                    CleanupRemoved();
            }

            public void Dispatch(object sender, GameEvent e)
            {
                TEvent eventArgs = (TEvent)e;
                m_DispatchCount++;
                try
                {
                    LinkedListNode<ListenerEntry> current = m_Listeners.First;
                    while (current != null && !eventArgs.handled)
                    {
                        // 先缓存 next，保证回调里反注册当前/后续监听时遍历仍然安全
                        LinkedListNode<ListenerEntry> next = current.Next;
                        ListenerEntry entry = current.Value;
                        if (!entry.Removed)
                        {
                            try
                            {
                                entry.Handler(sender, eventArgs);
                            }
                            catch (Exception ex)
                            {
                                Log.Error("[Event] Event {0} error : {1}", m_EventType.Name, ex);
                            }
                        }
                        current = next;
                    }
                }
                finally
                {
                    m_DispatchCount--;

                    if (m_DispatchCount == 0)
                        CleanupRemoved();
                }
            }

            private void CleanupRemoved()
            {
                LinkedListNode<ListenerEntry> current = m_Listeners.First;
                while (current != null)
                {
                    LinkedListNode<ListenerEntry> next = current.Next;
                    if (current.Value.Removed)
                        m_Listeners.Remove(current);
                    current = next;
                }
            }

            private sealed class ListenerEntry
            {
                public readonly GameEventHandler<TEvent> Handler;
                public readonly int Priority; //优先级越高越先执行
                public bool Removed; //允许执行回调期间移除回调

                public ListenerEntry(GameEventHandler<TEvent> handler, int priority)
                {
                    Handler = handler;
                    Priority = priority;
                    Removed = false;
                }
            }
        }
    }
}
