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
    public class SoundEffects
    {

        #region Private Fields
        private static Cue playerCollision;
        private const string cuename_PlayerCollision = "player-explosion";
        private static Cue playerLightGreen = null;
        private const string cuename_PlayerLightGreen = "Saxdual";
        private static Cue playerLightPurple = null;
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

        public SoundEffects()
        {
            this.init_cues();
        }

        /// <summary>
        /// I know it sucks, but that's it. You have to get fresh cues.
        /// </summary>
        /// 
        private void init_cues()
        {
            playerCollision =
                DuologueEnhancedAudioEngine.PlayerEffectsSoundBank().GetCue(cuename_PlayerCollision);
            playerLightGreen =
                DuologueEnhancedAudioEngine.PlayerEffectsSoundBank().GetCue(cuename_PlayerLightGreen);
            playerLightPurple =
                DuologueEnhancedAudioEngine.PlayerEffectsSoundBank().GetCue(cuename_PlayerLightPurple);
        }

        /// <summary>
        /// Brute fucking force every time. Oh, well.
        /// </summary>
        public void PlayerExplosion()
        {
            playerCollision.Play();
            //refresh immediately
            playerCollision =
                DuologueEnhancedAudioEngine.PlayerEffectsSoundBank().GetCue(cuename_PlayerCollision);
        }

    }
}