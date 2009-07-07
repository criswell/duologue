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
using Mimicware;
using Mimicware.Fx;
// Duologue
using Duologue;
using Duologue.Audio;
using Duologue.State;
using Duologue.Properties;
using Duologue.Screens;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.UI;
#endregion

namespace Duologue.Screens
{
    public enum BuyScreenState
    {
        FadeIn,
        Steady,
        FadeOut,
        Wait,
    }

    public class BuyScreen : DrawableGameComponent
    {
        #region Constants
        private const string filename_BackgroundLayer = "BuyScreen/bkg-layer";
        private const string filename_Screens = "BuyScreen/{0}";
        private const int numberOfScreens = 14;

        private const string filename_FeatureFont = "Fonts/deja-med";
        private const string filename_ButtonFont = "Fonts/deja-med";

        private const string filename_LogoBase = "logo-base";
        private const string filename_LogoBorder = "logo-border-{0}";
        private const int numberOfBorderFrames = 6;

        private const string filename_ButtonA = "PlayerUI/buttonA";
        private const string filename_ButtonB = "PlayerUI/buttonB";
        private const string filename_ButtonX = "PlayerUI/buttonX";
        private const string filename_ButtonY = "PlayerUI/buttonY";

        private const float alpha_Layer = 1f;//0.65f;

        private const float spacing_Buttons = 15f;

        private const float maxOffsetScreenshot = 80f;

        private const float screenWidth = 850f;
        private const float screenHeight = 525f;

        private const float minSize_FadeIn = 0.56f;
        private const float maxSize_FadeIn = 0.96f;

        private const float minSize_Steady = 0.96f;
        private const float maxSize_Steady = 1.1f;

        private const float minSize_FadeOut = 1.1f;
        private const float maxSize_FadeOut = 1.5f;

        private const int numOfBlurredScreens = 20;

        private const float buttonFontOffset = 14f;

        private const float shadowLength = 6f;

        private const float spacing_Title = 45f;
        private const float spacing_Features = 17f;
        private const float spacing_HorizFeatures = 25f;

        #region Timers
        private const double totalTime_BackgroundCycle = 4.3;
        private const double totalTime_FadeIn = 0.45;
        private const double totalTime_Steady = 2.1;
        private const double totalTime_FadeOut = 0.45;
        private const double totalTime_Wait = 0.15;

        private const double totalTime_Type = 0.23;
        private const double totalTime_LogoFrame = 0.075;
        #endregion
        #endregion

        #region Fields
        private Game myGame;
        private BuyScreenManager myManager;

        // Graphic stuff
        private Texture2D texture_Layer;
        private Vector2 position_Layer;
        private Vector2 scale_Layer;
        private Vector2 center_Screen;
        private Texture2D[] texture_Screenshots;
        private Vector2[] center_Screenshots;
        private int currentScreenshot;
        private BuyScreenState currentState;
        private Vector2 position_Screenshot;
        private Color color_Layer;
        private Texture2D[] texture_Buttons;
        private int[] possibleBackgrounds;
        private int currentBackground;
        private Rectangle buyScreenWindow;
        private float percentage;
        private Texture2D texture_Logo;
        private Texture2D[] texture_LogoBorder;
        private Vector2 center_Logo;
        private Vector2 position_Logo;
        private float scale_Logo;
        private int currentLogoFrame;

        // Text
        private TeletypeEntry[] teletype_Features;
        private Vector2 position_ButtonStart;
        private SpriteFont font_Features;
        private SpriteFont font_Buttons;
        private Vector2 size_Buy;
        private Vector2 size_Menu;
        private Vector2[] offset_Shadow;
        private Color color_Shadow;

        // Timers
        private double delta;
        private double timer_Background;
        private double timer_ScreenshotState;
        private double timer_LogoAnimation;

        // Input
        private Dictionary<Buttons, int> buttonLookup;
        private Buttons button_Buy;
        private Buttons button_Menu;
        #endregion

        #region Constructor / Init
        public BuyScreen(Game game, BuyScreenManager manager) 
            : base(game)
        {
            myGame = game;
            myManager = manager;

            possibleBackgrounds = new int[]
            {
                0, 2, 3
            };

            currentScreenshot = 0;
            currentBackground = 0;

            color_Layer = new Color(
                Color.Gray, alpha_Layer);

            buttonLookup = new Dictionary<Buttons, int>(4);

            offset_Shadow = new Vector2[]
            {
                Vector2.One,
                -Vector2.One,
                shadowLength * Vector2.One,
            };

            color_Shadow = new Color(
                0.12f, 0.12f, 0.12f, 0.87f);
                
                //new Color(Color.Black, 0.87f);
        }

        protected override void LoadContent()
        {
            center_Screen = Vector2.Zero;

            texture_Screenshots = null;

            base.LoadContent();
        }

        /// <summary>
        /// Only call if we really are in trial mode, otherwise, fuck it, why load all this data?
        /// </summary>
        public void LoadScreenshots()
        {
            texture_Layer = InstanceManager.AssetManager.LoadTexture2D(filename_BackgroundLayer);
            position_Layer = Vector2.Zero;

            texture_Screenshots = new Texture2D[numberOfScreens];
            center_Screenshots = new Vector2[numberOfScreens];
            Vector2 maxSize = Vector2.Zero;
            for (int i = 0; i < numberOfScreens; i++)
            {
                texture_Screenshots[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Screens, (i + 1).ToString()));
                center_Screenshots[i] = new Vector2(
                    texture_Screenshots[i].Width / 2f, texture_Screenshots[i].Height / 2f);
                if (texture_Screenshots[i].Width > maxSize.X)
                {
                    maxSize.X = texture_Screenshots[i].Width;
                    maxSize.Y = texture_Screenshots[i].Height;
                }
            }

            center_Screen = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f, InstanceManager.DefaultViewport.Height / 2f);

            scale_Layer = new Vector2(
                (float)InstanceManager.DefaultViewport.Width / (float)texture_Layer.Width,
                (float)InstanceManager.DefaultViewport.Height / (float)texture_Layer.Height);

            texture_Buttons = new Texture2D[]
            {
                InstanceManager.AssetManager.LoadTexture2D(filename_ButtonA),
                InstanceManager.AssetManager.LoadTexture2D(filename_ButtonB),
                InstanceManager.AssetManager.LoadTexture2D(filename_ButtonX),
                InstanceManager.AssetManager.LoadTexture2D(filename_ButtonY),
            };

            buttonLookup.Add(Buttons.A, 0);
            buttonLookup.Add(Buttons.B, 1);
            buttonLookup.Add(Buttons.X, 2);
            buttonLookup.Add(Buttons.Y, 3);

            font_Buttons = InstanceManager.AssetManager.LoadSpriteFont(filename_ButtonFont);
            font_Features = InstanceManager.AssetManager.LoadSpriteFont(filename_FeatureFont);

            // buy screen window stuff
            buyScreenWindow = new Rectangle(
                (int)(center_Screen.X - screenWidth / 2f),
                (int)(center_Screen.Y - screenHeight / 2f),
                (int)screenWidth, (int)screenHeight);

            // Screenshot
            position_Screenshot = new Vector2(
                buyScreenWindow.Right - maxSize.X/2f,
                buyScreenWindow.Top + maxSize.Y/2f);

            // Logo stuff
            texture_Logo = InstanceManager.AssetManager.LoadTexture2D(filename_LogoBase);
            texture_LogoBorder = new Texture2D[numberOfBorderFrames];
            for (int i = 0; i < numberOfBorderFrames; i++)
            {
                texture_LogoBorder[i] = InstanceManager.AssetManager.LoadTexture2D(String.Format(
                   filename_LogoBorder, i.ToString()));
            }

            center_Logo = Vector2.Zero;
            position_Logo = new Vector2(
                buyScreenWindow.X, buyScreenWindow.Y);
            scale_Logo = (buyScreenWindow.Width - maxSize.X * maxSize_Steady) / (float)texture_Logo.Width;

            // Teletype stuff
            Vector2 pos = new Vector2(buyScreenWindow.X, buyScreenWindow.Y);

            pos.Y += texture_Logo.Height * scale_Logo + spacing_Title;

            string[] temp = new string[]
            {
                Resources.BuyScreen_Feature1,
                Resources.BuyScreen_Feature2,
                Resources.BuyScreen_Feature3,
                Resources.BuyScreen_Feature4,
                Resources.BuyScreen_Feature5,
                Resources.BuyScreen_Feature6,
                Resources.BuyScreen_Feature7,
                Resources.BuyScreen_Feature8,
            };
            teletype_Features = new TeletypeEntry[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                teletype_Features[i] = new TeletypeEntry(
                    font_Features,
                    temp[i],
                    pos,
                    Vector2.Zero,
                    new Color(255,235,174),
                    totalTime_Type,
                    -1,
                    color_Shadow,
                    offset_Shadow,
                    InstanceManager.RenderSprite);

                pos.Y += font_Features.MeasureString(temp[i]).Y + spacing_Features;
                pos.X += spacing_HorizFeatures;
            }
        }
        #endregion

        #region Private methods
        public static bool CanPlayerBuyGame(PlayerIndex player)
        {
            SignedInGamer gamer = Gamer.SignedInGamers[player];

            // if the player isn't signed in, they can't buy games
            if (gamer == null)
                return false;

            // if the player isn't on LIVE, they can't buy games
            if (!gamer.IsSignedInToLive)
                return false;

            // lastly check to see if the account is allowed to buy games
            return gamer.Privileges.AllowPurchaseContent;
        }
        #endregion

        #region Public methods
        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (Enabled)
            {
                if (!Guide.IsTrialMode)
                {
                    LocalInstanceManager.CurrentGameState = GameState.MainMenuSystem;
                }
                else
                {
                    if (center_Screen == Vector2.Zero)
                    {
                        // Huh, poop.. shouldn't be here, but just load... whatever
                        myManager.LoadForTrialMode();
                    }

                    // Set up screen stuff
                    currentScreenshot = 0;
                    currentState = BuyScreenState.FadeIn;
                    timer_ScreenshotState = 0;
                    timer_LogoAnimation = 0;

                    // Set up buttons
                    if (MWMathHelper.CoinToss())
                    {
                        button_Buy = Buttons.A;
                        button_Menu = Buttons.B;
                    }
                    else
                    {
                        button_Buy = Buttons.B;
                        button_Menu = Buttons.A;
                    }

                    size_Buy = font_Buttons.MeasureString(Resources.BuyScreen_Buy);
                    size_Menu = font_Buttons.MeasureString(Resources.BuyScreen_Menu);

                    position_ButtonStart = new Vector2(
                            InstanceManager.DefaultViewport.TitleSafeArea.Right -
                            2f * texture_Buttons[0].Width -
                            size_Buy.X -
                            size_Menu.X -
                            spacing_Buttons,
                            InstanceManager.DefaultViewport.TitleSafeArea.Bottom -
                            MathHelper.Max(size_Menu.Y,
                                MathHelper.Max(texture_Buttons[0].Height, size_Buy.Y))
                         );

                    // Set up background
                    LocalInstanceManager.Background.SetParallaxElement(
                        LocalInstanceManager.Background.EmptyParallaxElement, false);
                    LocalInstanceManager.Background.SetParallaxElement(
                        LocalInstanceManager.Background.EmptyParallaxElement, true);
                    timer_Background = totalTime_BackgroundCycle;

                    // Set up teletypes
                    for (int i = 0; i < teletype_Features.Length; i++)
                    {
                        teletype_Features[i].Reset();
                        ServiceLocator.GetService<Teletype>().AddEntry(teletype_Features[i]);
                    }
                }
            }
            else
            {
                ServiceLocator.GetService<Teletype>().FlushEntries();
            }
            base.OnEnabledChanged(sender, args);
        }
        #endregion

        #region Draw / Update
        public override void Update(GameTime gameTime)
        {
            if (Guide.IsTrialMode)
            {
                delta = gameTime.ElapsedGameTime.TotalSeconds;

                // background
                timer_Background += delta;
                if (timer_Background > totalTime_BackgroundCycle)
                {
                    timer_Background = 0;
                    currentBackground++;
                    if (currentBackground >= possibleBackgrounds.Length)
                    {
                        currentBackground = 0;
                    }
                    LocalInstanceManager.Background.SetBackground(
                        possibleBackgrounds[currentBackground]);
                }

                // Logo
                timer_LogoAnimation += delta;
                if (timer_LogoAnimation > totalTime_LogoFrame)
                {
                    timer_LogoAnimation = 0;
                    currentLogoFrame++;
                    if (currentLogoFrame >= numberOfBorderFrames)
                        currentLogoFrame = 0;
                }

                // screenshot
                timer_ScreenshotState += delta;
                switch (currentState)
                {
                    case BuyScreenState.FadeIn:
                        if (timer_ScreenshotState > totalTime_FadeIn)
                        {
                            currentState = BuyScreenState.Steady;
                            timer_ScreenshotState = 0;
                        }
                        break;
                    case BuyScreenState.FadeOut:
                        if (timer_ScreenshotState > totalTime_FadeOut)
                        {
                            currentState = BuyScreenState.Wait;
                            timer_ScreenshotState = 0;
                        }
                        break;
                    case BuyScreenState.Steady:
                        if (timer_ScreenshotState > totalTime_Steady)
                        {
                            currentState = BuyScreenState.FadeOut;
                            timer_ScreenshotState = 0;
                        }
                        break;
                    default:
                        // Wait
                        if (timer_ScreenshotState > totalTime_Wait)
                        {
                            currentState = BuyScreenState.FadeIn;
                            timer_ScreenshotState = 0;
                            currentScreenshot += MWMathHelper.GetRandomInRange(1, 4);
                            if (currentScreenshot >= texture_Screenshots.Length)
                                currentScreenshot -= texture_Screenshots.Length;
                        }
                        break;
                }

                // Input handling
                if (InstanceManager.InputManager.NewButtonPressed(button_Menu))
                    LocalInstanceManager.CurrentGameState = LocalInstanceManager.NextGameState;
                else {
                    for (int i = 0; i < InputManager.MaxInputs; i++)
                    {
                        if(InstanceManager.InputManager.NewButtonPressed(button_Buy, (PlayerIndex)i))
                        {
                            if(CanPlayerBuyGame((PlayerIndex)i))
                            {
                                Guide.ShowMarketplace((PlayerIndex)i);
                                break;
                            }
                            else
                            {
                                // Sign in, buddy
                                Guide.ShowSignIn(1, false);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                LocalInstanceManager.CurrentGameState = LocalInstanceManager.NextGameState;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw layer
            InstanceManager.RenderSprite.Draw(
                texture_Layer,
                Vector2.Zero,
                position_Layer,
                null,
                color_Layer,
                0f,
                scale_Layer,
                0f);

            // Draw screenshot
            switch (currentState)
            {
                case BuyScreenState.FadeIn:
                    percentage = (float)(timer_ScreenshotState / totalTime_FadeIn);
                    DrawShotOffset(
                        percentage,
                        MathHelper.Lerp(minSize_FadeIn, maxSize_FadeIn, percentage));
                    break;
                case BuyScreenState.FadeOut:
                    percentage = 1f - (float)(timer_ScreenshotState / totalTime_FadeOut);
                    DrawShotOffset(
                        percentage,
                        MathHelper.Lerp(maxSize_FadeOut, minSize_FadeOut, percentage));
                    break;
                case BuyScreenState.Steady:
                    InstanceManager.RenderSprite.Draw(
                        texture_Screenshots[currentScreenshot],
                        position_Screenshot,
                        center_Screenshots[currentScreenshot],
                        null,
                        Color.White,
                        0f,
                        MathHelper.Lerp(minSize_Steady, maxSize_Steady, (float)(timer_ScreenshotState / totalTime_Steady)),
                        0f);
                    break;
                default:
                    // wait, do nothing
                    break;
            }

            // Draw Logo
            InstanceManager.RenderSprite.Draw(
                texture_Logo,
                position_Logo,
                center_Logo,
                null,
                new Color(Color.Yellow, 0.09f),
                0f,
                scale_Logo,
                0f,
                RenderSpriteBlendMode.Addititive);
            InstanceManager.RenderSprite.Draw(
                texture_LogoBorder[currentLogoFrame],
                position_Logo,
                center_Logo,
                null,
                Color.White,
                0f,
                scale_Logo,
                0f,
                RenderSpriteBlendMode.Addititive);
            InstanceManager.RenderSprite.Draw(
                texture_Logo,
                position_Logo,
                center_Logo,
                null,
                Color.White,
                0f,
                scale_Logo,
                0f,
                RenderSpriteBlendMode.Multiplicative);
            // Draw buttons
            InstanceManager.RenderSprite.Draw(
                texture_Buttons[buttonLookup[button_Buy]],
                position_ButtonStart,
                Vector2.Zero,
                null,
                Color.White,
                0f,
                1f,
                0f);

            InstanceManager.RenderSprite.DrawString(
                font_Buttons,
                Resources.BuyScreen_Buy,
                position_ButtonStart + texture_Buttons[buttonLookup[button_Buy]].Width * Vector2.UnitX + buttonFontOffset * Vector2.UnitY,
                Color.White);

            InstanceManager.RenderSprite.Draw(
                texture_Buttons[buttonLookup[button_Menu]],
                position_ButtonStart + (texture_Buttons[buttonLookup[button_Buy]].Width + size_Buy.X + spacing_Buttons) * Vector2.UnitX,
                Vector2.Zero,
                null,
                Color.White,
                0f,
                1f,
                0f);

            InstanceManager.RenderSprite.DrawString(
                font_Buttons,
                Resources.BuyScreen_Menu,
                position_ButtonStart + (texture_Buttons[buttonLookup[button_Buy]].Width + size_Buy.X + spacing_Buttons + texture_Buttons[buttonLookup[button_Menu]].Width) * Vector2.UnitX + buttonFontOffset * Vector2.UnitY,
                Color.White);

            base.Draw(gameTime);
        }

        private void DrawShotOffset(float percentage, float size)
        {
            Color tempColor = new Color(
                Color.White, percentage * 0.25f);

            for (int i = 0; i < numOfBlurredScreens; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    texture_Screenshots[currentScreenshot],
                    position_Screenshot + MathHelper.Lerp(maxOffsetScreenshot, 0, percentage) *
                        MWMathHelper.RotateVectorByRadians(
                            Vector2.UnitX, MathHelper.Lerp(-MathHelper.Pi, MathHelper.Pi, (float)(i) / (float)(numOfBlurredScreens))),
                    center_Screenshots[currentScreenshot],
                    null,
                    tempColor,
                    0f,
                    size,
                    0f);
            }
        }
        #endregion
    }
}
