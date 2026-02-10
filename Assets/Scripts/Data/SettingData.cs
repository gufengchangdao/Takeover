using GameFramework.Hot;
using SimpleJSON;
using UnityEngine;

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

        private void OnMusicVolumeChange(object sender, GameEvent e)
        {
            GFGlobal.Save.SetFloat(SaveKeyEnum.MusicVolume, ((MusicVolumeChangeEvent)e).musicVolume);
        }

        public void OnSave()
        {


        }
    }
}