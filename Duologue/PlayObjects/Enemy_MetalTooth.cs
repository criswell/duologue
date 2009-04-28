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
    public class Enemy_MetalTooth :Enemy
    {
        #region Constants
        #endregion

        #region Properties
        #endregion

        #region Fields
        #endregion

        #region Constructor / Init
        public Enemy_MetalTooth() : base() { }

        public Enemy_MetalTooth(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_MetalTooth;
            MajorType = MajorPlayObjectType.Enemy;
            Initialized = false;
            //shineOffset = new Vector2(delta_ShineOffsetX, delta_ShineOffsetY);

            // Set the RealSize by hand
            //RealSize = new Vector2(85, 87);
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

        public override string[] GetTextureFilenames()
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

        #region Draw/Update
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
