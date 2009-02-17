﻿#region File Description
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
    public struct StaticKingFrame
    {
        public Texture2D Texture;
        public byte Alpha;
        public float Scale;
        public float Rotation;
        public float DeltaRotation;
        public Vector2 Offset;
    }

    class Enemy_StaticKing : Enemy
    {
        #region Constants
        private const string filename_face = "Enemies/static-king-face";
        private const string filename_frames = "Enemies/static-king-{0}";

        private const int numberOfFrames = 4;

        /// <summary>
        /// The starting scale for each frame
        /// </summary>
        private const float startingScale = 0.6f;

        /// <summary>
        /// The delta scale per frame of animation
        /// </summary>
        private const float deltaScale = 0.1f;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 3;

        /// <summary>
        /// The maximum starting angle (in either direction)
        /// </summary>
        private const float maxStartingAngle = MathHelper.PiOver4 / 2f;

        /// <summary>
        /// The maximum delta angle
        /// </summary>
        private const float maxDeltaAngle = MathHelper.PiOver4 / 4f;

        /// <summary>
        /// The maximum vertical offset
        /// </summary>
        private const double maxVerticalOffset = -4.0;

        /// <summary>
        /// A pre-defined radius (since we can't really get this from the image
        /// </summary>
        private const float definedRadius = 120f;

        /// <summary>
        /// How far we can go outside the screen before we should stop
        /// </summary>
        private const float outsideScreenMultiplier = 1.5f;
        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 500;

        /// <summary>
        /// The point value when my shields disintegrate if I were it at perfect beat
        /// </summary>
        private const int myShieldPointValue = 10;

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
        private Texture2D textureFace;
        private StaticKingFrame[] frames;
        private Vector2 center;

        private bool isFleeing;

        private double timeSinceStart;

        private Color currentColor;

        private bool faceFlipped;

        // Movement
        private Vector2 offset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;
        private Vector2 lastDirection;
        #endregion

        #region Constructor / Init
        public Enemy_StaticKing(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_StaticKing;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Leader;
            RealSize = new Vector2(150, 143);
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
            //audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            textureFace = InstanceManager.AssetManager.LoadTexture2D(filename_face);
            frames = new StaticKingFrame[numberOfFrames];
            center = new Vector2(
                textureFace.Width / 2f,
                textureFace.Height / 2f);

            for (int i = 0; i < numberOfFrames; i++)
            {
                frames[i].Texture = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_frames, i.ToString()));
                SetupFrame(i);
                frames[i].Scale = startingScale + i * deltaScale;
            }

            Radius = definedRadius;

            isFleeing = false;

            currentColor = GetMyColor(ColorState.Medium);

            timeSinceStart = 0.0;

            faceFlipped = false;

            Initialized = true;
            Alive = true;
        }
        #endregion

        #region Private methods
        private void SetupFrame(int i)
        {
            frames[i].Scale = startingScale;
            frames[i].Alpha = Color.White.A;
            if (MWMathHelper.CoinToss())
            {
                frames[i].Rotation = (float)MWMathHelper.GetRandomInRange(-1.0 * maxStartingAngle, 0.0);
                frames[i].DeltaRotation = (float)MWMathHelper.GetRandomInRange(0.0, maxDeltaAngle);
            }
            else
            {
                frames[i].Rotation = (float)MWMathHelper.GetRandomInRange(0.0, maxStartingAngle);
                frames[i].DeltaRotation = (float)MWMathHelper.GetRandomInRange(-1.0 * maxDeltaAngle, 0.0);
            }
            frames[i].Offset = new Vector2(
                0f, (float)MWMathHelper.GetRandomInRange(0.0, maxVerticalOffset));
        }
        #endregion

        #region Public overrides
        public override bool StartOffset()
        {
            offset = Vector2.Zero;
            nearestPlayerRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            nearestPlayer = Vector2.Zero;
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
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
                    offset += standardEnemyRepulse * vToEnemy;
                }

                return true;
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            // First, apply the player offset
            if (nearestPlayer.Length() > 0f)
            {
                float modifier = playerAttract;

                nearestPlayer += new Vector2(nearestPlayer.Y, -nearestPlayer.X);
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
            }

            // Next apply the offset permanently
            if (offset.Length() >= minMovement)
            {
                this.Position += offset;
                lastDirection = offset;
                Orientation = new Vector2(-offset.Y, offset.X);
            }

            if (Orientation.X < 0)
                faceFlipped = true;
            else
                faceFlipped = false;

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
                if (CurrentHitPoints <= 0)
                {
                    // Fire off explosions for each frame, if we can
                    for (int i = 0; i < numberOfFrames; i++)
                    {
                        LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                            Position + (float)MWMathHelper.GetRandomInRange(-1.0, 1.0) * center,
                            currentColor);
                    }
                    timeSinceStart = 0.0;
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue, Position);
                    /*audio.soundEffects.PlayEffect(EffectID.BuzzDeath);*/
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(
                        frames[MWMathHelper.GetRandomInRange(0, numberOfFrames)].Texture,
                        currentColor,
                        Position,
                        0f);
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
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}