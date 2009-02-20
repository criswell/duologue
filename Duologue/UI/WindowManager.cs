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
    public class WindowManager
    {
        #region Constants
        private const string filename_bottom = "PlayerUI/ui-widget-bottom";
        private const string filename_left = "PlayerUI/ui-widget-left";
        private const string filename_right = "PlayerUI/ui-widget-right";
        private const string filename_top = "PlayerUI/ui-widget-top";

        private const string filename_bottomleft = "PlayerUI/ui-widget-corner-bottomleft";
        private const string filename_bottomright = "PlayerUI/ui-widget-corner-bottomright";
        private const string filename_topleft = "PlayerUI/ui-widget-corner-topleft";
        private const string filename_topright = "PlayerUI/ui-widget-corner-topright";

        private const string filename_text = "PlayerUI/ui-widget-text-{0}";

        private const string filename_screen = "PlayerUI/ui-screen";

        private const int numberOfTextFrames = 2;

        private const int lowerFrameOffsetY = 3;
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

        private float topScaleNum;
        private float sideScaleNum;
        private float bottomScaleNum;

        private Vector2 topScale;
        private Vector2 sideScale;
        private Vector2 bottomScale;

        private int currentFrame;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public WindowManager()
        {
            position = Vector2.Zero;
            windowSize = Vector2.Zero;
            screenRect = Rectangle.Empty;
            currentFrame = 0;
        }

        public void LoadContents()
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

            minimumSize = new Vector2(
                cornerBottomLeft.Width + cornerBottomRight.Width,
                cornerTopLeft.Height + cornerBottomLeft.Height);
        }
        #endregion

        #region Private Methods
        private void DrawBorderItem(Texture2D text, Vector2 pos, Vector2 scale)
        {
            InstanceManager.RenderSprite.Draw(
                text,
                pos,
                Vector2.Zero,
                null,
                Color.White,
                0f,
                scale,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Set the location of the window
        /// </summary>
        /// <param name="rect">Rectangle containing information on the window size and position</param>
        public void SetLocation(Rectangle rect)
        {
            SetLocation(
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.Width, rect.Height));
        }

        /// <summary>
        /// Set the location of the window
        /// </summary>
        /// <param name="pos">Vector of the position of the window</param>
        /// <param name="size">Vector of the size of the window</param>
        public void SetLocation(Vector2 pos, Vector2 size)
        {
            if (size.X >= minimumSize.X && size.Y >= minimumSize.Y)
            {
                position = pos;
                windowSize = size;

                screenRect = new Rectangle(
                    0, 0,
                    (int)(windowSize.X - screenOffset.X - screenSizeOffset.X),
                    (int)(windowSize.Y - screenOffset.Y - screenSizeOffset.Y));

                // Compute the scales
                topScaleNum =
                    (float)(windowSize.X - cornerTopLeft.Width - cornerTopRight.Width) /
                    (float)frameTop.Width;

                // The right and left scales will be the same, and we assume that
                // the size of the window ends with the start of the bottom texture
                sideScaleNum =
                    (float)(windowSize.Y - cornerTopLeft.Height) /
                    (float)frameLeft.Height;

                bottomScaleNum =
                    (float)(windowSize.X - cornerBottomLeft.Width - cornerBottomRight.Width) /
                    (float)frameBottom.Width;

                topScale = new Vector2(topScaleNum, 1f);
                sideScale = new Vector2(1f, sideScaleNum);
                bottomScale = new Vector2(bottomScaleNum, 1f);
            }
            else
            {
                windowSize = Vector2.Zero;
            }
        }
        #endregion

        #region Draw/Update
        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
            if (windowSize != Vector2.Zero)
            {
                Vector2 currentOffset = Vector2.Zero;

                // Start with the screen overlay
                InstanceManager.RenderSprite.Draw(
                    screenOverlay,
                    position + screenOffset,
                    Vector2.Zero,
                    screenRect,
                    Color.White,
                    0f,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);

                // Next do the top bar
                DrawBorderItem(cornerTopLeft, position, Vector2.One);

                currentOffset.X += cornerTopLeft.Width;

                DrawBorderItem(frameTop, position + currentOffset, topScale);

                currentOffset.X = windowSize.X - cornerTopRight.Width;

                DrawBorderItem(cornerTopRight, position + currentOffset, Vector2.One);

                // Next do the left and right bars
                currentOffset = Vector2.Zero;
                currentOffset.Y += cornerTopLeft.Height;

                DrawBorderItem(frameLeft, position + currentOffset, sideScale);

                currentOffset.X = windowSize.X - frameRight.Width;

                DrawBorderItem(frameRight, position + currentOffset, sideScale);

                // Next do the bottom bar
                currentOffset = Vector2.Zero;
                currentOffset.Y = windowSize.Y;

                DrawBorderItem(cornerBottomLeft, position + currentOffset, Vector2.One);

                currentOffset.Y += lowerFrameOffsetY;
                currentOffset.X += cornerBottomLeft.Width;

                DrawBorderItem(frameBottom, position + currentOffset, bottomScale);

                currentOffset.Y = windowSize.Y;
                currentOffset.X = windowSize.X - cornerBottomRight.Width;

                DrawBorderItem(cornerBottomRight, position + currentOffset, Vector2.One);

                // Finally do the text
                DrawBorderItem(textFrames[currentFrame], position + currentOffset, Vector2.One);
            }
        }
        #endregion
    }
}
