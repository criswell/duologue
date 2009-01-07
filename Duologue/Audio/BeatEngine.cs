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
        private float beatTimer = 0f;
        private float beatInterval = 1000.0f;
        private Cue beatSound;
        #endregion

        #region Properties

        public float PercentFromTarget()
        {
            return (MillisecondsFromTarget() - beatInterval)*100.0f;
        }

        public float MillisecondsFromTarget()
        {
            float halfPeriod = beatInterval/2.0f;
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
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            this.Enabled = false;
            base.Initialize();
        }

        /// <summary>
        /// Click, click, man, danged ol' Hank, I'll tell ya what.
        /// </summary>
        public void PlayBeat()
        {
            beatSound.Play();
            beatSound =
                DuologueEnhancedAudioEngine.PlayerEffectsSoundBank().GetCue("bambooclick");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            beatTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (beatTimer > beatInterval)
            {
                PlayBeat();
                beatTimer = 0f;
            }
            base.Update(gameTime);
        }
    }
}