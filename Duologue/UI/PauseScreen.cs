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
        private const string filename_fontTitle = "Fonts/inero-50";
        private const string filename_fontMenu = "Fonts/inero-40";

        private const byte overlayAlpha = 192;

        private const byte maxTextRed = 255;
        private const byte minTextRed = 50;
        private const int maxDeltaTextRed = 20;

        private const int numberOfUpdatesPerTick = 5;

        private const double timePerJiggle = 0.1;

        private const float extraLineSpacing = 12;

        private const int windowOffsetX = 30;
        private const int windowOffsetY = 10;
        private const int titleSpacing = 40;

        private const float selectOffset = 8;
        private const int numberOfOffsets = 4;
        #endregion

        #region Fields
        private Texture2D overlay;
        private SpriteFont fontTitle;
        private SpriteFont fontMenu;

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

        private Game myGame;
        private bool initialized;

        // Menu items
        private Vector2 menuOffset;
        private List<MenuItem> pauseMenuItems;
        private int resumeGame;
        private int exitMainMenu;
        private Rectangle pauseMenuWindowLocation;
        private Vector2 position;
        private Vector2[] shadowOffsets;
        private Vector2[] shadowOffsetsSelected;
        #endregion

        #region Properties
        #endregion

        #region Constructor/Init/Load
        public PauseScreen(Game game)
            : base(game)
        {
            myGame = game;

            pauseMenuItems = new List<MenuItem>();

            pauseMenuItems.Add(new MenuItem(Resources.PauseScreen_ResumeGame));
            resumeGame = 0;
            pauseMenuItems.Add(new MenuItem(Resources.PauseScreen_ExitMainMenu));
            exitMainMenu = 1;

            shadowOffsets = new Vector2[numberOfOffsets];
            shadowOffsets[0] = Vector2.One;
            shadowOffsets[1] = -1 * Vector2.One;
            shadowOffsets[2] = new Vector2(-1f, 1f);
            shadowOffsets[3] = new Vector2(1f, -1f);

            shadowOffsetsSelected = new Vector2[numberOfOffsets];
            shadowOffsetsSelected[0] = 2 * Vector2.One;
            shadowOffsetsSelected[1] = -2 * Vector2.One;
            shadowOffsetsSelected[2] = new Vector2(-2f, 2f);
            shadowOffsetsSelected[3] = new Vector2(2f, -2f);

            initialized = false;
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
            fontTitle = InstanceManager.AssetManager.LoadSpriteFont(filename_fontTitle);
            fontMenu = InstanceManager.AssetManager.LoadSpriteFont(filename_fontMenu);

            numberOfTiles = -1;

            color_overlay = new Color(Color.DarkSlateGray, overlayAlpha);

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
        }

        private void JumbleTile(int i)
        {
            if (MWMathHelper.CoinToss())
                tileEffects[i] = SpriteEffects.FlipHorizontally;
            else
                tileEffects[i] = SpriteEffects.None;
        }

        /// <summary>
        /// Generate the position
        /// </summary>
        private void SetPostion()
        {
            float center = InstanceManager.DefaultViewport.Width / 2f;
            float xOffset = center;
            float maxWidth = 0;
            float maxHeight = 0;
            Vector2 size;
            float xTest;

            // Start with the title
            size = fontTitle.MeasureString(Resources.PauseScreen_GamePaused);
            xTest = center - size.X / 2f;
            if (xTest < xOffset)
                xOffset = xTest;

            if (size.X > maxWidth)
                maxWidth = size.X;

            maxHeight += fontTitle.LineSpacing + titleSpacing;

            // Move to the menu
            foreach (MenuItem mi in pauseMenuItems)
            {
                size = fontMenu.MeasureString(mi.Text);
                xTest = center - size.X / 2f;
                if (xTest < xOffset)
                    xOffset = xTest;

                // Compute max width and height
                if (size.X > maxWidth)
                    maxWidth = size.X;
                maxHeight += fontMenu.LineSpacing + extraLineSpacing;
            }

            position = new Vector2(
                screenCenter.X - maxWidth/2f,
                screenCenter.Y - maxHeight/2f);

            pauseMenuWindowLocation = new Rectangle(
                (int)position.X - windowOffsetX,
                (int)position.Y - windowOffsetY,
                (int)maxWidth + 2 * windowOffsetX,
                (int)maxHeight + fontMenu.LineSpacing + (int)extraLineSpacing + 2 * windowOffsetY);

            LocalInstanceManager.WindowManager.SetLocation(pauseMenuWindowLocation);
            initialized = true;
        }

        /// <summary>
        /// Draw a list of menu items
        /// </summary>
        private void DrawMenu(List<MenuItem> mis, GameTime gameTime, Vector2 curPos)
        {
            foreach (MenuItem mi in mis)
            {
                mi.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                if (mi.Invisible)
                {
                    InstanceManager.RenderSprite.DrawString(fontMenu,
                        mi.Text,
                        curPos,
                        mi.Selected ? Color.DarkSlateBlue : Color.DarkSlateGray,
                        RenderSpriteBlendMode.AddititiveTop);
                }
                else
                {
                    InstanceManager.RenderSprite.DrawString(
                        fontMenu,
                        mi.Text,
                        curPos + mi.FadePercent * selectOffset * Vector2.UnitX,
                        mi.Selected ? Color.LightGoldenrodYellow : Color.Khaki,
                        Color.Black,
                        mi.Selected ? shadowOffsetsSelected : shadowOffsets,
                        RenderSpriteBlendMode.AlphaBlendTop);
                }
                curPos.Y += fontMenu.LineSpacing + extraLineSpacing;
            }

        }
        #endregion

        #region Overrides
        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if(this.Enabled && initialized)
                LocalInstanceManager.WindowManager.SetLocation(pauseMenuWindowLocation);
            base.OnEnabledChanged(sender, args);
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
            LocalInstanceManager.WindowManager.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (numberOfTiles <= 0)
            {
                InitAll();
                SetPostion();
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

            LocalInstanceManager.WindowManager.Draw(gameTime);

            // Draw the menu
            menuOffset = Vector2.Zero;

            InstanceManager.RenderSprite.DrawString(
                fontTitle,
                Resources.PauseScreen_GamePaused,
                position + menuOffset,
                color_text,
                color_outline,
                shadowOffset,
                RenderSpriteBlendMode.AlphaBlendTop);

            menuOffset.Y += fontTitle.LineSpacing + titleSpacing;

            DrawMenu(pauseMenuItems, gameTime, position + menuOffset);

            base.Draw(gameTime);
        }
        #endregion
    }
}