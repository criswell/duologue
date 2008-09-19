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

namespace Mimicware.Graphics
{
    public class SpriteObject
    {
        #region Fields
        private Texture2D texture;
        private Vector2 position;
        private Vector2 center;
        private Rectangle? source;
        private Color tint;
        private float rotation;
        private float scale;
        private float layer;
        #endregion

        #region Properties
        /// <summary>
        /// The sprite texture
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        /// <summary>
        /// The sprite position
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        /// <summary>
        /// The center of the sprite
        /// </summary>
        public Vector2 Center
        {
            get { return center; }
            set { center = value; }
        }
        /// <summary>
        /// The tint of the sprite
        /// </summary>
        public Color Tint
        {
            get { return tint; }
            set { tint = value; }
        }
        /// <summary>
        /// The rotation of the sprite
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        /// <summary>
        /// The scale of the sprite
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        /// <summary>
        /// The draw layer
        /// </summary>
        public float Layer
        {
            get { return layer; }
            set { layer = value; }
        }
        /// <summary>
        /// Source rectangle (can be nullable)
        /// </summary>
        public Rectangle? Source
        {
            get { return source; }
            set { source = value; }
        }
        #endregion

        #region Constructor/Init/Destructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture2D"></param>
        /// <param name="texturePosition"></param>
        /// <param name="textureCenter"></param>
        /// <param name="textureRect"></param>
        /// <param name="textureTint"></param>
        /// <param name="textureRotation"></param>
        /// <param name="textureScale"></param>
        /// <param name="textureLayer"></param>
        public SpriteObject(
            Texture2D texture2D,
            Vector2 texturePosition,
            Vector2 textureCenter,
            Rectangle? textureRect,
            Color textureTint,
            float textureRotation,
            float textureScale,
            float textureLayer)
        {
            texture = texture2D;
            position = texturePosition;
            center = textureCenter;
            tint = textureTint;
            rotation = textureRotation;
            scale = textureScale;
            source = textureRect;
            layer = textureLayer;
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        #endregion
    }
}
