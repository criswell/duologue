using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Mimicware;


namespace Duologue.Audio
{

    //the rule is: one sound bank = one song = one SongID
    public enum SongID { SelectMenu, Intensity, LandOfSand }

    //keep from having to tweak floats and add levels in many places
    public struct Loudness
    {
        public const float Silent = 0f;
        public const float Quiet = 40f;
        public const float Normal = 80f;
        public const float Full = 100f;
    }

    public class IntensityEventArgs : EventArgs
    {
        public int ChangeAmount;
    }

    public delegate void IntensityEventHandler(IntensityEventArgs e);
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AudioManager : Microsoft.Xna.Framework.GameComponent, IService
    {
        private AudioHelper helper;
        public SoundEffects soundEffects;

        private static Dictionary<SongID, Song> songMap = new Dictionary<SongID, Song>();
        private BeatEngine beatEngine;
        private MusicFactory music;

        public const string engine = "Content\\Audio\\Duologue.xgs";
        public event IntensityEventHandler Changed;
        protected virtual void OnChanged(IntensityEventArgs e)
        {
            if (Changed != null)
                Changed(e);
        }

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
            beatEngine = new BeatEngine(Game);
            soundEffects = new SoundEffects(this);

            Game.Components.Add(helper);
            Game.Components.Add(beatEngine);
            //Game.Components.Add(soundEffects);
            music = new MusicFactory(this);
            songMap.Add(SongID.SelectMenu, music.SelectSong);
            songMap.Add(SongID.Intensity, music.BeatEffects);
            songMap.Add(SongID.LandOfSand, music.LandOfSand);

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
            songMap[ID].FadeIn(Loudness.Normal);
        }

        public void FadeIn(SongID ID, float percentage)
        {
            int intensity =
                ((IntensitySong)(songMap[ID])).GetIntensityStepFromPercent(percentage);
            ((IntensitySong)songMap[ID]).FadeIn(intensity);
        }

        public void PlaySong(SongID ID)
        {
            songMap[ID].Play();
        }

        public void StopSong(SongID ID)
        {
            songMap[ID].Stop();
        }

        public void FadeOut(SongID ID)
        {
            songMap[ID].FadeOut();
        }

        public bool SongIsPlaying(SongID ID)
        {
            return songMap[ID].IsPlaying;
        }

        public float GetIntensity(SongID ID)
        {
            try
            {
                if (songMap[ID].IsPlaying)
                {
                    return ((IntensitySong)(songMap[ID])).GetIntensityPercentage();
                }
            }
            catch (Exception e)
            {
            }
            return 0f;
        }

        public void SetIntensity(SongID ID, float percentage)
        {
            try
            {
                ((IntensitySong)(songMap[ID])).SetIntensityPercentage(percentage);
            }
            catch (Exception e)
            {
            }
        }

        public void Intensify()
        {
            ChangeIntensity(1);
        }

        public void ChangeIntensity(int amount)
        {
            IntensityEventArgs e = new IntensityEventArgs();
            e.ChangeAmount = amount;
            OnChanged(e);
        }

        public void Detensify()
        {
            ChangeIntensity(-1);
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