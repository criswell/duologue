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
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        private LoadingScreen(
            bool DisplayLoadImage,
            params GameScreen[] ScreensToLoad)
        {
            displayLoadImage = DisplayLoadImage;
            screensToLoad = ScreensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        #endregion

        #region Update / Draw
        #endregion
    }
}
