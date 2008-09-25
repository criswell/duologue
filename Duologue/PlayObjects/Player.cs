using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Mimicware;
using Mimicware.Graphics;
using Mimicware.Manager;
using Duologue.State;

namespace Duologue.PlayObjects
{
    class Player : PlayObject
    {
        #region Fields
        private SpriteObject playerBase;
        private SpriteObject playerCannon;
        private SpriteObject playerLight;
        private SpriteObject beam;
        private SpriteObject shot;
        private SpriteObject beamBase;
        private const int treadFrames = 2;
        private Texture2D[] playerTreads;
        private Vector2 treadCenter;
        private int currentTread;
        private int treadTimer;
        private const int maxTreadTimer = 50;
        private const int shineFrames = 4;
        private Texture2D[] playerShines;
        private Vector2 shineCenter;
        private int currentShine;
        private int shineTimer;
        private const int maxShineTimer = 20;
        private bool lightIsNegative;
        private Vector2 lastPosition;

        // Stuff for computing the tread offset
        private Vector2 treadOffset;
        private const float maxTreadOffset = 10f;
        private Vector2 screenCenter;

        // The beam arc and radius
        private const float beamRadius = 400f;
        private float beamArcMin;
        private float beamArcMax;
        private float piOver6 = MathHelper.Pi / 6f;
        #endregion

        #region Properties
        /// <summary>
        /// The orientation of the player
        /// </summary>
        public Vector2 Orientation;
        /// <summary>
        /// The aim of the player
        /// </summary>
        public Vector2 Aim;

        /// <summary>
        /// The rotation of the player base
        /// </summary>
        public float BaseRotation;
        /// <summary>
        /// The rotation of the cannon
        /// </summary>
        public float CannonRotation;
        /// <summary>
        /// The rotation of the light
        /// </summary>
        public float LightRotation;
        /// <summary>
        /// The rotation of the beam
        /// </summary>
        public float BeamRotation;

        public float TreadRotation;

        public PlayerShot Shot;
        public ColorState colorState;

        public Color PlayerTint;
        #endregion

        #region Constructor / Init
        public Player(ColorState currentColorState, Color playerTint)
            : base()
        {
            colorState = currentColorState;
            lightIsNegative = true;
            PlayerTint = playerTint;
            Initialize();
        }

        private void Initialize()
        {
            // Gonna cause some errors me think
            Shot = new PlayerShot();
            Orientation = Vector2.UnitX;
            Aim = Vector2.Negate(Orientation);
            CaclulateRotations();
            lastPosition = Vector2.Zero;
            treadTimer = 0;
            shineTimer = 0;
            beamArcMax = 0f;
            beamArcMin = 0f;
            treadOffset = Vector2.Zero;

            if (AssetManager == null)
                AssetManager = InstanceManager.AssetManager;
        
            // Load player objects
            Position = new Vector2(InstanceManager.DefaultViewport.Width / 2f, InstanceManager.DefaultViewport.Height / 2f);
            playerBase = new SpriteObject(
                AssetManager.LoadTexture2D("player-base"),
                Position,
                new Vector2(AssetManager.LoadTexture2D("player-base").Width / 2f, AssetManager.LoadTexture2D("player-base").Height / 2f),
                null,
                PlayerTint,
                0f,
                1f,
                0.5f);

            playerCannon = new SpriteObject(
                AssetManager.LoadTexture2D("player-cannon"),
                Position,
                new Vector2(AssetManager.LoadTexture2D("player-cannon").Width / 2f, AssetManager.LoadTexture2D("player-cannon").Height / 2f),
                null,
                colorState.Positive[1],
                0f,
                1f,
                0.4f);

            playerLight = new SpriteObject(
                AssetManager.LoadTexture2D("player-light"),
                Position,
                new Vector2(AssetManager.LoadTexture2D("player-light").Width / 2f, AssetManager.LoadTexture2D("player-light").Height / 2f),
                null,
                colorState.Negative[1],
                0f,
                1f,
                0.4f);

            // Load projectile object
            shot = new SpriteObject(
                AssetManager.LoadTexture2D("shot"),
                Vector2.Zero,
                new Vector2(AssetManager.LoadTexture2D("shot").Width / 2f, AssetManager.LoadTexture2D("shot").Height / 2f),
                null,
                colorState.Positive[0],
                0f,
                1f,
                1f);

            shot.Alive = false;

            // Load beam object
            beam = new SpriteObject(
                AssetManager.LoadTexture2D("beam"),
                Position,
                new Vector2(971f, 253f),
                null,
                colorState.Negative[1],
                0f,
                1f,
                1f);

            beamBase = new SpriteObject(
                AssetManager.LoadTexture2D("beam-base"),
                Position,
                new Vector2(971f, 253f),
                null,
                colorState.Negative[0],
                0f,
                1f,
                1f);
            SetColors();

            playerTreads = new Texture2D[treadFrames];
            for (int i = 0; i < treadFrames; i++)
            {
                playerTreads[i] = AssetManager.LoadTexture2D(String.Format("{0:tread00}", treadFrames-i));
            }
            currentTread = 0;
            treadCenter = new Vector2(
                playerTreads[currentTread].Width / 2f,
                playerTreads[currentTread].Height / 2f);

            playerShines = new Texture2D[shineFrames];
            for (int i = 0; i < shineFrames; i++)
            {
                //string temp = String.Format("shine{0:00}", i + 1);
                playerShines[i] = AssetManager.LoadTexture2D(String.Format("shine{0:00}", i+1));
            }
            currentShine = 0;
            shineCenter = new Vector2(
                playerShines[currentShine].Width / 2f,
                playerShines[currentShine].Height / 2f);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Ensure that we're still inside screenboundaries.
        /// </summary>
        private void CheckScreenBoundary()
        {
            if (Position.X > InstanceManager.DefaultViewport.Width - playerBase.Texture.Width / 2f)
                Position.X = InstanceManager.DefaultViewport.Width - playerBase.Texture.Width / 2f;
            if (Position.X < playerBase.Texture.Width / 2f)
                Position.X = playerBase.Texture.Width / 2f;

            if (Position.Y > InstanceManager.DefaultViewport.Height - playerBase.Texture.Height / 2f)
                Position.Y = InstanceManager.DefaultViewport.Height - playerBase.Texture.Height / 2f;
            if (Position.Y < playerBase.Texture.Height / 2f)
                Position.Y = playerBase.Texture.Height / 2f;
        }

        /// <summary>
        /// Calculate the various rotations, should be called once per frame
        /// </summary>
        private void CaclulateRotations()
        {
            // The base is easy because we can fuck it up- the base is a circle with
            // no real orientation.
            BaseRotation = MWMathHelper.ComputeAngleAgainstX(Orientation);

            // The light rotation is a bit tricky because it starts in the left coordinate system
            LightRotation = BaseRotation + 3f * MathHelper.PiOver4;

            // Next up, the light beam rotation is 180 degrees from the base
            BeamRotation = BaseRotation + MathHelper.Pi;

            // We also need the arc that defines the beam;
            beamArcMin = BaseRotation - piOver6;
            beamArcMax = BaseRotation + piOver6;

            // Next, we do the cannon
            CannonRotation = MWMathHelper.ComputeAngleAgainstX(Aim);

            // We have to do this after the Aim.Y test because it could cross the angle = 0/Pi boundary
            CannonRotation += MathHelper.PiOver2;

            // Now, tread rotation
            TreadRotation = BaseRotation + MathHelper.PiOver2;
        }

        private void SetColors()
        {
            if (lightIsNegative)
            {
                playerLight.Tint = colorState.Negative[1];
                beam.Tint = colorState.Negative[1];
                beamBase.Tint = colorState.Negative[0];

                playerCannon.Tint = colorState.Positive[1];
                shot.Tint = colorState.Positive[0];
            }
            else
            {
                playerLight.Tint = colorState.Positive[1];
                beam.Tint = colorState.Positive[1];
                beamBase.Tint = colorState.Positive[0];

                playerCannon.Tint = colorState.Negative[1];
                shot.Tint = colorState.Negative[0];
            }
        }

        /// <summary>
        /// Figure out the current treadoffset
        /// </summary>
        private void ComputeTreadOffset()
        {
            // Get distance
            float distance = Vector2.Subtract(screenCenter, Position).Length();

            // Compute the size of the offset based on distance
            float size = maxTreadOffset * (distance / screenCenter.Length());

            // Aim at center of screen
            treadOffset = Vector2.Add(screenCenter, Position);
            treadOffset.Normalize();
            treadOffset *= size;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Call when the colors need to be swapped
        /// </summary>
        internal void SwapColors()
        {
            lightIsNegative = !lightIsNegative;
            SetColors();
        }

        /// <summary>
        /// Call when a fire request is made
        /// </summary>
        internal void Fire()
        {
            Vector2 startPos = Position; // FIXME
            Shot.Fire(Aim, playerCannon.Tint, startPos);
        }

        /// <summary>
        /// Given a position and color, will determine if it is in the beam, and if it is complementary or opposite
        /// </summary>
        /// <param name="vector2">Position vector</param>
        /// <param name="color">Color of the itme</param>
        /// <returns>Returns 0 if not in beam. -1 if in beam and opposite colors. +1 if in beam and complimentary colors.</returns>
        internal int IsInBeam(Vector2 vector2, Color color)
        {
            int retval = 0;
            // Check if in-beam
            Vector2 distance = vector2 - Position;
            if (Math.Abs(distance.Length()) < beamRadius)
            {
                // We're close enough... inside the arc?
                float rotation = MWMathHelper.ComputeAngleAgainstX(distance);
                if (rotation > beamArcMin && rotation < beamArcMax)
                {
                    // In the beam
                    // Check if complimentary color
                    if (colorState.SameColor(color, playerLight.Tint))
                    {
                        retval = 1;
                    }
                    else
                    {
                        retval = -1;
                    }
                }
            }

            return retval;
        }

        #endregion

        #region Draw / Update
        /// <summary>
        /// Draw the player object
        /// </summary>
        /// <param name="gameTime">Gametime</param>
        internal void Draw(GameTime gameTime)
        {
            if (RenderSprite == null)
                RenderSprite = InstanceManager.RenderSprite;

            CaclulateRotations();
            CheckScreenBoundary();

            // Treads
            RenderSprite.Draw(
                playerTreads[currentTread],
                Position + treadOffset,
                treadCenter,
                null,
                playerBase.Tint,
                TreadRotation,
                1f,
                0.5f);
                

            // Base
            RenderSprite.Draw(
                playerBase.Texture,
                Position,
                playerBase.Center,
                null,
                playerBase.Tint,
                BaseRotation,
                1f,
                0.5f);

            // Shine
            RenderSprite.Draw(
                playerShines[currentShine],
                Position,
                shineCenter,
                null,
                playerBase.Tint,
                0f,
                1f,
                0.5f);

            // Light
            RenderSprite.Draw(
                playerLight.Texture,
                Position,
                playerLight.Center,
                null,
                playerLight.Tint,
                LightRotation,
                1f,
                0.5f,
                true);

            // Lightbeam
            RenderSprite.Draw(
                beam.Texture,
                Position,
                beam.Center,
                null,
                beam.Tint,
                BeamRotation,
                1f,
                0.5f,
                true);

            RenderSprite.Draw(
                beamBase.Texture,
                Position,
                beamBase.Center,
                null,
                beamBase.Tint,
                BeamRotation,
                1f,
                0.5f,
                true);

            // Cannon
            RenderSprite.Draw(
                playerCannon.Texture,
                Position,
                playerCannon.Center,
                null,
                playerCannon.Tint,
                CannonRotation,
                1f,
                0.5f);

            Shot.Draw(gameTime);
        }

        /// <summary>
        /// Called once per frame
        /// </summary>
        /// <param name="gameTime"></param>
        internal void Update(GameTime gameTime)
        {
            if (lastPosition != Position)
            {
                treadTimer++;
                if (treadTimer > maxTreadTimer)
                {
                    treadTimer = 0;
                    currentTread++;
                    if (currentTread >= treadFrames)
                        currentTread = 0;
                }
            }
            if (lastPosition.X != Position.X)
            {
                shineTimer++;
                if (shineTimer > maxShineTimer)
                {
                    shineTimer = 0;
                    currentShine += (int)((lastPosition.X - Position.X) / Math.Abs(lastPosition.X - Position.X));
                    if (currentShine >= shineFrames)
                        currentShine = 0;
                    else if (currentShine < 0)
                        currentShine = shineFrames - 1;
                }
            }
            if (screenCenter == Vector2.Zero)
            {
                screenCenter = new Vector2(
                    InstanceManager.GraphicsDevice.Viewport.Width / 2f,
                    InstanceManager.GraphicsDevice.Viewport.Height / 2f);
                InstanceManager.Logger.LogEntry(screenCenter.ToString());
            }
            ComputeTreadOffset();
            lastPosition = Position;
            Shot.Update(gameTime);
        }
        #endregion


        #region Public Overrides
        /// <summary>
        /// Call to set the AssetManager
        /// </summary>
        /// <param name="manager">The AssetManager</param>
        public override void SetAssetManager(AssetManager manager)
        {
            Shot.SetAssetManager(manager);
            base.SetAssetManager(manager);
        }

        /// <summary>
        /// Call to set the graphics device
        /// </summary>
        /// <param name="device">The GraphicsDevice</param>
        public override void SetGraphicsDevice(GraphicsDevice device)
        {
            Shot.SetGraphicsDevice(device);
            base.SetGraphicsDevice(device);
        }

        /// <summary>
        /// Call to set the render sprite
        /// </summary>
        /// <param name="render">Do you really need to ask?</param>
        public override void SetRenderSprite(RenderSprite render)
        {
            Shot.SetRenderSprite(render);
            base.SetRenderSprite(render);
        }
        #endregion
    }
}
