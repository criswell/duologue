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
    public class Enemy_Buzzsaw : PlayObject
    {
        #region Constants
        #endregion

        #region Fields
        #endregion

        #region Constructor / Init / Load
        public Enemy_Buzzsaw()
            : base()
        {
            MyType = TypesOfPlayObjects.Enemy_Buzzsaw;
            Initialized = false;
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        #endregion

        #region Draw / Update
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
