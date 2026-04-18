using System;
using System.Collections.Generic;
using GameFramework.Hot;
using TableStructure;
using TMPro;
using UnityEngine;

namespace Takeover
{
    public class CampaignSelectView : BaseView<CampaignSelectControl>
    {
        [SerializeField] private List<CampaignSelectUI> campaigns;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            foreach (var campaign in campaigns)
            {
                InitCampaignUI(campaign.camp, campaign.txtProgress);
                campaign.btn.onClick.AddEventListener(e =>
                {
                    Global.LevelData.SetLevelData(Control.LevelDatas[campaign.camp]);
                    Close();
                    GFGlobal.Procedure.StartProcudre<LevelSelectProcedure>();
                });
            }
        }

        private void InitCampaignUI(ECamp camp, TextMeshProUGUI txtProgress)
        {
            var levelData = Control.LevelDatas[camp];
            txtProgress.text = $"{levelData.CurrentLevel}/{levelData.LevelCount}";
        }

        public override void OnRecycle()
        {
            foreach (var campaign in campaigns)
                campaign.btn.onClick.Clear();
            base.OnRecycle();
        }

        [Serializable]
        private class CampaignSelectUI
        {
            public ECamp camp;
            public GFButton btn;
            public TextMeshProUGUI txtProgress;
        }
    }
}