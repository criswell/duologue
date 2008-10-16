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
using Duologue.Screens;
using Duologue.PlayObjects;
#endregion

namespace Duologue.Screens
{
    /// <summary>
    /// The base backgrounds for the player select screen
    /// </summary>
    public class PlayerSelectBase : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const float fadeInTime = 0.5f;
        private const float fadeOutTime = 0.2f;
        private const string tileFilename = "multiplayer-select-tile";
        private const string fontFilename = "Fonts\\inero-28";
        #endregion

        #region Fields
        // The graphics textures
        private Texture2D tile;
        private PlayerColors[] playerColors;
        private Vector2[] center;
        private Vector2 position;
        private float[] rotation;
        private SpriteEffects[] seffects;
        // Time
        private float timeSinceStart;
        private bool begin;
        // Font/Text
        private SpriteFont font;
        private Color fontMain;
        private Color fontSub;
        private Vector2 mainTextPos;
        private string mainText;
        #endregion

        #region Properties
        public float Percentage
        {
            get
            {
                if (begin)
                {
                    return MathHelper.Min(timeSinceStart / fadeInTime, 1f);
                }
                else
                {
                    return MathHelper.Min(1f, timeSinceStart / fadeOutTime);
                }
            }
        }
        #endregion

        #region Constructor / Init / Load
        public PlayerSelectBase(Game game)
            : base(game)
        {
            center = new Vector2[InputManager.MaxInputs];
            rotation = new float[InputManager.MaxInputs];
            seffects = new SpriteEffects[InputManager.MaxInputs];
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            begin = true;
            timeSinceStart = 0f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            tile = InstanceManager.AssetManager.LoadTexture2D(tileFilename);

            center[0] = new Vector2(
                tile.Width,
                tile.Height);

            center[1] = new Vector2(
                0f,
                tile.Height);

            center[2] = new Vector2(
                tile.Width,
                0f);

            center[3] = new Vector2(
                tile.Width,
                tile.Height);

            rotation[0] = 0f;
            seffects[0] = SpriteEffects.None;
            rotation[1] = 0f;
            seffects[1] = SpriteEffects.FlipHorizontally;
            rotation[2] = 0f;
            seffects[2] = SpriteEffects.FlipVertically;
            rotation[3] = MathHelper.ToRadians(180);
            seffects[3] = SpriteEffects.None;

            playerColors = PlayerColors.GetPlayerColors();
            position = Vector2.Zero;

            // Text
            font = InstanceManager.AssetManager.LoadSpriteFont(fontFilename);
            fontMain = Color.Wheat;
            fontSub = Color.Black;
            mainText = Resources.PlayerSelect_MainText;
            mainTextPos = Vector2.Zero;
            base.LoadContent();
        }
        #endregion

        #region Private methods
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
            if (Percentage < 1f)
                timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (position == Vector2.Zero)
            {
                position = new Vector2(
                    InstanceManager.DefaultViewport.Width / 2f,
                    InstanceManager.DefaultViewport.Height / 2f);
            }

            if (mainTextPos == Vector2.Zero)
            {
                Vector2 titleSafe = new Vector2(
                    InstanceManager.DefaultViewport.Width/2f,
                    InstanceManager.DefaultViewport.Height - InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent);

                Vector2 textSize = font.MeasureString(mainText);

                mainTextPos = new Vector2(
                    titleSafe.X - textSize.X / 2f,
                    titleSafe.Y);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    tile,
                    position,
                    center[i],
                    null,
                    new Color(
                        playerColors[i].Colors[1].R,
                        playerColors[i].Colors[1].G,
                        playerColors[i].Colors[1].B,
                        (byte)(255 * Percentage)),
                    rotation[i],
                    1f,
                    0.9f,
                    RenderSpriteBlendMode.AlphaBlend,
                    seffects[i]);
            }
            if (Percentage >= 1f)
            {
                InstanceManager.RenderSprite.DrawString(
                    font,
                    mainText,
                    mainTextPos,
                    fontMain);

                InstanceManager.RenderSprite.DrawString(
                    font,
                    mainText,
                    mainTextPos + 2 * Vector2.One,
                    fontMain);

                InstanceManager.RenderSprite.DrawString(
                    font,
                    mainText,
                    mainTextPos + Vector2.One,
                    fontSub);

            }
            base.Draw(gameTime);
        }
        #endregion
    }
}