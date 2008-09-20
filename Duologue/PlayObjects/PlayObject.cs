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
    class PlayObject
    {
        #region Fields
        #endregion

        #region Properties
        /// <summary>
        /// Write-only property for setting the current asset manager
        /// Must be set before component is added to the game.
        /// </summary>
        public AssetManager AssetManager;

        /// <summary>
        /// Write-only property for setting the current render srpite instance
        /// Must be set before component is added to the game.
        /// </summary>
        public RenderSprite RenderSprite;

        /// <summary>
        /// Write-only property for setting the current graphics device
        /// Must be set before component is added to the game.
        /// </summary>
        public GraphicsDevice GraphicsDevice;

        /// <summary>
        /// The current position of the play object
        /// </summary>
        public Vector2 Position;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a new PlayObject
        /// </summary>
        /// <param name="manager">The game's asset manager</param>
        /// <param name="graphics">The game's graphics device</param>
        /// <param name="renderer">The game's sprite renderer</param>
        public PlayObject(AssetManager manager, GraphicsDevice graphics, RenderSprite renderer)
        {
            AssetManager = manager;
            RenderSprite = renderer;
            GraphicsDevice = graphics;
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public PlayObject()
        {
            // FIXME: We need to get rid of this
            AssetManager = null;
            GraphicsDevice = null;
            RenderSprite = null;
        }
        #endregion

        #region Public methods
        #endregion
    }
}
