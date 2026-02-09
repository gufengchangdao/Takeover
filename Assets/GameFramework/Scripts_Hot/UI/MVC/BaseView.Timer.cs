using System;
using System.Collections.Generic;

namespace GameFramework.Hot
{
    public abstract partial class AbstractBaseView
    {
        private HashSet<int> timerInfos = new();

        public int DelayRun(float time, Action action = null)
        {
            var timerId = GFGlobal.Timer.DelayRun(time, action);
            timerInfos.Add(timerId);
            return timerId;
        }

        public int Tick(float time, Action action = null)
        {
            var timerId = GFGlobal.Timer.Tick(time, action);
            timerInfos.Add(timerId);
            return timerId;
        }

        public void CancelTimer(int timerId)
        {
            GFGlobal.Timer.Cancel(timerId);
            timerInfos.Remove(timerId);
        }

        public void CancelAllTimers()
        {
            foreach (var id in timerInfos)
                GFGlobal.Timer.Cancel(id);
            timerInfos.Clear();
        }
    }
}