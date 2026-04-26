using UnityEngine;

namespace GameFramework.Hot
{
    public class CooldownTimer
    {
        private float lastTime = 0;

        private float _interval = 1; //默认1秒
        public float Interval
        {
            get
            {
                return _interval;
            }
            set
            {
                _interval = Mathf.Max(0, value);
            }
        }

        private float CurTime => Time.time;

        public CooldownTimer(float interval)
        {
            Interval = interval;
        }

        public bool IsReady()
        {
            if (CurTime - lastTime >= Interval)
            {
                lastTime = CurTime;
                return true;
            }
            return false;
        }

        public void Restart()
        {
            lastTime = CurTime;
        }

        public void SetDone()
        {
            lastTime = CurTime - Interval;
        }
    }
}