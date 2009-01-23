using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Net;
//using Microsoft.Xna.Framework.Storage;
using Mimicware;


namespace Duologue.Audio
{

    /*
    class Test
    {
        public static void penis() 
        {
            AudioManager notifier = new AudioManager(new Game());
            BeatEffectsSong listener = new BeatEffectsSong(notifier);
            notifier.Intensity = 0.22f;
            listener.Detach();
        }
    }
    */

    public delegate void IntensityEventHandler(object sender, EventArgs e);
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AudioManager : Microsoft.Xna.Framework.GameComponent
    {
        private Game localgame;
        private AudioHelper helper;
        private float intensity = -1.0f;
        private Music music;
        private SoundEffects soundEffects;

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
            helper = new AudioHelper(localgame, engine);
            music = new Music();
            soundEffects = new SoundEffects();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            SoundEffects.init(localgame);
            localgame.Components.Add(helper);

            base.Initialize();
        }

        public float Intensity
        {
            get { return intensity; }
            set
            {
                MWMathHelper.LimitToRange(value, 0.0f, 1.0f);
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