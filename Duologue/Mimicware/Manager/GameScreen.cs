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
    /// The current screen's state
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    /// <summary>
    /// FIXME - this description is retarded
    /// A game screen defines a basic layer element. In the game, multiple game screens
    /// can be stacked to define a blah blah
    /// </summary>
    class GameScreen
    {
        #region Fields
        private bool isPopup = false;
        private float transitionOnTime = 0f;
        private float transitionOffTime = 0f;
        private float transitionPercentage = 1f;
        private ScreenState screenState;
        private bool isExiting = false;
        private bool otherScreenHasFocus;
        private ScreenManager screenManager;
        #endregion

        #region Properties
        /// <summary>
        /// Is the game screen a pop up or not
        /// </summary>
        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        /// <summary>
        /// How long (in seconds) the transition on effect will be
        /// </summary>
        public float TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        /// <summary>
        /// How long (in seconds) the transition off effect will be
        /// </summary>
        public float TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        /// <summary>
        /// The percentage of the transition effect, between 0 and 1.
        /// 0 means that the screen is fully on,
        /// 1 means the screen is fully off.
        /// </summary>
        public float TransitionPercentage
        {
            get { return transitionPercentage; }
            protected set { transitionPercentage = value; }
        }

        /// <summary>
        /// Get the current transition alpha
        /// </summary>
        public byte TransitionAlpha
        {
            get { return (byte)(255 - TransitionPosition * 255); }
        }

        /// <summary>
        /// Gets the current screen transition state.
        /// </summary>
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        /// <summary>
        /// There are two possible reasons why a screen might be transitioning
        /// off. It could be temporarily going away to make room for another
        /// screen that is on top of it, or it could be going away for good.
        /// This property indicates whether the screen is exiting for real:
        /// if set, the screen will automatically remove itself as soon as the
        /// transition finishes.
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        /// <summary>
        /// Checks whether this screen is active and can respond to user input.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus &&
                       (screenState == ScreenState.TransitionOn ||
                        screenState == ScreenState.Active);
            }
        }

        /// <summary>
        /// Gets the manager that this screen belongs to.
        /// </summary>
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }
        #endregion

        #region Constructor / Init / Load
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        #endregion

        #region Draw / Update
        #endregion
    }
}
