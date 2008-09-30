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

namespace Duologue.Screens
{
    public class LoadingScreen : GameScreen
    {
        #region Constants
        /// <summary>
        /// Transition time in seconds.
        /// </summary>
        private const float transitionTime = 0.5f;
        #endregion

        #region Fields
        private bool displayLoadImage;
        private bool otherScreensAreGone;

        private GameScreen[] screensToLoad;
        private Texture2D loadingScreen;
        private Vector2 center;
        private Vector2 position;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        private LoadingScreen(
            bool DisplayLoadImage,
            params GameScreen[] ScreensToLoad)
        {
            ScreenManager = InstanceManager.ScreenManager;
            displayLoadImage = DisplayLoadImage;
            screensToLoad = ScreensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
        }

        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(bool DisplayLoadImage,
                                params GameScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in InstanceManager.ScreenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(DisplayLoadImage,
                                                            screensToLoad);

            InstanceManager.ScreenManager.AddScreen(loadingScreen);
        }

        /// <summary>
        /// Load up the loading screen image(s)
        /// </summary>
        public override void LoadContent()
        {
            // Eventually, it might be nice to load multiple images
            loadingScreen = InstanceManager.AssetManager.LoadTexture2D("loading01");
            center = new Vector2(
                loadingScreen.Width / 2f,
                loadingScreen.Height / 2f);
            position = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f,
                InstanceManager.DefaultViewport.Height / 2f);
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            loadingScreen.Dispose();
            base.UnloadContent();
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        #endregion

        #region Update / Draw
        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen);
                    }
                }

                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if ((ScreenState == ScreenState.Active) &&
                (ScreenManager.GetScreens().Length == 1))
            {
                otherScreensAreGone = true;
            }

            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (displayLoadImage)
            {
                InstanceManager.RenderSprite.Draw(
                    loadingScreen,
                    position,
                    center,
                    null,
                    Color.White,
                    0f,
                    1f,
                    0f);
            }
        }
        #endregion
    }
}
