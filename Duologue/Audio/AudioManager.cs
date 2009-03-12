using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Mimicware;


namespace Duologue.Audio
{

    //the rule is: one sound bank = one song = one SongID
    public enum SongID { None, SelectMenu, Dance8ths, LandOfSand16ths }

    //keep from having to tweak floats and add levels in many places
    public struct Loudness
    {
        public const float Silent = 0f;
        public const float Quiet = 40f;
        public const float Normal = 80f;
        public const float Full = 100f;
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AudioManager : Microsoft.Xna.Framework.GameComponent, IService
    {
        protected AudioHelper helper;
        public SoundEffects soundEffects;

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
            soundEffects = new SoundEffects(this);

            Game.Components.Add(helper);
            //Game.Components.Add(soundEffects);
            music = new MusicFactory(this);
            songMap.Add(SongID.SelectMenu, music.SelectSong);
            songMap.Add(SongID.Dance8ths, music.Dance8ths);
            songMap.Add(SongID.LandOfSand16ths, music.LandOfSand16ths);

            base.Initialize();
        }

        public void GameOver()
        {
            songMap.Values.ToList().ForEach(song =>
                {
                    if (song.IsPlaying)
                    {
                        song.FadeOut();
                    }
                });
        }

        public void FadeIn(SongID ID)
        {
            PlaySong(ID);
            //songMap[ID].FadeIn(Loudness.Normal);
        }

        public void FadeIn(SongID ID, float percentage)
        {
            PlaySong(ID);
            // FadeIn(ID);
            //int intensity = songMap[ID].GetIntensityStepFromPercent(percentage);
            //songMap[ID].FadeIn(intensity);
        }

        public void PlaySong(SongID ID)
        {
            if (!SongIsPlaying(ID))
            {
                songMap[ID].Play();
                PlayingSong = ID;
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

        public void FadeOut(SongID ID)
        {
            StopSong(ID);
            //songMap[ID].FadeOut();
        }

        public bool SongIsPlaying(SongID ID)
        {
            return songMap[ID].IsPlaying;
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