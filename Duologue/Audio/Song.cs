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
        private void these_are_the_cases_that_need_to_be_handled()
        {
            //Intensity manages itself event driven
            if (Managed)
            {
                //We don't have to do anything because it's Autolooped
                if (null == fader)
                    fader = new VolumeChangeWidget(this);
            }
            if (!Managed)
            {
                if (null == beater)
                    beater = new BeatWidget(this, 1, 1);
            }
        }

        protected DuologueGame localGame;
        public string SoundBankName;
        public string WaveBankName;
        protected bool playing = false;

        /// <summary>
        /// managed implies two important things:
        /// 1) The song can have post-play commands sent to it, like volume changes
        /// 2) AudioHelper will hold the song's Cue instances and dispose them when they are done
        /// </summary>
        protected bool managed = false;
        public Track[] Tracks;
        public int TrackCount;

        /// <summary>
        /// This should only be left true for Song composed of repeating cues
        /// (usually infinitely) in XACT.
        /// Better be managed, or we won't be able to ever stop it.
        /// </summary>
        public bool AutoLoop = true;

        /// <summary>
        /// The BeatWidget is what keeps firing off our cues for us on an unmanaged song.
        /// For system performance reasons, Managed must be false if the song has a BeatWidget
        /// </summary>
        public BeatWidget beater = null;

        /// <summary>
        /// VolumeChangeWidget is what keeps sending volume change commands during a fade.
        /// This can only work on a managed song. Managed must be true.
        /// </summary>
        public VolumeChangeWidget fader = null;

        /// <summary>
        /// IntensityWidget subscribes to the system IntensityNotifier, and turns tracks on
        /// and off based on the current intensity.
        /// It should be able to work on both managed and unmanaged songs, but isn't
        /// yet tested for managed songs.
        /// </summary>
        public IntensityWidget hyper = null;

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
            managed = true;
            string[,] arrange = new string[cues.Length, 1];
            for (int row = 0; row < cues.Length; row++)
            {
                arrange[row, 0] = cues[row];
            }
            ArrayToTracks(arrange);
            AudioHelper.Preload(this);
        }


        /// <summary>
        /// Constructor for Songs with just the "Intensity" feature.
        /// The "Intensity" feature means that various tracks are turned on and off
        /// to provide a sense of excitement which increases if the player is doing well.
        /// The IntensityWidget is equipped with a map that defines which tracks are playing
        /// at each possible intensity level.
        /// (Invocations of this constructor are probably rare, 
        /// as Intensity and Beat usually go together.)
        /// Note that this form does allow the song to be "Managed"
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="sbname">SoundBankName</param>
        /// <param name="wbname">WaveBankName</param>
        /// <param name="intensityMap">intensityMap in bool[intensity,track number]=track.Enabled form</param>
        public Song(Game game, string sbname, string wbname, bool[,] intensityMap)
            : this(game, sbname, wbname)
        {
            AutoLoop = false;
            hyper = new IntensityWidget(this, intensityMap);
            AudioHelper.Preload(this);
        }
        

        /// <summary>
        /// Constructor for Songs with just the "Beat" feature. No looping cues allowed.
        /// The "Beat" feature means that each track of the song has been sliced into
        /// a number of short (less than a second) snippets which we play end-to-end based
        /// on a carefully provided timing value.
        /// This type of song is *not* allowed to be managed (i.e. no Fades, volume changes)
        /// because maintaining that capability introduces too much latency
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
            beater = new BeatWidget(this, arrangement.GetLength(0), arrangement.GetLength(1));
            managed = false;
            ArrayToTracks(arrangement);
            AudioHelper.Preload(this);
        }

        /// <summary>
        /// Constructor for Songs with both "Intensity" and "Beat" features. No looping cues allowed.
        /// The "Beat" feature means that each track of the song has been sliced into
        /// a number of short (less than a second) snippets which we play end-to-end based
        /// on a carefully provided timing value.
        /// The "Intensity" feature means that various tracks are turned on and off
        /// to provide a sense of excitement which increases if the player is doing well.
        /// The IntensityWidget is equipped with a map that defines which tracks are playing
        /// at each possible intensity level.
        /// This type of song is *not* allowed to be managed (i.e. no Fades, volume changes)
        /// because maintaining that capability introduces too much latency
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="sbname">SoundBankName</param>
        /// <param name="wbname">WaveBankName</param>
        /// <param name="arrangement">arrangement in string[track#,cue#]=cueName form</param>
        /// <param name="intensityMap">intensityMap in bool[intensity,track number]=track.Enabled form</param>
        public Song(Game game, string sbname, string wbname, string[,] arrangement,
            bool[,] intensityMap)
            : this(game, sbname, wbname, intensityMap)
        {
            beater = new BeatWidget(this, arrangement.GetLength(0), arrangement.GetLength(1));
            managed = false;
            ArrayToTracks(arrangement);
            AudioHelper.Preload(this);
        }

        protected void ArrayToTracks(string[,] arrangement)
        {
            TrackCount = arrangement.GetLength(0);
            int cueCount = arrangement.GetLength(1);
            Tracks = new Track[TrackCount];

            for (int track = 0; track < TrackCount; track++)
            {
                string[] row = new string[cueCount];
                for (int q = 0; q < cueCount; q++)
                {
                    row[q] = arrangement[track, q];
                }
                Tracks[track] = new Track(SoundBankName, row);
            }
        }

        protected void initvars()
        {
            playing = false;
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
                managed = true; //must be, or we could never stop it!
                for (int t = 0; t < TrackCount; t++)
                {
                    //there should only be ONE cue per track in infinite loops
                    Tracks[t].Cues[0].Play();
                }
            }
            if (hyper != null)
            {
                hyper.Attach();
            }
            playing = true;
            Enabled = true;
        }

        public void Stop()
        {
            if (Managed)
            {
                //AudioHelper.Stop(this);
                for (int t = 0; t < TrackCount; t++)
                {
                    Tracks[t].Stop();
                }
                initvars();
                if (hyper != null)
                {
                    hyper.Detach();
                }
            }
            else
            {
                Enabled = false;
            }
        }

        public bool Playing
        {
            get 
            {
                return playing;
            } 
            set 
            { 
            } 
        }

        public bool Managed
        {
            get
            {
                return managed;
            }
            set
            {
            }
        }


        public void FadeOut()
        {
            if (Playing && Managed)
            {
                if (null == fader)
                {
                    fader = new VolumeChangeWidget(this);
                }
                fader.FadeOut();
            }
            else
            {
                Stop();
            }
        }

        public void FadeIn(float volume)
        {
            if (Managed)
            {
                if (null == fader)
                {
                    fader = new VolumeChangeWidget(this);
                }
                fader.FadeIn();
            }
            Play();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (Managed && null != fader)
                fader.Update(gameTime, this);
            else if (!Managed && null != beater)
                beater.Update(gameTime, this);
            base.Update(gameTime);
        }

    }
}