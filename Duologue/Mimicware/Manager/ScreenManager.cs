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
    /// The ScreenManager manages one or more GameScreens which detail different elements in the game.
    /// It maintains a stack of screens, calls their update & draw methods, and passes input to
    /// the active one.
    /// </summary>
    public class ScreenManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields
        List<GameScreen> screens = new List<GameScreen>();
        List<GameScreen> screensToUpdate = new List<GameScreen>();

        InputManager input = new InputManager();

        Texture2D blankTexture;

        bool isInitialized;
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
            isInitialized = true;

            base.Initialize();
        }

        /// <summary>
        /// Load content
        /// </summary>
        protected override void LoadContent()
        {
            blankTexture = InstanceManager.AssetManager.LoadTexture2D("Mimicware\\blank");

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
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