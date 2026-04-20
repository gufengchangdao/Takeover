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

            btnReturn.onClick.AddEventListener(e =>
            {
                GFGlobal.Procedure.ChangeState<ProcedureStart>();
            });
            btnSkill.onClick.AddEventListener(e =>
            {
                Log.Error("TODO技能面板");
            });

            var camp = Global.LevelData.Camp;
            txtCampaignName.text = GFGlobal.Tables.TbCampaignData[camp].ShowName;
        }

        public override void OnRecycle()
        {
            btnReturn.onClick.Clear();
            btnSkill.onClick.Clear();
            base.OnRecycle();
        }
    }
}