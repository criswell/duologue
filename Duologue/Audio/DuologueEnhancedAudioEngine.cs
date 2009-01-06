using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Duologue.Audio
{
    static class DuologueEnhancedAudioEngine
    {
        static private AudioEngine engine;
        static private WaveBank waveBank;
        static private SoundBank soundBank;
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
            waveBank = new WaveBank(engine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(engine, "Content\\Audio\\Sound Bank.xsb");
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
        static public SoundBank SoundBank()
        {
            return soundBank;
        }
    }
}
