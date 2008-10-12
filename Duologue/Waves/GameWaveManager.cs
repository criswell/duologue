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
            return new GameWave(Resources.GameScreen_InfiniteWave,
                GameWave.maxPlayerShotTypes,
                rand.Next(LocalInstanceManager.Background.NumBackgrounds),
                lastMajorWaveNo,
                lastMinorWaveNo);
        }
        #endregion
    }
}
