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
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.UI;
#endregion

namespace Duologue.Screens
{
    public enum GamePlayState
    {
        WaveIntro,
        InitPlayerSpawn,
        Playing,
        LoadNextWave,
        GameOver,
    }
    /// <summary>
    /// The main game play screen which coordinates the various game elements
    /// </summary>
    public class GamePlayScreenManager : GameScreen
    {
        #region Constants
        #endregion

        #region Fields
        private Game localGame;
        private GameWaveManager gameWaveManager;
        private GameWave currentWave;
        private GamePlayState currentState;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public GamePlayScreenManager(Game game) : base(game)
        {
            localGame = game;
        }

        protected override void InitializeConstants()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        #endregion
    }
}
