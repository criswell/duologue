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
        #endregion

        #region Properties
        #endregion

        #region Fields
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
            throw new NotImplementedException();
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
