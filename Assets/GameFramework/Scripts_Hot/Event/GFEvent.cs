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

        public void Subscribe<TEvent>(GameEventHandler<TEvent> handler, int priority = 0) where TEvent : GameEvent
        {
            m_EventSupport.Subscribe(handler, priority);
        }

        public void Unsubscribe<TEvent>(GameEventHandler<TEvent> handler) where TEvent : GameEvent
        {
            m_EventSupport.Unsubscribe(handler);
        }

        public void Fire(object sender, GameEvent e)
        {
            m_EventSupport.Fire(sender, e);
        }
    }
}
