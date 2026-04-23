using System.Numerics;
using GameFramework.AOT;

namespace Takeover
{
    public partial class Army
    {
        /// <summary>
        /// 单位之间的距离
        /// </summary>
        public const float UNIT_DIST = 14;

        // 阵型数组，主单位位于阵型最前面的中心线上，其他单位位于主单位两侧
        private const int DEFAULT_UNIT_DIST = 14;
        private static readonly Vector2[][] FORMATIONS = new Vector2[][]
        {
            // 1
            new Vector2[] { new(0, 0) },
            // 3
            new Vector2[] { new(0, 0), new(-14, 7), new(-14, -7) },
            // 5
            new Vector2[] { new(0, 0), new(-21, 14), new(-21, -14), new(-42, 14), new(-42, -14) },
            // 6
            new Vector2[] { new(0, 0), new(-14, 10.5f), new(-14, -10.5f), new(-21, 0), new(-31.5f, 10.5f), new(-31.5f, -10.5f) },
            // 7
            new Vector2[] { new(0, 0), new(-21, 14), new(-21, -14), new(-42, 14), new(-42, -14), new(-63, 14), new(-63, -14) },
            // 12
            new Vector2[] { new(0, 0), new(0, 14), new(0, -14), new(-14, 0), new(-14, 14), new(-14, -14), new(-28, 0), new(-28, 14), new(-28, -14), new(-42, 0), new(-42, 14), new(-42, -14) },
        };

        private Vector2[] GetFormationType(int unitCount)
        {
            for (int i = 0; i < FORMATIONS.Length; i++)
            {
                if (FORMATIONS[i].Length >= unitCount)
                    return FORMATIONS[i];
            }

            Log.Error("超过阵型数上限! {0}", unitCount);
            var formation = new Vector2[unitCount];
            for (int i = 0; i < unitCount; i++)
                formation[i] = Vector2.Zero;
            return formation;
        }

        private Vector2 GetUnitOffsetPosition(int unitIndex)
        {
            var formation = GetFormationType(Units.Count);
            return UNIT_DIST / DEFAULT_UNIT_DIST * formation[unitIndex];
        }
    }
}