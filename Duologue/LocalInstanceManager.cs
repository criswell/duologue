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
// Mimicware
using Mimicware;
using Mimicware.Graphics;
using Mimicware.Manager;
using Mimicware.Debug;
using Mimicware.Fx;
// Duologue
using Duologue.ParticleEffects;
using Duologue.PlayObjects;
using Duologue.UI;
using Duologue.State;
using Duologue.AchievementSystem;
using Duologue.Waves;
#endregion

namespace Duologue
{
    static class LocalInstanceManager
    {
        #region Fields
        private static GameState currentGameState;
        private static GameState lastGameState;
        #endregion

        #region Properties / Local Instances
        /// <summary>
        /// The local instance of the steam particle system.
        /// </summary>
        public static Steam Steam;

        /// <summary>
        /// The local players
        /// </summary>
        public static Player[] Players;

        /// <summary>
        /// The local player index mapping
        /// </summary>
        public static PlayerIndex[] PlayersIndex;

        /// <summary>
        /// The maximum number of players
        /// </summary>
        public static int MaxNumberOfPlayers
        {
            get { return 4; }
        }

        /// <summary>
        /// The local instance of the player explosion ring system
        /// </summary>
        public static PlayerRing PlayerRing;

        /// <summary>
        /// The local instance of the background object
        /// </summary>
        public static Background Background;

        /// <summary>
        /// The local achievement manager
        /// </summary>
        public static AchievementManager AchievementManager;

        /// <summary>
        /// The current game state
        /// </summary>
        public static GameState CurrentGameState
        {
            get { return currentGameState; }
            set
            {
                lastGameState = currentGameState;
                currentGameState = value;
            }
        }

        /// <summary>
        /// The last game state
        /// </summary>
        public static GameState LastGameState
        {
            get { return lastGameState; }
        }

        /// <summary>
        /// Set this to the next game state
        /// </summary>
        public static GameState NextGameState;

        /// <summary>
        /// The currentl GameWave
        /// </summary>
        public static GameWave CurrentGameWave;

        /// <summary>
        /// The current Spinner instance
        /// </summary>
        public static Spinner Spinner;
        #endregion

        #region Public methods
        /// <summary>
        /// Will initialize the player indexes
        /// </summary>
        internal static void InitializePlayers()
        {
            if (Players == null)
                Players = new Player[MaxNumberOfPlayers];
            if (PlayersIndex == null)
                PlayersIndex = new PlayerIndex[MaxNumberOfPlayers];
        }
        #endregion
    }
}
