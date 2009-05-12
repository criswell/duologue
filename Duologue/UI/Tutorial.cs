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
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Tutorial : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string filename_PopUpWindow = "PlayerUI/pop-up-window";
        private const string filename_Font = "Fonts\\inero-28";
        #endregion

        #region Fields
        private Texture2D texture_PopUpWindow;
        private Vector2 center_PopUpWindow;
        private SpriteFont font;
        private TutorialEntry[] theEntries;
        #endregion

        #region Constructor / Init
        public Tutorial(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

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
            }

            base.LoadContent();
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion

        #region Update / Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        #endregion
    }
}