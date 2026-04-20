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

        [SerializeField] private GFButton returnBtn;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            foreach (var campaign in campaigns)
            {
                InitCampaignUI(campaign.camp, campaign.txtProgress);
                BtnOnClick(campaign.btn, e =>
                {
                    Global.LevelData.SetLevelData(Control.LevelDatas[campaign.camp]);
                    Close();
                    GFGlobal.Procedure.ChangeState<ProcedureLevelSelect>();
                });
            }
            BtnOnClick(returnBtn, e =>
            {
                Close();
                GFGlobal.UI.OpenPanel<MainMenuControl>();
            });
        }

        private void InitCampaignUI(ECamp camp, TextMeshProUGUI txtProgress)
        {
            var levelData = Control.LevelDatas[camp];
            txtProgress.text = $"{levelData.CurrentLevel}/{levelData.LevelCount}";
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