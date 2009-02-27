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
    public class EndCinematicScreen : DrawableGameComponent
    {
        #region Constants
        private const string fontFilename = "Fonts/inero-50";
        #endregion

        #region Fields
        private EndCinematicScreenManager myManager;
        private SpriteFont font;

        private Vector2 pos;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public EndCinematicScreen(Game game, EndCinematicScreenManager manager)
            : base(game)
        {
            myManager = manager;
        }

        protected override void LoadContent()
        {
            font = InstanceManager.AssetManager.LoadSpriteFont(fontFilename);
            pos = new Vector2(400, 400);
            base.LoadContent();
        }
        #endregion

        #region Update / Draw
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            InstanceManager.RenderSprite.DrawString(
                font,
                "Placeholder for cinematics",
                pos,
                Color.Azure);
            base.Draw(gameTime);
        }
        #endregion
    }
}
