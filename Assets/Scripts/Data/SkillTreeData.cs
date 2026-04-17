using System.Collections.Generic;

namespace Takeover
{
    public class SkillTreeData : IDataSaveLoad
    {
        private Dictionary<string, object> skillTreeData = new();

        public void OnLoad()
        {
        }

        public bool ArmyIsUnlock(string armyId)
        {
            return skillTreeData.ContainsKey($"army_{armyId}_unlock");
        }

        public void OnSave() { }
    }
}