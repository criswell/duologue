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
using Duologue.State;
using Duologue.Properties;
using Duologue.Screens;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.UI;
#endregion


namespace Duologue.Screens
{
    public class ColorStateTestComponent : DrawableGameComponent
    {
        #region constants
        private const string filename_add = "PlayerUI/cstest_add";
        private const string filename_alph = "PlayerUI/cstest_alph";
        private const string filename_mult = "PlayerUI/cstest_mult";
        private const string filename_blend = "PlayerUI/cstest_blend";
        private const string filename_font = "Fonts/inero-28";
        #endregion

        #region fields
        private Texture2D add;
        private Texture2D alph;
        private Texture2D mult;
        private Texture2D blend;

        private SpriteFont font;

        private Vector2 startPos;

        private int vertSize;

        private ColorState[] colorStates;

        private int maxNameWidth;
        #endregion

        #region properties
        #endregion

        #region Constructor
        public ColorStateTestComponent(Game game)
            : base(game)
        {
            startPos = -1f * Vector2.One;
        }

        protected override void LoadContent()
        {
            add = InstanceManager.AssetManager.LoadTexture2D(filename_add);
            alph = InstanceManager.AssetManager.LoadTexture2D(filename_alph);
            mult = InstanceManager.AssetManager.LoadTexture2D(filename_mult);
            blend = InstanceManager.AssetManager.LoadTexture2D(filename_blend);
            font = InstanceManager.AssetManager.LoadSpriteFont(filename_font);
            vertSize = blend.Height + 2;
            colorStates = ColorState.GetColorStates();

            // Get the width of the names
            maxNameWidth = 0;
            Vector2 temp;
            for (int i = 0; i < ColorState.numberOfColorStates; i++)
            {
                temp = font.MeasureString(colorStates[i].PositiveName);
                if (temp.X > maxNameWidth)
                    maxNameWidth = (int)temp.X;
                temp = font.MeasureString(colorStates[i].NegativeName);
                if (temp.X > maxNameWidth)
                    maxNameWidth = (int)temp.X;
            }
            base.LoadContent();
        }
        #endregion

        #region Private
        private void DrawDetails(Vector2 pos, string text, Color c)
        {
            int delta = (int)font.MeasureString(text).X;

            InstanceManager.RenderSprite.DrawString(
                font,
                text,
                pos,
                c);

            InstanceManager.RenderSprite.Draw(
                add,
                pos + new Vector2((float)delta, 0f),
                Vector2.Zero,
                null,
                c,
                0f,
                1f,
                1f,
                RenderSpriteBlendMode.Addititive);

            InstanceManager.RenderSprite.Draw(
                blend,
                pos + new Vector2((float)(delta + add.Width), 0f),
                Vector2.Zero,
                null,
                c,
                0f,
                1f,
                1f,
                RenderSpriteBlendMode.Addititive);

            InstanceManager.RenderSprite.Draw(
                alph,
                pos + new Vector2((float)(delta + add.Width + blend.Width), 0f),
                Vector2.Zero,
                null,
                c,
                0f,
                1f,
                1f,
                RenderSpriteBlendMode.AlphaBlend);

            InstanceManager.RenderSprite.Draw(
                blend,
                pos + new Vector2((float)(delta + add.Width + alph.Width + blend.Width), 0f),
                Vector2.Zero,
                null,
                c,
                0f,
                1f,
                1f,
                RenderSpriteBlendMode.AlphaBlend);

            InstanceManager.RenderSprite.Draw(
                mult,
                pos + new Vector2((float)(delta + add.Width + alph.Width + 2 * blend.Width), 0f),
                Vector2.Zero,
                null,
                c,
                0f,
                1f,
                1f,
                RenderSpriteBlendMode.Multiplicative);

            InstanceManager.RenderSprite.Draw(
                blend,
                pos + new Vector2((float)(delta + add.Width + alph.Width + mult.Width + 2 * blend.Width), 0f),
                Vector2.Zero,
                null,
                c,
                0f,
                1f,
                1f,
                RenderSpriteBlendMode.Multiplicative);
        }
        #endregion

        #region Draw / Update
        public override void Update(GameTime gameTime)
        {
            if (startPos.X == -1f)
            {
                startPos = new Vector2(
                    (float)InstanceManager.DefaultViewport.TitleSafeArea.X,
                    (float)InstanceManager.DefaultViewport.TitleSafeArea.Y);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 pos;
            Vector2 offset = Vector2.Zero;
            Vector2 size;
            string temp;
            for (int i = 0; i < ColorState.numberOfColorStates; i++)
            {
                pos = startPos + new Vector2(0f, 6f * i * vertSize);
                for (int j = 0; j < ColorState.numberColorsPerPolarity; j++){
                    size = font.MeasureString(colorStates[i].PositiveName);
                    offset.X = maxNameWidth - size.X;
                    temp = String.Format("{0} {1} +", colorStates[i].PositiveName, j.ToString());
                    DrawDetails(pos + offset, temp, colorStates[i].Positive[j]);

                    pos += new Vector2(0f, (float)blend.Height);
                    size = font.MeasureString(colorStates[i].NegativeName);
                    offset.X = maxNameWidth - size.X;
                    temp = String.Format("{0} {1} -", colorStates[i].NegativeName, j.ToString());
                    DrawDetails(pos + offset, temp, colorStates[i].Negative[j]);
                    pos += new Vector2(0f, (float)blend.Height);
                }
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}
