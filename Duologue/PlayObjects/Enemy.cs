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
    public enum EnemyType
    {
        Standard,
        Leader,
        Follower
    }

    public abstract class Enemy : PlayObject
    {
        #region Constants
        private const string filename_bulletHit = "bullet-hit-0{0}";
        private const string filename_OffScreenArrowMeat = "Enemies/offscreen-arrow-meat";
        private const string filename_OffScreenArrowOutline = "Enemies/offscreen-arrow-outline";
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
        private const double timeBetweenColorSwitched = 0.15;
        private const float arrowPositionPercentage = 0.90f;
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

        // Arrow stuff
        private Texture2D texture_ArrowOutline;
        private Texture2D texture_ArrowMeat;
        private Vector2 center_Arrow;
        private Vector2 pos_Arrow;
        private float rotation_Arrow;
        private float scale_Arrow;
        private bool displayArrow;
        private Vector4 arrowScreenBoundaries;
        private bool hasSpawned;
        private Color[] color_Arrow;
        private int currentArrowColor;
        private double timer_ColorSwitch;
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

        /// <summary>
        /// The type of the enemy. Defaults to standard
        /// </summary>
        public EnemyType MyEnemyType = EnemyType.Standard;

        /// <summary>
        /// Set this to true if we want to use offscreen arrows
        /// </summary>
        public bool OffscreenArrow = false;

        /// <summary>
        /// The time delay before this enemy spawns
        /// </summary>
        public double SpawnTimeDelay = 0;

        /// <summary>
        /// A convenience timer for the derived objects
        /// </summary>
        public double SpawnTimer = 0;

        /// <summary>
        /// True if the spawn timer has elapsed
        /// </summary>
        public bool SpawnTimerElapsed
        {
            get { return SpawnTimer >= SpawnTimeDelay; }
        }
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

            pos_Arrow = Vector2.Zero;
            texture_ArrowMeat = InstanceManager.AssetManager.LoadTexture2D(
                filename_OffScreenArrowMeat);
            texture_ArrowOutline = InstanceManager.AssetManager.LoadTexture2D(
                filename_OffScreenArrowOutline);
            center_Arrow = new Vector2(
                texture_ArrowMeat.Width / 2f, texture_ArrowMeat.Height / 2f);
            displayArrow = false;
            scale_Arrow = 0;
            hasSpawned = false;

            currentArrowColor = -1;
            timer_ColorSwitch = 0;

            arrowScreenBoundaries = new Vector4(
                (float)(InstanceManager.DefaultViewport.Width - InstanceManager.DefaultViewport.Width * arrowPositionPercentage),
                (float)(InstanceManager.DefaultViewport.Height - InstanceManager.DefaultViewport.Height * arrowPositionPercentage),
                (float)(InstanceManager.DefaultViewport.Width * arrowPositionPercentage),
                (float)(InstanceManager.DefaultViewport.Height * arrowPositionPercentage));
        }

        /// <summary>
        /// The empty constructor for pre-caching
        /// </summary>
        public Enemy()
            : base()
        {
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
            int? hitPoints,
            double spawnDelay);
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

        /// <summary>
        /// Call this to clear any inner triggers such as bullet explosions and shield disintigrations
        /// </summary>
        public void ClearInnerTriggers()
        {
            currentFrame = maxNumBulletFrames;
            shieldTimer = shieldLifetime;
        }
        #endregion

        #region Inner Update / Draw
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

            // Arrow
            if (currentArrowColor < 0)
            {
                currentArrowColor = 0;
                color_Arrow = new Color[]
                {
                    Color.TransparentBlack,
                    GetMyColor(ColorState.Light),
                    GetMyColor(ColorState.Medium),
                    GetMyColor(ColorState.Dark)
                };
                timer_ColorSwitch = 0;
            }

            if (OffscreenArrow)
            {
                timer_ColorSwitch += gameTime.ElapsedGameTime.TotalSeconds;
                if (timer_ColorSwitch > timeBetweenColorSwitched)
                {
                    currentArrowColor++;
                    timer_ColorSwitch = 0;
                    if (currentArrowColor >= color_Arrow.Length)
                        currentArrowColor = 0;
                }

                pos_Arrow = Position;
                displayArrow = false;
                rotation_Arrow = 0;

                if (Position.X < -Radius && hasSpawned)
                {
                    displayArrow = true;
                    rotation_Arrow = 3f * MathHelper.PiOver2;
                    pos_Arrow.X = arrowScreenBoundaries.X;
                }
                else if (Position.X > InstanceManager.DefaultViewport.Width + Radius && hasSpawned)
                {
                    displayArrow = true;
                    rotation_Arrow = -3f * MathHelper.PiOver2;
                    pos_Arrow.X = arrowScreenBoundaries.Z;
                }

                if (Position.Y < -Radius && hasSpawned)
                {
                    displayArrow = true;
                    pos_Arrow.Y = arrowScreenBoundaries.Y;
                }
                else if (Position.Y > InstanceManager.DefaultViewport.Height + Radius && hasSpawned)
                {
                    displayArrow = true;
                    rotation_Arrow = MathHelper.Pi;
                    pos_Arrow.Y = arrowScreenBoundaries.W;
                }

                if (!hasSpawned &&
                    (Position.X > Radius && Position.X < InstanceManager.DefaultViewport.Width + Radius &&
                    Position.Y > Radius && Position.Y < InstanceManager.DefaultViewport.Height + Radius))
                    hasSpawned = true;
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

            // Arrow
            if (displayArrow)
            {
                if (scale_Arrow == 0)
                    scale_Arrow = RealSize.X / texture_ArrowMeat.Width;

                InstanceManager.RenderSprite.Draw(
                    texture_ArrowOutline,
                    pos_Arrow,
                    center_Arrow,
                    null,
                    Color.White,
                    rotation_Arrow,
                    scale_Arrow,
                    0f,
                    RenderSpriteBlendMode.AbsoluteTop);
                InstanceManager.RenderSprite.Draw(
                    texture_ArrowMeat,
                    pos_Arrow,
                    center_Arrow,
                    null,
                    color_Arrow[currentArrowColor],
                    rotation_Arrow,
                    scale_Arrow,
                    0f,
                    RenderSpriteBlendMode.AbsoluteTop);
            }
        }

        public Color GetMyColor()
        {
            /*Color c = ColorState.Negative[ColorState.Light];
            if(ColorPolarity == ColorPolarity.Positive)
                c = ColorState.Positive[ColorState.Light];*/

            return GetMyColor(ColorState.Light);
        }

        public Color GetMyColor(int ColorStateLevel)
        {
            // FIXME, there's no error checking here
            // We'd get a nasty crash if a caler wanted
            // a color state level outside of the range
            Color c = ColorState.Negative[ColorStateLevel];
            if (ColorPolarity == ColorPolarity.Positive)
                c = ColorState.Positive[ColorStateLevel];

            return c;
        }

        public Color GetMyColor(int ColorStateLevel, ColorPolarity Polarity)
        {
            // FIXME, same as above
            Color c = ColorState.Negative[ColorStateLevel];
            if (Polarity == ColorPolarity.Positive)
                c = ColorState.Positive[ColorStateLevel];

            return c;
        }
        #endregion
    }
}
