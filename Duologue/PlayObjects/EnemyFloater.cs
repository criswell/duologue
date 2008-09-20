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
    class EnemyFloater : PlayObject
    {
        #region Constants
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public EnemyFloater(AssetManager manager, GraphicsDevice graphics, RenderSprite renderer)
            : base(manager, graphics, renderer)
        {
            Initialize();
        }

        public EnemyFloater()
            : base()
        {
            // FIXME: We need to get rid of this
            Initialize();
        }

        /// <summary>
        /// Initialize
        /// </summary>
        private void Initialize()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}
