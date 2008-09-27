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
using Mimicware.Manager;
// Duologue
using Duologue;
#endregion

namespace Duologue.UI
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ScoreScroller : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        /// <summary>
        /// The filename for the font we use
        /// </summary>
        private const string fontFilename = "inero-40";
        #endregion

        #region Fields
        private SpriteFont font;
        private Vector2 position;
        private Vector2 finalPosition;
        private float timeToMove;
        private float timeSinceStart;
        #endregion

        #region Properties
        /// <summary>
        /// The game-wide render sprite instance
        /// </summary>
        public RenderSprite Render;

        /// <summary>
        /// The game-wide asset manager
        /// </summary>
        public AssetManager Assets;

        /// <summary>
        /// Read-only property telling where the current position is.
        /// If you wish to change this, you will have to use the proper method.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }

        /// <summary>
        /// Read-only property telling where the requested final position is.
        /// If you wish to change this, you will have to use the proper method.
        /// </summary>
        public Vector2 FinalPosition
        {
            get { return finalPosition; }
        }
        #endregion

        #region Constructor / Init / Load
        public ScoreScroller(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {
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
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        #endregion
    }
}