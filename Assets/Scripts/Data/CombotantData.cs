using System;
using GameFramework.Hot;
using TableStructure;
using UnityEngine;

namespace Takeover
{
    public class CombotantData
    {
        public const int MAX_MANA = 500;


        /// <summary>
        /// 阵营
        /// </summary>
        public ECamp Camp { get; private set; }

        /// <summary>
        /// 拥有的小队数量
        /// </summary>
        public int ArmyOwneds { get; private set; }

        /// <summary>
        /// 金币
        /// </summary>
        public float Gold { get; private set; }
        public float BaseGoldSpeed { get; private set; }

        public float GoldSpeed
        {
            get
            {
                if (ArmyOwneds == 0 || GoldSpeed == 0)
                    return GoldSpeed;

                //小队越多，金币会被扣得越狠
                var sqCount = Mathf.Min(ArmyOwneds, 10) - 1;
                var factor = (100 - (30 + sqCount * 5)) / 100;
                return Mathf.Round(Mathf.Max(GoldSpeed * factor, 1));
            }
        }

        /// <summary>
        /// 法力
        /// </summary>
        public int Mana { get; private set; }
        public int ManaSpeed { get; private set; }

        /// <summary>
        /// 军队总补给
        /// </summary>
        public int SupplyPower { get; private set; }

        /// <summary>
        /// 当前军队已经占用的补给
        /// </summary>
        public int ArmyPower { get; private set; }

        /// <summary>
        /// 终极技能充能需要时间
        /// </summary>
        public int UltimateMaxTime { get; private set; }
        /// <summary>
        /// 当前终极技能已积累的充能值
        /// </summary>
        public int UltimateCurTime { get; private set; }

        public int UltimateTimeSpeed { get; private set; }

        public void AddResCounters()
        {
            Gold += GoldSpeed;
            Mana = Math.Min(Mana + ManaSpeed, MAX_MANA);
            UltimateCurTime = Mathf.Min(UltimateCurTime + UltimateTimeSpeed, UltimateMaxTime);
        }

        /// <summary>
        /// 是否可以购买该小队
        /// </summary>
        public bool CheckCanBuy(string armyId)
        {
            var armyData = GFGlobal.Tables.TbArmyData[armyId];
            return Gold >= armyData.Cost && SupplyPower - ArmyPower >= armyData.Upkeep;
        }
    }
}