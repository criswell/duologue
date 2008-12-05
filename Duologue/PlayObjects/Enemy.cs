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
using Mimicware;
using Mimicware.Graphics;
using Mimicware.Manager;
// Duologue
using Duologue.State;
#endregion

namespace Duologue.PlayObjects
{
    public abstract class Enemy : PlayObject
    {
        #region Properties
        /// <summary>
        /// Set if this enemy object has been initialized
        /// </summary>
        public bool Initialized;

        /// <summary>
        /// The orientation of this enemy
        /// </summary>
        public Vector2 Orientation;

        /// <summary>
        /// Current colorstate
        /// </summary>
        public ColorState ColorState;

        /// <summary>
        /// This enemy's current color polarity
        /// </summary>
        public ColorPolarity ColorPolarity;

        /// <summary>
        /// How many hit points we currently have
        /// </summary>
        public int CurrentHitPoints;
        #endregion

        #region Load / Init
        /// <summary>
        /// Initialize this enemy
        /// </summary>
        /// <param name="startPos">Enemy's starting position</param>
        /// <param name="startOrientation">Enemy's starting orientation</param>
        /// <param name="currentColorState">Enemy's color state</param>
        /// <param name="startColorPolarity">Enemy's starting color polarity</param>
        public abstract void Initialize(
            Vector2 startPos,
            Vector2 startOrientation,
            ColorState currentColorState,
            ColorPolarity startColorPolarity,
            int? hitPoints);
        #endregion
    }
}
