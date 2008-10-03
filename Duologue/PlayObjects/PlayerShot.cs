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
    public class PlayerShot : PlayObject
    {
        #region Constants
        private const int numShots = 10;
        private const int timeBetweenShots = 8; // Frames
        private const int shotScaler = 10;
        #endregion

        #region Fields
        private SpriteObject[] shots;
        private int shotTimer;
        private Random rand;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public PlayerShot()
            : base()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (AssetManager == null)
                AssetManager = InstanceManager.AssetManager;
            shotTimer = timeBetweenShots;
            rand = new Random();
            shots = new SpriteObject[numShots];
            for (int i = 0; i < numShots; i++)
            {
                shots[i] = new SpriteObject(
                    AssetManager.LoadTexture2D("shot"),
                    Vector2.Zero,
                    new Vector2(AssetManager.LoadTexture2D("shot").Width / 2f, AssetManager.LoadTexture2D("shot").Height / 2f),
                    null,
                    Color.White,
                    0f,
                    1f,
                    0.5f);
                shots[i].Alive = false;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Ensure that we're still inside screenboundaries.
        /// </summary>
        private bool IsInScreen(Vector2 Position, Vector2 Dimensions)
        {
            if (Position.X > InstanceManager.DefaultViewport.Width + Dimensions.X / 2f ||
                Position.X < Dimensions.X / -2f ||
                Position.Y > InstanceManager.DefaultViewport.Height + Dimensions.Y / 2f ||
                Position.Y < Dimensions.Y / -2f)
                return false;
            else
                return true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Call when requesting we fire a shot in a given direction.
        /// Will only fire if there is a shot available, and a certain
        /// shot timer has elapsed.
        /// </summary>
        /// <param name="Aim">The direction to fire in</param>
        /// <param name="Tint">The tint of the blast</param>
        /// <param name="StartPos">The start position of the blast</param>
        internal void Fire(Vector2 Aim, Color Tint, Vector2 StartPos)
        {
            if (shotTimer >= timeBetweenShots)
            {
                for (int i = 0; i < shots.Length; i++)
                {
                    if (!shots[i].Alive)
                    {
                        shots[i].Alive = true;
                        shots[i].Rotation = (float)rand.NextDouble();
                        shots[i].Tint = Tint;
                        shots[i].Position = StartPos;
                        shots[i].Direction = Aim;
                        shotTimer = 0;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Draw the shots. Call once per frame.
        /// </summary>
        /// <param name="gameTime">The gametime</param>
        internal void Draw(GameTime gameTime)
        {
            if (RenderSprite == null)
                RenderSprite = InstanceManager.RenderSprite;
            for (int i = 0; i < shots.Length; i++)
            {
                if (shots[i].Alive)
                    RenderSprite.Draw(shots[i], RenderSpriteBlendMode.Addititive);
            }
        }

        /// <summary>
        /// Called once per frame
        /// </summary>
        /// <param name="gameTime"></param>
        internal void Update(GameTime gameTime)
        {
            if (shotTimer < timeBetweenShots)
                shotTimer++;
            for (int i = 0; i < shots.Length; i++)
            {
                if (shots[i].Alive)
                {
                    shots[i].Position += shotScaler * Vector2.Normalize(shots[i].Direction);
                    shots[i].Alive = IsInScreen(shots[i].Position,
                        new Vector2(shots[i].Texture.Width, shots[i].Texture.Height));
                }
            }
        }
        #endregion

    }
}
