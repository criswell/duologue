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
    public class CompanyIntroScreenManager : GameScreen
    {
        #region Constants
        #endregion

        #region Fields
        private CompanyIntroScreen introScreen;
        #endregion 

        #region Properties
        #endregion

        #region Constructor / Init
        public CompanyIntroScreenManager(Game game) : base(game)
        {
            introScreen = new CompanyIntroScreen(game, this);
            game.Components.Add(introScreen);
            introScreen.DrawOrder = 200;
            introScreen.Enabled = false;
            introScreen.Visible = false;
        }

        protected override void InitializeConstants()
        {
            MyComponents.Add(introScreen);

            this.SetEnable(false);
            this.SetVisible(false);
        }
        #endregion

        #region Public Methods
        public void Exit()
        {
            LocalInstanceManager.CurrentGameState = GameState.MainMenuSystem;
        }
        #endregion

        #region Public Overrides
        #endregion

        #region Update
        #endregion
    }
}
