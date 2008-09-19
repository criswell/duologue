using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Mimicware.Graphics;
using Mimicware.Manager;

namespace Duologue
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GamePlayTest : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields
        private SpriteObject playerBase;
        private SpriteObject playerCannon;
        private SpriteObject playerLight;
        private SpriteObject beam;
        private SpriteObject shot;
        private AssetManager assets;
        private RenderSprite render;
        private GraphicsDevice device;
        #endregion

        #region Properties
        /// <summary>
        /// Write-only property for setting the current asset manager
        /// Must be set before component is added to the game.
        /// </summary>
        public AssetManager AssetManager
        {
            set { assets = value; }
        }

        /// <summary>
        /// Write-only property for setting the current render srpite instance
        /// Must be set before component is added to the game.
        /// </summary>
        public RenderSprite RenderSprite
        {
            set { render = value; }
        }

        /// <summary>
        /// Write-only property for setting the current graphics device
        /// Must be set before component is added to the game.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            set { device = value; }
        }
        #endregion

        public GamePlayTest(Game game)
            : base(game)
        {
            render = null;
            assets = null;
            device = null;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            if (render == null)
                throw new NullReferenceException("The RenderSprite instance was not set before call to Init");
            if (assets == null)
                throw new NullReferenceException("The AssetManager instance was not set before call to Init");
            if (device == null)
                throw new NullReferenceException("The GraphicsDevice instance was not set before call to Init");
            base.Initialize();
        }

        /// <summary>
        /// Load the content
        /// </summary>
        protected override void LoadContent()
        {
            playerBase = new SpriteObject(
                assets.LoadTexture2D("player-base"),
                new Vector2(device.Viewport.Width / 2f, device.Viewport.Height / 2f),
                new Vector2(assets.LoadTexture2D("player-base").Width / 2f, assets.LoadTexture2D("player-base").Height / 2f),
                null,
                Color.Teal,
                0f,
                1f,
                0.5f);

            playerCannon = new SpriteObject(
                assets.LoadTexture2D("player-cannon"),
                new Vector2(device.Viewport.Width / 2f, device.Viewport.Height / 2f),
                new Vector2(assets.LoadTexture2D("player-cannon").Width / 2f, assets.LoadTexture2D("player-cannon").Height / 2f),
                null,
                Color.Red,
                0f,
                1f,
                0.4f);

            playerLight = new SpriteObject(
                assets.LoadTexture2D("player-light"),
                new Vector2(device.Viewport.Width / 2f, device.Viewport.Height / 2f),
                new Vector2(assets.LoadTexture2D("player-light").Width / 2f, assets.LoadTexture2D("player-light").Height / 2f),
                null,
                Color.Blue,
                0f,
                1f,
                0.4f);

            shot = new SpriteObject(
                assets.LoadTexture2D("shot"),
                Vector2.Zero,
                new Vector2(assets.LoadTexture2D("shot").Width / 2f, assets.LoadTexture2D("shot").Height / 2f),
                null,
                Color.Red,
                0f,
                1f,
                1f);

            shot.Alive = false;

            beam = new SpriteObject(
                assets.LoadTexture2D("beam"),
                new Vector2(device.Viewport.Width / 2f, device.Viewport.Height / 2f),
                new Vector2(971f, 254f),
                null,
                Color.Blue - new Color((byte)0, (byte)0, (byte)0, (byte)100),
                0f,
                1f,
                1f);

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game component
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}