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
        private TimeSpan transitionOnTime = TimeSpan.Zero;
        private TimeSpan transitionOffTime = TimeSpan.Zero;
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
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        /// <summary>
        /// How long (in seconds) the transition off effect will be
        /// </summary>
        public TimeSpan TransitionOffTime
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
            get { return (byte)(255 - TransitionPercentage * 255); }
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
        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public virtual void LoadContent() { }


        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent() { }
        #endregion

        #region Private methods
        /// <summary>
        /// Helper for updating the screen transition
        /// </summary>
        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            transitionPercentage += transitionDelta * direction;

            // Did we reach the end of the transition?
            if ((transitionPercentage <= 0) || (transitionPercentage >= 1))
            {
                transitionPercentage = MathHelper.Clamp(transitionPercentage, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public virtual void HandleInput(InputManager input) { }
        #endregion

        #region Draw / Update
        /// <summary>
        /// Allows the screen to update itself.
        /// </summary>
        /// <param name="gameTime">The current gametime</param>
        /// <param name="otherScreenHasFocus">If another screen has focus or not</param>
        /// <param name="coveredByOtherScreen">If we are covered by another screen</param>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus,
                                              bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (isExiting)
            {
                // If the screen is going away to die, it should transition off.
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    ScreenManager.RemoveScreen(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // Still busy transitioning.
                    screenState = ScreenState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenState.Hidden;
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    // Still busy transitioning.
                    screenState = ScreenState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenState.Active;
                }
            }
        }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime) { }
        #endregion
    }
}
