using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Hot
{
    /// <summary>
    /// 抽象一层给odin展示，并且统一管理
    /// </summary>
    [HideReferenceObjectPicker]
    public abstract class AbstractReferencePool
    {
        /// <summary>
        /// 容量，负数表示不限制
        /// </summary>
        protected int capacity;

        /// <summary>
        /// 过期秒数，负数表示不限制
        /// </summary>
        protected float expireTime;

        internal abstract List<string> __PoolInfo { get; }

        public AbstractReferencePool(int capacity, float expireTime)
        {
            this.capacity = capacity;
            this.expireTime = expireTime;
        }

        public abstract void Update();
        public abstract void Clear();
        public abstract void Destroy();
    }

    struct PoolObj<T>
    {
        public float releaseTime;
        public T obj;
    }

    public class ReferencePool<V> : AbstractReferencePool
    {
        private Func<V> createAction;
        private Action<V> recycleAction;
        private Action<V> releaseAction;

        private Queue<PoolObj<V>> pool = new();
        private float nextReleaseTime = 0;

        internal override List<string> __PoolInfo
        {
            get
            {
                var res = new List<string>();
                var time = Time.unscaledTime;
                foreach (var item in pool)
                    res.Add(item.obj.ToString() + " " + (item.releaseTime - time).ToString("F1"));
                return res;
            }
        }

        /// <summary>
        /// 创建一个引用池
        /// </summary>
        /// <param name="capacity">容量，负数表示不限制</param>
        /// <param name="expireTime">过期秒数，负数表示不限制</param>
        /// <param name="recycleAction">回收</param>
        /// <param name="createAction">创建</param>
        /// <param name="releaseAction">释放</param>
        internal ReferencePool(int capacity, float expireTime, Action<V> recycleAction, Func<V> createAction = null, Action<V> releaseAction = null) : base(capacity, expireTime)
        {
            this.createAction = createAction;
            this.recycleAction = recycleAction;
            this.releaseAction = releaseAction;
        }

        public void Recycle(V obj)
        {
            recycleAction?.Invoke(obj);

            if (capacity > 0 && pool.Count >= capacity)
            {
                // 池子满了
                releaseAction?.Invoke(obj);
                return;
            }

            float releaseTime = -1;
            if (expireTime > 0)
                releaseTime = Time.unscaledTime + expireTime;
            pool.Enqueue(new PoolObj<V> { releaseTime = releaseTime, obj = obj });
        }

        public V Get()
        {
            if (pool.Count <= 0)
            {
                if (createAction == null)
                    return default;
                return createAction();
            }

            return pool.Dequeue().obj;
        }

        public override void Update()
        {
            if (expireTime <= 0)
                return;

            var time = Time.unscaledTime;
            if (time < nextReleaseTime)
                return;

            // 判断有没有对象过期
            nextReleaseTime = float.MaxValue;
            bool hasObj = false;
            while (pool.Count > 0)
            {
                var objInfo = pool.Peek();
                if (time >= objInfo.releaseTime)
                {
                    pool.Dequeue();
                    releaseAction?.Invoke(objInfo.obj);
                }
                else
                {
                    hasObj = true;
                    nextReleaseTime = objInfo.releaseTime;
                    break;
                }
            }
            if (!hasObj)
                nextReleaseTime = 0;
        }

        public override void Clear()
        {
            while (pool.Count > 0)
                releaseAction?.Invoke(pool.Dequeue().obj);
        }

        public override void Destroy()
        {
            Clear();
            createAction = null;
            recycleAction = null;
            releaseAction = null;
        }
    }
}