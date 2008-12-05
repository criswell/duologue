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
        private const string filename_outline = "Enemies/buzzsaw-outline";
        private const string filename_blades = "Enemies/buzzsaw-blades";

        // Deltas
        private const float delta_Rotation = 0.1f;
        #endregion

        #region Fields
        // The textures for this enemy
        private Texture2D buzzBase;
        private Texture2D buzzOutline;
        private Texture2D buzzBlades;

        // What state we're in
        private bool isFleeing;

        // Housekeeping graphical doo-dads
        private Vector2 center;
        private float baseLayer;
        private float faceLayer;
        private float rotation;
        #endregion

        #region Constructor / Init / Load
        public Enemy_Buzzsaw()
            : base()
        {
            MyType = TypesOfPlayObjects.Enemy_Buzzsaw;
            MajorType = MajorPlayObjectType.Enemy;
            Initialized = false;
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
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            if (hitPoints == null)
            {
                hitPoints = 1;
            }
            CurrentHitPoints = hitPoints;
            LoadAndInitialize();
        }

        /// <summary>
        /// Load and initialize this enemy
        /// </summary>
        private void LoadAndInitialize()
        {
            // Load the textures
            buzzBase = InstanceManager.AssetManager.LoadTexture2D(filename_base);
            buzzOutline = InstanceManager.AssetManager.LoadTexture2D(filename_outline);
            buzzBlades = InstanceManager.AssetManager.LoadTexture2D(filename_blades);

            center = new Vector2(
                buzzBase.Width / 2f,
                buzzBase.Height / 2f);

            Radius = buzzBase.Width / 2f;
            if (buzzBase.Height / 2f > Radius)
                Radius = buzzBase.Height / 2f;

            baseLayer = 0.3f;
            faceLayer = 0.2f;

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
            switch (ColorPolarity)
            {
                case ColorPolarity.Negative:
                    c = ColorState.Negative[ColorState.Light];
                    break;
                default:
                    c = ColorState.Positive[ColorState.Light];
                    break;
            }
            if (isFleeing)
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
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (isFleeing)
            {
                // We only spin wildly when we're running the fuck away
                rotation += delta_Rotation;

                if (rotation > MathHelper.TwoPi)
                    rotation = 0f;
                else if (rotation < 0f)
                    rotation = MathHelper.TwoPi;
            }
        }
        #endregion

        #region Public overrides
        public override bool StartOffset()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool ApplyOffset()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}
