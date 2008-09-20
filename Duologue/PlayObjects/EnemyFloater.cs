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
using Duologue.State;

namespace Duologue.PlayObjects
{
    class FloaterObject : SpriteObject
    {
        public Color EyeTint;
        public int Speed;

        public FloaterObject(
            Texture2D texture2D,
            Vector2 texturePosition,
            Vector2 textureCenter,
            Rectangle? textureRect,
            Color textureTint,
            float textureRotation,
            float textureScale,
            float textureLayer) :
            base(texture2D, texturePosition, textureCenter, textureRect, textureTint, textureRotation, textureScale, textureLayer)
        {
            EyeTint = Color.White;
            Speed = 1;
        }
    }

    class EnemyFloater : PlayObject
    {
        #region Constants
        private const int defaultNumEnemies = 10;
        private const int minSpeed = 1;
        private const int maxSpeed = 5;
        private const int turnRadius = 100; // larger is slower
        #endregion

        #region Fields
        private FloaterObject[] enemies;
        private SpriteObject enemyEye;
        private int numEnemies;
        private Random rand;
        #endregion

        #region Properties
        public Player Player;
        public ColorState colorState;
        #endregion

        #region Constructor / Init
        public EnemyFloater(
            AssetManager manager,
            GraphicsDevice graphics,
            RenderSprite renderer,
            int numberEnemies,
            Player player,
            ColorState currentColorState)
            : base(manager, graphics, renderer)
        {
            numEnemies = numberEnemies;
            Player = player;
            colorState = currentColorState;
            Initialize();
        }

        public EnemyFloater()
            : base()
        {
            // FIXME: We need to get rid of this
            numEnemies = defaultNumEnemies;
            Initialize();
        }

        /// <summary>
        /// Initialize
        /// </summary>
        private void Initialize()
        {
            rand = new Random();
            enemies = new FloaterObject[numEnemies];
            for (int i = 0; i < numEnemies; i++)
            {
                enemies[i] = new FloaterObject(
                    AssetManager.LoadTexture2D("enemy-floater"),
                    Vector2.Zero,
                    new Vector2(AssetManager.LoadTexture2D("enemy-floater").Width / 2f, AssetManager.LoadTexture2D("enemy-floater").Height / 2f),
                    null,
                    Color.White,
                    0f,
                    1f,
                    0.5f);
                enemies[i].Alive = false;
            }

            enemyEye = new SpriteObject(
                    AssetManager.LoadTexture2D("enemy-floater-eye"),
                    Vector2.Zero,
                    new Vector2(AssetManager.LoadTexture2D("enemy-floater-eye").Width / 2f, AssetManager.LoadTexture2D("enemy-floater-eye").Height / 2f),
                    null,
                    Color.White,
                    0f,
                    1f,
                    0.5f);

            
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Call once per frame, will update all sprites and spawn new enemies
        /// </summary>
        /// <param name="gameTime">The current gametime</param>
        /// <param name="minMaxX">The minimum/maximum X values</param>
        /// <param name="minMaxY">The minimum/maximum Y values</param>
        public void Update(GameTime gameTime, Vector2 minMaxX, Vector2 minMaxY)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (!enemies[i].Alive)
                {
                    float x = rand.Next((int)minMaxX.X, (int)minMaxX.Y);
                    float y = rand.Next((int)minMaxY.X, (int)minMaxY.Y);
                    enemies[i].Alive = true;
                    enemies[i].Position = new Vector2(x, y);
                    if (rand.Next(2) == 0)
                    {
                        enemies[i].EyeTint = colorState.Positive[1];
                        enemies[i].Tint = colorState.Positive[0];
                    }
                    else
                    {
                        enemies[i].EyeTint = colorState.Negative[1];
                        enemies[i].Tint = colorState.Negative[0];
                    }
                    float dx = rand.Next(-10, 10);
                    float dy = rand.Next(-10, 10);
                    enemies[i].Direction = new Vector2(dx, dy);
                    enemies[i].Direction.Normalize();
                    enemies[i].Speed = rand.Next(minSpeed, maxSpeed);
                }
                else
                {
                    // Aim at the player
                    Vector2 vector2player = Player.Position - enemies[i].Position;
                    vector2player.Normalize();

                    enemies[i].Direction += turnRadius* vector2player;
                    enemies[i].Direction.Normalize();

                    // See if we're in the beam
                    int inBeam = Player.IsInBeam(enemies[i].Position, enemies[i].Tint);
                    //if (Player.IsInBeam(enemies[i].Position, enemies[i].Tint))
                    //    enemies[i].Direction = Vector2.Negate(enemies[i].Direction);

                    // Rotate
                    float dotDirection = Vector2.Dot(enemies[i].Direction, Vector2.UnitX);
                    enemies[i].Rotation = (float)Math.Acos((double)(dotDirection / 1f));
                    if (enemies[i].Direction.Y < 0)
                        enemies[i].Rotation *= -1;

                    enemies[i].Rotation += 3f * MathHelper.PiOver2;

                    // Update position
                    enemies[i].Position += enemies[i].Speed * enemies[i].Direction;
                    enemies[i].Speed = rand.Next(minSpeed, maxSpeed);
                    if (enemies[i].Position.X > GraphicsDevice.Viewport.Width - enemies[i].Texture.Width / 2f)
                    {
                        enemies[i].Position.X = GraphicsDevice.Viewport.Width - enemies[i].Texture.Width / 2f;
                        enemies[i].Direction.X *= -1;
                    }
                    else if (enemies[i].Position.X < enemies[i].Texture.Width / 2f)
                    {
                        enemies[i].Position.X = enemies[i].Texture.Width / 2f;
                        enemies[i].Direction.X *= -1;
                    }

                    if (enemies[i].Position.Y > GraphicsDevice.Viewport.Height - enemies[i].Texture.Height / 2f)
                    {
                        enemies[i].Position.Y = GraphicsDevice.Viewport.Height - enemies[i].Texture.Height / 2f;
                        enemies[i].Direction.Y *= -1;
                    }
                    else if (enemies[i].Position.Y < enemies[i].Texture.Height / 2f)
                    {
                        enemies[i].Position.Y = enemies[i].Texture.Height / 2f;
                        enemies[i].Direction.Y *= -1;
                    }
                }
            }
        }

        /// <summary>
        /// Draw the enemies alive
        /// </summary>
        /// <param name="gameTime">Current gameTime</param>
        internal void Draw(GameTime gameTime)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].Alive)
                {
                    RenderSprite.Draw(enemies[i]);
                    RenderSprite.Draw(new SpriteObject(
                        enemyEye.Texture,
                        enemies[i].Position,
                        enemyEye.Center,
                        null,
                        enemies[i].EyeTint,
                        enemies[i].Rotation,
                        enemies[i].Scale,
                        enemies[i].Layer));
                }
            }
        }
        #endregion
    }
}
