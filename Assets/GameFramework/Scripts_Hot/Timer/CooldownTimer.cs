using UnityEngine;

namespace GameFramework.Hot
{
    public class CooldownTimer
    {
        private float lastTime = 0;
        public float Interval { get; set; }

        public CooldownTimer(float interval)
        {
            Interval = interval;
        }

        public bool IsReady(bool reset)
        {
            if (Time.time - lastTime >= Interval)
            {
                if (reset)
                    lastTime = Time.time;
                return true;
            }
            return false;
        }

        public void Restart()
        {
            lastTime = Time.time;
        }

        public void SetDone()
        {
            lastTime = -Interval;
        }
    }
}