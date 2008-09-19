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
    public class RenderSprite
    {
        #region Fields
        private SpriteBatch batch;
        private List<SpriteObject> sprites;
        #endregion

        #region Properties
        #endregion

        #region Constructor/Init/Destructor
        public RenderSprite(SpriteBatch spriteBatch)
        {
            batch = spriteBatch;
            sprites = new List<SpriteObject>();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Adds a sprite to the batch
        /// </summary>
        /// <param name="sprite">The sprite object to add</param>
        private void Add(SpriteObject sprite)
        {
            sprites.Add(sprite);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Draw a sprite
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="center"></param>
        /// <param name="source"></param>
        /// <param name="tint"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public void Draw(
            Texture2D texture,
            Vector2 position,
            Vector2 center,
            Rectangle? source,
            Color tint,
            float rotation,
            float scale,
            float layer)
        {
            this.Add(new SpriteObject(
                texture,
                position,
                center,
                source,
                tint,
                rotation,
                scale,
                layer));
        }

        /// <summary>
        /// Run the batch of sprites
        /// </summary>
        public void Run()
        {
            // Render the standard alphablend sprites
            batch.Begin(SpriteBlendMode.AlphaBlend);

            foreach (SpriteObject sobj in sprites)
            {
                batch.Draw(
                    sobj.Texture,
                    sobj.Position,
                    sobj.Source,
                    sobj.Tint,
                    sobj.Rotation,
                    sobj.Center,
                    sobj.Scale,
                    SpriteEffects.None,
                    sobj.Layer);
            }
            batch.End();
            sprites.Clear();

            // Other renders as needed
        }
        #endregion
    }
}
