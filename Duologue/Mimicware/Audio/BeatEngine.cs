#region File Description
#endregion

#region Using statements
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
// Mimicware
using Mimicware;
using Mimicware.Graphics;
using Mimicware.Manager;
using Mimicware.Debug;
// Duologue
using Duologue.ParticleEffects;
using Duologue.AchievementSystem;
using Duologue.Tests;
using Duologue.State;
using Duologue.Screens;
using Duologue.UI;
#endregion

namespace Duologue.Mimicware.Audio
{
    class BeatEngine
    {

        #region Properties
        /// <summary>
        /// Hmmmm. There should somehow be singleton access to the audio device
        /// </summary>
        public AudioEngine engine;
        public WaveBank waveBank;
        public SoundBank soundBank;
        #endregion

        public BeatEngine()
        {
            engine = new AudioEngine("Content\\sound\\myFirstXACTProject.xgs");
            waveBank = new WaveBank(engine, "Content\\sound\\Wave Bank.xwb");
            soundBank = new SoundBank(engine, "Content\\sound\\Sound Bank.xsb");


        }

        /// <summary>
        /// Allows the BeatEngine to run logic such playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Update the input manager every update
            InstanceManager.InputManager.Update();

            // This should be a timer check
            if (LocalInstanceManager.CurrentGameState != LocalInstanceManager.LastGameState)
            {
            }
            else
            {
                if (!soundBank.IsInUse)
                    soundBank.PlayCue("Rhodes_96");
            }
            // Ensure that the last game state gets the current setting for next update
            LocalInstanceManager.CurrentGameState = LocalInstanceManager.CurrentGameState;
            engine.Update();
            base.Update(gameTime);
        }

    
    }
}
