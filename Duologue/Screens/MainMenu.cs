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
        #endregion

        #region Fields
        private SpriteFont font;
        private Vector2 position;
        private List<MenuItem> mainMenuItems;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public MainMenu(Game game)
            : base(game)
        {
            position = Vector2.Zero;
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
            mainMenuItems[3].Selected = true;
            
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
                InstanceManager.RenderSprite.DrawString(font,
                    mi.Text,
                    curPos + mi.FadePercent * selectOffset * Vector2.One,
                    mi.Selected ? Color.Azure : Color.Crimson,
                    RenderSpriteBlendMode.Addititive);
                curPos.Y += font.LineSpacing + extraLineSpacing;
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}