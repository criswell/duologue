using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Mimicware.Graphics;
using Mimicware.Manager;

namespace Duologue.PlayObjects
{
    class PlayerShot : PlayObject
    {
        #region Constants
        private const int numShots = 10;
        #endregion

        #region Fields
        private SpriteObject shot;
        private Vector2[] shots;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public PlayerShot(AssetManager manager, GraphicsDevice graphics, RenderSprite renderer)
            : base(manager, graphics, renderer)
        {
            Initialize();
        }

        public PlayerShot()
            : base()
        {
            Initialize();
        }

        private void Initialize()
        {
            
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        #endregion
    }
}
