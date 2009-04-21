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
    public class AssetManager
    {
        #region Fields
        private Dictionary<string, Texture2D> graphicAssets;
        private Dictionary<string, SpriteFont> fontAssets;
        private Dictionary<string, SoundEffect> soundAssets;
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
            graphicAssets = new Dictionary<string,Texture2D>();
            fontAssets = new Dictionary<string, SpriteFont>();
            soundAssets = new Dictionary<string, SoundEffect>();
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

        /// <summary>
        /// Load a SpriteFont
        /// </summary>
        /// <param name="p">The filename of the asset to load</param>
        /// <returns>The font</returns>
        public SpriteFont LoadSpriteFont(string filename)
        {
            if (!fontAssets.ContainsKey(filename))
            {
                fontAssets.Add(filename, content.Load<SpriteFont>(filename));
            }
            return fontAssets[filename];
        }

        /// <summary>
        /// Load a sound effect
        /// </summary>
        /// <param name="filename">The filename of the asset to load</param>
        /// <returns>The sound effect</returns>
        public SoundEffect LoadSoundEffect(string filename)
        {
            if (!soundAssets.ContainsKey(filename))
            {
                soundAssets.Add(filename, content.Load<SoundEffect>(filename));
            }
            return soundAssets[filename];
        }
        #endregion
    }
}
