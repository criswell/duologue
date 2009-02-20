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
using Mimicware;
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
    class WindowManager
    {
        #region Constants
        private const string filename_bottom = "PlayerUI/ui-widget-bottom";
        private const string filename_left = "PlayerUI/ui-widget-left";
        private const string filename_right = "PlayerUI/ui-widget-right";
        private const string filename_top = "PlayerUI/ui-widget-top";

        private const string filename_bottomleft = "PlayerUI/ui-widget-bottomleft";
        private const string filename_bottomright = "PlayerUI/ui-widget-bottomright";
        private const string filename_topleft = "PlayerUI/ui-widget-topleft";
        private const string filename_topright = "PlayerUI/ui-widget-topright";

        private const string filename_text = "PlayerUI/ui-widget-text-{0}";

        private const string filename_screen = "PlayerUI/ui-screen";

        private const int numberOfTextFrames = 2;
        #endregion

        #region Fields
        private Texture2D frameBottom;
        private Texture2D frameLeft;
        private Texture2D frameRight;
        private Texture2D frameTop;

        private Texture2D cornerBottomLeft;
        private Texture2D cornerBottomRight;
        private Texture2D cornerTopLeft;
        private Texture2D cornerTopRight;

        private Texture2D screenOverlay;

        private Texture2D[] textFrames;

        private Vector2 position;
        private Vector2 windowSize;

        private Rectangle screenRect;
        private Vector2 screenOffset;
        private Vector2 screenSizeOffset;

        private Vector2 minimumSize;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public WindowManager()
        {
            LoadContents();

            position = Vector2.Zero;
            windowSize = Vector2.Zero;
            screenRect = Rectangle.Empty;
        }

        private void LoadContents()
        {
            frameBottom = InstanceManager.AssetManager.LoadTexture2D(filename_bottom);
            frameLeft = InstanceManager.AssetManager.LoadTexture2D(filename_left);
            frameRight = InstanceManager.AssetManager.LoadTexture2D(filename_right);
            frameTop = InstanceManager.AssetManager.LoadTexture2D(filename_top);

            cornerBottomLeft = InstanceManager.AssetManager.LoadTexture2D(filename_bottomleft);
            cornerBottomRight = InstanceManager.AssetManager.LoadTexture2D(filename_bottomright);
            cornerTopLeft = InstanceManager.AssetManager.LoadTexture2D(filename_topleft);
            cornerTopRight = InstanceManager.AssetManager.LoadTexture2D(filename_topright);

            screenOverlay = InstanceManager.AssetManager.LoadTexture2D(filename_screen);

            textFrames = new Texture2D[numberOfTextFrames];
            for (int i = 0; i < numberOfTextFrames; i++)
            {
                textFrames[i] = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_text, i.ToString()));
            }

            screenOffset = new Vector2(
                cornerTopLeft.Width / 2f,
                cornerTopLeft.Height / 2f);

            screenSizeOffset = new Vector2(
                cornerBottomLeft.Width / 2f,
                cornerBottomLeft.Height / 2f);
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        #endregion
    }
}
