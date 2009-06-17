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
using Mimicware;
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
using Duologue.Audio;
#endregion

namespace Duologue.UI
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PauseScreen : Microsoft.Xna.Framework.DrawableGameComponent, IService
    {
        #region Constants
        private const string filename_overlay = "pause-overlay";
        private const string filename_fontTitle = "Fonts/inero-50";
        private const string filename_fontMenu = "Fonts/inero-40";
        private const string filename_fontWaveNum = "Fonts\\inero-28";

        private const string filename_LifeUp = "Audio/PlayerEffects/life-up";
        private const float volume_LifeUp = 1f;

        private const byte overlayAlpha = 192;

        private const byte maxTextRed = 255;
        private const byte minTextRed = 50;
        private const int maxDeltaTextRed = 20;

        private const int numberOfUpdatesPerTick = 5;

        private const double timePerJiggle = 0.1;

        private const float extraLineSpacing = 12;

        private const int windowOffsetX = 30;
        private const int windowOffsetY = 10;
        private const int titleSpacing = 40;

        private const float selectOffset = 8;
        private const int numberOfOffsets = 4;
        private const int konamiCodeLivesBonus = 100;

        private const float horizWaveNumOffset = 15f;
        #endregion

        #region Fields
        private Texture2D overlay;
        private SpriteFont fontTitle;
        private SpriteFont fontMenu;
        private SpriteFont fontWaveNum;

        private int numberOfTiles;
        private Vector2 tileCounts;

        private int screenWidth;
        private int screenHeight;
        private Vector2 screenCenter;

        private SpriteEffects[] tileEffects;

        private Color color_overlay;
        private Color color_text;
        private Color color_outline;
        private Vector2[] shadowOffset;
        private int deltaTextRed;

        private double timeSinceStart;

        private Game myGame;
        private bool initialized;

        // Menu items
        private Vector2 wavePosition;
        private Vector2 waveSize;
        private Vector2 menuOffset;
        private List<MenuItem> pauseMenuItems;
        private int resumeGame;
        private int exitMainMenu;
        private int medalCase;
        private Rectangle pauseMenuWindowLocation;
        private Vector2 position;
        private Vector2[] shadowOffsets;
        private Vector2[] shadowOffsetsSelected;
        private int currentSelection;

        // SHIT MY NIZZLE
        private int konamiCodeIndex;
        private Buttons[] konamiCode;
        private bool konamiCodeDone;
        private SoundEffect sfx_LifeUp;
        private SoundEffectInstance sfxi_LifeUp;

        // Medal screen stopper
        private bool inMedalScreen = false;
        #endregion

        #region Properties
        #endregion

        #region Constructor/Init/Load
        public PauseScreen(Game game)
            : base(game)
        {
            myGame = game;

            pauseMenuItems = new List<MenuItem>();

            pauseMenuItems.Add(new MenuItem(Resources.PauseScreen_ResumeGame));
            resumeGame = 0;
            pauseMenuItems.Add(new MenuItem(Resources.PauseScreen_MedalCase));
            medalCase = 1;
            pauseMenuItems.Add(new MenuItem(Resources.PauseScreen_ExitMainMenu));
            exitMainMenu = 2;

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

            konamiCode = new Buttons[]
            {
                Buttons.DPadUp,
                Buttons.DPadUp,
                Buttons.DPadDown,
                Buttons.DPadDown,
                Buttons.DPadLeft,
                Buttons.DPadRight,
                Buttons.DPadLeft,
                Buttons.DPadRight,
                Buttons.B,
                Buttons.A
            };

            initialized = false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            shadowOffset = new Vector2[2];
            shadowOffset[0] = 3 * Vector2.One;
            shadowOffset[1] = -3 * Vector2.One;

            color_outline = Color.Black;

            color_text = new Color(maxTextRed, 128, 128);
            deltaTextRed = -1 * maxDeltaTextRed;

            currentSelection = 0;
            timeSinceStart = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            overlay = InstanceManager.AssetManager.LoadTexture2D(filename_overlay);
            fontTitle = InstanceManager.AssetManager.LoadSpriteFont(filename_fontTitle);
            fontMenu = InstanceManager.AssetManager.LoadSpriteFont(filename_fontMenu);
            fontWaveNum = InstanceManager.AssetManager.LoadSpriteFont(filename_fontWaveNum);

            numberOfTiles = -1;

            color_overlay = new Color(Color.DarkSlateGray, overlayAlpha);

            sfx_LifeUp = InstanceManager.AssetManager.LoadSoundEffect(filename_LifeUp);
            sfxi_LifeUp = null;

            base.LoadContent();
        }
        #endregion

        #region Private Methods
        private void InitAll()
        {
            screenWidth = InstanceManager.DefaultViewport.Width;
            screenHeight = InstanceManager.DefaultViewport.Height;

            // Setup the tile stuff
            tileCounts = new Vector2(
                (float)Math.Ceiling((double)screenWidth / (double)overlay.Width),
                (float)Math.Ceiling((double)screenHeight / (double)overlay.Height));

            numberOfTiles = (int)(tileCounts.X * tileCounts.Y);

            tileEffects = new SpriteEffects[numberOfTiles];
            for (int i = 0; i < numberOfTiles; i++)
            {
                JumbleTile(i);
            }

            konamiCodeIndex = 0;
            konamiCodeDone = false;

            // Setup the font stuff
            screenCenter = new Vector2(
                screenWidth / 2f,
                screenHeight / 2f);
        }

        private void JumbleTile(int i)
        {
            if (MWMathHelper.CoinToss())
                tileEffects[i] = SpriteEffects.FlipHorizontally;
            else
                tileEffects[i] = SpriteEffects.None;
        }

        /// <summary>
        /// Generate the position
        /// </summary>
        private void SetPostion()
        {
            float center = InstanceManager.DefaultViewport.Width / 2f;
            float xOffset = center;
            float maxWidth = 0;
            float maxHeight = 0;
            Vector2 size;
            float xTest;

            // Start with the title
            size = fontTitle.MeasureString(Resources.PauseScreen_GamePaused);
            xTest = center - size.X / 2f;
            if (xTest < xOffset)
                xOffset = xTest;

            if (size.X > maxWidth)
                maxWidth = size.X;

            maxHeight += fontTitle.LineSpacing + titleSpacing;

            // Move to the menu
            foreach (MenuItem mi in pauseMenuItems)
            {
                size = fontMenu.MeasureString(mi.Text);
                xTest = center - size.X / 2f;
                if (xTest < xOffset)
                    xOffset = xTest;

                // Compute max width and height
                if (size.X > maxWidth)
                    maxWidth = size.X;
                maxHeight += fontMenu.LineSpacing + extraLineSpacing;
            }

            position = new Vector2(
                screenCenter.X - maxWidth/2f,
                screenCenter.Y - maxHeight/2f);

            pauseMenuWindowLocation = new Rectangle(
                (int)position.X - windowOffsetX,
                (int)position.Y - windowOffsetY,
                (int)maxWidth + 2 * windowOffsetX,
                (int)maxHeight + fontMenu.LineSpacing + (int)extraLineSpacing + 2 * windowOffsetY);

            waveSize = fontWaveNum.MeasureString(Resources.PauseScreen_WaveNum);

            wavePosition = new Vector2(
                screenCenter.X - waveSize.X / 2f,
                pauseMenuWindowLocation.Y - (extraLineSpacing + 2f * waveSize.Y));

            LocalInstanceManager.WindowManager.SetLocation(pauseMenuWindowLocation);
            initialized = true;
        }

        /// <summary>
        /// Draw a list of menu items FIXME should be in abstract class
        /// </summary>
        private void DrawMenu(List<MenuItem> mis, GameTime gameTime, Vector2 curPos)
        {
            foreach (MenuItem mi in mis)
            {
                mi.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                if (mi.Invisible)
                {
                    InstanceManager.RenderSprite.DrawString(fontMenu,
                        mi.Text,
                        curPos,
                        mi.Selected ? Color.DarkSlateBlue : Color.DarkSlateGray,
                        RenderSpriteBlendMode.AddititiveTop);
                }
                else
                {
                    InstanceManager.RenderSprite.DrawString(
                        fontMenu,
                        mi.Text,
                        curPos + mi.FadePercent * selectOffset * Vector2.UnitX,
                        mi.Selected ? Color.LightGoldenrodYellow : Color.Khaki,
                        Color.Black,
                        mi.Selected ? shadowOffsetsSelected : shadowOffsets,
                        RenderSpriteBlendMode.AlphaBlendTop);
                }
                curPos.Y += fontMenu.LineSpacing + extraLineSpacing;
            }

        }

        /// <summary>
        /// When a menu item is selected, this is where we parse it
        /// </summary>
        private void ParseSelected()
        {
            if (currentSelection == resumeGame)
            {
                ResetMenuItems(pauseMenuItems);
                LocalInstanceManager.Pause = false;
            }
            else if (currentSelection == exitMainMenu)
            {
                ResetMenuItems(pauseMenuItems);
                LocalInstanceManager.Pause = false;
                LocalInstanceManager.CurrentGameState = GameState.MainMenuSystem;
            }
            else if (currentSelection == medalCase)
            {
                ResetMenuItems(pauseMenuItems);
                // OMG HACKERY
                ServiceLocator.GetService<GamePlayLoop>().Visible = false;
                for (int i = 0; i < InputManager.MaxInputs; i++)
                {
                    if (LocalInstanceManager.Players[i].Active)
                        LocalInstanceManager.Scores[i].Visible = false;
                }
                // Turn off background
                LocalInstanceManager.Background.Enabled = false;
                LocalInstanceManager.Background.Visible = false;
                // Turn off particle effects
                LocalInstanceManager.BulletParticle.Visible = false;
                LocalInstanceManager.EnemyExplodeSystem.Visible = false;
                LocalInstanceManager.EnemySplatterSystem.Visible = false;
                LocalInstanceManager.PlayerExplosion.Visible = false;
                LocalInstanceManager.PlayerRing.Visible = false;
                LocalInstanceManager.PlayerSmoke.Visible = false;
                LocalInstanceManager.Steam.Visible = false;

                LocalInstanceManager.AchievementManager.EnableMedalScreen();
                LocalInstanceManager.AchievementManager.ReturnToPause = true;
                inMedalScreen = true;
            }
        }

        private void ResetMenuItems(List<MenuItem> mis)
        {
            foreach (MenuItem mi in mis)
                mi.Selected = false;
            currentSelection = 0;
        }

        /// <summary>
        /// Inner update FIXME should be in abstract class
        /// </summary>
        private void InnerUpdate(List<MenuItem> mis)
        {
            mis[currentSelection].Selected = true;

            // See if we have a button down to select
            if (CheckButtonA() && konamiCodeIndex < 9)
            {
                ParseSelected();
            }

            if (!konamiCodeDone)
            {
                bool nothingElsePushed = true;
                // Check to make sure none of the other sequences were pressed
                for (int i = 0; i < konamiCode.Length; i++)
                {
                    if (konamiCode[i] != konamiCode[konamiCodeIndex])
                    {
                        if (InstanceManager.InputManager.NewButtonPressed(konamiCode[i]))
                        {
                            nothingElsePushed = false;
                            break;
                        }
                    }
                }
                if (nothingElsePushed)
                {
                    if (InstanceManager.InputManager.NewButtonPressed(konamiCode[konamiCodeIndex]))
                    {
                        InstanceManager.Logger.LogEntry(String.Format(
                            "Cheat code: {0}-{1}", konamiCodeIndex.ToString(), konamiCode[konamiCodeIndex].ToString()));
                        konamiCodeIndex++;
                        if (konamiCodeIndex >= konamiCode.Length)
                        {
                            bool addedScore = false;
                            for (int i = 0; i < LocalInstanceManager.Scores.Length; i++)
                            {
                                if (LocalInstanceManager.Players[i].Active &&
                                    LocalInstanceManager.Players[i].State != PlayerState.Dead &&
                                    LocalInstanceManager.Players[i].State != PlayerState.Dying)
                                {
                                    LocalInstanceManager.Scores[i].SetLives(
                                        LocalInstanceManager.Scores[i].Lives + konamiCodeLivesBonus);
                                    addedScore = true;
                                }
                            }

                            if (addedScore)
                            {

                                if (sfxi_LifeUp == null)
                                {
                                    try
                                    {
                                        sfxi_LifeUp = sfx_LifeUp.Play(volume_LifeUp);
                                    }
                                    catch { }
                                }
                                else
                                {
                                    try
                                    {
                                        if (sfxi_LifeUp.State != SoundState.Playing)
                                            sfxi_LifeUp.Play();
                                    }
                                    catch { }
                                }
                                InstanceManager.Logger.LogEntry(String.Format(
                                    "Bing, K0n4m1 code! {0}", konamiCodeIndex));
                                konamiCodeIndex = 0;
                                konamiCodeDone = true;
                            }
                            else
                            {
                                InstanceManager.Logger.LogEntry(
                                    "Tried to do K0n4m1 code, but no living players, sorry man..");
                                konamiCodeIndex = 0;
                                konamiCodeDone = false;

                            }
                        }
                    }
                }
                else
                {
                        konamiCodeIndex = 0;
                }
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

        /// <summary>
        /// Check to see if the A or select button is pressed
        /// FIXME should be in abstract class
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
        /// FIXME should be in abstract class
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
        /// FIXME should be in abstract class
        /// </summary>
        private bool IsMenuDown()
        {
            bool pressed = false;

            for (int i = 0; i < InstanceManager.InputManager.CurrentGamePadStates.Length; i++)
            {
                if (InstanceManager.InputManager.CurrentKeyboardStates[i].IsKeyDown(Keys.Down) &&
                    InstanceManager.InputManager.LastKeyboardStates[i].IsKeyUp(Keys.Down))
                {
                    pressed = true;
                    break;
                }
                if ((InstanceManager.InputManager.CurrentGamePadStates[i].DPad.Down == ButtonState.Pressed &&
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
        /// FIXME should be in abstract class
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

        #region Overrides
        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (this.Enabled && initialized)
            {
                LocalInstanceManager.WindowManager.SetLocation(pauseMenuWindowLocation);
                konamiCodeIndex = 0;
                konamiCodeDone = false;
            }
            base.OnEnabledChanged(sender, args);
        }
        #endregion

        #region Public Methods
        public void ReturnFromMedalScreen()
        {
            inMedalScreen = false;
            // OMG HACKERY
            ServiceLocator.GetService<GamePlayLoop>().Visible = true;
            LocalInstanceManager.AchievementManager.DisableMedalScreen();
            // Turn on background
            LocalInstanceManager.Background.Enabled = true;
            LocalInstanceManager.Background.Visible = true;
            // Turn on particle effects
            LocalInstanceManager.BulletParticle.Visible = true;
            LocalInstanceManager.EnemyExplodeSystem.Visible = true;
            LocalInstanceManager.EnemySplatterSystem.Visible = true;
            LocalInstanceManager.PlayerExplosion.Visible = true;
            LocalInstanceManager.PlayerRing.Visible = true;
            LocalInstanceManager.PlayerSmoke.Visible = true;
            LocalInstanceManager.Steam.Visible = true;

            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                if (LocalInstanceManager.Players[i].Active)
                    LocalInstanceManager.Scores[i].Visible = true;
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
            if (inMedalScreen)
            {
                LocalInstanceManager.AchievementManager.Update(gameTime);
            }
            else
            {
                timeSinceStart += gameTime.ElapsedGameTime.TotalSeconds;

                // Check for start, back, or B
                if ((InstanceManager.InputManager.NewButtonPressed(Buttons.B) && konamiCodeIndex != 8) ||
                    InstanceManager.InputManager.NewButtonPressed(Buttons.Start) ||
                    InstanceManager.InputManager.NewButtonPressed(Buttons.Back))
                {
                    ResetMenuItems(pauseMenuItems);
                    LocalInstanceManager.Pause = false;
                }

                InnerUpdate(pauseMenuItems);

                // We only want to proceed provided the InitAll() was called
                // Since we have no guarantee that the screen is initialized here in
                // update, we ultimately will have to pass until Draw() is called
                if (numberOfTiles > 0)
                {
                    // Update the font color
                    int t = (int)color_text.R + deltaTextRed;

                    if (t > maxTextRed)
                    {
                        t = (int)maxTextRed;
                        deltaTextRed = -1 * maxDeltaTextRed;
                    }
                    else if (t < minTextRed)
                    {
                        t = (int)minTextRed;
                        deltaTextRed = maxDeltaTextRed;
                    }

                    color_text = new Color((byte)t, 128, 128);

                    // Update the overlays
                    if (timeSinceStart > timePerJiggle)
                    {
                        timeSinceStart = 0;
                        int i;
                        for (t = 0; t < numberOfUpdatesPerTick; t++)
                        {
                            i = MWMathHelper.GetRandomInRange(0, numberOfTiles);
                            if (tileEffects[i] == SpriteEffects.None)
                                tileEffects[i] = SpriteEffects.FlipHorizontally;
                            else
                                tileEffects[i] = SpriteEffects.None;
                        }
                    }
                }
                LocalInstanceManager.WindowManager.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!inMedalScreen)
            {
                if (numberOfTiles <= 0)
                {
                    InitAll();
                    SetPostion();
                }

                float x = 0f;
                float y = 0f;
                // Draw the tiles
                for (int i = 0; i < numberOfTiles; i++)
                {
                    InstanceManager.RenderSprite.Draw(
                        overlay,
                        new Vector2(x, y),
                        Vector2.Zero,
                        null,
                        color_overlay,
                        0f,
                        1f,
                        0f,
                        RenderSpriteBlendMode.AlphaBlendTop,
                        tileEffects[i]);

                    x += overlay.Width;
                    if (x >= screenWidth)
                    {
                        x = 0f;
                        y += overlay.Height;
                        if (y >= screenHeight)
                        {
                            // okay, wtf, we shouldn't be here
                            y = 0f;
                        }
                    }
                }

                LocalInstanceManager.WindowManager.Draw(gameTime);

                // Draw the menu
                menuOffset = Vector2.Zero;

                InstanceManager.RenderSprite.DrawString(
                    fontTitle,
                    Resources.PauseScreen_GamePaused,
                    position + menuOffset,
                    color_text,
                    color_outline,
                    shadowOffset,
                    RenderSpriteBlendMode.AlphaBlendTop);

                menuOffset.Y += fontTitle.LineSpacing + titleSpacing;

                DrawMenu(pauseMenuItems, gameTime, position + menuOffset);

                InstanceManager.RenderSprite.DrawString(
                    fontWaveNum,
                    String.Format(
                        Resources.PauseScreen_WaveNum,
                        LocalInstanceManager.CurrentGameWave.MajorWaveNumber,
                        LocalInstanceManager.CurrentGameWave.MinorWaveNumber,
                        LocalInstanceManager.CurrentGameWave.CurrentWavelet),
                        wavePosition,
                    Color.BlanchedAlmond,
                    RenderSpriteBlendMode.AlphaBlendTop);

                InstanceManager.RenderSprite.DrawString(
                    fontWaveNum,
                    InstanceManager.Localization.Get(
                        LocalInstanceManager.CurrentGameWave.Name),
                    wavePosition + Vector2.UnitX * (waveSize.X / 2f - fontWaveNum.MeasureString(InstanceManager.Localization.Get(LocalInstanceManager.CurrentGameWave.Name)).X / 2f)
                    + Vector2.UnitY * fontWaveNum.LineSpacing,
                    Color.BlanchedAlmond,
                    RenderSpriteBlendMode.AlphaBlendTop);

            }
            base.Draw(gameTime);
        }
        #endregion
    }
}