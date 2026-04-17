using GameFramework.Hot;
using SimpleJSON;

namespace Takeover
{
    public class SettingData : IDataSaveLoad
    {
        public void OnLoad()
        {
            // 初始化
            float musicVolume = 0.5f; //默认值

            var jsonData = GFGlobal.Save.GetString(nameof(SettingData));
            if (!string.IsNullOrEmpty(jsonData))
            {
                JSONNode jsonNode = JSON.Parse(jsonData);
                musicVolume = jsonNode[SaveKeyEnum.MusicVolume].AsFloat;
            }

            GFGlobal.Sound.MusicVolume = musicVolume;

            // 监听
            GFGlobal.Event.Subscribe<MusicVolumeChangeEvent>(OnMusicVolumeChange);
        }

        private void OnMusicVolumeChange(object sender, MusicVolumeChangeEvent e)
        {
            GFGlobal.Save.SetFloat(SaveKeyEnum.MusicVolume, e.musicVolume);
        }

        public void OnSave()
        {


        }
    }
}