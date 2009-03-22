#region File Description
#endregion

#region Using statements
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
using Mimicware;
using Mimicware.Graphics;
using Mimicware.Manager;
using Mimicware.Debug;
using Mimicware.Fx;
#endregion

namespace Duologue
{
    public struct ParallaxElement
    {
        public int Intensity;
        public Color Tint;
        public bool MotionPositive;
        public bool Clouds;
        public bool Debris;
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Background : DrawableGameComponent
    {
        #region Constants
        private const string filename_Background = "background-{0:00}";
        private const string filename_Cloud = "Background/clouds-{0:D2}";
        private const int numBackgrounds = 5;
        private const int numClouds = 2;
        #endregion

        #region Fields
        private RenderSprite render;
        private AssetManager assets;
        private Vector2[] center;
        private Vector2 screenCenter;
        private Game localGame;
        private Texture2D[] backgrounds;
        private int currentBackground;
        private int lastBackground;
        private Color lastColor;
        private Color currentColor;
        private float timeSinceTransitionRequest;

        // Parallax items
        private Texture2D[] texture_Clouds;
        private ParallaxElement currentBottomParallax;
        private ParallaxElement lastBottomParallax;
        private bool bottomParallaxChange;
        private ParallaxElement currentTopParallax;
        private ParallaxElement lastTopParallax;
        private bool topParallaxChange;
        #endregion

        #region Properties
        /// <summary>
        /// How long it takes to transition between backgrounds
        /// </summary>
        public float TransitionTime;

        public bool InTransition
        {
            get { return timeSinceTransitionRequest < TransitionTime; }
        }

        /// <summary>
        /// Details how many possible backgrounds there are
        /// </summary>
        public int NumBackgrounds
        {
            get
            {
                return numBackgrounds;
            }
        }
        #endregion

        #region Constructor / Init / Load
        public Background(Game game)
            : base(game)
        {
            localGame = Game;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            backgrounds = new Texture2D[numBackgrounds];
            center = new Vector2[numBackgrounds];
            base.Initialize();
        }

        /// <summary>
        /// Load the background images
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            if (assets == null)
                assets = InstanceManager.AssetManager;

            for (int i = 0; i < numBackgrounds; i++)
            {
                backgrounds[i] = assets.LoadTexture2D(String.Format(filename_Background, i+1));
                center[i] = new Vector2(backgrounds[i].Width / 2f, backgrounds[i].Height / 2f);
            }

            /*for (int i = 0; i < numClouds; i++)
            {
                texture_Clouds[i] = assets.LoadTexture2D(String.Format(filename_Cloud, i + 1));
            }*/

            currentBackground = 0;
            lastBackground = numBackgrounds - 1;
            TransitionTime = 0.5f;
            timeSinceTransitionRequest = 1f;
            currentColor = Color.White;
            lastColor = Color.White;
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        /// <summary>
        /// Request a switch to the next background
        /// </summary>
        public void NextBackground()
        {
            lastBackground = currentBackground;
            currentBackground++;
            if (currentBackground >= numBackgrounds)
                currentBackground = 0;
            timeSinceTransitionRequest = 0f;
            lastColor = Color.White;
            currentColor = new Color(Vector4.Zero);
        }

        /// <summary>
        /// Set the background and begin a new transition
        /// </summary>
        /// <param name="backgroundNum">The background number to use</param>
        public void SetBackground(int backgroundNum)
        {
            lastBackground = currentBackground;
            if (backgroundNum >= numBackgrounds)
                backgroundNum = numBackgrounds - numBackgrounds % backgroundNum;

            currentBackground = backgroundNum;
            timeSinceTransitionRequest = 0f;
            lastColor = Color.White;
            currentColor = new Color(Vector4.Zero);
        }
        #endregion

        #region Update / Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (InTransition)
            {
                float curAlpha = timeSinceTransitionRequest / TransitionTime;
                float lastAlpha = 1 - curAlpha;
                currentColor = new Color(new Vector4(
                    1f,
                    1f,
                    1f,
                    curAlpha));
                lastColor = new Color(new Vector4(
                    1f,
                    1f,
                    1f,
                    lastAlpha));
                timeSinceTransitionRequest += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (screenCenter == Vector2.Zero)
            {
                screenCenter = new Vector2(
                    InstanceManager.GraphicsDevice.Viewport.Width / 2f,
                    InstanceManager.GraphicsDevice.Viewport.Height / 2f);
                InstanceManager.Logger.LogEntry(screenCenter.ToString());
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (render == null)
                render = InstanceManager.RenderSprite;

            if (InTransition)
            {
                render.Draw(
                    backgrounds[lastBackground],
                    screenCenter,
                    center[lastBackground],
                    null,
                    lastColor,
                    0f,
                    1f,
                    1f);
                render.Draw(
                    backgrounds[currentBackground],
                    screenCenter,
                    center[currentBackground],
                    null,
                    currentColor,
                    0f,
                    1f,
                    1f);
            }
            else
            {
                render.Draw(
                    backgrounds[currentBackground],
                    screenCenter,
                    center[currentBackground],
                    null,
                    Color.White,
                    0f,
                    1f,
                    1f);
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}