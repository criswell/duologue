using System;
using System.Collections.Generic;
using System.Text;

namespace Duologue.AchievementManager
{
    /// <summary>
    /// Represents an individual achievement in the game
    /// </summary>
    class Achievement
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
        /// The points this achievement is worth
        /// </summary>
        public int Points;
        /// <summary>
        /// Whether or not this player has unlocked it
        /// </summary>
        public bool Unlocked;
        /// <summary>
        /// Whether or not we've displayed this achievement
        /// </summary>
        public bool Displayed;
        #endregion

        #region Construct / Init
        /// <summary>
        /// Default constructor
        /// </summary>
        public Achievement()
        {
            Name = null;
            Description = null;
            Points = 0;
            Unlocked = false;
            Displayed = false;
        }

        /// <summary>
        /// Constructor that pre-populates the achievement
        /// </summary>
        /// <param name="name">Name of the achievement</param>
        /// <param name="description">Description of the achievement</param>
        /// <param name="points">Points this achievement is worth</param>
        public Achievement(string name, string description, int points)
        {
            Name = name;
            Description = description;
            Unlocked = false;
            Displayed = false;
            Points = points;
        }
        #endregion
    }
}
