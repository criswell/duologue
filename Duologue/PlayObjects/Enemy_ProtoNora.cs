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

        private const float radiusMultiplier = 0.4f;

        private const int numberOfGlobules = 5;

        private const float minThrobScale = 0.9f;
        private const float maxThrobScale = 1.1f;

        private const float scale_MainGlob = 0.85f;
        private const float scale_SubGlobs = 0.35f;

        private const double timeBetweenTurns = 1.1;
        private const double minTurnAngle = -1f * MathHelper.PiOver4;
        private const double maxTurnAngle = MathHelper.PiOver4;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 400;

        /// <summary>
        /// The point value when my shields disintegrate if I were it at perfect beat
        /// </summary>
        private const int myShieldPointValue = 5;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 20;

        private const float vertHighlightOffset = -10f;
        #region Forces
        private const float speed = 1.1f;

        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;
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
        private Color color_Current;
        private float subGlobOffset;
        private Vector2 tempOffset;
        private Vector2 offset;
        private double timeSinceSwitch;
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
            if (startOrientation == Vector2.Zero)
            {
                Orientation = GetStartingVector();
            }
            else
            {
                Orientation = startOrientation;
            }
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            if (hitPoints == null || (int)hitPoints == 0)
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

            Radius = RealSize.Length() * 0.5f * radiusMultiplier;
            currentPhi = 0;
            rotation_Bubble = 0;

            timeSinceSwitch = 0;

            deltaPhiForGlobules = (double)(MathHelper.TwoPi / (float)numberOfGlobules);

            color_Bubble = Color.Azure;
            color_Current = GetMyColor(ColorState.Medium);

            subGlobOffset = (center_Gloop.Length() * scale_MainGlob + center_Gloop.Length() * scale_SubGlobs) /2f;

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
            // Check boundaries
            if (this.Position.X < -1 * RealSize.X)
            {
                this.Position.X = -1 * RealSize.X;
                Orientation = GetVectorPointingAtOrigin();
            }
            else if (this.Position.X > InstanceManager.DefaultViewport.Width + RealSize.X)
            {
                this.Position.X = InstanceManager.DefaultViewport.Width + RealSize.X;
                Orientation = GetVectorPointingAtOrigin();
            }

            if (this.Position.Y < -1 * RealSize.Y)
            {
                this.Position.Y = -1 * RealSize.Y;
                Orientation = GetVectorPointingAtOrigin();
            }
            else if (this.Position.Y > InstanceManager.DefaultViewport.Height + RealSize.Y)
            {
                this.Position.Y = InstanceManager.DefaultViewport.Height + RealSize.Y;
                Orientation = GetVectorPointingAtOrigin();
            }
            
            Orientation.Normalize();

            offset += Orientation * speed;

            this.Position += offset;
            Orientation = offset;

            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.PlayerBullet)
            {
                CurrentHitPoints--;
                if (CurrentHitPoints <= 0)
                {
                    //LocalInstanceManager.EnemyExplodeSystem.AddParticles(Position, currentColor);
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue, Position);
                    Alive = false;
                    for (int i = 0; i < numberOfGlobules; i++)
                    {
                        tempOffset = new Vector2(
                            (float)Math.Cos(i * deltaPhiForGlobules + currentPhi),
                            (float)Math.Sin(i * deltaPhiForGlobules + currentPhi));
                        tempOffset.Normalize();
                        tempOffset = tempOffset * (subGlobOffset * (float)Math.Cos(currentPhi) + 5f);
                        if (MWMathHelper.CoinToss())
                            LocalInstanceManager.EnemySplatterSystem.AddParticles(Position + tempOffset, color_Bubble);
                        else
                            LocalInstanceManager.EnemySplatterSystem.AddParticles(Position + tempOffset, color_Current);
                    }
                    /*audio.soundEffects.PlayEffect(EffectID.BuzzDeath);*/
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(texture_Bubble, color_Bubble, Position, 0f);
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
                Position + Vector2.UnitY * vertHighlightOffset,
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
                tempOffset = tempOffset * (subGlobOffset * (float)Math.Cos(currentPhi) + 5f);

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
                    RenderSpriteBlendMode.AbsoluteTop,
                    SpriteEffects.FlipVertically);
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

            timeSinceSwitch += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceSwitch > timeBetweenTurns)
            {
                // Turn randomly
                Orientation = MWMathHelper.RotateVectorByRadians(Orientation,
                    (float)MWMathHelper.GetRandomInRange(minTurnAngle, maxTurnAngle));
                timeSinceSwitch = 0;
            }
        }
        #endregion
    }
}
