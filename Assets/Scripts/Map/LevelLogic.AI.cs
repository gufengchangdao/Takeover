using UnityEngine;

namespace Takeover
{
    public partial class LevelLogic
    {
        // AI 的主循环
        private void UpdateCpuCombotantLogic(float dt)
        {
            for (int i = 0; i < Combotants.Count; i++)
            {
                CombotantData combotantData = Combotants[i];
                if (combotantData.Camp == Global.LevelData.Camp)
                    continue; //这是玩家

                TryGoAttack(combotantData);

            }
        }

        // 尝试出兵进攻
        private void TryGoAttack(CombotantData combotant)
        {
            for (int i = 0; i < Castles.Count; i++)
            {
                Castle castle = Castles[i];
                if (castle.Camp != combotant.Camp)
                    continue;

                if (castle.Defenders.Count > 0)
                {
                    SendToAttackFrom(castle, castle.Defenders[0]); //把第一个驻城的派出去
                    return;
                }
            }
        }

        private void SendToAttackFrom(Castle castle, Army army)
        {
            Castle target = SeekNearestEnemyKeyNode(castle);
            if (target)
                army.CommandGotoTarget(target);
        }

        // 给指定城堡查找派兵目标，不同阵营或者是已经被摧毁了的城堡
        private Castle SeekNearestEnemyKeyNode(Castle castle)
        {
            float minDistSq = Mathf.Infinity;
            Castle target = null;

            for (int i = 0; i < Castles.Count; i++)
            {
                var c = Castles[i];
                if (c.Camp != castle.Camp || c.Health.IsDead)
                {
                    float distSq = GetCastleDistanceSq(castle, c);
                    if (distSq < minDistSq)
                    {
                        minDistSq = distSq;
                        target = c;
                    }
                }
            }
            return target;
        }
    }
}