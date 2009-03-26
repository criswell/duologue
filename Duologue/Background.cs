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
        public float Speed;
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
        private const int numClouds = 3;
        private const int totalNumPossibleLayers = 4;
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
        private ParallaxElement emptyParallax;
        private double timeSinceTopChange;
        private double timeSinceBottomChange;

        private int[] cloudLayers;
        private Vector2[] center_TopClouds;
        private Vector2[] center_BottomClouds;
        private float[] cloudLayerSpeedOffsets;
        private float[] cloudLayerAlphaModifiers;
        #endregion

        #region Properties
        /// <summary>
        /// How long it takes to transition between backgrounds
        /// </summary>
        public float TransitionTime;

        /// <summary>
        /// The transition time for parallax changes
        /// </summary>
        public double ParallaxTransitionTime;

        public bool InTransition
        {
            get { return timeSinceTransitionRequest < TransitionTime; }
        }

        public float TopFadeAlpha
        {
            get { return (float)timeSinceTopChange / TransitionTime; }
        }
        public float BottomFadeAlpha
        {
            get { return (float)timeSinceBottomChange / TransitionTime; }
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

        /// <summary>
        /// An empty parallax element
        /// </summary>
        public ParallaxElement EmptyParallaxElement
        {
            get { return emptyParallax; }
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
            texture_Clouds = new Texture2D[numClouds];

            bottomParallaxChange = false;
            topParallaxChange = false;

            emptyParallax = new ParallaxElement();
            emptyParallax.Clouds = false;
            emptyParallax.Debris = false;
            emptyParallax.Intensity = 0;
            emptyParallax.Speed = 0f;
            emptyParallax.Tint = Color.TransparentWhite;

            currentBottomParallax = emptyParallax;
            currentTopParallax = emptyParallax;
            lastBottomParallax = emptyParallax;
            lastTopParallax = emptyParallax;

            base.Initialize();
        }

        /// <summary>
        /// Load the background images
        /// </summary>
        protected override void LoadContent()
        {
            if (assets == null)
                assets = InstanceManager.AssetManager;

            for (int i = 0; i < numBackgrounds; i++)
            {
                backgrounds[i] = assets.LoadTexture2D(String.Format(filename_Background, i+1));
                center[i] = new Vector2(backgrounds[i].Width / 2f, backgrounds[i].Height / 2f);
            }

            for (int i = 0; i < numClouds; i++)
            {
                texture_Clouds[i] = assets.LoadTexture2D(String.Format(filename_Cloud, i + 1));
            }

            #region Layer definitions (hard coded bullshit)
            // Set up the layer stuff- lots of hardcoded nastiness, can't be helped
            cloudLayers = new int[totalNumPossibleLayers];
            cloudLayerAlphaModifiers = new float[totalNumPossibleLayers];
            cloudLayerSpeedOffsets = new float[totalNumPossibleLayers];
            center_TopClouds = new Vector2[totalNumPossibleLayers];
            center_BottomClouds = new Vector2[totalNumPossibleLayers];

            cloudLayers[0] = 1;
            center_TopClouds[0] = Vector2.Zero;//new Vector2(0, (float)texture_Clouds[cloudLayers[0]].Height);
            center_BottomClouds[0] = new Vector2(0, (float)texture_Clouds[cloudLayers[0]].Height);
            cloudLayerAlphaModifiers[0] = 0.25f;
            cloudLayerSpeedOffsets[0] = 1f;

            cloudLayers[1] = 0;
            center_TopClouds[1] = new Vector2(0, texture_Clouds[cloudLayers[1]].Height * 0.2f);
            center_BottomClouds[1] = new Vector2(0, (float)texture_Clouds[cloudLayers[1]].Height - texture_Clouds[cloudLayers[1]].Height * 0.2f);
            cloudLayerAlphaModifiers[1] = 0.5f;
            cloudLayerSpeedOffsets[1] = 5f;

            cloudLayers[2] = 0;
            center_TopClouds[2] = new Vector2(0, texture_Clouds[cloudLayers[2]].Height * 0.6f);
            center_BottomClouds[2] = new Vector2(0, (float)texture_Clouds[cloudLayers[2]].Height - texture_Clouds[cloudLayers[2]].Height * 0.6f);
            cloudLayerAlphaModifiers[2] = 0.70f;
            cloudLayerSpeedOffsets[2] = 7f;

            cloudLayers[3] = 2;
            center_TopClouds[3] = new Vector2(0, texture_Clouds[cloudLayers[3]].Height*0.4f);
            center_BottomClouds[3] = new Vector2(0, (float)texture_Clouds[cloudLayers[3]].Height - texture_Clouds[cloudLayers[3]].Height * 0.4f);
            cloudLayerAlphaModifiers[3] = 0.5f;
            cloudLayerSpeedOffsets[3] = 9f;
            #endregion

            currentBackground = 0;
            lastBackground = numBackgrounds - 1;
            TransitionTime = 0.5f;
            ParallaxTransitionTime = 0.25f;
            timeSinceTransitionRequest = 1f;
            currentColor = Color.White;
            lastColor = Color.White;
            timeSinceBottomChange = 0;
            timeSinceTopChange = 0;
            base.LoadContent();
        }
        #endregion

        #region Private methods
        private void UpdateParallax(GameTime gameTime)
        {
            // Update the center offsets
            for (int i = 0; i < totalNumPossibleLayers; i++)
            {
                center_TopClouds[i] += Vector2.UnitX * currentTopParallax.Speed * cloudLayerSpeedOffsets[i];
                center_BottomClouds[i] += Vector2.UnitX * currentBottomParallax.Speed * cloudLayerSpeedOffsets[i];

                if (center_TopClouds[i].X > texture_Clouds[cloudLayers[i]].Width)
                    center_TopClouds[i].X = 0;
                else if (center_TopClouds[i].X < 0)
                    center_TopClouds[i].X = (float)texture_Clouds[cloudLayers[i]].Width;

                if (center_BottomClouds[i].X > texture_Clouds[cloudLayers[i]].Width)
                    center_BottomClouds[i].X = 0;
                else if (center_BottomClouds[i].X <0)
                    center_BottomClouds[i].X = (float)texture_Clouds[cloudLayers[i]].Width;
            }
            
            // Update any transitions
            if (topParallaxChange)
            {
                timeSinceTopChange += gameTime.ElapsedGameTime.TotalSeconds;
                if (timeSinceTopChange > TransitionTime)
                {
                    topParallaxChange = false;
                    timeSinceTopChange = 0;
                }
            }

            if (bottomParallaxChange)
            {
                timeSinceBottomChange += gameTime.ElapsedGameTime.TotalSeconds;
                if (timeSinceBottomChange > TransitionTime)
                {
                    bottomParallaxChange = false;
                    timeSinceBottomChange = 0;
                }
            }
        }

        private void DrawParallax(GameTime gameTime)
        {
            // Do the top
            if (topParallaxChange)
            {
                DrawElement(lastTopParallax, 1f - TopFadeAlpha, true);
                DrawElement(currentTopParallax, TopFadeAlpha, true);
            }
            else
            {
                DrawElement(currentTopParallax, 1f, true);
            }

            // Do the bottom
            if (bottomParallaxChange)
            {
                DrawElement(lastBottomParallax, 1f - BottomFadeAlpha, false);
                DrawElement(currentBottomParallax, BottomFadeAlpha, false);
            }
            else
            {
                DrawElement(currentBottomParallax, 1f, false);
            }
        }

        private void DrawElement(ParallaxElement pe, float alpha, bool isTop)
        {
            // Draw the top
            int i = pe.Intensity;
            if (i > 0)
            {
                // We do nothing if the intensity is zero
                if (i > totalNumPossibleLayers)
                    i = totalNumPossibleLayers;
                i--;

                if (isTop)
                {
                    for (int t = 0; t <= i; t++)
                    {
                        DrawLayer(
                            texture_Clouds[cloudLayers[t]],
                            Vector2.Zero,
                            center_TopClouds[t],
                            new Color(pe.Tint, (byte)((float)pe.Tint.A * alpha * cloudLayerAlphaModifiers[t])),
                            isTop);
                    }
                }
                else
                {
                    for (int t = 0; t <= i; t++)
                    {
                        DrawLayer(
                            texture_Clouds[cloudLayers[t]],
                            (float)InstanceManager.DefaultViewport.Height * Vector2.UnitY,
                            center_BottomClouds[t],
                            new Color(pe.Tint, (byte)((float)pe.Tint.A * alpha * cloudLayerAlphaModifiers[t])),
                            isTop);
                    }

                }
            }
        }

        /// <summary>
        /// Draw a given layer
        /// </summary>
        /// <param name="texture2D">The texture to draw</param>
        /// <param name="vector2">The position</param>
        /// <param name="vector2_3">The center</param>
        /// <param name="c">The color</param>
        /// <param name="p">True if we need to flip the texture</param>
        private void DrawLayer(
            Texture2D texture2D, 
            Vector2 vector2, 
            Vector2 vector2_3, 
            Color c, 
            bool p)
        {
            SpriteEffects se = SpriteEffects.None;
            if (p)
                se = SpriteEffects.FlipVertically;

            int maxX = (int)(((float)InstanceManager.DefaultViewport.Width + (float)texture2D.Width) / (float)texture2D.Width) + 1;

            for (int x = 0; x < maxX; x++)
            {
                InstanceManager.RenderSprite.Draw(
                    texture2D,
                    vector2 + (float)x * (float)texture2D.Width * Vector2.UnitX,
                    vector2_3,
                    null,
                    c,
                    0f,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AbsoluteTop,
                    se);
            }
        }
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
                backgroundNum = numBackgrounds % backgroundNum - 1;

            currentBackground = backgroundNum;
            timeSinceTransitionRequest = 0f;
            lastColor = Color.White;
            currentColor = new Color(Vector4.Zero);
        }

        /// <summary>
        /// Set the next parallax element
        /// </summary>
        /// <param name="pe">The parallax element to transition to</param>
        /// <param name="top">Set to true if you want the top parallax, false if the bottom</param>
        public void SetParallaxElement(ParallaxElement pe, bool top)
        {
            if (top)
            {
                lastTopParallax = currentTopParallax;
                currentTopParallax = pe;
                topParallaxChange = true;
                timeSinceTopChange = 0;
            }
            else
            {
                lastBottomParallax = currentBottomParallax;
                currentBottomParallax = pe;
                bottomParallaxChange = true;
                timeSinceBottomChange = 0;
            }
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
            UpdateParallax(gameTime);
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

            DrawParallax(gameTime);
            base.Draw(gameTime);
        }
        #endregion
    }
}