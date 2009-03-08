using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Mimicware;

namespace Duologue.Audio
{

    //Things relevant to songs with volume fades
    public class VolumeChangeWidget
    {        
        //Most recently commanded Volume
        public float Volume;

        public const int UPDATE_MILLISECONDS = 50;
        //Number of volume change steps between StartVolume and EndVolume
        //We should calculate that, and the step amounts
        //steps = total mS/update mS

        protected int steps;
        protected float stepAmount;
        public float StartVolume; //prefer read-only
        public float EndVolume; //prefer read-only
        public bool VolumeChanging = false; //prefer read-only

        public VolumeChangeWidget(float start, float end, int milliseconds)
        {
            StartVolume = MWMathHelper.LimitToRange(start, 0f, 1f);
            EndVolume = MWMathHelper.LimitToRange(end, 0f, 1f);
            milliseconds = MWMathHelper.LimitToRange(steps, 1, 5000);
            steps = milliseconds / UPDATE_MILLISECONDS;
            stepAmount = (EndVolume - StartVolume) / steps;
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

        public void FadeIn(float volume)
        {
                Volume = Loudness.Quiet;
                ChangeVolume(volume);
        }

        public float IncrementFade()
        {
            if (VolumeChanging)
            {
                Volume += (EndVolume - StartVolume) / (float)steps;
                if (((StartVolume > EndVolume) && !(Volume > EndVolume)) ||
                    ((StartVolume < EndVolume) && !(EndVolume > Volume)))
                {
                    VolumeChanging = false;
                }
            }
            return Volume;
        }
    }

    public class Song : GameComponent
    {
        //relevant to all songs:
        protected DuologueGame localGame;
        public string SoundBankName;
        public string WaveBankName;
        protected bool isPlaying = false;
        public float Volume;
        public List <Track> Tracks = new List <Track>();

        // this should only be left true for Song composed of repeating cues
        // (usually infinitely) in XACT.
        public bool AutoLoop = true;

        //relevant to "Fadeable" songs:
        protected bool volumeChanging;
        protected const float updateDeltaT = 50f;
        protected double previousVolChangeTime;
        protected bool stopAfterVolChange;

        //"sliced" songs
        public bool IsBeatSong = false;
        public int NumberOfBeats;
        public int NumberOfTracks;

        //"intensity" songs (selectively sound tracks when game intensity increases)
        public bool IsIntensitySong = false;

        public Song(Game game, string sbname, string wbname)
            : base(game) 
        {
            Enabled = false;
            localGame = (DuologueGame)game;
            SoundBankName = sbname;
            WaveBankName = wbname;
            Tracks = new List<Track>();
        }

        /// <summary>
        /// Constructor for "Looping" songs
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="sbname">Pathed Name of SoundBank file</param>
        /// <param name="wbname">Pathed Name of WaveBank file</param>
        /// <param name="cues">array of names of infinitely repeating cues which are meant to play concurrently</param>
        public Song(Game game, string sbname, string wbname, string[] cues)
            : this(game, sbname, wbname)
        {
            string[,] arrange = new string[cues.Length, 1];
            for (int row = 0; row < cues.Length; row++)
            {
                arrange[row, 0] = cues[row];
            }
            ArrayToTracks(arrange);
        }

        //adding the arrangement parameter sets this up as a "Beat" song
        //Hence, not AutoLooping, until we discover looping manually sucks
        public Song(Game game, string sbname, string wbname, string[,] arrangement)
            : this(game, sbname, wbname)
        {
            AutoLoop = false;
            IsBeatSong = true;
            initvars();
            NumberOfTracks = arrangement.GetLength(0);
            NumberOfBeats = arrangement.GetLength(1);
            ArrayToTracks(arrangement);
        }

        //adding the intensityMap parameter lets this set up "intensity" songs
        public Song(Game game, string sbname, string wbname, float[,] intensityMap)
            : this(game, sbname, wbname)
        {
            AutoLoop = false;
            IsIntensitySong = true;
            //FIXME
            //Create data property for use by intensity update
            //subscribe to intensity changes
        }

        //constructor for songs with both "intensity" and "beat"
        public Song(Game game, string sbname, string wbname, string[,] arrangement,
            float[,] intensityMap)
            : this(game, sbname, wbname, intensityMap)
        {
            IsBeatSong = true;
            ArrayToTracks(arrangement);
        }

        protected void ArrayToTracks(string[,] arrangement)
        {
            NumberOfTracks = arrangement.GetLength(0);
            NumberOfBeats = arrangement.GetLength(1);
            for (int track = 0; track < NumberOfTracks; track++)
            {
                string[] row = new string[NumberOfBeats];
                for (int q = 0; q < NumberOfBeats; q++)
                {
                    row[q] = arrangement[track, q];
                }
                Tracks.Add(new Track(SoundBankName, row));
            }
            AudioHelper.Preload(this);
        }

        protected void PlayBeat(int beat)
        {
            Tracks.ForEach(track => { track.PlayBeat(beat); });
        }

        protected void initvars()
        {
            isPlaying = false;
            volumeChanging = false;
            stopAfterVolChange = false;
        }

        public void Play()
        {
            if (AutoLoop) //simplest case - parallel infinite Cues
            {
                //AudioHelper.Play(this);
                Tracks.ForEach(track =>
                    {
                        track.Play();
                    });
                isPlaying = true;
            }
            Enabled = true;
        }

        public void Stop()
        {
            //AudioHelper.Stop(this);
            Tracks.ForEach(track =>
            {
                track.Stop();
            });
            initvars();
            Enabled = false;
        }

        public bool IsPlaying
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
            Tracks.ForEach(track =>
            {
                track.ChangeVolume(Loudness.Silent);
            });
            ChangeVolume(true);        
        }

        public void FadeIn(float volume)
        {
            Tracks.ForEach(track => 
            {
                track.FadeIn(volume);
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
                Tracks.ForEach(track =>
                {
                    previousVolChangeTime = gameTime.TotalRealTime.TotalMilliseconds;
                    if (track.VolumeChanging())
                    {
                        track.IncrementFade();
                        volumeChanging |= track.VolumeChanging();
                    }
                });
                AudioHelper.UpdateCues(this);
                if (!volumeChanging && stopAfterVolChange)
                {
                    Stop();
                }
            }
        }

        protected void UpdateTracks()
        {
            Tracks.ForEach(track =>
                {
                });
        }

        protected float beatTimer = 0f;
        protected float lengthOfBeat = (3433.039f / 8.000f); //FIXME this should be set per-instance
        protected int beatState = 0;
        protected int currentBeat = 0;

        protected void UpdateBeatSong(GameTime gameTime)
        {
            switch (beatState)
            {
                case 0:
                    currentBeat = 0;
                    beatState = 20;
                    break;
                case 20:
                    if (currentBeat >= NumberOfBeats)
                    {
                        currentBeat = 0;
                    }
                    beatTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (beatTimer > lengthOfBeat)
                    {
                        currentBeat++;
                        PlayBeat(currentBeat);
                        beatTimer = 0f;
                    }
                    break;
                case 999:
                    throw new Exception("unexpected beatState");
                default:
                    throw new Exception("bad state value");
            }
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (IsBeatSong && Enabled)
            {
                UpdateBeatSong(gameTime);
            }
            if (IsIntensitySong && Enabled)
            {
                //should need this: intensity changes come from notification
                //and take effect immediately
            }
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
