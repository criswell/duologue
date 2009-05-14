#region File Description
#endregion

#region Using statements
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
#endregion

namespace Duologue.AchievementSystem
{
    [Serializable]
    public struct AchievementData
    {
        public bool[] MedalEarned;
        public int NumberOfEnemiesKilled;
        public bool[] EnemyTypesKilled;
        public int DataVersion;
        public int NumberOfGamesPlayed;
    }
    /// <summary>
    /// Represents an individual achievement in the game
    /// </summary>
    public class Achievement
    {
        #region Properties
        /// <summary>
        /// The name of the achievement
        /// </summary>
        public string Name;
        /// <summary>
        /// The description of the achievement
        /// </summary>
        public string Description;
        /// <summary>
        /// Whether or not this player has unlocked it
        /// </summary>
        public bool Unlocked;
        /// <summary>
        /// Whether or not we've displayed this achievement
        /// </summary>
        public bool Displayed;
        /// <summary>
        /// The icon for this achievement
        /// </summary>
        public Texture2D Icon;

        /// <summary>
        /// The greyed out icon
        /// </summary>
        public Texture2D IconGrey;
        /// <summary>
        /// The weight of this achievement
        /// </summary>
        public int Weight;
        #endregion

        #region Construct / Init
        /// <summary>
        /// Default constructor
        /// </summary>
        public Achievement()
        {
            Name = null;
            Description = null;
            Unlocked = false;
            Displayed = false;
        }

        /// <summary>
        /// Constructor that pre-populates the achievement
        /// </summary>
        /// <param name="name">Name of the achievement</param>
        /// <param name="description">Description of the achievement</param>
        public Achievement(string name, string description)
        {
            Name = name;
            Description = description;
            Unlocked = false;
            Displayed = false;
        }
        #endregion
    }
}
