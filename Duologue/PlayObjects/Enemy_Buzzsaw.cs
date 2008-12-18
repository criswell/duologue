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
#endregion

namespace Duologue.PlayObjects
{
    public class Enemy_Buzzsaw : Enemy
    {
        #region Constants
        // Filenames
        private const string filename_base = "Enemies/buzzsaw-base";
        private const string filename_blades = "Enemies/buzzsaw-blades";
        private const string filename_shine = "Enemies/buzzsaw-shine";

        // Deltas
        private const float delta_Rotation = 0.1f;
        private const float delta_ShineOffsetX = -0f;
        private const float delta_ShineOffsetY = -4f;

        /// <summary>
        /// The divisor that determines the radius of the entire
        /// enemy given the radius of the base
        /// </summary>
        private const float radiusDivisor = 1.5f;

        /// <summary>
        /// The maximum number of hitpoints
        /// </summary>
        private const int maxHitPoints = 5;

        /// <summary>
        /// The hit point array
        /// </summary>
        static readonly byte[] hitPointArray = new byte[] {(byte)128, (byte)160, (byte)192, (byte)224, (byte)255};
        #region Force attractions / Repulsions

        /// <summary>
        /// The player attract modifier
        /// </summary>
        private const float playerAttract = 2.5f;

        /// <summary>
        /// The repulsion to the player's light
        /// </summary>
        private const float playerRepulse = 4f;

        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        /// <summary>
        /// The minimum movement required before we register motion
        /// </summary>
        private const float minMovement = 2f;
        #endregion

        #endregion

        #region Fields
        // The textures for this enemy
        private Texture2D buzzBase;
        private Texture2D buzzBlades;
        private Texture2D buzzShine;

        // What state we're in
        private bool isFleeing;
        private bool inBeam;

        // Housekeeping graphical doo-dads
        private Vector2 baseCenter;
        private Vector2 bladesCenter;
        private Vector2 shineCenter;
        private Vector2 shineOffset;
        private float baseLayer;
        private float bladesLayer;
        private float rotation;
        private float bladesRotation;

        // Movement
        private Vector2 offset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;
        #endregion

        #region Constructor / Init / Load
        public Enemy_Buzzsaw()
            : base()
        {
            MyType = TypesOfPlayObjects.Enemy_Buzzsaw;
            MajorType = MajorPlayObjectType.Enemy;
            Initialized = false;
            shineOffset = new Vector2(delta_ShineOffsetX, delta_ShineOffsetY);
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
            rotation = MWMathHelper.ComputeAngleAgainstX(Orientation);
            bladesRotation = 0f;
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            if (hitPoints == null)
            {
                hitPoints = 0;
            }
            CurrentHitPoints = (int)hitPoints;
            LoadAndInitialize();
        }

        /// <summary>
        /// Load and initialize this enemy
        /// </summary>
        private void LoadAndInitialize()
        {
            // Load the textures
            buzzBase = InstanceManager.AssetManager.LoadTexture2D(filename_base);
            buzzBlades = InstanceManager.AssetManager.LoadTexture2D(filename_blades);
            buzzShine = InstanceManager.AssetManager.LoadTexture2D(filename_shine);

            baseCenter = new Vector2(
                buzzBase.Width / 2f,
                buzzBase.Height / 2f);

            bladesCenter = new Vector2(
                buzzBlades.Width / 2f,
                buzzBlades.Height / 2f);

            shineCenter = new Vector2(
                buzzShine.Width / 2f,
                buzzShine.Height / 2f);

            Radius = buzzBase.Width/radiusDivisor;
            if (buzzBase.Height/radiusDivisor > Radius)
                Radius = buzzBase.Height/radiusDivisor;

            baseLayer = 0.3f;
            bladesLayer = 0.4f;

            isFleeing = false;

            Initialized = true;
            Alive = true;
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            Color c;
            Color s;
            switch (ColorPolarity)
            {
                case ColorPolarity.Negative:
                    c = ColorState.Negative[ColorState.Light];
                    break;
                default:
                    c = ColorState.Positive[ColorState.Light];
                    break;
            }

            s = c;
            if (CurrentHitPoints < maxHitPoints)
                s.A = hitPointArray[CurrentHitPoints];
            else
                s.A = hitPointArray[0];
            /*if (isFleeing)
            {
                InstanceManager.RenderSprite.Draw(
                    buzzOutline,
                    Position,
                    center,
                    null,
                    c,
                    rotation,
                    1f,
                    baseLayer);
            }
            else
            {
                InstanceManager.RenderSprite.Draw(
                    buzzBase,
                    Position,
                    center,
                    null,
                    c,
                    MWMathHelper.ComputeAngleAgainstX(Orientation),
                    1f,
                    baseLayer);
                InstanceManager.RenderSprite.Draw(
                    buzzBlades,
                    Position,
                    center,
                    null,
                    Color.White,
                    0f,
                    1f,
                    faceLayer);
            }*/

            InstanceManager.RenderSprite.Draw(
                buzzBlades,
                Position,
                bladesCenter,
                null,
                Color.White,
                bladesRotation,
                1f,
                bladesLayer);
            InstanceManager.RenderSprite.Draw(
                buzzBase,
                Position,
                baseCenter,
                null,
                c,
                rotation,
                1f,
                baseLayer);
            InstanceManager.RenderSprite.Draw(
                buzzShine,
                Position + shineOffset,
                shineCenter,
                null,
                s,
                0f,
                1f,
                baseLayer);

        }

        public override void Update(GameTime gameTime)
        {
            /*if (isFleeing)
            {
                // We only spin wildly when we're running the fuck away
                rotation += delta_Rotation;

                if (rotation > MathHelper.TwoPi)
                    rotation = 0f;
                else if (rotation < 0f)
                    rotation = MathHelper.TwoPi;
            }*/

            rotation = MWMathHelper.ComputeAngleAgainstX(Orientation);

            bladesRotation += delta_Rotation;
            if (bladesRotation > MathHelper.TwoPi)
                bladesRotation = 0f;
            else if (bladesRotation < 0f)
                bladesRotation = MathHelper.TwoPi;
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
                    pobj.TriggerHit(this);
                }

                // Beam handling
                int temp = ((Player)pobj).IsInBeam(this);
                inBeam = false;
                isFleeing = false;
                if (temp != 0)
                {
                    inBeam = true;
                    if (temp == -1)
                        isFleeing = true;
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

        public override bool ApplyOffset()
        {
            // First, apply the player offset
            if (nearestPlayer.Length() > 0f)
            {
                nearestPlayer += new Vector2(nearestPlayer.Y, -nearestPlayer.X);
                Orientation = new Vector2(-nearestPlayer.Y, nearestPlayer.X);
                nearestPlayer.Normalize();

                if (isFleeing)
                {
                    offset += playerRepulse * nearestPlayer;
                }
                else
                {
                    nearestPlayer = Vector2.Negate(nearestPlayer);
                    offset += playerAttract * nearestPlayer;
                }
            }

            // Next apply the offset permanently
            if (offset.Length() >= minMovement)
                this.Position += offset;
            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}
