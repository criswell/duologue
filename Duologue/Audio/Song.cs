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
        protected const float volSteps = 20f;
        public string CueName;
        public float Volume;
        public float StartVolume;
        public float EndVolume;
        public bool VolumeChanging = false;
        public Track() { }
        public Track(string cue, float vol)
        {
            CueName = cue;
            Volume = vol;
            StartVolume = vol;
            EndVolume = vol;
        }

        public void ChangeVolume(float newVol)
        {
            if (newVol != Volume)
            {
                StartVolume = Volume;
                EndVolume = newVol;
                VolumeChanging = true;
            }
        }

        public void IncrementFade()
        {
            if (VolumeChanging)
            {
                Volume += (EndVolume - StartVolume) / volSteps;
                if (((StartVolume > EndVolume) && !(Volume > EndVolume)) ||
                    ((StartVolume < EndVolume) && !(EndVolume > Volume)))
                {
                    VolumeChanging = false;
                }
            }
        }
    }

    public class Song : GameComponent
    {
        public string SoundBankName;
        public string WaveBankName;

        protected bool isPlaying;
        protected bool volumeChanging;
        protected const float updateDeltaT = 100f;
        protected double previousVolChangeTime;
        protected bool stopAfterVolChange;

        public Dictionary<string, Track> Tracks = new Dictionary<string, Track>();
        public Song(Game game, string sbname, string wbname)
            : base(game) 
        {
            SoundBankName = sbname;
            WaveBankName = wbname;
        }

        public Song(Game game, string sbname, string wbname, List<string> cues)
            : this(game, sbname, wbname)
        {
            initvars();
            foreach (string cue in cues)
            {
                Track newTrack = new Track(cue, Loudness.Normal);
                Tracks.Add(cue, newTrack);
            }
            AudioHelper.AddBank(SoundBankName, WaveBankName, cues);
        }

        protected void initvars()
        {
            isPlaying = false;
            volumeChanging = false;
            stopAfterVolChange = false;
        }

        public virtual void Play()
        {
            AudioHelper.Play(this);
            isPlaying = true;
        }

        public virtual void Stop()
        {
            AudioHelper.Stop(this);
            initvars();
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

        public void FadeOut()
        {
            Tracks.Values.ToList().ForEach(track =>
            {
                track.ChangeVolume(Loudness.Silent);
            });
            ChangeVolume(true);        
        }

        public virtual void FadeIn(float volume)
        {
            Tracks.Values.ToList().ForEach(track => 
            {
                track.Volume = Loudness.Quiet;
                track.ChangeVolume(volume);
            });
            ChangeVolume(false);
            Play();
        }

        public void ChangeVolume(bool stop)
        {
            stopAfterVolChange = stopAfterVolChange || stop;
            volumeChanging = true;
        }


        protected void UpdateVolumeChange(GameTime gameTime)
        {
            if (gameTime.TotalRealTime.TotalMilliseconds - previousVolChangeTime > updateDeltaT)
            {
                volumeChanging = false;
                Tracks.Values.ToList().ForEach(track =>
                {
                    previousVolChangeTime = gameTime.TotalRealTime.TotalMilliseconds;
                    if (track.VolumeChanging)
                    {
                        track.IncrementFade();
                        volumeChanging |= track.VolumeChanging;
                    }
                });
                AudioHelper.UpdateCues(this);
                if (!volumeChanging)
                {
                    if (stopAfterVolChange)
                    {
                        Stop();
                    }
                }
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (volumeChanging)
            {
                UpdateVolumeChange(gameTime);
            }
            else
            {
                previousVolChangeTime = gameTime.TotalRealTime.TotalMilliseconds;
            }
            base.Update(gameTime);
        }

    }
}
