#region File description
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Mimicware.Graphics;
using Mimicware.Manager;
using Mimicware;
#endregion

namespace Duologue.PlayObjects
{
    public struct BKG_SmokeParticle
    {
        public Vector2 Position;
        public float StartSize;
        public float EndSize;
        public double Timer;
    }

    public class BKG_LavaBurp : PlayObject
    {
        #region Constants
        private const string filename_Flame = "Cinematics/flame-{0}";
        private const int frames_Flame = 4;
        private const string filename_Base = "Cinematics/flame-base";
        private const string filename_Smoke = "Cinematics/smoke";

        private const int numberOfSmokeParticles = 30;
        private const float alpha_SmokeStart = 0.78f;
        private const float alpha_SmokeEnd = 0.0002f;

        private const double totalTime_SmokeRise = 5.1;
        private const double totalTime_FlameChange = 0.03;

        private const double size_MinStart = 0.08;
        private const double size_MaxStart = 0.4;

        private const double size_MinEnd = 1.26;
        private const double size_MaxEnd = 2.1;

        private const float maxRandomOffsetPercent = 0.8f;

        private const float deltaMinX = 0.001f;
        private const float deltaMaxX = 0.9f;
        private const float deltaMinY = 0.01f;
        private const float deltaMaxY = 0.008f;

        private const float percentSmokeBlitModeSwitch = 0.25f;
        #endregion

        #region Fields
        private Texture2D[] texture_Flames;
        private Texture2D texture_Base;
        private Texture2D texture_Smoke;
        private Vector2 center_Flame;
        private Vector2 center_Base;
        private Vector2 center_Smoke;

        private Vector2 position_End;

        private int flameFrame_Add;
        private int flameFrame_Alpha;

        private BKG_SmokeParticle[] smokeParticles;
        private Color color_SmokeStart;
        private Color color_SmokeEnd;

        private double timer_FlameChange;
        #endregion

        #region Constructor / Init
        public BKG_LavaBurp()
            : base()
        {
        }

        public void Initialize(Vector2 position)
        {
            Position = position;

            // Load the textures
            texture_Base = AssetManager.LoadTexture2D(filename_Base);
            texture_Smoke = AssetManager.LoadTexture2D(filename_Smoke);

            texture_Flames = new Texture2D[frames_Flame];
            for (int i = 0; i < frames_Flame; i++)
            {
                texture_Flames[i] = AssetManager.LoadTexture2D(String.Format(
                    filename_Flame, i.ToString()));
            }

            center_Base = new Vector2(
                texture_Base.Width / 2f, texture_Base.Height / 2f);
            center_Flame = new Vector2(
                texture_Flames[0].Width / 2f, texture_Flames[0].Height / 2f);
            center_Smoke = new Vector2(
                texture_Smoke.Width / 2f, texture_Smoke.Height / 2f);

            flameFrame_Add = 1;
            flameFrame_Alpha = 0;
            timer_FlameChange = 0;

            // Set up the smoke particles
            smokeParticles = new BKG_SmokeParticle[numberOfSmokeParticles];
            for (int i = 0; i < numberOfSmokeParticles; i++)
            {
                smokeParticles[i] = GenerateFreshParticle();
                smokeParticles[i].Timer = (double)MathHelper.Lerp(
                    0, (float)totalTime_SmokeRise, (float)i / (float)numberOfSmokeParticles);

                smokeParticles[i].Position += new Vector2(
                    deltaMinX * i, deltaMinY * i);
            }

            color_SmokeStart = new Color(255, 62, 62, 90);
            color_SmokeEnd = new Color(160, 164, 164, 15);
        }

        public override String[] GetTextureFilenames()
        {
            String[] filenames = new String[frames_Flame + 2];
            int i = 0;

            filenames[i] = filename_Base;
            i++;
            filenames[i] = filename_Smoke;
            i++;

            for (int t = 0; t < frames_Flame; t++)
            {
                filenames[i] = String.Format(filename_Flame, t);
                i++;
            }

            return filenames;
        }
        #endregion

        #region Private methods
        private BKG_SmokeParticle GenerateFreshParticle()
        {
            BKG_SmokeParticle temp = new BKG_SmokeParticle();

            temp.Timer = 0;

            temp.Position = Position + new Vector2(
                (float)MWMathHelper.GetRandomInRange(-maxRandomOffsetPercent * center_Base.X, maxRandomOffsetPercent * center_Base.X),
                (float)MWMathHelper.GetRandomInRange(-maxRandomOffsetPercent * center_Base.Y, maxRandomOffsetPercent * center_Base.Y));

            temp.StartSize = (float)MWMathHelper.GetRandomInRange(size_MinStart, size_MaxStart);
            temp.EndSize = (float)MWMathHelper.GetRandomInRange(size_MinEnd, size_MaxEnd);

            return temp;
        }
        #endregion

        #region Stuff that should never be called, bitch
        public override bool StartOffset()
        {
            throw new NotImplementedException();
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            throw new NotImplementedException();
        }

        public override bool ApplyOffset()
        {
            throw new NotImplementedException();
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            // Draw the flames
            RenderSprite.Draw(
                texture_Flames[flameFrame_Alpha],
                Position,
                center_Flame,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.Addititive);

            RenderSprite.Draw(
                texture_Flames[flameFrame_Add],
                Position,
                center_Flame,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.Addititive);

            // Draw the base
            RenderSprite.Draw(
                texture_Base,
                Position,
                center_Base,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlend);

            float percent;
            // Draw the smoke
            for (int i = 0; i < numberOfSmokeParticles; i++)
            {
                percent = (float)smokeParticles[i].Timer / (float)totalTime_SmokeRise;
                if (percent < percentSmokeBlitModeSwitch)
                {
                    RenderSprite.Draw(
                        texture_Smoke,
                        smokeParticles[i].Position,
                        center_Smoke,
                        null,
                        new Color(
                            (byte)MathHelper.Lerp(color_SmokeStart.R, color_SmokeEnd.R, percent),
                            (byte)MathHelper.Lerp(color_SmokeStart.G, color_SmokeEnd.G, percent),
                            (byte)MathHelper.Lerp(color_SmokeStart.B, color_SmokeEnd.B, percent),
                            (byte)MathHelper.Lerp(color_SmokeStart.A, color_SmokeEnd.A, percent)),
                        0f,
                        MathHelper.Lerp(
                            smokeParticles[i].StartSize,
                            smokeParticles[i].EndSize, percent),
                        0f,
                        RenderSpriteBlendMode.Addititive);
                }
                else
                {
                    RenderSprite.Draw(
                        texture_Smoke,
                        smokeParticles[i].Position,
                        center_Smoke,
                        null,
                        new Color(
                            (byte)MathHelper.Lerp(color_SmokeStart.R, color_SmokeEnd.R, percent),
                            (byte)MathHelper.Lerp(color_SmokeStart.G, color_SmokeEnd.G, percent),
                            (byte)MathHelper.Lerp(color_SmokeStart.B, color_SmokeEnd.B, percent),
                            (byte)MathHelper.Lerp(color_SmokeStart.A, color_SmokeEnd.A, percent)),
                        0f,
                        MathHelper.Lerp(
                            smokeParticles[i].StartSize,
                            smokeParticles[i].EndSize, percent),
                        0f,
                        RenderSpriteBlendMode.AlphaBlendTop);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            double deltaTimer = gameTime.ElapsedGameTime.TotalSeconds;
            float percent;
            for (int i = 0; i < numberOfSmokeParticles; i++)
            {
                smokeParticles[i].Timer += deltaTimer;
                if (smokeParticles[i].Timer <= totalTime_SmokeRise)
                {
                    percent = (float)smokeParticles[i].Timer / (float)totalTime_SmokeRise;
                    smokeParticles[i].Position += new Vector2(
                        MathHelper.Lerp(deltaMinX, deltaMaxX, percent),
                        MathHelper.Lerp(deltaMinY, deltaMaxY, percent));
                }
                else
                {
                    smokeParticles[i] = GenerateFreshParticle();
                }
            }

            timer_FlameChange += deltaTimer;
            if (timer_FlameChange > totalTime_FlameChange)
            {
                timer_FlameChange = 0;
                flameFrame_Alpha = flameFrame_Add;
                flameFrame_Add++;
                if (flameFrame_Add >= frames_Flame)
                    flameFrame_Add = 0;
            }
        }
        #endregion
    }
}