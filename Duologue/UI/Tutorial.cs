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
using Duologue.State;
#endregion


namespace Duologue.UI
{
    public struct TutorialEntry
    {
        public string Text;
        public Vector2 TextSize;
        public Vector2 Center;
    }

    public enum PopUpState
    {
        ScaleVert,
        ScaleHoriz,
        Steady,
        InScaleHoriz,
        OffDelay,
        None,
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Tutorial : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string filename_PopUpWindow = "PlayerUI/pop-up-window";
        private const string filename_Font = "Fonts\\inero-28";

        private const double time_ScaleVert = 0.2f;
        private const double time_ScaleHoriz = 0.6f;
        private const double time_Steady = 2f;
        private const double time_InScaleHoriz = 0.3f;
        private const double time_OffDelay = 0.1f;

        /// <summary>
        /// This is how many separate game times the player has to play before
        /// the tutorial goes away
        /// </summary>
        private const int numberOfTimesToDisplayTutorial = 5;

        private const float width_MinPopup = 10f;
        private const float width_ExtraPopup = 20f;

        private const float offset_WindowVert = -25f;
        private const float offset_ShadowScale = 2f;
        #endregion

        #region Fields
        private Texture2D texture_PopUpWindow;
        private Vector2 center_PopUpWindow;
        private SpriteFont font;
        private TutorialEntry[] theEntries;
        private Queue<TutorialEntry> requestedToBeDisplayed;
        private TutorialEntry currentEntry;
        private int lastEntryCount;
        private PopUpState currentState;
        private double stateTimer;

        private Vector2 position;
        private Color color_Window;
        private Color color_Text;
        private Color color_TextShadow;
        private Vector2[] shadowOffset;
        #endregion

        #region Constructor / Init
        public Tutorial(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            color_Window = Color.White;
            color_Text = Color.PowderBlue;
            color_TextShadow = Color.Black;

            shadowOffset = new Vector2[]
            {
                Vector2.One * offset_ShadowScale,
            };
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            lastEntryCount = 0;

            requestedToBeDisplayed = new Queue<TutorialEntry>(5); // Hard coded to 5 since we don't want to have more than that

            Reset();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture_PopUpWindow = InstanceManager.AssetManager.LoadTexture2D(filename_PopUpWindow);
            center_PopUpWindow = new Vector2(
                texture_PopUpWindow.Width / 2f, texture_PopUpWindow.Height / 2f);

            font = InstanceManager.AssetManager.LoadSpriteFont(filename_Font);

            theEntries = new TutorialEntry[3];
            theEntries[0].Text = Resources.Tutorial_1;
            theEntries[1].Text = Resources.Tutorial_2;
            theEntries[2].Text = Resources.Tutorial_3;
            for (int i = 0; i < theEntries.Length; i++)
            {
                theEntries[i].TextSize = font.MeasureString(theEntries[i].Text);
                theEntries[i].Center = new Vector2(
                    theEntries[i].TextSize.X / 2f, theEntries[i].TextSize.Y / 2f);
            }

            base.LoadContent();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Called at the begining or end of the game to reset the tutorials
        /// </summary>
        public void Reset()
        {
            lastEntryCount = 0;
            requestedToBeDisplayed.Clear();
            currentState = PopUpState.None;
            position = Vector2.Zero;
            Visible = false;
            Enabled = false;
        }

        /// <summary>
        /// Once the game has started, call this every level
        /// </summary>
        public void NewLevel()
        {
            if (lastEntryCount < 1)
            {
                if (LocalInstanceManager.AchievementManager.DisplayTutorial(numberOfTimesToDisplayTutorial))
                {
                    // Cool, we can display the tutorial
                    requestedToBeDisplayed.Enqueue(theEntries[lastEntryCount]);
                    lastEntryCount++;
                    Visible = true;
                    Enabled = true;
                }
            }
            else if (lastEntryCount < theEntries.Length - 1)
            {
                requestedToBeDisplayed.Enqueue(theEntries[lastEntryCount]);
                lastEntryCount++;
                Visible = true;
                Enabled = true;
            }
        }
        #endregion

        #region Private Methods
        private void SetPosition()
        {
            position = new Vector2(
                InstanceManager.GraphicsDevice.DisplayMode.Width / 2f,
                InstanceManager.GraphicsDevice.DisplayMode.TitleSafeArea.Bottom - center_PopUpWindow.Y + offset_WindowVert);
        }
        /// <summary>
        /// Draw the window at a specified size
        /// </summary>
        /// <param name="w">The width of the window</param>
        /// <param name="h">The height as a percentage</param>
        private void DrawWindow(float w, double h)
        {
            if (position == Vector2.Zero)
                SetPosition();

            Vector2 scale = new Vector2(
                w / texture_PopUpWindow.Width, (float)h);

            InstanceManager.RenderSprite.Draw(
                texture_PopUpWindow,
                position,
                center_PopUpWindow,
                null,
                color_Window,
                0f,
                scale,
                0f,
                RenderSpriteBlendMode.AbsoluteTop);
        }

        private void DrawEntry()
        {
            if (position == Vector2.Zero)
                SetPosition();

            InstanceManager.RenderSprite.DrawString(
                font,
                currentEntry.Text,
                position,
                color_Text,
                color_TextShadow,
                1f,
                currentEntry.Center,
                shadowOffset,
                RenderSpriteBlendMode.AbsoluteTop);
        }
        #endregion

        #region Update / Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            stateTimer += gameTime.ElapsedGameTime.TotalSeconds;

            switch (currentState)
            {
                case PopUpState.ScaleVert:
                    if (stateTimer > time_ScaleVert)
                    {
                        stateTimer = 0;
                        currentState = PopUpState.ScaleHoriz;
                    }
                    break;
                case PopUpState.ScaleHoriz:
                    if (stateTimer > time_ScaleHoriz)
                    {
                        stateTimer = 0;
                        currentState = PopUpState.Steady;
                    }
                    break;
                case PopUpState.Steady:
                    if (stateTimer > time_Steady)
                    {
                        stateTimer = 0;
                        currentState = PopUpState.InScaleHoriz;
                    }
                    break;
                case PopUpState.InScaleHoriz:
                    if (stateTimer > time_InScaleHoriz)
                    {
                        stateTimer = 0;
                        currentState = PopUpState.OffDelay;
                    }
                    break;
                case PopUpState.OffDelay:
                    if (stateTimer > time_OffDelay)
                    {
                        stateTimer = 0;
                        currentState = PopUpState.None;
                    }
                    break;
                default:
                    // Check for a nother one in the queue
                    if (requestedToBeDisplayed.Count > 0)
                    {
                        currentEntry = requestedToBeDisplayed.Dequeue();
                        currentState = PopUpState.ScaleVert;
                        stateTimer = 0;
                    }
                    else
                    {
                        // Nothing left, shut ourselves down
                        Enabled = false;
                        Visible = false;
                        stateTimer = 0;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            switch (currentState)
            {
                case PopUpState.ScaleVert:
                    DrawWindow(width_MinPopup, stateTimer / time_ScaleVert);
                    break;
                case PopUpState.ScaleHoriz:
                    DrawWindow(
                        MathHelper.Lerp(width_MinPopup, width_ExtraPopup + (float)currentEntry.TextSize.X, (float)(stateTimer / time_ScaleHoriz)),
                        1.0);
                    break;
                case PopUpState.Steady:
                    DrawWindow(width_ExtraPopup + (float)currentEntry.TextSize.X, 1.0);
                    DrawEntry();
                    break;
                case PopUpState.InScaleHoriz:
                    DrawWindow(
                        MathHelper.Lerp(width_ExtraPopup + (float)currentEntry.TextSize.X, 0f, (float)(stateTimer / time_ScaleHoriz)),
                        1.0);
                    break;
                default:
                    // We do nothing for None and OffDelay
                    break;
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}