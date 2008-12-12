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
    /// A Wavelet is a segment of a larger wave which contains distinct enemies that
    /// must be destroyed before moving on to the next wavelet.
    /// All enemies in the wavelet will spawn and be on screen at the same time.
    /// </summary>
    public class Wavelet
    {
        #region Properties
        /// <summary>
        /// The enemies in this wavelet
        /// </summary>
        public TypesOfPlayObjects[] Enemies;

        /// <summary>
        /// The starting hit points for each enemy in this wavelet
        /// </summary>
        public int StartHitPoints;

        /// <summary>
        /// the starting angle for each enemy
        /// </summary>
        public float[] StartAngle;
        #endregion

        #region Constructor
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Wavelet()
        {
        }
        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="NumEnemies">The Number of enemies to initialize</param>
        /// <param name="StartHP">The starting HP for said enemies</param>
        public Wavelet(int NumEnemies, int StartHP)
        {
            Enemies = new TypesOfPlayObjects[NumEnemies];
            StartAngle = new float[NumEnemies];
            StartHitPoints = StartHP;
        }
        #endregion
    }
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

        /// <summary>
        /// The number of enemies each wavelet
        /// </summary>
        public int NumEnemies;

        /// <summary>
        /// The number of wavelets we have
        /// </summary>
        public int NumWavelets;

        /// <summary>
        /// The current wavelet
        /// </summary>
        public int CurrentWavelet;

        /// <summary>
        /// The wavelets of enemies we'll be fighting this wave
        /// </summary>
        public Wavelet[] Wavelet;
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
        public GameWave(string name,
            int? numShotTypes,
            int background,
            ColorState colorState,
            int majorWaveNo, int minorWaveNo)
        {
            Name = name;
            if (numShotTypes == null)
                PossiblePlayerShotTypes = new PlayerShotType[maxPlayerShotTypes];
            else
                PossiblePlayerShotTypes = new PlayerShotType[(int)numShotTypes];
            Background = background;
            MajorWaveNumber = majorWaveNo;
            MinorWaveNumber = minorWaveNo;
            ColorState = colorState;
        }
        #endregion
    }
}
