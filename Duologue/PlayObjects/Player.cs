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
        #endregion

        #region Constructor / Init
        public Player(AssetManager manager, GraphicsDevice graphics, RenderSprite renderer)
            : base(manager, graphics, renderer)
        {

            Initialize();
        }

        public Player()
            : base()
        {
            Initialize();
        }

        private void Initialize()
        {
            Orientation = Vector2.UnitX;
            Aim = Vector2.Negate(Orientation);
            CaclulateRotations();
            
            if (AssetManager != null && GraphicsDevice != null)
            {
                // Load player objects
                Position = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f);
                playerBase = new SpriteObject(
                    AssetManager.LoadTexture2D("player-base"),
                    Position,
                    new Vector2(AssetManager.LoadTexture2D("player-base").Width / 2f, AssetManager.LoadTexture2D("player-base").Height / 2f),
                    null,
                    Color.Teal,
                    0f,
                    1f,
                    0.5f);

                playerCannon = new SpriteObject(
                    AssetManager.LoadTexture2D("player-cannon"),
                    Position,
                    new Vector2(AssetManager.LoadTexture2D("player-cannon").Width / 2f, AssetManager.LoadTexture2D("player-cannon").Height / 2f),
                    null,
                    Color.Red,
                    0f,
                    1f,
                    0.4f);

                playerLight = new SpriteObject(
                    AssetManager.LoadTexture2D("player-light"),
                    Position,
                    new Vector2(AssetManager.LoadTexture2D("player-light").Width / 2f, AssetManager.LoadTexture2D("player-light").Height / 2f),
                    null,
                    Color.Blue,
                    0f,
                    1f,
                    0.4f);

                // Load projectile object
                shot = new SpriteObject(
                    AssetManager.LoadTexture2D("shot"),
                    Vector2.Zero,
                    new Vector2(AssetManager.LoadTexture2D("shot").Width / 2f, AssetManager.LoadTexture2D("shot").Height / 2f),
                    null,
                    Color.Red,
                    0f,
                    1f,
                    1f);

                shot.Alive = false;

                // Load beam object
                beam = new SpriteObject(
                    AssetManager.LoadTexture2D("beam"),
                    Position,
                    new Vector2(971f, 254f),
                    null,
                    new Color(Color.Blue.R, Color.Blue.G, Color.Blue.B, (byte)50),
                    0f,
                    1f,
                    1f);

            }
        }
        #endregion

        /// <summary>
        /// Draw the player object
        /// </summary>
        /// <param name="gameTime">Gametime</param>
        internal void Draw(GameTime gameTime)
        {
            CaclulateRotations();

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

            // Light
            RenderSprite.Draw(
                playerLight.Texture,
                Position,
                playerLight.Center,
                null,
                playerLight.Tint,
                LightRotation,
                1f,
                0.5f);

            // Lightbeam
            RenderSprite.Draw(
                beam.Texture,
                Position,
                beam.Center,
                null,
                beam.Tint,
                BeamRotation,
                1f,
                0.5f);

        }

        /// <summary>
        /// Calculate the various rotations, should be called once per frame
        /// </summary>
        private void CaclulateRotations()
        {
            // The base is easy because we can fuck it up- the base is a circle with
            // no real orientation.
            float dotOrientation = Vector2.Dot(Orientation, Vector2.UnitX);
            BaseRotation = (float)Math.Acos((double)(dotOrientation / Orientation.Length()));
            if (Orientation.Y < 0)
                BaseRotation *= -1;

            // The light rotation is a bit tricky because it starts in the left coordinate system
            LightRotation = BaseRotation +3f*MathHelper.PiOver4;

            // Next up, the light beam rotation is 180 degrees from the base
            BeamRotation = BaseRotation + MathHelper.Pi;
        }
    }
}
