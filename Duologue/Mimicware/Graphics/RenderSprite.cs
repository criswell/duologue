#region File info
#endregion

#region Using statements
// System
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
// Mimicware
using Mimicware.Manager;
#endregion

namespace Mimicware.Graphics
{
    /// <summary>
    /// The blend mode used in RenderSprite.
    /// </summary>
    /// <remarks>
    /// The blend mode used in RenderSprite is a bit more expansive than the
    /// stock SpriteBlendModes for SpriteBatch. We include the standard AlphaBlend and
    /// Additive. But we add Multiplicative.
    /// </remarks>
    public enum RenderSpriteBlendMode
    {
        AlphaBlend,
        Addititive,
        Multiplicative,
    }
    public class RenderSprite
    {
        #region Fields
        private SpriteBatch batch;
        private List<SpriteObject> sprites;
        private List<SpriteObject> additiveSprites;
        private List<SpriteObject> multiplicativeSprites;
        #endregion

        #region Properties
        #endregion

        #region Constructor/Init/Destructor
        public RenderSprite(SpriteBatch spriteBatch)
        {
            batch = spriteBatch;
            sprites = new List<SpriteObject>();
            additiveSprites = new List<SpriteObject>();
            multiplicativeSprites = new List<SpriteObject>();
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

        /// <summary>
        /// Adds a sprite to the additive batch
        /// </summary>
        /// <param name="spriteObject">The sprite object to add</param>
        private void AddAdditive(SpriteObject spriteObject)
        {
            additiveSprites.Add(spriteObject);
        }

        /// <summary>
        /// Adds a sprite to the multiplicative batch
        /// </summary>
        /// <param name="spriteObject">The sprite object to add</param>
        private void AddMultiplicative(SpriteObject spriteObject)
        {
            multiplicativeSprites.Add(spriteObject);
        }

        /// <summary>
        /// Given a list of sprites, render them
        /// </summary>
        private void RenderAllSprites(List<SpriteObject> sobjs)
        {
            foreach (SpriteObject sobj in sobjs)
            {
                if (sobj.Texture != null)
                    batch.Draw(
                        sobj.Texture,
                        sobj.Position,
                        sobj.Source,
                        sobj.Tint,
                        sobj.Rotation,
                        sobj.Center,
                        sobj.Scale,
                        sobj.SpriteEffects,
                        sobj.Layer);
                else if (sobj.Text != null)
                    batch.DrawString(
                        sobj.Font,
                        sobj.Text,
                        sobj.Position,
                        sobj.Tint,
                        sobj.Rotation,
                        sobj.Center,
                        sobj.Scale,
                        sobj.SpriteEffects,
                        sobj.Layer);
            }
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
            Vector2 scale,
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
            float layer,
            RenderSpriteBlendMode mode)
        {
            if (mode == RenderSpriteBlendMode.Addititive)
                this.AddAdditive(
                    new SpriteObject(
                        texture,
                        position,
                        center,
                        source,
                        tint,
                        rotation,
                        scale,
                        layer));
            else if (mode == RenderSpriteBlendMode.Multiplicative)
                this.AddMultiplicative(
                    new SpriteObject(
                        texture,
                        position,
                        center,
                        source,
                        tint,
                        rotation,
                        scale,
                        layer));
            else
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
            Vector2 scale,
            float layer,
            RenderSpriteBlendMode mode)
        {
            if (mode == RenderSpriteBlendMode.Addititive)
                this.AddAdditive(
                    new SpriteObject(
                        texture,
                        position,
                        center,
                        source,
                        tint,
                        rotation,
                        scale,
                        layer));
            else if (mode == RenderSpriteBlendMode.Multiplicative)
                this.AddMultiplicative(
                    new SpriteObject(
                        texture,
                        position,
                        center,
                        source,
                        tint,
                        rotation,
                        scale,
                        layer));
            else
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
        /// 
        /// </summary>
        /// <param name="font"></param>
        /// <param name="p"></param>
        /// <param name="vector2"></param>
        /// <param name="color"></param>
        internal void DrawString(SpriteFont font, string p, Vector2 vector2, Color color)
        {
            this.Add(new SpriteObject(
                font,
                p,
                vector2,
                color));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="font"></param>
        /// <param name="p"></param>
        /// <param name="vector2"></param>
        /// <param name="color"></param>
        /// <param name="scale"></param>
        internal void DrawString(SpriteFont font, string p, Vector2 vector2, Color color, Vector2 scale)
        {
            SpriteObject sobj = new SpriteObject(
                font,
                p,
                vector2,
                color);
            sobj.Scale = scale;
            this.Add(sobj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="font"></param>
        /// <param name="p"></param>
        /// <param name="vector2"></param>
        /// <param name="color"></param>
        /// <param name="scale"></param>
        /// <param name="cent"></param>
        internal void DrawString(SpriteFont font, string p, Vector2 vector2, Color color, Vector2 scale, Vector2 cent)
        {
            SpriteObject sobj = new SpriteObject(
                font,
                p,
                vector2,
                color);
            sobj.Scale = scale;
            sobj.Center = cent;
            this.Add(sobj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="font"></param>
        /// <param name="p"></param>
        /// <param name="vector2"></param>
        /// <param name="color"></param>
        internal void DrawString(SpriteFont font, string p, Vector2 vector2, Color color, RenderSpriteBlendMode mode)
        {
            if (mode == RenderSpriteBlendMode.Addititive)
                this.AddAdditive(new SpriteObject(
                    font,
                    p,
                    vector2,
                    color));
            else if (mode == RenderSpriteBlendMode.Multiplicative)
                this.AddMultiplicative(new SpriteObject(
                    font,
                    p,
                    vector2,
                    color));
            else
                this.Add(new SpriteObject(
                    font,
                    p,
                    vector2,
                    color));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteObject"></param>
        internal void Draw(SpriteObject spriteObject)
        {
            this.Add(spriteObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteObject"></param>
        internal void Draw(SpriteObject spriteObject, RenderSpriteBlendMode mode)
        {
            if (mode == RenderSpriteBlendMode.Addititive)
                this.AddAdditive(spriteObject);
            else if (mode == RenderSpriteBlendMode.Multiplicative)
                this.AddMultiplicative(spriteObject);
            else
                this.Add(spriteObject);
        }

        internal void Draw(
            Texture2D texture,
            Vector2 position,
            Vector2 center,
            Rectangle? source,
            Color tint,
            float rotation,
            float scale,
            float layer,
            RenderSpriteBlendMode mode,
            SpriteEffects spriteEffects)
        {
            SpriteObject sobj = new SpriteObject(
                texture,
                position,
                center,
                source,
                tint,
                rotation,
                scale,
                layer);
            sobj.SpriteEffects = spriteEffects;
            if (mode == RenderSpriteBlendMode.Addititive)
                this.AddAdditive(sobj);
            else if (mode == RenderSpriteBlendMode.Multiplicative)
                this.AddMultiplicative(sobj);
            else
                this.Add(sobj);
        }

        /// <summary>
        /// Run the batch of sprites
        /// </summary>
        public void Run()
        {
            // Render the standard alphablend sprites
            if (sprites.Count > 0)
            {
                batch.Begin(SpriteBlendMode.AlphaBlend);
                RenderAllSprites(sprites);
                batch.End();
            }
            sprites.Clear();

            if (multiplicativeSprites.Count > 0)
            {
                batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                InstanceManager.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationColor;
                InstanceManager.GraphicsDevice.RenderState.DestinationBlend = Blend.SourceColor;

                RenderAllSprites(multiplicativeSprites);

                batch.End();
            }
            multiplicativeSprites.Clear();

            if (additiveSprites.Count > 0)
            {
                batch.Begin(SpriteBlendMode.Additive);

                RenderAllSprites(additiveSprites);

                batch.End();
            }
            additiveSprites.Clear();

            // Other renders as needed
        }
        #endregion
    }
}
