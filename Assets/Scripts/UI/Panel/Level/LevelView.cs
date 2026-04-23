using GameFramework.Hot;
using UnityEngine;

namespace Takeover
{
    public class LevelView : BaseView<LevelControl>
    {
        [SerializeField] private GFButton btnSpeed;
        [SerializeField] private GFImage imgSpeed;
        [SerializeField] private GFText txtTime;

        [SerializeField] private BtnSkillNode skillCopy;
        [SerializeField] private GFText txtGold;
        [SerializeField] private GFText txtSupply;
        [SerializeField] private GFText txtSupplyPenalty;
        [SerializeField] private GFText txtMana;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            GFGlobal.Event.Subscribe<GameSpeedChangeEvent>(OnGameSpeedChange);

            BtnOnClick(btnSpeed, e =>
            {
                Control.ChangeGameSpeed();
            });


            // 技能
            // TODO

            OnGameSpeedChange(null, null);
            OnGoldChange();
            OnSupplyChange();
            OnSupplyPenaltyChange();
            OnManaChange();
            UpdateGameTime();

            Tick(1, UpdateGameTime);
        }

        public override void OnRecycle()
        {
            GFGlobal.Event.Unsubscribe<GameSpeedChangeEvent>(OnGameSpeedChange);

            base.OnRecycle();
        }

        private void OnGameSpeedChange(object sender, GameSpeedChangeEvent data)
        {
            imgSpeed.SetImageByStatus($"Speed{Global.LevelLogic.GameSpeed}");
        }

        private void OnGoldChange()
        {
            var speed = Global.CombotantData.GoldSpeed;
            txtGold.text = $"{Global.CombotantData.Gold}<color=#FFB23D>({(speed > 0 ? "+" : (speed < 0 ? "-" : ""))}{speed})</color>";
        }

        private void OnSupplyChange()
        {
            txtSupply.text = "0<color=#22AE2E>(+3)</color>";
        }

        private void OnSupplyPenaltyChange()
        {
            txtSupplyPenalty.text = "-1";
        }

        private void OnManaChange()
        {
            txtMana.text = "0<color=#3D70FF>(+2)</color>";
        }

        private void UpdateGameTime()
        {
            txtTime.text = TimeUtility.FormatElapsedTime(Mathf.FloorToInt(Global.LevelLogic.LifeTime));
        }
    }
}