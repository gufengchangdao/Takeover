using GameFramework.AOT;
using GameFramework.Hot;
using UnityEngine.UI;

namespace Takeover
{
    public class SettingView : BaseView<SettingControl>
    {
        public SliderPro musicSlider;
        public SliderPro sfxSlider;

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
        }

        private void UpdateSoundSliderMask(Slider slider)
        {
            slider.transform.Find("Handle Slide Area/Handle/IconDisable").gameObject.SetActive(slider.value == 0);
        }
    }
}