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
// Duologue
using Duologue;
using Duologue.Properties;
using Duologue.State;
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
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string fontFilename = "Fonts/inero-50";
        private const float yOffset = 325f;
        private const float extraLineSpacing = 12;
        private const float selectOffset = 8;
        private const int numberOfOffsets = 4;
        #endregion

        #region Fields
        private SpriteFont font;
        private Vector2 position;
        private List<MenuItem> mainMenuItems;
        private List<MenuItem> gameSelectItems;
        private int currentSelection;
        // The list of menu items
        private int menuPlayGame;
        private int menuAchievements;
        private int menuCredits;
        private int menuExit;
        private int gameSelectCampaign;
        private int gameSelectInfinite;
        private int gameSelectBack;
        private MainMenuState currentState;
        /// <summary>
        /// Used for the debug sequence
        /// </summary>
        private int debugSequence;

        private Game myGame;

        private Vector2[] shadowOffsets;
        private Vector2[] shadowOffsetsSelected;
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

            // Set up the main menu
            mainMenuItems.Add(new MenuItem(Resources.MainMenu_Play));
            menuPlayGame = 0;
            mainMenuItems.Add(new MenuItem(Resources.MainMenu_Achievements));
            menuAchievements = 1;
            mainMenuItems.Add(new MenuItem(Resources.MainMenu_Credits));
            menuCredits = 2;
            mainMenuItems.Add(new MenuItem(Resources.MainMenu_Exit));
            menuExit = 3;

            // Set up the game select menu
            gameSelectItems.Add(new MenuItem(Resources.MainMenu_GameSelect_Campaign));
            gameSelectCampaign = 0;
            gameSelectItems.Add(new MenuItem(Resources.MainMenu_GameSelect_Infinite));
            gameSelectInfinite = 1;
            gameSelectItems.Add(new MenuItem(Resources.MainMenu_GameSelect_Back));
            gameSelectBack = 2;

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
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            foreach (MenuItem mi in mainMenuItems)
                mi.Invisible = false;

            // Turn off those items we don't support yet
            mainMenuItems[menuAchievements].Invisible = true;

            foreach (MenuItem mi in gameSelectItems)
                mi.Invisible = false;

            ResetMenuItems();

            currentState = MainMenuState.MainMenu;
            currentSelection = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = InstanceManager.AssetManager.LoadSpriteFont(fontFilename);
            base.LoadContent();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Generate the position
        /// </summary>
        private void SetPostion()
        {
            float center = InstanceManager.DefaultViewport.Width / 2f;
            float xOffset = center;
            foreach (MenuItem mi in mainMenuItems)
            {
                Vector2 size = font.MeasureString(mi.Text);
                float xTest = center - size.X / 2f;
                if (xTest < xOffset)
                    xOffset = xTest;
            }
            foreach (MenuItem mi in gameSelectItems)
            {
                Vector2 size = font.MeasureString(mi.Text);
                float xTest = center - size.X / 2f;
                if (xTest < xOffset)
                    xOffset = xTest;
            }
            position = new Vector2(xOffset, yOffset);
        }

        /// <summary>
        /// When a menu item is selected, this is where we parse it
        /// </summary>
        private void ParseSelected()
        {
            if (currentState == MainMenuState.MainMenu)
            {
                if (currentSelection == menuExit)
                    LocalInstanceManager.CurrentGameState = GameState.Exit;
                else if (currentSelection == menuPlayGame)
                {
                    currentState = MainMenuState.GameSelect;
                    currentSelection = 0;
                    ResetMenuItems();
                }
            }
            else
            {
                if (currentSelection == gameSelectBack)
                {
                    currentState = MainMenuState.MainMenu;
                    currentSelection = 0;
                    ResetMenuItems();
                }
                else if (currentSelection == gameSelectInfinite)
                {
                    currentState = MainMenuState.MainMenu;
                    currentSelection = 0;
                    ResetMenuItems();
                    LocalInstanceManager.CurrentGameState = GameState.PlayerSelect;
                    LocalInstanceManager.NextGameState = GameState.InfinityGame;
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
                InstanceManager.RenderSprite.DrawString(font,
                    mi.Text,
                    curPos,
                    Color.Azure,
                    RenderSpriteBlendMode.Multiplicative);
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
                        curPos + mi.FadePercent * selectOffset * Vector2.One,
                        mi.Selected ? Color.Gold : Color.LightGoldenrodYellow,
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
            }

            // Up
            if (IsMenuUp())
            {
                mis[currentSelection].Selected = false;
                currentSelection--;
            }

            if (currentSelection >= mis.Count)
                currentSelection = 0;
            else if (currentSelection < 0)
                currentSelection = mis.Count - 1;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Update / Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (currentState == MainMenuState.MainMenu)
                InnerUpdate(mainMenuItems);
            else
            {
                if (CheckButtonB())
                    currentState = MainMenuState.MainMenu;
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
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (position == Vector2.Zero)
            {
                SetPostion();
            }

            if (currentState == MainMenuState.MainMenu)
                DrawMenu(mainMenuItems, gameTime);
            else
                DrawMenu(gameSelectItems, gameTime);
            base.Draw(gameTime);
        }
        #endregion
    }
}