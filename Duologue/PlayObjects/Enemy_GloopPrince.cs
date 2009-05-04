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
    public class Enemy_GloopPrince : Enemy
    {
        #region Constants
        private const string filename_gloopletHighlight = "Enemies/gloop/glooplet-highlight";
        private const string filename_gloopletDeath = "Enemies/gloop/glooplet-death";
        private const string filename_body = "Enemies/gloop/prince-gloop-body";
        private const string filename_base = "Enemies/gloop/prince-gloop-base";
        private const string filename_eye = "Enemies/gloop/king-gloop-eye";

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 2;

        /// <summary>
        /// How far we can go outside the screen before we should stop
        /// </summary>
        private const float outsideScreenMultiplier = 2.5f;

        /// <summary>
        /// The scale of the pupil
        /// </summary>
        private const float scale_eyePupil = 0.9f;

        /// <summary>
        /// The max offset of the pupil
        /// </summary>
        private const float scale_eyeOffset = 20f;

        private const float verticalOffsetHighlight = -30f;

        private const float radiusMultiplier = 0.8f;
        private const float outlineScale = 1.1f;

        #region Force interactions
        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        private const float minComfortDistance = 5.4f;
        private const float maxComfortDistance = 11.2f;

        /// <summary>
        /// The player attract modifier
        /// </summary>
        private const float maxPlayerAttract = 2.1f;

        private const float walkingSpeed = 1.3f;
        #endregion
        #endregion

        #region Properties
        #endregion

        #region Fields
        // Image rendering stuff
        private Texture2D texture_body;
        private Texture2D texture_base;
        private Texture2D texture_eye;
        private Texture2D texture_highlight;
        private Texture2D texture_death;
        private Vector2 center_body;
        private Vector2 center_highlight;
        private Vector2 center_eye;
        private Vector2 offset_eye;
        private Color eyeColor;
        private Color myColor;

        // Movement stuff
        private Vector2 offset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;
        private float minDistance;
        private float maxDistance;
        private bool upSpin;
        #endregion

        #region Constructor / Init
        public Enemy_GloopPrince() : base() { }

        public Enemy_GloopPrince(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_GloopPrince;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Leader;
            RealSize = new Vector2(96, 96);
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
                Orientation = GetStartingVector();
            else
                Orientation = startOrientation;
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            if (hitPoints == null || (int)hitPoints == 0)
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
            texture_base = InstanceManager.AssetManager.LoadTexture2D(filename_base);
            texture_body = InstanceManager.AssetManager.LoadTexture2D(filename_body);
            texture_death = InstanceManager.AssetManager.LoadTexture2D(filename_gloopletDeath);
            texture_eye = InstanceManager.AssetManager.LoadTexture2D(filename_eye);
            texture_highlight = InstanceManager.AssetManager.LoadTexture2D(filename_gloopletHighlight);

            center_body = new Vector2(
                texture_base.Width / 2f, texture_base.Height / 2f);
            center_eye = new Vector2(
                texture_eye.Width / 2f, texture_eye.Height / 2f);
            center_highlight = new Vector2(
                texture_highlight.Width / 2f, texture_highlight.Height / 2f);

            if (MWMathHelper.CoinToss())
            {
                eyeColor = new Color(85, 145, 36);
            }
            else
            {
                eyeColor = new Color(142, 109, 42);
            }
            offset_eye = Vector2.Zero;
            myColor = GetMyColor(ColorState.Medium);

            Radius = RealSize.X / 2f * radiusMultiplier;

            minDistance = Radius * minComfortDistance;
            maxDistance = Radius * maxComfortDistance;

            Initialized = true;
            upSpin = false;
            Alive = true;
        }

        public override String[] GetTextureFilenames()
        {
            return new String[]
            {
                filename_base,
                filename_body,
                filename_eye,
                filename_gloopletDeath,
                filename_gloopletHighlight
            };
        }
        #endregion

        #region Movement overrides
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
                if (temp != 0)
                {
                    if (temp == -1)
                    {
                        upSpin = true;
                        LocalInstanceManager.Steam.AddParticles(Position, myColor);
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
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            if (nearestPlayer.Length() > 0f)
            {
                // Check if we're too far away
                if (nearestPlayerRadius >= maxDistance)
                {
                    // Too far away, let's get closer
                    upSpin = false;
                    nearestPlayer.Normalize();
                    offset += maxPlayerAttract * Vector2.Negate(nearestPlayer);
                    offset_eye = scale_eyeOffset * Vector2.Negate(nearestPlayer);
                    //offset += leaderAttractFromDistance * Vector2.Negate(nearestLeader);
                }
                else if (nearestPlayerRadius < minDistance)
                {
                    // Too close, let's GTFO
                    upSpin = true;
                    nearestPlayer.Normalize();
                    offset += standardEnemyRepulse * nearestPlayer;
                    offset_eye = scale_eyeOffset * nearestPlayer;
                }
                else if (nearestPlayerRadius < maxDistance &&
                    nearestPlayerRadius >= minDistance)
                {
                    // Sweet spot, let's move around here
                    Vector2 temp = new Vector2(nearestPlayer.Y, -nearestPlayer.X);
                    temp.Normalize();

                    if (!upSpin)
                        nearestPlayer = Vector2.Negate(nearestPlayer);
                    nearestPlayer.Normalize();
                    nearestPlayer += temp;

                    offset += maxPlayerAttract * nearestPlayer;
                    offset_eye = scale_eyeOffset * nearestPlayer;
                    //offset += rotateSpeedLeader * nearestLeader;
                }
            }
            else
            {
                // If not near leader, move according to orientation
                nearestPlayer = Orientation;

                nearestPlayer.Normalize();

                offset += walkingSpeed * nearestPlayer;
            }

            this.Position += offset;
            Orientation = offset;
            Orientation.Normalize();

            if (offset_eye.Y < 0)
                offset_eye.Y = 0;

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
            return true;
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            // Outline
            InstanceManager.RenderSprite.Draw(
                texture_body,
                Position,
                center_body,
                null,
                Color.DarkSlateGray,
                0f,
                outlineScale,
                0f,
                RenderSpriteBlendMode.AlphaBlend);

            // Eye base
            InstanceManager.RenderSprite.Draw(
                texture_base,
                Position,
                center_body,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            // Pupil
            InstanceManager.RenderSprite.Draw(
                texture_eye,
                Position + offset_eye,
                center_eye,
                null,
                eyeColor,
                0f,
                scale_eyePupil,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            // Body
            InstanceManager.RenderSprite.Draw(
                texture_body,
                Position,
                center_body,
                null,
                myColor,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            // Highlight
            InstanceManager.RenderSprite.Draw(
                texture_highlight,
                Position + Vector2.UnitY * verticalOffsetHighlight,
                center_highlight,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        public override void Update(GameTime gameTime)
        {
            // blah
        }
        #endregion

        #region Private stuff
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
    }
}
