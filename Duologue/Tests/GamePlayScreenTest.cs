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

namespace Duologue.Tests
{
    public class GamePlayScreenTest : GameScreen
    {
        #region Constants
        #endregion

        #region Fields
        private Game localGame;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public GamePlayScreenTest(Game game)
            : base(game)
        {
            localGame = game;
        }

        protected override void InitializeConstants()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        #endregion

        #region Update
        #endregion
    }
}
