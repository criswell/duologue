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
using Mimicware.Fx;
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
    public class BuyScreen : DrawableGameComponent
    {
        #region Constants
        #endregion

        #region Fields
        private Game myGame;
        private BuyScreenManager myManager;
        #endregion

        #region Constructor / Init
        public BuyScreen(Game game, BuyScreenManager manager) 
            : base(game)
        {
            myGame = game;
            myManager = manager;
        }
        #endregion

        #region Public methods
        #endregion

        #region Draw / Update
        #endregion
    }
}
