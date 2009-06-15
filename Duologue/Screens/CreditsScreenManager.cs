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
    public class CreditsScreenManager : GameScreen, IService
    {
        #region Constants
        #endregion

        #region Fields
        private DuologueGame localGame;
        private CreditsScreen creditsScreen;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public CreditsScreenManager(Game game)
            : base(game)
        {
            localGame = (DuologueGame)game;
            creditsScreen = new CreditsScreen(game, this);
            creditsScreen.DrawOrder = 4;
            creditsScreen.Enabled = false;
            creditsScreen.Visible = false;
            localGame.Components.Add(creditsScreen);
        }
        protected override void InitializeConstants()
        {
            MyComponents.Add(creditsScreen);

            this.SetEnable(false);
            this.SetVisible(false);
        }
        #endregion

        #region Public
        public override void ScreenEntrance(GameTime gameTime)
        {
            creditsScreen.ResetAll();
            base.ScreenEntrance(gameTime);
        }

        /*
        public override void ScreenExit(GameTime gameTime)
        {
            creditsScreen.ResetAll();
            base.ScreenExit(gameTime);
        }*/
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (InstanceManager.InputManager.NewButtonPressed(Buttons.Back) || 
                InstanceManager.InputManager.NewButtonPressed(Buttons.B))
                LocalInstanceManager.CurrentGameState = GameState.MainMenuSystem;
            base.Update(gameTime);
        }
        #endregion
    }
}
