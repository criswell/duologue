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
#endregion

namespace Duologue.Screens
{
    public enum IntroState
    {
        LogoFadeIn,
        Stablize,
        MottoFadein,
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

        private Game myGame;
        private IntroState myState;
        private double timeSinceSwitch;
        private float totalHeight;
        #endregion

        #region Properties
        #endregion

        #region Constructor / init
        public CompanyIntroScreen(Game game)
            : base(game)
        {
            myGame = game;
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
                        myState = IntroState.Stablize;
                    }
                    break;
                case IntroState.Stablize:
                    if(timeSinceSwitch > delta_Stabilize)
                    {
                        timeSinceSwitch = 0;
                        myState = IntroState.MottoFadein;
                    }
                    break;
                case IntroState.MottoFadein:
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

            base.Draw(gameTime);
        }
        #endregion
    }
}
