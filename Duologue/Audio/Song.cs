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
        public bool IsVolumeChanging = false;
        public float VolChangeDeltaV;
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
                IsVolumeChanging = true;
            }
        }
    }

    public class Song : GameComponent
    {
        public string SoundBankName;
        public string WaveBankName;

        protected bool isPlaying;

        protected const float fadeOutDeltaV = -1f;
        protected const float fadeInDeltaV = 1f;

        protected bool isVolumeChanging = false;
        protected  const float UpdateDeltaT = 100f;
        protected double previousVolChangeTime;
        protected bool stopAfterVolChange;

        public PlayType playType;

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
            foreach (string cue in cues)
            {
                Track newTrack = new Track(cue, Loudness.Normal);
                Tracks.Add(cue, newTrack);
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
            stopAfterVolChange = stop;
            isVolumeChanging = true;
        }


        protected void UpdateVolumeChange(GameTime gameTime)
        {
            double updateDiff =
                gameTime.TotalRealTime.TotalMilliseconds - previousVolChangeTime;
            if (updateDiff > UpdateDeltaT)
            {
                bool completed = true;
                this.Tracks.Values.ToList().ForEach(track =>
                {
                    if (track.IsVolumeChanging)
                    {
                        track.Volume += (track.EndVolume - track.StartVolume) / 20f; //FIXME

                        if (((track.StartVolume > track.EndVolume) &&
                            (track.Volume > track.EndVolume)) ||
                            ((track.StartVolume < track.EndVolume) &&
                            (track.EndVolume > track.Volume)))
                        {
                            completed = false;
                        }
                        else
                        {
                            isVolumeChanging = true;
                        }
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (isVolumeChanging)
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
