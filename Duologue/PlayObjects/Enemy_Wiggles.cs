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
    public class Enemy_Wiggles : Enemy
    {
        public enum WigglesState
        {
            Walking,
            Running,
            Dying
        }
        #region Constants
        private const string filename_base = "Enemies/wiggles/0{0}-base"; // FIXME, silliness
        private const string filename_outline = "Enemies/wiggles/0{0}-outline"; // FIXME, silliness
        private const string filename_invertOutline = "Enemies/wiggles/0{0}-invert-outline"; // Bah, who cares

        private const int numberOfWalkingFrames = 8;

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
        /// How far we can go outside the screen before we should stop
        /// </summary>
        private const float outsideScreenMultiplier = 3;

        /// <summary>
        /// The radius multiplier for determining radius from size
        /// </summary>
        private const float radiusMultiplier = 0.2f;

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
        private Texture2D[] invertOutlineFrames;
        private Vector2[] walkingCenters;
        private int currentFrame;

        private int playersDetected;

        private float rotation;

        private float baseLayer;
        private float outlineLayer;

        private double timeSinceStart;

        // Movement
        private Vector2 offset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;
        private Vector2 lastDirection;
        private float walkingSpeed;
        #endregion

        #region Properties
        /// <summary>
        /// The current state for the animation of Mr. Wiggles
        /// </summary>
        public WigglesState CurrentState;
        #endregion

        #region Constructor / Init
        public Enemy_Wiggles(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Wiggles;
            MajorType = MajorPlayObjectType.Enemy;
            baseLayer = LocalInstanceManager.BlitLayer_EnemyBase;
            outlineLayer = LocalInstanceManager.BlitLayer_EnemyBase - 0.1f;
            RealSize = new Vector2(82, 90);
            Initialized = false;
            Alive = false;
        }

        public override void Initialize(
            Vector2 startPos,
            Vector2 startOrientation,
            ColorState currentColorState, 
            ColorPolarity startColorPolarity, 
            int? hitPoints)
        {
            Position = startPos;
            Orientation = startOrientation;
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
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
                Orientation = GetVectorPointingAtOrigin() +  new Vector2(
                    (float)MWMathHelper.GetRandomInRange(-.5,.5),
                    (float)MWMathHelper.GetRandomInRange(-.5,.5));
                Orientation.Normalize();
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
            invertOutlineFrames = new Texture2D[numberOfWalkingFrames];

            // load the base frames
            for (int i = 1; i <= numberOfWalkingFrames; i++)
            {
                baseFrames[i - 1] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_base, i.ToString()));
                outlineFrames[i - 1] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_outline, i.ToString()));
                invertOutlineFrames[i - 1] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_invertOutline, i.ToString()));

                walkingCenters[i - 1] = new Vector2(
                    baseFrames[i - 1].Width / 2f,
                    baseFrames[i - 1].Height / 2f);
            }

            Radius = RealSize.Length() * radiusMultiplier;

            // load the death frames FIXME TODO

            // We want a random starting frame, otherwise everyone will look "the same"
            currentFrame = MWMathHelper.GetRandomInRange(0, numberOfWalkingFrames);

            CurrentState = WigglesState.Walking;

            timeSinceStart = 0;

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
        #endregion

        #region Public Methods
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            Color c = ColorState.Negative[ColorState.Light];
            if(ColorPolarity == ColorPolarity.Positive)
                c = ColorState.Positive[ColorState.Light];

            rotation = MWMathHelper.ComputeAngleAgainstX(Orientation) + MathHelper.Pi + MathHelper.PiOver2;

            // Draw base
            InstanceManager.RenderSprite.Draw(
                baseFrames[currentFrame],
                Position,
                walkingCenters[currentFrame],
                null,
                c,
                rotation,
                1f,
                baseLayer);

            // Draw Outline
            InstanceManager.RenderSprite.Draw(
                outlineFrames[currentFrame],
                Position,
                walkingCenters[currentFrame],
                null,
                Color.White,
                rotation,// + MathHelper.PiOver2,
                1f,
                outlineLayer);
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceStart += gameTime.ElapsedGameTime.TotalSeconds;

            //Orientation.Normalize();

            switch(CurrentState)
            {
                case WigglesState.Walking:
                    if (timeSinceStart > timePerFrameWalking)
                    {
                        currentFrame++;
                        timeSinceStart = 0;
                    }
                    break;
                case WigglesState.Running:
                    if (timeSinceStart > timePerFrameRunning)
                    {
                        currentFrame++;
                        timeSinceStart = 0;
                    }
                    break;
                default:
                    // We're dying
                    break;
            }
            if (currentFrame >= numberOfWalkingFrames)
                currentFrame = 0;
        }
        #endregion

        #region Public overrides
        public override bool StartOffset()
        {
            if (CurrentState != WigglesState.Dying)
            {
                offset = Orientation * walkingSpeed;
                if (CurrentState == WigglesState.Running)
                    offset = Vector2.Zero;
                nearestPlayerRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
                nearestPlayer = Vector2.Zero;
            }
            playersDetected = 0;
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Player)
            {
                playersDetected++;
                // Player
                Vector2 vToPlayer = this.Position - pobj.Position;
                float len = vToPlayer.Length();
                if (len < nearestPlayerRadius)
                {
                    nearestPlayerRadius = len;
                    nearestPlayer = vToPlayer;
                }
                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em
                    return pobj.TriggerHit(this);
                }

                /*
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
                        if(ColorPolarity == ColorPolarity.Positive)
                            c = ColorState.Positive[ColorState.Light];
                        LocalInstanceManager.Steam.AddParticles(Position, c);
                    }
                }*/
                return true;
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
                    //InstanceManager.Logger.LogEntry(String.Format("Pre {0}", offset));
                    offset += standardEnemyRepulse * vToEnemy;
                    //InstanceManager.Logger.LogEntry(String.Format("Post {0}", offset));
                }
                return true;
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            if (playersDetected < 1)
            {
                Vector2 temp = Vector2.Negate(GetVectorPointingAtOrigin());
                temp.Normalize();
                offset += temp * walkingSpeed;
            }

            // Next apply the offset permanently
            if (offset.Length() >= minMovement)
            {
                this.Position += offset;
                offset.Normalize();
                lastDirection = offset;
                Orientation = offset;
            }

            // Check boundaries
            if (this.Position.X < -1 * RealSize.X * outsideScreenMultiplier)
            {
                this.Position.X = -1 * RealSize.X * outsideScreenMultiplier;
                Orientation.X = Math.Abs(Orientation.X);
            }
            else if (this.Position.X > InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier)
            {
                this.Position.X = InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier;
                Orientation.X = -1 * Math.Abs(Orientation.X);
            }

            if (this.Position.Y < -1 * RealSize.Y * outsideScreenMultiplier)
            {
                this.Position.Y = -1 * RealSize.Y * outsideScreenMultiplier;
                Orientation.Y = Math.Abs(Orientation.Y);
            }
            else if (this.Position.Y > InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier)
            {
                this.Position.Y = InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier;
                Orientation.Y = -1 * Math.Abs(Orientation.Y) ;
            }

            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            return true; // FIXME
        }
        #endregion
    }
}
