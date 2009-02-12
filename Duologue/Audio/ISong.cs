using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Duologue.Audio
{
    public interface ISong
    {
        void Play();
        void Stop();
        void Fade(bool stop);
    }

    public class AudioContentBase
    {
        public string CueName;
        public float Volume;
    }

    public class AudioCollectionBase : GameComponent
    {
        public AudioCollectionBase(Game game) : base(game) { }
        public string SoundBankName;
        public string WaveBankName;
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

    public class Song : AudioCollectionBase, ISong
    {
        protected bool isPlaying;
        protected bool isFading;
        protected const float fadeDeltaV = 1f;
        protected bool stopAfterFade;

        protected float volume;

        public List<Track> Tracks = new List<Track>();
        public Song(Game game, string sbname, string wbname) 
            : base(game) 
        {
            SoundBankName = sbname;
            WaveBankName = wbname;
        }
        public Song(Game game, string sbname, string wbname, List<string> cues)
            : this(game, sbname, wbname)
        {
            foreach (string cue in cues)
            {
                Track newTrack = new Track(cue, Loudness.Full);
                Tracks.Add(newTrack);
            }
            AudioHelper.AddBank(SoundBankName, WaveBankName, cues);
        }

        public virtual void Play()
        {
            AudioHelper.PlayCues(SoundBankName, PlayType.Nonstop);
            isPlaying = true;
        }

        public virtual void Stop()
        {
            AudioHelper.StopCues(SoundBankName);
            IsPlaying = false;
        }

        public virtual bool IsPlaying
        {
            /*
            get
            {
                isPlaying = AudioHelper.CueIsPlaying(SoundBankName, CueName);
                return isPlaying;
            }
            set
            {
                isPlaying = value;
            }
            */
            get 
            {
                return isPlaying;
            } 
            set 
            { 
            } 
        }

        public void Fade(bool stop)
        {
            stopAfterFade = stop;
            isFading = true;
        }

        protected void UpdateFadingCues(GameTime gameTime)
        {
            if (isFading)
            {
                volume = volume - fadeDeltaV;
                AudioHelper.UpdateCues(SoundBankName, volume);
                if (volume <= Loudness.Silent)
                {
                    if (stopAfterFade)
                    {
                        Stop();
                    }
                    isFading = false;
                }
            }
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            UpdateFadingCues(gameTime);
            base.Update(gameTime);
        }

    }


    public class SoundEffect : AudioContentBase
    {
        public SoundEffect() { }
        public SoundEffect(string cue)
        {
            CueName = cue;
            this.Volume = Loudness.Full;
        }
        public SoundEffect(string cue, float vol)
        {
            CueName = cue;
            this.Volume = vol;
        }
    }

    public class EffectsBank : AudioCollectionBase
    {
        //the dictionary key is the cue name...which we also need in
        //each of the sound effects. Crap.
        public Dictionary<string, SoundEffect> Effects =
            new Dictionary<string, SoundEffect>();
        public EffectsBank(Game game) : base(game) { }
    }

}
