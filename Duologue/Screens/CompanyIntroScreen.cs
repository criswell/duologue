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
using Duologue.PlayObjects;
#endregion

namespace Duologue.Screens
{
    public enum IntroState
    {
        Black,
        ScreenFadeIn,
        LogoFadeIn,
        Stablize,
        MottoFadeIn,
        Wait,
        Loading,
        FadeOut,
        Blank
    }

    public class CompanyIntroScreen : DrawableGameComponent
    {
        #region Constants
        private const string filename_Font = "Fonts/deja-vu-sans-small";
        private const string filename_Logo = "funavision-logo";
        private const string filename_Motto = "funavision-motto";
        private const string filename_LoadingFont = "Fonts\\inero-28";
        private const string filename_Blank = "Mimicware/blank";

        private const float spacing_Motto = 15f;
        private const float spacing_Copyright = 200f;

        private const double delta_LogoFadeIn = 1.0;
        private const double delta_Stabilize = 0.5;
        private const double delta_MottoFadeIn = 0.8;
        private const double delta_Wait = 1.5;
        private const double delta_ScreenBlack = 0.5;
        private const double delta_ScreenFadeIn = 1.0;
        private const double delta_ScreenFadeOut = 1.0;
        private const double delta_Blank = 0.5;

        private const double maxDelta = 99;

        private const float minLogoSize = 0.8f;
        private const float maxLogoSize = 1.1f;

        private const float loadingPadding = 15f;
        #endregion

        #region Fields
        private SpriteFont font;
        private SpriteFont loadingFont;
        private Texture2D texture_Logo;
        private Texture2D texture_Motto;
        private Texture2D texture_Blank;
        private Vector2 center_Logo;
        private Vector2 center_Motto;
        private Vector2 center_Copyright;
        private Vector2 scale_Blank;

        private Vector2 position_Logo;
        private Vector2 position_Motto;
        private Vector2 position_Copyright;

        private Color textColor;

        //private Game myGame;
        private CompanyIntroScreenManager myManager;
        private IntroState myState;
        private double timeSinceSwitch;
        private float totalHeight;

        private float deltaSize;

        // Loading screen stuff
        private TypesOfPlayObjects[] playObjects;
        private int currentPlayObjectIndex;
        private PlayObject currentPlayObject;
        private String[] currentFilenames;
        private int currentFilenameIndex;
        private Vector2 position_Loading;
        private Vector2 loadingSize;

        // Pre-cached texture
        private Texture2D tempTexture;
        #endregion

        #region Properties
        #endregion

        #region Constructor / init
        public CompanyIntroScreen(Game game, CompanyIntroScreenManager manager)
            : base(game)
        {
            //myGame = game;
            myManager = manager;

            // Set up the play objects
            playObjects = new TypesOfPlayObjects[]
            {
                TypesOfPlayObjects.Enemy_Buzzsaw,
                TypesOfPlayObjects.Enemy_Ember,
                TypesOfPlayObjects.Enemy_Gloop,
                TypesOfPlayObjects.Enemy_KingGloop,
                TypesOfPlayObjects.Enemy_Mirthworm,
                TypesOfPlayObjects.Enemy_Pyre,
                TypesOfPlayObjects.Enemy_Spitter,
                TypesOfPlayObjects.Enemy_StaticGloop,
                TypesOfPlayObjects.Enemy_UncleanRot,
                TypesOfPlayObjects.Enemy_Wiggles,
                TypesOfPlayObjects.Player,
                TypesOfPlayObjects.PlayerBullet
            };

            currentPlayObjectIndex = 0;
        }

        protected override void LoadContent()
        {
            font = InstanceManager.Localization.GetLocalizedFont(filename_Font);
            loadingFont = InstanceManager.Localization.GetLocalizedFont(filename_LoadingFont);
            texture_Logo = InstanceManager.AssetManager.LoadTexture2D(filename_Logo);
            texture_Motto = InstanceManager.Localization.GetLocalizedTexture(filename_Motto);
            texture_Blank = InstanceManager.AssetManager.LoadTexture2D(filename_Blank);

            center_Logo = new Vector2(
                texture_Logo.Width / 2f, texture_Logo.Height / 2f);
            center_Motto = new Vector2(
                texture_Motto.Width / 2f, texture_Motto.Height / 2f);

            Vector2 copyrightSize = font.MeasureString(Resources.Intro_Copyright);
            center_Copyright = new Vector2(
                copyrightSize.X / 2f, copyrightSize.Y / 2f);

            totalHeight = (float)texture_Logo.Height + spacing_Motto +
                (float)texture_Motto.Height + spacing_Copyright + (float)copyrightSize.Y;

            loadingSize = font.MeasureString(Resources.Intro_Loading);
            position_Loading = Vector2.Zero;

            myState = IntroState.Black;
            timeSinceSwitch = 0;

            position_Logo = Vector2.Zero;

            deltaSize = maxLogoSize - minLogoSize;

            textColor = Color.SeaShell;

            base.LoadContent();
        }
        #endregion

        #region Private Methods
        private void SetPositions()
        {
            position_Logo = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f,
                InstanceManager.DefaultViewport.Height / 2f - totalHeight / 2f + center_Logo.Y);

            position_Motto = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f,
                position_Logo.Y + center_Logo.Y + spacing_Motto + center_Motto.Y);

            position_Copyright = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f,
                position_Motto.Y + center_Motto.Y + spacing_Copyright + center_Copyright.Y);

            scale_Blank = new Vector2(
                (float)InstanceManager.DefaultViewport.Width,
                (float)InstanceManager.DefaultViewport.Height);
        }


        private void LoadData(double currentTimer)
        {
            if (currentFilenameIndex < currentFilenames.Length)
            {
                // pre-cache next image
                tempTexture =
                    InstanceManager.AssetManager.LoadTexture2D(currentFilenames[currentFilenameIndex]);
                currentFilenameIndex++;
            }
            else
            {
                if (currentPlayObjectIndex < playObjects.Length)
                {
                    // load up the next play object
                    SetCurrentFilenames();
                }
                else
                {
                    // We're all done
                    VoidSpinner();
                    timeSinceSwitch = 0;
                    myState = IntroState.FadeOut;
                }
            }
        }

        private void SetCurrentFilenames()
        {
            // Get the current play object
            GetCurrentPlayObject();
            currentPlayObjectIndex++;

            // Get the filenames
            currentFilenames = currentPlayObject.GetFilenames();
            currentFilenameIndex = 0;            
        }

        private void GetCurrentPlayObject()
        {
            switch (playObjects[currentPlayObjectIndex])
            {
                case TypesOfPlayObjects.Enemy_Buzzsaw:
                    currentPlayObject = new Enemy_Buzzsaw();
                    break;
                case TypesOfPlayObjects.Enemy_Ember:
                    currentPlayObject = new Enemy_Ember();
                    break;
                case TypesOfPlayObjects.Enemy_Gloop:
                    currentPlayObject = new Enemy_Gloop();
                    break;
                case TypesOfPlayObjects.Enemy_KingGloop:
                    currentPlayObject = new Enemy_GloopKing();
                    break;
                case TypesOfPlayObjects.Enemy_Mirthworm:
                    currentPlayObject = new Enemy_Mirthworm();
                    break;
                case TypesOfPlayObjects.Enemy_Pyre:
                    currentPlayObject = new Enemy_Pyre();
                    break;
                case TypesOfPlayObjects.Enemy_Spitter:
                    currentPlayObject = new Enemy_Spitter();
                    break;
                case TypesOfPlayObjects.Enemy_StaticGloop:
                    currentPlayObject = new Enemy_StaticGloop();
                    break;
                case TypesOfPlayObjects.Enemy_UncleanRot:
                    currentPlayObject = new Enemy_UncleanRot();
                    break;
                case TypesOfPlayObjects.Enemy_Wiggles:
                    currentPlayObject = new Enemy_Wiggles();
                    break;
                case TypesOfPlayObjects.Player:
                    currentPlayObject = new Player();
                    break;
                default:
                    // Player Bullet
                    currentPlayObject = new PlayerBullet();
                    break;
            }
        }

        private void TriggerLoadingSpinner()
        {
            LocalInstanceManager.Spinner.Initialize();

            // Font stuff
            LocalInstanceManager.Spinner.DisplayFont = loadingFont;
            LocalInstanceManager.Spinner.DisplayText = Resources.Intro_Loading;
            LocalInstanceManager.Spinner.FontColor = Color.AntiqueWhite;
            LocalInstanceManager.Spinner.FontShadowColor = Color.DarkGray;

            // Spinner colors
            LocalInstanceManager.Spinner.BaseColor = Color.SlateBlue;
            LocalInstanceManager.Spinner.TrackerColor = Color.SkyBlue;
            
            // Location and scale
            position_Loading = new Vector2(
                (float)InstanceManager.DefaultViewport.TitleSafeArea.Width - loadingSize.X,
                (float)InstanceManager.DefaultViewport.TitleSafeArea.Height -
                    loadingSize.Y - 2f*loadingPadding);
            LocalInstanceManager.Spinner.Position = position_Loading;
            LocalInstanceManager.Spinner.Scale = Vector2.One;

            LocalInstanceManager.Spinner.Enabled = true;
            LocalInstanceManager.Spinner.Visible = true;
        }

        private void VoidSpinner()
        {
            LocalInstanceManager.Spinner.Enabled = false;
            LocalInstanceManager.Spinner.Visible = false;
            LocalInstanceManager.Spinner.Initialize();
        }
        #endregion

        #region Private Draw methods
        private void Draw_LogoFadeIn()
        {
            InstanceManager.RenderSprite.Draw(
                texture_Logo,
                position_Logo,
                center_Logo,
                null,
                new Color(textColor, (float)(timeSinceSwitch / delta_LogoFadeIn)),
                0f,
                minLogoSize + deltaSize * (float)(timeSinceSwitch / delta_LogoFadeIn),
                0f,
                RenderSpriteBlendMode.Addititive);
        }

        private void Draw_Stablize()
        {
            // Draw part fading out
            InstanceManager.RenderSprite.Draw(
                texture_Logo,
                position_Logo,
                center_Logo,
                null,
                new Color(textColor, 1 - (float)(timeSinceSwitch/delta_Stabilize)),
                0f,
                maxLogoSize - deltaSize * (float)(timeSinceSwitch / delta_Stabilize),
                0f,
                RenderSpriteBlendMode.Addititive);

            // Draw part fading in
            InstanceManager.RenderSprite.Draw(
                texture_Logo,
                position_Logo,
                center_Logo,
                null,
                new Color(textColor, (float)(timeSinceSwitch / delta_Stabilize)),
                0f,
                maxLogoSize - deltaSize * (float)(timeSinceSwitch / delta_Stabilize),
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        private void Draw_Logo()
        {
            InstanceManager.RenderSprite.Draw(
                texture_Logo,
                position_Logo,
                center_Logo,
                null,
                textColor,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlend);
        }

        private void Draw_MottoFadeIn()
        {
            InstanceManager.RenderSprite.Draw(
                texture_Motto,
                position_Motto,
                center_Motto,
                null,
                new Color(textColor, (float)(timeSinceSwitch / delta_MottoFadeIn)),
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlend);
        }

        private void Draw_Motto()
        {
            InstanceManager.RenderSprite.Draw(
                texture_Motto,
                position_Motto,
                center_Motto,
                null,
                textColor,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlend);
        }

        private void Draw_Copyright()
        {
            InstanceManager.RenderSprite.DrawString(
                font,
                Resources.Intro_Copyright,
                position_Copyright,
                textColor,
                Vector2.One,
                center_Copyright,
                RenderSpriteBlendMode.AlphaBlend);
        }

        private void Draw_Black()
        {
            InstanceManager.RenderSprite.Draw(
                texture_Blank,
                Vector2.Zero,
                Vector2.Zero,
                null,
                Color.Black,
                0f,
                scale_Blank,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }


        private void Draw_ScreenFadeIn()
        {
            InstanceManager.RenderSprite.Draw(
                texture_Blank,
                Vector2.Zero,
                Vector2.Zero,
                null,
                new Color(Color.Black, 1f - (float)(timeSinceSwitch / delta_ScreenBlack)),
                0f,
                scale_Blank,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        private void Draw_FadeOut()
        {
            InstanceManager.RenderSprite.Draw(
                texture_Logo,
                position_Logo,
                center_Logo,
                null,
                new Color(textColor, 1f-(float)(timeSinceSwitch/delta_ScreenFadeOut)),
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlend);

            InstanceManager.RenderSprite.Draw(
                texture_Motto,
                position_Motto,
                center_Motto,
                null,
                new Color(textColor, 1f - (float)(timeSinceSwitch / delta_ScreenFadeOut)),
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlend);

            InstanceManager.RenderSprite.DrawString(
                font,
                Resources.Intro_Copyright,
                position_Copyright,
                new Color(textColor, 1f - (float)(timeSinceSwitch / delta_ScreenFadeOut)),
                Vector2.One,
                center_Copyright,
                RenderSpriteBlendMode.AlphaBlend);
        }
        #endregion

        #region Draw/Update
        public override void Update(GameTime gameTime)
        {
            timeSinceSwitch += gameTime.ElapsedGameTime.TotalSeconds;

            switch(myState)
            {
                case IntroState.Black:
                    if (timeSinceSwitch > delta_ScreenBlack)
                    {
                        timeSinceSwitch = 0;
                        myState = IntroState.ScreenFadeIn;
                    }
                    break;
                case IntroState.ScreenFadeIn:
                    if (timeSinceSwitch > delta_ScreenFadeIn)
                    {
                        timeSinceSwitch = 0;
                        myState = IntroState.LogoFadeIn;
                    }
                    break;
                case IntroState.LogoFadeIn:
                    if(timeSinceSwitch > delta_LogoFadeIn)
                    {
                        timeSinceSwitch = 0;
                        deltaSize = maxLogoSize - 1f;
                        myState = IntroState.Stablize;
                    }
                    break;
                case IntroState.Stablize:
                    if(timeSinceSwitch > delta_Stabilize)
                    {
                        timeSinceSwitch = 0;
                        myState = IntroState.MottoFadeIn;
                    }
                    break;
                case IntroState.MottoFadeIn:
                    if(timeSinceSwitch > delta_MottoFadeIn)
                    {
                        timeSinceSwitch = 0;
                        myState = IntroState.Wait;
                    }
                    break;
                case IntroState.Wait:
                    if(timeSinceSwitch > delta_Wait)
                    {
                        timeSinceSwitch = 0;
                        // FIXME switch on the loading indicator here
                        currentPlayObjectIndex = 0;
                        SetCurrentFilenames();
                        TriggerLoadingSpinner();
                        myState = IntroState.Loading;
                    }
                    break;
                case IntroState.FadeOut:
                    if (timeSinceSwitch > delta_ScreenFadeOut)
                    {
                        timeSinceSwitch = 0;
                        myState = IntroState.Blank;
                    }
                    break;
                case IntroState.Blank:
                    if (timeSinceSwitch > delta_Blank)
                    {
                        timeSinceSwitch = delta_Blank;
                        myManager.Exit();
                    }
                    break;
                default:
                    // Default is loading
                    LoadData(timeSinceSwitch);
                    break;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (position_Logo == Vector2.Zero)
                SetPositions();

            switch (myState)
            {
                case IntroState.Black:
                    Draw_Black();
                    break;
                case IntroState.ScreenFadeIn:
                    Draw_ScreenFadeIn();
                    break;
                case IntroState.LogoFadeIn:
                    Draw_LogoFadeIn();
                    break;
                case IntroState.Stablize:
                    Draw_Stablize();
                    break;
                case IntroState.MottoFadeIn:
                    Draw_Logo();
                    Draw_MottoFadeIn();
                    break;
                case IntroState.Wait:
                    Draw_Logo();
                    Draw_Motto();
                    Draw_Copyright();
                    break;
                case IntroState.FadeOut:
                    Draw_FadeOut();
                    break;
                case IntroState.Blank:
                    // NADA
                    break;
                default:
                    Draw_Logo();
                    Draw_Motto();
                    Draw_Copyright();
                    // Default is loading
                    break;
            }

            base.Draw(gameTime);
        }
        #endregion
    }
}
