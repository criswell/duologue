using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Mimicware;


namespace Duologue.Audio
{

    public enum SongID { None, SelectMenu, Dance8ths, LandOfSand16ths, 
        Credits, Ultrafix, WinOne, SecondChance, SuperbowlIntro, Superbowl, Tr8or }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AudioManager : Microsoft.Xna.Framework.GameComponent, IService
    {

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

            AudioHelper.SetMusicVolume(AudioConstants.MusicCategoryDefaultVolume);

            base.Initialize();
        }

        protected void Recycle(SongID ID)
        {
            /*Option A:
             * - "recycle" all the cues in the existing Song in songMap
             * 
             * Option B:
             * - Dispose of the the existing song in its entirety
             * remove it from songMap, create a new one, and add it to songMap
             * 
             * Trying Option A first
             * 
             */

        }

        public void FadeOutAllSongs()
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

        private void PlaySong(SongID ID, float volume)
        {
            if (!SongIsPlaying(ID))
            {
                PlaySong(ID);
                if (songMap[ID].fader != null)
                {
                    //probably an erroneous call. Hope this doesn't get used.
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

        protected float PlayingIntensity()
        {
            if (songMap[PlayingSong].hyper == null)
            {
                return 0f;
            }
            if (this.PlayingSong == SongID.None)
            {
                return 0f;
            }
            else
            {
                return songMap[PlayingSong].hyper.PercentIntensity();
            }
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