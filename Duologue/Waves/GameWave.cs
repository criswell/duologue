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
using Duologue.Audio;
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

        public int NumEnemies
        {
            get { return Enemies.Length; }
        }

        /// <summary>
        /// The starting hit points for each enemy in this wavelet
        /// </summary>
        public int[] StartHitPoints;

        /// <summary>
        /// the starting angle for each enemy
        /// </summary>
        public float[] StartAngle;

        /// <summary>
        /// the spawn delays for each enemy
        /// </summary>
        public double[] SpawnDelay;

        /// <summary>
        /// The color polarities for each enemy
        /// </summary>
        public ColorPolarity[] ColorPolarities;

        public SongID SongID;
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
        /// <param name="polarity">The color polarities for each enemy</param>
        public Wavelet(int NumEnemies, int StartHP, ColorPolarity polarity)
        {
            Enemies = new TypesOfPlayObjects[NumEnemies];
            StartAngle = new float[NumEnemies];
            StartHitPoints = new int[NumEnemies];
            ColorPolarities = new ColorPolarity[NumEnemies];
            SpawnDelay = new double[NumEnemies];
            for (int i = 0; i < NumEnemies; i++)
            {
                StartHitPoints[i] = StartHP;
                ColorPolarities[i] = polarity;
                SpawnDelay[i] = 0;
            }
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
            StartHitPoints = new int[NumEnemies];
            ColorPolarities = new ColorPolarity[NumEnemies];
            SpawnDelay = new double[NumEnemies];
            for (int i = 0; i < NumEnemies; i++)
            {
                StartHitPoints[i] = StartHP;
                ColorPolarities[i] = ColorPolarity.Positive;
                SpawnDelay[i] = 0;
            }
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
        /// The background for this GameWave
        /// </summary>
        public int Background;

        /// <summary>
        /// The color of the throb
        /// </summary>
        public Color ThrobColor;

        /// <summary>
        /// The top parallax element for this GameWave
        /// </summary>
        public ParallaxElement ParallaxElementTop;

        /// <summary>
        /// The bottom parallax element for this GameWave
        /// </summary>
        public ParallaxElement ParallaxElementBottom;

        /// <summary>
        /// The color state for this wave
        /// </summary>
        public int ColorState;

        /// <summary>
        /// The current wavelet
        /// </summary>
        public int CurrentWavelet;

        /// <summary>
        /// The wavelets of enemies we'll be fighting this wave
        /// </summary>
        public Wavelet[] Wavelets;

        public int NumWavelets
        {
            get { return Wavelets.Length; }
        }

        public int NumEnemies
        {
            get { return Wavelets[CurrentWavelet].NumEnemies; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default, empty constructor
        /// </summary>
        public GameWave()
        {
            CurrentWavelet = 0;
            ThrobColor = Color.WhiteSmoke;
        }

        /// <summary>
        /// Constructs a Game Wave object
        /// </summary>
        public GameWave(string name,
            int background,
            int colorState,
            int majorWaveNo, int minorWaveNo)
        {
            Name = name;
            Background = background;
            MajorWaveNumber = majorWaveNo;
            MinorWaveNumber = minorWaveNo;
            ColorState = colorState;
            CurrentWavelet = 0;
            ThrobColor = Color.WhiteSmoke;
            ParallaxElementTop = LocalInstanceManager.Background.EmptyParallaxElement;
            ParallaxElementBottom = LocalInstanceManager.Background.EmptyParallaxElement;
        }
        #endregion
    }
}
