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
using Duologue.State;
using Duologue.Screens;
#endregion

namespace Duologue.PlayObjects
{
    class Enemy_Spitter : Enemy
    {
        public enum SpitterState
        {
            Spawning,
            Alive,
        }
        #region Constants
        private const string filename_base = "Enemies/spitter/0{0}-base";
        private const string filename_outline = "Enemies/spitter/0{0}-outline";
        private const string filename_spawnExplode = "Enemies/spitter/spawn-explode";
        private const string filename_spit = "Enemies/spitter/spit-{0}";

        /// <summary>
        /// Max number of frames of animation
        /// </summary>
        private const int maxAnimationFrames = 5;

        /// <summary>
        /// Max number of frames for the spit
        /// </summary>
        private const int maxSpitFrames = 3;

        /// <summary>
        /// The delta size per frame
        /// </summary>
        private const int spitDeltaDefaultSize = 4;

        private const int lowerSpitAlpha = 50;
        private const int upperSpitAlpha = 200;
        #endregion

        #region Fields
        private Texture2D[] textureBase;
        private Texture2D[] textureOutline;
        private Vector2[] frameCenters;
        private Texture2D[] textureSpit;
        private Vector2[] spitCenters;
        private Texture2D textureSpawnExplode;
        private Vector2 spawnExplodeCenter;

        private Vector2 screenCenter;

        private int currentFrame;
        private byte[] currentSpitAlphas;
        private int[] spitAlphaDeltas;

        private float upperLeftAngle;
        private float upperRightAngle;
        private float lowerRightAngle;
        private float lowerLeftAngle;

        private Vector2 upperLeftCorner;
        private Vector2 upperRightCorner;
        private Vector2 lowerRightCorner;
        private Vector2 lowerLeftCorner;

        private float upperBoundaryX;
        private float lowerBoundaryX;
        private float upperBoundaryY;
        private float lowerBoundaryY;

        private float innerBoundsWidth;
        private float innerBoundsHeight;

        private float rotation;
        #endregion

        #region Properties
        public SpitterState MyState;
        #endregion

        #region Constructor / Init
        public Enemy_Spitter(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Spitter;
            MajorType = MajorPlayObjectType.Enemy;
            RealSize = new Vector2(85, 106);
            Initialized = false;
            Alive = false;
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

            LoadAndInitialize();
        }
        #endregion

        #region Private methods
        private void LoadAndInitialize()
        {
            textureBase = new Texture2D[maxAnimationFrames];
            textureOutline = new Texture2D[maxAnimationFrames];
            frameCenters = new Vector2[maxAnimationFrames];
            textureSpit = new Texture2D[maxSpitFrames];
            spitCenters = new Vector2[maxSpitFrames];
            currentSpitAlphas = new byte[maxSpitFrames];
            spitAlphaDeltas = new int[maxSpitFrames];

            // Load the animation frames
            for (int i = 0; i < maxAnimationFrames; i++)
            {
                textureBase[i] = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_base, (i + 1).ToString()));
                textureOutline[i] = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_outline, (i + 1).ToString()));
                frameCenters[i] = new Vector2(textureOutline[i].Width / 2f, textureOutline[i].Height / 2f);
            }

            // Load the spit frames
            for (int i = 0; i < maxSpitFrames; i++)
            {
                textureSpit[i] = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_spit, (i + 1).ToString()));
                spitCenters[i] = new Vector2(textureSpit[i].Width / 2f, textureSpit[i].Height / 2f);
            }

            // Texture for the spawn/explode image
            textureSpawnExplode = InstanceManager.AssetManager.LoadTexture2D(filename_spawnExplode);
            spawnExplodeCenter = new Vector2(textureSpawnExplode.Width / 2f, textureSpawnExplode.Height / 2f);

            // Compute orientation
            Orientation = GetStartingVector();
            rotation = MWMathHelper.ComputeAngleAgainstX(Orientation);

            currentFrame = 1;
            screenCenter = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f,
                InstanceManager.DefaultViewport.Height / 2f);

            // Determine the corners and their angles
            // One thing to remember here is we're actually flipped upside down
            // (which means the upper become lower and visa versa)
            lowerBoundaryX = InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent;
            lowerBoundaryY = InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent;

            upperBoundaryX = InstanceManager.DefaultViewport.Width
                    - InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent;
            upperBoundaryY = InstanceManager.DefaultViewport.Height
                    - InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent;

            innerBoundsWidth = upperBoundaryX - lowerBoundaryX;
            innerBoundsHeight = upperBoundaryY - lowerBoundaryY;

            lowerLeftCorner = new Vector2(lowerBoundaryX, lowerBoundaryY);
            lowerRightCorner = new Vector2(upperBoundaryX, lowerBoundaryY);

            upperRightCorner = new Vector2(upperBoundaryX, upperBoundaryY);
            upperLeftCorner = new Vector2(lowerBoundaryX, upperBoundaryY);

            upperLeftAngle = MWMathHelper.ComputeAngleAgainstX(upperLeftCorner, screenCenter);
            upperRightAngle = MWMathHelper.ComputeAngleAgainstX(upperRightCorner, screenCenter);
            lowerLeftAngle = MWMathHelper.ComputeAngleAgainstX(lowerLeftCorner, screenCenter);
            lowerRightAngle = MWMathHelper.ComputeAngleAgainstX(lowerRightCorner, screenCenter);

            // Place us at the max position
            SetAtMaxPosition();

            MyState = SpitterState.Spawning;

            Initialized = true;
        }

        /// <summary>
        /// Generate the spit alphas (should be called before each firing)
        /// </summary>
        private void GenerateSpitAlphas()
        {
            for (int i = 0; i < maxSpitFrames; i++)
            {
                currentSpitAlphas[i] = (byte)MWMathHelper.GetRandomInRange(lowerSpitAlpha, upperSpitAlpha);
                if (MWMathHelper.CoinToss())
                    spitAlphaDeltas[i] = -1 * spitDeltaDefaultSize;
                else
                    spitAlphaDeltas[i] = spitDeltaDefaultSize;
            }
        }

        /// <summary>
        /// Sets us at the maximum position away from center based upon our rotation around the center of screen
        /// </summary>
        private void SetAtMaxPosition()
        {
            float angle = MWMathHelper.ComputeAngleAgainstX(Position, screenCenter);
            // x = L cos(angle), y = L sin(angle)
            float x;
            float y;
            float tan = (float)Math.Tan(angle);
            // Remember, we need to flip the Y coords
            if (angle >= 0 && angle <= upperRightAngle || angle > lowerRightAngle && angle <= MathHelper.TwoPi)
            {
                // We are on the right side of the screen
                x = innerBoundsWidth;
                y = (0.5f) * (innerBoundsHeight - tan * innerBoundsWidth);
            }
            else if (angle > upperRightAngle && angle <= upperLeftAngle)
            {
                // We are on the top of the screen
                x = (0.5f) * (innerBoundsWidth + innerBoundsHeight / tan);
                y = lowerBoundaryY;
            }
            else if (angle > upperLeftAngle && angle <= lowerLeftAngle)
            {
                // We are on the left side of the screen
                x = lowerBoundaryX;
                y = (0.5f) * (innerBoundsHeight + innerBoundsWidth * tan);
            }
            else //if (angle > lowerLeftAngle && angle <= lowerRightAngle)
            {
                // We are on the bottom of the screen
                x = (0.5f) * (innerBoundsWidth - innerBoundsHeight / tan);
                y = innerBoundsHeight;
            }
            Position.X = x;
            Position.Y = y;
        }

        /// <summary>
        /// Returns a vector pointing to the origin
        /// </summary>
        private Vector2 GetVectorPointingAtOrigin()
        {
            Vector2 sc = new Vector2(
                    InstanceManager.DefaultViewport.Width / 2f,
                    InstanceManager.DefaultViewport.Height / 2f);
            return sc - Position;
        }

        /// <summary>
        /// Get a starting vector for this dude
        /// </summary>
        private Vector2 GetStartingVector()
        {
            // Just aim at the center of screen for now
            Vector2 temp = GetVectorPointingAtOrigin() + new Vector2(
                (float)MWMathHelper.GetRandomInRange(-.5, .5),
                (float)MWMathHelper.GetRandomInRange(-.5, .5));
            temp.Normalize();
            return temp;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Public Overrides
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
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if (Position.X < frameCenters[currentFrame].X
                || Position.X > InstanceManager.DefaultViewport.Width - frameCenters[currentFrame].X
                || Position.Y < frameCenters[currentFrame].Y
                || Position.Y > InstanceManager.DefaultViewport.Height - frameCenters[currentFrame].Y)
            {
                SetAtMaxPosition();
            }
        }
        #endregion
    }
}
