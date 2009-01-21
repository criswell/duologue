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
        void Play(float percentVolume);
        void Play(float percentVolume, float fadeinTime);
        void Pause();
        void Stop();
        void Stop(float fadeoutTime);
        void SetVolume(float percent);
        float GetVolume();
    }

    public class AudioContentBase
    {
        public string CueName;
        public Cue CueObj;
        public float Volume;
        public float FadeInSecs;
        public float FadeOutSecs;
    }

    public class AudioCollectionBase
    {
        public string SoundBankName;
        public SoundBank SoundBankObj;
        public string WaveBankName;
        public WaveBank WaveBankObj;
    }

    public class Track : AudioContentBase
    {
        public Track() { }
        public Track(string cue, float vol)
        {
            this.CueName = cue;
            this.Volume = vol;
        }
    }

    public class Song : AudioCollectionBase
    {
        public List<Track> Tracks = new List<Track>();
        public Song() { }
    }

    public class SoundEffect : AudioContentBase
    {
        public SoundEffect() { }
        public SoundEffect(string cue)
        {
            this.CueName = cue;
            this.Volume = 999.0f;
        }
        public SoundEffect(string cue, float vol)
        {
            this.CueName = cue;
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
