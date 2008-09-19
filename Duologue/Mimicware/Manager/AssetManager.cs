#region File info
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Mimicware.Manager
{
    class AssetManager
    {
        #region Fields
        private Dictionary<string, Texture2D> graphicAssets;
        private ContentManager content;
        #endregion

        #region Properties
        #endregion

        #region Constructor/Init
        /// <summary>
        /// The Mimicware AssetManager handles pre-loading common images. Use it
        /// when you have an image that you will need to "load" multiple times, but
        /// only want to actually load once.
        /// </summary>
        /// <param name="ContentManager">The game's content manager instance</param>
        public AssetManager(ContentManager ContentManager)
        {
            content = ContentManager;
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Load a Texture2D
        /// </summary>
        /// <param name="filename">The filename of the asset to load</param>
        /// <returns>The texture</returns>
        public Texture2D LoadTexture2D(string filename)
        {
            if (!graphicAssets.ContainsKey(filename))
            {
                graphicAssets.Add(filename, content.Load<Texture2D>(filename));
            }
            return graphicAssets[filename];
        }
        #endregion
    }
}
