using TableStructure;
using GameFramework.Hot;
using System.Collections.Generic;

namespace Takeover
{
    public class CastleOperateControl : BaseControl
    {
        // 选中的城堡
        public Castle Castle { get; private set; }
        public ECamp Camp => Castle ? Castle.Camp : default;

        public List<string> showArmies;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            Castle = userData as Castle;

            var castleData = GFGlobal.Tables.TbCastleData[Castle.castleId];

            Dictionary<string, (string, int)> temp = new();
            foreach (var soldierType in castleData.Armies)
            {
                foreach (var armyData in GFGlobal.Tables.TbArmyData.DataList)
                {
                    if (armyData.UnitType != soldierType)
                        continue; //类型不同

                    if (!armyData.Camp.Contains(Camp))
                        continue; //阵营不同

                    string baseArmy = string.IsNullOrEmpty(armyData.BaseArmy) ? armyData.Id : armyData.BaseArmy;
                    if (temp.ContainsKey(baseArmy))
                    {
                        if (armyData.Tech <= temp[baseArmy].Item2)
                            continue; //等级比原来的还低

                        if (!Global.SkillTree.ArmyIsUnlock(armyData.Id))
                            continue; //未解锁
                    }

                    temp[baseArmy] = (armyData.Id, armyData.Tech);
                }
            }

            showArmies = new();
            foreach (var part in temp)
                showArmies.Add(part.Value.Item1);
        }

        public override void OnRecycle()
        {
            Castle = null;
            base.OnRecycle();
        }
    }
}