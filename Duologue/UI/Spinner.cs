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
using Mimicware.Graphics;
using Mimicware.Manager;
// Duologue
using Duologue;
#endregion

namespace Duologue.UI
{
    public enum SpinnerState
    {
        Normal,
        Filler,
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Spinner : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string baseFilename = "PlayerUI/spinner-base";
        private const string trackerFilename = "PlayerUI/spinner-tracker";
        private const string shadowFilename = "PlayerUI/spinner-shadow";

        private const float defaultTickTime = 0.1f;

        private const float rotationDelta = MathHelper.PiOver4;
        private const float defaultLayer = 0.8f;
        #endregion

        #region Fields
        // Textures
        private Texture2D spinnerBase;
        private Texture2D spinnerTracker;
        private Texture2D spinnerShadow;

        // housekeeping
        private Vector2 center;
        private Vector2 shadowCenter;
        private float rotation;
        private SpinnerState currentState;
        private Game localGame;
        private Vector2[] offsetModifiers;

        // Timer stuff
        private float timeSinceTick;
        #endregion

        #region Properties
        /// <summary>
        /// The position of the spinner
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The color of the base
        /// </summary>
        public Color BaseColor;

        /// <summary>
        /// The color of the tracker
        /// </summary>
        public Color TrackerColor;

        /// <summary>
        /// The total time for each tick
        /// </summary>
        public float TotalTickTime;

        /// <summary>
        /// Gets the percentage of the current tick
        /// </summary>
        public float Percentage
        {
            get { return MathHelper.Min(timeSinceTick / TotalTickTime, 1f); }
        }

        /// <summary>
        /// Any scaling for the spinner
        /// </summary>
        public Vector2 Scale;

        /// <summary>
        /// The layer we should draw at
        /// </summary>
        public float Layer;

        /// <summary>
        /// Any text you want to have displayed over the spinner
        /// </summary>
        public string DisplayText;

        /// <summary>
        /// Font to use
        /// </summary>
        public SpriteFont DisplayFont;

        /// <summary>
        /// The font color
        /// </summary>
        public Color FontColor;

        /// <summary>
        /// The font's shadow color
        /// </summary>
        public Color FontShadowColor;
        #endregion

        #region Constructor / Init / Load
        public Spinner(Game game)
            : base(game)
        {
            localGame = game;
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
            // Some sensible defaults
            Position = Vector2.Zero;
            BaseColor = Color.BlueViolet;
            TrackerColor = Color.Azure;
            TotalTickTime = defaultTickTime;
            timeSinceTick = 0f;
            rotation = 0f;
            currentState = SpinnerState.Normal;
            Scale = Vector2.One;
            Layer = defaultLayer;
            DisplayText = null;
            DisplayFont = null;
            FontColor = Color.Black;
            FontShadowColor = Color.Black;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spinnerBase = InstanceManager.AssetManager.LoadTexture2D(baseFilename);
            spinnerTracker = InstanceManager.AssetManager.LoadTexture2D(trackerFilename);
            center = new Vector2(
                spinnerBase.Width / 2f,
                spinnerBase.Height / 2f);

            spinnerShadow = InstanceManager.AssetManager.LoadTexture2D(shadowFilename);
            shadowCenter = new Vector2(
                spinnerShadow.Width / 2f,
                spinnerShadow.Height / 2f);
            base.LoadContent();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Trigger the begining of the next tick
        /// </summary>
        private void TriggerNextTick()
        {
            timeSinceTick = 0f;
            rotation += rotationDelta;
            if (rotation > MathHelper.TwoPi)
            {
                rotation = 0f;
                switch (currentState)
                {
                    case SpinnerState.Normal:
                        currentState = SpinnerState.Filler;
                        break;
                    default:
                        currentState = SpinnerState.Normal;
                        break;
                }
            }
        }

        /// <summary>
        /// Local drawstring for putting borders around our fonts
        /// </summary>
        private void DrawString(Vector2 vector2, Vector2 c)
        {
            for (int i = 0; i < offsetModifiers.Length; i++)
                InstanceManager.RenderSprite.DrawString(
                    DisplayFont,
                    DisplayText,
                    vector2 - offsetModifiers[i] * Vector2.One,
                    FontShadowColor,
                    Scale,
                    c);

            InstanceManager.RenderSprite.DrawString(
                DisplayFont,
                DisplayText,
                vector2,
                FontColor,
                Scale,
                c);
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
            timeSinceTick += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Percentage >= 1f)
                TriggerNextTick();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            InstanceManager.RenderSprite.Draw(
                spinnerShadow,
                Position,
                shadowCenter,
                null,
                Color.White,
                0f,
                Scale,
                Layer);

            InstanceManager.RenderSprite.Draw(
                spinnerBase,
                Position,
                center,
                null,
                BaseColor,
                0f,
                Scale,
                Layer);

            if (currentState == SpinnerState.Filler)
            {
                for (float i = 0f; i < rotation; i += rotationDelta)
                {
                    InstanceManager.RenderSprite.Draw(
                        spinnerTracker,
                        Position,
                        center,
                        null,
                        TrackerColor,
                        i,
                        Scale,
                        Layer);
                }
            }

            InstanceManager.RenderSprite.Draw(
                spinnerTracker,
                Position,
                center,
                null,
                TrackerColor,
                rotation,
                Scale,
                Layer);
            base.Draw(gameTime);

            // Draw any text
            if (DisplayText != null && DisplayFont != null)
            {
                Vector2 size = DisplayFont.MeasureString(DisplayText);
                DrawString(Position, size/2f);
            }
        }
        #endregion
    }
}