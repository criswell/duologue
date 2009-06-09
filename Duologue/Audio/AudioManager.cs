using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Mimicware;


namespace Duologue.Audio
{

    public enum SongID { None, SelectMenu, Dance8ths, LandOfSand16ths, 
        Credits, Ultrafix, WinOne, SecondChance, SuperbowlIntro, Superbowl, Tr8or }

    //keep from having to tweak floats and add levels in many places
    //stupidly, these values are from 0f to 100f
    //I swear to god, I ended up with that from following an MS tutorial
    //In fact, I've never tried creating a map of 0 to 1 in XACT.
    public struct VolumePresets
    {
        public const float Silent = 0f;
        public const float Quiet = 40f;
        public const float Normal = 80f;
        public const float Full = 100f;

        public const float SelectMenu = 71f;
        public const float Credits = 71f;
        public const float Tr8or = 71f; //Medals Display
        public const float Ultrasux = 72f;

    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AudioManager : Microsoft.Xna.Framework.GameComponent, IService
    {
        //constants for milliseconds per beat
        public const float BPM120 = 1000f * 60f / 120f;
        public const float BPM140 = (3433.039f / 8.000f);
        //got 3433.039 by visual measure of one song, seems to be working
        public const float BPM170 = 1000f * 60f / 170f;

        public static Dictionary<SongID, float> VolumeOverrideTable =
            new Dictionary<SongID,float>();

        protected AudioHelper helper;
        protected SoundEffects soundEffects;

        protected SongID playingSong;
        public SongID PlayingSong
        {
            get
            {
                return playingSong;
            }
            set
            {
                playingSong = value;
            }
        }

        protected static Dictionary<SongID, Song> songMap = new Dictionary<SongID, Song>();
        protected MusicFactory music;

        public const string engine = "Content\\Audio\\Duologue.xgs";

        public AudioManager(Game game) : base(game)
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            helper = new AudioHelper(Game, engine);
            soundEffects = new SoundEffects();

            VolumeOverrideTable.Add(SongID.SelectMenu, VolumePresets.SelectMenu);
            VolumeOverrideTable.Add(SongID.Credits, VolumePresets.Credits);
            VolumeOverrideTable.Add(SongID.Tr8or, VolumePresets.Tr8or);
            VolumeOverrideTable.Add(SongID.Ultrafix, VolumePresets.Ultrasux);

            Game.Components.Add(helper);
            //Game.Components.Add(soundEffects);
            music = new MusicFactory(this);
            //FIXME: It would be kinda handy if we didn't have to explicitly add each song here
            songMap.Add(SongID.SelectMenu, music.SelectSong);
            songMap.Add(SongID.Dance8ths, music.Dance8ths);
            songMap.Add(SongID.LandOfSand16ths, music.LandOfSand16ths);
            songMap.Add(SongID.Credits, music.Credits);
            songMap.Add(SongID.Ultrafix, music.Ultrafix);
            songMap.Add(SongID.WinOne, music.WinOne);
            songMap.Add(SongID.SecondChance, music.SecondChance);
            songMap.Add(SongID.SuperbowlIntro, music.SuperbowlIntro);
            songMap.Add(SongID.Superbowl, music.Superbowl);
            songMap.Add(SongID.Tr8or, music.Tr8or);

            base.Initialize();
        }

        public void GameOver()
        {
            songMap.Values.ToList().ForEach(song =>
                {
                    if (song.Playing)
                    {
                        song.FadeOut();
                    }
                });
        }

        public void FadeIn(SongID ID)
        {
            if (VolumeOverrideTable.ContainsKey(ID))
            {
                songMap[ID].FadeIn(VolumeOverrideTable[ID]);
            }
            else
            {
                songMap[ID].FadeIn(VolumePresets.Normal);
            }
            if (songMap[ID].hyper != null)
                songMap[ID].hyper.SetIntensity(
                    ServiceLocator.GetService<IntensityNotifier>().Intensity);
            PlayingSong = ID;
        }

        public void PlaySong(SongID ID)
        {
            if (!SongIsPlaying(ID))
            {
                songMap[ID].Play();
                PlayingSong = ID;
            }
        }

        public void PlaySong(SongID ID, float volume)
        {
            if (!SongIsPlaying(ID))
            {
                if (songMap[ID].fader == null)
                {
                }
                PlaySong(ID);
                if (songMap[ID].fader != null)
                {
                    songMap[ID].fader.ChangeVolume(volume, 1, false);
                }
            }
        }

        public void StopSong(SongID ID)
        {
            if (SongIsPlaying(ID))
            {
                songMap[ID].Stop();
                PlayingSong = SongID.None;
            }
        }

        public void PauseSong(SongID ID)
        {
            if (SongIsPlaying(ID))
            {
                songMap[ID].Pause();
                PlayingSong = SongID.None;
            }
        }

        public void ResumeSong(SongID ID)
        {
            if (SongIsPaused(ID))
            {
                songMap[ID].Resume();
                PlayingSong = ID;
            }
        }

        public void FadeOut(SongID ID)
        {
            songMap[ID].FadeOut();
            PlayingSong = SongID.None;
        }

        public bool SongIsPlaying(SongID ID)
        {
            return songMap[ID].Playing;
        }

        public bool SongIsPaused(SongID ID)
        {
            return songMap[ID].Paused;
        }

        public void PlayEffect(EffectID ID)
        {
            soundEffects.PlayEffect(ID);
        }

        public double BeatPercentage()
        {
            double retVal = 1d;
            if (PlayingSong != SongID.None)
            {
                if (null != songMap[PlayingSong].beater)
                {
                    retVal = songMap[PlayingSong].beater.BeatPercentage();
                }
            }
            return retVal;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            base.Update(gameTime);
        }
    }
}