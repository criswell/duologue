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
using Duologue.State;
#endregion

namespace Duologue.Screens
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string fontFilename = "Fonts/bloj-36";
        private const float yOffset = 325f;
        private const float extraLineSpacing = 12;
        private const float selectOffset = 8;
        // FIXME
        // The following should be deleted, testing
        private const float lifetime = 1f;
        #endregion

        #region Fields
        private SpriteFont font;
        private Vector2 position;
        private List<MenuItem> mainMenuItems;
        private int currentSelection;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public MainMenu(Game game)
            : base(game)
        {
            position = Vector2.Zero;
            currentSelection = 0;
            mainMenuItems = new List<MenuItem>();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            mainMenuItems.Add(new MenuItem(Resources.MainMenu_SinglePlayer));
            mainMenuItems.Add(new MenuItem(Resources.MainMenu_MultiPlayer));
            mainMenuItems.Add(new MenuItem(Resources.MainMenu_Options));
            mainMenuItems.Add(new MenuItem(Resources.MainMenu_Achievements));
            mainMenuItems.Add(new MenuItem(Resources.MainMenu_Leaderboards));
            mainMenuItems.Add(new MenuItem(Resources.MainMenu_Exit));

            foreach (MenuItem mi in mainMenuItems)
            {
                mi.Invisible = false;
            }

            // Turn off those items we don't support yet
            mainMenuItems[3].Invisible = true;
            mainMenuItems[4].Invisible = true;

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
            position = new Vector2(xOffset, yOffset);
        }

        /// <summary>
        /// When a menu item is selected, this is where we parse it
        /// </summary>
        private void ParseSelected()
        {
            switch (currentSelection)
            {
                case 5:
                    LocalInstanceManager.CurrentGameState = GameState.Exit;
                    break;
                default:
                    break;
            }
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
            mainMenuItems[currentSelection].Selected = true;

            // See if we have a button down to select
            if (CheckButtonA())
            {
                ParseSelected();
            }

            // Determine if we've got a new selection
            // Down -- Yeech, this be fugly
            if (IsMenuDown())
            {
                mainMenuItems[currentSelection].Selected = false;
                currentSelection++;
            }

            // Up
            if (IsMenuUp())
            {
                mainMenuItems[currentSelection].Selected = false;
                currentSelection--;
            }

            if (currentSelection >= mainMenuItems.Count)
                currentSelection = 0;
            else if (currentSelection < 0)
                currentSelection = mainMenuItems.Count - 1;
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (position == Vector2.Zero)
            {
                SetPostion();
            }
            Vector2 curPos = position;
            foreach (MenuItem mi in mainMenuItems)
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
                        RenderSpriteBlendMode.Addititive);
                }
                else
                {
                    InstanceManager.RenderSprite.DrawString(font,
                        mi.Text,
                        curPos + mi.FadePercent * selectOffset * Vector2.One,
                        mi.Selected ? Color.Azure : Color.DimGray,
                        RenderSpriteBlendMode.Addititive);
                }
                curPos.Y += font.LineSpacing + extraLineSpacing;
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}