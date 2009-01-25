#region File Description
#endregion

#region Using Statements
// System
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
// Mimicware
using Mimicware.Manager;
using Mimicware.Graphics;
using Mimicware;
// Duologue
using Duologue;
using Duologue.Audio;
using Duologue.State;
using Duologue.Properties;
using Duologue.Screens;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.UI;
#endregion

namespace Duologue.Screens
{
    public enum GamePlayState
    {
        WaveIntro,
        InitPlayerSpawn,
        Playing,
        LoadNextWave,
        Delay,
        GameOver,
    }
    /// <summary>
    /// The main game play screen which coordinates the various game elements
    /// </summary>
    public class GamePlayScreenManager : GameScreen
    {
        #region Constants
        private const float delayLifetime = 1f;

        /// <summary>
        /// When the counter reaches this limit, we increase the intensity
        /// </summary>
        private const int intensityIncreaseLimit = 2;

        /// <summary>
        /// The rate of increase for the intensity counter
        /// </summary>
        //private const float intensityIncreaseDelta = 0.5f;

        /// <summary>
        /// How long it takes for nothing happening to decrease intensity
        /// </summary>
        private const float intensityLifetime = 4f;

        /// <summary>
        /// The min beat percent to bump up the intensity
        /// </summary>
        private const float minBeatPercent = 0.75f;
        #endregion

        #region Fields
        private DuologueGame localGame;
        private GameWaveManager gameWaveManager;
        //private GameWave currentWave;
        private GamePlayState currentState;
        private GamePlayState nextState;
        private WaveDisplay waveDisplay;
        private GamePlayLoop gamePlayLoop;
        private float timeSinceStart;
        private GameOver gameOver;

        /// <summary>
        /// When this reaches intensityIncreaseLimit, we bump the intensity up
        /// Over time, we bump it by intensityDecreaseDelta, when it reaches zero, we decrease intensity
        /// </summary>
        private float intensityCounter;

        /// <summary>
        /// Countdown timer for intensity
        /// </summary>
        private float intensityTimer;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the current state of the gameplay
        /// </summary>
        public GamePlayState CurrentState
        {
            get { return currentState; }
        }
        /// <summary>
        /// Percentage complete for a delay
        /// </summary>
        public float PercentComplete
        {
            get { return Math.Min(timeSinceStart / delayLifetime, 1f); }
        }
        #endregion

        #region Constructor / Init
        public GamePlayScreenManager(Game game) : base(game)
        {
            localGame = (DuologueGame)game;
            gameWaveManager = new GameWaveManager(null);
            waveDisplay = new WaveDisplay(localGame);
            localGame.Components.Add(waveDisplay);
            gamePlayLoop = new GamePlayLoop(localGame, this);
            gamePlayLoop.DrawOrder = 4;
            localGame.Components.Add(gamePlayLoop);

            // Game over screen starts out invisble
            gameOver = new GameOver(localGame, this);
            gameOver.Enabled = false;
            gameOver.Visible = false;
            localGame.Components.Add(gameOver);
            //Reset();
        }

        /// <summary>
        /// Reset everything
        /// </summary>
        private void Reset()
        {
            currentState = GamePlayState.WaveIntro;
            gameWaveManager.CurrentMajorNumber = 1;
            gameWaveManager.CurrentMinorNumber = 0;
            LocalInstanceManager.CurrentGameWave = null;
            timeSinceStart = 0f;
            intensityCounter = 0f;
            intensityTimer = 0f;
        }

        /*
        public override void Initialize()
        {
            base.Initialize();
        }*/

        protected override void InitializeConstants()
        {
            MyComponents.Add(waveDisplay);
            MyComponents.Add(gamePlayLoop);

            this.SetEnable(false);
            this.SetVisible(false);
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        public override void SetEnable(bool t)
        {
            if (t)
                Reset();
            else
            {
                // Here is where we want to turn everything off

                // First, turn off the score
                for (int i = 0; i < InputManager.MaxInputs; i++)
                {
                    LocalInstanceManager.Scores[i].Enabled = false;
                    LocalInstanceManager.Scores[i].Visible = false;   
                }
            }
            base.SetEnable(t);
        }

        /// <summary>
        /// Set the Game Over sequence
        /// </summary>
        /// <param name="t">True to begin gameover, false to end it</param>
        public void GameOver(bool t)
        {
            if (t && currentState != GamePlayState.GameOver)
            {
                gameOver.Reset();
                // FIXME
                // Should probably do some gameover music here
                gameOver.Visible = true;
                gameOver.Enabled = true;
                currentState = GamePlayState.GameOver;
            }
            else if (!t && currentState == GamePlayState.GameOver)
            {
                localGame.Audio.music.StopSong(SongID.Intensity);
                //Music.StopSong(SongID.Intensity);
                gameOver.Visible = false;
                gameOver.Enabled = false;
                // FIXME
                // Should probably reset things here, like the music, etc
                LocalInstanceManager.CurrentGameState = GameState.MainMenuSystem;
            }
        }

        /// <summary>
        /// Called when an enemy has been destroyed, will spawn pointlets as well
        /// as possibly increase the intensity of the music
        /// </summary>
        /// <param name="pindex">The PlayerIndex of the player who gets the points</param>
        /// <param name="pointValue">The "best" point value (if the beat hit was perfect)</param>
        /// <param name="startPos">The starting position of the pointlet</param>
        public void TriggerPoints(PlayerIndex pindex, int pointValue, Vector2 startPos)
        {
            // Get the beat percentage
            // FIXME for now it's just random
            double bp = MWMathHelper.GetRandomInRange(0.5, 1);

            // Update the score based on that
            int pointValueM = (int)(bp * (double)pointValue);

            // Spawn a pointlet
            LocalInstanceManager.Scores[(int)pindex].AddScore(pointValueM, startPos);

            InstanceManager.Logger.LogEntry(String.Format(
                "P{0} enemy hit- MP{1}, BP{2}, SC{3}- INT{4}",
                ((int)pindex).ToString(),
                pointValue.ToString(),
                bp.ToString(),
                pointValueM.ToString(),
                intensityCounter.ToString()));

            // update intensity
            if (bp > minBeatPercent)
            {
                intensityTimer = 0f;
                intensityCounter += (float)bp; // intensityIncreaseDelta;
                if (intensityCounter > intensityIncreaseLimit)
                {
                    InstanceManager.Logger.LogEntry("INTENSITY++");
                    localGame.Audio.Intensify();
                    intensityCounter = 0f;
                }
            }
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (timeSinceStart < delayLifetime)
                timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;

            intensityTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (intensityTimer > intensityLifetime)
            {
                intensityTimer = 0f;
                intensityCounter = 0f;
                InstanceManager.Logger.LogEntry("INTENSITY--");
                localGame.Audio.Detensify();
            }

            /*if (LocalInstanceManager.CurrentGameWave == null)
            {
                LocalInstanceManager.CurrentGameWave = gameWaveManager.GetNextWave();
            }*/

            switch (currentState)
            {
                case GamePlayState.WaveIntro:
                    // Get the next wave
                    LocalInstanceManager.CurrentGameWave = gameWaveManager.GetNextWave();

                    // Display the wave intro
                    string[] text = new string[2];
                    text[0] = String.Format(Resources.GameScreen_Wave,
                        LocalInstanceManager.CurrentGameWave.MajorWaveNumber.ToString(),
                        LocalInstanceManager.CurrentGameWave.MinorWaveNumber.ToString());
                    text[1] = LocalInstanceManager.CurrentGameWave.Name;
                    waveDisplay.Text = text;
                    // Set up the background and enemies
                    LocalInstanceManager.Background.SetBackground(LocalInstanceManager.CurrentGameWave.Background);

                    // Set up the exit stuff
                    currentState = GamePlayState.Delay;
                    timeSinceStart = 0f;
                    nextState = GamePlayState.InitPlayerSpawn;
                    break;
                case GamePlayState.Delay:
                    if (PercentComplete >= 1f)
                        currentState = nextState;
                    break;
                case GamePlayState.InitPlayerSpawn:
                    for (int i = 0; i < InputManager.MaxInputs; i++)
                    {
                        if (LocalInstanceManager.Players[i].Active)
                        {
                            LocalInstanceManager.Players[i].ColorState = LocalInstanceManager.CurrentGameWave.ColorState;
                            LocalInstanceManager.Players[i].Spawn();

                            LocalInstanceManager.Scores[i].Enabled = true;
                            LocalInstanceManager.Scores[i].Visible = true;
                        }
                    }
                    currentState = GamePlayState.Delay;
                    nextState = GamePlayState.Playing;
                    localGame.Audio.music.PlaySong(SongID.Intensity);
                    break;
                default:
                    // Play the game or game over
                    break;
            }
            base.Update(gameTime);
        }
        #endregion
    }
}
