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

        static private Dictionary<string, Cue> cues;

        static private Cue beatSound = null;
        static private Cue danceBass = null;
        static private Cue danceBassplus = null;
        static private Cue danceBeat = null;
        static private Cue danceOrgan = null;
        static private Cue danceGuitar = null;
        static private int intensity = -1;

        public BeatEffectsSong()
        {
            cues = new Dictionary<string, Cue>();
            if(intensity<0)
                initCues();
        }

        protected void initCues()
        {/*
            intensity = 1;
            danceBeat =
                AudioManager.getCue(DuologueAudioNames.danceSoundBank, DuologueAudioNames.beatName);
            danceBass =
                AudioManager.getCue(DuologueAudioNames.danceSoundBank, DuologueAudioNames.bassName);
            danceBassplus =
                AudioManager.getCue(DuologueAudioNames.danceSoundBank, DuologueAudioNames.bassplusName);
            danceOrgan =
                AudioManager.getCue(DuologueAudioNames.danceSoundBank, DuologueAudioNames.organName);
            danceGuitar =
                AudioManager.getCue(DuologueAudioNames.danceSoundBank, DuologueAudioNames.guitarName);
            danceBeat.SetVariable(Loudness.param, Loudness.Full);
            danceBass.SetVariable(Loudness.param, Loudness.Silent);
            danceBassplus.SetVariable(Loudness.param, Loudness.Silent);
            danceGuitar.SetVariable(Loudness.param, Loudness.Silent);
            danceOrgan.SetVariable(Loudness.param, Loudness.Silent);*/
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
    }
}