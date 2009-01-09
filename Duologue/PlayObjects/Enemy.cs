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
using Duologue.Screens;
#endregion

namespace Duologue.PlayObjects
{
    public abstract class Enemy : PlayObject
    {
        #region Constants
        private const string filename_bulletHit = "bullet-hit-0{0}"; // FIXME, silliness
        private const int maxNumBulletFrames = 6;
        private const float bulletLifetime = 1f;
        private const float shieldLifetime = 1.5f;
        #endregion

        #region Fields
        // Bullet stuff
        private Texture2D[] bulletFrames;
        private Vector2 bulletPos;
        private Color bulletColor;
        private float bulletTimer;

        // Shield stuff
        private Texture2D shield;
        private Vector2 shieldPos;
        private Color shieldColor;
        private float shieldTimer;
        #endregion

        #region Properties
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

        /// <summary>
        /// The starting hit points we had when we spawned
        /// </summary>
        public int StartHitPoints;

        /// <summary>
        /// The GamePlay Screen Manager parent instance
        /// </summary>
        public GamePlayScreenManager MyManager;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a new Enemy instance (abstract class)
        /// </summary>
        /// <param name="manager">The parent GamePlay Screen manager</param>
        public Enemy(GamePlayScreenManager manager)
            : base()
        {
            MyManager = manager;
            bulletFrames = new Texture2D[maxNumBulletFrames];
            for (int i = 0; i < maxNumBulletFrames; i++)
            {
                bulletFrames[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_bulletHit, i.ToString()));
            }
            bulletTimer = bulletLifetime;
            shieldTimer = shieldLifetime;
        }
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

        #region Inner Update
        public void InnerUpdate(GameTime gameTime)
        {
        }

        public void InnerDraw(GameTime gameTime)
        {
        }
        #endregion
    }
}
