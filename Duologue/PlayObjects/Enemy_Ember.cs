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
using Mimicware.Graphics;
using Mimicware.Manager;
// Duologue
using Duologue.Audio;
using Duologue.State;
using Duologue.Screens;
#endregion

namespace Duologue.PlayObjects
{
    public class Enemy_Ember : Enemy
    {
        #region Constants
        private const string filename_Ember = "Enemies/ember";
        private const string filename_frames = "Enemies/static-king-{0}";

        private const int numberOfFrames = 4;

        /// <summary>
        /// The minimum scale for the ember
        /// </summary>
        private const double minScale = 0.3;

        /// <summary>
        /// The maximum scale for the ember
        /// </summary>
        private const double maxScale = 0.9;

        /// <summary>
        /// The vertical offset of the ember
        /// </summary>
        private const float verticalOffset = 8f;

        /// <summary>
        /// The vertical scale for the flame
        /// </summary>
        private const float flameVerticalScale = 1.2f;

        /// <summary>
        /// The starting scale for each frame
        /// </summary>
        private const float startingScale = 0.6f;

        /// <summary>
        /// The delta scale per frame of animation
        /// </summary>
        private const float deltaScale = 0.005f;

        /// <summary>
        /// The initial delta scale per frame
        /// </summary>
        private const float initDeltaScale = 0.1f;

        /// <summary>
        /// The minimum scale to begin a fade
        /// </summary>
        private const float minScaleToTriggerFade = 1.05f;

        /// <summary>
        /// The minimum alpha that the frame can be
        /// </summary>
        private const byte minAlpha = 45;

        /// <summary>
        /// The delta alpha per frame once we start fading
        /// </summary>
        private const int deltaAlpha = -20;

        /// <summary>
        /// The maximum starting angle (in either direction)
        /// </summary>
        private const float maxStartingAngle = MathHelper.PiOver4 / 2f;

        /// <summary>
        /// The maximum delta angle
        /// </summary>
        private const float maxDeltaAngle = MathHelper.PiOver4 * 0.005f;

        /// <summary>
        /// The maximum vertical offset
        /// </summary>
        private const double maxVerticalOffset = -4.0;

        /// <summary>
        /// The size of the outline
        /// </summary>
        private const float outlineSize = 1.05f;

        /// <summary>
        /// The time it takes to change ember colors
        /// </summary>
        private const double emberColorChangeFadeTime = 0.1;

        /// <summary>
        /// The time it takes between ember color changes
        /// </summary>
        private const double emberColorChangeDelta = 0.4;
        #endregion

        #region Fields
        private Texture2D ember;
        private Vector2 emberCenter;
        private StaticKingFrame[] flameFrames;
        private Vector2 flameCenter;
        private Vector2 flameScale;
        private float emberScale;
        private Vector2 emberOffset;

        private Color flameColor;
        private Color[] emberColors;
        private int currentEmberColor;
        private int lastEmberColor;
        private double timeSinceColorChange;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public Enemy_Ember(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Ember;
            MajorType = MajorPlayObjectType.Enemy;
            Initialized = false;

            // Set the RealSize by hand
            RealSize = new Vector2(100, 95);
        }

        public override void Initialize(
            Vector2 startPos, 
            Vector2 startOrientation, 
            ColorState currentColorState, 
            ColorPolarity startColorPolarity, 
            int? hitPoints)
        {
            Position = startPos;
            Orientation = startOrientation;
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            if (hitPoints == null)
            {
                hitPoints = 0;
            }
            StartHitPoints = (int)hitPoints;
            CurrentHitPoints = (int)hitPoints;
            //audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            ember = InstanceManager.AssetManager.LoadTexture2D(filename_Ember);
            flameFrames = new StaticKingFrame[numberOfFrames];

            for (int i = 0; i < numberOfFrames; i++)
            {
                flameFrames[i].Texture = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_frames, (i + 1).ToString()));
                SetupFrame(i);
                flameFrames[i].Scale = startingScale + i * initDeltaScale;
            }

            flameCenter = new Vector2(
                flameFrames[0].Texture.Width / 2f,
                flameFrames[0].Texture.Height / 2f);
            emberCenter = new Vector2(
                ember.Width / 2f,
                ember.Height / 2f);

            emberScale = (float)MWMathHelper.GetRandomInRange(minScale, maxScale);
            emberOffset = new Vector2(0, verticalOffset * emberScale);
            flameScale = new Vector2(emberScale, emberScale * flameVerticalScale);

            flameColor = GetMyColor(ColorState.Medium);
            emberColors = new Color[ColorState.NumberColorsPerPolarity];
            emberColors[0] = GetMyColor(ColorState.Light);
            emberColors[1] = GetMyColor(ColorState.Medium);
            emberColors[2] = GetMyColor(ColorState.Dark);
            currentEmberColor = 0;
            lastEmberColor = 0;
            timeSinceColorChange = 0;

            Initialized = true;
            Alive = true;
        }
        #endregion

        #region Private methods
        private void SetupFrame(int i)
        {
            flameFrames[i].Scale = startingScale;
            flameFrames[i].Alpha = Color.White.A;
            if (MWMathHelper.CoinToss())
            {
                flameFrames[i].Rotation = (float)MWMathHelper.GetRandomInRange(-1.0 * maxStartingAngle, 0.0);
                flameFrames[i].DeltaRotation = (float)MWMathHelper.GetRandomInRange(0.0, maxDeltaAngle);
            }
            else
            {
                flameFrames[i].Rotation = (float)MWMathHelper.GetRandomInRange(0.0, maxStartingAngle);
                flameFrames[i].DeltaRotation = (float)MWMathHelper.GetRandomInRange(-1.0 * maxDeltaAngle, 0.0);
            }
            flameFrames[i].Offset = new Vector2(
                0f, (float)MWMathHelper.GetRandomInRange(0.0, maxVerticalOffset));
        }
        #endregion

        #region Overrides
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

        #region Draw/Update
        public override void Draw(GameTime gameTime)
        {
            // Draw the outline (FIXME would be lovely if we didn't have to do this in two for loops)
            for (int i = 0; i < numberOfFrames; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    flameFrames[i].Texture,
                    Position,
                    flameCenter,
                    null,
                    new Color(Color.Black, flameFrames[i].Alpha),
                    flameFrames[i].Rotation,
                    flameFrames[i].Scale * outlineSize * flameScale,
                    0f);
            }
            // Draw the vapors
            for (int i = 0; i < numberOfFrames; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    flameFrames[i].Texture,
                    Position,
                    flameCenter,
                    null,
                    new Color(flameColor, flameFrames[i].Alpha),
                    flameFrames[i].Rotation,
                    flameFrames[i].Scale * flameScale,
                    0f);
            }
            // Draw the ember
            if (lastEmberColor != currentEmberColor)
            {
                InstanceManager.RenderSprite.Draw(
                    ember,
                    Position + emberOffset,
                    emberCenter,
                    null,
                    emberColors[lastEmberColor],
                    0f,
                    emberScale,
                    0f);

                InstanceManager.RenderSprite.Draw(
                    ember,
                    Position + emberOffset,
                    emberCenter,
                    null,
                    new Color(emberColors[currentEmberColor], (float)(timeSinceColorChange / emberColorChangeFadeTime)),
                    0f,
                    emberScale,
                    0f);
            }
            else
            {
                InstanceManager.RenderSprite.Draw(
                    ember,
                    Position + emberOffset,
                    emberCenter,
                    null,
                    emberColors[currentEmberColor],
                    0f,
                    emberScale,
                    0f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceColorChange += gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < numberOfFrames; i++)
            {
                flameFrames[i].Scale += deltaScale;
                flameFrames[i].Rotation += flameFrames[i].DeltaRotation;
                if (flameFrames[i].Rotation < 0f)
                    flameFrames[i].Rotation = MathHelper.TwoPi;
                else if (flameFrames[i].Rotation > MathHelper.TwoPi)
                    flameFrames[i].Rotation = 0f;

                if (flameFrames[i].Scale > minScaleToTriggerFade)
                {
                    flameFrames[i].Alpha = (byte)(flameFrames[i].Alpha + deltaAlpha);
                    if (flameFrames[i].Alpha < minAlpha)
                        SetupFrame(i);
                }
            }

            if (lastEmberColor != currentEmberColor)
            {
                if (timeSinceColorChange >= emberColorChangeFadeTime)
                {
                    lastEmberColor = currentEmberColor;
                    timeSinceColorChange = 0;
                }
            }
            else
            {
                if (timeSinceColorChange >= emberColorChangeDelta)
                {
                    lastEmberColor = currentEmberColor;
                    currentEmberColor++;
                    if (currentEmberColor >= emberColors.Length)
                        currentEmberColor = 0;
                    timeSinceColorChange = 0;
                }
            }
        }
        #endregion
    }
}
