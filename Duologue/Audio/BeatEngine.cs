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
        #region Protected Fields
        protected float beatTimer = 0f;
        protected float beatInterval = 3433.039f/8.000f;
        protected bool wait = false;
        protected int beatState = 0;
        protected DuologueGame localGame;
        protected AudioManager audio;

        protected const string beatEffectsSounds = "Content\\Audio\\Intensity.xsb";
        protected const string Intensity1 = "beat2";
        protected const string Intensity2 = "bass2";
        protected const string Intensity3 = "bassplus2";
        protected const string Intensity4 = "organ2";
        protected const string Intensity5 = "guitar2";

        protected int currentBeat = 0;
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

        protected string[][] intenseTracks;

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
            this.Enabled = true;
            audio = ServiceLocator.GetService<AudioManager>();
            intenseTracks = GetIntenseTracks();
            base.Initialize();
        }

        public void IncrementBeat()
        {
            if (currentBeat < arrangement.Length)
            {
                currentBeat++;
            }
            else
            {
                currentBeat = 1;
            }

            for (int i = 0; i < arrangement[currentBeat-1].Length; i++)
            {
                audio.soundEffects.PlayPluckNote(arrangement[currentBeat-1][i]);
            }
        }


        public string[][] GetIntenseTracks()
        {
            string[][] tracks = {
        
            //new string[]{ Intensity1, Intensity2, Intensity3, Intensity4, Intensity5 },
            new string[]{},
            new string[]{},new string[]{},new string[]{},new string[]{},
            new string[]{},new string[]{},new string[]{},
            new string[]{},
            new string[]{},new string[]{},new string[]{},new string[]{},
            new string[]{},new string[]{},new string[]{},
            new string[]{},
            new string[]{},new string[]{},new string[]{},new string[]{},
            new string[]{},new string[]{},new string[]{},
            new string[]{},
            new string[]{},new string[]{},new string[]{},new string[]{},
            new string[]{},new string[]{},new string[]{},
            new string[]{},
            new string[]{},new string[]{},new string[]{},new string[]{},
            new string[]{},new string[]{},new string[]{}
                                };
            return tracks;
        }

        protected void PlayBeat(int beat)
        {
            for (int i = 0; i < intenseTracks[beat - 1].Length; i++)
            {
                AudioHelper.PlayCue(beatEffectsSounds, intenseTracks[currentBeat - 1][i]);
            }
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}