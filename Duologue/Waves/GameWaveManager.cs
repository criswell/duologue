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
using Duologue.UI;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.State;
#endregion

namespace Duologue.Waves
{
    /// <summary>
    /// The manager of the game waves
    /// </summary>
    public class GameWaveManager
    {
        #region Constants
        /// <summary>
        /// The maximum number of gamewaves
        /// </summary>
        public const int MaxNumOfGameWaves = 100;

        /// <summary>
        /// The maximum number for minor wave numbers
        /// </summary>
        public const int MaxMinorNumber = 10;

        /// <summary>
        /// The maxium number for major wave numbers
        /// </summary>
        public const int MaxMajorNumber = 999;
        #endregion

        #region Fields
        /// <summary>
        /// The list of game waves this manager owns
        /// </summary>
        private List<GameWave> waves;

        /// <summary>
        /// The current wave we are on
        /// </summary>
        private int currentWaveIndex;

        private Random rand;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current wave index
        /// Note: If you try to set this outside the possible range, it will
        /// default to 0 or MaxNumofGameWaves
        /// </summary>
        public int CurrentWaveIndex
        {
            get { return currentWaveIndex; }
            set
            {
                currentWaveIndex = Math.Max(0, Math.Min(value, waves.Count-1));
            }
        }

        /// <summary>
        /// The current wave's Major Number
        /// </summary>
        public int CurrentMajorNumber;

        /// <summary>
        /// The current wave's Minor number
        /// </summary>
        public int CurrentMinorNumber;

        #endregion

        #region Constructor / Init
        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="maxNumOfGameWaves"></param>
        public GameWaveManager(int? maxNumOfGameWaves)
        {
            rand = new Random();
            if (maxNumOfGameWaves == null)
            {
                waves = new List<GameWave>(MaxNumOfGameWaves);
            }
            else
            {
                waves = new List<GameWave>((int)maxNumOfGameWaves);
            }

            // Sensible defaults
            CurrentMajorNumber = 0;
            CurrentMinorNumber = 0;
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Generate a random wave
        /// </summary>
        /// <returns>A random game wave</returns>
        public GameWave GenerateRandomWave(int lastMajorWaveNo, int lastMinorWaveNo)
        {
            lastMinorWaveNo++;
            if (lastMinorWaveNo > MaxMinorNumber)
            {
                lastMinorWaveNo = 0;
                lastMajorWaveNo++;
                if (lastMajorWaveNo > MaxMajorNumber)
                {
                    lastMajorWaveNo = 0;
                    // FIXME: Need an achievement here, I think
                }
            }

            // Get a random color state
            ColorState[] theStates = ColorState.GetColorStates();

            GameWave thisWave = new GameWave(Resources.GameScreen_InfiniteWave,
                GameWave.maxPlayerShotTypes,
                rand.Next(LocalInstanceManager.Background.NumBackgrounds),
                theStates[rand.Next(theStates.Length)],
                lastMajorWaveNo,
                lastMinorWaveNo);

            // Okay, this is gonna get fugly... sorry folks
            // FIXME: If you feel adventurous enough
            // .
            // Okay, we'll divide up every ten levels, beyond 100 they just repeat
            /*if (lastMajorWaveNo < 10)
            {

            }*/

            // ERE I AM JH
            thisWave.NumWavelets = 1;
            thisWave.NumEnemies = 25;
            thisWave.CurrentWavelet = 0;
            thisWave.Wavelet = new Wavelet[thisWave.NumWavelets];
            thisWave.Wavelet[thisWave.CurrentWavelet] = new Wavelet(thisWave.NumEnemies, 0);
            for (int i = 0; i < thisWave.NumEnemies; i++)
            {
                // For now, we're just spawning all buzzsaws
                thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[i] = (int)TypesOfPlayObjects.Enemy_Buzzsaw;
            }

            return thisWave;
        }

        /// <summary>
        /// Get the next wave
        /// </summary>
        /// <returns></returns>
        public GameWave GetNextWave()
        {
            GameWave temp;
            if (LocalInstanceManager.CurrentGameState == Duologue.State.GameState.InfinityGame)
            {
                temp = GenerateRandomWave(CurrentMajorNumber, CurrentMinorNumber);
                CurrentMajorNumber = temp.MajorWaveNumber;
                CurrentMinorNumber = temp.MinorWaveNumber;
            }
            else
            {
                // FIXME
                // Need to handle campaign here
                temp = new GameWave();
            }
            return temp;
        }
        #endregion
    }
}
