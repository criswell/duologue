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
using Duologue.State;
using Duologue.Properties;
using Duologue.Screens;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.UI;
#endregion

namespace Duologue.Screens
{
    public class ColorStateTestScreen : GameScreen
    {
        #region Constants
        private const float backgroundTimer = 8f;
        #endregion

        #region Fields
        private ColorStateTestComponent test;
        private Game localGame;
        private float timeSinceStart;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public ColorStateTestScreen(Game game)
            : base(game)
        {
            localGame = game;
            test = new ColorStateTestComponent(game);
            test.Enabled = false;
            test.Visible = false;
            game.Components.Add(test);
            //MyComponents.Add(test);
            timeSinceStart = 0f;
        }

        protected override void InitializeConstants()
        {
            MyComponents.Add(test);
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceStart > backgroundTimer)
            {
                timeSinceStart = 0f;
                LocalInstanceManager.Background.NextBackground();
            }
            if (InstanceManager.InputManager.ButtonPressed(Buttons.Back))
                LocalInstanceManager.CurrentGameState = GameState.MainMenuSystem;
            base.Update(gameTime);
        }
        #endregion
    }
}
