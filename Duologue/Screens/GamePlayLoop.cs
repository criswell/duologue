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
using Duologue.Waves;
using Duologue.UI;
using Duologue.State;
using Duologue.Audio;
#endregion

namespace Duologue.Screens
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GamePlayLoop : Microsoft.Xna.Framework.DrawableGameComponent, IService
    {
        #region Constants
        private const float playerMovementModifier_X = 4f;
        private const float playerMovementModifier_Y = -4f;

        private const string filename_BeamIn = "Audio/PlayerEffects/beam-in";
        private const float volume_BeamIn = 0.5f;
        #endregion

        #region Fields
        private GamePlayScreenManager myManager;
        private SoundEffect sfx_BeamIn;
        private SoundEffectInstance sfxi_BeamIn;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        /// <summary>
        /// Constructs the Game Play Loop class instance
        /// </summary>
        /// <param name="game">My game</param>
        /// <param name="manager">My manager</param>
        public GamePlayLoop(Game game, GamePlayScreenManager manager)
            : base(game)
        {
            myManager = manager;
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
            sfx_BeamIn = InstanceManager.AssetManager.LoadSoundEffect(filename_BeamIn);
            base.LoadContent();
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        public void PlayBeamIn()
        {
            try
            {
                sfxi_BeamIn.Volume = volume_BeamIn;
                sfxi_BeamIn.Play();
            }
            catch
            {
                sfxi_BeamIn = sfx_BeamIn.Play(volume_BeamIn);
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
            int livingPlayers = 0;
            int livingEnemies = 0;
            int activePlayers = 0;
            bool dumb;
            bool skip = false;

            #region Player stuff
            // First, run through the players, doing their stuff
            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                Player p = LocalInstanceManager.Players[i];
                if (p.Active)
                {
                    activePlayers++;
                    if (p.State == PlayerState.Alive ||
                       p.State == PlayerState.GettingReady)
                    {
                        livingPlayers++;
                        // Check for pause requests
                        if (InstanceManager.InputManager.NewButtonPressed(Buttons.Start, p.MyPlayerIndex) &&
                            LocalInstanceManager.CanPause)
                            LocalInstanceManager.Pause = true;

                        // Update player position
                        p.Position.X += 
                            InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Left.X
                            * playerMovementModifier_X;
                        p.Position.Y +=
                            InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Left.Y
                            * playerMovementModifier_Y;

                        // Update player's orientation
                        if (InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Left.X != 0 ||
                           InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Left.Y != 0)
                        {
                            p.Orientation.X =
                                InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Left.X;
                            p.Orientation.Y = -1f *
                                InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Left.Y;
                        }

                        // Update player's aim
                        if (InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Right.X != 0 ||
                            InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Right.Y != 0)
                        {
                            p.Aim.X =
                                InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Right.X;
                            p.Aim.Y = -1f *
                                InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Right.Y;
                            p.Fire();
                        }

                        // Button handling
                        if ((InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].Triggers.Left > 0 &&
                            InstanceManager.InputManager.LastGamePadStates[(int)p.MyPlayerIndex].Triggers.Left == 0) ||
                           (InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].Triggers.Right > 0 &&
                            InstanceManager.InputManager.LastGamePadStates[(int)p.MyPlayerIndex].Triggers.Right == 0))
                        {
                            // Swap color
                            p.SwapColors();
                        }

                        // Level skip
                        // FIXME this is for debugging... if we leave it in, we should make it harder to pull off
                        if (InstanceManager.InputManager.NewKeyPressed(Keys.PageUp))
                            skip = true;

                        // Now, make sure no one is stepping on eachother
                        dumb = p.StartOffset();
                        // Yeah, not efficient... but we have very low n in O(n^2)
                        for (int j = 0; j < InputManager.MaxInputs; j++)
                        {
                            if (j != i)
                            {
                                if (LocalInstanceManager.Players[j].Active)
                                {
                                    dumb = p.UpdateOffset(LocalInstanceManager.Players[i]);
                                }
                            }
                        }
                        dumb = p.ApplyOffset();
                    }
                    else if (p.State == PlayerState.Dead)
                    {
                        p.Spawn();
                    }

                    p.Update(gameTime);
                }

                #region Bullet updates
                for (int j = 0; j < LocalInstanceManager.MaxNumberOfBulletsPerPlayer; j++)
                {
                    if (LocalInstanceManager.Bullets[i][j].Alive)
                    {
                        LocalInstanceManager.Bullets[i][j].Update(gameTime);
                        LocalInstanceManager.Bullets[i][j].StartOffset();
                        // Run through active enemies
                        // FIXME- Blech, is there a more efficient way?
                        for (int k = 0; k < LocalInstanceManager.CurrentNumberEnemies; k++)
                        {
                            if (LocalInstanceManager.Enemies[k] != null &&
                                LocalInstanceManager.Enemies[k].Alive)
                            {
                                LocalInstanceManager.Bullets[i][j].UpdateOffset(
                                    LocalInstanceManager.Enemies[k]);
                            }
                        }
                        LocalInstanceManager.Bullets[i][j].ApplyOffset();
                    }
                }
                #endregion
            }
            #endregion Player Stuff

            // FIXME as well... delete this fucker for release
            if (InstanceManager.InputManager.NewKeyPressed(Keys.End))
            {
                for (int j = 0; j < InputManager.MaxInputs; j++)
                {
                    if (LocalInstanceManager.Scores[j].Enabled)
                    {
                        LocalInstanceManager.Scores[j].GameEndCinematics();
                    }
                }
                LocalInstanceManager.CurrentGameState = GameState.EndCinematics;
                skip = true;
            }

            #region Enemy Stuff
            // Only do this stuff if we're not in wave intro
            if (myManager.CurrentState != GamePlayState.WaveIntro && !myManager.TutorialManager.TutorialOnscreen)
            {
                for (int i = 0; i < LocalInstanceManager.CurrentNumberEnemies; i++)
                {
                    if (LocalInstanceManager.Enemies[i] == null ||
                        !LocalInstanceManager.Enemies[i].Initialized)
                    {
                        LocalInstanceManager.Enemies[i].Initialize(
                            LocalInstanceManager.GenerateEnemyStartPos(
                            LocalInstanceManager.CurrentGameWave.Wavelets[LocalInstanceManager.CurrentGameWave.CurrentWavelet].StartAngle[i],
                            LocalInstanceManager.Enemies[i].RealSize.Length()/2f),
                            Vector2.Zero,
                            ColorState.GetColorStates()[LocalInstanceManager.CurrentGameWave.ColorState],
                            LocalInstanceManager.CurrentGameWave.Wavelets[LocalInstanceManager.CurrentGameWave.CurrentWavelet].ColorPolarities[i],
                            LocalInstanceManager.CurrentGameWave.Wavelets[LocalInstanceManager.CurrentGameWave.CurrentWavelet].StartHitPoints[i],
                            LocalInstanceManager.CurrentGameWave.Wavelets[LocalInstanceManager.CurrentGameWave.CurrentWavelet].SpawnDelay[i]);
                        livingEnemies++;
                    }
                    else if (LocalInstanceManager.Enemies[i].Alive)
                    {
                        if (LocalInstanceManager.Enemies[i].SpawnTimerElapsed)
                        {
                            dumb = LocalInstanceManager.Enemies[i].StartOffset();
                            // Update each enemy with player objects
                            for (int j = 0; j < InputManager.MaxInputs; j++)
                            {
                                if (LocalInstanceManager.Players[j].Active &&
                                    LocalInstanceManager.Players[j].State == PlayerState.Alive)
                                {
                                    dumb = LocalInstanceManager.Enemies[i].UpdateOffset(LocalInstanceManager.Players[j]);
                                }
                            }
                            // Update the enemy with remaining enemy objects

                            for (int j = i + 1; j < LocalInstanceManager.CurrentNumberEnemies; j++)
                            {
                                if (LocalInstanceManager.Enemies[j] != null &&
                                    LocalInstanceManager.Enemies[j].Initialized &&
                                    LocalInstanceManager.Enemies[j].Alive)
                                {
                                    dumb = LocalInstanceManager.Enemies[i].UpdateOffset(LocalInstanceManager.Enemies[j]);
                                }
                            }
                            dumb = LocalInstanceManager.Enemies[i].ApplyOffset();

                            LocalInstanceManager.Enemies[i].Update(gameTime);
                            LocalInstanceManager.Enemies[i].InnerUpdate(gameTime);
                        }
                        else
                        {
                            LocalInstanceManager.Enemies[i].SpawnTimer += gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        livingEnemies++;
                    }
                }
            }
            #endregion Enemy Stuff

            if (activePlayers < 1 && myManager.CurrentState != GamePlayState.GameOver)
            {
                // Game Over
                myManager.GameOver(true);
            }

            // If we have no living enemies, it means we need to get them from the next wavelet,
            // or move to next wave
            if ((livingEnemies < 1 && livingPlayers > 0 && !myManager.TutorialManager.TutorialOnscreen) || skip)
            {
                if (skip)
                    myManager.CleanEnemies();

                if (myManager.CurrentState == GamePlayState.Playing)
                    LocalInstanceManager.CurrentGameWave.CurrentWavelet++;

                if (!WaveletInit.Initialize(myManager))
                {
                    // No further wavelets, move up to next wave
                    LocalInstanceManager.CurrentGameWave.CurrentWavelet--;
                    myManager.GetNextWave();
                    myManager.SetNextMusic();
                    WaveletInit.Initialize(myManager);
                    myManager.NextTutorial();
                }
                else
                {
                    myManager.SetNextMusic();
                    myManager.NextTutorial();
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // First, run through the players
            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                Player p = LocalInstanceManager.Players[i];
                if (p.Active)
                {
                    p.Draw(gameTime);
                }
                for (int j = 0; j < LocalInstanceManager.MaxNumberOfBulletsPerPlayer; j++)
                {
                    if (LocalInstanceManager.Bullets[i][j].Alive)
                        LocalInstanceManager.Bullets[i][j].Draw(gameTime);
                }
            }

            // Next, run through the enemies
            for (int i = 0; i < LocalInstanceManager.CurrentNumberEnemies; i++)
            {
                Enemy e = LocalInstanceManager.Enemies[i];
                if (e.Alive && e.SpawnTimerElapsed)
                {
                    e.Draw(gameTime);
                    e.InnerDraw(gameTime);
                }
            }

            base.Draw(gameTime);
        }
        #endregion
    }
}