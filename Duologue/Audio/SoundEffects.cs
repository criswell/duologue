using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Duologue.Audio
{

    //the rule is: one sound bank = one group of effects = one EffectsGroupID
    public enum EffectsBankID { Player, Wiggles, Plucks }
    public enum PluckNote { A, A3, C, C3, E }
    public enum EffectID
    {
        Clock,
        PlayerExplosion,
        PlayerBeamA,
        PlayerBeamB,
        CokeBottle,
        WigglesDeath,
        CLONK,
        Ricochet,
        Sploosh,
        BuzzDeath
        //,newname
    }

    public class SoundEffect
    {
        public string CueName;
        public float Volume;

        public SoundEffect() { }
        public SoundEffect(string cue)
        {
            CueName = cue;
            Volume = Loudness.Normal;
        }
        public SoundEffect(string cue, float vol)
        {
            CueName = cue;
            Volume = vol;
        }
    }

    public class EffectsBank : GameComponent
    {
        public string SoundBankName;
        public string WaveBankName;
        //the dictionary key is the cue name...which we also need in
        //each of the sound effects. Crap.
        public Dictionary<string, SoundEffect> Effects =
            new Dictionary<string, SoundEffect>();
        public EffectsBank(Game game, string waves, string sounds) : base(game) 
        {
            WaveBankName = waves;
            SoundBankName = sounds;
        }
    }


    /// <summary>
    /// Convenience class. Provides no-arg methods that invoke AudioHelper generics
    /// </summary>
    public class SoundEffects
    {
        public const string PlayerEffectsWB = "Content\\Audio\\PlayerEffects.xwb";
        public const string PlayerEffectsSB = "Content\\Audio\\PlayerEffects.xsb";

        public const string PlucksWB = "Content\\Audio\\Plucks.xwb";
        public const string PlucksSB = "Content\\Audio\\Plucks.xsb";

        public const string A = "A";
        public const string A3 = "A3";
        public const string C = "C";
        public const string C3 = "C3";
        public const string E = "E";
        
        public const string Bamboo = "bambooclick";
        public const string Explosion = "player-explosion";
        public const string LightColorA = "Saxdual";
        public const string LightColorB = "Saxmachine-high";
        public const string CokeBottle = "edwin_p_manchester";
        public const string WigglesDeath = "WigglesDeath";
        public const string CLONK = "CLONK";
        public const string Ricochet = "ricochet";
        public const string Sploosh = "Sploosh";
        public const string BuzzDeath = "BuzzDeath";
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
                {EffectID.Ricochet, Ricochet},
                {EffectID.Sploosh, Sploosh},
                {EffectID.BuzzDeath, BuzzDeath}

                //,EffectID.YourNewEffectID, YourNewCueName
            };

        private static Dictionary<PluckNote, string> PluckMap =
            new Dictionary<PluckNote, string>
            {
                {PluckNote.A, A},
                {PluckNote.A3, A3},
                {PluckNote.C, C},
                {PluckNote.C3, C3},
                {PluckNote.E, E}
            };

        private EffectsBank playerBank;
        private EffectsBank plucksBank;

        public SoundEffects(AudioManager manager)
        {
            notifier = manager;
            playerBank = new EffectsBank(notifier.Game, PlayerEffectsWB, PlayerEffectsSB);

            List<string> effectNames = new List<string>();
            foreach (string name in IDNameMap.Values)
            {
                effectNames.Add(name);
                playerBank.Effects.Add(name, new SoundEffect(name));
            }
            AudioHelper.Preload(PlayerEffectsSB, PlayerEffectsWB, effectNames);

            plucksBank = new EffectsBank(notifier.Game, PlucksWB, PlucksSB);
            List<string> plucksNames = new List<string>();
            foreach (string name in PluckMap.Values)
            {
                plucksNames.Add(name);
                plucksBank.Effects.Add(name, new SoundEffect(name));
            }
            AudioHelper.Preload(PlucksSB, PlucksWB, plucksNames);

            notifier.Changed += new IntensityEventHandler(UpdateIntensity);
        }

        public void PlayEffect(EffectID ID)
        {
            AudioHelper.PlayCue(PlayerEffectsSB, IDNameMap[ID]);
            //I want this to be:
            //AudioManager.PlayEffect(EffectsMap[ID]);
        }

        public void PlayPluckNote(PluckNote note)
        {
            AudioHelper.PlayCue(PlucksSB, PluckMap[note]);
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

        public void A_()
        {
            PlayPluckNote(PluckNote.A);
        }

        public void A3_()
        {
            PlayPluckNote(PluckNote.A3);
        }

        public void C_()
        {
            PlayPluckNote(PluckNote.C);
        }

        public void C3_()
        {
            PlayPluckNote(PluckNote.C3);
        }

        public void E_()
        {
            PlayPluckNote(PluckNote.E);
        }
    }
}