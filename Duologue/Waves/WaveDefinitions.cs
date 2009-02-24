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
using Duologue.PlayObjects;
using Duologue.Waves;
#endregion

namespace Duologue.Waves
{
    /// <summary>
    /// For now, we'll just be storing these in memory. At a later date, we likely will
    /// want these to be stored on disk
    /// </summary>
    public class WaveDefinitions
    {
        #region Constants
        private int numberOfWaves = 20;
        #endregion

        #region Fields
        /// <summary>
        /// The waves used in this game
        /// </summary>
        private GameWave[] Waves;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs the wave def object
        /// </summary>
        public WaveDefinitions()
        {
            // For now, this is all in-memory, we may eventually want to dump this crap to a file
            // FIXME
            Waves = new GameWave[numberOfWaves];

            Waves[GetIndex(1, 1)] = new GameWave();
            Waves[GetIndex(1, 1)].Background = 0;
            Waves[GetIndex(1, 1)].ColorState = 0;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the wave specified by major and minor numbers
        /// </summary>
        /// <param name="MajorNum">The major number of the wave</param>
        /// <param name="MinorNum">The minor number of the wave. Must be 1-4. If greater than 4, or less than 1, will be capped.</param>
        /// <returns>The gamewave specified by the major and minor numbers</returns>
        public GameWave GetWave(int MajorNum, int MinorNum)
        {
            return Waves[GetIndex(MajorNum, MinorNum)];
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Given a major and minor number for a game wave, return the index for it
        /// </summary>
        private int GetIndex(int MajorNum, int MinorNum)
        {
            if (MinorNum > 4)
                MinorNum = 4;
            else if (MinorNum < 1)
                MinorNum = 1;
            int index = (MajorNum * 4) - 5 + MinorNum;
            if (index >= numberOfWaves)
                index = numberOfWaves - 1;
            return index;
        }
        #endregion
    }
}
