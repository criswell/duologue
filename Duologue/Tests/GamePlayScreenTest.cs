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

namespace Duologue.Tests
{
    public class GamePlayScreenTest : GameScreen
    {
        #region Constants
        #endregion

        #region Fields
        private Game localGame;
        // The background elements
        private Background background;
        // The UI elements
        private ScoreScroller[] scores;
        // The Player Elements
        private Player[] players;
        private PlayerIndex[] playerIndex;
        // The Enemies

        // The GameWaves
        private List<GameWave> gameWaves;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public GamePlayScreenTest(Game game)
            : base(game)
        {
            localGame = game;
            // Initialize the players
            LocalInstanceManager.InitializePlayers();
            players = LocalInstanceManager.Players;
            playerIndex = LocalInstanceManager.PlayersIndex;

            // Initialize the scores
            scores = new ScoreScroller[LocalInstanceManager.MaxNumberOfPlayers];
            for(int i = 0; i < LocalInstanceManager.MaxNumberOfPlayers; i++)
            {
                scores[i] = new ScoreScroller(
                    localGame,
                    i,
                    1f,
                    Vector2.Zero,
                    Vector2.Zero,
                    0,
                    1f);
                scores[i].Enabled = false;
                scores[i].Visible = false;
                localGame.Components.Add(scores[i]);
            }

            // Initialize the GameWaves
            gameWaves = LocalInstanceManager.GameWaves;
        }

        protected override void InitializeConstants()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        #endregion

        #region Update
        #endregion
    }
}
