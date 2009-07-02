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
    public struct Screenshots
    {
        public Texture2D Texture;
        public Vector2 Center;
        public Vector2 Position;
        public Vector2 Speed;
    }

    public class BuyScreen : DrawableGameComponent
    {
        #region Constants
        private const string filename_Background = "Medals/medal-case-background";
        private const string filename_BackgroundLayer = "BuyScreen/bkg-layer";
        private const string filename_Screens = "BuyScreen/{0}";
        private const int numberOfScreens = 14;

        private const float delta_LayerOffset = 0.32416f;
        #endregion

        #region Fields
        private Game myGame;
        private BuyScreenManager myManager;

        // Graphic stuff
        private Texture2D texture_Background;
        private Texture2D texture_Layer;
        private Vector2 center_Background;
        private Vector2 position_Layer;
        private Vector2 center_Screen;
        private Screenshots[] screenshots;
        private Vector2[] possibleSpeeds;
        private int currentSpeed;
        #endregion

        #region Constructor / Init
        public BuyScreen(Game game, BuyScreenManager manager) 
            : base(game)
        {
            myGame = game;
            myManager = manager;
            possibleSpeeds = new Vector2[]
            {
                -1.5f * Vector2.UnitX,
                -0.1f * Vector2.UnitX,
                -3.3f * Vector2.UnitX,
                -2.1f * Vector2.UnitX,
                -1.9f * Vector2.UnitX,
                -1.7f * Vector2.UnitX,
                -2.8f * Vector2.UnitX
            };
            currentSpeed = 0;
        }

        protected override void LoadContent()
        {
            center_Screen = Vector2.Zero;

            screenshots = null;

            base.LoadContent();
        }

        /// <summary>
        /// Only call if we really are in trial mode, otherwise, fuck it, why load all this data?
        /// </summary>
        public void LoadScreenshots()
        {
            texture_Background = InstanceManager.AssetManager.LoadTexture2D(filename_Background);
            center_Background = new Vector2(
                texture_Background.Width / 2f, texture_Background.Height / 2f);

            texture_Layer = InstanceManager.AssetManager.LoadTexture2D(filename_BackgroundLayer);
            position_Layer = Vector2.Zero;

            screenshots = new Screenshots[numberOfScreens];
            for (int i = 0; i < numberOfScreens; i++)
            {
                screenshots[i].Texture = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Screens, (i + 1).ToString()));
                screenshots[i].Center = new Vector2(
                    screenshots[i].Texture.Width / 2f, screenshots[i].Texture.Height / 2f);
            }

            center_Screen = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f, InstanceManager.DefaultViewport.Height / 2f);

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
                    for (int i = 0; i < screenshots.Length; i++)
                    {
                        screenshots[i].Position = new Vector2(
                            (float)MWMathHelper.GetRandomInRange(
                                InstanceManager.DefaultViewport.TitleSafeArea.Left,
                                InstanceManager.DefaultViewport.TitleSafeArea.Right),
                            (float)MWMathHelper.GetRandomInRange(
                                InstanceManager.DefaultViewport.TitleSafeArea.Top,
                                InstanceManager.DefaultViewport.TitleSafeArea.Bottom));
                        screenshots[i].Speed = possibleSpeeds[currentSpeed];
                        currentSpeed++;
                        if (currentSpeed >= possibleSpeeds.Length)
                            currentSpeed = 0;
                    }
                }
            }
            base.OnEnabledChanged(sender, args);
        }
        #endregion

        #region Draw / Update
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < screenshots.Length; i++)
            {
                screenshots[i].Position += screenshots[i].Speed;
                if (screenshots[i].Position.X < -screenshots[i].Center.X ||
                    screenshots[i].Position.X > screenshots[i].Position.X + InstanceManager.DefaultViewport.Width)
                {
                    screenshots[i].Position = new Vector2(
                        screenshots[i].Position.X + InstanceManager.DefaultViewport.Width,
                        (float)MWMathHelper.GetRandomInRange(
                                InstanceManager.DefaultViewport.TitleSafeArea.Top,
                                InstanceManager.DefaultViewport.TitleSafeArea.Bottom));
                    screenshots[i].Speed = possibleSpeeds[currentSpeed];
                    currentSpeed++;
                    if (currentSpeed >= possibleSpeeds.Length)
                        currentSpeed = 0;
                }
            }

            position_Layer.X += delta_LayerOffset;
            if (position_Layer.X > texture_Layer.Width)
                position_Layer.X = 0;
            else if (position_Layer.X < 0)
                position_Layer.X = texture_Layer.Width;

            if (InstanceManager.InputManager.NewButtonPressed(Buttons.Back))
                LocalInstanceManager.CurrentGameState = GameState.MainMenuSystem;

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
            for (int i = 0; i < screenshots.Length; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    screenshots[i].Texture,
                    screenshots[i].Position,
                    screenshots[i].Center,
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
                Color.White,
                0f,
                1f,
                0f);
            InstanceManager.RenderSprite.Draw(
                texture_Layer,
                Vector2.UnitX * ((float)texture_Layer.Width - position_Layer.X),
                Vector2.Zero,
                null,
                Color.White,
                0f,
                1f,
                0f);

            // Draw dialog

            base.Draw(gameTime);
        }
        #endregion
    }
}
