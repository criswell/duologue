using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Duologue.Audio
{
    /// <summary>
    /// 
    /// </summary>
    public class BeatEffectsSong
    {

        static private Cue beatSound = null;
        static private Cue danceBass = null;
        static private Cue danceBassplus = null;
        static private Cue danceBeat = null;
        static private Cue danceOrgan = null;
        static private Cue danceGuitar = null;
        private const string beatName = "beat";
        private const string bassName = "bass";
        private const string bassplusName = "bassplus";
        private const string guitarName = "guitar";
        private const string organName = "organ";
        static private int intensity = -1;

        public BeatEffectsSong()
        {
            if(intensity<0)
                initCues();
        }

        protected void initCues()
        {
            intensity = 1;
            danceBeat =
                DuologueEnhancedAudioEngine.BeatEffectsSoundBank().GetCue(beatName);
            danceBass =
                DuologueEnhancedAudioEngine.BeatEffectsSoundBank().GetCue(bassName);
            danceBassplus =
                DuologueEnhancedAudioEngine.BeatEffectsSoundBank().GetCue(bassplusName);
            danceGuitar =
                DuologueEnhancedAudioEngine.BeatEffectsSoundBank().GetCue(guitarName);
            danceOrgan =
                DuologueEnhancedAudioEngine.BeatEffectsSoundBank().GetCue(organName);
            danceBeat.SetVariable(Loudness.param, Loudness.Full);
            danceBass.SetVariable(Loudness.param, Loudness.Silent);
            danceBassplus.SetVariable(Loudness.param, Loudness.Silent);
            danceGuitar.SetVariable(Loudness.param, Loudness.Silent);
            danceOrgan.SetVariable(Loudness.param, Loudness.Silent);
        }

        /// <summary>
        /// Increase musical excitement!
        /// </summary>
        public void IncreaseIntensity()
        {
            switch (intensity)
            {
                case 1:
                    danceBass.SetVariable(Loudness.param, Loudness.Full);
                    intensity++;
                    break;
                case 2:
                    danceBassplus.SetVariable(Loudness.param, Loudness.Full);
                    intensity++;
                    break;
                case 3:
                    danceOrgan.SetVariable(Loudness.param, Loudness.Full);
                    intensity++;
                    break;
                case 4:
                    danceGuitar.SetVariable(Loudness.param, Loudness.Full);
                    intensity++;
                    break;
                case 5:
                    break;
                default:
                    intensity = 1;
                    break;
            }
        }


        /// <summary>
        /// Reduce musical excitement!
        /// </summary>
        public void DecreaseIntensity()
        {
            switch (intensity)
            {
                case 1:
                    intensity--;
                    break;
                case 2:
                    danceBass.SetVariable("Volume", Loudness.Silent);
                    intensity--;
                    break;
                case 3:
                    danceBassplus.SetVariable("Volume", Loudness.Silent);
                    intensity--;
                    break;
                case 4:
                    danceOrgan.SetVariable("Volume", Loudness.Silent);
                    intensity--;
                    break;
                case 5:
                    danceGuitar.SetVariable("Volume", Loudness.Silent);
                    intensity--;
                    break;
                default:
                    intensity = 1;
                    break;
            }
        }

        /// <summary>
        /// Techno. Sounds like Keeper's Pub up in here!
        /// </summary>
        public void PlayDance()
        {
            danceBass.Play();
            danceBassplus.Play();
            danceBeat.Play();
            danceGuitar.Play();
            danceOrgan.Play();
        }

        /// <summary>
        /// Stop that. Just stop it.
        /// </summary>
        public void StopDance()
        {
            danceBass.Stop(AudioStopOptions.AsAuthored);
            danceBassplus.Stop(AudioStopOptions.AsAuthored);
            danceBeat.Stop(AudioStopOptions.AsAuthored);
            danceGuitar.Stop(AudioStopOptions.AsAuthored);
            danceOrgan.Stop(AudioStopOptions.AsAuthored);
            initCues();
        }

    }
}