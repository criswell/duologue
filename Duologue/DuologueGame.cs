#region File Description
#endregion

#region Using statements
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
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
// Duologue
using Duologue.ParticleEffects;
using Duologue.AchievementSystem;
using Duologue.Tests;
using Duologue.State;
using Duologue.Screens;
using Duologue.UI;
using Duologue.Audio;
#endregion

namespace Duologue
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DuologueGame : Microsoft.Xna.Framework.Game
    {
        #region Constants
        /// <summary>
        /// FIXME
        /// </summary>
        public const int MaxSteamEffects = 4;
        /// <summary>
        /// FIXME
        /// </summary>
        public const int MaxExplosionEffects = 5;
        /// <summary>
        /// FIXME - We want this zero?
        /// </summary>
        public const float scoreMoveTime = 0f;
        /// <summary>
        /// FIXME
        /// </summary>
        public const float scoreScrollTime = 1f;
        /// <summary>
        /// We will only ever have 4 players
        /// </summary>
        public const int MaxPlayerExplosionEffects = 4;
        /// <summary>
        /// Maximum smoke effects for player explosions
        /// </summary>
        public const int MaxPlayerSmokeEffects = 8;
        /// <summary>
        /// Maximum number of bullet particle effects
        /// </summary>
        public const int MaxBulletParticles = 30;

        /// <summary>
        /// Maximum number of enemy explosion effects
        /// </summary>
        public const int MaxEnemyExplosions = 20;
        #endregion

        #region Fields
        private bool debug;
        #endregion

        #region Properties
        #endregion
        public GraphicsDeviceManager Graphics;
        public AudioManager Audio;
        SpriteBatch spriteBatch;
        //GamePlayTest gamePlayTest;
        public AssetManager Assets;
        public RenderSprite Render;
        public Logger Log;
        public Steam steamSystem;
        public PlayerRing playerRing;
        public PlayerExplosion playerExplosion;
        public EnemyExplodeSystem enemyExplodeSystem;
        public PlayerSmoke playerSmoke;
        public BulletParticle bulletParticle;
        public Background background;
        public AchievementManager achievements;
        public Spinner spinner;
        public ColorStateTestScreen colorStateTest;

        // Screens
        //public MainMenuTest mainMenuTest;
        public MainMenuScreen mainMenuScreen;
        public ExitScreen exitScreen;
        public PlayerSelectScreen playerSelectScreen;
        public GamePlayScreenManager gamePlayScreenManager;

        /// <summary>
        /// The dispatch table for game state changes
        /// </summary>
        public Dictionary<GameState, GameScreen> dispatchTable;

        /// <summary>
        /// Determines whether we are in debugging mode or not
        /// </summary>
        public bool Debug
        {
            get { return debug; }
            set
            {
                debug = value;
                Log.Enabled = value;
                Log.Visible = value;
            }
        }

        public DuologueGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Components.Add(new GamerServicesComponent(this));
            debug = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Determine what our aspect ratio is, and set resolution accordingly
            // FIXME
            // BAH! We really should check for available modes and not just do this
            DisplayMode currentMode = GraphicsDevice.DisplayMode;
            if (currentMode.AspectRatio > 1.25)
            {
                // Set to 720p (1280×720)
                Graphics.PreferredBackBufferWidth = 1280;
                Graphics.PreferredBackBufferHeight = 720;
            }
            else
            {
                // Set to SDTV (1024x768- not really SDTV, but 360 will scale for us)
                Graphics.PreferredBackBufferWidth = 1024;
                Graphics.PreferredBackBufferHeight = 768;
            }
            Graphics.ApplyChanges();

            // FIXME:
            // In the final game, the enable/visible stuff set here should be in a game state engine
            Assets = new AssetManager(Content);

            background = new Background(this);
            background.Visible = true;
            background.Enabled = true;
            this.Components.Add(background);
            
            /*
            gamePlayTest = new GamePlayTest(this);
            gamePlayTest.Enabled = true;
            gamePlayTest.Visible = true;
            this.Components.Add(gamePlayTest);*/
            Log = new Logger(this);
            Log.Enabled = false;
            Log.Visible = false;
            this.Components.Add(Log);
            Log.DrawOrder = 200;

            spinner = new Spinner(this);
            spinner.Enabled = false;
            spinner.Visible = false;
            this.Components.Add(spinner);
            spinner.DrawOrder = 100;

            steamSystem = new Steam(this, MaxSteamEffects);
            /*steamSystem.Enabled = true;
            steamSystem.Visible = true;*/
            this.Components.Add(steamSystem);
            steamSystem.DrawOrder = 7;

            playerRing = new PlayerRing(this, MaxExplosionEffects);
            /*playerRing.Enabled = true;
            playerRing.Visible = true;*/
            this.Components.Add(playerRing);
            playerRing.DrawOrder = 7;

            playerExplosion = new PlayerExplosion(this, MaxPlayerExplosionEffects);
            this.Components.Add(playerExplosion);
            playerExplosion.DrawOrder = 7;

            enemyExplodeSystem = new EnemyExplodeSystem(this, MaxEnemyExplosions);
            this.Components.Add(enemyExplodeSystem);
            enemyExplodeSystem.DrawOrder = 7;

            playerSmoke = new PlayerSmoke(this, MaxPlayerSmokeEffects);
            this.Components.Add(playerSmoke);
            playerSmoke.DrawOrder = 7;

            bulletParticle = new BulletParticle(this, MaxBulletParticles);
            bulletParticle.Enabled = true;
            bulletParticle.Visible = true;
            this.Components.Add(bulletParticle);
            bulletParticle.DrawOrder = 8;

            achievements = new AchievementManager(this);
            /*achievements.Enabled = true;
            achievements.Visible = true;*/
            this.Components.Add(achievements);

            // Set the instance manager
            InstanceManager.AssetManager = Assets;
            InstanceManager.Logger = Log;
            InstanceManager.InputManager = new InputManager();
            
            // Set the local instance manager
            LocalInstanceManager.Steam = steamSystem;
            LocalInstanceManager.PlayerRing = playerRing;
            LocalInstanceManager.PlayerSmoke = playerSmoke;
            LocalInstanceManager.PlayerExplosion = playerExplosion;
            LocalInstanceManager.EnemyExplodeSystem = enemyExplodeSystem;
            LocalInstanceManager.BulletParticle = bulletParticle;
            LocalInstanceManager.Background = background;
            LocalInstanceManager.AchievementManager = achievements;
            LocalInstanceManager.Spinner = spinner;

            LocalInstanceManager.Scores = new ScoreScroller[LocalInstanceManager.MaxNumberOfPlayers];
            for (int i = 0; i < LocalInstanceManager.MaxNumberOfPlayers; i++)
            {
                LocalInstanceManager.Scores[i] = new ScoreScroller(
                    this,
                    i,
                    scoreMoveTime,
                    Vector2.Zero,
                    Vector2.Zero,
                    0,
                    scoreScrollTime);
                this.Components.Add(LocalInstanceManager.Scores[i]);
                LocalInstanceManager.Scores[i].DrawOrder = 99;
                LocalInstanceManager.Scores[i].Enabled = false;
                LocalInstanceManager.Scores[i].Visible = false;
            }

            // A bit of trickery to ensure we have a lastGameState
            LocalInstanceManager.CurrentGameState = GameState.Exit;
            LocalInstanceManager.CurrentGameState = GameState.MainMenuSystem;

            // Configure up the various GameScreens and dispatch
            dispatchTable = new Dictionary<GameState, GameScreen>();

            // Exit screen
            exitScreen = new ExitScreen(this);
            this.Components.Add(exitScreen);
            dispatchTable.Add(GameState.Exit, exitScreen);

            // Main menu
            mainMenuScreen = new MainMenuScreen(this);
            this.Components.Add(mainMenuScreen);
            dispatchTable.Add(GameState.MainMenuSystem, mainMenuScreen);

            // Player select
            playerSelectScreen = new PlayerSelectScreen(this);
            this.Components.Add(playerSelectScreen);
            dispatchTable.Add(GameState.PlayerSelect, playerSelectScreen);

            // Gameplay screen
            gamePlayScreenManager = new GamePlayScreenManager(this);
            this.Components.Add(gamePlayScreenManager);
            dispatchTable.Add(GameState.InfinityGame, gamePlayScreenManager);

            // Color State Test screen
            colorStateTest = new ColorStateTestScreen(this);
            this.Components.Add(colorStateTest);
            dispatchTable.Add(GameState.ColorStateTest, colorStateTest);

            // Ensure that everything in the dispatch is disabled
            foreach (GameScreen scr in dispatchTable.Values)
            {
                scr.SetEnable(false);
                scr.SetVisible(false);
                scr.Enabled = false;
            }

            Audio = new AudioManager(this);
            this.Components.Add(Audio);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Render = new RenderSprite(spriteBatch);
            InstanceManager.RenderSprite = Render;
            InstanceManager.GraphicsDevice = GraphicsDevice;
            InstanceManager.DefaultViewport = GraphicsDevice.Viewport;
            //Log.RenderSprite = Render;
            /*gamePlayTest.RenderSprite = Render;
            gamePlayTest.AssetManager = Assets;
            gamePlayTest.Device = GraphicsDevice;*/

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Update the input manager every update
            InstanceManager.InputManager.Update();

            // Determine which GameScreen should be running based upon the state
            if (LocalInstanceManager.CurrentGameState != LocalInstanceManager.LastGameState)
            {                
                dispatchTable[LocalInstanceManager.LastGameState].SetEnable(false);
                dispatchTable[LocalInstanceManager.LastGameState].SetVisible(false);
                dispatchTable[LocalInstanceManager.LastGameState].Enabled = false;

                dispatchTable[LocalInstanceManager.CurrentGameState].SetEnable(true);
                dispatchTable[LocalInstanceManager.CurrentGameState].SetVisible(true);
                dispatchTable[LocalInstanceManager.CurrentGameState].Enabled=true;
            }
            // Ensure that the last game state gets the current setting for next update
            LocalInstanceManager.CurrentGameState = LocalInstanceManager.CurrentGameState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Render.Run();
            base.Draw(gameTime);
        }
    }
}
