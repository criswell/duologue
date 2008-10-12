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
        #region Fields
        #endregion

        #region Properties
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
        public GameWave(string name, int numShotTypes, int background)
        {
            Name = name;
            PossiblePlayerShotTypes = new PlayerShotType[numShotTypes];
            Background = background;
        }
        #endregion
    }
}
