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

namespace Duologue.Screens
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MainGameLogo : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string baseFilename = "logo-base";
        private const string borderFilename = "logo-border";
        private const float yOffset = 200f;
        private const float fadeLifetime = 2f;
        #endregion

        #region Fields
        private SpriteObject logoBase;
        private SpriteObject logoBorder;
        private Vector2 position;
        private float timeSinceStart;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public MainGameLogo(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            timeSinceStart = 0f;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            position = Vector2.Zero;
            logoBase = new SpriteObject(
                InstanceManager.AssetManager.LoadTexture2D(baseFilename),
                position,
                new Vector2(
                    InstanceManager.AssetManager.LoadTexture2D(baseFilename).Width / 2f,
                    InstanceManager.AssetManager.LoadTexture2D(baseFilename).Height / 2f),
                null,
                Color.White,
                0f,
                1f,
                0f);

            logoBorder = new SpriteObject(
                InstanceManager.AssetManager.LoadTexture2D(borderFilename),
                position,
                new Vector2(
                    InstanceManager.AssetManager.LoadTexture2D(borderFilename).Width / 2f,
                    InstanceManager.AssetManager.LoadTexture2D(borderFilename).Height / 2f),
                null,
                Color.White,
                0f,
                1f,
                0f);
            base.LoadContent();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Set the colors
        /// </summary>
        private void SetColors()
        {
            if (timeSinceStart < fadeLifetime)
            {
                Color c = new Color(new Vector4(
                    (float)Color.White.R,
                    (float)Color.White.G,
                    (float)Color.White.B,
                    timeSinceStart / fadeLifetime));
                logoBase.Tint = c;
                logoBorder.Tint = c;
            }
            else
            {
                logoBase.Tint = Color.White;
                logoBorder.Tint = Color.White;
            }
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
            if(timeSinceStart < fadeLifetime)
                timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (position == Vector2.Zero) {
                position = new Vector2(
                    InstanceManager.DefaultViewport.Width / 2f,
                    yOffset);
                logoBase.Position = position;
                logoBorder.Position = position;
            }

            SetColors();

            InstanceManager.RenderSprite.Draw(
                logoBorder);
            InstanceManager.RenderSprite.Draw(
                logoBase,
                RenderSpriteBlendMode.Multiplicative);
            base.Draw(gameTime);
        }
        #endregion
    }
}