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

        #region Properties
        /// <summary>
        /// Hmmmm. There should somehow be singleton access to the audio device
        /// </summary>
        public AudioEngine engine;
        public WaveBank waveBank;
        public SoundBank soundBank;

        public float PercentFromTarget()
        {
            return 0;
        }

        public float MillisecondsFromTarget()
        {
            return 0;
        }

        public float TargetLengthInMilliseconds()
        {
            return 0;
        }

        public float BeatPeriodInMilliseconds()
        {
            return 0;
        }
        #endregion

        public BeatEngine(Game game)
            : base(game)
        {
            engine = new AudioEngine("Content\\sound\\myFirstXACTProject.xgs");
            waveBank = new WaveBank(engine, "Content\\sound\\Wave Bank.xwb");
            soundBank = new SoundBank(engine, "Content\\sound\\Sound Bank.xsb");
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
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