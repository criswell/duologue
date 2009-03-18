﻿#region File Description
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
#endregion

namespace Duologue.Screens
{
    public enum IntroState
    {
        LogoFadeIn,
        Stablize,
        MottoFadeIn,
        Wait,
        Loading
    }

    public class CompanyIntroScreen : DrawableGameComponent
    {
        #region Constants
        private const string filename_Font = "Fonts/deja-vu-serif-12";
        private const string filename_Logo = "funavision-logo";
        private const string filename_Motto = "funavision-motto";

        private const float spacing_Motto = 15f;
        private const float spacing_Copyright = 200f;

        private const double delta_LogoFadeIn = 0.2;
        private const double delta_Stabilize = 0.01;
        private const double delta_MottoFadeIn = 0.1;
        private const double delta_Wait = 0.5;

        private const float minLogoSize = 0.8f;
        private const float maxLogoSize = 1.1f;
        #endregion

        #region Fields
        private SpriteFont font;
        private Texture2D texture_Logo;
        private Texture2D texture_Motto;
        private Vector2 center_Logo;
        private Vector2 center_Motto;
        private Vector2 center_Copyright;

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
        #endregion

        #region Properties
        #endregion

        #region Constructor / init
        public CompanyIntroScreen(Game game, CompanyIntroScreenManager manager)
            : base(game)
        {
            //myGame = game;
            myManager = manager;
        }

        protected override void LoadContent()
        {
            font = InstanceManager.Localization.GetLocalizedFont(filename_Font);
            texture_Logo = InstanceManager.AssetManager.LoadTexture2D(filename_Logo);
            texture_Motto = InstanceManager.Localization.GetLocalizedTexture(filename_Motto);

            center_Logo = new Vector2(
                texture_Logo.Width / 2f, texture_Logo.Height / 2f);
            center_Motto = new Vector2(
                texture_Motto.Width / 2f, texture_Motto.Height / 2f);

            Vector2 copyrightSize = font.MeasureString(Resources.Intro_Copyright);
            center_Copyright = new Vector2(
                copyrightSize.X / 2f, copyrightSize.Y / 2f);

            totalHeight = (float)texture_Logo.Height + spacing_Motto +
                (float)texture_Motto.Height + spacing_Copyright + (float)copyrightSize.Y;

            myState = IntroState.LogoFadeIn;
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
        }


        private void LoadData(double timeSinceSwitch)
        {
            throw new NotImplementedException();
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
                minLogoSize - deltaSize * (float)(timeSinceSwitch / delta_Stabilize),
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
        #endregion

        #region Draw/Update
        public override void Update(GameTime gameTime)
        {
            timeSinceSwitch += gameTime.ElapsedGameTime.TotalSeconds;

            switch(myState)
            {
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
                        myState = IntroState.Loading;
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
