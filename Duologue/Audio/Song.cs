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
        protected void these_are_the_cases_that_need_to_be_handled()
        {
            //There are two methods to start a song:
            //Play() and
            //FadeIn(float volume)
            //They both take care of this condition
            if (Managed)
            {
                if (null == fader)
                    fader = new VolumeChangeWidget(this);
            }
            if (!Managed)
            {
                //There are two methods to start a song:
                //Play() and
                //FadeIn(float volume)
                //They both take care of this condition
                //(The removal of a constructor with no timing parameter
                // and required change to this code points out the trouble with doing this.)
                if (null == beater)
                    beater = new BeatWidget(this, 1, AudioConstants.BPM140);

                //Shouldn't be needed
                //Just because a song isn't Managed doesn't mean
                //it's an intensity song, and, if you do this, it will
                //try to be one. The creation of an intensity song
                //involves calling the proper constructor with the proper
                //arguments passed in.
                //And there aren't any code paths that access hyper outside
                //of intensity handling.
                if (null == hyper)
                    hyper = new IntensityWidget(this, new bool[1, 1]);
            }
        }

        protected DuologueGame localGame;
        public string SoundBankName;
        public string WaveBankName;
        protected bool playing = false;
        protected bool paused = false;

        /// <summary>
        /// managed implies three important things:
        /// 1) The song can have post-play commands sent to it, like volume changes
        /// 2) AudioHelper will hold the song's Cue instances and dispose them when they are done
        /// 3) The Cues were defined as looping infinitely in XACT
        /// </summary>
        protected bool managed = false;
        public Track[] Tracks;
        public int TrackCount;

        /// <summary>
        /// The BeatWidget is what keeps firing off our cues for us on an unmanaged song.
        /// For system performance reasons, Managed must be false if the song has a BeatWidget
        /// If a song isn't managed, it's a beater
        /// </summary>
        public BeatWidget beater = null;

        /// <summary>
        /// VolumeChangeWidget is what keeps sending volume change commands during a fade.
        /// This can only work on a managed song. Managed must be true.
        /// An intensity song can't have a fader.
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
            playing = false;
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
            AudioHelper.Preload(this, false);
        }


        /// <summary>
        /// Constructor for Songs with just the "Intensity" feature.
        /// The "Intensity" feature means that various tracks are turned on and off
        /// to provide a sense of excitement which increases if the player is doing well.
        /// The IntensityWidget is equipped with a map that defines which tracks are playing
        /// at each possible intensity level.
        /// (Invocations of this constructor are probably rare, 
        /// as Intensity and Beat usually go together.)
        /// Note that this form does allow the song to be "Managed",
        /// and sets that field. Other constructors that call this one
        /// will have to set that false.
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="sbname">SoundBankName</param>
        /// <param name="wbname">WaveBankName</param>
        /// <param name="intensityMap">intensityMap in bool[intensity,track number]=track.Enabled form</param>
        public Song(Game game, string sbname, string wbname, bool[,] intensityMap)
            : this(game, sbname, wbname)
        {
            managed = true;
            hyper = new IntensityWidget(this, intensityMap);
            AudioHelper.Preload(this, false);
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
        /// <param name="beatLength">length of beat in milliseconds</param>
        public Song(Game game, string sbname, string wbname, string[,] arrangement, float beatLength)
            : this(game, sbname, wbname)
        {
            beater = new BeatWidget(this, arrangement.GetLength(1), beatLength);
            managed = false;
            ArrayToTracks(arrangement);
            AudioHelper.Preload(this, false);
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
        /// <param name="beatLength">beatLength in milliseconds</param>
        public Song(Game game, string sbname, string wbname, string[,] arrangement,
            bool[,] intensityMap, float beatLength)
            : this(game, sbname, wbname, arrangement, beatLength)
        {
            hyper = new IntensityWidget(this, intensityMap);
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

        public void Play()
        {
            //Intensity manages itself event driven
            if (Managed)
            {
                for (int t = 0; t < TrackCount; t++)
                {
                    //there should only be ONE cue per track in infinite loops
                    Tracks[t].Cues[0].Play();
                }
                if (null == fader)
                {
                    fader = new VolumeChangeWidget(this);
                    fader.Volume = VolumePresets.Full;
                }
            }
            if (!Managed)
            {
                //this should never happen. The BPM is a wild guess
                if (null == beater)
                    beater = new BeatWidget(this, 1, AudioConstants.BPM140);
            }

            playing = true;
            Enabled = true;
            ServiceLocator.GetService<IntensityNotifier>().RequestUpdate();
        }

        public void Stop()
        {
            if (Managed)
            {
                for (int t = 0; t < TrackCount; t++)
                {
                    Tracks[t].Stop();
                }
                if (hyper != null)
                {
                    hyper.Detach();
                }
            }
            if (null != fader)
            {
                fader = null;
            }
            //could it really be this simple? No.
            AudioHelper.Preload(this, true);
            Enabled = false;
            playing = false;
            paused = false;
        }

        public void Pause()
        {
            if (Managed)
            {
                for (int t = 0; t < TrackCount; t++)
                {
                    Tracks[t].Pause();
                }
            }
            Enabled = false;
            playing = false;
            paused = true;
        }

        public void Resume()
        {
            if (Managed && Paused)
            {
                for (int t = 0; t < TrackCount; t++)
                {
                    Tracks[t].Resume();
                }
            }
            Enabled = true;
            playing = true;
            paused = false;
        }

        public bool Playing { get{return playing;} set{ } }

        public bool Paused { get { return paused; } set { } }

        public bool Managed { get { return managed; } set { } }


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
            AudioHelper.SetTimedMusicVolume(VolumePresets.Silent, AudioConstants.PREFADE_MUTE_MS);
            if (Managed)
            {
                for (int t = 0; t < TrackCount; t++)
                {
                    Tracks[t].Cues[0].ChangeVolume(VolumePresets.Silent);
                }
                if (null == fader)
                {
                    fader = new VolumeChangeWidget(this);
                }
                fader.FadeIn(volume);
                Enabled = true;
            }
            else
            {
                Play();
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (Managed)
            {
                if (null != fader)
                {
                    fader.Update(gameTime, this);
                }
                if (null != fader)
                {
                    if (!Playing && !Paused)
                    {
                        Play();
                        for (int t = 0; t < TrackCount; t++)
                        {
                            Tracks[t].Cues[0].ChangeVolume(VolumePresets.Silent);
                        }
                    }
                    else if (Paused)
                    {
                    }
                }
            }
            else
            {
                if (null != beater)
                {
                    beater.Update(gameTime, this);
                }
            }
            base.Update(gameTime);
        }

    }
}