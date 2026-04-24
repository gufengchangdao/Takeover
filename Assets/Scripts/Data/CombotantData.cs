using System;
using GameFramework.Hot;
using TableStructure;
using UnityEngine;

namespace Takeover
{
    public class CombotantData
    {
        public const int MAX_MANA = 500;

        public bool IsPlayer { get; set; }

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
        public BindableProperty<int> Gold = new();
        public BindableProperty<int> BaseGoldSpeed = new();
        /// <summary>
        /// 金币惩罚，为正数
        /// </summary>
        public BindableProperty<int> GoldSpeedPenalty = new();
        public BindableProperty<int> GoldSpeed = new();

        /// <summary>
        /// 法力
        /// </summary>
        public BindableProperty<int> Mana = new();
        public BindableProperty<int> ManaSpeed = new();

        /// <summary>
        /// 军队总补给
        /// </summary>
        public BindableProperty<int> SupplyPower = new();

        /// <summary>
        /// 当前军队已经占用的补给
        /// </summary>
        public BindableProperty<int> ArmyPower = new();

        /// <summary>
        /// 终极技能充能需要时间
        /// </summary>
        public int UltimateMaxTime { get; private set; }
        /// <summary>
        /// 当前终极技能已积累的充能值
        /// </summary>
        public int UltimateCurTime { get; private set; }

        public int UltimateTimeSpeed { get; set; }

        public CombotantData(ECamp camp)
        {
            Camp = camp;
        }

        public void AddResCounters()
        {
            Gold.Value += GoldSpeed.Value;
            Mana.Value = Math.Min(Mana.Value + ManaSpeed.Value, MAX_MANA);
            UltimateCurTime = Mathf.Min(UltimateCurTime + UltimateTimeSpeed, UltimateMaxTime);
        }

        /// <summary>
        /// 是否可以购买该小队
        /// </summary>
        public bool CheckCanBuy(string armyId)
        {
            var armyData = GFGlobal.Tables.TbArmyData[armyId];
            return Gold.Value >= armyData.Cost && SupplyPower.Value - ArmyPower.Value >= armyData.Upkeep;
        }

        public void OnBuyArmy(string armyId)
        {
            var armyData = GFGlobal.Tables.TbArmyData[armyId];
            Gold.Value = Mathf.Max(0, Gold.Value - armyData.Cost);
        }

        public bool CheckCanUpgradeCastle(Castle castle)
        {
            var need = castle.NextLevelCost;
            return need != -1 && Gold.Value > need;
        }
    }
}