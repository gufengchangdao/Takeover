using GameFramework.Hot;
using TableStructure;
using UnityEngine;

namespace Takeover
{
    public partial class LevelLogic
    {
        public Army CreateArmy(ECamp camp, string armyId)
        {
            var go = GFGlobal.Resource.InstantiatePrefab(GFGlobal.GlobalTableData.ArmyPrefabPath, ArmyTransform);
            Army army = go.GFGetOrAddComponent<Army>();
            army.Init(armyId, camp);
            Armies.Add(army);
            UpdateResSpeed(camp);
            return army;
        }

        public void RemoveArmy(Army army)
        {
            Armies.Remove(army);
            GameObject.Destroy(army);
            UpdateResSpeed(army.Camp);
        }

        public int GetArmyCount(ECamp camp)
        {
            int count = 0;
            for (int i = 0; i < Armies.Count; i++)
            {
                if (Armies[i].Camp == camp)
                    count++;
            }
            return count;
        }

        public bool BuyArmy(Castle castle, string armyId)
        {
            ECamp camp = castle.Camp;
            if (!Combotants[camp].CheckCanBuy(armyId))
                return false;

            Combotants[camp].OnBuyArmy(armyId);
            var army = CreateArmy(camp, armyId);
            army.EnterCastle(castle, true);
            return true;
        }
    }
}