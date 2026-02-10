using UnityEngine;

namespace GameFramework.Hot
{
    public class GFSound : GFBaseModule
    {
        private AudioSource musicSource;

        void Awake()
        {
            gameObject.AddComponent<AudioListener>();

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        public void PlayMusic(AudioClip clip)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }

        public float MusicVolume
        {
            get
            {
                return musicSource.volume;
            }
            set
            {
                musicSource.volume = value;
                GFGlobal.Event.Fire(this, MusicVolumeChangeEvent.Create(value));
            }
        }

        public float SFXVolume { get; set; } = 1f;

        // public void PlaySFX(AudioClip clip)
        // {
        // }
    }
}