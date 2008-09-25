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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Background : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const int numBackgrounds = 3;
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
        private float timeSinceTransitionRequest;
        #endregion

        #region Properties
        /// <summary>
        /// How long it takes to transition between backgrounds
        /// </summary>
        public float TransitionTime;

        public bool InTransition
        {
            get { timeSinceTransitionRequest < TransitionTime; }
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
                backgrounds[i] = assets.LoadTexture2D(String.Format("background-{0:00}", i+1));
                center[i] = new Vector2(backgrounds[i].Width / 2f, backgrounds[i].Height / 2f);
            }

            currentBackground = 2;
        }
        #endregion

        #region Private methods
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

            render.Draw(
                backgrounds[currentBackground],
                screenCenter,
                center[currentBackground],
                null,
                Color.White,
                0f,
                1f,
                1f);
            base.Draw(gameTime);
        }
        #endregion
    }
}