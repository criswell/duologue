﻿using System;
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
        //calls to Update aren't sufficiently evenly time spaced, so...
        protected const float fadeIntervalMilli = 250f;
        protected double lastFadeSecs;
        protected bool stopAfterFade;
        private List<string> update_stamps = new List<string>();

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
            AudioHelper.PlayCues(SoundBankName, PlayType.Nonstop);
            isPlaying = true;
        }

        public virtual void Stop()
        {
            AudioHelper.StopCues(SoundBankName);
            isPlaying = false;
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
                double updateDiff =
                    gameTime.TotalRealTime.TotalMilliseconds -
                    lastFadeSecs;
                if ( updateDiff > fadeIntervalMilli )
                {
                    update_stamps.Add(gameTime.TotalRealTime.TotalMilliseconds.ToString() + " mS gametime");
                    update_stamps.Add(updateDiff + " mS Diff");
                    bool readyToStop = true;

                    this.Tracks.ForEach(track =>
                    {
                        track.Volume = track.Volume - fadeDeltaV;
                        if (track.Volume > Loudness.Silent)
                        {
                            readyToStop = false;
                        }
                    });
                    AudioHelper.UpdateCues(SoundBankName, Tracks);
                    lastFadeSecs = gameTime.TotalRealTime.TotalMilliseconds;

                    if (readyToStop)
                    {
                        if (stopAfterFade)
                        {
                            update_stamps.Add(gameTime.TotalRealTime.TotalMilliseconds.ToString() + " mS");
                            Stop();
                        }
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


    public class SoundEffect : AudioContentBase
    {
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

    public class EffectsBank : AudioCollectionBase
    {
        //the dictionary key is the cue name...which we also need in
        //each of the sound effects. Crap.
        public Dictionary<string, SoundEffect> Effects =
            new Dictionary<string, SoundEffect>();
        public EffectsBank(Game game) : base(game) { }
    }

}
