using System.Collections.Generic;
using UnityEngine;

namespace Takeover
{
    public static class UnitUtility
    {
        public static bool FindClosestTarget<T>(List<T> targets, Vector2 position, out int targetIndex, out float distance) where T : Component
        {
            targetIndex = -1;
            distance = float.MaxValue;
            if (targets == null || targets.Count == 0)
                return false;

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                    continue;

                Vector2 targetPosition = targets[i].transform.position;
                float dist = Vector2.Distance(position, targetPosition);
                if (dist < distance)
                {
                    distance = dist;
                    targetIndex = i;
                }
            }
            return targetIndex >= 0;
        }
    }
}