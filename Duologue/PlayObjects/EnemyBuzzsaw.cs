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
    class BuzzsawObject : SpriteObject
    {
        public Color FaceTint;
        public float Speed;
        public float SpeedMultiplier;
        public Texture2D FleeTexture;
        public bool Fleeing;

        public BuzzsawObject(
            Texture2D texture2D,
            Texture2D fleeTexture,
            Vector2 texturePosition,
            Vector2 textureCenter,
            Rectangle? textureRect,
            Color textureTint,
            float textureRotation,
            float textureScale,
            float textureLayer) :
            base(texture2D, texturePosition, textureCenter, textureRect, textureTint, textureRotation, textureScale, textureLayer)
        {
            FaceTint = Color.White;
            Speed = 0.5f;
            FleeTexture = fleeTexture;
            Fleeing = false;
            SpeedMultiplier = 1f;
        }
    }

    class EnemyBuzzsaw : PlayObject
    {
        #region Constants
        private const int defaultNumEnemies = 10;
        private const float minSpeed = 0.2f;
        private const float maxSpeed = 2.5f;
        private const int turnRadius = 100; // larger is slower
        #endregion

        #region Fields
        private BuzzsawObject[] enemies;
        private Texture2D enemyFaceAgg;
        private Texture2D enemyFaceFlee;
        private int numEnemies;
        private Random rand;
        #endregion

        #region Properties
        public Player Player;
        public ColorState colorState;
        #endregion

        #region Constructor / Init
        public EnemyBuzzsaw(
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

        /// <summary>
        /// Initialize
        /// </summary>
        private void Initialize()
        {
            rand = new Random();
            enemies = new BuzzsawObject[numEnemies];
            for (int i = 0; i < numEnemies; i++)
            {
                enemies[i] = new BuzzsawObject(
                    AssetManager.LoadTexture2D("buzzsaw-agg"),
                    AssetManager.LoadTexture2D("buzzsaw-flee"),
                    Vector2.Zero,
                    new Vector2(AssetManager.LoadTexture2D("buzzsaw-agg").Width / 2f, AssetManager.LoadTexture2D("buzzsaw-agg").Height / 2f),
                    null,
                    Color.White,
                    0f,
                    1f,
                    0.5f);
                enemies[i].Alive = false;
            }

            enemyFaceAgg = AssetManager.LoadTexture2D("buzzsaw-face-agg");
            enemyFaceFlee = AssetManager.LoadTexture2D("buzzsaw-face-flee");
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
                        enemies[i].FaceTint = colorState.Positive[1];
                        enemies[i].Tint = colorState.Positive[0];
                    }
                    else
                    {
                        enemies[i].FaceTint = colorState.Negative[1];
                        enemies[i].Tint = colorState.Negative[0];
                    }
                    float dx = rand.Next(-10, 10);
                    float dy = rand.Next(-10, 10);
                    enemies[i].Direction = new Vector2(dx, dy);
                    enemies[i].Direction.Normalize();
                    enemies[i].Speed = maxSpeed * (float)rand.NextDouble() + minSpeed;
                }
                else
                {
                    // Aim at the player
                    Vector2 vector2player = Player.Position - enemies[i].Position;
                    vector2player.Normalize();

                    enemies[i].Direction += turnRadius* vector2player;
                    enemies[i].Direction.Normalize();

                    // See if we're in the beam
                    //int inBeam = Player.IsInBeam(enemies[i].Position, enemies[i].Tint);
                    switch (Player.IsInBeam(enemies[i].Position, enemies[i].Tint))
                    {
                        case -1:
                            // RUN AWAY!
                            enemies[i].Direction = Vector2.Negate(enemies[i].Direction);
                            enemies[i].Fleeing = true;
                            enemies[i].SpeedMultiplier = 2f;
                            break;
                        case 0:
                            // Not in beam
                            enemies[i].Fleeing = false;
                            enemies[i].SpeedMultiplier = 1f;
                            break;
                        default:
                            // In beam, soak it up!
                            enemies[i].Fleeing = false;
                            enemies[i].SpeedMultiplier = 2f;
                            break;
                    }

                    // Rotate
                    enemies[i].Direction.Normalize();
                    enemies[i].Rotation = MWMathHelper.ComputeAngleAgainstX(enemies[i].Direction);
                    enemies[i].Rotation += 3f * MathHelper.PiOver2;

                    // Update position
                    enemies[i].Position += enemies[i].Speed * enemies[i].Direction; //rand.Next(minSpeed, maxSpeed) * 
                    enemies[i].Speed = enemies[i].SpeedMultiplier * (maxSpeed * (float)rand.NextDouble() + minSpeed);
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
                    if (enemies[i].Fleeing)
                    {
                        RenderSprite.Draw(
                            enemies[i].FleeTexture,
                            enemies[i].Position,
                            enemies[i].Center,
                            null,
                            enemies[i].Tint,
                            enemies[i].Rotation,
                            enemies[i].Scale,
                            enemies[i].Layer);
                        RenderSprite.Draw(
                            enemyFaceFlee,
                            enemies[i].Position,
                            enemies[i].Center,
                            null,
                            enemies[i].Tint,
                            0f,
                            enemies[i].Scale,
                            enemies[i].Layer);
                    }
                    else
                    {
                        RenderSprite.Draw(enemies[i]);
                        RenderSprite.Draw(
                            enemyFaceAgg,
                            enemies[i].Position,
                            enemies[i].Center,
                            null,
                            enemies[i].Tint,
                            0f,
                            enemies[i].Scale,
                            enemies[i].Layer);
                    }
                }
            }
        }
        #endregion
    }
}
