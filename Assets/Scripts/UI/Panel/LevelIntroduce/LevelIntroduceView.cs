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

            var levelData = Global.TableCache.GetLevelData(Global.LevelData.Camp, Control.Level);
            txtTitle.text = levelData.Title;
            txtContent.text = levelData.Content;

            btnStart.onClick.AddEventListener(e =>
            {
                GFGlobal.Procedure.ChangeState<ProcedureLevel>(levelData.Id);
                Close();
            });
        }

        public override void OnRecycle()
        {
            btnStart.onClick.Clear();
            base.OnRecycle();
        }
    }
}