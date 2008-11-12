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
        #endregion

        #region Fields
        // The textures for this enemy
        private Texture2D baseAgg;
        private Texture2D baseFlee;
        private Texture2D faceAgg;
        private Texture2D faceFlee;

        // What state we're in
        private bool isFleeing;

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
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Update(GameTime gameTime)
        {
            throw new Exception("The method or operation is not implemented.");
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
