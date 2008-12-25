using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace Duologue.Audio
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SoundEffectsEngine : Microsoft.Xna.Framework.GameComponent
    {

        #region Private Fields
        private AudioEngine engine;
        private WaveBank waveBank;
        private SoundBank soundBank;
        private Cue playerCollision = null;
        #endregion

        public SoundEffectsEngine(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            engine = new AudioEngine("Content\\Audio\\Duologue.xgs");
            waveBank = new WaveBank(engine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(engine, "Content\\Audio\\Sound Bank.xsb");

            base.Initialize();
        }


        /// <summary>
        /// Click, click, man, danged ol' Hank, I'll tell ya what.
        /// </summary>
        public void PlayerExplosion()
        {
            if (playerCollision == null || playerCollision.IsStopped)
            {
                playerCollision = soundBank.GetCue("player-explosion");
                playerCollision.Play();
            }
            else if (playerCollision.IsPaused)
            {
                playerCollision.Resume();
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