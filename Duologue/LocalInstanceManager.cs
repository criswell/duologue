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
    public static class LocalInstanceManager
    {
        #region Fields
        private static GameState currentGameState;
        private static GameState lastGameState;
        private static Vector2 centerOfScreen;
        private static float screenRadius = 0f;
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
        /// The local score scrollers
        /// </summary>
        public static ScoreScroller[] Scores;

        /// <summary>
        /// The local bullet arrays
        /// </summary>
        public static PlayerBullet[][] Bullets;

        /// <summary>
        /// The number of bullets per player
        /// </summary>
        public static int MaxNumberOfBulletsPerPlayer
        {
            get { return 10; }
        }

        /// <summary>
        /// The maximum number of players
        /// </summary>
        public static int MaxNumberOfPlayers
        {
            get { return InputManager.MaxInputs; }
        }

        /// <summary>
        /// The local enemies
        /// </summary>
        public static Enemy[] Enemies;

        /// <summary>
        /// The maximum number of on screen enemies
        /// </summary>
        public static int MaxNumberOfEnemiesOnScreen
        {
            get { return 100; }
        }

        /// <summary>
        /// The current number of enemies
        /// NOTE: This needs to be less than MaxNumberOfEnemiesOnScreen!
        /// </summary>
        public static int CurrentNumberEnemies;

        /// <summary>
        /// The local instance of the player explosion ring system
        /// </summary>
        public static PlayerRing PlayerRing;

        /// <summary>
        /// The local instance of the player explosion system
        /// </summary>
        public static PlayerExplosion PlayerExplosion;

        /// <summary>
        /// The local instance of the player smoke system
        /// </summary>
        public static PlayerSmoke PlayerSmoke;

        /// <summary>
        /// The local instance of the bullet particle system
        /// </summary>
        public static BulletParticle BulletParticle;

        /// <summary>
        /// The local instance of the enemy explosion system
        /// </summary>
        public static EnemyExplodeSystem EnemyExplodeSystem;

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

            if (Bullets == null)
                Bullets = new PlayerBullet[MaxNumberOfPlayers][];

            for (int i = 0; i < MaxNumberOfPlayers; i++)
            {
                Players[i] = new Player();
                Bullets[i] = new PlayerBullet[MaxNumberOfBulletsPerPlayer];
                for (int t = 0; t < MaxNumberOfBulletsPerPlayer; t++)
                {
                    Bullets[i][t] = new PlayerBullet();
                }
            }
        }

        /// <summary>
        /// Will generate a starting position for an enemy
        /// </summary>
        /// <param name="angleRad">The angle (in radians) of the start pos</param>
        /// <param name="radius">The radius of the enemy</param>
        /// <returns>Starting position</returns>
        internal static Vector2 GenerateEnemyStartPos(float angleRad, float radius)
        {
            Vector2 startPos;

            // Get centerOfScreen if we don't have it yet
            if(centerOfScreen == Vector2.Zero)
            {
                centerOfScreen = new Vector2(
                    InstanceManager.DefaultViewport.Width/2f,
                    InstanceManager.DefaultViewport.Height/2f);
            }

            //InstanceManager.Logger.LogEntry(String.Format("centerOfScreen: {0} radius: {1}", centerOfScreen.ToString(), radius.ToString()));

            // Get the screenRadius if we dont have it yet
            if (screenRadius < 1f)
            {
                //screenRadius = (float)Math.Sqrt((double)(centerOfScreen.X * centerOfScreen.X + centerOfScreen.Y * centerOfScreen.Y));
                screenRadius = centerOfScreen.Length();
            }

            //InstanceManager.Logger.LogEntry(String.Format("screenRadius: {0}", screenRadius.ToString()));

            // Now, make that a vector pointing in the opposite direction at the screenRadius distance
            // FIXME - We're still spawning the enemies in visible screen
            startPos = new Vector2(
                (screenRadius + radius) * (float)Math.Cos((double)angleRad),
                (screenRadius + radius) * -1 * (float)Math.Sin((double)angleRad));

            //InstanceManager.Logger.LogEntry(String.Format("startPos(pre): {0}", startPos.ToString()));

            //startPos = Vector2.Reflect(Vector2.Negate(startPos),Vector2.UnitX)+ centerOfScreen;
            startPos = startPos + centerOfScreen;

            //InstanceManager.Logger.LogEntry(String.Format("startPos: {0}", startPos.ToString()));
            //InstanceManager.Logger.LogEntry("-------");


            return startPos;
        }
        #endregion

        #region Layer and Draw Order information
        /// <remarks>
        /// -----------
        /// BLIT LAYERS
        /// -----------
        /// 1.0f        -       Background
        ///             -       Beam, shot items
        /// 0.9f
        /// 0.8f        -       Spinner (default)
        /// 0.7f        -       Enemies (default)
        /// 0.6f        -       Enemies (additional elements)
        /// 0.5f        -       Players (default)
        /// 0.4f        -       Players (additional elements, tracks, etc)
        /// 0.3f
        /// 0.2f
        /// 0.1f
        /// 0.0f
        /// </remarks>

        public const float BlitLayer_EnemyBase = 0.7f;
        public const float BlitLayer_PlayerBase = 0.5f;

        /// <remarks>
        /// ----------
        /// DRAW ORDER
        /// ----------
        /// 1           -       Background
        /// 2
        /// 3
        /// 4
        /// 5
        /// 6
        /// 7           -       Explosion and steam elements
        /// 8           -       Bullet particle effects
        /// 9
        /// ...
        /// 99          -       Score scroller
        /// 100         -       Spinner
        /// 200         -       Logger
        /// </remarks>
        #endregion
    }
}
