using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    public partial class Track
    {
        //actual name of the cue in its content file
        public string CueName;
        //commanded volume to play the cue
        public float Volume;
        public float FadeInSecs;
        public float FadeOutSecs;
        public Track() { }
        public Track(string cue, float vol)
        {
            this.CueName = cue;
            this.Volume = vol;
        }
    }

    public partial class Song
    {
        //file name of the sound bank
        public string SoundBankName;
        public string WaveBankName;
        //name of the cue and a volume parameter
        public List<Track> Tracks = new List<Track>();
        public Song() { }
    }
}
