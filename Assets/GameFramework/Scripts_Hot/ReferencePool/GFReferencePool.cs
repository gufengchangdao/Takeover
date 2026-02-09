using System;
using System.Collections.Generic;

namespace GameFramework.Hot
{
    public class GFReferencePool : GFBaseModule
    {
        /// <summary>
        /// IRecyclable全局引用池
        /// </summary>
        private KeyedReferencePool<Type, IRecyclable> __TypeReferencePool => TypeReferencePool.TypePool;

        /// <summary>
        /// 引用池集合
        /// </summary>
        private Dictionary<string, AbstractReferencePool> pools = new Dictionary<string, AbstractReferencePool>();

        public ReferencePool<T> GetPool<T>(string name)
        {
            if (pools.TryGetValue(name, out var pool))
                return pool as ReferencePool<T>;
            return null;
        }

        /// <summary>
        /// 用于IRecyclable子类
        /// </summary>
        public ReferencePool<V> CreatePool<V>(string name,
                                               int capacity,
                                               float expireTime,
                                               Func<V> createAction = null,
                                               Action<V> releaseAction = null) where V : IRecyclable
        {
            return CreatePool(name, capacity, expireTime, obj => obj.OnRecycle(), createAction, releaseAction);
        }

        public ReferencePool<V> CreatePool<V>(string name,
                                                int capacity,
                                                float expireTime,
                                                Action<V> recycleAction,
                                                Func<V> createAction = null,
                                                Action<V> releaseAction = null)
        {
            var pool = new ReferencePool<V>(capacity, expireTime, recycleAction, createAction, releaseAction);
            pools.Add(name, pool);
            return pool;
        }

        public KeyedReferencePool<K, V> CreatePool<K, V>(string name,
                                                int capacity,
                                                float expireTime,
                                                Action<V> releaseAction = null) where V : class,IRecyclable
        {
            var pool = new KeyedReferencePool<K, V>(capacity, expireTime, obj => obj.OnRecycle(), releaseAction);
            pools.Add(name, pool);
            return pool;
        }

        public KeyedReferencePool<K, V> CreatePool<K, V>(string name,
                                                int capacity,
                                                float expireTime,
                                                Action<V> recycleAction,
                                                Action<V> releaseAction = null) where V : class
        {
            var pool = new KeyedReferencePool<K, V>(capacity, expireTime, recycleAction, releaseAction);
            pools.Add(name, pool);
            return pool;
        }

        public void DestroyPool(AbstractReferencePool pool)
        {
            string name = null;
            foreach (var part in pools)
            {
                if (part.Value == pool)
                {
                    name = part.Key;
                    break;
                }
            }

            if (name != null)
                pools.Remove(name);
            pool.Destroy();
        }

        public override void ModuleUpdate()
        {
            base.ModuleUpdate();
            foreach (var pool in pools.Values)
                pool.Update();
        }

        // 不能随便调用，有些组件可能已经销毁，触发回调说不定会报错
        // void OnDestroy()
        // {
        //     foreach (var pool in pools.Values)
        //         pool.Destroy();
        //     pools.Clear();
        // }
    }
}