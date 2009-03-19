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
using Duologue.Audio;
#endregion

namespace Duologue.PlayObjects
{
    public enum GloopKingState
    {
        Standard,
        CloseEye,
        OpenEye,
        Dying
    }
    public class Enemy_GloopKing : Enemy
    {
        #region Constants
        private const string filename_kingBase = "Enemies/gloop/king-gloop-base";
        private const string filename_kingEye = "Enemies/gloop/king-gloop-eye";
        private const string filename_kingShield = "Enemies/gloop/king-gloop-shield";
        private const string filename_kingBody = "Enemies/gloop/king-gloop-{0}";
        private const string filename_kingDeath = "Enemies/gloop/king-gloop-death-{0}";

        private const int bodyFrames = 7;
        private const int deathFrames = 5;

        private const float outlineScale = 1.05f;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 1000;

        /// <summary>
        /// The point value when my shields disintegrate if I were it at perfect beat
        /// </summary>
        private const int myShieldPointValue = 10;

        /// <summary>
        /// The vertical middle of the king's base. This is offset from the true middle because the king is
        /// not strictly viewed from the top-down, but rather from a top-side-down viewpoint.
        /// </summary>
        private const float kingBaseVerticalMiddle = 82f;

        /// <summary>
        /// The offset size for the iris
        /// </summary>
        private const float irisOffsetSize = 20f;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 5;

        /// <summary>
        /// A pre-defined radius (since we can't really get this from the image
        /// </summary>
        private const float definedRadius = 70f;

        /// <summary>
        /// How far we can go outside the screen before we should stop
        /// </summary>
        private const float outsideScreenMultiplier = 1.5f;

        /// <summary>
        /// The time the eye remains open
        /// </summary>
        private const double timeEyeOpen = 1.0;

        /// <summary>
        /// The time, per frame, of the blink animation
        /// </summary>
        private const double timePerFrameBlinking = 0.1;

        /// <summary>
        /// The time, per frame, of the dying animation
        /// </summary>
        private const double timePerFrameDying = 0.07;

        #region Force interactions
        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        /// <summary>
        /// The player attract modifier
        /// </summary>
        private const float playerAttract = 2.1f;

        /// <summary>
        /// The minimum movement required before we register motion
        /// </summary>
        private const float minMovement = 2f;
        #endregion
        #endregion

        #region Fields
        private Texture2D kingBase;
        private Texture2D kingEye;
        private Texture2D kingShield;
        private Texture2D[] kingBody;
        private Texture2D[] kingDeath;

        private Vector2 kingCenter;
        private Vector2 eyeCenter;
        private Vector2 irisOffset;
        private Color currentColor;
        private Color eyeColor;
        private int currentFrame;
        private bool isFleeing;
        private GloopKingState currentState;

        private double timeSinceStart;

        // Movement
        private Vector2 offset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;
        private Vector2 lastDirection;
        #endregion

        #region Properties
        #endregion

        #region Construct/ Init
        /// <summary>
        /// The empty constructor for pre-caching
        /// </summary>
        public Enemy_GloopKing() : base() { }

        public Enemy_GloopKing(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_KingGloop;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Leader;
            RealSize = new Vector2(192, 158);
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
            if (hitPoints == null)
            {
                hitPoints = 1;
            }
            StartHitPoints = (int)hitPoints * realHitPointMultiplier;
            CurrentHitPoints = (int)hitPoints * realHitPointMultiplier;
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            kingBase = InstanceManager.AssetManager.LoadTexture2D(filename_kingBase);
            kingCenter = new Vector2(
                kingBase.Width / 2f,
                kingBaseVerticalMiddle);
            kingEye = InstanceManager.AssetManager.LoadTexture2D(filename_kingEye);
            eyeCenter = new Vector2(
                kingEye.Width / 2f,
                kingEye.Height / 2f);
            kingShield = InstanceManager.AssetManager.LoadTexture2D(filename_kingShield);

            kingBody = new Texture2D[bodyFrames];
            kingDeath = new Texture2D[deathFrames];

            for (int i = 0; i < bodyFrames; i++)
            {
                kingBody[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_kingBody, (i+1).ToString()));
            }

            for (int i = 0; i < deathFrames; i++)
            {
                kingDeath[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_kingDeath, (i + 1).ToString()));
            }

            irisOffset = Vector2.Zero;

            Radius = definedRadius;

            isFleeing = false;

            SetCurrentColors();

            currentFrame = 0;
            timeSinceStart = 0.0;

            currentState = GloopKingState.Standard;

            Initialized = true;
            Alive = true;

        }
        #endregion

        #region Private methods
        /// <summary>
        /// Call when the color state information has changed
        /// </summary>
        private void SetCurrentColors()
        {
            currentColor = GetMyColor(ColorState.Dark);
            eyeColor = GetMyColor(ColorState.Medium);
        }
        #endregion

        #region Public Overrides
        public override bool StartOffset()
        {
            offset = Vector2.Zero;
            nearestPlayerRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            nearestPlayer = Vector2.Zero;
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (currentState != GloopKingState.Dying)
            {
                if (pobj.MajorType == MajorPlayObjectType.Player)
                {
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

                    // Beam handling
                    int temp = ((Player)pobj).IsInBeam(this);
                    //inBeam = false;
                    isFleeing = false;
                    if (temp != 0)
                    {
                        //inBeam = true;
                        if (temp == -1)
                        {
                            isFleeing = true;
                            Color c = ColorState.Negative[ColorState.Light];
                            if (ColorPolarity == ColorPolarity.Positive)
                                c = ColorState.Positive[ColorState.Light];
                            LocalInstanceManager.Steam.AddParticles(Position, c);
                        }
                    }
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
                else
                {
                    // Other

                    return true;
                }
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            if (currentState != GloopKingState.Dying)
            {
                // First, apply the player offset
                if (nearestPlayer.Length() > 0f)
                {
                    float modifier = playerAttract;

                    irisOffset = Vector2.Negate(nearestPlayer);
                    irisOffset.Normalize();
                    irisOffset = irisOffset * irisOffsetSize;

                    nearestPlayer += new Vector2(nearestPlayer.Y, -nearestPlayer.X);
                    //Orientation = nearestPlayer;
                    nearestPlayer.Normalize();

                    if (!isFleeing)
                        nearestPlayer = Vector2.Negate(nearestPlayer);

                    offset += modifier * nearestPlayer;
                }
                else
                {
                    // If no near player, move in previous direction
                    nearestPlayer = lastDirection;

                    //nearestPlayer += new Vector2(nearestPlayer.Y, -nearestPlayer.X);
                    nearestPlayer.Normalize();

                    offset += playerAttract * nearestPlayer;

                    // Eyeballs
                    irisOffset = Vector2.Zero;
                }

                // Next apply the offset permanently
                if (offset.Length() >= minMovement)
                {
                    this.Position += offset;
                    lastDirection = offset;
                    Orientation = new Vector2(-offset.Y, offset.X);
                }
            }

            // Check boundaries
            if (this.Position.X < -1 * RealSize.X * outsideScreenMultiplier)
                this.Position.X = -1 * RealSize.X * outsideScreenMultiplier;
            else if (this.Position.X > InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier)
                this.Position.X = InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier;

            if (this.Position.Y < -1 * RealSize.Y * outsideScreenMultiplier)
                this.Position.Y = -1 * RealSize.Y * outsideScreenMultiplier;
            else if (this.Position.Y > InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier)
                this.Position.Y = InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier;

            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.PlayerBullet)
            {
                CurrentHitPoints--;
                if (CurrentHitPoints <= 0 && currentState != GloopKingState.Dying)
                {
                    //LocalInstanceManager.EnemyExplodeSystem.AddParticles(Position, currentColor);
                    currentState = GloopKingState.Dying;
                    timeSinceStart = 0.0;
                    currentFrame = 0;
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue, Position);
                    /*audio.soundEffects.PlayEffect(EffectID.BuzzDeath);*/
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(kingShield, currentColor, Position, 0f);
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myShieldPointValue, Position);
                    /*audio.soundEffects.PlayEffect(EffectID.CokeBottle);*/
                    return true;
                }
            }
            return true;
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            if (currentState == GloopKingState.Dying)
            {
                InstanceManager.RenderSprite.Draw(
                    kingDeath[currentFrame],
                    Position,
                    kingCenter,
                    null,
                    currentColor,
                    0f,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);
            }
            else
            {
                // Outline
                InstanceManager.RenderSprite.Draw(
                    kingBody[currentFrame],
                    Position,
                    kingCenter,
                    null,
                    Color.Black,
                    0f,
                    outlineScale,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);

                // Draw the base
                InstanceManager.RenderSprite.Draw(
                    kingBase,
                    Position,
                    kingCenter,
                    null,
                    Color.White,
                    0f,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);

                // Draw the eyeball
                InstanceManager.RenderSprite.Draw(
                    kingEye,
                    Position + irisOffset,
                    eyeCenter,
                    null,
                    eyeColor,
                    0f,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);

                // Draw the body
                InstanceManager.RenderSprite.Draw(
                    kingBody[currentFrame],
                    Position,
                    kingCenter,
                    null,
                    currentColor,
                    0f,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
            }
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceStart += gameTime.ElapsedGameTime.TotalSeconds;

            switch (currentState)
            {
                case GloopKingState.Dying:
                    if (timeSinceStart > timePerFrameDying)
                    {
                        currentFrame++;
                        timeSinceStart = 0.0;
                        if (currentFrame >= deathFrames)
                        {
                            Alive = false;
                            currentFrame = 0;
                        }
                    }
                    break;
                case GloopKingState.OpenEye:
                    if (timeSinceStart > timePerFrameBlinking)
                    {
                        currentFrame--;
                        timeSinceStart = 0.0;
                        if (currentFrame < 0)
                        {
                            currentFrame = 0;
                            currentState = GloopKingState.Standard;
                        }
                    }
                    break;
                case GloopKingState.CloseEye:
                    if (timeSinceStart > timePerFrameBlinking)
                    {
                        currentFrame++;
                        timeSinceStart = 0.0;
                        if(currentFrame >= bodyFrames)
                        {
                            currentFrame = bodyFrames - 1;
                            currentState = GloopKingState.OpenEye;
                            if (ColorPolarity == ColorPolarity.Positive)
                                ColorPolarity = ColorPolarity.Negative;
                            else
                                ColorPolarity = ColorPolarity.Positive;

                            SetCurrentColors();
                        }
                    }
                    break;
                default:
                    // Standard
                    if (timeSinceStart > timeEyeOpen)
                    {
                        currentFrame = 0;
                        timeSinceStart = 0.0;
                        currentState = GloopKingState.CloseEye;
                    }

                    break;
            }
        }
        #endregion
    }
}
