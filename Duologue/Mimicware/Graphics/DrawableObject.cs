#region File Description
#endregion

#region Using statements
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
using Mimicware.Debug;
#endregion

namespace Mimicware.Graphics
{
    public class DrawableObject
    {
        #region Fields
        private bool alive;
        #endregion

        #region Properties
        public Vector2 Position;
        public virtual bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }
        public Color Tint;
        public Vector2 Center;
        #endregion

        #region Constructor/Init
        public DrawableObject(
            Vector2 position,
            Vector2 center,
            Color tint)
        {
            this.Position = position;
            this.Tint = tint;
            this.Center = center;
        }

        #endregion

        #region Public Methods
        #endregion
    }
}
