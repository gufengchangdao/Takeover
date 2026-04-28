using System.Collections.Generic;
using UnityEngine;

namespace Takeover
{
    public partial class Army
    {
        public List<Army> Targets { get; private set; } = new();

        public bool HasTarget => Targets.Count > 0;

        public float AttackRange { get; private set; }
        private float RadiusEnemySensor => Mathf.Max(AttackRange, 0.9f);

        private void UpdateNearTargets()
        {
            Targets.Clear();
            if (IsAllUnitDead)
                return;

            var armies = Global.LevelLogic.Armies;
            float maxDist2 = Mathf.Pow(RadiusEnemySensor + 0.4f, 2);
            var pos = MainUnitPosition;
            for (int i = 0; i < armies.Count; i++)
            {
                var target = armies[i];
                if (target.IsAllUnitDead || target.Camp == Camp) continue;

                float dist2 = (target.MainUnitPosition - pos).sqrMagnitude + target.Radius * target.Radius;
                if (dist2 < maxDist2)
                    Targets.Add(target);
            }
        }
    }
}