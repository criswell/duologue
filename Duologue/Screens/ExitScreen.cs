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


namespace Duologue.Screens
{
    /// <summary>
    /// Dummy exit screen until we do something fancier
    /// </summary>
    public class ExitScreen : GameScreen
    {
        #region Fields
        private Game localGame;
        #endregion
        protected override void InitializeConstants()
        {
            
        }

        public ExitScreen(Game game)
            :
            base(game)
        {
            localGame = game;
        }

        public override void Update(GameTime gameTime)
        {
            localGame.Exit();
        }
    }
}
