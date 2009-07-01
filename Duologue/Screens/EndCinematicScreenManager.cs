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
        private const int timesButtonMustBePressed = 10;
        #endregion

        #region Fields
        private DuologueGame localGame;
        private EndCinematicScreen endCinematicScreen;
        private int numberOfButtonPresses;
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
            ServiceLocator.RegisterService(endCinematicScreen);
        }
        protected override void InitializeConstants()
        {
            MyComponents.Add(endCinematicScreen);

            this.SetEnable(false);
            this.SetVisible(false);
        }

        public override void ScreenEntrance(GameTime gameTime)
        {
            numberOfButtonPresses = 0;
            base.ScreenEntrance(gameTime);
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (InstanceManager.InputManager.NewButtonPressed(Buttons.A))
            {
                numberOfButtonPresses++;
                if(numberOfButtonPresses > timesButtonMustBePressed)
                    LocalInstanceManager.CurrentGameState = GameState.Credits;
            }
            base.Update(gameTime);
        }
        #endregion
    }
}
