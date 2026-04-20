using GameFramework.AOT;
using GameFramework.Hot;
using TMPro;
using UnityEngine;

namespace Takeover
{
    public class LevelSelectView : BaseView<LevelSelectControl>
    {
        [SerializeField] private GFButton btnReturn;
        [SerializeField] private GFButton btnSkill;
        [SerializeField] private TextMeshProUGUI txtCampaignName;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            BtnOnClick(btnReturn, e =>
            {
                GFGlobal.Procedure.ChangeState<ProcedureStart>();
            });
            BtnOnClick(btnSkill, e =>
            {
                Log.Error("TODO技能面板");
            });

            var camp = Global.LevelData.Camp;
            txtCampaignName.text = GFGlobal.Tables.TbCampaignData[camp].ShowName;
        }

    }
}