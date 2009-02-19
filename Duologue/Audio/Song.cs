using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Duologue.Audio
{

    public class Track
    {
        public string CueName;
        public float Volume;
        public Track() { }
        public Track(string cue, float vol)
        {
            this.CueName = cue;
            this.Volume = vol;
        }
    }

    public class Song : GameComponent
    {
        public string SoundBankName;
        public string WaveBankName;

        protected bool isPlaying;
        protected bool isFading;
        protected const float fadeDeltaV = 1f;
        protected const float fadeIntervalMilli = 100f;
        protected double lastFadeSecs;
        protected bool stopAfterFade;
        public PlayType playType;

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
                Track newTrack = new Track(cue, Loudness.Normal);
                Tracks.Add(newTrack);
            }
            AudioHelper.AddBank(SoundBankName, WaveBankName, cues);
        }

        public virtual void Play()
        {
            AudioHelper.Play(this);
            isPlaying = true;
        }

        public virtual void Stop()
        {
            AudioHelper.Stop(this);
            isPlaying = false;
        }

        public virtual bool IsPlaying
        {
            get 
            {
                return isPlaying;
            } 
            set 
            { 
            } 
        }

        public void Fade()
        {
            isFading = true;
        }

        protected void UpdateFadingCues(GameTime gameTime)
        {
            if (isFading)
            {
                double updateDiff =
                    gameTime.TotalRealTime.TotalMilliseconds - lastFadeSecs;
                if ( updateDiff > fadeIntervalMilli )
                {
                    bool readyToStop = true;

                    this.Tracks.ForEach(track =>
                    {
                        track.Volume = track.Volume - fadeDeltaV;
                        if (track.Volume > Loudness.Silent)
                        {
                            readyToStop = false;
                        }
                    });
                    AudioHelper.UpdateCues(this);
                    lastFadeSecs = gameTime.TotalRealTime.TotalMilliseconds;

                    if (readyToStop)
                    {
                        Stop();
                        isFading = false;
                    }
                }
            }
            else
            {
                lastFadeSecs = gameTime.TotalRealTime.TotalMilliseconds;
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


    public class SoundEffect
    {
        public string CueName;
        public float Volume;
        //FIXME

        public SoundEffect() { }
        public SoundEffect(string cue)
        {
            CueName = cue;
            this.Volume = Loudness.Normal;
        }
        public SoundEffect(string cue, float vol)
        {
            CueName = cue;
            this.Volume = vol;
        }
    }

    public class EffectsBank : GameComponent
    {
        public string SoundBankName;
        public string WaveBankName;
        //the dictionary key is the cue name...which we also need in
        //each of the sound effects. Crap.
        public Dictionary<string, SoundEffect> Effects =
            new Dictionary<string, SoundEffect>();
        public EffectsBank(Game game) : base(game) { }
    }

}
