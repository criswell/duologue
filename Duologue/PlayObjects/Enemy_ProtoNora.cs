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
    public class Enemy_ProtoNora : Enemy
    {
        #region Constants
        private const string filename_Glooplet = "Enemies/gloop/glooplet";
        private const string filename_GloopletHighlight = "Enemies/gloop/glooplet-highlight";
        private const string filename_Bubble = "Enemies/iridescent_bubble";

        private const float radiusMultiplier = 0.6f;

        private const int numberOfGlobules = 5;

        private const float minThrobScale = 0.9f;
        private const float maxThrobScale = 1.1f;

        private const float scale_MainGlob = 1.2f;
        private const float scale_SubGlobs = 0.35f;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 5;
        #region Forces
        private const float meanderAttract = 0.6f;
        private const float originAttract = 0.1f;
        private const float originRepluse = 0.1f;
        private const float minOriginComfortZone = 0.5f;

        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        /*/// <summary>
        /// The minimum movement required before we register motion
        /// </summary>
        private const float minMovement = 1.2f;*/
        #endregion
        #endregion

        #region Properties
        #endregion

        #region Fields
        private Texture2D texture_Gloop;
        private Texture2D texture_Highlight;
        private Texture2D texture_Bubble;
        private Vector2 center_Gloop;
        private Vector2 center_Highlight;
        private Vector2 center_Bubble;

        private double currentPhi;
        private double deltaPhiForGlobules;
        private Color color_Bubble;
        private float rotation_Bubble;
        private float subGlobOffset;
        private Color color_Current;
        private float lengthOfGlobExtension;
        private Vector2 tempOffset;
        private Vector2 offset;
        private bool originApproach;
        //private bool isFleeing;
        #endregion

        #region Constructor / Init
        public Enemy_ProtoNora() : base() { }

        public Enemy_ProtoNora(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_ProtoNora;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Leader;
            RealSize = new Vector2(208, 200);
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
            texture_Bubble = InstanceManager.AssetManager.LoadTexture2D(filename_Bubble);
            texture_Gloop = InstanceManager.AssetManager.LoadTexture2D(filename_Glooplet);
            texture_Highlight = InstanceManager.AssetManager.LoadTexture2D(filename_GloopletHighlight);
            center_Bubble = new Vector2(
                texture_Bubble.Width / 2f, texture_Bubble.Height / 2f);
            center_Gloop = new Vector2(
                texture_Gloop.Width / 2f, texture_Gloop.Height / 2f);
            center_Highlight = new Vector2(
                texture_Highlight.Width / 2f, texture_Highlight.Height / 2f);

            Radius = RealSize.Length() * radiusMultiplier;
            currentPhi = 0;
            rotation_Bubble = 0;

            originApproach = true;

            deltaPhiForGlobules = (double)(MathHelper.TwoPi / (float)numberOfGlobules);

            color_Bubble = Color.Azure;
            color_Current = GetMyColor(ColorState.Medium);

            lengthOfGlobExtension = center_Gloop.Length() * scale_MainGlob + center_Gloop.Length() * scale_SubGlobs;

            Alive = true;
            Initialized = true;
        }

        public override string[] GetFilenames()
        {
            return new String[]
            {
                filename_Glooplet,
                filename_GloopletHighlight,
                filename_Bubble
            };
        }
        #endregion

        #region Private methods
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
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Player)
            {
                // Player
                Vector2 vToPlayer = this.Position - pobj.Position;
                float len = vToPlayer.Length();
                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em
                    return pobj.TriggerHit(this);
                }

                // Beam handling
                /*if (((Player)pobj).IsInBeam(this) == -1)
                {
                    isFleeing = true;
                    LocalInstanceManager.Steam.AddParticles(Position, color_Current);
                }*/
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
            // Apply offset due to attraction to center
            tempOffset = GetVectorPointingAtOrigin();
            float modifier = originRepluse;

            if (tempOffset.Length() < minOriginComfortZone * Radius ||
                (Position.X > InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent ||
                Position.X < InstanceManager.DefaultViewport.Width - InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent ||
                Position.Y > InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent ||
                Position.Y < InstanceManager.DefaultViewport.Height - InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent))
            {
                originApproach = !originApproach;
            }

            if (originApproach)
                modifier = originAttract;

            tempOffset.Normalize();
            offset += modifier * tempOffset;

            // Apply offset due to direction field
            tempOffset = new Vector2(
                (float)Math.Cos(Position.Y), (float)Math.Sin(Position.X));
            tempOffset.Normalize();
            offset += meanderAttract * tempOffset;

            // Apply the final offset to position
            // Next apply the offset permanently
            //if (offset.Length() >= minMovement)
            //{
            this.Position += offset;
            //}
            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            return true;
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            // Draw the globules
            InstanceManager.RenderSprite.Draw(
                texture_Gloop,
                Position,
                center_Gloop,
                null,
                color_Current,
                0f,
                scale_MainGlob,
                0f,
                RenderSpriteBlendMode.AlphaBlend);
            InstanceManager.RenderSprite.Draw(
                texture_Highlight,
                Position,
                center_Highlight,
                null,
                Color.White,
                0f,
                scale_MainGlob,
                0f,
                RenderSpriteBlendMode.AbsoluteTop);

            for (int i = 0; i < numberOfGlobules; i++)
            {
                tempOffset = new Vector2(
                        (float)Math.Cos(i * deltaPhiForGlobules + currentPhi),
                        (float)Math.Sin(i * deltaPhiForGlobules + currentPhi));
                tempOffset.Normalize();
                tempOffset = tempOffset * lengthOfGlobExtension;

                InstanceManager.RenderSprite.Draw(
                    texture_Gloop,
                    Position + tempOffset,
                    center_Gloop,
                    null,
                    color_Current,
                    0f,
                    scale_SubGlobs,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);
                InstanceManager.RenderSprite.Draw(
                    texture_Highlight,
                    Position + tempOffset,
                    center_Highlight,
                    null,
                    Color.White,
                    0f,
                    scale_SubGlobs,
                    0f,
                    RenderSpriteBlendMode.AbsoluteTop);
            }

            // Draw the bubble
            InstanceManager.RenderSprite.Draw(
                texture_Bubble,
                Position,
                center_Bubble,
                null,
                color_Bubble,
                rotation_Bubble,
                new Vector2(
                    (maxThrobScale - minThrobScale) * (float)Math.Cos(currentPhi) + minThrobScale,
                    (maxThrobScale - minThrobScale) * (float)Math.Sin(currentPhi) + minThrobScale),
                0f,
                RenderSpriteBlendMode.AbsoluteTop);
        }

        public override void Update(GameTime gameTime)
        {
            currentPhi += gameTime.ElapsedGameTime.TotalSeconds;
            if (currentPhi > MathHelper.TwoPi)
                currentPhi = 0;
        }
        #endregion
    }
}
