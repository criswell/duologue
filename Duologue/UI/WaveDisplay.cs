#region File Description
#endregion

#region Using Statements
// System
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
using Mimicware.Manager;
using Mimicware.Graphics;
// Duologue
using Duologue;
using Duologue.Properties;
#endregion

namespace Duologue.UI
{
    public enum TextTransitionType
    {
        ZoomIn=0,
        ZoomOut,
        Fade,
        MaxNum,
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class WaveDisplay : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string fontFilename = "Content/inero-50";
        private const float fadeLifetime = 1f;
        private const float fadeInLifetime = 0.2f;
        private const float fadeOutLifetime = 0.8f;
        #endregion

        #region Fields
        private SpriteFont font;
        private Game localGame;
        private string[] theText;
        private float timeSinceStart;
        private TextTransitionType currentType;
        private Random rand;
        private Vector2 screenCenter;
        private Vector2[] shadowOffset;
        #endregion

        #region Properties
        /// <summary>
        /// The text
        /// </summary>
        public string[] Text
        {
            get
            {
                return theText;
            }
            set
            {
                theText = value;
                timeSinceStart = 0f;
                GetNewTransitionType();
            }
        }

        /// <summary>
        /// Tells the percente complete in the fade in
        /// </summary>
        public float PercentComplete
        {
            get { return Math.Min(timeSinceStart / fadeLifetime, 1f); }
        }

        /// <summary>
        /// The main color of the text
        /// </summary>
        public Color MainColor;

        /// <summary>
        /// The shadow color of the text
        /// </summary>
        public Color ShadowColor;
        #endregion

        #region Constructor / Init / Load
        public WaveDisplay(Game game)
            : base(game)
        {
            localGame = game;
            rand = new Random();
            screenCenter = Vector2.Zero;
            shadowOffset = new Vector2[1];
            shadowOffset[0] = Vector2.One;
            // Sensible default colors
            MainColor = Color.Azure;
            ShadowColor = Color.Black;
            // start out off, we change when someone sets new text
            timeSinceStart = fadeLifetime;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = InstanceManager.AssetManager.LoadSpriteFont(fontFilename);
            base.LoadContent();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Get a new transition type
        /// </summary>
        private void GetNewTransitionType()
        {
            currentType = (TextTransitionType)rand.Next((int)TextTransitionType.MaxNum);
        }

        /// <summary>
        /// The fade draw method
        /// </summary>
        private void DrawFade(GameTime gameTime)
        {
            // Get the position
            Vector2 totalSize = GetTotalSize();
            Vector2 pos = screenCenter - totalSize / 2f;

            // Calculate the fade percent
            float fadePercent = 1f;
            if (timeSinceStart < fadeInLifetime)
            {
                // We're fading in
                fadePercent = timeSinceStart / fadeInLifetime;
            }
            else if (timeSinceStart > fadeOutLifetime)
            {
                // We're fading out
                fadePercent = 1f - timeSinceStart / fadeOutLifetime;
            }

            for (int i = 0; i < theText.Length; i++)
            {
                pos.X = screenCenter.X - font.MeasureString(theText[i]).X / 2f;
                InstanceManager.RenderSprite.DrawString(
                    font,
                    theText[i],
                    pos,
                    new Color(
                        MainColor.R,
                        MainColor.G,
                        MainColor.B,
                        (byte)(255 * fadePercent)),
                    new Color(
                        ShadowColor.R,
                        ShadowColor.G,
                        ShadowColor.B,
                        (byte)(255 * fadePercent)),
                    shadowOffset,
                    RenderSpriteBlendMode.AlphaBlend);
            }
        }

        /// <summary>
        /// Get the total size of theText
        /// </summary>
        private Vector2 GetTotalSize()
        {
            Vector2 theSize = Vector2.Zero;
            for (int i = 0; i < theText.Length; i++)
            {
                Vector2 size = font.MeasureString(theText[i]);
                if (size.X > theSize.X)
                    theSize.X = size.X;

                theSize.Y += size.Y;
            }
            return theSize;
        }
        #endregion

        #region Public methods
        #endregion

        #region Update / Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (timeSinceStart < fadeLifetime)
                timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (screenCenter == Vector2.Zero)
            {
                screenCenter = new Vector2(
                    InstanceManager.DefaultViewport.Width / 2f,
                    InstanceManager.DefaultViewport.Height / 2f);
            }
            if (PercentComplete < 1f)
            {
                switch (currentType)
                {
                    case TextTransitionType.ZoomIn:
                        break;
                    default:
                        // Fade
                        DrawFade(gameTime);
                        break;
                }
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}