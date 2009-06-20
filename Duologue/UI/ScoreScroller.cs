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
using Duologue.State;
using Duologue.PlayObjects;
using Duologue.AchievementSystem;
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
        private const string smallFontFilename = "Fonts/inero-28";
        private const string filename_LifeUp = "Audio/PlayerEffects/life-up";
        private const float volume_LifeUp = 1f;
        private const double timeToDisplayLifeUp = 1.2f;
        private const double timePerLifeUpBlink = 0.1f;
        //private const string livesDot = "PlayerUI\\live-dot";
        private const int defaultLives = 4;
        private const int maxScore = 9999999;
        private const int defaultDeltaScore = 5;
        private const int numPointlets = 10;
        private const float timeToMovePointlet = 1f;
        private const int minPointletAlpha = 150;
        private const int maxPointletAlpha = 225;

        /// <summary>
        /// Extra lifes rewarded at multiples of this
        /// </summary>
        private const int extraLifeAt = 3000;

        /// <summary>
        /// The maximum number of lives we can have
        /// </summary>
        private const int maxLives = 999;
        #endregion

        #region Fields
        // Font stuff
        private SpriteFont scoreFont;
        private SpriteFont playerFont;
        private Vector2 scoreFontCharSize;
        private Vector2 playerFontCharSize;
        // Lives stuff
        //private Texture2D life;
        private int lives;
        // Position, moving and timing
        private Vector2 position;
        private Vector2 finalPosition;
        private Vector2 origin;
        private float timeToMove;
        private float timeSinceStart;
        private Vector2 alignment;
        private Vector2 scoreSize;
        //private ColorState colorState;
        // Score stuff
        private int score;
        private int scrollingScore;
        private int lastScore;
        //private float timeToScroll;
        private float timeSinceScrollStart;
        private int lengthOfMaxScore;
        private int deltaScore;
        private string playerText;
        private string playerTextInfinite;
        private Vector2 playerTextSizeInfinite;
        private Vector2 playerTextSize;
        private string gameOverText;
        // Pointlets
        private Pointlet[] pointlets;
        private Queue<Pointlet> freePointlets;
        // Misc stuff
        private Random rand;
        private Game localGame;
        /// <summary>
        /// Get or set the current player we're associated with
        /// </summary>
        private Player associatedPlayer;
        private int myPlayerNumber;
        private SoundEffect sfx_LifeUp;
        private SoundEffectInstance sfxi_LifeUp;
        private double timeSinceLifeUpColorChange;
        private double timeSinceLifeUpSpawn;
        private bool currentLifeUpColorIsDark = true;
        private int nextExtraLifeAt;
        private Vector2 size_ExtraLife;
        private Vector2 offset_ExtraLife;
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
        //public ColorState ColorState
        //{
            //get { return colorState; }
        //}

        /// <summary>
        ///  Read-only access to the current score
        /// </summary>
        public int Score
        {
            get { return score; }
        }

        /// <summary>
        /// Read-only access to the score text
        /// </summary>
        public string ScoreText
        {
            get { return playerText;}
        }

        /// <summary>
        /// Read-only access to the number of lives left
        /// </summary>
        public int Lives
        {
            get { return lives; }
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
            int myPlayer,
            float moveTime,
            Vector2 startPosition,
            Vector2 endPosition,
            int defaultScore,
            float scoreScrollTime)
            : base(game)
        {
            localGame = game;
            myPlayerNumber = myPlayer;
            score = defaultScore;
            timeToMove = moveTime;
            position = startPosition;
            finalPosition = endPosition;
            lives = defaultLives;
            alignment = -1f * Vector2.One;
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
            pointlets = new Pointlet[numPointlets];
            freePointlets = new Queue<Pointlet>(numPointlets);
            /*for (int i = 0; i < numPointlets; i++)
            {
                pointlets[i] = new Pointlet(
                    position,
                    Color.White);
                freePointlets.Enqueue(pointlets[i]);
            }*/

            PurgePointlets();

            // Set the score text
            playerTextInfinite = Resources.ScoreUI_InfiniteMode_Lives;
            playerText = Resources.ScoreUI_Lives;
            gameOverText = Resources.ScoreUI_GameOver;
            base.Initialize();
        }

        /// <summary>
        /// Load the object's content
        /// </summary>
        protected override void LoadContent()
        {
            if (Assets == null)
                Assets = InstanceManager.AssetManager;

            sfx_LifeUp = Assets.LoadSoundEffect(filename_LifeUp);
            sfxi_LifeUp = null;

            scoreFont = Assets.LoadSpriteFont(fontFilename);
            playerFont = Assets.LoadSpriteFont(smallFontFilename);

            // Determine the max width a character needs to display
            scoreFontCharSize = scoreFont.MeasureString("0");
            // We assume small font only needs Y
            playerFontCharSize = playerFont.MeasureString("0");
            for(int i = 1; i < 10; i++)
            {
                Vector2 w = scoreFont.MeasureString(i.ToString());
                if(w.X > scoreFontCharSize.X)
                    scoreFontCharSize = w;
            }

            playerTextSize = playerFont.MeasureString(playerText);
            playerTextSizeInfinite = scoreFont.MeasureString(playerTextInfinite);
            scoreSize = scoreFont.MeasureString(maxScore.ToString());

            scoreSize.Y += playerTextSize.Y;

            timeSinceLifeUpSpawn = timeToDisplayLifeUp + 1.0;
            timeSinceLifeUpColorChange = 0.0;

            size_ExtraLife = playerFont.MeasureString(Resources.ScoreUI_ExtraLife);
            offset_ExtraLife = Vector2.UnitY * -2f * size_ExtraLife.Y +
                Vector2.UnitX * size_ExtraLife.X / -2f;

            //life = Assets.LoadTexture2D(livesDot);
            base.LoadContent();
        }
        #endregion

        #region Private methods
        private bool InfiniteMode()
        {
            return
                (LocalInstanceManager.CurrentGameState == GameState.InfiniteGame ||
                LocalInstanceManager.NextGameState == GameState.InfiniteGame);
        }


        /// <summary>
        /// Add a pointlet
        /// </summary>
        /// <param name="points">Points to add</param>
        /// <param name="pointPos">Pointlet position</param>
        private void AddPointlet(int points, Vector2 pointPos)
        {
            if (freePointlets.Count > 0)
            {
                Pointlet p = freePointlets.Dequeue();
                p.Initialize(pointPos,
                    new Color(
                        associatedPlayer.PlayerColor.Colors[PlayerColors.Light].R,
                        associatedPlayer.PlayerColor.Colors[PlayerColors.Light].G,
                        associatedPlayer.PlayerColor.Colors[PlayerColors.Light].B,
                        (byte)rand.Next(minPointletAlpha, maxPointletAlpha)),
                    points,
                    new Rectangle(
                        (int)position.X,
                        (int)position.Y,
                        (int)(scoreFontCharSize.X * maxScore.ToString().Length),
                        (int)(scoreFontCharSize.Y)),
                        timeToMovePointlet);
            }
        }

        /// <summary>
        /// Called when we need to update the origin
        /// </summary>
        private void UpdateOrigin()
        {
            // Default is the upper left corner
            origin = position;

            if (alignment.X > 0)
                origin.X = position.X - scoreSize.X;
            if (alignment.Y > 0)
                origin.Y = position.Y - scoreSize.Y;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Call when you want to add new points to the score
        /// </summary>
        /// <param name="points">The points to add to the score</param>
        /// <param name="pointPos">The position of the pointlet to launch</param>
        public void AddScore(int points, Vector2? pointPos)
        {
            lastScore = score;
            scrollingScore = score;
            score += points;
            if(pointPos != null)
                AddPointlet(points, (Vector2)pointPos);
            if (score > maxScore)
            {
                LocalInstanceManager.AchievementManager.UnlockAchievement(PossibleMedals.HeavyRoller);
                score -= maxScore;
                scrollingScore = 0;
            }

            if (score >= nextExtraLifeAt && !InfiniteMode())
            {
                timeSinceLifeUpSpawn = 0.0;
                timeSinceLifeUpColorChange = 0.0;
                nextExtraLifeAt += extraLifeAt;
                lives++;
                if (lives > maxLives)
                    lives = maxLives;
                if (sfxi_LifeUp == null)
                {
                    sfxi_LifeUp = sfx_LifeUp.Play(volume_LifeUp);
                }
                else
                {
                    if (sfxi_LifeUp.State == SoundState.Playing)
                        sfxi_LifeUp.Stop();
                    sfxi_LifeUp.Play();
                }
            }
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
            UpdateOrigin();
        }

        /// <summary>
        /// Call to reset the score to some number.
        /// <remarks>Note that this will reset the score outright, it wont animate the change</remarks>
        /// </summary>
        /// <param name="p">Score to reset to</param>
        public void SetScore(int p)
        {
            score = p;
            scrollingScore = p;
            nextExtraLifeAt = p + extraLifeAt;
        }

        /// <summary>
        /// Call to set the lives
        /// </summary>
        /// <param name="p">Number of lives</param>
        public void SetLives(int p)
        {
            if (!InfiniteMode())
            {
                lives = p;
                if (lives > maxLives)
                    lives = maxLives;
            } // We ignore this request otherwise
        }

        /// <summary>
        /// Call when we need to reset lives
        /// </summary>
        public void ResetLives()
        {
            if (InfiniteMode())
                lives = 1;
            else
                lives = defaultLives;
        }

        /// <summary>
        /// Call to set the alignment of the score
        /// </summary>
        /// <param name="p">The X direction sets left-right alignment (-1,1).
        /// The Y direction sets up-down alignment (-1,1).</param>
        public void SetAlignment(Vector2 p)
        {
            alignment = p;
            UpdateOrigin();
        }

        /// <summary>
        /// Call when the player loses a life
        /// </summary>
        /// <returns>True if they have another life, false if not</returns>
        public bool LoseLife()
        {
            if (LocalInstanceManager.CurrentGameState == GameState.InfiniteGame)
            {
                if(lives < maxLives)
                    lives++;
                return true;
            }
            else
            {
                if (lives > 0)
                    lives--;
                return lives > 0;
            }
        }

        /// <summary>
        /// Called at the end of a game to purge all active pointlets
        /// </summary>
        public void PurgePointlets()
        {
            for (int i = 0; i < numPointlets; i++)
            {
                pointlets[i] = new Pointlet(
                    position,
                    Color.White);
                freePointlets.Enqueue(pointlets[i]);
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

            // Update pointlets
            foreach (Pointlet p in pointlets)
            {
                if (p.Alive)
                {
                    p.Update(dt);
                    if (!p.Alive)
                    {
                        freePointlets.Enqueue(p);
                    }
                }
            }

            if (timeSinceLifeUpSpawn < timeToDisplayLifeUp)
            {
                timeSinceLifeUpSpawn += gameTime.ElapsedGameTime.TotalSeconds;
                timeSinceLifeUpColorChange += gameTime.ElapsedGameTime.TotalSeconds;
                if (timeSinceLifeUpColorChange > timePerLifeUpBlink)
                {
                    currentLifeUpColorIsDark = !currentLifeUpColorIsDark;
                    timeSinceLifeUpColorChange = 0.0;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (Render == null)
                Render = InstanceManager.RenderSprite;

            if (associatedPlayer == null)
                associatedPlayer = LocalInstanceManager.Players[myPlayerNumber];

            // Do pointlets first
            foreach (Pointlet p in pointlets)
            {
                if (p.Alive)
                {
                    Render.DrawString(
                        playerFont,
                        p.Points.ToString(),
                        p.Position,
                        p.Tint);
                }
            }
            int length = scrollingScore.ToString().Length;
            CharEnumerator chars = scrollingScore.ToString().GetEnumerator();
            int currentChar = 0;
            Vector2 offsetPos = origin + new Vector2(0f, playerFontCharSize.Y/2f);
            Vector2 charPos = offsetPos;
            int difference = score - scrollingScore;
            int diffLength = difference.ToString().Length;

            // Next do the scoreText
            if (InfiniteMode())
            {
                // Here, we do score first
                for (int i = 0; i < lengthOfMaxScore - length; i++)
                {
                    charPos = origin + new Vector2((float)(currentChar * playerFontCharSize.X), 0f);
                    Render.DrawString(
                        playerFont,
                        "0",
                        charPos,
                        associatedPlayer.PlayerColor.Colors[PlayerColors.Light],
                        RenderSpriteBlendMode.AbsoluteTop);
                    currentChar++;
                }

                while (chars.MoveNext())
                {
                    charPos = origin + new Vector2((float)(currentChar * playerFontCharSize.X), 0f);
                    Render.DrawString(
                        playerFont,
                        chars.Current.ToString(),
                        charPos,
                        associatedPlayer.PlayerColor.Colors[PlayerColors.Light],
                        RenderSpriteBlendMode.AbsoluteTop);

                    if (scrollingScore < score &&
                        diffLength > 0 &&
                        currentChar >= lengthOfMaxScore - diffLength)
                    {
                        Render.DrawString(
                            playerFont,
                            rand.Next(9).ToString(),
                            charPos,
                            new Color(
                                associatedPlayer.PlayerColor.Colors[PlayerColors.Light].R,
                                associatedPlayer.PlayerColor.Colors[PlayerColors.Light].G,
                                associatedPlayer.PlayerColor.Colors[PlayerColors.Light].B,
                                (byte)100),
                            RenderSpriteBlendMode.AddititiveTop);
                    }

                    currentChar++;
                }
                // Next, we do lives
                Render.DrawString(
                    scoreFont,
                    playerTextInfinite,
                    offsetPos,
                    associatedPlayer.PlayerColor.Colors[PlayerColors.Light],
                    RenderSpriteBlendMode.AbsoluteTop);
                Render.DrawString(
                    scoreFont,
                    (lives - 1).ToString(),
                    offsetPos + new Vector2(playerTextSizeInfinite.X, 0f),
                    associatedPlayer.PlayerColor.Colors[PlayerColors.Light],
                    RenderSpriteBlendMode.AbsoluteTop);


            }
            else
            {
                if (lives > 0)
                {
                    Render.DrawString(
                        playerFont,
                        playerText,
                        origin,
                        associatedPlayer.PlayerColor.Colors[PlayerColors.Light],
                        RenderSpriteBlendMode.AbsoluteTop);
                    Render.DrawString(
                        playerFont,
                        (lives - 1).ToString(),
                        origin + new Vector2(playerTextSize.X, 0f),
                        associatedPlayer.PlayerColor.Colors[PlayerColors.Light],
                        RenderSpriteBlendMode.AbsoluteTop);
                    if (timeSinceLifeUpSpawn < timeToDisplayLifeUp)
                    {
                        if (currentLifeUpColorIsDark)
                            Render.DrawString(
                                playerFont,
                                Resources.ScoreUI_ExtraLife,
                                associatedPlayer.Position + offset_ExtraLife,
                                associatedPlayer.PlayerColor.Colors[PlayerColors.Dark],
                                RenderSpriteBlendMode.AbsoluteTop);
                        else
                            Render.DrawString(
                                playerFont,
                                Resources.ScoreUI_ExtraLife,
                                associatedPlayer.Position + offset_ExtraLife,
                                associatedPlayer.PlayerColor.Colors[PlayerColors.Light],
                                RenderSpriteBlendMode.AbsoluteTop);
                    }
                }
                else
                {
                    Render.DrawString(
                        playerFont,
                        gameOverText,
                        origin,
                        associatedPlayer.PlayerColor.Colors[PlayerColors.Light],
                        RenderSpriteBlendMode.AbsoluteTop);
                }

                // The score itself
                for (int i = 0; i < lengthOfMaxScore - length; i++)
                {
                    charPos = offsetPos + new Vector2((float)(currentChar * scoreFontCharSize.X), 0f);
                    Render.DrawString(
                        scoreFont,
                        "0",
                        charPos,
                        associatedPlayer.PlayerColor.Colors[PlayerColors.Light],
                        RenderSpriteBlendMode.AbsoluteTop);
                    currentChar++;
                }

                while (chars.MoveNext())
                {
                    charPos = offsetPos + new Vector2((float)(currentChar * scoreFontCharSize.X), 0f);
                    Render.DrawString(
                        scoreFont,
                        chars.Current.ToString(),
                        charPos,
                        associatedPlayer.PlayerColor.Colors[PlayerColors.Light],
                        RenderSpriteBlendMode.AbsoluteTop);

                    if (scrollingScore < score &&
                        diffLength > 0 &&
                        currentChar >= lengthOfMaxScore - diffLength)
                    {
                        Render.DrawString(
                            scoreFont,
                            rand.Next(9).ToString(),
                            charPos,
                            new Color(
                                associatedPlayer.PlayerColor.Colors[PlayerColors.Light].R,
                                associatedPlayer.PlayerColor.Colors[PlayerColors.Light].G,
                                associatedPlayer.PlayerColor.Colors[PlayerColors.Light].B,
                                (byte)100),
                            RenderSpriteBlendMode.AddititiveTop);
                    }

                    currentChar++;
                }
            }

            base.Draw(gameTime);
        }
        #endregion
    }
}