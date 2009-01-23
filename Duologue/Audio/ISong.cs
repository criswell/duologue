using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Duologue.Audio
{
    public interface ISong
    {
        void Play();
        //void Play(float percentVolume);
        //void Play(float percentVolume, float fadeinTime);
        //void Pause();
        void Stop();
        //void Stop(float fadeoutTime);
        //void SetVolume(float percent);
        //float GetVolume();
    }

    public class AudioContentBase
    {
        public static string CueName;
        public float Volume;
        //public float FadeInSecs;
        //public float FadeOutSecs;
    }

    public class AudioCollectionBase
    {
        public static string SoundBankName;
        public static string WaveBankName;
    }

    public class Track : AudioContentBase
    {
        public Track() { }
        public Track(string cue, float vol)
        {
            CueName = cue;
            this.Volume = vol;
        }
    }

    public class Song : AudioCollectionBase
    {
        public List<Track> Tracks = new List<Track>();
        public Song() { }
        public virtual void Play(){}
        public virtual void Stop() { }
    }

    public class SoundEffect : AudioContentBase
    {
        public SoundEffect() { }
        public SoundEffect(string cue)
        {
            CueName = cue;
            this.Volume = 999.0f;
        }
        public SoundEffect(string cue, float vol)
        {
            CueName = cue;
            this.Volume = vol;
        }
    }

    public class SoundEffectsGroup : AudioCollectionBase
    {
        //the dictionary key is the cue name...which we also need in
        //each of the sound effects. Crap.
        public Dictionary<string, SoundEffect> Effects =
            new Dictionary<string, SoundEffect>();
        public SoundEffectsGroup() { }
    }

}
