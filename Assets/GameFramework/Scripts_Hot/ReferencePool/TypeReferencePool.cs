using System;

namespace GameFramework.Hot
{
    /// <summary>
    /// 用于IRecyclable对象的引用池，不会自动回收
    /// </summary>
    public static class TypeReferencePool
    {
        public static KeyedReferencePool<Type, IRecyclable> TypePool = new(-1, -1, obj => obj.OnRecycle());

        public static T GetOrNew<T>() where T : class, new()
        {
            var obj = TypePool.Get(typeof(T));
            if (obj != null)
                return obj as T;
            return new T();
        }

        public static void Recycle(IRecyclable recyclable)
        {
            TypePool.Recycle(recyclable.GetType(), recyclable);
        }
    }
}