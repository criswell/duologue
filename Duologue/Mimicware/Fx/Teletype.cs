#region File Description
#endregion

#region Using statements
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
// Mimicware
using Mimicware;
using Mimicware.Graphics;
using Mimicware.Manager;
using Mimicware.Debug;
using Duologue.Audio; // BAH! IService and SL need to be in Mimicware space FIXME
#endregion

namespace Mimicware.Fx
{
    /// <summary>
    /// An entry object for the teletype
    /// </summary>
    public class TeletypeEntry
    {
        #region Constants
        private const float maxAlpha = 1f;
        private const float minAlpha = 0f;
        #endregion

        #region Fields
        private double masterTimer;
        #endregion

        #region Properties
        /// <summary>
        /// The text to be displayed
        /// </summary>
        public String Text;
        /// <summary>
        /// The font to be used
        /// </summary>
        public SpriteFont Font;
        /// <summary>
        /// The position of this text
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// The center of the text which defines where position is applied on this text
        /// </summary>
        public Vector2 Center;
        /// <summary>
        /// The color of the text
        /// </summary>
        public Color Color;
        /// <summary>
        /// Determines if the text fades in or just appears as typing
        /// </summary>
        public bool FadeIn;
        /// <summary>
        /// The time it takes for this entry to type on screen
        /// </summary>
        public double TimeToType;
        /// <summary>
        /// The total time this entry will be on screen
        /// </summary>
        public double TotalTimeOnScreen;
        /// <summary>
        /// If you use a shadow for this text, this will be the color
        /// </summary>
        public Color ShadowColor;
        /// <summary>
        /// Shadow offsets. If non-null, will be used.
        /// </summary>
        public Vector2[] ShadowOffset = null;
        /// <summary>
        /// The rendersprite instance to use
        /// </summary>
        public RenderSprite RenderSprite;
        #endregion

        #region Constructor
        public TeletypeEntry(RenderSprite renderSprite)
        {
            RenderSprite = renderSprite;
        }

        public TeletypeEntry(
            SpriteFont font,
            String text,
            Vector2 position,
            Color color,
            bool fadeIn,
            double timeToType,
            double totalTimeOnScree,
            RenderSprite renderSprite)
        {
            Font = font;
            Text = text;
            Position = position;
            Color = color;
            FadeIn = fadeIn;
            TimeToType = timeToType;
            TotalTimeOnScreen = totalTimeOnScree;
            RenderSprite = renderSprite;
        }

        #endregion
    }

    public class Teletype : DrawableGameComponent, IService
    {
        #region Constants
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        #endregion

        #region Public methods
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion
    }
}
