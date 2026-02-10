namespace GameFramework.Hot
{
    public class MusicVolumeChangeEvent : BaseGameEvent<MusicVolumeChangeEvent>
    {
        public float musicVolume;

        public static MusicVolumeChangeEvent Create(float musicVolume)
        {
            var data = Create();
            data.musicVolume = musicVolume;
            return data;
        }

        public override void OnRecycle()
        {
        }
    }
}