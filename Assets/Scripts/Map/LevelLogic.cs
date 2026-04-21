using System.Collections.Generic;
using TableStructure;
using UnityEngine;

namespace Takeover
{
    public partial class LevelLogic : UpdateableComponent
    {
        public List<Castle> Castles { get; private set; } = new();
        public List<Army> Armys { get; private set; } = new();
        public List<CombotantData> Combotants { get; private set; } = new();

        private bool lockAI = false;

        private float lifeTime = 0;

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            lifeTime += dt;
            if (!(Mathf.Floor(lifeTime) > lifeTime))
                return; //每秒更新一次

            // 增加资源
            for (int i = 0; i < Combotants.Count; i++)
                Combotants[i].AddResCounters();

            if (!lockAI)
                UpdateCpuCombotantLogic(1);
        }

        /// <summary>
        /// 计算指定阵营小队补给占用
        /// </summary>
        public int CalcCombotantSquadsUpkeep(ECamp camp)
        {
            int upkeep = 0;

            for (int i = 0; i < Armys.Count; i++)
            {
                if (Armys[i].Camp == camp)
                    upkeep += Armys[i].Upkeep;
            }
            return upkeep;
        }

        /// <summary>
        /// 计算两个城堡之间的路程
        /// </summary>
        private float GetCastleDistanceSq(Castle castle1, Castle castle2)
        {
            return 0;
        }
    }
}