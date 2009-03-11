using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Mimicware;
using Duologue.Audio.Widgets;

namespace Duologue.Audio
{
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

        protected VolumeChangeWidget fader = null;
        protected BeatWidget beater = null;
        protected IntensityWidget hyper = null;

        /// <summary>
        /// Protected c'tor for "least common denominator"
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="sbname">SoundBankName</param>
        /// <param name="wbname">WaveBankName</param>
        protected Song(Game game, string sbname, string wbname)
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


        /// <summary>
        /// Constructor for Songs with just the "Beat" feature. No looping cues allowed.
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="sbname">SoundBankName</param>
        /// <param name="wbname">WaveBankName</param>
        /// <param name="arrangement">arrangement in string[track#,cue#]=cueName form</param>
        public Song(Game game, string sbname, string wbname, string[,] arrangement)
            : this(game, sbname, wbname)
        {
            AutoLoop = false;
            initvars();
            beater = new BeatWidget(arrangement.GetLength(0), arrangement.GetLength(1));
            ArrayToTracks(arrangement);
        }

        /// <summary>
        /// Constructor for Songs with just the "Intensity" feature.
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="sbname">SoundBankName</param>
        /// <param name="wbname">WaveBankName</param>
        /// <param name="intensityMap">intensityMap in string[intensity,track number]=track.Volume form</param>
        public Song(Game game, string sbname, string wbname, float[,] intensityMap)
            : this(game, sbname, wbname)
        {
            AutoLoop = false;
            hyper = new IntensityWidget(this);
            //FIXME  subscribe to intensity changes
        }

        
        /// <summary>
        /// Constructor for Songs with both "Intensity" and "Beat" features. No looping cues allowed.
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="sbname">SoundBankName</param>
        /// <param name="wbname">WaveBankName</param>
        /// <param name="arrangement">arrangement in string[track#,cue#]=cueName form</param>
        /// <param name="intensityMap">intensityMap in string[intensity,track number]=track.Volume form</param>
        public Song(Game game, string sbname, string wbname, string[,] arrangement,
            float[,] intensityMap)
            : this(game, sbname, wbname, intensityMap)
        {
            beater = new BeatWidget(arrangement.GetLength(0), arrangement.GetLength(1));
            ArrayToTracks(arrangement);
        }

        protected void ArrayToTracks(string[,] arrangement)
        {
            for (int track = 0; track < arrangement.GetLength(0); track++)
            {
                string[] row = new string[arrangement.GetLength(1)];
                for (int q = 0; q < arrangement.GetLength(1); q++)
                {
                    row[q] = arrangement[track, q];
                }
                Tracks.Add(new Track(SoundBankName, row));
            }
            AudioHelper.Preload(this);
        }

        protected void initvars()
        {
            isPlaying = false;
            //could probably null the fader each time it is done, unless
            //we want to hold it to reduce memory thrash
            if (null != fader)
            {
                fader.VolumeChanging = false;
                fader.StopAfterChange = false;
            }
        }

        public void Play()
        {
            if (AutoLoop) //simplest case - parallel infinite Cues
            {
                Tracks.ForEach(track =>
                    {
                        //there should only be ONE cue per track in infinite loops
                        track.cues[0].Play();
                    });
            }
            isPlaying = true;
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
            if (IsPlaying)
            {
                fader = new VolumeChangeWidget(this);
                fader.Volume = Loudness.Normal;
                fader.FadeOut(500);
                //Tracks.ForEach(track =>
                //{
                //    track.ChangeVolume(Loudness.Silent);
                //});
                ChangeVolume(true);
            }
        }

        public void FadeIn(float volume)
        {
            fader = new VolumeChangeWidget(this);
            fader.FadeIn(500);
            //Tracks.ForEach(track => 
            //{
            //    track.FadeIn(volume);
            //});
            //ChangeVolume(false);
            Play();
        }

        public void ChangeVolume(bool stop)
        {
            fader.StopAfterChange = fader.StopAfterChange || stop;
            fader.VolumeChanging = true;
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                if (null != beater)
                    beater.Update(gameTime, this);
                if (null != fader)
                    fader.Update(gameTime, this);
                //should not need this: intensity changes come from notification
                //and take effect immediately
                if (null != hyper) { }
            }
            base.Update(gameTime);
        }

    }
}