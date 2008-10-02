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
using Mimicware.Debug;
using Duologue.PlayObjects;
using Duologue.State;
using Duologue;
using Duologue.UI;

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
        //private Player player;
        private GamePadState lastState;
        private Vector2 motionScaler;
        private Game1 localGame;
        private EnemyBuzzsaw floater;
        private Vector2 minMaxX;
        private Vector2 minMaxY;
        private ColorState[] colorStates;
        private int currentColorState;
        private bool leftFireTriggered;
        private bool rightFireTriggered;
        private int timeSinceExplosion;
        private const int ticksExplosion = 60 * 10;
        private ScoreScroller score;
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
            leftFireTriggered = false;
            rightFireTriggered = false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            motionScaler = new Vector2(10f, 10f);
            timeSinceExplosion = 0;
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

            Color[] playerTints = new Color[LocalInstanceManager.MaxNumberOfPlayers];
            playerTints[0] = Color.LimeGreen;
            playerTints[1] = Color.LightSkyBlue;
            playerTints[2] = Color.LemonChiffon;
            playerTints[3] = Color.Peru;

            LocalInstanceManager.InitializePlayers();
            for (int i=0; i < LocalInstanceManager.MaxNumberOfPlayers; i++)
            {
                LocalInstanceManager.Players[i] =
                    new Player(colorStates[currentColorState], playerTints[i]);
                LocalInstanceManager.Players[i].Position = new Vector2(
                device.Viewport.Width / 2f,
                device.Viewport.Height / 2f);

                LocalInstanceManager.PlayersIndex[i] = (PlayerIndex)i;
                GamePadState tempState = GamePad.GetState((PlayerIndex)i);
                if (tempState.IsConnected)
                    LocalInstanceManager.Players[i].Alive = true;
                else
                    LocalInstanceManager.Players[i].Alive = false;
            }
            LocalInstanceManager.Players[0].Alive = true;

            // See what controllers are active
            floater = new EnemyBuzzsaw(20, colorStates[currentColorState]);
            minMaxX = new Vector2(
                64f, device.Viewport.Width - 64f);
            minMaxY = new Vector2(
                64f, device.Viewport.Height - 64f);

            // Make the score
            score = new ScoreScroller(
                localGame,
                0,
                1f,
                Vector2.Zero,
                Vector2.Zero,
                0,
                1f);
            score.Enabled = true;
            score.Visible = true;
            localGame.Components.Add(score);
            score.Initialize();
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            timeSinceExplosion++;
            for (int i = 0; i < LocalInstanceManager.MaxNumberOfPlayers; i++)
            {
                if (LocalInstanceManager.Players[i].Alive)
                {
                    // Okay, just realized how fucked up this is... the lastState
                    // is updated EVERY time... meaning the states get muddled
                    // FIXME
                    GamePadState currentState = GamePad.GetState(LocalInstanceManager.PlayersIndex[i]);

                    Player player = LocalInstanceManager.Players[i];

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

                        /*if (Log != null)
                            Log.LogEntry("Fire requested");*/
                        player.Fire();
                    }

                    player.Position.X += currentState.ThumbSticks.Left.X * motionScaler.X;
                    player.Position.Y -= currentState.ThumbSticks.Left.Y * motionScaler.Y;

                    // Button handling
                    if (currentState.Triggers.Left > 0 &&
                        !leftFireTriggered)
                    {
                        if (Log != null)
                            Log.LogEntry("Color swap requested");
                        player.SwapColors();
                        leftFireTriggered = true;
                    }
                    else if (currentState.Triggers.Left == 0 &&
                        leftFireTriggered)
                    {
                        leftFireTriggered = false;
                    }

                    // Background cycler
                    if (currentState.Triggers.Right > 0 &&
                        !rightFireTriggered)
                    {
                        if (Log != null)
                            Log.LogEntry("Background swap requested");
                        LocalInstanceManager.Background.NextBackground();
                        rightFireTriggered = true;
                    }
                    else if (currentState.Triggers.Right == 0 &&
                        rightFireTriggered)
                    {
                        rightFireTriggered = false;
                    }

                    //Test the score
                    if (currentState.Buttons.A == ButtonState.Pressed &&
                        lastState.Buttons.A == ButtonState.Released)
                    {
                        score.AddScore(200, LocalInstanceManager.Players[i].Position);
                    }

                    lastState = currentState;

                    player.Update(gameTime);
                }
            }
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
                // FIXME
                // We dont even need render in this test any more
                render = InstanceManager.RenderSprite;
                //player.SetRenderSprite(render);
                //floater.SetRenderSprite(render);
            }

            for (int i =0 ; i < LocalInstanceManager.MaxNumberOfPlayers; i++)
            {
                if(LocalInstanceManager.Players[i].Alive)
                    LocalInstanceManager.Players[i].Draw(gameTime);
                if (timeSinceExplosion > ticksExplosion)
                {
                    timeSinceExplosion = 0;
                    LocalInstanceManager.PlayerRing.AddRing(
                        LocalInstanceManager.Players[i].Position,
                        Color.White);
                }
            }
            floater.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}