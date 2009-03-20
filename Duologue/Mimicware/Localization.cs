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
using Mimicware;
using Mimicware.Manager;
#endregion

namespace Mimicware
{
    /// <summary>
    /// Mimicware Localization class. Used to localize various data types based upon descriptive data of the same type
    /// FIXME FIXME - for now this is just a stub class
    /// </summary>
    public class Localization
    {
        #region Constants
        #endregion

        #region Fields
        private string dataPath;
        #endregion

        #region Properties
        /// <summary>
        /// Read-only access to our localization data path
        /// </summary>
        public string Path
        {
            get { return dataPath; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a Localization object
        /// </summary>
        /// <param name="path">The path to the localization data</param>
        public Localization(string path)
        {
            dataPath = path;
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        /// <summary>
        /// Get a localized texture
        /// </summary>
        /// <param name="s">The filename</param>
        /// <returns>The localized texture</returns>
        public Texture2D GetLocalizedTexture(string s)
        {
            // TODO
            return InstanceManager.AssetManager.LoadTexture2D(s);
        }

        /// <summary>
        /// Get a localized font
        /// </summary>
        /// <param name="s">Filename</param>
        /// <returns>The localized font</returns>
        public SpriteFont GetLocalizedFont(string s)
        {
            // TODO
            return InstanceManager.AssetManager.LoadSpriteFont(s);
        }

        /// <summary>
        /// Get a localized string
        /// </summary>
        /// <param name="s">The string in your native language you wish localized</param>
        /// <returns>The localized string</returns>
        public string Get(string s)
        {
            // TODO : For now we just return the original string
            return s;
        }
        #endregion
    }
}
