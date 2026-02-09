using System;
using System.Collections.Generic;
using GameFramework.AOT;

namespace GameFramework.Hot
{
    /// <summary>
    /// 让对象支持事件系统，并且可以在检视器面板可视化
    /// </summary>
    public class EventSupport : IRecyclable
    {
        private Dictionary<Type, LinkedList<EventHandler<GameEvent>>> m_Event = new();

#if UNITY_EDITOR
        private void __UpdateEventList()
        {
            __CacheEvent.Clear();
            foreach (var entry in m_Event)
            {
                LinkedList<string> methodList = new();
                foreach (var h in entry.Value)
                {
                    methodList.AddLast($"{h.Method.DeclaringType?.Name}.{h.Method.Name}");
                }
                __CacheEvent[entry.Key.Name] = methodList;
            }
        }

        private Dictionary<string, LinkedList<string>> __CacheEvent = new();
#endif

        public static EventSupport Create()
        {
            return TypeReferencePool.GetOrNew<EventSupport>();
        }

        public void Subscribe<T>(EventHandler<GameEvent> handler) where T : GameEvent
        {
            Subscribe(typeof(T), handler);
        }

        public void Subscribe(Type eventType, EventHandler<GameEvent> handler)
        {
            if (!m_Event.TryGetValue(eventType, out var handlerList))
            {
                handlerList = new();
                m_Event[eventType] = handlerList;
            }

            foreach (var h in handlerList)
            {
                if (h == handler)
                {
                    Log.Warning("[Event] Event {0} already registered :{1}.{2}", eventType.Name, h.Method.DeclaringType?.Name, h.Method.Name);
                    return; //已经添加过这个处理器了
                }
            }

            handlerList.AddLast(handler);
        }

        public void Unsubscribe<T>(EventHandler<GameEvent> handler) where T : GameEvent
        {
            Unsubscribe(typeof(T), handler);
        }

        public void Unsubscribe(Type eventType, EventHandler<GameEvent> handler)
        {
            if (!m_Event.TryGetValue(eventType, out var handlerList))
                return;

            handlerList.Remove(handler);
            if (handlerList.Count <= 0)
                m_Event.Remove(eventType);
        }

        public void Fire(object sender, GameEvent e)
        {
            Type eventType = e.GetType();
            if (!m_Event.TryGetValue(eventType, out var handlerList))
                return;

            LinkedListNode<EventHandler<GameEvent>> current = handlerList.First;
            while (current != null && current.Value != null)
            {
                var next = current.Next;
                try
                {
                    current.Value(sender, e); //执行回调
                }
                catch (Exception ex)
                {
                    Log.Error("[Event] Event {0} error : {1}", eventType.Name, ex);
                }
                current = next;
            }

            TypeReferencePool.Recycle(e);
        }

        public void OnRecycle()
        {
            m_Event.Clear();
        }
    }
}