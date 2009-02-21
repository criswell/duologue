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
        private const string borderFilename = "logo-border-{0}";
        private const int numberOfFrames = 6;
        private const float yOffset = 200f;
        private const float fadeLifetime = 2f;

        private const double timePerFrame = 0.075;
        #endregion

        #region Fields
        private SpriteObject logoBase;
        //private SpriteObject logoBorder;
        private Texture2D[] logoBorder;
        private Color borderTint;
        private Vector2 center;
        private Vector2 position;
        private float timeSinceStart;
        private double frameTime;
        private int currentFrame;
        #endregion

        #region Properties
        /// <summary>
        /// Tells the percente complete in the fade in
        /// </summary>
        public float PercentComplete
        {
            get { return Math.Min(timeSinceStart / fadeLifetime, 1f); }
        }
        #endregion

        #region Constructor / Init / Load
        public MainGameLogo(Game game)
            : base(game)
        {
            logoBorder = new Texture2D[numberOfFrames];
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            timeSinceStart = 0f;
            frameTime = 0;
            currentFrame = 0;
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

            /*logoBorder = new SpriteObject(
                InstanceManager.AssetManager.LoadTexture2D(borderFilename),
                position,
                new Vector2(
                    InstanceManager.AssetManager.LoadTexture2D(borderFilename).Width / 2f,
                    InstanceManager.AssetManager.LoadTexture2D(borderFilename).Height / 2f),
                null,
                Color.White,
                0f,
                1f,
                0f);*/

            for (int i = 0; i < numberOfFrames; i++)
            {
                logoBorder[i] = InstanceManager.AssetManager.LoadTexture2D(String.Format(
                   borderFilename, i.ToString()));
            }

            center = new Vector2(
                logoBorder[0].Width / 2f,
                logoBorder[0].Height / 2f);
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
                borderTint = c;
            }
            else
            {
                logoBase.Tint = Color.White;
                borderTint = Color.White;
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

            frameTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (frameTime > timePerFrame)
            {
                frameTime = 0;
                currentFrame++;
                if (currentFrame >= numberOfFrames)
                    currentFrame = 0;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (position == Vector2.Zero) {
                position = new Vector2(
                    InstanceManager.DefaultViewport.Width / 2f,
                    yOffset);
                logoBase.Position = position;
                //logoBorder.Position = position;
            }

            SetColors();

            /*InstanceManager.RenderSprite.Draw(
                logoBorder);*/
            InstanceManager.RenderSprite.Draw(
                logoBorder[currentFrame],
                position,
                center,
                null,
                borderTint,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.Addititive);
            InstanceManager.RenderSprite.Draw(
                logoBase,
                RenderSpriteBlendMode.Multiplicative);
            base.Draw(gameTime);
        }
        #endregion
    }
}