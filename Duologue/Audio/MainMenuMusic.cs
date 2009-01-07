using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Duologue.Audio
{
    public class MainMenuMusic
    {
        private static Cue music = null;
        private const string musicName = "nicStage_gso";

        private void init()
        {
            music = DuologueEnhancedAudioEngine.OneFileMusicSoundBank().GetCue(musicName);
        }

        public MainMenuMusic()
        {
            if(music == null)
                this.init();
        }

        public void Play()
        {
            if (!music.IsPlaying)
            {
                music.Play();
            }
            else
            {
                music.SetVariable(Loudness.param, Loudness.Full);
            }
        }

        public void Stop()
        {
            music.Stop(AudioStopOptions.AsAuthored);
            this.init();
        }

        /// <summary>
        /// This won't work until I make a change in the XACT project. Love, DoctorCal
        /// </summary>
        public void Mute()
        {
            music.SetVariable(Loudness.param, Loudness.Silent);
        }
    }
}
