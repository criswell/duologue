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

        private const string filename_TitleFont = "Fonts/bloj-36";
        private const string filename_FeatureFont = "Fonts/deja-med";
        private const string filename_ButtonFont = "Fonts/deja-med";

        private const string filename_ButtonA = "PlayerUI/buttonA";
        private const string filename_ButtonB = "PlayerUI/buttonB";
        private const string filename_ButtonX = "PlayerUI/buttonX";
        private const string filename_ButtonY = "PlayerUI/buttonY";

        //private const float delta_LayerOffset = 0.32416f;

        private const float alpha_Layer = 1f;//0.65f;

        private const float spacing_Buttons = 15f;

        #region Timers
        private const double totalTime_BackgroundCycle = 4.3;
        #endregion
        #endregion

        #region Fields
        private Game myGame;
        private BuyScreenManager myManager;

        // Graphic stuff
        //private Texture2D texture_Background;
        private Texture2D texture_Layer;
        //private Vector2 center_Background;
        private Vector2 position_Layer;
        private Vector2 scale_Layer;
        private Vector2 center_Screen;
        private Texture2D[] texture_Screenshots;
        private Vector2[] center_Screenshots;
        private int currentScreenshot;
        //private Vector2[] possibleSpeeds;
        //private int currentSpeed;
        private Color color_Layer;
        private Texture2D[] texture_Buttons;
        private int[] possibleBackgrounds;
        private int currentBackground;

        // Text
        private TeletypeEntry teletype_Title;
        private TeletypeEntry[] teletype_Features;
        private Vector2 position_ButtonStart;
        private SpriteFont font_Title;
        private SpriteFont font_Features;
        private SpriteFont font_Buttons;
        private Vector2 size_Buy;
        private Vector2 size_Menu;

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

            currentScreenshot = 0;

            color_Layer = new Color(
                Color.Gray, alpha_Layer);

            buttonLookup = new Dictionary<Buttons, int>(4);
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
            for (int i = 0; i < numberOfScreens; i++)
            {
                texture_Screenshots[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Screens, (i + 1).ToString()));
                center_Screenshots[i] = new Vector2(
                    texture_Screenshots[i].Width / 2f, texture_Screenshots[i].Height / 2f);
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
            font_Title = InstanceManager.AssetManager.LoadSpriteFont(filename_TitleFont);

        }
        #endregion

        #region Private methods
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

                    // Set up screenshots
                    /*for (int i = 0; i < texture_Screenshots.Length; i++)
                    {
                        texture_Screenshots[i].Position = new Vector2(
                            (float)MWMathHelper.GetRandomInRange(
                                InstanceManager.DefaultViewport.TitleSafeArea.Left,
                                InstanceManager.DefaultViewport.TitleSafeArea.Right),
                            (float)MWMathHelper.GetRandomInRange(
                                InstanceManager.DefaultViewport.TitleSafeArea.Top,
                                InstanceManager.DefaultViewport.TitleSafeArea.Bottom));
                        texture_Screenshots[i].Speed = possibleSpeeds[currentSpeed];
                        currentSpeed++;
                        if (currentSpeed >= possibleSpeeds.Length)
                            currentSpeed = 0;
                    }*/

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
                            spacing_Buttons
                        ,
                            InstanceManager.DefaultViewport.TitleSafeArea.Bottom -
                            MathHelper.Max(size_Menu.Y,
                                MathHelper.Max(texture_Buttons[0].Height, size_Buy.Y))
                         );

                }
            }
            base.OnEnabledChanged(sender, args);
        }
        #endregion

        #region Draw / Update
        public override void Update(GameTime gameTime)
        {
            if (Guide.IsTrialMode)
            {
                /*for (int i = 0; i < texture_Screenshots.Length; i++)
                {
                    texture_Screenshots[i].Position += texture_Screenshots[i].Speed;
                    if (texture_Screenshots[i].Position.X < -texture_Screenshots[i].Center.X ||
                        texture_Screenshots[i].Position.X > texture_Screenshots[i].Position.X + InstanceManager.DefaultViewport.Width)
                    {
                        texture_Screenshots[i].Position = new Vector2(
                            texture_Screenshots[i].Center.X + InstanceManager.DefaultViewport.Width,
                            (float)MWMathHelper.GetRandomInRange(
                                    InstanceManager.DefaultViewport.TitleSafeArea.Top,
                                    InstanceManager.DefaultViewport.TitleSafeArea.Bottom));
                        texture_Screenshots[i].Speed = possibleSpeeds[currentSpeed];
                        currentSpeed++;
                        if (currentSpeed >= possibleSpeeds.Length)
                            currentSpeed = 0;
                    }
                }*/

                /*position_Layer.X += delta_LayerOffset;
                if (position_Layer.X > texture_Layer.Width)
                    position_Layer.X = 0;
                else if (position_Layer.X < 0)
                    position_Layer.X = texture_Layer.Width;*/

                if (InstanceManager.InputManager.NewButtonPressed(Buttons.Back))
                    LocalInstanceManager.CurrentGameState = LocalInstanceManager.NextGameState;

            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw background
            InstanceManager.RenderSprite.Draw(
                texture_Background,
                center_Screen,
                center_Background,
                null,
                Color.White,
                0f,
                1f,
                0f);

            // Draw screenshots
            for (int i = 0; i < texture_Screenshots.Length; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    texture_Screenshots[i].Texture,
                    texture_Screenshots[i].Position,
                    texture_Screenshots[i].Center,
                    null,
                    Color.White,
                    0f,
                    1f,
                    0f);
            }

            // Draw layers
            InstanceManager.RenderSprite.Draw(
                texture_Layer,
                Vector2.Zero,
                position_Layer,
                null,
                color_Layer,
                0f,
                scale_Layer,
                0f);

            // Draw dialog

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
                position_ButtonStart + texture_Buttons[buttonLookup[button_Buy]].Width * Vector2.UnitX,
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
                position_ButtonStart + (texture_Buttons[buttonLookup[button_Buy]].Width + size_Buy.X + spacing_Buttons + texture_Buttons[buttonLookup[button_Menu]].Width) * Vector2.UnitX,
                Color.White);

            base.Draw(gameTime);
        }
        #endregion
    }
}
