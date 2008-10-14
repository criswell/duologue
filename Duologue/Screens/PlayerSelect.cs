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
    class PlayerSelect : GameScreen
    {
        #region Constants
        #endregion

        #region Fields
        private Game localGame;
        private PlayerColors playerColors;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public PlayerSelect(Game game)
            : base(game)
        {
            localGame = game;
            playerColors = PlayerColors.GetPlayerColors();
        }

        protected override void InitializeConstants()
        {
            this.SetEnable(true);
            this.SetVisible(true);
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        #endregion

        #region Update
        #endregion
    }
}
