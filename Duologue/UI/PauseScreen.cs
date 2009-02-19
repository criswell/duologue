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
using Mimicware;
using Mimicware.Manager;
using Mimicware.Graphics;
// Duologue
using Duologue;
using Duologue.Properties;
using Duologue.Screens;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.UI;
using Duologue.State;
#endregion

namespace Duologue.UI
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PauseScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string filename_overlay = "pause-overlay";
        private const string filename_textPopper = "pause-text-popper";
        private const string filename_font = "Fonts/inero-50";

        private const byte overlayAlpha = 192;

        private const byte maxTextRed = 255;
        private const byte minTextRed = 50;
        private const int maxDeltaTextRed = 20;

        private const int numberOfUpdatesPerTick = 5;

        private const double timePerJiggle = 0.1;
        #endregion

        #region Fields
        private Texture2D overlay;
        private Texture2D textPopper;
        private Vector2 textPopperCenter;
        private SpriteFont font;

        private Vector2 fontPosition;
        private int numberOfTiles;
        private Vector2 tileCounts;

        private int screenWidth;
        private int screenHeight;
        private Vector2 screenCenter;

        private SpriteEffects[] tileEffects;

        private Color color_overlay;
        private Color color_text;
        private Color color_outline;
        private Vector2[] shadowOffset;
        private int deltaTextRed;

        private double timeSinceStart;
        #endregion

        #region Properties
        #endregion

        #region Constructor/Init/Load
        public PauseScreen(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            shadowOffset = new Vector2[2];
            shadowOffset[0] = 3 * Vector2.One;
            shadowOffset[1] = -3 * Vector2.One;

            color_outline = Color.Black;

            color_text = new Color(maxTextRed, 128, 128);
            deltaTextRed = -1 * maxDeltaTextRed;

            timeSinceStart = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            overlay = InstanceManager.AssetManager.LoadTexture2D(filename_overlay);
            font = InstanceManager.AssetManager.LoadSpriteFont(filename_font);

            textPopper = InstanceManager.AssetManager.LoadTexture2D(filename_textPopper);
            textPopperCenter = new Vector2(
                textPopper.Width / 2f,
                textPopper.Height / 2f);

            numberOfTiles = -1;

            color_overlay = new Color(Color.White, overlayAlpha);

            base.LoadContent();
        }
        #endregion

        #region Private Methods
        private void InitAll()
        {
            screenWidth = InstanceManager.DefaultViewport.Width;
            screenHeight = InstanceManager.DefaultViewport.Height;

            // Setup the tile stuff
            tileCounts = new Vector2(
                (float)Math.Ceiling((double)screenWidth / (double)overlay.Width),
                (float)Math.Ceiling((double)screenHeight / (double)overlay.Height));

            numberOfTiles = (int)(tileCounts.X * tileCounts.Y);

            tileEffects = new SpriteEffects[numberOfTiles];
            for (int i = 0; i < numberOfTiles; i++)
            {
                JumbleTile(i);
            }

            // Setup the font stuff
            screenCenter = new Vector2(
                screenWidth / 2f,
                screenHeight / 2f);
            fontPosition = new Vector2(
                screenWidth / 2f - font.MeasureString(Resources.PauseScreen_GamePaused).X / 2f,
                screenHeight / 2f - font.MeasureString(Resources.PauseScreen_GamePaused).Y / 2f);
        }

        private void JumbleTile(int i)
        {
            if (MWMathHelper.CoinToss())
                tileEffects[i] = SpriteEffects.FlipHorizontally;
            else
                tileEffects[i] = SpriteEffects.None;
        }
        #endregion

        #region Update / Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            timeSinceStart += gameTime.ElapsedGameTime.TotalSeconds;

            // Check for user input
            if (InstanceManager.InputManager.NewButtonPressed(Buttons.B) ||
                InstanceManager.InputManager.NewButtonPressed(Buttons.Start) ||
                InstanceManager.InputManager.NewButtonPressed(Buttons.Back) ||
                InstanceManager.InputManager.NewButtonPressed(Buttons.A))
                LocalInstanceManager.Pause = false;

            // We only want to proceed provided the InitAll() was called
            // Since we have no guarantee that the screen is initialized here in
            // update, we ultimately will have to pass until Draw() is called
            if (numberOfTiles > 0)
            {
                // Update the font color
                int t = (int)color_text.R + deltaTextRed;

                if (t > maxTextRed)
                {
                    t = (int)maxTextRed;
                    deltaTextRed = -1 * maxDeltaTextRed;
                }
                else if (t < minTextRed)
                {
                    t = (int)minTextRed;
                    deltaTextRed = maxDeltaTextRed;
                }

                color_text = new Color((byte)t, 128, 128);

                // Update the overlays
                if (timeSinceStart > timePerJiggle)
                {
                    timeSinceStart = 0;
                    int i;
                    for (t = 0; t < numberOfUpdatesPerTick; t++)
                    {
                        i = MWMathHelper.GetRandomInRange(0, numberOfTiles);
                        if (tileEffects[i] == SpriteEffects.None)
                            tileEffects[i] = SpriteEffects.FlipHorizontally;
                        else
                            tileEffects[i] = SpriteEffects.None;
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (numberOfTiles <= 0)
            {
                InitAll();
            }

            float x = 0f;
            float y = 0f;
            // Draw the tiles
            for (int i = 0; i < numberOfTiles; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    overlay,
                    new Vector2(x, y),
                    Vector2.Zero,
                    null,
                    color_overlay,
                    0f,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop,
                    tileEffects[i]);

                x += overlay.Width;
                if (x >= screenWidth)
                {
                    x = 0f;
                    y += overlay.Height;
                    if (y >= screenHeight)
                    {
                        // okay, wtf, we shouldn't be here
                        y = 0f;
                    }
                }
            }

            // Draw the text
            InstanceManager.RenderSprite.Draw(
                textPopper,
                screenCenter,
                textPopperCenter,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            InstanceManager.RenderSprite.DrawString(
                font,
                Resources.PauseScreen_GamePaused,
                fontPosition,
                color_text,
                color_outline,
                shadowOffset,
                RenderSpriteBlendMode.AlphaBlendTop);

            base.Draw(gameTime);
        }
        #endregion
    }
}