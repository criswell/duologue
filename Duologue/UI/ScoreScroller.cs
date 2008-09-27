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
using Duologue.State;
using Duologue.PlayObjects;
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
        private ColorState colorState;
        private Game localGame;
        private int score;
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

        /// <summary>
        /// Get the current color state of the player this scroller is associated with (read-only)
        /// </summary>
        public ColorState ColorState
        {
            get { return colorState; }
        }

        /// <summary>
        /// Get or set the current player we're associated with
        /// </summary>
        public Player AssociatedPlayer;

        /// <summary>
        ///  Read-only access to the current score
        /// </summary>
        public int Score
        {
            get { return score; }
        }
        #endregion

        #region Constructor / Init / Load
        /// <summary>
        /// Constructs a score scroller object
        /// </summary>
        /// <param name="game">The game this object belongs to</param>
        /// <param name="myPlayer">The player this object is associated with</param>
        public ScoreScroller(Game game, Player myPlayer)
            : base(game)
        {
            localGame = game;
            AssociatedPlayer = myPlayer;
            score = 0;
        }

        public ScoreScroller(Game game, Player myPlayer, int defaultScore)
            : base(game)
        {
            localGame = game;
            AssociatedPlayer = myPlayer;
            score = 0;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            timeSinceStart = 0;
            base.Initialize();
        }

        /// <summary>
        /// Load the object's content
        /// </summary>
        protected override void LoadContent()
        {
            if (Assets == null)
                Assets = InstanceManager.AssetManager;

            font = Assets.LoadSpriteFont(fontFilename);
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