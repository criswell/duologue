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
#endregion

namespace Duologue.Screens
{
    public enum FadeState
    {
        In,
        Out,
        Selected,
        None,
    }
    public class MenuItem
    {
        #region Constants
        /// <summary>
        /// The time it takes for us to fade in/out
        /// </summary>
        public const float Fadetime = 0.5f;
        #endregion

        #region Fields
        private FadeState fadeState;
        private float timeSinceStart;
        private bool lastSelected;
        #endregion

        #region Properties
        /// <summary>
        /// The text this menu item displays
        /// </summary>
        public string Text;

        /// <summary>
        /// Returns the current fade state
        /// </summary>
        public FadeState FadeState
        {
            get { return fadeState; }
        }

        /// <summary>
        /// Returns the current fade percentage between 0.0 and 1.0
        /// </summary>
        public float FadePercent
        {
            get 
            {
                if (fadeState == FadeState.Selected)
                    return 1f;
                else if (fadeState == FadeState.None)
                    return 0f;
                else if (fadeState == FadeState.In)
                    return MathHelper.Min(timeSinceStart / Fadetime, 1f);
                else
                    return 1f - MathHelper.Min(timeSinceStart / Fadetime, 1f);
            }
        }

        /// <summary>
        /// Set this when the item is selected, false if not
        /// </summary>
        public bool Selected;

        /// <summary>
        /// Set this if you want the menu item grayed out
        /// </summary>
        public bool Invisible;
        #endregion

        #region Constructor / Init
        /// <summary>
        /// Construct a menu item
        /// </summary>
        /// <param name="text">The text of this menu item</param>
        public MenuItem(string text)
        {
            Text = text;
            Initialize();
        }

        private void Initialize()
        {
            fadeState = FadeState.None;
            timeSinceStart = 0f;
            lastSelected = false;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Update / Draw
        /// <summary>
        /// Called once per frame to update our state
        /// </summary>
        /// <param name="dt">Delta time since last call</param>
        public void Update(float dt)
        {
            if (fadeState == FadeState.In)
            {
                timeSinceStart += dt;
                if (timeSinceStart > Fadetime)
                    if (Selected)
                        fadeState = FadeState.Selected;
                    else
                    {
                        fadeState = FadeState.Out;
                        timeSinceStart = 0f;
                    }
            }
            else if (fadeState == FadeState.Out)
            {
                timeSinceStart += dt;
                if (timeSinceStart > Fadetime)
                    if (Selected)
                    {
                        fadeState = FadeState.In;
                        timeSinceStart = 0f;
                    }
                    else
                        fadeState = FadeState.None;
            }

            if (Selected && !lastSelected && fadeState != FadeState.In)
            {
                timeSinceStart = 0f;
                fadeState = FadeState.In;
            }

            if (!Selected && lastSelected && fadeState != FadeState.Out)
            {
                timeSinceStart = 0f;
                fadeState = FadeState.Out;
            }
            lastSelected = Selected;
        }
        #endregion
    }
}
