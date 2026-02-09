using System;
using System.Collections.Generic;
using GameFramework.AOT;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Hot
{
    public class GFTimer : GFBaseModule
    {
        private int autoSerialId = 1;

        [LabelText("定时器"),
        ShowInInspector,
        Searchable,
        DisableInEditorMode,
        HideInEditorMode]
        private readonly Dictionary<int, TimerInfo> timers = new(); // TODO 也许可以用优先队列优化一下
        private readonly List<int> temp = new();

        // 所有计时中最早结束时间，用来Update中优化判断用的
        private float minEndTime = Mathf.Infinity;

        private ReferencePool<TimerInfo> timerInfoPool;

        void Awake()
        {
            timerInfoPool = GFGlobal.ReferencePool.CreatePool<TimerInfo>("Timer", 200, -1, () => new TimerInfo());
        }

        void OnDestroy()
        {
            foreach (var pair in timers)
                temp.Add(pair.Key);
            for (int i = 0; i < temp.Count; i++)
            {
                int serialId = temp[i];
                var timerInfo = timers[serialId];
                timers.Remove(serialId);
                timerInfoPool.Recycle(timerInfo);
            }
            temp.Clear();
        }

        public override void ModuleUpdate()
        {
            base.ModuleUpdate();
            if (Time.time < minEndTime)
                return;

            foreach (var pair in timers)
            {
                if (pair.Value.EndTime <= Time.time)
                    temp.Add(pair.Key);
            }

            for (int i = 0; i < temp.Count; i++)
            {
                int serialId = temp[i];
                if (!timers.TryGetValue(serialId, out var timerInfo))
                    continue; //考虑到TimerDone中有可能移除其他的timer

                try
                {
                    timerInfo.Action?.Invoke();
                }
                catch (Exception e)
                {
                    Log.Error("[Timer] Timer done error.：{0}", e);
                }

                if (timerInfo.IsLoop)
                {
                    timerInfo.ResetTime();
                }
                else
                {
                    timers.Remove(serialId);
                    timerInfoPool.Recycle(timerInfo);
                }
            }
            temp.Clear();

            UpdateMinEndTime();
        }

        public bool TimerExists(int serialId)
        {
            return timers.ContainsKey(serialId);
        }

        public int DelayRun(float time, Action action = null)
        {
            return TickImpl(time, action, false);
        }

        public int Tick(float time, Action action = null)
        {
            return TickImpl(time, action, true);
        }

        private int TickImpl(float time, Action action, bool isLoop)
        {
            int serialId = autoSerialId++;
            TimerInfo timerInfo = timerInfoPool.Get();
            timerInfo.Init(time, action, isLoop);
            timers.Add(serialId, timerInfo);
            minEndTime = Mathf.Min(minEndTime, timerInfo.EndTime);
            return serialId;
        }

        public bool Cancel(int serialId)
        {
            if (timers.TryGetValue(serialId, out var timerInfo))
            {
                timers.Remove(serialId);
                timerInfoPool.Recycle(timerInfo);
                UpdateMinEndTime();
                return true;
            }
            return false;
        }

        public float GetTimeLeft(int serialId)
        {
            if (!TimerExists(serialId))
                return -1;

            return timers[serialId].EndTime - Time.time;
        }

        private void UpdateMinEndTime()
        {
            minEndTime = Mathf.Infinity;
            foreach (var pair in timers)
                minEndTime = Mathf.Min(minEndTime, pair.Value.EndTime);
        }
    }

    [InlineProperty, HideReferenceObjectPicker]
    class TimerInfo : IRecyclable
    {
        public float Interval { get; private set; }
        public float StartTime { get; private set; }
        public float EndTime { get; private set; }
        public Action Action { get; private set; }
        public bool IsLoop { get; private set; }

        public void Init(float interval, Action action, bool isLoop)
        {
            Interval = interval;
            Action = action;
            IsLoop = isLoop;
            ResetTime();
        }

        public void ResetTime()
        {
            StartTime = Time.time;
            EndTime = StartTime + Interval;
        }

        public void OnRecycle()
        {
            Action = null;
            IsLoop = false;
        }
    }
}