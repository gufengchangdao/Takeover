using System.Collections.Generic;

namespace Takeover
{
    public class SkillTreeData
    {
        private Dictionary<string, object> skillTreeData = new();

        public void Init()
        {
            
        }

        public bool ArmyIsUnlock(string armyId)
        {
            return skillTreeData.ContainsKey($"army_{armyId}_unlock");
        }
    }
}