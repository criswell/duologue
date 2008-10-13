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
        private Game localGame;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public MainMenuScreen(Game game)
            : base(game)
        {
            timeSinceStart = 0f;
            localGame = game;
            mainGameLogo = new MainGameLogo(localGame);
            mainMenu = new MainMenu(localGame);
            localGame.Components.Add(mainGameLogo);
            localGame.Components.Add(mainMenu);
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }
        #endregion

        #region Initialize Constants
        protected override void InitializeConstants()
        {
            MyComponents.Add(mainGameLogo);
            MyComponents.Add(mainMenu);

            this.SetEnable(true);
            this.SetVisible(true);
            //this.InitAll();
        }
        #endregion

        #region Private methods
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
        #endregion
    }
}
