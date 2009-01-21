using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
////using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Net;
//using Microsoft.Xna.Framework.Storage;


namespace Duologue.Audio
{
    /// <summary>
    /// Convenience class. Provides no-arg methods that invoke AudioHelper generics
    /// </summary>
    public class SoundEffects
    {
        public const string PlayerWaveBank = "Content\\Audio\\Wave Bank.xwb";
        public const string PlayerSoundBank = "Content\\Audio\\Sound Bank.xsb";
        
        public const string Bamboo = "bambooclick";
        public const string Explosion = "player-explosion";
        public const string LightColorA = "Saxdual";
        public const string LightColorB = "Saxmachine-high";

        //am I going to need static containers of sounds here? We will see!

        public SoundEffects()
        {
        }

        /// <summary>
        /// Passes the proper name parameters to the AudioHelper
        /// </summary>
        public static void PlayerExplosion()
        {
            AudioHelper.PlayCue(PlayerSoundBank, Explosion, PlayType.Single);
        }

        /// <summary>
        /// Passes the proper name parameters to the AudioHelper
        /// </summary>
        public static void BambooClick()
        {
            AudioHelper.PlayCue(PlayerSoundBank, Bamboo, PlayType.Single);
        }

        public static void init(Game param_game)
        {
            List<string> effectNames = new List<string> {
                Bamboo, Explosion, LightColorA, LightColorB
            };
            AudioHelper.AddBank(PlayerSoundBank, PlayerWaveBank, effectNames);

            /* What's all this good for?

            SoundEffectsGroup playerFX = new SoundEffectsGroup();
            playerFX.SoundBankName = PlayerSoundBank;
            playerFX.WaveBankName = PlayerWaveBank;
            SoundEffect effect1 = new SoundEffect(Bamboo);
            SoundEffect effect2 = new SoundEffect(Explosion);
            SoundEffect effect3 = new SoundEffect(LightColorA);
            SoundEffect effect4 = new SoundEffect(LightColorB);
            playerFX.Effects = new Dictionary<string, SoundEffect> {
                {Bamboo, effect1},
                {Explosion, effect2},
                {LightColorA, effect3},
                {LightColorB, effect4}
            };
             */
        }
    }
}