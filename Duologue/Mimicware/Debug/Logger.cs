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

namespace Mimicware.Debug
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Logger : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Properties
        /// <summary>
        /// The log (please don't write unless you know what you're doing! Use LogEntry() instead
        /// </summary>
        public string[] Log;
        /// <summary>
        /// The AssetManager
        /// </summary>
        public AssetManager AssetManager;
        /// <summary>
        /// The RenderSprite
        /// </summary>
        public RenderSprite RenderSprite;
        #endregion

        #region Fields
        private Game localGame;
        private SpriteFont font;
        #endregion

        #region Constants
        private const int maxLogLength = 10;
        #endregion

        #region Constructor / Init / Load
        public Logger(Game game)
            : base(game)
        {
            Log = new string[maxLogLength];
            for (int i = 0; i < maxLogLength; i++)
            {
                Log[i] = "";
            }
            AssetManager = null;
            RenderSprite = null;
            localGame = game;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            if (AssetManager == null)
                throw new NullReferenceException("AssetManager not set before LoadContent() call!");

            font = AssetManager.LoadSpriteFont("Mimicware\\Arial12");
            base.LoadContent();
        }
        #endregion

        #region Draw/Update
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            this.RenderSprite.DrawString(
                font,
                "Log:",
                new Vector2(20f, 20f),
                Color.Bisque);

            for (int i = 0; i < Log.Length; i++)
            {
                this.RenderSprite.DrawString(
                    font,
                    Log[i],
                    new Vector2(20f, 15f * i + 35f),
                    Color.White);
            }
            base.Draw(gameTime);
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Put a log entry into the log
        /// </summary>
        /// <param name="p">The text of the log entry</param>
        public void LogEntry(string p)
        {
            // Shift every entry up one.... could be done better, but lazy now
            for (int i = 1; i < Log.Length; i++)
            {
                Log[i - 1] = Log[i];
            }
            Log[Log.Length - 1] = p;
        }
        #endregion
    }
}