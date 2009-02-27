using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;

// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Duologue.Audio
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class BeatEngine : Microsoft.Xna.Framework.GameComponent
    {
        #region Private Fields
        protected float beatTimer = 0f;
        protected float beatInterval = 420f;
        //protected float beatInterval = 3433f;
        protected DuologueGame localGame;
        protected AudioManager audio;

        protected const string beatEffectsSounds = "Content\\Audio\\Intensity.xsb";
        protected const string Intensity1 = "beat";
        protected const string Intensity2 = "bass";
        protected const string Intensity3 = "bassplus";
        protected const string Intensity4 = "organ";
        protected const string Intensity5 = "guitar";

        protected int currentMeasure = 0;
        protected PluckNote[][] arrangement = 
            {
                new PluckNote[]{PluckNote.A, PluckNote.A, PluckNote.A, PluckNote.A},
                new PluckNote[]{PluckNote.C},
                new PluckNote[]{PluckNote.A},
                new PluckNote[]{PluckNote.C},
                new PluckNote[]{PluckNote.A, PluckNote.A3},
                new PluckNote[]{PluckNote.C, PluckNote.C3},
                new PluckNote[]{PluckNote.A, PluckNote.A3},
                new PluckNote[]{PluckNote.C, PluckNote.C3},
                new PluckNote[]{PluckNote.A},
                new PluckNote[]{PluckNote.C},
                new PluckNote[]{PluckNote.A},
                new PluckNote[]{PluckNote.C},
                new PluckNote[]{PluckNote.C, PluckNote.A3},
                new PluckNote[]{PluckNote.C, PluckNote.C3},
                new PluckNote[]{PluckNote.A, PluckNote.A3},
                new PluckNote[]{PluckNote.A, PluckNote.C3}
            };

        protected string[][] intenseTracks =
        {
            new string[]{ Intensity1, Intensity2, Intensity3, Intensity4, Intensity5 },
            new string[]{},
            new string[]{},
            new string[]{},
            new string[]{},
            new string[]{},
            new string[]{},
            new string[]{}
        };

        #endregion

        #region Properties

        public float PercentFromTarget()
        {
            return (MillisecondsFromTarget() - beatInterval)*100f;
        }

        public float MillisecondsFromTarget()
        {
            float halfPeriod = beatInterval/2f;
            return beatTimer < halfPeriod ? beatTimer : beatInterval - beatTimer;
        }

        /// <summary>
        /// If we decide to implement, this will return the audible length of the beat
        /// </summary>
        /// <returns></returns>
        public float TargetLengthInMilliseconds()
        {
            return 0;
        }

        public float BeatPeriodInMilliseconds()
        {
            return beatInterval;
        }
        #endregion

        public BeatEngine(Game game)
            : base(game)
        {
            localGame = (DuologueGame)game;
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            this.Enabled = true;
            audio = ServiceLocator.GetService<AudioManager>();

            base.Initialize();
        }

        public void IncrementMeasure()
        {
            if (currentMeasure < arrangement.Length)
            {
                currentMeasure++;
            }
            else
            {
                currentMeasure = 1;
            }

            for (int i = 0; i < arrangement[currentMeasure-1].Length; i++)
            {
                audio.soundEffects.PlayPluckNote(arrangement[currentMeasure-1][i]);
            }
        }

        protected void PlayIntensitySong()
        {
            if (currentMeasure < intenseTracks.Length)
            {
                currentMeasure++;
            }
            else
            {
                currentMeasure = 1;
            }

            for (int i = 0; i < intenseTracks[currentMeasure - 1].Length;)
            {
                if (false && (i == 0) && (AudioHelper.CueIsPlaying(beatEffectsSounds,
                        intenseTracks[currentMeasure-1][i])))
                {
                    bool waiting = true;
                }
                else
                {
                    AudioHelper.PlayCue(
                        beatEffectsSounds,
                        intenseTracks[currentMeasure - 1][i],
                        PlayType.Single);
                    i++;
                }
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            beatTimer += (float)gameTime.ElapsedRealTime.TotalMilliseconds;
            //beatTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (beatTimer > beatInterval)
            {
                //PlayIntensitySong();
                //IncrementMeasure();
                //audio.soundEffects.BambooClick();
                beatTimer = 0f;
            }
            base.Update(gameTime);
        }
    }
}