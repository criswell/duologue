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
using Duologue.State;
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
        private EnemyBuzzsaw floater;
        private Vector2 minMaxX;
        private Vector2 minMaxY;
        private ColorState[] colorStates;
        private int currentColorState;
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
            colorStates = ColorState.GetColorStates();
            currentColorState = 0;
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
            if (device == null)
            {
                device = localGame.GraphicsDevice;
            }
            if (assets == null)
            {
                assets = InstanceManager.AssetManager;
            }
            player = new Player(colorStates[currentColorState]);
            player.Position = new Vector2(
                device.Viewport.Width / 2f,
                device.Viewport.Height / 2f);
            floater = new EnemyBuzzsaw(20, player, colorStates[currentColorState]);
            minMaxX = new Vector2(
                64f, device.Viewport.Width - 64f);
            minMaxY = new Vector2(
                64f, device.Viewport.Height - 64f);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            // Update player position, aim, fire, and orientatio
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

            // Button handling
            if (currentState.Triggers.Left > 0 &&
                lastState.Triggers.Left == 0)
            {
                if (Log != null)
                    Log.LogEntry("Color swap requested");
                player.SwapColors();
            }

            lastState = currentState;

            player.Update(gameTime);
            floater.Update(gameTime, minMaxX, minMaxY);
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
                render = InstanceManager.RenderSprite;
                player.SetRenderSprite(render);
                floater.SetRenderSprite(render);
            }
            
            player.Draw(gameTime);
            floater.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}