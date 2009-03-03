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
        #region Constants
        private const float originPiOver4 = MathHelper.PiOver4;
        private const float origin3PiOver4 = 3f * MathHelper.PiOver4;
        private const float origin5PiOver4 = 5f * MathHelper.PiOver4;
        private const float origin7PiOver4 = 7f * MathHelper.PiOver4;
        #endregion

        #region Fields
        private static GameState currentGameState;
        private static GameState lastGameState;
        private static Vector2 centerOfScreen;
        //private static float screenRadius = 0f;

        private static float localPiOver4 = 0f;
        private static float local3PiOver4 = 0f;
        private static float local5PiOver4 = 0f;
        private static float local7PiOver4 = 0f;
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
        /// The local instance of the enemy splatter system
        /// </summary>
        public static EnemySplatterSystem EnemySplatterSystem;

        /// <summary>
        /// The local instance of the background object
        /// </summary>
        public static Background Background;

        /// <summary>
        /// The local achievement manager
        /// </summary>
        public static AchievementManager AchievementManager;

        /// <summary>
        /// The local window manager
        /// </summary>
        public static WindowManager WindowManager;

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

        /// <summary>
        /// True if the system is paused, false otherwise
        /// </summary>
        public static bool Pause;
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
            float localAngle;
            //float converter;
            float startRadius;

            // Get centerOfScreen if we don't have it yet
            if(centerOfScreen == Vector2.Zero)
            {
                centerOfScreen = new Vector2(
                    InstanceManager.DefaultViewport.Width/2f,
                    InstanceManager.DefaultViewport.Height/2f);
                //centerOfScreen.X = -centerOfScreen.X;
                Console.WriteLine(centerOfScreen.ToString());
            }

            // Get the corners if we don't have them yet
            if (localPiOver4 == 0f)
            {
                localPiOver4 = MWMathHelper.ComputeAngleAgainstX(
                    new Vector2(
                        InstanceManager.DefaultViewport.Width,
                        InstanceManager.DefaultViewport.Height),
                    centerOfScreen);

                local3PiOver4 = MWMathHelper.ComputeAngleAgainstX(
                    new Vector2(
                        0f,
                        InstanceManager.DefaultViewport.Height),
                    centerOfScreen);

                local5PiOver4 = MWMathHelper.ComputeAngleAgainstX(
                    new Vector2(
                        0f,
                        0f),
                    centerOfScreen) + MathHelper.TwoPi;

                local7PiOver4 = MWMathHelper.ComputeAngleAgainstX(
                    new Vector2(
                        InstanceManager.DefaultViewport.Width,
                        0f),
                    centerOfScreen) + MathHelper.TwoPi;

                Console.WriteLine(
                    String.Format("({0}={1}), ({2}={3}), ({4}={5}), ({6}={7})", 
                        originPiOver4, localPiOver4, 
                        origin3PiOver4, local3PiOver4, 
                        origin5PiOver4, local5PiOver4, 
                        origin7PiOver4, local7PiOver4));
            }

            // Convert the angle to our local coordinate system and get intersect points
            if (angleRad >= 0f && angleRad < originPiOver4)
            {
                // In the upper right corner of the screen
                //converter = localPiOver4 / originPiOver4;
                localAngle = localPiOver4 * (angleRad / originPiOver4);
                // Intersection would be (x,y) = (DV.Width, y)
                // so r = DV.Width / COS localAngle
                startRadius = centerOfScreen.X / (float)Math.Cos(localAngle);
            }
            else if (angleRad >= originPiOver4 && angleRad < origin3PiOver4)
            {
                // Top of screen
                //converter = (local3PiOver4 - localPiOver4) / (origin3PiOver4 - originPiOver4);
                localAngle = local3PiOver4 * (angleRad / origin3PiOver4);// +localPiOver4;
                // Intersection would be (x,y) = (x, 0)
                // so r = DV.Height / SIN localAngle
                startRadius = centerOfScreen.Y / (float)Math.Sin(localAngle);
            }
            else if (angleRad >= origin3PiOver4 && angleRad < origin5PiOver4)
            {
                // Right of screen
                //converter = local5PiOver4 /origin5PiOver4;
                localAngle = local5PiOver4 * (angleRad /origin5PiOver4);// +local3PiOver4;
                // Intersection would be (x,y) = (0, y)
                // so r = DV.Width / COS localAngle
                startRadius = centerOfScreen.X / (float)Math.Cos(localAngle);
            }
            else if (angleRad >= origin5PiOver4 && angleRad < origin7PiOver4)
            {
                // Bottom of screen
                //converter = local7PiOver4 / origin7PiOver4;
                localAngle = local7PiOver4 * (angleRad / origin7PiOver4);// + local5PiOver4;
                // Intersection would be (x,y) = (x, DV.Height)
                // so r = DV.Height / SiIN localAngle
                startRadius = centerOfScreen.Y / (float)Math.Sin(localAngle);
            }
            else
            {
                // Only thing left is the lower right corner of the screen
                //converter = (MathHelper.TwoPi - local7PiOver4) / (MathHelper.TwoPi - origin7PiOver4);
                localAngle = angleRad;// + local7PiOver4;
                // Intersection would be (x,y) = (DV.Width, y)
                // so r = DV.Width / COS localAngle
                startRadius = centerOfScreen.X / (float)Math.Cos(localAngle);
            }

            Console.WriteLine(String.Format("From {0} To {1}", MathHelper.ToDegrees(angleRad), MathHelper.ToDegrees(localAngle)));

            // Get the start radius
            startRadius = radius + Math.Abs(startRadius);

            startPos = new Vector2(
                startRadius * (float)Math.Cos((double)localAngle),
                startRadius * -1 * (float)Math.Sin((double)localAngle)) +centerOfScreen;

            Console.WriteLine(String.Format("(x,y)=({0}, {1})",
                startPos.X,
                startPos.Y));

            //InstanceManager.Logger.LogEntry(String.Format("centerOfScreen: {0} radius: {1}", centerOfScreen.ToString(), radius.ToString()));

            // Get the screenRadius if we dont have it yet
            /*if (screenRadius < 1f)
            {
                //screenRadius = (float)Math.Sqrt((double)(centerOfScreen.X * centerOfScreen.X + centerOfScreen.Y * centerOfScreen.Y));
                screenRadius = centerOfScreen.Length();
            }*/

            //InstanceManager.Logger.LogEntry(String.Format("screenRadius: {0}", screenRadius.ToString()));

            // Now, make that a vector pointing in the opposite direction at the screenRadius distance
            // FIXME - We're still spawning the enemies in visible screen
            /*startPos = new Vector2(
                (screenRadius + radius) * (float)Math.Cos((double)angleRad),
                (screenRadius + radius) * -1 * (float)Math.Sin((double)angleRad));

            //InstanceManager.Logger.LogEntry(String.Format("startPos(pre): {0}", startPos.ToString()));

            //startPos = Vector2.Reflect(Vector2.Negate(startPos),Vector2.UnitX)+ centerOfScreen;
            startPos = startPos + centerOfScreen;*/

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
        /// 4           -       GamePlayItems
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
