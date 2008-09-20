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
using Duologue.PlayObjects;
using Mimicware.Debug;

namespace Duologue
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GamePlayTest : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields
        private AssetManager assets;
        private RenderSprite render;
        private GraphicsDevice device;
        private Player player;
        private GamePadState lastState;
        private Vector2 motionScaler;
        private Game1 localGame;
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
        public GraphicsDevice Device
        {
            set { device = value; }
        }

        /// <summary>
        /// Set to enable logging
        /// </summary>
        public Logger Log;
        #endregion

        public GamePlayTest(Game game)
            : base(game)
        {
            localGame = (Game1)game;
            render = null;
            assets = null;
            device = null;
            Log = null;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            motionScaler = new Vector2(10f, 10f);
            base.Initialize();
        }

        /// <summary>
        /// Load the content
        /// </summary>
        protected override void LoadContent()
        {
            /*if (render == null)
                throw new NullReferenceException("The RenderSprite instance was not set before call to Init");
            if (assets == null)
                throw new NullReferenceException("The AssetManager instance was not set before call to Init");
            if (device == null)
                throw new NullReferenceException("The GraphicsDevice instance was not set before call to Init");*/

            if (device == null)
            {
                device = localGame.GraphicsDevice;
            }
            if (assets == null)
            {
                assets = localGame.Assets;
            }
            player = new Player(assets, device, render);
            player.Position = new Vector2(
                device.Viewport.Width / 2f,
                device.Viewport.Height / 2f);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            // Update player position, aim, and orientation
            if (currentState.ThumbSticks.Left.X != 0f ||
                currentState.ThumbSticks.Left.Y != 0f)
            {
                player.Orientation.X = currentState.ThumbSticks.Left.X;
                player.Orientation.Y = -1 * currentState.ThumbSticks.Left.Y;
            }
            if (currentState.ThumbSticks.Right.X != 0f ||
                currentState.ThumbSticks.Right.Y != 0f)
            {
                player.Aim.X = currentState.ThumbSticks.Right.X;
                player.Aim.Y = currentState.ThumbSticks.Right.Y * -1;
                
                if (Log != null)
                    Log.LogEntry("Fire requested");
                player.Fire();
            }

            player.Position.X += currentState.ThumbSticks.Left.X * motionScaler.X;
            player.Position.Y -= currentState.ThumbSticks.Left.Y * motionScaler.Y;

            if (currentState.Triggers.Left > 0 &&
                lastState.Triggers.Left == 0)
            {
                player.SwapColors();
            }

            lastState = currentState;

            player.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game component
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            if (render == null)
            {
                render = localGame.Render;
                player.RenderSprite = render;
                player.Shot.RenderSprite = render;
            }

            //if (Log != null)
            //    Log.LogEntry("O:" + player.Orientation.ToString() + ", TS.L: " + lastState.ThumbSticks.Left.ToString() + ", BR: " + player.BaseRotation.ToString());
            
            player.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}