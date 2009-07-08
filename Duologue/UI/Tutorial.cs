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
using Mimicware;
// Duologue
using Duologue;
using Duologue.Properties;
using Duologue.Screens;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.UI;
using Duologue.State;
using Duologue.Audio;
#endregion

namespace Duologue.UI
{
    public struct TutorialEntry
    {
        public string Text;
        public Vector2 TextSize;
        public Vector2 Center;
        public bool SmallFont;
        public bool IsTip;
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
    public class Tutorial : Microsoft.Xna.Framework.DrawableGameComponent, IService
    {
        #region Constants
        private const string filename_PopUpWindow = "PlayerUI/pop-up-window";
        private const string filename_Font = "Fonts/deja-med";//"Fonts\\inero-28";
        private const string filename_TipFont = "Fonts/deja-med";//"Fonts/inero-small";

        private const double time_ScaleVert = 0.2f;
        private const double time_ScaleHoriz = 0.6f;
        private const double time_Steady = 3.5f;
        private const double time_InScaleHoriz = 0.3f;
        private const double time_OffDelay = 0.1f;

        /// <summary>
        /// This is how many separate game times the player has to play before
        /// the tutorial goes away
        /// </summary>
        private const int numberOfTimesToDisplayTutorial = 8;

        private const float width_MinPopup = 10f;
        private const float width_ExtraPopup = 50f;

        private const float offset_WindowVert = -25f;
        private const float offset_ShadowScale = 2f;

        private const double percentageToSpawnEnemies = 0.75;
        #endregion

        #region Fields
        private Texture2D texture_PopUpWindow;
        private Vector2 center_PopUpWindow;
        private SpriteFont font;
        private SpriteFont tipFont;
        private TutorialEntry[] theEntries;
        private Queue<TutorialEntry> requestedToBeDisplayed;
        private TutorialEntry currentEntry;
        private TutorialEntry infiniteIntro;
        private int lastEntryCount;
        private PopUpState currentState;
        private double stateTimer;

        private Vector2 position;
        private Color color_Window;
        private Color color_Text;
        private Color color_TextShadow;
        private Vector2[] shadowOffset;

        private TutorialEntry[] proTips;

        private bool tutOnscreen;
        private bool infiniteIntroDisplayed = false;
        #endregion

        #region Properties
        public bool TutorialOnscreen
        {
            get { return tutOnscreen; }
        }
        #endregion

        #region Constructor / Init
        public Tutorial(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            color_Window = new Color(182, 96, 255);
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
            tipFont = InstanceManager.AssetManager.LoadSpriteFont(filename_TipFont);

            theEntries = new TutorialEntry[3];
            theEntries[0].Text = Resources.Tutorial_1;
            theEntries[1].Text = Resources.Tutorial_2;
            theEntries[2].Text = Resources.Tutorial_3;
            for (int i = 0; i < theEntries.Length; i++)
            {
                theEntries[i].TextSize = font.MeasureString(theEntries[i].Text);
                theEntries[i].Center = new Vector2(
                    theEntries[i].TextSize.X / 2f, theEntries[i].TextSize.Y / 2f);
                theEntries[i].SmallFont = false;
            }

            string[] tempTips = new string[]
            {
                Resources.Tip001,
                Resources.Tip002,
                Resources.Tip003,
                Resources.Tip004,
                Resources.Tip005,
                Resources.Tip006,
                Resources.Tip007,
                Resources.Tip008,
                Resources.Tip009,
                Resources.Tip010,
            };

            proTips = new TutorialEntry[tempTips.Length];

            for (int i = 0; i < tempTips.Length; i++)
            {
                proTips[i].Text = tempTips[i];
                proTips[i].TextSize = tipFont.MeasureString(tempTips[i]);
                proTips[i].Center = new Vector2(
                    proTips[i].TextSize.X / 2f, proTips[i].TextSize.Y / 2f);
                proTips[i].SmallFont = true;
                proTips[i].IsTip = true;
            }

            infiniteIntro = new TutorialEntry();
            infiniteIntro.Text = Resources.Tutorial_InfiniteMode;
            infiniteIntro.TextSize = font.MeasureString(infiniteIntro.Text);
            infiniteIntro.Center = new Vector2(
                infiniteIntro.TextSize.X / 2f, infiniteIntro.TextSize.Y / 2f);
            infiniteIntro.SmallFont = false;
            infiniteIntro.IsTip = false;

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
            tutOnscreen = false;
            infiniteIntroDisplayed = false;
            Visible = false;
            Enabled = false;
        }

        /// <summary>
        /// Once the game has started, call this every level
        /// </summary>
        public void NewLevel()
        {
            if (infiniteIntroDisplayed || LocalInstanceManager.CurrentGameState != GameState.InfiniteGame)
            {
                if (lastEntryCount < 1)
                {
                    if (LocalInstanceManager.AchievementManager.DisplayTutorial(numberOfTimesToDisplayTutorial))
                    {
                        // Cool, we can display the tutorial
                        InstanceManager.Logger.LogEntry(String.Format("Tutorial to display: {0}", theEntries[lastEntryCount].Text));
                        requestedToBeDisplayed.Enqueue(theEntries[lastEntryCount]);
                        lastEntryCount++;
                        Visible = true;
                        Enabled = true;
                        tutOnscreen = true;
                    }
                }
                else if (lastEntryCount < theEntries.Length)
                {
                    requestedToBeDisplayed.Enqueue(theEntries[lastEntryCount]);
                    lastEntryCount++;
                    Visible = true;
                    Enabled = true;
                    tutOnscreen = true;
                }
            }
            else
            {
                requestedToBeDisplayed.Enqueue(infiniteIntro);
                Visible = true;
                Enabled = true;
                tutOnscreen = true;
                infiniteIntroDisplayed = true;
            }
        }

        /// <summary>
        /// Call when the game is over to (maybe) display a tip
        /// </summary>
        public void TipPopUp()
        {
            int i = MWMathHelper.GetRandomInRange(0, proTips.Length);

            InstanceManager.Logger.LogEntry(String.Format("Pro-tip to display: {0}", i.ToString()));

            requestedToBeDisplayed.Enqueue(proTips[i]);
            Visible = true;
            Enabled = true;
        }
        #endregion

        #region Private Methods
        private void SetPosition()
        {
            position = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f,
                (float)InstanceManager.DefaultViewport.TitleSafeArea.Bottom - center_PopUpWindow.Y + offset_WindowVert);
            InstanceManager.Logger.LogEntry(String.Format("Tutorial pos: {0}", position));
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

            if (currentEntry.SmallFont)
            {
                InstanceManager.RenderSprite.DrawString(
                    tipFont,
                    currentEntry.Text,
                    position,
                    color_Text,
                    color_TextShadow,
                    1f,
                    currentEntry.Center,
                    shadowOffset,
                    RenderSpriteBlendMode.AbsoluteTop);
            }
            else
            {
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
                    if (stateTimer > time_Steady * percentageToSpawnEnemies)
                    {
                        tutOnscreen = false;
                    }
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
                        if (currentEntry.IsTip)
                            tutOnscreen = false;
                        else
                            tutOnscreen = true;
                        stateTimer = 0;
                    }
                    else
                    {
                        // Nothing left, shut ourselves down
                        Enabled = false;
                        Visible = false;
                        tutOnscreen = false;
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