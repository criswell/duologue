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
        private const string fontFilename = "Fonts/inero-40";
        private const int maxScore = 999999;
        private const int defaultDeltaScore = 5;
        #endregion

        #region Fields
        // Font stuff
        private SpriteFont font;
        private Vector2 fontCharSize;
        // Position, moving and timing
        private Vector2 position;
        private Vector2 finalPosition;
        private float timeToMove;
        private float timeSinceStart;
        private ColorState colorState;
        // Score stuff
        private int score;
        private int scrollingScore;
        private int lastScore;
        private float timeToScroll;
        private float timeSinceScrollStart;
        private int lengthOfMaxScore;
        private int deltaScore;
        // Misc stuff
        private Random rand;
        private Game localGame;
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
        /// <param name="defaultScore">The default or starting score</param>
        /// <param name="moveTime">The time it takes to move the score from the start position
        /// to the end position</param>
        /// <param name="startPosition">The starting position for this score</param>
        /// <param name="endPosition">The end position for this score</param>
        public ScoreScroller(
            Game game,
            Player myPlayer,
            float moveTime,
            Vector2 startPosition,
            Vector2 endPosition,
            int defaultScore,
            float scoreScrollTime)
            : base(game)
        {
            localGame = game;
            AssociatedPlayer = myPlayer;
            score = defaultScore;
            timeToMove = moveTime;
            position = startPosition;
            finalPosition = endPosition;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            timeSinceStart = 0;
            timeSinceScrollStart = 0f;
            lengthOfMaxScore = maxScore.ToString().Length;
            rand = new Random();
            deltaScore = defaultDeltaScore;
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

            // Determine the max width a character needs to display
            fontCharSize = font.MeasureString("0");
            for(int i = 1; i < 10; i++)
            {
                Vector2 w = font.MeasureString(i.ToString());
                if(w.X > fontCharSize.X)
                    fontCharSize = w;
            }
            base.LoadContent();
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        /// <summary>
        /// Call when you want to add new points to the score
        /// </summary>
        /// <param name="points">The points to add to the score</param>
        public void AddScore(int points)
        {
            lastScore = score;
            score += points;
        }

        /// <summary>
        /// Set the desired start and end positions. Note that this will be treated
        /// differently depending upon whether the component is visible or not. If it
        /// is visible, the startPosition will be ignored, and the endPosition will
        /// be set. If the component is not visible, then the start and end positions
        /// will be respected outright.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        public void SetPositions(Vector2? startPosition, Vector2 endPosition)
        {
            if (this.Visible)
            {
                finalPosition = endPosition;
            }
            else if (!this.Visible)
            {
                finalPosition = endPosition;
                if (startPosition != null)
                    position = (Vector2)startPosition;
                else
                    position = endPosition;
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
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeSinceStart += dt;
            timeSinceScrollStart += dt;
            // Update the position

            // Update the scrolling score
            if (scrollingScore < score)
            {
                scrollingScore += deltaScore;
            }
            if (scrollingScore > score)
                scrollingScore = score;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Render == null)
                Render = InstanceManager.RenderSprite;

            int length = scrollingScore.ToString().Length;
            CharEnumerator chars = scrollingScore.ToString().GetEnumerator();
            int currentChar = 0;
            Vector2 charPos = position;
            int difference = score - scrollingScore;
            int diffLength = difference.ToString().Length;

            for (int i = 0; i < lengthOfMaxScore - length; i++)
            {
                charPos = position + new Vector2((float)(currentChar * fontCharSize.X), 0f);
                Render.DrawString(
                    font,
                    "0",
                    charPos,
                    AssociatedPlayer.PlayerTint);
                currentChar++;
            }

            while (chars.MoveNext())
            {
                charPos = position + new Vector2((float)(currentChar * fontCharSize.X), 0f);
                Render.DrawString(
                    font,
                    chars.Current.ToString(),
                    charPos,
                    AssociatedPlayer.PlayerTint);

                if(scrollingScore < score &&
                    diffLength > 0 &&
                    currentChar >= lengthOfMaxScore - diffLength)
                {
                    Render.DrawString(
                        font,
                        rand.Next(9).ToString(),
                        charPos,
                        new Color(
                            AssociatedPlayer.PlayerTint.R,
                            AssociatedPlayer.PlayerTint.G,
                            AssociatedPlayer.PlayerTint.B,
                            (byte)100),
                        true);
                }

                currentChar++;
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}