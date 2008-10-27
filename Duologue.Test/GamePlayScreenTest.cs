//System
using System;
using System.Collections.Generic;
using System.Text;
//Xna
using Microsoft.Xna.Framework;
//Mimicware
//Duologue
using Duologue;
using Duologue.PlayObjects;
using Duologue.UI;
using Duologue.Waves;
//MbUnit
using MbUnit.Framework;

namespace Duologue.Test
{
    public
    class GamePlayScreenTest
    {
        [Test]
        public void runatestmethod()
        {
            Assert.IsNotNull(this);
        }

        [Test]
        public void wreck_of_the_edmund_fitzgerald()
        {
            Game localGame = new Game();
            Assert.IsNotNull(localGame);

            // Initialize the players
            LocalInstanceManager.InitializePlayers();
            Player[] players = LocalInstanceManager.Players;
            PlayerIndex[] playerIndex = LocalInstanceManager.PlayersIndex;

            //// Initialize the scores
            ScoreScroller[] scores = new ScoreScroller[LocalInstanceManager.MaxNumberOfPlayers];
            for (int i = 0; i < LocalInstanceManager.MaxNumberOfPlayers; i++)
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
            GameWave gameWave = LocalInstanceManager.CurrentGameWave;
        }
    }
}
