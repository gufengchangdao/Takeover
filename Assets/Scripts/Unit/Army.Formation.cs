using GameFramework.AOT;
using GameFramework.Hot;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace Takeover
{
    public partial class Army
    {
        // 阵型数组，主单位位于阵型最前面的中心线上，其他单位位于主单位两侧
        private const int DEFAULT_UNIT_DIST = 14;
        private static readonly Vector2[][] FORMATIONS_OFFSET = new Vector2[][]
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

        //角度和距离数据
        private Vector2[] formation;

        private void InitFormation()
        {
            Vector2[] offsets = null;
            for (int i = 0; i < FORMATIONS_OFFSET.Length; i++)
            {
                if (FORMATIONS_OFFSET[i].Length >= UnitCount)
                {
                    offsets = FORMATIONS_OFFSET[i];
                    break;
                }
            }

            if (offsets == null)
            {
                Log.Error("超过阵型数上限! {0}", UnitCount);
                var formation = new Vector2[UnitCount];
                for (int i = 0; i < UnitCount; i++)
                    formation[i] = Vector2.zero;
                return;
            }

            formation = new Vector2[offsets.Length];
            for (int j = 0; j < offsets.Length; j++)
            {
                var psn = offsets[j];
                float dist = GFGlobal.GlobalTableData.UnitDist / DEFAULT_UNIT_DIST * Mathf.Sqrt(psn.x * psn.x + psn.y * psn.y);
                Vector2 polyPoInt = new(Mathf.Atan2(psn.y, psn.x), dist);
                formation[j] = polyPoInt;
            }
        }

        public Vector2 GetOtherUnitTargetPosition(Vector2 pos, int index, float curAngl)
        {
            var polyPoInt = formation[index];
            float angl = polyPoInt.x + curAngl;
            float dist = polyPoInt.y;
            return new Vector2(pos.x + Mathf.Cos(angl) * dist, pos.y + Mathf.Sin(angl) * dist);
        }

        /// <summary>
        /// 给主单位找下一个到达的节点
        /// </summary>
        public void SendToNextNode()
        {
            if (CurPathList == null || CurPathList.Count == 0)
                return;

            while (CurPathList.Count > 0)
            {
                int nodeIndex = CurPathList[CurPathList.Count - 1];
                var targetPos = Global.MapPath.GetNodePosition(nodeIndex);
                var mainUnit = GetMainUnit();
                mainUnit.TargetPos = targetPos; //下一个目标点分配给主单位
                if (mainUnit.AtTarget)
                {
                    Log.Debug($"单位{TableId}到达节点{CurPathList[CurPathList.Count - 1]}，开始下一个节点");
                    CurPathList.RemoveAt(CurPathList.Count - 1); //到了才会移除
                }
                else
                {
                    // 给所有单位分配坐标
                    var dPos = targetPos - mainUnit.Position;
                    var curAngl = mainUnit.TargetAngle;
                    if (!(dPos.x == 0 && dPos.y == 0))
                        curAngl = Mathf.Atan2(dPos.y, dPos.x);
                    int k = 0;
                    for (int i = 0; i < Units.Count; i++)
                    {
                        var unit = Units[i];
                        if (!unit.IsDead)
                        {
                            unit.TargetPos = GetOtherUnitTargetPosition(targetPos, k, curAngl);
                            unit.ResetImpulse();
                            k++;
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 把各个士兵往主单位周围的编队位置推一点
        /// </summary>
        public void RepairFormation()
        {
            int k = 1; //跳过第一个主单位
            var mainUnit = GetMainUnit();
            var pos = mainUnit.Position;
            float curAngl = mainUnit.TargetAngle;
            for (int i = 0; i < Units.Count; i++)
            {
                var unit = Units[i];
                if (!unit.IsDead && unit != mainUnit)
                {
                    var targetPos = GetOtherUnitTargetPosition(pos, k, curAngl);
                    PushUnitTo(unit, targetPos.x, targetPos.y, unit.Speed * 0.01f);
                    k++;
                }
            }
        }

        /// <summary>
        /// 拉进单位
        /// </summary>
        private void PushUnitTo(Unit unit, float x, float y, float power)
        {
            var pos = unit.Position;
            float dx = x - pos.x;
            float dy = y - pos.y;
            var dist = Mathf.Sqrt(dx * dx + dy * dy);
            if (dist < power)
            {
                unit.Position = new Vector2(x, y);
            }
            else
            {
                var angl = Mathf.Atan2(dy, dx);
                unit.AddMoveImpulse(Mathf.Cos(angl) * power, Mathf.Sin(angl) * power);
            }
        }
    }
}