#region File Description
#endregion

#region Using Statements
// System
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
// Mimicware
using Mimicware.Manager;
using Mimicware.Graphics;
// Duologue
using Duologue;
using Duologue.Audio;
using Duologue.Properties;
using Duologue.Screens;
#endregion


namespace Duologue.Screens
{
    public class MainMenuScreen : GameScreen
    {
        #region Constants
        private const float backgroundTimer = 8f;
        #endregion

        #region Fields
        private MainGameLogo mainGameLogo;
        private MainMenu mainMenu;
        private float timeSinceStart;
        private AudioManager audio;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public MainMenuScreen(Game game)
            : base(game)
        {
            timeSinceStart = 0f;
            mainGameLogo = new MainGameLogo(Game);
            mainMenu = new MainMenu(Game);
            Game.Components.Add(mainGameLogo);
            Game.Components.Add(mainMenu);
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            audio = ServiceLocator.GetService<AudioManager>();
            base.Initialize();
        }
        #endregion

        #region Initialize Constants
        protected override void InitializeConstants()
        {
            MyComponents.Add(mainGameLogo);
            MyComponents.Add(mainMenu);

            this.SetEnable(false);
            this.SetVisible(false);
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        internal void LivePurchaseReset()
        {
            mainMenu.LivePurchaseReset();
        }
        #endregion

        #region Overrides
        public override void SetEnable(bool t)
        {
            if(t)
                mainMenu.Reset();
            base.SetEnable(t);
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (mainGameLogo.PercentComplete < 1f)
            {
                mainMenu.Enabled = false;
                mainMenu.Visible = false;
            }
            else
            {
                mainMenu.Enabled = true;
                mainMenu.Visible = true;
            }
            if (timeSinceStart < backgroundTimer)
                timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
            {
                timeSinceStart = 0f;
                LocalInstanceManager.Background.NextBackground();
            }

            base.Update(gameTime);
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (null != audio)
            {
                if (Enabled)
                {
                    if (audio.SongIsPaused(SongID.SelectMenu))
                    {
                        audio.ResumeSong(SongID.SelectMenu);
                    }
                    else
                    {
                        audio.FadeIn(SongID.SelectMenu);
                    }
                }
                else
                {
                    audio.PauseSong(SongID.SelectMenu);
                }
            }
            base.OnEnabledChanged(sender, args);
        }
        #endregion
    }
}
