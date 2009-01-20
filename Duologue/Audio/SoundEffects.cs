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
    //the rule is: one sound bank = one group of effects = one EffectsGroupID
    public enum EffectsGroupID { Player }

    public partial class SoundEffect
    {
        //stupid redundancy...
        //might be able to drop CueName. We only handle it along with SoundBankName.
        public string CueName;
        //commanded volume to play the cue
        public float Volume;
        public float FadeInSecs;
        public float FadeOutSecs;
        public SoundEffect() { }
        public SoundEffect(string cue)
        {
            this.CueName = cue;
            this.Volume = 1.0f;
        }
        public SoundEffect(string cue, float vol)
        {
            this.CueName = cue;
            this.Volume = vol;
        }
    }

    public partial class SoundEffectsGroup
    {
        //file name of the sound bank
        public string SoundBankName;
        public string WaveBankName;
        //the dictionary key is the cue name...which we also need in
        //each of the sound effects. Crap.
        public Dictionary<string, SoundEffect> Effects = 
            new Dictionary<string, SoundEffect>();
        public SoundEffectsGroup() { }
    }

    /// <summary>
    /// Convenience class. Provides no-arg methods that invoke AudioManager generics
    /// </summary>
    public class SoundEffects
    {
        public const string playerEffectsWaveBankName = "Content\\Audio\\Wave Bank.xwb";
        public const string playerEffectsSoundBankName = "Content\\Audio\\Sound Bank.xsb";
        
        public const string bambooClick = "bambooclick";
        public const string Explosion = "player-explosion";
        public const string LightColorA = "Saxdual";
        public const string LightColorB = "Saxmachine-high";

        private static bool OMG_RU_REDY_YET = false;

        public SoundEffects()
        {
            if (!OMG_RU_REDY_YET)
            {
                init();
            }
        }

        /// <summary>
        /// Passes the proper name parameters to the AudioManager for you!
        /// </summary>
        public static void PlayerExplosion()
        {
            AudioManager.PlaySoundEffect(EffectsGroupID.Player, Explosion);
        }

        /// <summary>
        /// Passes the proper name parameters to the AudioManager for you!
        /// </summary>
        public static void BambooClick()
        {
            AudioManager.PlaySoundEffect(EffectsGroupID.Player, bambooClick);
        }


        private static void init()
        {
            SoundEffectsGroup playerFX = new SoundEffectsGroup();
            playerFX.SoundBankName = playerEffectsSoundBankName;
            playerFX.WaveBankName = playerEffectsWaveBankName;
            SoundEffect effect1 = new SoundEffect(bambooClick);
            SoundEffect effect2 = new SoundEffect(Explosion);
            SoundEffect effect3 = new SoundEffect(LightColorA);
            SoundEffect effect4 = new SoundEffect(LightColorB);
            playerFX.Effects = new Dictionary<string, SoundEffect> {
                {bambooClick, effect1},
                {Explosion, effect2},
                {LightColorA, effect3},
                {LightColorB, effect4}
            };
            AudioManager.AddSoundEffectsGroup(EffectsGroupID.Player, playerFX);
            OMG_RU_REDY_YET = true;
        }

        public static void LoadAudio(Game param_game)
        {
            if (!OMG_RU_REDY_YET)
            {
                init();
            }
        }
    }
}