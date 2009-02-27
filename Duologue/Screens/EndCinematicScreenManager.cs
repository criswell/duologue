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
    public class EndCinematicScreenManager : GameScreen
    {
        #region Constants
        #endregion

        #region Fields
        private DuologueGame localGame;
        private EndCinematicScreen endCinematicScreen;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public EndCinematicScreenManager(Game game)
            : base(game)
        {
            localGame = (DuologueGame)game;
            endCinematicScreen = new EndCinematicScreen(game, this);
            endCinematicScreen.DrawOrder = 4;
            endCinematicScreen.Enabled = false;
            endCinematicScreen.Visible = false;
            localGame.Components.Add(endCinematicScreen);
        }
        protected override void InitializeConstants()
        {
            MyComponents.Add(endCinematicScreen);

            this.SetEnable(false);
            this.SetVisible(false);
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (InstanceManager.InputManager.NewButtonPressed(Buttons.Start) || InstanceManager.InputManager.NewButtonPressed(Buttons.A))
                LocalInstanceManager.CurrentGameState = GameState.MainMenuSystem;
            base.Update(gameTime);
        }
        #endregion
    }
}
