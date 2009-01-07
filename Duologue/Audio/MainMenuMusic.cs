﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Duologue.Audio
{
    public class MainMenuMusic
    {
        private static Cue music;
        private const string musicName = "nicStage_gso";

        public MainMenuMusic()
        {
            music = DuologueEnhancedAudioEngine.OneFileMusicSoundBank().GetCue(musicName);
        }

        public void Play()
        {
            music.Play();
        }
    }
}
