#define TSATEST

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
using Mimicware.Fx;
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
    public class DuologueGame : Microsoft.Xna.Framework.Game, IService
    {
        #region Constants
        /// <summary>
        /// Max number of steam effects
        /// </summary>
        public const int MaxSteamEffects = 4;
        /// <summary>
        /// Max number of explosion effects
        /// </summary>
        public const int MaxExplosionEffects = 5;
        /// <summary>
        /// The time it takes for the score to move into place
        /// </summary>
        public const float scoreMoveTime = 0f;
        /// <summary>
        /// The time it takes for the score to scroll
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

        /// <summary>
        /// The total trial time (currently set to roughly 7 and a quarter minutes)
        /// </summary>
        public const double TotalTrialTime = 7.25;
        #endregion

        #region Fields
        private bool debug;
#if TSATEST
        private Texture2D tsaTexture;
        private Vector2 tsaCenter;
        private Vector2[] tsaPos;
#endif
        #endregion

        #region Properties
        // XNA stuff
        public GraphicsDeviceManager Graphics;
        public SpriteBatch spriteBatch;
        public GamerServicesComponent GamerServices;

        // Mimicware stuff
        public AssetManager Assets;
        public RenderSprite Render;
        public Logger Log;

        // Particle system stuff
        public Steam steamSystem;
        public PlayerRing playerRing;
        public PlayerExplosion playerExplosion;
        public EnemyExplodeSystem enemyExplodeSystem;
        public EnemySplatterSystem enemySplatterSystem;
        public PlayerSmoke playerSmoke;
        public BulletParticle bulletParticle;

        // Elements
        public Background background;
        public AchievementManager achievements;
        public Spinner spinner;
        public WindowManager windowManager;
        public AudioManager Audio;
        public IntensityNotifier Intensity;

        // Screens
        public ColorStateTestScreen colorStateTest;
        public MainMenuScreen mainMenuScreen;
        public ExitScreen exitScreen;
        public PauseScreen pauseScreen;
        public PlayerSelectScreen playerSelectScreen;
        public GamePlayScreenManager gamePlayScreenManager;
        public EndCinematicScreenManager endCinematicScreenManager;
        public CreditsScreenManager creditsScreenManager;
        public CompanyIntroScreenManager companyIntroScreenManager;
        public MedalCaseScreenManager medalCaseScreenManager;
        public BuyScreenManager buyScreenManager;

        public double trialTimer;

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
        #endregion

        public DuologueGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            GamerServices = new GamerServicesComponent(this);
            this.Components.Add(GamerServices);
            debug = false;
            // Uncomment to simulate trial mode
            //Guide.SimulateTrialMode = true;
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
            if(GraphicsAdapter.DefaultAdapter.IsWideScreen)
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

            Assets = new AssetManager(Content);

            background = new Background(this);
            background.Visible = true;
            background.Enabled = true;
            this.Components.Add(background);
            
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
            this.Components.Add(steamSystem);
            steamSystem.DrawOrder = 7;

            playerRing = new PlayerRing(this, MaxExplosionEffects);
            this.Components.Add(playerRing);
            playerRing.DrawOrder = 7;

            playerExplosion = new PlayerExplosion(this, MaxPlayerExplosionEffects);
            this.Components.Add(playerExplosion);
            playerExplosion.DrawOrder = 7;

            enemyExplodeSystem = new EnemyExplodeSystem(this, MaxEnemyExplosions);
            this.Components.Add(enemyExplodeSystem);
            enemyExplodeSystem.DrawOrder = 7;

            enemySplatterSystem = new EnemySplatterSystem(this, MaxEnemyExplosions);
            this.Components.Add(enemySplatterSystem);
            enemySplatterSystem.DrawOrder = 7;

            playerSmoke = new PlayerSmoke(this, MaxPlayerSmokeEffects);
            this.Components.Add(playerSmoke);
            playerSmoke.DrawOrder = 7;

            bulletParticle = new BulletParticle(this, MaxBulletParticles);
            bulletParticle.Enabled = true;
            bulletParticle.Visible = true;
            this.Components.Add(bulletParticle);
            bulletParticle.DrawOrder = 8;

            achievements = new AchievementManager(this);
            achievements.Enabled = true;
            achievements.Visible = true;
            this.Components.Add(achievements);
            achievements.DrawOrder = 300;

            pauseScreen = new PauseScreen(this);
            pauseScreen.Enabled = false;
            pauseScreen.Visible = false;
            this.Components.Add(pauseScreen);
            pauseScreen.DrawOrder = 300;
            ServiceLocator.RegisterService(pauseScreen);

            Teletype teletype = new Teletype(this);
            ServiceLocator.RegisterService(teletype);
            teletype.DrawOrder = 200;
            teletype.Enabled = false;
            teletype.Visible = false;
            this.Components.Add(teletype);

            windowManager = new WindowManager();

            // Set the instance manager
            InstanceManager.AssetManager = Assets;
            InstanceManager.Logger = Log;
            InstanceManager.InputManager = new InputManager();
            InstanceManager.Localization = new Localization("Content/Localize");
            
            // Set the local instance manager
            LocalInstanceManager.Steam = steamSystem;
            LocalInstanceManager.PlayerRing = playerRing;
            LocalInstanceManager.PlayerSmoke = playerSmoke;
            LocalInstanceManager.PlayerExplosion = playerExplosion;
            LocalInstanceManager.EnemyExplodeSystem = enemyExplodeSystem;
            LocalInstanceManager.EnemySplatterSystem = enemySplatterSystem;
            LocalInstanceManager.BulletParticle = bulletParticle;
            LocalInstanceManager.Background = background;
            LocalInstanceManager.AchievementManager = achievements;
            LocalInstanceManager.Spinner = spinner;
            LocalInstanceManager.WindowManager = windowManager;

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
            LocalInstanceManager.CurrentGameState = GameState.CompanyIntro;

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
            ServiceLocator.RegisterService(mainMenuScreen);

            // Player select
            playerSelectScreen = new PlayerSelectScreen(this);
            this.Components.Add(playerSelectScreen);
            dispatchTable.Add(GameState.PlayerSelect, playerSelectScreen);

            // Gameplay screen
            gamePlayScreenManager = new GamePlayScreenManager(this);
            this.Components.Add(gamePlayScreenManager);
            dispatchTable.Add(GameState.SurvivalGame, gamePlayScreenManager);
            dispatchTable.Add(GameState.CampaignGame, gamePlayScreenManager);
            dispatchTable.Add(GameState.InfiniteGame, gamePlayScreenManager);

            // Color State Test screen
            colorStateTest = new ColorStateTestScreen(this);
            this.Components.Add(colorStateTest);
            dispatchTable.Add(GameState.ColorStateTest, colorStateTest);

            // End cinematics screen
            endCinematicScreenManager = new EndCinematicScreenManager(this);
            this.Components.Add(endCinematicScreenManager);
            dispatchTable.Add(GameState.EndCinematics, endCinematicScreenManager);

            // Intro screen
            companyIntroScreenManager = new CompanyIntroScreenManager(this);
            this.Components.Add(companyIntroScreenManager);
            dispatchTable.Add(GameState.CompanyIntro, companyIntroScreenManager);

            // Credits screen
            creditsScreenManager = new CreditsScreenManager(this);
            this.Components.Add(creditsScreenManager);
            dispatchTable.Add(GameState.Credits, creditsScreenManager);

            // Medal case screen
            medalCaseScreenManager = new MedalCaseScreenManager(this);
            this.Components.Add(medalCaseScreenManager);
            dispatchTable.Add(GameState.MedalCase, medalCaseScreenManager);

            // Buy screen manager
            buyScreenManager = new BuyScreenManager(this);
            this.Components.Add(buyScreenManager);
            dispatchTable.Add(GameState.BuyScreen, buyScreenManager);
            ServiceLocator.RegisterService(buyScreenManager);

            // Ensure that everything in the dispatch is disabled
            foreach (GameScreen scr in dispatchTable.Values)
            {
                scr.SetEnable(false);
                scr.SetVisible(false);
                scr.Enabled = false;
            }

            Audio = new AudioManager(this);
            ServiceLocator.RegisterService(Audio);
            this.Components.Add(Audio);

            Intensity = new IntensityNotifier();
            ServiceLocator.RegisterService(Intensity);

            ServiceLocator.RegisterService(this);

            // Gamepad stuff
            LocalInstanceManager.GamePadHelpers = new GamePadHelper[LocalInstanceManager.MaxNumberOfPlayers];
            for (int i = 0; i < LocalInstanceManager.MaxNumberOfPlayers; i++)
            {
                LocalInstanceManager.GamePadHelpers[i] = new GamePadHelper(this, (PlayerIndex)i);
            }

            // Pause stuff
            LocalInstanceManager.Pause = false;
            LocalInstanceManager.CanPause = true;

            trialTimer = 0;

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

#if TSATEST
            tsaTexture = Content.Load<Texture2D>("PlayerUI/tsa_test");
            tsaCenter = new Vector2(
                tsaTexture.Width / 2f, tsaTexture.Height / 2f);
            tsaPos = null;
#endif

            windowManager.LoadContents();
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
            // Trial stuff
            if (Guide.IsTrialMode)
            {
                trialTimer += gameTime.ElapsedGameTime.TotalMinutes;
                if (trialTimer > TotalTrialTime)
                {
                    trialTimer = 0;
                    LocalInstanceManager.Pause = false;
                    LocalInstanceManager.Background.Enabled = true;
                    LocalInstanceManager.Background.Visible = true;
                    LocalInstanceManager.CurrentGameState = GameState.BuyScreen;
                    LocalInstanceManager.NextGameState = GameState.MainMenuSystem;
                    LocalInstanceManager.AchievementManager.DisableMedalScreen();
                    LocalInstanceManager.AchievementManager.ReturnToPause = false;
                }
            }

            // Update the input manager every update
            InstanceManager.InputManager.Update();

            // Determine which GameScreen should be running based upon the state
            if (LocalInstanceManager.CurrentGameState != LocalInstanceManager.LastGameState)
            {
                dispatchTable[LocalInstanceManager.LastGameState].ScreenExit(gameTime);
                dispatchTable[LocalInstanceManager.LastGameState].SetEnable(false);
                dispatchTable[LocalInstanceManager.LastGameState].SetVisible(false);
                dispatchTable[LocalInstanceManager.LastGameState].Enabled = false;

                dispatchTable[LocalInstanceManager.CurrentGameState].SetEnable(true);
                dispatchTable[LocalInstanceManager.CurrentGameState].SetVisible(true);
                dispatchTable[LocalInstanceManager.CurrentGameState].Enabled=true;
                dispatchTable[LocalInstanceManager.CurrentGameState].ScreenEntrance(gameTime);
            }
            // Ensure that the last game state gets the current setting for next update
            LocalInstanceManager.CurrentGameState = LocalInstanceManager.CurrentGameState;

            if (Guide.IsVisible)
            {
                GamerServices.Update(gameTime);
            }
            else if (LocalInstanceManager.Pause)
            {
                GamerServices.Update(gameTime);
                if (!pauseScreen.Enabled)
                {
                    pauseScreen.Enabled = true;
                    pauseScreen.Visible = true;
                }
                pauseScreen.Update(gameTime);
            }
            else
            {
                if (pauseScreen.Enabled)
                {
                    pauseScreen.Enabled = false;
                    pauseScreen.Visible = false;
                }
                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Render.Run();

#if TSATEST
            if (tsaPos == null)
            {
                tsaPos = new Vector2[]
                {
                    new Vector2(InstanceManager.DefaultViewport.TitleSafeArea.Left, InstanceManager.DefaultViewport.TitleSafeArea.Top),
                    new Vector2(InstanceManager.DefaultViewport.TitleSafeArea.Left, InstanceManager.DefaultViewport.TitleSafeArea.Bottom),
                    new Vector2(InstanceManager.DefaultViewport.TitleSafeArea.Right, InstanceManager.DefaultViewport.TitleSafeArea.Top),
                    new Vector2(InstanceManager.DefaultViewport.TitleSafeArea.Right, InstanceManager.DefaultViewport.TitleSafeArea.Bottom)
                };
            }
            spriteBatch.Begin();
            for (int i = 0; i < tsaPos.Length; i++)
            {
                spriteBatch.Draw(
                    tsaTexture,
                    tsaPos[i],
                    null,
                    Color.White,
                    0f,
                    tsaCenter,
                    1f,
                    SpriteEffects.None,
                    0f);
            }
            spriteBatch.End();
            base.Draw(gameTime);
#endif
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            LocalInstanceManager.AchievementManager.SaveStorageData();
            base.OnExiting(sender, args);
        }

        public void LivePurchaseReset()
        {
            mainMenuScreen.LivePurchaseReset();
            pauseScreen.LivePurchaseReset();
        }
    }
}
