#region File Description
#endregion

#region Using Statements
// System
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
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
using Duologue.Audio;
using Duologue.State;
using Duologue.Screens;
#endregion

namespace Duologue.PlayObjects
{
    public class Enemy_Roggles : Enemy
    {
        public enum RogglesState
        {
            Walking,
            Running,
        }
        #region Constants
        private const string filename_base = "Enemies/roller-0{0}-color"; // FIXME, silliness
        private const string filename_outline = "Enemies/roller-0{0}-face"; // FIXME, silliness

        private const float maxShadowOffset = 10f;

        private const int numberOfWalkingFrames = 6;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 50;

        /// <summary>
        /// The multiplier for point value tweaks based upon hitpoints
        /// </summary>
        private const int hitPointMultiplier = 2;

        /// <summary>
        /// The time per frame while we're walking
        /// </summary>
        private const double timePerFrameWalking = 0.15;

        /// <summary>
        /// The time per frame while we're running
        /// </summary>
        private const double timePerFrameRunning = 0.05;

        /// <summary>
        /// Our speed when we're just randomly walking around
        /// </summary>
        private const double minWalkingSpeed = 1.4;

        private const double maxWalkingSpeed = 1.8;

        /// <summary>
        /// The acceleration of the rotation
        /// </summary>
        private const float minRotationAccel = 0.001f;
        private const float maxRotationAccel = 0.009f;

        private const double timePerRotationChange = 1.5;

        /// <summary>
        /// The speed we move when there's no players and we're just trying to get off the screen
        /// </summary>
        private const float egressSpeed = 2.3f;

        /// <summary>
        /// How far we can go outside the screen before we should stop
        /// </summary>
        private const float outsideScreenMultiplier = 1.1f;

        /// <summary>
        /// The radius multiplier for determining radius from size
        /// </summary>
        private const float radiusMultiplier = 0.2f;

        /// <summary>
        /// The minimum distance a player needs to be before I notice them
        /// </summary>
        private const float minPlayerDistanceMultiplier = 5f;

        #region Forces
        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        /// <summary>
        /// The player attract modifier
        /// </summary>
        private const float playerAttract = 2.5f;

        /// <summary>
        /// The player attract modifier for when we're accelerated
        /// </summary>
        private const float playerAttractAccel = 4f;
        #endregion

        /// <summary>
        /// The minimum movement required before we register motion
        /// </summary>
        private const float minMovement = 1.25f;
        #endregion

        #region Fields
        private Texture2D[] baseFrames;
        private Texture2D[] outlineFrames;
        private Vector2[] walkingCenters;
        private int currentFrame;
        private Vector2 shadowOffset;
        private Color shadowColor;

        private Vector2 screenCenter;

        private int playersDetected;

        private float rotation;

        private float baseLayer;
        private float outlineLayer;

        private double timeSinceStart;
        //private double currentTimePerFrameDying;

        private double rotationChangeTimer;

        // Movement
        private Vector2 offset;
        private Vector2 playerOffset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;
        private Player nearestPlayerObject;
        private Vector2 lastDirection;
        private float walkingSpeed;
        private bool startedMoving;
        private int rotationAccelSign;
        private float rotationAccel;

        private bool isFleeing;
        private bool inBeam;

        private AudioManager audio;
        #endregion

        #region Properties
        /// <summary>
        /// The current state for the animation of Mr. Wiggles
        /// </summary>
        public RogglesState CurrentState;
        #endregion

        #region Constructor / Init
        /// <summary>
        /// The empty constructor for pre-caching
        /// </summary>
        public Enemy_Roggles() : base() { }

        public Enemy_Roggles(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Roggles;
            MajorType = MajorPlayObjectType.Enemy;
            baseLayer = 0f;
            outlineLayer = 0f;
            RealSize = new Vector2(82, 90);
            startedMoving = false;
            Initialized = false;
            Alive = false;
            OffscreenArrow = true;
            audio = ServiceLocator.GetService<AudioManager>();
        }

        public override void Initialize(
            Vector2 startPos,
            Vector2 startOrientation,
            ColorState currentColorState,
            ColorPolarity startColorPolarity,
            int? hitPoints,
            double spawnDelay)
        {
            Position = startPos;
            Orientation = startOrientation;
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            SpawnTimeDelay = spawnDelay;
            SpawnTimer = 0;
            rotation = MWMathHelper.ComputeAngleAgainstX(Orientation);
            if (hitPoints == null)
            {
                hitPoints = 0;
            }
            StartHitPoints = (int)hitPoints;
            CurrentHitPoints = (int)hitPoints;

            if (Orientation == Vector2.Zero)
            {
                // Just aim at the center of screen for now
                Orientation = GetStartingVector();
            }

            walkingSpeed = (float)MWMathHelper.GetRandomInRange(minWalkingSpeed, maxWalkingSpeed);
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            // Textures
            baseFrames = new Texture2D[numberOfWalkingFrames];
            outlineFrames = new Texture2D[numberOfWalkingFrames];
            walkingCenters = new Vector2[numberOfWalkingFrames];

            // load the base frames
            for (int i = 1; i <= numberOfWalkingFrames; i++)
            {
                baseFrames[i - 1] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_base, i.ToString()));
                outlineFrames[i - 1] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_outline, i.ToString()));

                walkingCenters[i - 1] = new Vector2(
                    baseFrames[i - 1].Width / 2f,
                    baseFrames[i - 1].Height / 2f);
            }

            Radius = RealSize.Length() * radiusMultiplier;

            // We want a random starting frame, otherwise everyone will look "the same"
            currentFrame = MWMathHelper.GetRandomInRange(0, numberOfWalkingFrames);

            CurrentState = RogglesState.Walking;

            // Handle the rotation accel
            rotationAccel = (float)MWMathHelper.GetRandomInRange(minRotationAccel, maxRotationAccel);
            rotationAccelSign = -1;
            if (MWMathHelper.GetRandomInRange(0, 2) == 1)
                rotationAccelSign = 1;

            rotationChangeTimer = MWMathHelper.GetRandomInRange(0.0, timePerRotationChange);

            timeSinceStart = 0;
            rotationChangeTimer = 0;

            shadowColor = new Color(Color.Black, 200);

            Initialized = true;
            Alive = true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Returns a vector pointing to the origin
        /// </summary>
        private Vector2 GetVectorPointingAtOrigin()
        {
            Vector2 sc = new Vector2(
                    InstanceManager.DefaultViewport.Width / 2f,
                    InstanceManager.DefaultViewport.Height / 2f);
            return sc - Position;
        }

        /// <summary>
        /// Get a starting vector for this dude
        /// </summary>
        private Vector2 GetStartingVector()
        {
            // Just aim at the center of screen for now
            Vector2 temp = GetVectorPointingAtOrigin() + new Vector2(
                (float)MWMathHelper.GetRandomInRange(-.5, .5),
                (float)MWMathHelper.GetRandomInRange(-.5, .5));
            temp.Normalize();
            return temp;
        }

        /// <summary>
        /// Figure out the current treadoffset
        /// </summary>
        private void ComputeShadowOffset()
        {
            if (screenCenter == Vector2.Zero)
            {
                screenCenter = new Vector2(
                    InstanceManager.GraphicsDevice.Viewport.Width / 2f,
                    InstanceManager.GraphicsDevice.Viewport.Height / 2f);
                InstanceManager.Logger.LogEntry("Rare enemy spawn: Roggles- go get 'em, ya cheater!");
            }

            // Get distance
            float distance = Vector2.Subtract(screenCenter, Position).Length();

            // Compute the size of the offset based on distance
            float size = maxShadowOffset * (distance / screenCenter.Length());

            // Aim at center of screen
            shadowOffset = Vector2.Add(screenCenter, Position);
            shadowOffset.Normalize();
            shadowOffset *= size;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            Color c = ColorState.Negative[ColorState.Light];
            if (ColorPolarity == ColorPolarity.Positive)
                c = ColorState.Positive[ColorState.Light];

            Color outlineC = Color.White;
            Color shadC = shadowColor;
            float scale = 1f;
            SpriteEffects se = SpriteEffects.None;

            rotation = MWMathHelper.ComputeAngleAgainstX(Orientation);
            if (rotation > MathHelper.PiOver2 && rotation < MathHelper.Pi + MathHelper.PiOver2)
            {
                se = SpriteEffects.FlipHorizontally;
            }
            Texture2D baseImg = baseFrames[currentFrame];
            Texture2D outlineImg = outlineFrames[currentFrame];
            Vector2 cent = walkingCenters[currentFrame];

            // Draw shadow
            InstanceManager.RenderSprite.Draw(
                baseImg,
                Position + shadowOffset,
                cent,
                null,
                shadC,
                0f,
                scale,
                baseLayer,
                RenderSpriteBlendMode.AlphaBlend,
                se);

            // Draw base
            InstanceManager.RenderSprite.Draw(
                baseImg,
                Position,
                cent,
                null,
                c,
                0f,
                scale,
                baseLayer,
                RenderSpriteBlendMode.AlphaBlend,
                se);

            // Draw Outline
            InstanceManager.RenderSprite.Draw(
                outlineImg,
                Position,
                walkingCenters[currentFrame],
                null,
                outlineC,
                0f,
                1f,
                outlineLayer,
                RenderSpriteBlendMode.AlphaBlend,
                se);

        }

        public override void Update(GameTime gameTime)
        {
            timeSinceStart += gameTime.ElapsedGameTime.TotalSeconds;
            rotationChangeTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (rotationChangeTimer > timePerRotationChange)
            {
                rotationAccelSign *= -1;
                rotationChangeTimer = 0;
            }

            switch (CurrentState)
            {
                case RogglesState.Walking:
                    if (timeSinceStart > timePerFrameWalking)
                    {
                        currentFrame++;
                        timeSinceStart = 0;
                        if (currentFrame >= numberOfWalkingFrames)
                            currentFrame = 0;
                    }
                    break;
                default:
                    if (timeSinceStart > timePerFrameRunning)
                    {
                        currentFrame++;
                        timeSinceStart = 0;
                        if (currentFrame >= numberOfWalkingFrames)
                            currentFrame = 0;
                    }
                    break;
            }

            ComputeShadowOffset();
        }
        #endregion

        #region Public overrides
        public override String[] GetTextureFilenames()
        {
            String[] filenames = new String[2 * numberOfWalkingFrames];

            int t = 0;
            for (int i = 0; i < numberOfWalkingFrames; i++)
            {
                filenames[t] = String.Format(filename_base, (i + 1).ToString());
                t++;
                filenames[t] = String.Format(filename_outline, (i + 1).ToString());
                t++;
            }

            return filenames;
        }

        public override bool StartOffset()
        {
            offset = Orientation * walkingSpeed;
            if (CurrentState == RogglesState.Running)
                offset = Vector2.Zero;
            nearestPlayerRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            nearestPlayer = Vector2.Zero;
            playersDetected = 0;
            playerOffset = Vector2.Zero;
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Player)
            {
                playersDetected++;
                startedMoving = true;
                // Player
                Vector2 vToPlayer = this.Position - pobj.Position;
                float len = vToPlayer.Length();

                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em
                    return pobj.TriggerHit(this);
                }

                // Figure out if we need to attack them
                if (len < nearestPlayerRadius)
                {
                    nearestPlayerRadius = len;
                    nearestPlayer = vToPlayer;
                    nearestPlayerObject = (Player)pobj;
                }

                // Beam handling
                int temp = ((Player)pobj).IsInBeam(this);
                inBeam = false;
                isFleeing = false;
                if (temp != 0)
                {
                    inBeam = true;
                    if (temp == -1)
                    {
                        isFleeing = true;
                        Color c = ColorState.Negative[ColorState.Light];
                        if (ColorPolarity == ColorPolarity.Positive)
                            c = ColorState.Positive[ColorState.Light];
                        LocalInstanceManager.Steam.AddParticles(Position, c);
                    }
                }
            }
            else if (pobj.MajorType == MajorPlayObjectType.Enemy)
            {
                // Enemy
                Vector2 vToEnemy = pobj.Position - this.Position;
                float len = vToEnemy.Length();
                if (len < this.Radius + pobj.Radius)
                {
                    // Too close, BTFO
                    if (len == 0f)
                    {
                        // Well, bah, we're on top of each other!
                        vToEnemy = new Vector2(
                            (float)InstanceManager.Random.NextDouble() - 0.5f,
                            (float)InstanceManager.Random.NextDouble() - 0.5f);
                    }
                    vToEnemy = Vector2.Negate(vToEnemy);
                    vToEnemy.Normalize();
                    offset += standardEnemyRepulse * vToEnemy;
                }
                return true;
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            // First, no motion if no players have ever been detected
            if (playersDetected < 1 && !startedMoving)
            {
                Vector2 temp = Vector2.Negate(GetVectorPointingAtOrigin());
                temp.Normalize();
                offset += temp * egressSpeed;
            }
            else
            {

                // Next do any player offset
                if (nearestPlayerRadius < minPlayerDistanceMultiplier * (this.Radius + nearestPlayerObject.Radius))
                {
                    float modifier = playerAttract;
                    if (inBeam)
                        modifier = playerAttractAccel;

                    nearestPlayer.Normalize();

                    if (!isFleeing)
                        nearestPlayer = Vector2.Negate(nearestPlayer);

                    offset += modifier * nearestPlayer;
                    CurrentState = RogglesState.Running;
                }
                else
                {
                    CurrentState = RogglesState.Walking;
                }
            }

            // Next apply the offset permanently
            if (offset.Length() >= minMovement)
            {
                this.Position += offset;
                offset.Normalize();
                lastDirection = offset;
                Orientation = MWMathHelper.RotateVectorByRadians(offset, rotationAccel * rotationAccelSign);
            }

            // Check boundaries
            if (this.Position.X < -1 * RealSize.X * outsideScreenMultiplier)
            {
                this.Position.X = -1 * RealSize.X * outsideScreenMultiplier;
                Orientation = GetStartingVector();
            }
            else if (this.Position.X > InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier)
            {
                this.Position.X = InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier;
                Orientation = GetStartingVector();
            }

            if (this.Position.Y < -1 * RealSize.Y * outsideScreenMultiplier)
            {
                this.Position.Y = -1 * RealSize.Y * outsideScreenMultiplier;
                Orientation = GetStartingVector();
            }
            else if (this.Position.Y > InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier)
            {
                this.Position.Y = InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier;
                Orientation = GetStartingVector();
            }

            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.PlayerBullet)
            {
                Color c = ColorState.Negative[ColorState.Light];
                if (ColorPolarity == ColorPolarity.Positive)
                    c = ColorState.Positive[ColorState.Light];

                CurrentHitPoints--;
                if (CurrentHitPoints <= 0)
                {
                    currentFrame = 0;
                    timeSinceStart = 0;
                    Alive = false;
                    LocalInstanceManager.AchievementManager.EnemyDeathCount(MyType);
                    InstanceManager.Logger.LogEntry("Roggles dead");
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue + hitPointMultiplier * StartHitPoints, Position);
                    audio.PlayEffect(EffectID.WigglesDeath);
                    LocalInstanceManager.EnemySplatterSystem.AddParticles(Position, c);
                    return false;
                }
                else
                {
                    audio.PlayEffect(EffectID.Sploosh);
                    TriggerShieldDisintegration(outlineFrames[currentFrame], c, Position, 0f);
                    return true;
                }
            }
            return true;
        }
        #endregion
    }
}
