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
    public class Enemy_AnnMoeba :Enemy
    {
        #region Constants
        private const string filename_Glooplet = "Enemies/gloop/glooplet";
        private const string filename_GloopletHighlight = "Enemies/gloop/glooplet-highlight";
        private const string filename_Death = "Enemies/gloop/glooplet-death";
        private const string filename_Bubble = "Enemies/iridescent_bubble";

        private const float bubbleScale = 0.43f;

        private const float radiusMultiplier = 0.8f;

        private const int numberOfGlobules = 5;

        private const float globulesRadiusScale = 0.25f;

        private const double minScale = 0.6;
        private const double maxScale = 1.2;

        private const float minThrobScale = 0.9f;
        private const float maxThrobScale = 1.1f;

        private const double minGlobuleScale = 0.1;
        private const double maxGlobuleScale = 0.4;

        private const double minGlobuleAddition = -10.0;
        private const double maxGlobuleAddition = 10.0;

        private const float delta_Rotation = MathHelper.PiOver4 * 0.01f;

        private const double time_Spawning = 1.8;
        #region Force interactions
        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        /// <summary>
        /// Min number of how many of our radius size away we're comfortable with the player
        /// </summary>
        private const float minPlayerComfortRadiusMultiplier = 4.4f;

        /// <summary>
        /// The repulsion from the player if the player gets too close
        /// this should be lower than attract so that the player can bump into them
        /// </summary>
        private const float playerRepulse = 1.5f;

        /// <summary>
        /// Our repulsion force when we're hit by a light
        /// </summary>
        private const float lightRepulse = 1.2f;

        /// <summary>
        /// Our attraction to the center if we move off screen
        /// </summary>
        private const float centerAttract = 1.5f;

        /// <summary>
        /// The minimum movement required before we register motion
        /// </summary>
        private const float minMovement = 1.2f;
        #endregion
        #endregion

        #region Fields
        #endregion

        #region Properties
        private Texture2D texture_Glooplet;
        private Texture2D texture_Highlight;
        private Texture2D texture_Death;
        private Texture2D texture_Bubble;
        private Vector2 center_Bubble;
        private Vector2 center_Glooplet;
        private Vector2 center_Highlight;
        private Vector2 center_Death;

        private Color color_Bubble;
        private Color color_Current;

        private Vector2[] offset_Globules;
        private Vector2[] phiOperands_Addition;
        private float[] scale_Globules;
        private float mainScale;

        private Vector2 throbScale;

        private double currentPhi;
        private float bubbleRotation;

        private bool isSpawning;
        private double timeSinceSwitch;
        private float spawnScale;

        private Vector2 offset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;
        private Vector2 nearestLeader;
        private float nearestLeaderRadius;
        private Enemy nearestLeaderObject;

        private bool isFleeing;
        #endregion

        #region Constructor / Init
        /// <summary>
        /// The empty constructor for pre-caching
        /// </summary>
        public Enemy_AnnMoeba() : base() { }

        public Enemy_AnnMoeba(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_AnnMoeba;
            MajorType = MajorPlayObjectType.Enemy;
            RealSize = new Vector2(90, 87);
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
            // We define our own start position
            Position = new Vector2(
                (float)MWMathHelper.GetRandomInRange(
                    (double)InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent,
                    (double)InstanceManager.DefaultViewport.Width - (double)InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent),
                (float)MWMathHelper.GetRandomInRange(
                    (double)InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent,
                    (double)InstanceManager.DefaultViewport.Height - (double)InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent));
            Orientation = startOrientation;
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            if (hitPoints == null)
            {
                hitPoints = 0;
            }
            StartHitPoints = (int)hitPoints;
            CurrentHitPoints = (int)hitPoints;
            //audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            texture_Glooplet = InstanceManager.AssetManager.LoadTexture2D(filename_Glooplet);
            texture_Death = InstanceManager.AssetManager.LoadTexture2D(filename_Death);
            texture_Highlight = InstanceManager.AssetManager.LoadTexture2D(filename_GloopletHighlight);
            texture_Bubble = InstanceManager.AssetManager.LoadTexture2D(filename_Bubble);
            center_Bubble = new Vector2(
                texture_Bubble.Width / 2f, texture_Bubble.Height / 2f);
            center_Glooplet = new Vector2(
                texture_Glooplet.Width / 2f, texture_Glooplet.Height / 2f);
            center_Highlight = new Vector2(
                texture_Highlight.Width / 2f, texture_Highlight.Height / 2f);
            center_Death = new Vector2(
                texture_Death.Width / 2f, texture_Death.Height / 2f);

            mainScale = (float)MWMathHelper.GetRandomInRange(minScale, maxScale);
            scale_Globules = new float[numberOfGlobules];
            offset_Globules = new Vector2[numberOfGlobules];
            phiOperands_Addition = new Vector2[numberOfGlobules];

            Radius = RealSize.Length() * mainScale * radiusMultiplier * bubbleScale;

            for (int i = 0; i < numberOfGlobules; i++)
            {
                scale_Globules[i] = (float)MWMathHelper.GetRandomInRange(minGlobuleScale, maxGlobuleScale);
                phiOperands_Addition[i] = new Vector2(
                    (float)MWMathHelper.GetRandomInRange(minGlobuleAddition, maxGlobuleAddition),
                    (float)MWMathHelper.GetRandomInRange(minGlobuleAddition, maxGlobuleAddition));
            }
            currentPhi = MWMathHelper.GetRandomInRange(0, MathHelper.TwoPi);
            ComputeGlobuleOffsets();

            bubbleRotation = (float)MWMathHelper.GetRandomInRange(0, MathHelper.TwoPi);

            color_Bubble = new Color(2, 109, 74);
            color_Current = GetMyColor(ColorState.Medium);

            isSpawning = true;
            spawnScale = 0;
            timeSinceSwitch = 0;

            Initialized = true;
            Alive = true;
        }

        public override string[] GetFilenames()
        {
            return new String[]
            {
                filename_Death,
                filename_Glooplet,
                filename_GloopletHighlight,
                filename_Bubble
            };
        }
        #endregion

        #region Private methods
        private void ComputeGlobuleOffsets()
        {
            for (int i = 0; i < numberOfGlobules; i++)
            {
                offset_Globules[i] = new Vector2(
                    (float)Math.Cos(currentPhi + phiOperands_Addition[i].X),
                    (float)Math.Sin(currentPhi + phiOperands_Addition[i].Y)) * Radius * globulesRadiusScale;
            }

            throbScale = new Vector2(
                (maxThrobScale - minThrobScale) * (float)Math.Cos(currentPhi) + minThrobScale,
                (maxThrobScale - minThrobScale) * (float)Math.Sin(currentPhi) + minThrobScale) * bubbleScale * mainScale;
        }

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

        #region Public overrides
        public override bool StartOffset()
        {
            offset = Vector2.Zero;
            nearestPlayerRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            nearestPlayer = Vector2.Zero;
            nearestLeaderRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            nearestLeader = Vector2.Zero;
            nearestLeaderObject = null;
            isFleeing = false;
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
                    if(!isSpawning)
                        return pobj.TriggerHit(this);
                }

                // Beam handling
                if (((Player)pobj).IsInBeam(this) == -1)
                {
                    isFleeing = true;
                    LocalInstanceManager.Steam.AddParticles(Position, GetMyColor());
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

            }
            return true;
        }

        public override bool ApplyOffset()
        {
            // First, apply the player offset
            if (nearestPlayer.Length() > 0f)
            {
                nearestPlayer.Normalize();
                if (nearestPlayerRadius < minPlayerComfortRadiusMultiplier * Radius)
                {
                    offset += playerRepulse * nearestPlayer;
                }

                if (isFleeing)
                {
                    offset += lightRepulse * nearestPlayer;
                }
            }

            // Check boundaries
            if (Position.X > InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent ||
                Position.X < InstanceManager.DefaultViewport.Width - InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent ||
                Position.Y > InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent ||
                Position.Y < InstanceManager.DefaultViewport.Height - InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent)
            {
                Vector2 temp = GetVectorPointingAtOrigin();
                temp.Normalize();
                offset += centerAttract * temp;
            }

            // Next apply the offset permanently
            if (offset.Length() >= minMovement)
            {
                this.Position += offset;
            }

            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            return false;
        }
        #endregion

        #region Public Draw/Update
        public override void Draw(GameTime gameTime)
        {
            // Start by doing the innards
            for (int i = 0; i < numberOfGlobules; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    texture_Glooplet,
                    Position + offset_Globules[i],
                    center_Glooplet,
                    null,
                    color_Current,
                    0f,
                    scale_Globules[i] * spawnScale,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);

                InstanceManager.RenderSprite.Draw(
                    texture_Highlight,
                    Position + offset_Globules[i],
                    center_Highlight,
                    null,
                    Color.White,
                    0f,
                    scale_Globules[i] * spawnScale,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
            }

            // Finish by doing the bubble
            InstanceManager.RenderSprite.Draw(
                texture_Bubble,
                Position,
                center_Bubble,
                null,
                Color.White,
                bubbleRotation,
                throbScale * spawnScale,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        public override void Update(GameTime gameTime)
        {
            currentPhi += gameTime.ElapsedGameTime.TotalSeconds;
            if (currentPhi > MathHelper.TwoPi)
                currentPhi = 0;

            if (isSpawning)
            {
                timeSinceSwitch += gameTime.ElapsedGameTime.TotalSeconds;
                spawnScale = (float)(timeSinceSwitch / time_Spawning);
                if (timeSinceSwitch > time_Spawning)
                {
                    timeSinceSwitch = 0;
                    spawnScale = 1f;
                    isSpawning = false;
                }
            }

            bubbleRotation += delta_Rotation;
            if (bubbleRotation > MathHelper.TwoPi)
                bubbleRotation = 0;
            else if (bubbleRotation < 0)
                bubbleRotation = (float)MathHelper.TwoPi;

            ComputeGlobuleOffsets();
        }
        #endregion
    }
}
