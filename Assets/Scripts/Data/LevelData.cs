using System.Collections.Generic;
using TableStructure;

namespace Takeover
{
    public class LevelData : IDataSaveLoad
    {
        public ECamp Camp { get; private set; }

        /// <summary>
        /// 关卡总数
        /// </summary>
        public int LevelCount { get; private set; }

        public int CurrentLevel { get; private set; }

        /// <summary>
        /// 战胜过的关卡评级
        /// </summary>
        public List<int> LevelScore { get; private set; }

        public void OnLoad()
        {
        }

        public void OnSave()
        {
        }
    }
}