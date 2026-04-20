using GameFramework.Hot;
using TMPro;
using UnityEngine;

namespace Takeover
{
    public class LevelIntroduceView : BaseView<LevelIntroduceControl>
    {
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TextMeshProUGUI txtContent;
        [SerializeField] private GFButton btnStart;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            Global.TableCache.GetLevelInfo(Global.LevelData.Camp, Control.Level, out string title, out string content);
            txtTitle.text = title;
            txtContent.text = content;

            btnStart.onClick.AddEventListener(e =>
            {
                GFGlobal.Procedure.ChangeState<ProcedureLevel>(Control.Level);
            });
        }

        public override void OnRecycle()
        {
            btnStart.onClick.Clear();
            base.OnRecycle();
        }
    }
}