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

            Global.CombotantData.Gold.AddOnChange(UpdateGold);
            Global.CombotantData.GoldSpeed.AddOnChange(UpdateGold);
            Global.CombotantData.Mana.AddOnChange(UpdateMana);
            Global.CombotantData.SupplyPower.AddOnChange(UpdateSupply);
            Global.CombotantData.ArmyPower.AddOnChange(UpdateSupply);
            Global.LevelLogic.LifeTime.AddOnChange(UpdateGameTime);
            BtnOnClick(btnSpeed, e =>
            {
                Control.ChangeGameSpeed();
            });


            // 技能
            // TODO

            OnGameSpeedChange(null, null);
            UpdateGold();
            UpdateMana();
            UpdateSupply();
            UpdateGameTime(Global.LevelLogic.LifeTime.Value, Global.LevelLogic.LifeTime.Value);
        }

        public override void OnRecycle()
        {
            GFGlobal.Event.Unsubscribe<GameSpeedChangeEvent>(OnGameSpeedChange);
            Global.CombotantData.Gold.RemoveOnChange(UpdateGold);
            Global.CombotantData.GoldSpeed.RemoveOnChange(UpdateGold);
            Global.CombotantData.Mana.RemoveOnChange(UpdateMana);
            Global.CombotantData.SupplyPower.RemoveOnChange(UpdateSupply);
            Global.CombotantData.ArmyPower.RemoveOnChange(UpdateSupply);
            Global.LevelLogic.LifeTime.RemoveOnChange(UpdateGameTime);
            base.OnRecycle();
        }

        private void OnGameSpeedChange(object sender, GameSpeedChangeEvent data)
        {
            imgSpeed.SetImageByStatus($"Speed{Global.LevelLogic.GameSpeed}");
        }

        private void UpdateGold()
        {
            var speed = Global.CombotantData.GoldSpeed.Value;
            txtGold.text = $"{Global.CombotantData.Gold.Value}<color=#FFB23D>({(speed > 0 ? "+" : (speed < 0 ? "-" : ""))}{speed})</color>";
        }

        private void UpdateSupply()
        {
            txtSupply.text = $"{Global.CombotantData.ArmyPower}/<color=#22AE2E>{Global.CombotantData.SupplyPower}</color>";
            txtSupplyPenalty.text = $"-{Global.CombotantData.GoldSpeedPenalty}";
        }

        private void UpdateMana()
        {
            var speed = Global.CombotantData.ManaSpeed.Value;
            txtMana.text = $"{Global.CombotantData.Mana}<color=#3D70FF>({(speed > 0 ? "+" : (speed < 0 ? "-" : ""))}{speed})</color>";
        }

        private void UpdateGameTime(float cur, float old)
        {
            txtTime.text = TimeUtility.FormatElapsedTime(Mathf.FloorToInt(cur));
        }
    }
}