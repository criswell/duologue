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
    ///
    /// </summary>
    public class SoundEffectsEngine
    {

        #region Private Fields
        private AudioEngine engine;
        private const string filename_AudioEngine = "Content\\Audio\\Duologue.xgs";
        private WaveBank waveBank;
        private const string filename_WaveBank = "Content\\Audio\\Wave Bank.xwb";
        private SoundBank soundBank;
        private const string filename_SoundBank = "Content\\Audio\\Sound Bank.xsb";
        private Cue playerCollision = null;
        private const string cuename_PlayerCollision = "player-explosion";
        private Cue playerLightGreen = null;
        private const string cuename_PlayerLightGreen = "Saxdual";
        private Cue playerLightPurple = null;
        private const string cuename_PlayerLightPurple = "Saxmachine-high";
        #endregion

        public Cue PlayerCollision
        {
            get
            {
                return playerCollision;
            }
        }

        public Cue PlayerLightGreen
        {
            get
            {
                return playerLightGreen;
            }
        }

        public Cue PlayerLightPurple
        {
            get
            {
                return playerLightPurple;
            }
        }

        public SoundEffectsEngine()
        {
            engine = new AudioEngine(filename_AudioEngine);
            waveBank = new WaveBank(engine, filename_WaveBank);
            soundBank = new SoundBank(engine, filename_SoundBank);
            playerLightGreen = soundBank.GetCue(cuename_PlayerLightGreen);
            playerLightPurple = soundBank.GetCue(cuename_PlayerLightPurple);
        }

        /// <summary>
        /// Click, click, man, danged ol' Hank, I'll tell ya what.
        /// </summary>
        public void PlayerExplosion()
        {
            if (playerCollision == null || playerCollision.IsStopped)
            {
                playerCollision = soundBank.GetCue(cuename_PlayerCollision);
                playerCollision.Play();
            }
            else if (playerCollision.IsPaused)
            {
                playerCollision.Resume();
            }

        }
    }
}