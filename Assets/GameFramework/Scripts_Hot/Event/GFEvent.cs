using System;

namespace GameFramework.Hot
{
    public class GFEvent : GFBaseModule
    {
        private EventSupport m_EventSupport;

        void Awake()
        {
            m_EventSupport = EventSupport.Create();
        }

        public void Subscribe<T>(EventHandler<GameEvent> handler) where T : GameEvent
        {
            Subscribe(typeof(T), handler);
        }

        public void Subscribe(Type eventType, EventHandler<GameEvent> handler)
        {
            m_EventSupport.Subscribe(eventType, handler);
        }

        public void Unsubscribe<T>(EventHandler<GameEvent> handler) where T : GameEvent
        {
            Unsubscribe(typeof(T), handler);
        }

        public void Unsubscribe(Type eventType, EventHandler<GameEvent> handler)
        {
            m_EventSupport.Unsubscribe(eventType, handler);
        }

        public void Fire(object sender, GameEvent e)
        {
            m_EventSupport.Fire(sender, e);
        }
    }
}