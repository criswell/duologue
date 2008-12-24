#region File Description
#endregion

#region Using statements
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
using Mimicware;
using Mimicware.Graphics;
using Mimicware.Manager;
using Mimicware.Debug;
#endregion

namespace Mimicware.Manager
{
    static class InstanceManager
    {
        #region Constants
        /// <summary>
        /// The percentage of the screen we consider titlesafe
        /// </summary>
        public const float TitleSafePercent = 0.90f;
        #endregion

        #region Fields
        #endregion

        #region Properties
        /// <summary>
        /// The game-wide asset manager
        /// </summary>
        public static AssetManager AssetManager;

        /// <summary>
        /// The game-wide render sprite
        /// </summary>
        public static RenderSprite RenderSprite;

        /// <summary>
        /// The game-wide.. erm... game
        /// </summary>
        public static Game Game;

        /// <summary>
        /// The game-wide viewport
        /// </summary>
        public static Viewport DefaultViewport;

        /// <summary>
        /// The game-wide logger instance
        /// </summary>
        public static Logger Logger;

        /// <summary>
        /// The game-wide graphics device
        /// </summary>
        public static GraphicsDevice GraphicsDevice;

        /// <summary>
        /// The game-wide input manager
        /// </summary>
        public static InputManager InputManager;

        /// <summary>
        /// The game-wide random instance
        /// </summary>
        public static Random Random = new Random();

        /// <summary>
        /// The Mimicware defined Title Safe Region
        /// </summary>
        public static Rectangle TitleSafeRegion;
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the Asset Manager game-wide
        /// </summary>
        /// <param name="assetManager"></param>
        public static void SetAssetManager(AssetManager assetManager)
        {
            AssetManager = assetManager;
        }
        /// <summary>
        /// Sets the RenderSprite instance game-wide
        /// </summary>
        /// <param name="renderSprite"></param>
        public static void SetRenderSprite(RenderSprite renderSprite)
        {
            RenderSprite = renderSprite;
        }
        /// <summary>
        /// Sets the Game instance
        /// </summary>
        /// <param name="game"></param>
        public static void SetGame(Game game)
        {
            Game = game;
        }

        /// <summary>
        /// Sets the Game-wide viewport
        /// </summary>
        /// <param name="veiwport"></param>
        public static void SetDefaultViewport(Viewport veiwport)
        {
            DefaultViewport = veiwport;
        }

        /// <summary>
        /// Sets the game-wide logger instance
        /// </summary>
        /// <param name="logger"></param>
        public static void SetLogger(Logger logger)
        {
            Logger = logger;
        }

        public static void ComputeTitleSafe()
        {
            
        }
        #endregion
    }
}
