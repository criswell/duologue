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
        private const string AbuttonFilename = "PlayerUI\\buttonA";
        private const string controllerButtonsFilename = "PlayerUI\\controller-buttons";
        private const string controllerFaceFilename = "PlayerUI\\controller-face";
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
        private Vector2[] offsetModifiers;
        private Vector2 centerOfButton;

        // Players
        private bool[] activePlayers;
        private bool[] controllerPluggedIn;
        private SignedInGamer[] signedInGamers;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the current list of active players
        /// </summary>
        public bool[] ActivePlayers
        {
            get { return activePlayers; }
        }

        /// <summary>
        /// Set when the screen is alive
        /// </summary>
        public bool Alive;
        #endregion

        #region Constructor/Init/load
        public PlayerSelect(Game game)
            : base(game)
        {
            centerOfScreen = Vector2.Zero;
            playerBases = new Texture2D[InputManager.MaxInputs];
            activePlayers = new bool[InputManager.MaxInputs];
            controllerPluggedIn = new bool[InputManager.MaxInputs];
            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
            SignedInGamer.SignedOut += new EventHandler<SignedOutEventArgs>(SignedInGamer_SignedOut);
            offsetModifiers = new Vector2[InputManager.MaxInputs];
            Alive = false;
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

            // Set up the offset modifiers
            offsetModifiers[0] = new Vector2(-1f, -1f);
            offsetModifiers[1] = new Vector2(1f, -1f);
            offsetModifiers[2] = new Vector2(-1f, 1f);
            offsetModifiers[3] = new Vector2(1f, 1f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = InstanceManager.AssetManager.LoadSpriteFont(fontFilename);
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
        /// <summary>
        /// Get the initial list of signed in gamers and register callbacks
        /// </summary>
        private void UpdateSignedInGamers()
        {
            SignedInGamerCollection signedIn = Gamer.SignedInGamers;

            signedInGamers = new SignedInGamer[InputManager.MaxInputs];
            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                signedInGamers[i] = signedIn[(PlayerIndex)i];
            }
        }

        /// <summary>
        /// Local drawstring for putting borders around our fonts
        /// </summary>
        private void DrawString(string p, Vector2 vector2, Color color1, Color color2)
        {
            for(int i = 0; i < offsetModifiers.Length; i++)
                InstanceManager.RenderSprite.DrawString(
                    font,
                    p,
                    vector2 - offsetModifiers[i]* Vector2.One,
                    color2);

            InstanceManager.RenderSprite.DrawString(
                font,
                p,
                vector2,
                color1);
        }

        /// <summary>
        /// Local draw wrapper for putting borders around our images
        /// </summary>
        private void DrawTexture(Texture2D texture, Vector2 vector2, Vector2 center, Color color, Color color2)
        {
            for (int i = 0; i < offsetModifiers.Length; i++)
                InstanceManager.RenderSprite.Draw(
                    texture,
                    vector2 - offsetModifiers[i]*Vector2.One,
                    center,
                    null,
                    color2,
                    0f,
                    1f,
                    0.5f);

            InstanceManager.RenderSprite.Draw(
                texture,
                vector2,
                center,
                null,
                color,
                0f,
                1f,
                0.5f);
        }
        #endregion

        #region Public methods
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle the signed out event
        /// </summary>
        void SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
        {
            int i = (int)e.Gamer.PlayerIndex;

            signedInGamers[i] = null;
            
            // I'm not certain if we should remove them from the game or not,
            // but for not I'm assuming we should
            activePlayers[i] = false;
        }

        /// <summary>
        /// Handle the signed in event
        /// </summary>
        void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            int i = (int)e.Gamer.PlayerIndex;

            signedInGamers[i] = e.Gamer;
        }
        #endregion

        #region Update /Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (Alive & !Guide.IsVisible)
            {
                // Get center of screen if we don't already have it
                if (centerOfScreen == Vector2.Zero)
                {
                    centerOfScreen = new Vector2(
                        InstanceManager.DefaultViewport.Width / 2f,
                        InstanceManager.DefaultViewport.Height / 2f);

                    selectOffset = centerOfScreen / 2f;
                }

                // Get the current signed in gamers
                if (signedInGamers == null)
                    UpdateSignedInGamers();

                // Cycle through the inputs, seeing if their status has changed
                for (int i = 0; i < InputManager.MaxInputs; i++)
                {
                    controllerPluggedIn[i] = InstanceManager.InputManager.LastGamePadStates[i].IsConnected;
                    if (activePlayers[i])
                    {
                        // Check for disconnect or cancel
                        if (!InstanceManager.InputManager.CurrentGamePadStates[i].IsConnected &&
                            InstanceManager.InputManager.LastGamePadStates[i].IsConnected)
                        {
                            activePlayers[i] = false;
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
                            if (signedInGamers[i] != null)
                            {
                                // We're signed in, make us active
                                activePlayers[i] = true;
                            }
                            else
                            {
                                // We're not signed in, launch the interface
                                Guide.ShowSignIn(1, false);
                            }
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Alive)
            {
                for (int i = 0; i < InputManager.MaxInputs; i++)
                {
                    if (activePlayers[i])
                    {
                        // Player is active
                    }
                    else if (controllerPluggedIn[i])
                    {
                        // Controller is plugged in, want them to press A
                        Vector2 size = font.MeasureString(Resources.PlayerSelect_PressA);
                        DrawTexture(
                            aButton,
                            centerOfScreen + offsetModifiers[i] * selectOffset - centerOfButton,
                            centerOfButton,
                            Color.White,
                            Color.Black);
                        DrawString(
                            Resources.PlayerSelect_PressA,
                            centerOfScreen + offsetModifiers[i] * selectOffset + new Vector2(
                                0f, centerOfButton.Y) - size / 2f,
                            playerColors[i].Colors[0],
                            Color.Black);
                    }
                    else
                    {
                        // Display default sign in request
                        Vector2 size = font.MeasureString(Resources.PlayerSelect_ConnectController);
                        DrawString(
                            Resources.PlayerSelect_ConnectController,
                            centerOfScreen + offsetModifiers[i] * selectOffset - size / 2f,
                            playerColors[i].Colors[0],
                            Color.Black);
                    }
                }
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}