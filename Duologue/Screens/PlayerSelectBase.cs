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
    /// The base backgrounds for the player select screen
    /// </summary>
    public class PlayerSelectBase : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const float fadeInTime = 1f;
        private const float fadeOutTime = 0.2f;
        private const string tileFilename = "multiplayer-select-tile";
        #endregion

        #region Fields
        // The graphics textures
        private Texture2D tile;
        private PlayerColors[] playerColors;
        private Vector2[] center;
        private Vector2 position;
        // Time
        private float timeSinceStart;
        private bool begin;
        #endregion

        #region Properties
        public float Percentage
        {
            get
            {
                if (begin)
                {
                    return MathHelper.Min(timeSinceStart / fadeInTime, 1f);
                }
                else
                {
                    return MathHelper.Min(1f, timeSinceStart / fadeOutTime);
                }
            }
        }
        #endregion

        #region Constructor / Init / Load
        public PlayerSelectBase(Game game)
            : base(game)
        {
            begin = true;
            timeSinceStart = 0f;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            center = new Vector2[InputManager.MaxInputs];

            base.Initialize();
        }

        protected override void LoadContent()
        {
            tile = InstanceManager.AssetManager.LoadTexture2D(tileFilename);

            center[0] = new Vector2(
                tile.Width,
                tile.Height);

            center[1] = new Vector2(
                0f,
                tile.Height);

            center[2] = new Vector2(
                tile.Width,
                0f);

            center[3] = new Vector2(
                0f,
                0f);

            playerColors = PlayerColors.GetPlayerColors();
            base.LoadContent();
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
            if (Percentage < 1f)
                timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(position == Vector2.Zero)
                position = new Vector2(
                    InstanceManager.DefaultViewport.Width / 2f,
                    InstanceManager.DefaultViewport.Height / 2f);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    tile,
                    position,
                    center[i],
                    null,
                    new Color(
                        playerColors[i].Colors[1].R,
                        playerColors[i].Colors[1].G,
                        playerColors[i].Colors[1].B,
                        (byte)(255 * Percentage)),
                    0f,
                    1f,
                    0.9f);
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}