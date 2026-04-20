using GameFramework.Hot;
using UnityEngine.UI;
using UnityEngine;

namespace Takeover
{
    public class SettingView : BaseView<SettingControl>
    {
        [SerializeField] private GFSlider musicSlider;
        [SerializeField] private GFSlider sfxSlider;
        [SerializeField] private GFButton btnReturnMenu;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            musicSlider.value = GFGlobal.Sound.MusicVolume;
            UpdateSoundSliderMask(musicSlider);
            SliderOnValueChanged(musicSlider, (v) =>
            {
                GFGlobal.Sound.MusicVolume = v;
                UpdateSoundSliderMask(musicSlider);
            });

            sfxSlider.value = GFGlobal.Sound.SFXVolume;
            UpdateSoundSliderMask(sfxSlider);
            SliderOnValueChanged(sfxSlider, (v) =>
            {
                GFGlobal.Sound.SFXVolume = v;
                UpdateSoundSliderMask(sfxSlider);
            });

            btnReturnMenu.gameObject.SetActive(!GFGlobal.UI.HasPanel<MainMenuControl>());
            BtnOnClick(btnReturnMenu, e =>
            {
                GFGlobal.Procedure.ChangeState<ProcedureStart>();
                Close();
            });
        }

        private void UpdateSoundSliderMask(Slider slider)
        {
            slider.transform.Find("Handle Slide Area/Handle/IconDisable").gameObject.SetActive(slider.value == 0);
        }
    }
}