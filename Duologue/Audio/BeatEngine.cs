using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;

// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Duologue.Audio
{


    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class BeatEngine : Microsoft.Xna.Framework.GameComponent
    {
        #region Private Fields
        private float beatTimer = 0f;
        private float beatInterval = 1000.0f;
        private AudioEngine engine;
        private WaveBank waveBank;
        private SoundBank soundBank;
        private Cue beatSound = null;
        private WaveBank danceWaveBank;
        private SoundBank danceSoundBank;
        private Cue danceBass = null;
        private Cue danceBassplus = null;
        private Cue danceBeat = null;
        private Cue danceOrgan = null;
        private Cue danceGuitar = null;
        private int intensity;
        private float mute = 0.0f;
        private float loud = 999.0f;
        #endregion

        #region Public Fields
        #endregion

        #region Properties
        /// <summary>
        /// Hmmmm. There should somehow be singleton access to the audio device
        /// </summary>

        public float PercentFromTarget()
        {
            return (MillisecondsFromTarget() - beatInterval)*100.0f;
        }

        public float MillisecondsFromTarget()
        {
            float halfPeriod = beatInterval/2.0f;
            return beatTimer < halfPeriod ? beatTimer : beatInterval - beatTimer;
        }

        /// <summary>
        /// If we decide to implement, this will return the audible length of the beat
        /// </summary>
        /// <returns></returns>
        public float TargetLengthInMilliseconds()
        {
            return 0;
        }

        public float BeatPeriodInMilliseconds()
        {
            return beatInterval;
        }
        #endregion

        public BeatEngine(Game game)
            : base(game)
        {
            intensity = 1;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            this.Enabled = false;
            engine = new AudioEngine("Content\\Audio\\Duologue.xgs");
            waveBank = new WaveBank(engine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(engine, "Content\\Audio\\Sound Bank.xsb");
            danceWaveBank = new WaveBank(engine, "Content\\Audio\\Flick3r_Dance.xwb");
            danceSoundBank = new SoundBank(engine, "Content\\Audio\\Flick3r_Dance.xsb");
            danceBeat = danceSoundBank.GetCue("beat");
            danceBass = danceSoundBank.GetCue("bass");
            danceBassplus = danceSoundBank.GetCue("bassplus");
            danceGuitar = danceSoundBank.GetCue("guitar");
            danceOrgan = danceSoundBank.GetCue("organ");
            danceBeat.SetVariable("Volume", loud);
            danceBass.SetVariable("Volume", mute);
            danceBassplus.SetVariable("Volume", mute);
            danceGuitar.SetVariable("Volume", mute);
            danceOrgan.SetVariable("Volume", mute);

            base.Initialize();
        }


        /// <summary>
        /// Increase musical excitement!
        /// </summary>
        public void IncreaseIntensity()
        {
            switch (intensity)
            {
                case 1:
                    danceBass.SetVariable("Volume", loud);
                    intensity++;
                    break;
                case 2:
                    danceBassplus.SetVariable("Volume", loud);
                    intensity++;
                    break;
                case 3:
                    danceOrgan.SetVariable("Volume", loud);
                    intensity++;
                    break;
                case 4:
                    danceGuitar.SetVariable("Volume", loud);
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
                    danceBass.SetVariable("Volume", mute);
                    intensity--;
                    break;
                case 3:
                    danceBassplus.SetVariable("Volume", mute);
                    intensity--;
                    break;
                case 4:
                    danceOrgan.SetVariable("Volume", mute);
                    intensity--;
                    break;
                case 5:
                    danceGuitar.SetVariable("Volume", mute);
                    intensity--;
                    break;
                default:
                    intensity = 1;
                    break;
            }
        }

        /// <summary>
        /// Click, click, man, danged ol' Hank, I'll tell ya what.
        /// </summary>
        public void PlayBeat()
        {
            if (beatSound == null || beatSound.IsStopped)
            {
                beatSound = soundBank.GetCue("bambooclick");
                beatSound.Play();
            }
            else if (beatSound.IsPaused)
            {
                beatSound.Resume();
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
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            //beatTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //if (beatTimer > beatInterval)
            //{
                // Do Something
              //  PlayBeat();
              //  beatTimer = 0f;
            //}
            base.Update(gameTime);
        }
    }
}