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
        #endregion

        #region Fields
        private Game localGame;
        private GameWaveManager gameWaveManager;
        //private GameWave currentWave;
        private GamePlayState currentState;
        private GamePlayState nextState;
        private WaveDisplay waveDisplay;
        private GamePlayLoop gamePlayLoop;
        private float timeSinceStart;
        private GameOver gameOver;
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
            localGame = game;
            gameWaveManager = new GameWaveManager(null);
            waveDisplay = new WaveDisplay(localGame);
            localGame.Components.Add(waveDisplay);
            gamePlayLoop = new GamePlayLoop(localGame, this);
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
                ((DuologueGame)localGame).beatEngine.StopDance();
                gameOver.Visible = false;
                gameOver.Enabled = false;
                // FIXME
                // Should probably reset things here, like the music, etc
                LocalInstanceManager.CurrentGameState = GameState.MainMenuSystem;
            }
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (timeSinceStart < delayLifetime)
                timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;

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
                    ((DuologueGame)localGame).beatEngine.Enabled = true;
                    ((DuologueGame)localGame).beatEngine.PlayDance();
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
