#region File Description
#endregion

#region Using statements
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
using Mimicware;
using Mimicware.Graphics;
using Mimicware.Manager;
using Mimicware.Debug;
#endregion

namespace Mimicware.Manager
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ScreenManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public ScreenManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Load content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        internal static void RemoveScreen(GameScreen gameScreen)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region Update / Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        #endregion
    }
}