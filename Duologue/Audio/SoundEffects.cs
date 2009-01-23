using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Duologue.Audio
{
    /// <summary>
    /// Convenience class. Provides no-arg methods that invoke AudioHelper generics
    /// </summary>
    public class SoundEffects
    {
        public const string PlayerEffectsWB = "Content\\Audio\\PlayerEffects.xwb";
        public const string PlayerEffectsSB = "Content\\Audio\\PlayerEffects.xsb";
        
        public const string Bamboo = "bambooclick";
        public const string Explosion = "player-explosion";
        public const string LightColorA = "Saxdual";
        public const string LightColorB = "Saxmachine-high";

        public SoundEffects()
        {
        }

        /// <summary>
        /// Passes the proper name parameters to the AudioHelper
        /// </summary>
        public static void PlayerExplosion()
        {
            AudioHelper.PlayCue(PlayerEffectsSB, Explosion, PlayType.Single);
        }

        /// <summary>
        /// Passes the proper name parameters to the AudioHelper
        /// </summary>
        public static void BambooClick()
        {
            AudioHelper.PlayCue(PlayerEffectsSB, Bamboo, PlayType.Single);
        }

        public static void init(Game param_game)
        {
            List<string> effectNames = new List<string> {
                Bamboo, Explosion, LightColorA, LightColorB
            };
            AudioHelper.AddBank(PlayerEffectsSB, PlayerEffectsWB, effectNames);

            /* What's all this good for?

            SoundEffectsGroup playerFX = new SoundEffectsGroup();
            playerFX.SoundBankName = PlayerEffectsSB;
            playerFX.WaveBankName = PlayerEffectsWB;
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