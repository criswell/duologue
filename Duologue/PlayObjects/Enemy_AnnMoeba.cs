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

        private const int numberOfGlobules = 5;

        private const double minScale = 0.6;
        private const double maxScale = 1.2;
        #region Force interactions
        #endregion
        #endregion

        #region Fields
        #endregion

        #region Properties
        private Texture2D texture_Glooplet;
        private Texture2D texture_Highlight;
        private Texture2D texture_Death;
        private Vector2 center_Glooplet;
        private Vector2 center_Highlight;

        private Vector2[] offset_Globules;
        private float[] scale_Globules;
        private float mainScale;
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
            RealSize = new Vector2(83, 83);
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

        public override void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
