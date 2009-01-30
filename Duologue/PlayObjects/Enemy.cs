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
        private const float bulletLifetime = 0.05f;
        private const byte bulletAlpha = 200;

        private const float shieldLifetime = 0.7f;
        private const double minShieldRotationDelta = 0.04;
        private const double maxShieldRotationDelta = 0.08;
        private const double minShieldSpeed = 0.1;
        private const double maxShieldSpeed = 0.5;
        private const double minEndShieldSizeMultiplier = 0.2;
        private const double maxEndShieldSizeMultiplier = 0.7;
        private const byte maxShieldAlpha = 255;
        #endregion

        #region Fields
        // Bullet stuff
        private Texture2D[] bulletFrames;
        private Vector2 bulletPos;
        private Vector2 bulletCenter;
        private Color bulletColor;
        private float bulletTimer;
        private int currentFrame;
        private float bulletRotation;

        // Shield stuff
        private Texture2D shield;
        private Vector2 shieldCenter;
        private Vector2 shieldPos;
        private Color shieldColor;
        private float shieldTimer;
        private float shieldRotation;
        private float shieldRotationDelta;
        private Vector2 shieldDirection;
        private float shieldSpeed;
        private float shieldSize;
        private float shieldEndSize;
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

        /// <summary>
        /// The real size of this enemy
        /// </summary>
        public Vector2 RealSize;
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
            for (int i = 1; i <= maxNumBulletFrames; i++)
            {
                bulletFrames[i-1] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_bulletHit, i.ToString()));
            }
            bulletTimer = 0f;
            shieldTimer = shieldLifetime;
            shieldRotation = 0f;
            bulletCenter = new Vector2(
                bulletFrames[0].Width / 2f,
                bulletFrames[0].Height / 2f);
            currentFrame = maxNumBulletFrames;
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

        #region Public methods
        /// <summary>
        /// Call when we wish to trigger a bullet explosion
        /// </summary>
        public void TriggerBulletExplosion(Vector2 pos, Color c)
        {
            bulletPos = pos;
            bulletColor = new Color(c, bulletAlpha);
            currentFrame = 0;
            bulletTimer = 0f;
            bulletRotation = (float)MWMathHelper.GetRandomInRange(MathHelper.PiOver2, MathHelper.TwoPi);
        }

        /// <summary>
        /// Call when we wish to trigger the disintegration of an enemy's shield
        /// </summary>
        public void TriggerShieldDisintegration(Texture2D t, Color c, Vector2 pos, float startRotation)
        {
            shield = t;
            shieldColor = c;
            shieldPos = pos;
            shieldRotation = startRotation;
            shieldCenter = new Vector2(shield.Width / 2f, shield.Height / 2f);
            shieldTimer = 0f;
            shieldRotationDelta = (float)MWMathHelper.GetRandomInRange(
                minShieldRotationDelta,
                maxShieldRotationDelta);
            shieldDirection = new Vector2(
                (float)MWMathHelper.GetRandomInRange(-1, 1),
                (float)MWMathHelper.GetRandomInRange(-1, 1));
            shieldSpeed = (float)MWMathHelper.GetRandomInRange(
                minShieldSpeed, maxShieldSpeed);
            shieldEndSize = (float)MWMathHelper.GetRandomInRange(
                minEndShieldSizeMultiplier, maxEndShieldSizeMultiplier);
        }
        #endregion

        #region Inner Update
        public void InnerUpdate(GameTime gameTime)
        {
            // Update the bullet explosion
            if (currentFrame < maxNumBulletFrames)
            {
                bulletTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (bulletTimer > bulletLifetime)
                {
                    currentFrame++;
                    bulletTimer = 0f;
                }
            }

            // Update the shield
            if (shieldTimer < shieldLifetime)
            {
                shieldTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                shieldRotation += shieldRotationDelta;
                shieldSize = 1f + (shieldTimer / shieldLifetime) * shieldEndSize;
                shieldPos += shieldSpeed * shieldDirection;
            }
        }

        public void InnerDraw(GameTime gameTime)
        {
            // Bullet
            if(currentFrame < maxNumBulletFrames)
                InstanceManager.RenderSprite.Draw(
                    bulletFrames[currentFrame],
                    bulletPos,
                    bulletCenter,
                    null,
                    bulletColor,
                    bulletRotation,
                    1f,
                    1f,
                    RenderSpriteBlendMode.AlphaBlendTop);

            // Shield
            if (shieldTimer < shieldLifetime)
            {
                float p = shieldTimer / shieldLifetime;
                Color c = new Color(
                    shieldColor,
                    (byte)(maxShieldAlpha - p * maxShieldAlpha));
                InstanceManager.RenderSprite.Draw(
                    shield,
                    shieldPos,
                    shieldCenter,
                    null,
                    c,
                    shieldRotation,
                    shieldSize,
                    1f,
                    RenderSpriteBlendMode.AlphaBlendTop);
            }
        }

        public Color GetMyColor()
        {
            Color c = ColorState.Negative[ColorState.Light];
            if(ColorPolarity == ColorPolarity.Positive)
                c = ColorState.Positive[ColorState.Light];

            return c;
        }
        #endregion
    }
}
