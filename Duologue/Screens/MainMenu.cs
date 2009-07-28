#region File Description
#endregion

#region Using Statements
// System
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
using Mimicware.Manager;
using Mimicware.Graphics;
using Mimicware.Fx;
// Duologue
using Duologue;
using Duologue.Properties;
using Duologue.State;
using Duologue.Audio;
#endregion

namespace Duologue.Screens
{
    /// <summary>
    /// The possible states for the main menu
    /// </summary>
    public enum MainMenuState
    {
        MainMenu,
        GameSelect,
        PressStart,
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string fontFilename = "Fonts/inero-50";
        private const string tipFontFilename = "Fonts/inero-28";
        private const float yOffset = 325f;
        private const float extraLineSpacing = 12;
        private const float selectOffset = 8;
        private const int numberOfOffsets = 4;

        private const int windowOffsetX = 30;
        private const int windowOffsetY = 10;
        private const double startThrobTime = 1.1;

        private const float tooltipOffset = 50f;
        private const float shadowOffsetToolTip = 5f;

        private const double time_TypeToolTip = 1.4;
        private const double time_ToolTipOnscreen = 30.1;
        #endregion

        #region Fields
        private SpriteFont font;
        private SpriteFont tipFont;
        private Vector2 position;
        private List<MenuItem> mainMenuItems;
        private List<MenuItem> gameSelectItems;
        private int currentSelection;
        // The list of menu items
        private int menuPlayGame;
        private int menuAchievements;
        private int menuCredits;
        private int menuBuyMe;
        private int menuExit;
        private int gameSelectCampaign;
        private int gameSelectSurvival;
        private int gameSelectInfinite;
        private int gameSelectBack;
        private MainMenuState currentState;
        private Rectangle mainMenuWindowLocation;
        private Rectangle gameSelectWindowLocation;
        private Rectangle creditsWindowLocation;

        // Tool tip stuff
        private Teletype teletype;
        private TeletypeEntry[] gameSelectTips;
        private Color color_ToolTip;
        private Color color_ToolTipShadow;
        private Vector2[] toolTipShadowOffset;
        private bool isMenuSet;
        private bool trialMode;

        /// <summary>
        /// Used for the debug sequence
        /// </summary>
        private int debugSequence;

        private Game myGame;

        private Vector2[] shadowOffsets;
        private Vector2[] shadowOffsetsSelected;

        private bool initialized;

        // Start stuff
        private bool startPressed;
        private PlayerIndex playerWhoPressedStart;
        private IAsyncResult guideResult;
        private double timer_StartThrob;
        private StorageDevice device;
        private bool storageText = false;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public MainMenu(Game game)
            : base(game)
        {
            position = Vector2.Zero;
            mainMenuItems = new List<MenuItem>();
            gameSelectItems = new List<MenuItem>();

            myGame = game;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            currentState = MainMenuState.PressStart;
            currentSelection = 0;
            timer_StartThrob = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = InstanceManager.AssetManager.LoadSpriteFont(fontFilename);
            tipFont = InstanceManager.AssetManager.LoadSpriteFont(tipFontFilename);

            color_ToolTip = Color.LightGoldenrodYellow;
            color_ToolTipShadow = new Color(Color.DarkRed, 175);

            toolTipShadowOffset = new Vector2[]
            {
                shadowOffsetToolTip * Vector2.One,
                Vector2.One,
                -Vector2.One,
                Vector2.UnitX,
                Vector2.UnitY,
            };
            base.LoadContent();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Generate the position
        /// </summary>
        private void SetPosition()
        {
            if (!isMenuSet)
            {
                int menuIndex = 0;
                mainMenuItems.Clear();
                gameSelectItems.Clear();

                // Set up the main menu
                mainMenuItems.Add(new MenuItem(Resources.MainMenu_Play));
                menuPlayGame = menuIndex;
                menuIndex++;
                mainMenuItems.Add(new MenuItem(Resources.MainMenu_Achievements));
                menuAchievements = menuIndex;
                menuIndex++;
                mainMenuItems.Add(new MenuItem(Resources.MainMenu_Credits));
                menuCredits = menuIndex;
                menuIndex++;
                if (Guide.IsTrialMode)
                {
                    mainMenuItems.Add(new MenuItem(Resources.MainMenu_BuyMe));
                    menuBuyMe = menuIndex;
                    menuIndex++;
                }
                else
                {
                    menuBuyMe = -1337; // LEET! We been bought!
                }
                mainMenuItems.Add(new MenuItem(Resources.MainMenu_Exit));
                menuExit = menuIndex;

                // Set up the game select menu
                gameSelectItems.Add(new MenuItem(Resources.MainMenu_GameSelect_Campaign));
                gameSelectCampaign = 0;
                gameSelectItems.Add(new MenuItem(Resources.MainMenu_GameSelect_InfiniteMode));
                gameSelectInfinite = 1;
                gameSelectItems.Add(new MenuItem(Resources.MainMenu_GameSelect_Survival));
                gameSelectSurvival = 2;

                gameSelectItems.Add(new MenuItem(Resources.MainMenu_GameSelect_Back));
                gameSelectBack = 3;

                debugSequence = 0;

                shadowOffsets = new Vector2[numberOfOffsets];
                shadowOffsets[0] = Vector2.One;
                shadowOffsets[1] = -1 * Vector2.One;
                shadowOffsets[2] = new Vector2(-1f, 1f);
                shadowOffsets[3] = new Vector2(1f, -1f);

                shadowOffsetsSelected = new Vector2[numberOfOffsets];
                shadowOffsetsSelected[0] = 2 * Vector2.One;
                shadowOffsetsSelected[1] = -2 * Vector2.One;
                shadowOffsetsSelected[2] = new Vector2(-2f, 2f);
                shadowOffsetsSelected[3] = new Vector2(2f, -2f);

                //initialized = false;
                startPressed = false;

                teletype = ServiceLocator.GetService<Teletype>();
                gameSelectTips = new TeletypeEntry[gameSelectBack + 1];
                for (int i = 0; i < gameSelectTips.Length; i++)
                {
                    gameSelectTips[i] = null;
                }

                foreach (MenuItem mi in mainMenuItems)
                    mi.Invisible = false;

                // Turn off those items we don't support yet
                //mainMenuItems[menuAchievements].Invisible = true;

                foreach (MenuItem mi in gameSelectItems)
                    mi.Invisible = false;

                if (Guide.IsTrialMode)
                {
                    // Only campaign mode available in trial mode
                    gameSelectItems[gameSelectInfinite].Invisible = true;
                    gameSelectItems[gameSelectSurvival].Invisible = true;
                }

                isMenuSet = true;
                trialMode = Guide.IsTrialMode;

                ResetMenuItems();
            }

            float center = InstanceManager.DefaultViewport.Width / 2f;
            float xOffset = center;
            float maxWidth = 0;
            float maxHeight = 0;
            int tempW;
            int tempH;

            foreach (MenuItem mi in mainMenuItems)
            {
                Vector2 size = font.MeasureString(mi.Text);
                float xTest = center - size.X / 2f;
                if (xTest < xOffset)
                    xOffset = xTest;

                // Compute max width and height
                if (size.X > maxWidth)
                    maxWidth = size.X;
                maxHeight += font.LineSpacing + extraLineSpacing;
            }

            tempW = (int)maxWidth + 2*windowOffsetX;
            tempH = (int)maxHeight + font.LineSpacing + (int)extraLineSpacing + 2*windowOffsetY;

            maxWidth = 0;
            maxHeight = 0;

            foreach (MenuItem mi in gameSelectItems)
            {
                Vector2 size = font.MeasureString(mi.Text);
                float xTest = center - size.X / 2f;
                if (xTest < xOffset)
                    xOffset = xTest;

                // Compute max width and height
                if (size.X > maxWidth)
                    maxWidth = size.X;
                maxHeight += font.LineSpacing + extraLineSpacing;
            }
            position = new Vector2(xOffset, yOffset);

            gameSelectWindowLocation = new Rectangle(
                (int)position.X - windowOffsetX,
                (int)position.Y - windowOffsetY,
                (int)maxWidth + 2*windowOffsetX,
                (int)maxHeight + font.LineSpacing + (int)extraLineSpacing + 2*windowOffsetY);

            creditsWindowLocation = new Rectangle(
                (int)position.X - windowOffsetX,
                (int)position.Y - windowOffsetY,
                (int)maxWidth + 2 * windowOffsetX,
                (int)maxHeight + font.LineSpacing + (int)extraLineSpacing + 2 * windowOffsetY);

            mainMenuWindowLocation = new Rectangle(
                (int)position.X - windowOffsetX,
                (int)position.Y - windowOffsetY,
                tempW, tempH);

            LocalInstanceManager.WindowManager.SetLocation(mainMenuWindowLocation);

            // Setup the tool tips
            Vector2 tempSizeInfMode = tipFont.MeasureString(
                Guide.IsTrialMode ? Resources.MainMenu_ToolTip_TrialMode : Resources.MainMenu_ToolTip_InfiniteMode);
            Vector2 tempSizeSurMode = tipFont.MeasureString(
                Guide.IsTrialMode ? Resources.MainMenu_ToolTip_TrialMode : Resources.MainMenu_ToolTip_SurvivalMode);

            Vector2 tempPos = new Vector2(
                center,
                gameSelectWindowLocation.Bottom + tooltipOffset);

            Vector2 tempCent = new Vector2(
                tempSizeInfMode.X/2f, tempSizeInfMode.Y/2f);

            gameSelectTips[gameSelectInfinite] = new TeletypeEntry(
                tipFont,
                Guide.IsTrialMode ? Resources.MainMenu_ToolTip_TrialMode : Resources.MainMenu_ToolTip_InfiniteMode,
                tempPos,
                tempCent,
                color_ToolTip,
                time_TypeToolTip,
                time_ToolTipOnscreen,
                color_ToolTipShadow,
                toolTipShadowOffset,
                InstanceManager.RenderSprite);

            tempCent = new Vector2(
                tempSizeSurMode.X / 2f, tempSizeSurMode.Y / 2f);

            gameSelectTips[gameSelectSurvival] = new TeletypeEntry(
                tipFont,
                Guide.IsTrialMode ? Resources.MainMenu_ToolTip_TrialMode : Resources.MainMenu_ToolTip_SurvivalMode,
                tempPos,
                tempCent,
                color_ToolTip,
                time_TypeToolTip,
                time_ToolTipOnscreen,
                color_ToolTipShadow,
                toolTipShadowOffset,
                InstanceManager.RenderSprite);

            initialized = true;
        }

        /// <summary>
        /// When a menu item is selected, this is where we parse it
        /// </summary>
        private void ParseSelected()
        {
            if (currentState == MainMenuState.MainMenu)
            {
                if (!mainMenuItems[currentSelection].Invisible)
                {
                    teletype.FlushEntries();
                    if (currentSelection == menuExit)
                    {
                        if (Guide.IsTrialMode)
                        {
                            LocalInstanceManager.CurrentGameState = GameState.BuyScreen;
                            LocalInstanceManager.NextGameState = GameState.Exit;
                        }
                        else
                        {
                            LocalInstanceManager.CurrentGameState = GameState.Exit;
                        }
                    }
                    else if (currentSelection == menuPlayGame)
                    {
                        currentState = MainMenuState.GameSelect;
                        LocalInstanceManager.WindowManager.SetLocation(gameSelectWindowLocation);
                        currentSelection = 0;
                        ResetMenuItems();
                    }
                    else if (currentSelection == menuCredits)
                    {
                        LocalInstanceManager.CurrentGameState = GameState.Credits;

                    }
                    else if (currentSelection == menuAchievements)
                    {
                        LocalInstanceManager.NextGameState = GameState.MainMenuSystem;
                        LocalInstanceManager.CurrentGameState = GameState.MedalCase;
                    }
                    else if (Guide.IsTrialMode && currentSelection == menuBuyMe)
                    {
                        LocalInstanceManager.NextGameState = GameState.MainMenuSystem;
                        LocalInstanceManager.CurrentGameState = GameState.BuyScreen;
                    }
                }
            }
            else
            {
                if (!gameSelectItems[currentSelection].Invisible)
                {
                    teletype.FlushEntries();
                    if (currentSelection == gameSelectBack)
                    {
                        currentState = MainMenuState.MainMenu;
                        currentSelection = 0;
                        LocalInstanceManager.WindowManager.SetLocation(mainMenuWindowLocation);
                        ResetMenuItems();
                    }
                    else if (currentSelection == gameSelectSurvival)
                    {
                        currentState = MainMenuState.MainMenu;
                        currentSelection = 0;
                        ResetMenuItems();
                        LocalInstanceManager.CurrentGameState = GameState.PlayerSelect;
                        LocalInstanceManager.NextGameState = GameState.SurvivalGame;
                    }
                    else if (currentSelection == gameSelectInfinite)
                    {
                        currentState = MainMenuState.MainMenu;
                        currentSelection = 0;
                        ResetMenuItems();
                        LocalInstanceManager.CurrentGameState = GameState.PlayerSelect;
                        LocalInstanceManager.NextGameState = GameState.InfiniteGame;
                    }
                    else if (currentSelection == gameSelectCampaign)
                    {
                        currentState = MainMenuState.MainMenu;
                        currentSelection = 0;
                        ResetMenuItems();
                        LocalInstanceManager.CurrentGameState = GameState.PlayerSelect;
                        LocalInstanceManager.NextGameState = GameState.CampaignGame;
                    }
                }
            }
        }

        private void ResetMenuItems()
        {
            foreach (MenuItem mi in mainMenuItems)
                mi.Selected = false;
            foreach (MenuItem mi in gameSelectItems)
                mi.Selected = false;
        }

        /// <summary>
        /// Check to see if the A or select button is pressed
        /// </summary>
        /// <returns>True if it was</returns>
        private bool CheckButtonA()
        {
            bool pressed = false;

            for (int i = 0; i < InstanceManager.InputManager.CurrentGamePadStates.Length; i++)
            {
                if (InstanceManager.InputManager.CurrentKeyboardStates[i].IsKeyDown(Keys.Enter) &&
                    InstanceManager.InputManager.LastKeyboardStates[i].IsKeyUp(Keys.Enter))
                {
                    pressed = true;
                    break;
                }
                if (InstanceManager.InputManager.CurrentGamePadStates[i].Buttons.A == ButtonState.Pressed &&
                   InstanceManager.InputManager.LastGamePadStates[i].Buttons.A == ButtonState.Released)
                {
                    pressed = true;
                    break;
                }
                if (InstanceManager.InputManager.CurrentGamePadStates[i].Buttons.Start == ButtonState.Pressed &&
                   InstanceManager.InputManager.LastGamePadStates[i].Buttons.Start == ButtonState.Released)
                {
                    pressed = true;
                    break;
                }
            }
            return pressed;
        }

        /// <summary>
        /// Check to see if the B or select button is pressed
        /// </summary>
        /// <returns>True if it was</returns>
        private bool CheckButtonB()
        {
            bool pressed = false;

            for (int i = 0; i < InstanceManager.InputManager.CurrentGamePadStates.Length; i++)
            {
                if (InstanceManager.InputManager.CurrentKeyboardStates[i].IsKeyDown(Keys.Escape) &&
                    InstanceManager.InputManager.LastKeyboardStates[i].IsKeyUp(Keys.Escape))
                {
                    pressed = true;
                    break;
                }
                if (InstanceManager.InputManager.CurrentGamePadStates[i].Buttons.B == ButtonState.Pressed &&
                   InstanceManager.InputManager.LastGamePadStates[i].Buttons.B == ButtonState.Released)
                {
                    pressed = true;
                    break;
                }
            }
            return pressed;
        }

        /// <summary>
        /// Checks to see if the "menu down" controll was triggered
        /// </summary>
        private bool IsMenuDown()
        {
            bool pressed = false;
            
            for(int i = 0; i < InstanceManager.InputManager.CurrentGamePadStates.Length; i++)
            {
                if(InstanceManager.InputManager.CurrentKeyboardStates[i].IsKeyDown(Keys.Down) &&
                    InstanceManager.InputManager.LastKeyboardStates[i].IsKeyUp(Keys.Down))
                {
                    pressed = true;
                    break;
                }
                if((InstanceManager.InputManager.CurrentGamePadStates[i].DPad.Down == ButtonState.Pressed &&
                    InstanceManager.InputManager.LastGamePadStates[i].DPad.Down == ButtonState.Released) ||
                   (InstanceManager.InputManager.CurrentGamePadStates[i].ThumbSticks.Left.Y < 0 &&
                    InstanceManager.InputManager.LastGamePadStates[i].ThumbSticks.Left.Y >= 0))
                {
                    pressed = true;
                    break;
                }
            }
            return pressed;
        }

        /// <summary>
        /// Checks to see if the "menu up" control was triggered
        /// </summary>
        private bool IsMenuUp()
        {
            bool pressed = false;

            for (int i = 0; i < InstanceManager.InputManager.CurrentGamePadStates.Length; i++)
            {
                if (InstanceManager.InputManager.CurrentKeyboardStates[i].IsKeyDown(Keys.Up) &&
                    InstanceManager.InputManager.LastKeyboardStates[i].IsKeyUp(Keys.Up))
                {
                    pressed = true;
                    break;
                }
                if ((InstanceManager.InputManager.CurrentGamePadStates[i].DPad.Up == ButtonState.Pressed &&
                    InstanceManager.InputManager.LastGamePadStates[i].DPad.Up == ButtonState.Released) ||
                   (InstanceManager.InputManager.CurrentGamePadStates[i].ThumbSticks.Left.Y > 0 &&
                    InstanceManager.InputManager.LastGamePadStates[i].ThumbSticks.Left.Y <= 0))
                {
                    pressed = true;
                    break;
                }
            }
            return pressed;
        }

        /// <summary>
        /// Draw a list of menu items
        /// </summary>
        private void DrawMenu(List<MenuItem> mis, GameTime gameTime)
        {
            Vector2 curPos = position;
            foreach (MenuItem mi in mis)
            {
                mi.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                /*InstanceManager.RenderSprite.DrawString(font,
                    mi.Text,
                    curPos,
                    Color.SeaGreen,
                    RenderSpriteBlendMode.Multiplicative);*/
                if (mi.Invisible)
                {
                    InstanceManager.RenderSprite.DrawString(font,
                        mi.Text,
                        curPos,
                        mi.Selected ? Color.DarkSlateBlue : Color.DarkSlateGray,
                        RenderSpriteBlendMode.AddititiveTop);
                }
                else
                {
                    InstanceManager.RenderSprite.DrawString(
                        font,
                        mi.Text,
                        curPos + mi.FadePercent * selectOffset * Vector2.UnitX,
                        mi.Selected ? Color.LightGoldenrodYellow : Color.Khaki,
                        Color.Black,
                        mi.Selected ? shadowOffsetsSelected : shadowOffsets,
                        RenderSpriteBlendMode.AlphaBlendTop);
                }
                curPos.Y += font.LineSpacing + extraLineSpacing;
            }

        }

        /// <summary>
        /// Inner update
        /// </summary>
        private void InnerUpdate(List<MenuItem> mis)
        {
            mis[currentSelection].Selected = true;

            // See if we have a button down to select
            if (CheckButtonA())
            {
                ParseSelected();
            }

            // Determine if we've got a new selection
            // Down
            if (IsMenuDown())
            {
                mis[currentSelection].Selected = false;
                currentSelection++;
                if (currentSelection >= mis.Count)
                    currentSelection = 0;
                else
                    SetTooltip();
            }

            // Up
            if (IsMenuUp())
            {
                mis[currentSelection].Selected = false;
                currentSelection--;
                if (currentSelection < 0)
                    currentSelection = mis.Count - 1;
                else
                    SetTooltip();
            }

            /*if (currentSelection >= mis.Count)
                currentSelection = 0;
            else if (currentSelection < 0)
                currentSelection = mis.Count - 1;*/
        }

        private void SetTooltip()
        {
            teletype.FlushEntries();

            if (currentState == MainMenuState.GameSelect)
            {
                if (gameSelectTips[currentSelection] != null)
                {
                    gameSelectTips[currentSelection].Reset();
                    teletype.AddEntry(gameSelectTips[currentSelection]);
                }
            }
        }


        private void DrawPressStart(GameTime gameTime)
        {
            timer_StartThrob += gameTime.ElapsedRealTime.TotalSeconds;
            if (timer_StartThrob > startThrobTime)
                timer_StartThrob = 0;

            Vector2 temp = font.MeasureString(Resources.MainMenu_PressStart);
            Vector2 tempSave = tipFont.MeasureString(Resources.MainMenu_SelectSave);

            InstanceManager.RenderSprite.DrawString(
                font,
                Resources.MainMenu_PressStart,
                new Vector2(
                    InstanceManager.DefaultViewport.Width / 2f,
                    InstanceManager.DefaultViewport.Height / 2f),
                Color.White,
                Vector2.One,
                temp / 2f);

            InstanceManager.RenderSprite.DrawString(
                font,
                Resources.MainMenu_PressStart,
                new Vector2(
                    InstanceManager.DefaultViewport.Width / 2f,
                    InstanceManager.DefaultViewport.Height / 2f),
                new Color(Color.Tan, (float)(timer_StartThrob/startThrobTime)),
                Vector2.One,
                temp / 2f);

            if (storageText)
            {
                InstanceManager.RenderSprite.DrawString(
                    tipFont,
                    Resources.MainMenu_SelectSave,
                    new Vector2(
                        InstanceManager.DefaultViewport.Width / 2f,
                        InstanceManager.DefaultViewport.Height / 2f + temp.Y/2f),
                    Color.Tan,
                    Vector2.One,
                    tempSave / 2f);
            }
        }
        #endregion

        #region Public Methods
        internal void Reset()
        {
            if (initialized)
            {
                SetPosition();
                LocalInstanceManager.WindowManager.SetLocation(mainMenuWindowLocation);
                // Get rid of parallax
                LocalInstanceManager.Background.SetParallaxElement(
                    LocalInstanceManager.Background.EmptyParallaxElement, true);
                LocalInstanceManager.Background.SetParallaxElement(
                    LocalInstanceManager.Background.EmptyParallaxElement, false);
            }
        }

        /// <summary>
        /// Call when we've made a live purchase and need to reset everything
        /// </summary>
        public void LivePurchaseReset()
        {
            currentState = MainMenuState.PressStart;
            guideResult = null;
        }
        #endregion

        #region Update / Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (Guide.IsTrialMode != trialMode)
            {
                // Thanks for the purchase, but fuck XNA
                isMenuSet = false;
                SetPosition();
                trialMode = Guide.IsTrialMode;
            }
            LocalInstanceManager.WindowManager.Update(gameTime);

            if (currentState == MainMenuState.PressStart)
            {
                if (startPressed && guideResult.IsCompleted)
                {
                    device = Guide.EndShowStorageDeviceSelector(guideResult);
                    try
                    {
                        if (device.IsConnected)
                        {
                            LocalInstanceManager.AchievementManager.InitStorageData(device);

                            startPressed = true;
                            currentState = MainMenuState.MainMenu;
                            storageText = false;
                        }
                        else
                        {
                            startPressed = false;
                            currentState = MainMenuState.PressStart;
                            storageText = false;
                            storageText = true;
                        }
                    }
                    catch
                    {
                        startPressed = false;
                        guideResult = null;
                        SetTooltip();
                        storageText = true;
                    }
                }
                else
                {
                    for (int i = 0; i < LocalInstanceManager.MaxNumberOfPlayers; i++)
                    {
                        if(InstanceManager.InputManager.ButtonPressed(Buttons.Start, (PlayerIndex)i))
                        {
                            SignedInGamerCollection signedIn = Gamer.SignedInGamers;
                            if (signedIn[(PlayerIndex)i] != null)
                            {
                                try
                                {
                                    playerWhoPressedStart = (PlayerIndex)i;
                                    guideResult = Guide.BeginShowStorageDeviceSelector(playerWhoPressedStart, null, null);
                                    startPressed = true;
                                    break;
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                Guide.ShowSignIn(1, false);
                            }
                        }
                    }
                }
            }
            else
            {
                if (device.IsConnected)
                {
                    if (currentState == MainMenuState.MainMenu)
                        InnerUpdate(mainMenuItems);
                    else
                    {
                        if (CheckButtonB())
                        {
                            teletype.FlushEntries();
                            ResetMenuItems();
                            currentSelection = 0;
                            currentState = MainMenuState.MainMenu;
                            LocalInstanceManager.WindowManager.SetLocation(mainMenuWindowLocation);
                        }
                        else
                            InnerUpdate(gameSelectItems);
                    }

                    // Our debug sequence can happen in any menu
                    switch (debugSequence)
                    {
                        case 3:
                            // the fourth and final button is X for logger
                            if (InstanceManager.InputManager.ButtonPressed(Buttons.X))
                            {
                                debugSequence = 0;
                                ((DuologueGame)myGame).Debug = !((DuologueGame)myGame).Debug;
                                InstanceManager.Logger.LogEntry("MainMenu.Update() - Debugging toggle");
                            }
                            else if (InstanceManager.InputManager.ButtonPressed(Buttons.RightShoulder))
                            {
                                // Or RB if we want the color state test
                                debugSequence = 0;
                                LocalInstanceManager.CurrentGameState = GameState.ColorStateTest;
                            }
                            break;
                        case 2:
                            // the third button is Y
                            if (InstanceManager.InputManager.ButtonPressed(Buttons.Y))
                                debugSequence = 3;
                            break;
                        case 1:
                            // the second button is RB
                            if (InstanceManager.InputManager.ButtonPressed(Buttons.RightShoulder))
                                debugSequence = 2;
                            break;
                        default:
                            // the first button is LB
                            if (InstanceManager.InputManager.ButtonPressed(Buttons.LeftShoulder))
                                debugSequence = 1;
                            break;
                    }
                }
                else
                {
                    currentState = MainMenuState.PressStart;
                    startPressed = false;
                    guideResult = null;
                    SetTooltip();
                }
            }
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (position == Vector2.Zero)
            {
                SetPosition();
                //initialized = true;
            }

            if (currentState == MainMenuState.PressStart)
            {
                DrawPressStart(gameTime);
            }
            else
            {

                LocalInstanceManager.WindowManager.Draw(gameTime);

                if (currentState == MainMenuState.MainMenu)
                    DrawMenu(mainMenuItems, gameTime);
                else if (currentState == MainMenuState.GameSelect)
                    DrawMenu(gameSelectItems, gameTime);
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}