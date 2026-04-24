using TableStructure;

namespace Takeover
{
    public partial class LevelLogic
    {
        public Army CreateArmy(ECamp camp, string armyId)
        {



            Army army = new(camp);
            UpdateResSpeed(camp);
            return army;
        }

        public void RemoveArmy(Army army)
        {

            UpdateResSpeed(army.Camp);
        }

        public int GetArmyCount(ECamp camp)
        {
            int count = 0;
            for (int i = 0; i < Armys.Count; i++)
            {
                if (Armys[i].Camp == camp)
                    count++;
            }
            return count;
        }

        public Army BuyArmy(Castle castle, string armyId)
        {
            ECamp camp = castle.Camp;
            Combotants[camp].OnBuyArmy(armyId);
            var army = CreateArmy(camp, armyId);



            return army;
        }
    }
}