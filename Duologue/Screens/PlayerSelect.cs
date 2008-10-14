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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PlayerSelect : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string fontFilename = "Fonts\\inero-28";
        private const string AbuttonFilename = "PlayerUI\\buttonA.png";
        private const string controllerButtonsFilename = "PlayerUI\\controller-buttons";
        private const string controllerFaceFilename = "PlayerUI\\controller-fase";
        private const string playerRootFilename = "PlayerUI\\player-root";
        private const string playerBaseFilename = "PlayerUI\\P{0}-base";
        #endregion

        #region Fields
        // Textures
        private Texture2D aButton;
        private Texture2D controllerButtons;
        private Texture2D controllerFace;
        private Texture2D playerRoot;
        private Texture2D[] playerBases;

        // Fonts
        private SpriteFont font;

        // Player colors
        private PlayerColors[] playerColors;

        // Positions
        private Vector2 centerOfScreen;
        private Vector2 centerOfPlayer;
        private Vector2 selectOffset;
        private Vector2 centerOfButton;

        // Players
        private bool[] activePlayers;
        private bool[] controllerPluggedIn;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the current list of active players
        /// </summary>
        public bool[] ActivePlayers
        {
            get { return activePlayers; }
        }
        #endregion

        #region Constructor/Init/load
        public PlayerSelect(Game game)
            : base(game)
        {
            playerBases = new Texture2D[InputManager.MaxInputs];
            activePlayers = new bool[InputManager.MaxInputs];
            controllerPluggedIn = new bool[InputManager.MaxInputs];
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            playerColors = PlayerColors.GetPlayerColors();

            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                activePlayers[i] = false;
                controllerPluggedIn[i] = false;
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            aButton = InstanceManager.AssetManager.LoadTexture2D(AbuttonFilename);
            centerOfButton = new Vector2(
                aButton.Width / 2f,
                aButton.Height / 2f);

            controllerButtons = InstanceManager.AssetManager.LoadTexture2D(controllerButtonsFilename);
            controllerFace = InstanceManager.AssetManager.LoadTexture2D(controllerFaceFilename);
            playerRoot = InstanceManager.AssetManager.LoadTexture2D(playerRootFilename);
            centerOfPlayer = new Vector2(
                playerRoot.Width / 2f,
                playerRoot.Height / 2f);

            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                playerBases[i] =
                    InstanceManager.AssetManager.LoadTexture2D(String.Format(playerBaseFilename, i+1));
            }

            base.LoadContent();
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        #endregion

        #region Update /Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Cycle through the inputs, seeing if their status has changed
            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                if (activePlayers[i])
                {
                    // Check for disconnect or cancel
                    if (!InstanceManager.InputManager.CurrentGamePadStates[i].IsConnected &&
                        InstanceManager.InputManager.LastGamePadStates[i].IsConnected)
                    {
                        activePlayers[i] = false;
                        controllerPluggedIn[i] = false;
                    }
                }
                else
                {
                    if (InstanceManager.InputManager.CurrentGamePadStates[i].IsConnected &&
                        !InstanceManager.InputManager.LastGamePadStates[i].IsConnected)
                    {
                        controllerPluggedIn[i] = true;
                    }

                    if (InstanceManager.InputManager.CurrentGamePadStates[i].Buttons.A == ButtonState.Pressed &&
                        InstanceManager.InputManager.LastGamePadStates[i].Buttons.A == ButtonState.Released)
                    {
                        // CHECK FOR SIGN IN
                        // ERE I AM JH
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            
            base.Draw(gameTime);
        }
        #endregion
    }
}