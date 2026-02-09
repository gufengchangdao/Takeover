using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Hot
{
    /// <summary>
    /// 针对不同索引方式的引用池
    /// </summary>
    /// <typeparam name="K">索引类型，比如Type、string</typeparam>
    /// <typeparam name="V">引用值类型</typeparam>
    public class KeyedReferencePool<K, V> : AbstractReferencePool where V : class
    {
        private Action<V> recycleAction;
        private Action<V> releaseAction;

        private Dictionary<K, Queue<PoolObj<V>>> pool = new();
        private float nextReleaseTime = 0;

        internal override List<string> __PoolInfo
        {
            get
            {
                var res = new List<string>();
                foreach (var kvp in pool)
                {
                    var time = Time.unscaledTime;
                    foreach (var item in kvp.Value)
                        res.Add($"{kvp.Key}: {item.obj} {item.releaseTime - time:F1}");
                }
                return res;
            }
        }

        internal KeyedReferencePool(int capacity, float expireTime, Action<V> recycleAction, Action<V> releaseAction = null) : base(capacity, expireTime)
        {
            this.recycleAction = recycleAction;
            this.releaseAction = releaseAction;
        }

        public void Recycle(K key, V obj)
        {
            recycleAction?.Invoke(obj);

            if (!pool.TryGetValue(key, out var queue))
            {
                queue = new Queue<PoolObj<V>>();
                pool.Add(key, queue);
            }
            else
            {
                // 检查池中是否已经有该对象
                foreach (var item in queue)
                    if (item.obj == obj)
                        return;
            }

            if (capacity > 0 && queue.Count >= capacity)
            {
                // 池子满了
                releaseAction?.Invoke(obj);
                return;
            }

            float releaseTime = -1;
            if (expireTime > 0)
                releaseTime = Time.unscaledTime + expireTime;

            queue.Enqueue(new PoolObj<V> { releaseTime = releaseTime, obj = obj });
        }

        public V Get(K key)
        {
            if (pool.TryGetValue(key, out var queue) && queue.Count > 0)
                return queue.Dequeue().obj;
            return default;
        }

        private Dictionary<K, Queue<PoolObj<V>>> tempPool = new();
        public override void Update()
        {
            if (expireTime <= 0)
                return;

            var time = Time.unscaledTime;
            if (time < nextReleaseTime)
                return;

            nextReleaseTime = float.MaxValue;
            bool hasObj = false;

            foreach (var part in pool)//因为action中可能会修改字典
                tempPool.Add(part.Key, part.Value);
            foreach (var queue in tempPool.Values)
            {
                // 判断有没有对象过期
                while (queue.Count > 0)
                {
                    var objInfo = queue.Peek();
                    if (time >= objInfo.releaseTime)
                    {
                        queue.Dequeue();
                        releaseAction?.Invoke(objInfo.obj);
                    }
                    else
                    {
                        hasObj = true;
                        nextReleaseTime = Mathf.Min(nextReleaseTime, objInfo.releaseTime);
                        break;
                    }
                }
            }
            tempPool.Clear();
            if (!hasObj)
                nextReleaseTime = 0;
        }

        public void Remove(K key, bool invokeRelease = true)
        {
            if (!pool.TryGetValue(key, out var queue))
                return;

            while (queue.Count > 0)
            {
                var obj = queue.Dequeue().obj;
                if (invokeRelease)
                    releaseAction?.Invoke(obj);
            }
            pool.Remove(key);
        }

        public override void Clear()
        {
            foreach (var queue in pool.Values)
            {
                while (queue.Count > 0)
                    releaseAction?.Invoke(queue.Dequeue().obj);
            }
            pool.Clear();
        }

        public override void Destroy()
        {
            Clear();
            recycleAction = null;
            releaseAction = null;
        }
    }
}