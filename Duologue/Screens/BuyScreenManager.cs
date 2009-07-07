#region File Description
#endregion

#region Using Statements
// System
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
// Mimicware
using Mimicware.Manager;
using Mimicware.Graphics;
using Mimicware;
// Duologue
using Duologue;
using Duologue.Audio;
using Duologue.State;
using Duologue.Properties;
using Duologue.Screens;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.UI;
#endregion

namespace Duologue.Screens
{
    public class BuyScreenManager : GameScreen, IService
    {
        #region Constants
        #endregion

        #region Fields
        private Game myGame;
        private BuyScreen buyScreen;
        #endregion

        #region Properties
        #endregion

        #region Constructor/Init
        public BuyScreenManager(Game game)
            : base(game)
        {
            myGame = game;
            buyScreen = new BuyScreen(myGame, this);
            buyScreen.DrawOrder = 4;
            buyScreen.Enabled = false;
            buyScreen.Visible = false;
            myGame.Components.Add(buyScreen);
        }
        protected override void InitializeConstants()
        {
            MyComponents.Add(buyScreen);

            this.SetEnable(false);
            this.SetVisible(false);
        }

        /// <summary>
        /// Only call when in trial mode
        /// </summary>
        public void LoadForTrialMode()
        {
            buyScreen.LoadScreenshots();
            // Load music
        }
        #endregion

        #region Public methods
        public override void ScreenEntrance(GameTime gameTime)
        {
            base.ScreenEntrance(gameTime);
        }

        public override void ScreenExit(GameTime gameTime)
        {
            base.ScreenExit(gameTime);
        }
        #endregion

        #region Update
        #endregion
    }
}
