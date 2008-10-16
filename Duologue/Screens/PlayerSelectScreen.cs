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
using Duologue.PlayObjects;
#endregion

namespace Duologue.Screens
{
    /// <summary>
    /// The player select game screen
    /// </summary>
    public class PlayerSelectScreen : GameScreen
    {
        #region Constants
        private const float backgroundTimer = 8f;
        #endregion

        #region Fields
        private Game localGame;
        private PlayerColors[] playerColors;
        private PlayerSelectBase playerSelectBase;
        private PlayerSelect playerSelect;
        private float timeSinceStart;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public PlayerSelectScreen(Game game)
            : base(game)
        {
            timeSinceStart = 0f;
            localGame = game;
            playerColors = PlayerColors.GetPlayerColors();

            playerSelectBase = new PlayerSelectBase(localGame);
            localGame.Components.Add(playerSelectBase);
            playerSelect = new PlayerSelect(localGame);
            localGame.Components.Add(playerSelect);
        }

        protected override void InitializeConstants()
        {
            MyComponents.Add(playerSelectBase);
            MyComponents.Add(playerSelect);

            this.SetEnable(false);
            this.SetVisible(false);
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        public override void SetVisible(bool t)
        {
            if (!t)
                base.ReInitAll();
            base.SetVisible(t);
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (timeSinceStart < backgroundTimer)
                timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
            {
                timeSinceStart = 0f;
                LocalInstanceManager.Background.NextBackground();
            }
            if (playerSelectBase.Percentage >= 1f)
                playerSelect.Alive = true;
            base.Update(gameTime);
        }
        #endregion
    }
}
