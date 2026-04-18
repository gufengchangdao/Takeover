using System;
using System.Collections.Generic;
using GameFramework.Hot;
using TableStructure;

namespace Takeover
{
    public class CampaignSelectControl : BaseControl
    {
        public Dictionary<ECamp, LevelData> LevelDatas { get; private set; } = new();
        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            LevelDatas.Clear();
            foreach (ECamp camp in Enum.GetValues(typeof(ECamp)))
                LevelDatas[camp] = LevelData.LoadData(camp);
        }
    }
}