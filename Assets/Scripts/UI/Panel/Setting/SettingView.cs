using GameFramework.AOT;
using GameFramework.Hot;
using UnityEngine.UI;
using UnityEngine;

namespace Takeover
{
    public class SettingView : BaseView<SettingControl>
    {
        [SerializeField] private SliderPro musicSlider;
        [SerializeField] private SliderPro sfxSlider;
        [SerializeField] private GFButton btnReturnMenu;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            musicSlider.value = GFGlobal.Sound.MusicVolume;
            UpdateSoundSliderMask(musicSlider);
            musicSlider.onValueChanged.AddListener((v) =>
            {
                GFGlobal.Sound.MusicVolume = v;
                UpdateSoundSliderMask(musicSlider);
            });

            sfxSlider.value = GFGlobal.Sound.SFXVolume;
            UpdateSoundSliderMask(sfxSlider);
            sfxSlider.onValueChanged.AddListener((v) =>
            {
                GFGlobal.Sound.SFXVolume = v;
                UpdateSoundSliderMask(sfxSlider);
            });

            btnReturnMenu.gameObject.SetActive(!GFGlobal.UI.HasPanel<MainMenuControl>());
            btnReturnMenu.onClick.AddEventListener(e =>
            {
                GFGlobal.Procedure.ChangeState<ProcedureStart>();
                Close();
            });
        }

        public override void OnRecycle()
        {
            musicSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.RemoveAllListeners();
            btnReturnMenu.onClick.Clear();
            base.OnRecycle();
        }

        private void UpdateSoundSliderMask(Slider slider)
        {
            slider.transform.Find("Handle Slide Area/Handle/IconDisable").gameObject.SetActive(slider.value == 0);
        }
    }
}