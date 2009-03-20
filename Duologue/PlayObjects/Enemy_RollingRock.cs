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
using Duologue.Audio;
using Duologue.State;
using Duologue.Screens;
#endregion

namespace Duologue.PlayObjects
{
    class Enemy_RollingRock : Enemy
    {
        #region Constants
        private const string filename_Rocky = "Enemies/rocky/pepper.png";
        private const string filename_Smoke = "Enemies/spitter/spit-{0}";

        private const int numberOfSmokeFrames = 3;

        private const double maxScale = 0.75;
        private const double minScale = 0.50;
        #endregion

        #region Properties
        #endregion

        #region Fields
        private Texture2D texture_Rock;
        private Texture2D[] texture_Smoke;

        private Vector2 center_Rock;
        private Vector2 center_Smoke;

        private float scale;
        #endregion

        #region Constructor / Init
        public Enemy_RollingRock(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Mirthworm;
            MajorType = MajorPlayObjectType.Enemy;
            Initialized = false;

            // Set the RealSize by hand
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
            throw new NotImplementedException();
        }
        #endregion

        #region Public overrides
        public override string[] GetFilenames()
        {
            throw new NotImplementedException();
        }

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

        #region Draw / Update
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
