using UnityEngine;

namespace GameFramework.Hot
{
    public class GFSave : GFBaseModule
    {
        public void SetString(string key, string jsonData)
        {
            PlayerPrefs.SetString(key, jsonData);
        }

        public string GetString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public float GetFloat(string key)
        {
            return PlayerPrefs.GetFloat(key);
        }

        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public int GetInt(string key)
        {
            return PlayerPrefs.GetInt(key);
        }
    }
}