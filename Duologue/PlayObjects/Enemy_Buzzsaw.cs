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
        private const string filename_baseAgg = "Enemies/buzzsaw-agg";
        private const string filename_baseFlee = "Enemies/buzzsaw-flee";
        private const string filename_faceAgg = "Enemies/buzzsaw-face-agg";
        private const string filename_faceFlee = "Enemies/buzzsaw-face-flee";

        // Deltas
        private const float delta_Rotation = 0.1f;
        #endregion

        #region Fields
        // The textures for this enemy
        private Texture2D baseAgg;
        private Texture2D baseFlee;
        private Texture2D faceAgg;
        private Texture2D faceFlee;

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
            ColorPolarity startColorPolarity)
        {
            Position = startPos;
            Orientation = startOrientation;
            rotation = MWMathHelper.ComputeAngleAgainstX(Orientation);
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            LoadAndInitialize();
        }

        /// <summary>
        /// Load and initialize this enemy
        /// </summary>
        private void LoadAndInitialize()
        {
            // Load the textures
            baseAgg = InstanceManager.AssetManager.LoadTexture2D(filename_baseAgg);
            baseFlee = InstanceManager.AssetManager.LoadTexture2D(filename_baseFlee);
            faceAgg = InstanceManager.AssetManager.LoadTexture2D(filename_faceAgg);
            faceFlee = InstanceManager.AssetManager.LoadTexture2D(filename_faceFlee);

            center = new Vector2(
                baseAgg.Width / 2f,
                baseAgg.Height / 2f);

            Radius = baseAgg.Width / 2f;
            if (baseAgg.Height / 2f > Radius)
                Radius = baseAgg.Height / 2f;

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
                    baseFlee,
                    Position,
                    center,
                    null,
                    c,
                    rotation,
                    1f,
                    baseLayer);
                InstanceManager.RenderSprite.Draw(
                    faceFlee,
                    Position,
                    center,
                    null,
                    Color.White,
                    0f,
                    1f,
                    faceLayer);
            }
            else
            {
                InstanceManager.RenderSprite.Draw(
                    baseAgg,
                    Position,
                    center,
                    null,
                    c,
                    MWMathHelper.ComputeAngleAgainstX(Orientation),
                    1f,
                    baseLayer);
                InstanceManager.RenderSprite.Draw(
                    faceAgg,
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
