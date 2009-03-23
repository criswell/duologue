using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Duologue.Mimicware;

namespace Duologue.Audio
{

    public enum EffectID
    {
        Clock, PlayerExplosion, CokeBottle, WigglesDeath, CLONK, 
        Ricochet, Sploosh, BuzzDeath
        //,newname
    }

    public enum EffectFileID
    {
        PlayerExplosion
    }

    public class SoundEffectThing
    {
        public string CueName;
        public float Volume;

        public SoundEffectThing() { }
        public SoundEffectThing(string cue)
        {
            CueName = cue;
            Volume = Loudness.Normal;
        }
        public SoundEffectThing(string cue, float vol)
        {
            CueName = cue;
            Volume = vol;
        }
    }


    /// <summary>
    /// Convenience class. Provides no-arg methods that invoke AudioHelper generics
    /// </summary>
    public class SoundEffects
    {
        public const string EffectsFilePath = "Audio\\PlayerEffects\\";
        public const string PlayerExplosionFile = EffectsFilePath + "player-explosion";

        public const string PlayerEffectsWB = "Content\\Audio\\PlayerEffects.xwb";
        public const string PlayerEffectsSB = "Content\\Audio\\PlayerEffects.xsb";

        public const string Bamboo = "bambooclick";
        public const string Explosion = "player-explosion";
        public const string CokeBottle = "edwin_p_manchester";
        public const string WigglesDeath = "WigglesDeath";
        public const string CLONK = "CLONK";
        public const string Ricochet = "ricochet";
        public const string Sploosh = "Sploosh";
        public const string BuzzDeath = "BuzzDeath";
        //public const string YourNewCueName = "Cue as named in XACT";

        protected IntensityNotifier notifier;
        protected DuologueGame game;

        private static Dictionary<EffectID, string> IDNameMap = 
            new Dictionary<EffectID, string>
            {
                {EffectID.Clock, Bamboo},
                {EffectID.PlayerExplosion, Explosion},
                {EffectID.CokeBottle, CokeBottle},
                {EffectID.WigglesDeath, WigglesDeath},
                {EffectID.CLONK, CLONK},
                {EffectID.Ricochet, Ricochet},
                {EffectID.Sploosh, Sploosh},
                {EffectID.BuzzDeath, BuzzDeath}
                //,EffectID.YourNewEffectID, YourNewCueName
            };

        private static Dictionary<EffectFileID, string> IDFileMap =
            new Dictionary<EffectFileID, string>
            {
                {EffectFileID.PlayerExplosion, PlayerExplosionFile}
            };

        public SoundEffects()
        {
            notifier = ServiceLocator.GetService<IntensityNotifier>();
            game = ServiceLocator.GetService<DuologueGame>();

            List<string> effectNames = new List<string>();
            foreach (string name in IDNameMap.Values)
            {
                effectNames.Add(name);
            }
            AudioHelper.Preload(PlayerEffectsSB, PlayerEffectsWB, effectNames);
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);
        }

        public void PlayEffect(EffectID ID)
        {
            AudioHelper.PlayCue(PlayerEffectsSB, IDNameMap[ID]);
            //FIXME I want this to be:
            //AudioManager.PlayEffect(EffectsMap[ID]);
        }

        public SoundEffectInstance PlayEffectFile(EffectFileID ID, 
            float volume, float pitch, float pan)
        {
            SoundEffect effect = game.Content.Load<SoundEffect>(IDFileMap[ID]);
            return effect.Play(volume, pitch, pan, false);
        }

        public SoundEffectInstance PlayEffectFile(EffectFileID ID)
        {
            return PlayEffectFile(ID, 1f, 0f, 0f);
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
            if (false)
            {
                PlayEffect(EffectID.PlayerExplosion);
            }
            else
            {
                SoundEffectInstance explosionInstance =
                    PlayEffectFile(EffectFileID.PlayerExplosion);
                GamePadHelper pad = new GamePadHelper(game, PlayerIndex.One);
                pad.ChirpIt(500f, 0f, 1f);                
            }
        }

        public void FinalPlayerExplosion()
        {
            SoundEffectInstance explosionInstance =
                PlayEffectFile(EffectFileID.PlayerExplosion);
            SoundEffectInstance highBlast =
                PlayEffectFile(EffectFileID.PlayerExplosion, .4f, 1f, .3f);
            SoundEffectInstance deepBlast = 
                PlayEffectFile(EffectFileID.PlayerExplosion, .6f, -1f, -.3f);
            GamePadHelper pad = new GamePadHelper(game, PlayerIndex.One);
            //FIXME should be for whichever player is exploding^^^^^
            pad.ShakeIt(500f, 1f, 1f);
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