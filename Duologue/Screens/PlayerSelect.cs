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
using Duologue.State;
#endregion

namespace Duologue.Screens
{
    public enum PlayerSelectState
    {
        PlayerSelect,
        Countdown,
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PlayerSelect : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const int maxTimer = 5;
        private const float timeCountdown = 1f;
        private const float ScaleOffset = 1f;
        private const float ScaleFactor = 0.25f;

        private const string fontFilename = "Fonts\\inero-28";
        private const string countdownFontFilename = "Fonts\\inero-50";
        private const string AbuttonFilename = "PlayerUI\\buttonA";
        #endregion

        #region Fields
        // Textures
        private Texture2D aButton;

        // Fonts
        private SpriteFont font;
        private SpriteFont countdownFont;

        // Player colors
        private PlayerColors[] playerColors;

        // Positions
        private Vector2 centerOfScreen;
        private Vector2 selectOffset;
        private Vector2[] offsetModifiers;
        private Vector2 centerOfButton;

        // Players
        private bool[] activePlayers;
        private bool[] controllerPluggedIn;
        private SignedInGamer[] signedInGamers;
        private GamerProfile[] profiles;

        // Counters and misc
        private float timeSinceStart;
        private int numActive;
        private PlayerSelectState currentState;
        private int currentCountdown;
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

        /// <summary>
        /// The percentage of the countdown click
        /// </summary>
        public float Percentage
        {
            get { return MathHelper.Min(1f, timeSinceStart / timeCountdown); }
        }

        /// <summary>
        /// Returns the current scaling factor based on percentage
        /// </summary>
        public float Scale
        {
            get
            {
                return ScaleOffset + ScaleFactor*(float)Math.Cos((double)(Percentage * MathHelper.TwoPi));
            }
        }
        #endregion

        #region Constructor/Init/load
        public PlayerSelect(Game game)
            : base(game)
        {
            centerOfScreen = Vector2.Zero;
            activePlayers = new bool[InputManager.MaxInputs];
            controllerPluggedIn = new bool[InputManager.MaxInputs];
            signedInGamers = new SignedInGamer[InputManager.MaxInputs];
            profiles = new GamerProfile[InputManager.MaxInputs];
            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
            SignedInGamer.SignedOut += new EventHandler<SignedOutEventArgs>(SignedInGamer_SignedOut);
            offsetModifiers = new Vector2[InputManager.MaxInputs];
            // Set up the offset modifiers
            offsetModifiers[0] = new Vector2(-1f, -1f);
            offsetModifiers[1] = new Vector2(1f, -1f);
            offsetModifiers[2] = new Vector2(-1f, 1f);
            offsetModifiers[3] = new Vector2(1f, 1f);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            Alive = false;
            numActive = 0;

            playerColors = PlayerColors.GetPlayerColors();

            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                activePlayers[i] = false;
                controllerPluggedIn[i] = false;
            }

            currentState = PlayerSelectState.PlayerSelect;
            currentCountdown = maxTimer;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = InstanceManager.AssetManager.LoadSpriteFont(fontFilename);
            countdownFont = InstanceManager.AssetManager.LoadSpriteFont(countdownFontFilename);
            aButton = InstanceManager.AssetManager.LoadTexture2D(AbuttonFilename);
            centerOfButton = new Vector2(
                aButton.Width / 2f,
                aButton.Height / 2f);

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

            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                signedInGamers[i] = signedIn[(PlayerIndex)i];
                if (signedInGamers[i] != null)
                {
                    // Yeah, probably bad if we ever do network multiplayer
                    profiles[i] = signedInGamers[i].GetProfile();
                }
            }
        }

        /// <summary>
        /// Local drawstring for putting borders around our fonts
        /// </summary>
        private void DrawString(string p, Vector2 vector2, Color color1, Color color2)
        {
            // FIXME
            // We now have an actual method for this in rendersprite we should use instead
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

        /// <summary>
        /// Was there a back request?
        /// </summary>
        private bool BackRequest(int i)
        {
            // MEIN EYES! DEY BLEED!
            return (InstanceManager.InputManager.NewButtonPressed(Buttons.B, (PlayerIndex)i)
                    ||
                   InstanceManager.InputManager.NewButtonPressed(Buttons.Back, (PlayerIndex)i));
        }

        /// <summary>
        /// Trigger a move back to the main menu
        /// </summary>
        private void TriggerBack()
        {
            if (numActive < 1)
            {
                LocalInstanceManager.CurrentGameState = GameState.MainMenuSystem;
                LocalInstanceManager.NextGameState = GameState.MainMenuSystem;
            }

        }

        /// <summary>
        /// Trigger the countdown
        /// </summary>
        private void TriggerCountdown()
        {
            currentCountdown = maxTimer;
            currentState = PlayerSelectState.Countdown;
            LocalInstanceManager.Spinner.Position = centerOfScreen;
            // FIXME
            // These colors probably shouldn't be hardcoded
            LocalInstanceManager.Spinner.BaseColor = Color.Red;
            LocalInstanceManager.Spinner.TrackerColor = new Color(new Vector4(0f, 252f, 255f, 255f));
            LocalInstanceManager.Spinner.DisplayFont = countdownFont;
            LocalInstanceManager.Spinner.FontColor = Color.Violet;
            LocalInstanceManager.Spinner.FontShadowColor = Color.DarkBlue;
            LocalInstanceManager.Spinner.Enabled = true;
            LocalInstanceManager.Spinner.Visible = true;
            timeSinceStart = 0f;
        }

        /// <summary>
        /// Void any active countdown, or otherwise just ensure it's not
        /// happening.
        /// </summary>
        private void VoidCountdown()
        {
            currentState = PlayerSelectState.PlayerSelect;
            LocalInstanceManager.Spinner.Enabled = false;
            LocalInstanceManager.Spinner.Visible = false;
            LocalInstanceManager.Spinner.Initialize();
        }

        /// <summary>
        /// Triggers a tick
        /// </summary>
        private void TriggerTick()
        {
            timeSinceStart = 0f;
            if (currentState == PlayerSelectState.Countdown)
            {
                currentCountdown--;

                if (currentCountdown < 0)
                {
                    currentCountdown = 0;

                    TriggerNext();
                }
            }
        }

        /// <summary>
        /// Trigger the next screen
        /// </summary>
        private void TriggerNext()
        {
            // Reset the countdown
            VoidCountdown();
            if (numActive > 0 && LocalInstanceManager.CurrentGameState == GameState.PlayerSelect)
            {
                LocalInstanceManager.CurrentGameState = LocalInstanceManager.NextGameState;
                // Set the players
                SetPlayers();
            }
        }

        /// <summary>
        /// Called at the end after we're finished to set the players up
        /// </summary>
        private void SetPlayers()
        {
            if (LocalInstanceManager.Players == null)
                LocalInstanceManager.InitializePlayers();

            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                LocalInstanceManager.Players[i].Active = activePlayers[i];
                if (activePlayers[i])
                    LocalInstanceManager.Players[i].Initialize(
                        playerColors[i],
                        (PlayerIndex)i,
                        signedInGamers[i],
                        profiles[i],
                        ColorState.GetColorStates()[0],
                        GetPlayerStartPos(i),
                        offsetModifiers[i]);
            }
        }

        /// <summary>
        /// Gets the player start position
        /// </summary>
        /// <param name="i">The player's index</param>
        /// <returns>A vector describing the current player's start position</returns>
        private Vector2 GetPlayerStartPos(int i)
        {
            // FIXME
            // Wow, fixme... bad hard-coded bullshit values
            // just used to get the thing going
            Vector2 screenCenter = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f,
                InstanceManager.DefaultViewport.Height / 2f);
            float offset = 100f;
            return screenCenter + offset * offsetModifiers[i];
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
            profiles[i] = null;

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
            // Yeah, sucky if networked multiplayer... I know
            profiles[i] = signedInGamers[i].GetProfile();
        }
        #endregion

        #region Update /Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (Percentage < 1f)
                timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                TriggerTick();

            if (currentState == PlayerSelectState.Countdown)
            {
                LocalInstanceManager.Spinner.DisplayText = currentCountdown.ToString();
                LocalInstanceManager.Spinner.Scale = new Vector2(Scale);
            }

            if (Alive & !Guide.IsVisible)
            {
                numActive = 0;

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
                        numActive++;
                        // Check for disconnect or cancel
                        if ((!InstanceManager.InputManager.CurrentGamePadStates[i].IsConnected &&
                            InstanceManager.InputManager.LastGamePadStates[i].IsConnected) ||
                            BackRequest(i))
                        {
                            activePlayers[i] = false;
                            numActive--;
                            VoidCountdown();
                        }
                        else if(InstanceManager.InputManager.NewButtonPressed(Buttons.Start, (PlayerIndex)i))
                        {
                            if (currentState != PlayerSelectState.Countdown)
                                TriggerCountdown();
                            else
                                TriggerTick();
                        }
                    }
                    else
                    {
                        if (InstanceManager.InputManager.NewButtonPressed(Buttons.A, (PlayerIndex)i))
                        {
                            if (signedInGamers[i] != null)
                            {
                                // We're signed in, make us active
                                activePlayers[i] = true;
                                numActive++;
                            }
                            else
                            {
                                // We're not signed in, launch the interface
                                Guide.ShowSignIn(1, false);
                                break;
                            }
                        }
                        else if (BackRequest(i))
                        {
                            // Cancel back to main menu
                            VoidCountdown();
                            TriggerBack();
                        }

                    }
                }

                if (InstanceManager.InputManager.KeyPressed(Keys.Escape))
                    TriggerBack();
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
                        Vector2 gamertagSize = font.MeasureString(signedInGamers[i].Gamertag);
                        Texture2D pic = profiles[i].GamerPicture;

                        Vector2 boxCenter = new Vector2(
                            (gamertagSize.X + pic.Width)/2f,
                            MathHelper.Max(gamertagSize.Y, (float)pic.Height)/2f);

                        DrawTexture(
                            pic,
                            centerOfScreen + offsetModifiers[i] * selectOffset - boxCenter,
                            new Vector2(pic.Width/2f,pic.Height/2f),
                            Color.White,
                            Color.Black);

                        DrawString(
                            signedInGamers[i].Gamertag,
                            centerOfScreen + offsetModifiers[i] * selectOffset - boxCenter +
                            new Vector2(
                                pic.Width,
                                -1 * gamertagSize.Y / 2f),
                            playerColors[i].Colors[0],
                            Color.Black);

                        if (currentState == PlayerSelectState.PlayerSelect)
                        {
                            Vector2 size = font.MeasureString(Resources.PlayerSelect_PressStart);
                            DrawString(
                                Resources.PlayerSelect_PressStart,
                                centerOfScreen + offsetModifiers[i] * selectOffset -
                                new Vector2(
                                    size.X / 2f,
                                    -1 * size.Y / 2f),
                                playerColors[i].Colors[0],
                                Color.Black);
                        }
                        else
                        {
                            Vector2 size = font.MeasureString(Resources.PlayerSelect_CountdownPressB);
                            DrawString(
                                Resources.PlayerSelect_CountdownPressB,
                                centerOfScreen + offsetModifiers[i] * selectOffset -
                                new Vector2(
                                    size.X / 2f,
                                    -1 * size.Y / 2f),
                                playerColors[i].Colors[0],
                                Color.Black);
                        }
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
                    else if(currentState == PlayerSelectState.PlayerSelect)
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