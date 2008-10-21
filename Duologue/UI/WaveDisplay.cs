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

namespace Duologue.UI
{
    public enum TextTransitionType
    {
        ZoomIn=0,
        ZoomOut,
        Fade,
        MaxNum,
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class WaveDisplay : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string fontFilename = "Content/inero-50";
        private const float fadeLifetime = 1f;
        #endregion

        #region Fields
        private SpriteFont font;
        private Game localGame;
        private string[] theText;
        private float timeSinceStart;
        private TextTransitionType currentType;
        private Random rand;
        #endregion

        #region Properties
        /// <summary>
        /// The text
        /// </summary>
        public string[] Text
        {
            get
            {
                return theText;
            }
            set
            {
                theText = Text;
                timeSinceStart = 0f;
                GetNewTransitionType();
            }
        }

        /// <summary>
        /// Tells the percente complete in the fade in
        /// </summary>
        public float PercentComplete
        {
            get { return Math.Min(timeSinceStart / fadeLifetime, 1f); }
        }
        #endregion

        #region Constructor / Init / Load
        public WaveDisplay(Game game)
            : base(game)
        {
            localGame = game;
            rand = new Random();
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
            font = InstanceManager.AssetManager.LoadSpriteFont(fontFilename);
            base.LoadContent();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Get a new transition type
        /// </summary>
        private void GetNewTransitionType()
        {
            currentType = (TextTransitionType)rand.Next((int)TextTransitionType.MaxNum);
        }
        #endregion

        #region Public methods
        #endregion

        #region Update / Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (timeSinceStart < fadeLifetime)
                timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (PercentComplete < 1f)
            {
                switch (currentType)
                {
                    case TextTransitionType.ZoomIn:
                        break;
                    default:
                        // Fade
                        break;
                }
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}