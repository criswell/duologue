using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Mimicware;


namespace Duologue.Audio
{

    public delegate void IntensityEventHandler(object sender, EventArgs e);
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AudioManager : Microsoft.Xna.Framework.GameComponent
    {
        private Game localgame;
        private AudioHelper helper;
        private float intensity = 0.0f;
        public Music music;
        public SoundEffects soundEffects;

        public const string engine = "Content\\Audio\\Duologue.xgs";

        public event IntensityEventHandler Changed;
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        public AudioManager(Game game)
            : base(game)
        {
            localgame = game;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            helper = new AudioHelper(localgame, engine);
            music = new Music(this);
            soundEffects = new SoundEffects();
            SoundEffects.init(localgame);
            localgame.Components.Add(helper);

            base.Initialize();
        }

        public float Intensity
        {
            get { return intensity; }
            set
            {
                intensity = value;
                MWMathHelper.LimitToRange(intensity, 0.0f, 1.0f);
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}