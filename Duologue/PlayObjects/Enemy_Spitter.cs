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
    class Enemy_Spitter : Enemy
    {
        #region Constants
        private const string filename_base = "Enemies/spitter/0{0}-base";
        private const string filename_outline = "Enemies/spitter/0{0}-outline";
        private const string filename_spawnExplode = "Enemies/spitter/spawn-explode";

        /// <summary>
        /// Max number of frames of animation
        /// </summary>
        private const int maxFrames = 5;
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public Enemy_Spitter(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Spitter;
            MajorType = MajorPlayObjectType.Enemy;
            RealSize = new Vector2(82, 90);
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

        #region Private methods
        #endregion

        #region Public Methods
        #endregion

        #region Public Overrides
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
