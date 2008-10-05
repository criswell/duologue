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
#endregion

namespace Duologue.Waves
{
    public enum PlayerShotType
    {
        Default,
    }

    public class GameWave
    {
        #region Fields
        #endregion

        #region Properties
        /// <summary>
        /// The name of the wave
        /// </summary>
        public string Name;

        /// <summary>
        /// The list of wavelets associated with this game wave
        /// </summary>
        public Wavelet[] Wavelets;

        /// <summary>
        /// The list of possible player shop types in this GameWave
        /// </summary>
        public PlayerShotType[] PossiblePlayerShotTypes;
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
        /// <param name="name">The name of this wave</param>
        /// <param name="numWavelets">The maximum number of wavelets associated with this wave</param>
        /// <param name="numShotTypes">The maximum number of shots associated with this wave</param>
        public GameWave(string name, int numWavelets, int numShotTypes)
        {
            Name = name;
            Wavelets = new Wavelet[numWavelets];
            PossiblePlayerShotTypes = new PlayerShotType[numShotTypes];
        }
        #endregion
    }
}
