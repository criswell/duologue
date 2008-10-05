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
    class GameWaveManager
    {
        #region Constants
        /// <summary>
        /// The maximum number of gamewaves
        /// </summary>
        public const int MaxNumOfGameWaves = 100;
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
                currentWaveIndex = Math.Max(0, Math.Min(value, MaxNumOfGameWaves));
            }
        }
        #endregion

        #region Constructor / Init
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        #endregion
    }
}
