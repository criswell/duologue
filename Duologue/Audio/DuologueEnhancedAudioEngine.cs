using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Duologue.Audio
{
    //keep from having to tweak floats and add levels in many places
    public struct Loudness
    {
        public const float Silent = 0.0f;
        public const float Full = 999.0f;
        public const string param = "Volume";
    }

    public static class DuologueEnhancedAudioEngine
    {
        static private AudioEngine engine;
        static private WaveBank playerEffectsWaveBank;
        static private SoundBank playerEffectsSoundBank;
        static private WaveBank danceWaveBank;
        static private SoundBank danceSoundBank;
        static private WaveBank oneFileMusicWaveBank;
        static private SoundBank oneFileMusicSoundBank;

        /// <summary>
        /// Static class can only have a static c'tor
        /// </summary>
        static DuologueEnhancedAudioEngine()
        {
            engine = new AudioEngine("Content\\Audio\\Duologue.xgs");
            playerEffectsWaveBank = new WaveBank(engine, "Content\\Audio\\Wave Bank.xwb");
            playerEffectsSoundBank = new SoundBank(engine, "Content\\Audio\\Sound Bank.xsb");
            danceWaveBank = new WaveBank(engine, "Content\\Audio\\Flick3r_Dance.xwb");
            danceSoundBank = new SoundBank(engine, "Content\\Audio\\Flick3r_Dance.xsb");
            oneFileMusicWaveBank = new WaveBank(engine, "Content\\Audio\\OneFileMusic.xwb");
            oneFileMusicSoundBank = new SoundBank(engine, "Content\\Audio\\OneFileMusic.xsb");
        }

        /// <summary>
        /// Return access to the Main Menu sound bank
        /// </summary>
        /// 
        static public SoundBank OneFileMusicSoundBank()
        {
            return oneFileMusicSoundBank;
        }

        /// <summary>
        /// Return access to the Beat Engine sound bank
        /// </summary>
        static public SoundBank BeatEffectsSoundBank()
        {
            return danceSoundBank;
        }

        /// <summary>
        /// Return access to the default sound bank
        /// </summary>
        static public SoundBank PlayerEffectsSoundBank()
        {
            return playerEffectsSoundBank;
        }
    }
}
