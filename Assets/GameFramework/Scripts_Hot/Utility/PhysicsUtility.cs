using System.Collections.Generic;
using GameFramework.AOT;
using UnityEngine;

namespace GameFramework.Hot
{
    /// <summary>
    /// 不能嵌套查找
    /// </summary>
    public static class Physics2DUtility
    {
        private static readonly Physics2DResult data = new();

        public static Physics2DResult OverlapCircleNonAlloc(Vector2 point,
            float radius,
            int layerMask = Physics2D.DefaultRaycastLayers)
        {
            data.Count = Physics2D.OverlapCircleNonAlloc(point, radius, data.results, layerMask);
            return data;
        }

        public static Physics2DResult OverlapBoxNonAlloc(Vector2 point,
            Vector2 size,
            float angle,
            int layerMask = Physics2D.DefaultRaycastLayers)
        {
            int count = Physics2D.OverlapBoxNonAlloc(point, size, angle, data.results, layerMask);
            data.Count = count;
            return data;
        }

        public class Physics2DResult
        {
            private int lastIndex; //用这个检查是不是嵌套查找了
            public int Count { get; internal set; }
            internal Collider2D[] results = new Collider2D[256];

            public IEnumerator<Collider2D> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    if (results[i] != null)
                        yield return results[i];
            }

            public Collider2D this[int index]
            {
                get
                {
                    if (index < 0 || index >= results.Length)
                    {
                        Log.Error("[Physics] 索引超出范围: {0}", index);
                        return null;
                    }
                    if (index != 0 && index != lastIndex + 1)
                        Log.Error("[Physics] 索引不连续，是否嵌套查找了？: last index: {0}, index: {1}", lastIndex, index);
                    lastIndex = index;

                    return results[index];
                }
            }
        }
    }

    public static class Physics3DUtility
    {

    }
}