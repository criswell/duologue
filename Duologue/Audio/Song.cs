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
        public float StartVolume;
        public float EndVolume;
        public Track() { }
        public Track(string cue, float vol)
        {
            CueName = cue;
            Volume = vol;
            StartVolume = vol;
            EndVolume = vol;
        }

        public void PrepareVolumesForNextFadeUpdate()
        {

        }
    }

    public class Song : GameComponent
    {
        public string SoundBankName;
        public string WaveBankName;

        protected bool isPlaying;

        protected bool isFadingOut;
        protected const float fadeOutDeltaV = 1f;
        protected const float fadeOutDeltaT = 100f;
        protected double previousFadeOutTime;

        protected const float fadeInDeltaV = 1f;
        protected const float fadeInDeltaT = 100f;

        protected bool isVolumeChanging;
        protected float volChangeDeltaV;
        protected float volChangeDeltaT;
        protected double previousVolChangeTime;
        protected float startVolume;
        protected float endVolume;
        protected bool stopAfterVolChange;

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

        public void FadeOot()
        {
            Tracks.ForEach(track =>
            {
                track.StartVolume = track.Volume;
                track.EndVolume = Loudness.Silent;
            });
            ChangeVolume(true, fadeOutDeltaV, fadeOutDeltaT);        
        }

        public void FadeOut()
        {
            isFadingOut = true;
        }

        public void FadeIn(float volume)
        {
            Tracks.ForEach(track => 
            { 
                track.Volume = Loudness.Silent;
                track.StartVolume = Loudness.Silent;
                track.EndVolume = volume;
            });
            Play();
            ChangeVolume(false, fadeInDeltaV, fadeInDeltaT);
        }

        public void ChangeVolume(bool stop, float dV, float dT)
        {
            if (isVolumeChanging)
                throw new Exception("already changing volume yo");
            stopAfterVolChange = stop;
            volChangeDeltaV = dV;
            volChangeDeltaT = dT;
            isVolumeChanging = true;
        }

        protected void UpdateFadingCues(GameTime gameTime)
        {
            if (isFadingOut)
            {
                double updateDiff =
                    gameTime.TotalRealTime.TotalMilliseconds - previousFadeOutTime;
                if ( updateDiff > fadeOutDeltaT )
                {
                    bool readyToStop = true;

                    this.Tracks.ForEach(track =>
                    {
                        track.Volume = track.Volume - fadeOutDeltaV;
                        if (track.Volume > Loudness.Silent)
                        {
                            readyToStop = false;
                        }
                    });
                    AudioHelper.UpdateCues(this);
                    previousFadeOutTime = gameTime.TotalRealTime.TotalMilliseconds;

                    if (readyToStop)
                    {
                        Stop();
                        isFadingOut = false;
                    }
                }
            }
            else
            {
                previousFadeOutTime = gameTime.TotalRealTime.TotalMilliseconds;
            }
        }

        protected void UpdateVolumeChange(GameTime gameTime)
        {
            if (isVolumeChanging)
            {
                double updateDiff =
                    gameTime.TotalRealTime.TotalMilliseconds - previousVolChangeTime;
                if (updateDiff > volChangeDeltaT)
                {
                    bool completed = true;
                    this.Tracks.ForEach(track =>
                    {
                        track.Volume = track.Volume + volChangeDeltaV;

                        //   if (abs(track.StartVolume - track.EndVolume) < 
                        //        abs(track.StartVolume - track.Volume))
                        if (((track.StartVolume > track.EndVolume) &&
                            (track.Volume > track.EndVolume)) ||
                            ((track.StartVolume < endVolume) &&
                            (track.EndVolume > track.Volume)))
                        {
                            completed = false;
                        }
                    });
                    AudioHelper.UpdateCues(this);
                    previousVolChangeTime = gameTime.TotalRealTime.TotalMilliseconds;
                    if (completed)
                    {
                        if (stopAfterVolChange)
                        {
                            Stop();
                        }
                        isVolumeChanging = false;
                    }
                }
            }
            else
            {
                previousVolChangeTime = gameTime.TotalRealTime.TotalMilliseconds;
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
            UpdateVolumeChange(gameTime);
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
