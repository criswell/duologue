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
using Duologue.Waves;
using Duologue.UI;
using Duologue.State;
#endregion

namespace Duologue.UI
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GameOver : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const float scaleTime = 3f;
        private const float startScale = 100f;
        private const float endScale = 1f;
        #endregion

        #region Fields
        private string filename_gameOver;
        private Texture2D gameOverTexture;
        private Vector2 gameOverCenter;
        private Vector2 screenCenter;
        private float gameOverRotation;
        private float gameOverScale;
        private GamePlayScreenManager myManager;
        private float timeSinceStart;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public GameOver(Game game, GamePlayScreenManager manager)
            : base(game)
        {
            filename_gameOver = Resources.Filename_GameOver;
            myManager = manager;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            Reset();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            gameOverTexture = InstanceManager.AssetManager.LoadTexture2D(filename_gameOver);
            gameOverCenter = new Vector2(
                gameOverTexture.Width / 2f,
                gameOverTexture.Height / 2f);

            screenCenter = Vector2.Zero;
            base.LoadContent();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Call to reset the game over screen
        /// </summary>
        public void Reset()
        {
            timeSinceStart = 0f;
            gameOverScale = startScale;
            gameOverRotation = 0f;
        }
        #endregion

        #region Draw & Update
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;

            gameOverScale = startScale * (timeSinceStart / scaleTime) + endScale;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (screenCenter == Vector2.Zero)
                screenCenter = new Vector2(
                    InstanceManager.DefaultViewport.Width / 2f,
                    InstanceManager.DefaultViewport.Height / 2f);

            InstanceManager.RenderSprite.Draw(
                gameOverTexture,
                screenCenter,
                gameOverCenter,
                null,
                Color.White,
                gameOverRotation,
                gameOverScale,
                1f,
                RenderSpriteBlendMode.AlphaBlendTop);

            base.Draw(gameTime);
        }
        #endregion
    }
}