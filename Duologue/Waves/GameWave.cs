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
#endregion

namespace Duologue.Waves
{
    /// <summary>
    /// The basic class defining a wave in the game
    /// </summary>
    public class GameWave
    {
        #region Constants
        /// <summary>
        /// Defines the maximum number of enemies in a given wave
        /// </summary>
        public const int maxEnemiesInWave = 200;

        /// <summary>
        /// The default max player shot types
        /// </summary>
        public const int maxPlayerShotTypes = 1;
        #endregion

        #region Fields
        #endregion

        #region Properties
        /// <summary>
        /// The major wave number of this wave
        /// </summary>
        public int MajorWaveNumber;

        /// <summary>
        /// The minor wave number of this wave
        /// </summary>
        public int MinorWaveNumber;

        /// <summary>
        /// The name of the wave
        /// </summary>
        public string Name;

        /// <summary>
        /// The list of possible player shop types in this GameWave
        /// </summary>
        public PlayerShotType[] PossiblePlayerShotTypes;

        /// <summary>
        /// The background for this GameWave
        /// </summary>
        public int Background;

        /// <summary>
        /// The color state for this wave
        /// </summary>
        public ColorState ColorState;
        #endregion

        #region Constructors
        /// <summary>
        /// Default, empty constructor
        /// </summary>
        public GameWave()
        {
        }

        /// <summary>
        /// Constructs a Game Wave object
        /// </summary>
        public GameWave(string name, int? numShotTypes, int background, int majorWaveNo, int minorWaveNo)
        {
            Name = name;
            if (numShotTypes == null)
                PossiblePlayerShotTypes = new PlayerShotType[maxPlayerShotTypes];
            else
                PossiblePlayerShotTypes = new PlayerShotType[(int)numShotTypes];
            Background = background;
            MajorWaveNumber = majorWaveNo;
            MinorWaveNumber = minorWaveNo;
        }
        #endregion
    }
}
