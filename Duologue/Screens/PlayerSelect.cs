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
#endregion

namespace Duologue.Screens
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PlayerSelect : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string fontFilename = "Fonts\\inero-28";
        private const string AbuttonFilename = "PlayerUI\\buttonA.png";
        private const string controllerButtonsFilename = "PlayerUI\\controller-buttons";
        private const string controllerFaceFilename = "PlayerUI\\controller-fase";
        private const string playerRootFilename = "PlayerUI\\player-root";
        private const string playerBaseFilename = "PlayerUI\\P{0}-base";
        #endregion

        #region Fields
        // Textures
        private Texture2D aButton;
        private Texture2D controllerButtons;
        private Texture2D controllerFace;
        private Texture2D playerRoot;
        private Texture2D[] playerBases;

        // Fonts
        private SpriteFont font;

        // Player colors
        private PlayerColors playerColors;

        // Positions
        private Vector2 centerOfScreen;
        private Vector2 centerOfPlayer;
        private Vector2 selectOffset;
        private Vector2 centerOfButton;
        #endregion

        #region Properties
        #endregion

        #region Constructor/Init/load
        public PlayerSelect(Game game)
            : base(game)
        {
            playerBases = new Texture2D[InputManager.MaxInputs];
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
            aButton = InstanceManager.AssetManager.LoadTexture2D(AbuttonFilename);
            centerOfButton = new Vector2(
                aButton.Width / 2f,
                aButton.Height / 2f);

            controllerButtons = InstanceManager.AssetManager.LoadTexture2D(controllerButtonsFilename);
            controllerFace = InstanceManager.AssetManager.LoadTexture2D(controllerFaceFilename);
            playerRoot = InstanceManager.AssetManager.LoadTexture2D(playerRootFilename);
            centerOfPlayer = new Vector2(
                playerRoot.Width / 2f,
                playerRoot.Height / 2f);


            base.LoadContent();
        }
        #endregion

        #region Private methods
        #endregion

        #region Public methods
        #endregion

        #region Update /Draw
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