using GameFramework.AOT;
using GameFramework.Hot;
using UnityEngine;

public static class GFSoundExtensions
{
    public static void PlayMusic(this GFSound Sound, string fileName)
    {
        string path = GFGlobal.Tables.TbGlobalSettingData.SoundPath + "/" + fileName;
        AudioClip clip = GFGlobal.Resource.LoadAssetSync<AudioClip>(path);
        if (clip != null)
            Sound.PlayMusic(clip);
        else
            Log.Error("[Sound] Music not found: {0}", fileName);
    }
}