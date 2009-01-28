using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Duologue.Audio
{

    //the rule is: one sound bank = one group of effects = one EffectsGroupID
    public enum EffectsBankID { Player, Wiggles }
    public enum EffectID
    {
        Clock,
        PlayerExplosion,
        PlayerBeamA,
        PlayerBeamB,
        CokeBottle,
        WigglesDeath,
        CLONK,
        Ricochet//, YourNewEffectID
    }

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
        public const string CokeBottle = "edwin_p_manchester";
        public const string WigglesDeath = "WigglesDeath";
        public const string CLONK = "CLONK";
        public const string Ricochet = "ricochet";
        //public const string YourNewCueName = "Cue as named in XACT";

        private AudioManager notifier;

        private static Dictionary<EffectID, string> IDNameMap = 
            new Dictionary<EffectID, string>
            {
                {EffectID.Clock, Bamboo},
                {EffectID.PlayerExplosion, Explosion},
                {EffectID.PlayerBeamA, LightColorA},
                {EffectID.PlayerBeamB, LightColorB},
                {EffectID.CokeBottle, CokeBottle},
                {EffectID.WigglesDeath, WigglesDeath},
                {EffectID.CLONK, CLONK},
                {EffectID.Ricochet, Ricochet}

                //,EffectID.YourNewEffectID, YourNewCueName
            };
        private EffectsBank playerBank = new EffectsBank();

        public SoundEffects(AudioManager manager)
        {
            playerBank.SoundBankName = PlayerEffectsSB;
            playerBank.WaveBankName = PlayerEffectsWB;

            List<string> effectNames = new List<string>();
            foreach (string name in IDNameMap.Values)
            {
                effectNames.Add(name);
                playerBank.Effects.Add(name, new SoundEffect(name));
            }

            AudioHelper.AddBank(PlayerEffectsSB, PlayerEffectsWB, effectNames);

            notifier = manager;
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);
        }

        public void PlayEffect(EffectID ID)
        {
            AudioHelper.PlayCue(PlayerEffectsSB, IDNameMap[ID], PlayType.Single);
        }

        public void StopEffect(EffectID ID)
        {
            AudioHelper.StopCue(PlayerEffectsSB, IDNameMap[ID]);
        }

        public void UpdateIntensity(IntensityEventArgs e)
        {
        }

        public void Detach()
        {
            notifier.Changed -= new IntensityEventHandler(UpdateIntensity);
            notifier = null;
        }

        /// <summary>
        /// Passes the proper name parameters to the AudioHelper
        /// </summary>
        public void PlayerExplosion()
        {
            PlayEffect(EffectID.PlayerExplosion);
        }

        /// <summary>
        /// Passes the proper name parameters to the AudioHelper
        /// </summary>
        public void BambooClick()
        {
            PlayEffect(EffectID.Clock);
        }

    }
}