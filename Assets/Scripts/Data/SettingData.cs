using GameFramework.AOT;
using GameFramework.Hot;
using SimpleJSON;

namespace Takeover
{
    public class SettingData : IDataSaveLoad
    {
        public void OnLoad()
        {
            GFGlobal.Sound.MusicVolume = GFGlobal.Save.GetFloat(SaveKeyEnum.MusicVolume, 0.5f);

            // 监听
            GFGlobal.Event.Subscribe<MusicVolumeChangeEvent>(OnMusicVolumeChange);
        }

        private void OnMusicVolumeChange(object sender, MusicVolumeChangeEvent e)
        {
            GFGlobal.Save.SetFloat(SaveKeyEnum.MusicVolume, e.musicVolume);
        }

        public void OnSave(bool isQuit)
        {
        }
    }
}