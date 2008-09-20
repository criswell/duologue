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
        private Vector2 center;
        private Rectangle? source;
        private Color tint;
        private float rotation;
        private float scale;
        private float layer;
        private bool alive;
        #endregion

        #region Properties
        /// <summary>
        /// Determines if the sprite is alive or not
        /// </summary>
        public bool Alive
        {
            set { alive = value; }
            get { return alive; }
        }
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
        public Vector2 Position;

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

        /// <summary>
        /// Text (if this is rendered text)
        /// </summary>
        public string Text;

        /// <summary>
        /// If rendered text, this is the font
        /// </summary>
        public SpriteFont Font;

        /// <summary>
        /// The direction of this spriteobject
        /// </summary>
        public Vector2 Direction;
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
            Position = texturePosition;
            center = textureCenter;
            tint = textureTint;
            rotation = textureRotation;
            scale = textureScale;
            source = textureRect;
            layer = textureLayer;
            Text = null;
        }

        public SpriteObject(
            SpriteFont font,
            string text,
            Vector2 textPosition,
            Color textTint)
        {
            Font = font;
            Text = text;
            texture = null;
            Position = textPosition;
            tint = textTint;
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        #endregion
    }
}
