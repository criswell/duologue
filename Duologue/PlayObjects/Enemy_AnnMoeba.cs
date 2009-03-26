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

        private const double minScale = 0.6;
        private const double maxScale = 1.2;

        private const double minGlobuleScale = 0.1;
        private const double maxGlobuleScale = 0.4;

        private const double minGlobuleAddition = -1.0;
        private const double maxGlobuleAddition = 1.0;

        private const double minGlobuleMultiplication = -3.0;
        private const double maxGlobuleMultiplication = 3.0;
        #region Force interactions
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

        private Color color_Bubble;

        private Vector2[] offset_Globules;
        private Vector2[] phiOperands_Addition;
        private Vector2[] phiOperands_Multiplication;
        private float[] scale_Globules;
        private float mainScale;

        private double currentPhi;
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
            Position = startPos;
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

            mainScale = (float)MWMathHelper.GetRandomInRange(minScale, maxScale);
            scale_Globules = new float[numberOfGlobules];
            offset_Globules = new Vector2[numberOfGlobules];
            phiOperands_Addition = new Vector2[numberOfGlobules];
            phiOperands_Multiplication = new Vector2[numberOfGlobules];

            Radius = RealSize.Length() * mainScale * radiusMultiplier;

            for (int i = 0; i < numberOfGlobules; i++)
            {
                scale_Globules[i] = (float)MWMathHelper.GetRandomInRange(minGlobuleScale, maxGlobuleScale);
                phiOperands_Addition[i] = new Vector2(
                    (float)MWMathHelper.GetRandomInRange(minGlobuleAddition, maxGlobuleAddition),
                    (float)MWMathHelper.GetRandomInRange(minGlobuleAddition, maxGlobuleAddition));
                phiOperands_Multiplication[i] = new Vector2(
                    (float)MWMathHelper.GetRandomInRange(minGlobuleMultiplication, maxGlobuleMultiplication),
                    (float)MWMathHelper.GetRandomInRange(minGlobuleMultiplication, maxGlobuleMultiplication));
            }
            currentPhi = 0;
            ComputeGlobuleOffsets();

            color_Bubble = new Color(2, 109, 74, 200);

            Initialized = true;
            Alive = true;
        }

        public override string[] GetFilenames()
        {
            return new String[]
            {
                filename_Death,
                filename_Glooplet,
                filename_GloopletHighlight
            };
        }
        #endregion

        #region Private methods
        private void ComputeGlobuleOffsets()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Public overrides
        public override bool StartOffset()
        {
            throw new NotImplementedException();
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            throw new NotImplementedException();
        }

        public override bool ApplyOffset()
        {
            throw new NotImplementedException();
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Public Draw/Update
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
