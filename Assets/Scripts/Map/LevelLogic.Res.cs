using GameFramework.Hot;
using TableStructure;
using UnityEngine;

namespace Takeover
{
    public partial class LevelLogic
    {
        public int GetGoldSpeedPenalty(ECamp camp, int baseGoldSpeed)
        {
            if (baseGoldSpeed == 0)
                return 0;

            int armyCount = GetArmyCount(camp);
            if (armyCount == 0)
                return 0;

            //小队越多，金币会被扣得越狠
            var sqCount = Mathf.Min(armyCount, 10) - 1;
            float factor = (100 - (30 + sqCount * 5)) / 100;
            int goldSpeed = Mathf.RoundToInt(Mathf.Max(baseGoldSpeed * factor, 1));
            return baseGoldSpeed - goldSpeed;
        }

        // 城堡状态、建筑状态、小队增减变化时更新
        private void UpdateResSpeed(ECamp camp)
        {
            int baseGoldSpeed = 0;
            int manaSpeed = 0;
            int supplyPower = 0;
            int ultimateTimeSpeed = 0;

            // 城堡
            for (int i = 0; i < Castles.Count; i++)
            {
                var castle = Castles[i];
                if (castle.Camp == camp && castle.IsActive)
                {
                    var castleData = GFGlobal.Tables.TbCastleData[castle.TableId];
                    baseGoldSpeed += castleData.GoldSpeed;
                    manaSpeed += castleData.ManaSpeed;
                    supplyPower += castleData.SupplyPower;
                    ultimateTimeSpeed += castleData.UltimateTimeSpeed;
                }
            }

            // 建筑
            for (int i = 0; i < Buildings.Count; i++)
            {
                if (Buildings[i].Camp == camp && Buildings[i].IsActive)
                {
                    var buildingData = GFGlobal.Tables.TbBuildingData[Buildings[i].TableId];
                    baseGoldSpeed += buildingData.GoldSpeed;
                    manaSpeed += buildingData.ManaSpeed;
                    supplyPower += buildingData.SupplyPower;
                    ultimateTimeSpeed += buildingData.UltimateTimeSpeed;
                }
            }

            // 小队
            int armyPower = 0;
            for (int i = 0; i < Armys.Count; i++)
            {
                var armyData = GFGlobal.Tables.TbArmyData[Armys[i].TableId];
                armyPower += armyData.Upkeep;
            }

            var combotant = Combotants[camp];

            combotant.BaseGoldSpeed.Value = baseGoldSpeed;
            combotant.GoldSpeedPenalty.Value = GetGoldSpeedPenalty(camp, combotant.BaseGoldSpeed.Value);
            combotant.GoldSpeed.Value = baseGoldSpeed - combotant.GoldSpeedPenalty.Value;

            combotant.ManaSpeed.Value = manaSpeed;
            combotant.UltimateTimeSpeed = ultimateTimeSpeed;
            combotant.SupplyPower.Value = supplyPower;
            combotant.ArmyPower.Value = armyPower;
        }

        private void OnCastleStateChange(object sender, OnCastleStateChange data)
        {
            UpdateResSpeed(data.Castle.Camp);
        }
        private void OnBuildingStateChange(object sender, OnBuildingStateChange data)
        {
            UpdateResSpeed(data.Building.Camp);
        }
    }
}