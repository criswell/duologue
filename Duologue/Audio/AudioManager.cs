using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Mimicware;


namespace Duologue.Audio
{
    public class IntensityEventArgs : EventArgs
    {
        public int ChangeAmount;
    }

    public delegate void IntensityEventHandler(IntensityEventArgs e);
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AudioManager : Microsoft.Xna.Framework.GameComponent, IService
    {
        private DuologueGame localgame;
        private AudioHelper helper;
        public Music music;
        public SoundEffects soundEffects;

        public const string engine = "Content\\Audio\\Duologue.xgs";
        public event IntensityEventHandler Changed;
        protected virtual void OnChanged(IntensityEventArgs e)
        {
            if (Changed != null)
                Changed(e);
        }

        public AudioManager(Game game) : base(game)
        {
            localgame = ((DuologueGame)game);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            helper = new AudioHelper(localgame, engine);
            music = new Music(this);
            soundEffects = new SoundEffects(this);
            localgame.Components.Add(helper);

            base.Initialize();
        }

        public void Intensify()
        {
            ChangeIntensity(1);
        }

        public void ChangeIntensity(int amount)
        {
            IntensityEventArgs e = new IntensityEventArgs();
            e.ChangeAmount = amount;
            OnChanged(e);
        }

        public void Detensify()
        {
            ChangeIntensity(-1);
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