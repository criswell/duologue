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
            WaitingToFire,
            Firing,
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

        private const float startSpawnScale = 4f;
        private const float endSpawnScale = 0.7f;
        private const float deltaSpawnScale = -0.05f;
        private const float maxOpacity = 255f;

        private const float radiusMultiplier = 0.35f;

        private const float maxShadowOffset = 10f;

        /// <summary>
        /// The time per frame of animation while we're waiting to fire
        /// </summary>
        private const double timePerFrameWaiting = 0.2;

        /// <summary>
        /// The inclusive upper bounds for the animation frames we do when just standing around waiting
        /// </summary>
        private const int maxWaitingFrame = 1;

        private const double minFiringTime = 2.0;
        private const double maxFiringTime = 4.0;

        #region Forces/Attractions/Repulsions
        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        /// <summary>
        /// The minimum movement required before we register motion
        /// </summary>
        private const float minMovement = 2f;

        /// <summary>
        /// The player attract modifier
        /// </summary>
        private const float playerAttract = 2.5f;

        /// <summary>
        /// The repulsion from the player if the player gets too close
        /// </summary>
        private const float playerRepluse = 3.5f;

        /// <summary>
        /// Min number of how many of our radius size away we're comfortable with the player
        /// </summary>
        private const float minPlayerComfortRadiusMultiplier = 14f;

        /// <summary>
        /// Max number of our radius we're comfortable with the player
        /// </summary>
        private const float maxPlayerComfortRadiusMultiplier = 14.5f;
        #endregion
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

        /*private float upperLeftAngle;
        private float upperRightAngle;
        private float lowerRightAngle;
        private float lowerLeftAngle;

        private Vector2 upperLeftCorner;
        private Vector2 upperRightCorner;
        private Vector2 lowerRightCorner;
        private Vector2 lowerLeftCorner;*/

        private float upperBoundaryX;
        private float lowerBoundaryX;
        private float upperBoundaryY;
        private float lowerBoundaryY;

        private float innerBoundsWidth;
        private float innerBoundsHeight;

        private float rotation;

        private double timeSinceStart;
        private double timeToNextFire;
        private double timeToNextFrame;

        private float spawnScale;
        private float spawnCalc_m = 1f / (endSpawnScale - startSpawnScale);
        private float spawnCalc_h = -1f * startSpawnScale / (endSpawnScale - startSpawnScale);

        private Vector2 shadowOffset;
        private Color shadowColor;

        private Vector2 offset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;

        private bool inBeam;
        private bool isFleeing;
        #endregion

        #region Properties
        public SpitterState MyState;

        /// <summary>
        /// Returns a percentage complete for spawn crosshair
        /// </summary>
        public float SpawnCrosshairPercentage
        {
            get
            {
                float p = spawnCalc_m * spawnScale + spawnCalc_h;
                if (p > 1f)
                    return 1f;
                else if (p < 0f)
                    return 0f;
                else
                    return p;
            }
        }
        #endregion

        #region Constructor / Init
        public Enemy_Spitter(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Spitter;
            MajorType = MajorPlayObjectType.Enemy;
            RealSize = new Vector2(63, 56);
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

            Radius = RealSize.Length() * radiusMultiplier;

            // Compute orientation
            Orientation = GetStartingVector();
            SetRotation();

            currentFrame = MWMathHelper.GetRandomInRange(0, maxWaitingFrame+1);
            screenCenter = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f,
                InstanceManager.DefaultViewport.Height / 2f);

            shadowColor = new Color(Color.Black, 200);
            ComputeShadowOffset();

            // Determine the corners and their angles
            // One thing to remember here is we're actually flipped upside down due to
            // graphic conventions of y increasing from top to bottom (messes things up for
            // the trig functions, which assume otherwise)
            upperBoundaryX = InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent;
            upperBoundaryY = InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent;

            lowerBoundaryX = InstanceManager.DefaultViewport.Width - upperBoundaryX;
            lowerBoundaryY = InstanceManager.DefaultViewport.Height - upperBoundaryY;

            innerBoundsWidth = upperBoundaryX - lowerBoundaryX;
            innerBoundsHeight = upperBoundaryY - lowerBoundaryY;

            /*lowerLeftCorner = new Vector2(lowerBoundaryX, lowerBoundaryY);
            lowerRightCorner = new Vector2(upperBoundaryX, lowerBoundaryY);

            upperRightCorner = new Vector2(upperBoundaryX, upperBoundaryY);
            upperLeftCorner = new Vector2(lowerBoundaryX, upperBoundaryY);

            upperLeftAngle = MWMathHelper.ComputeAngleAgainstX(upperLeftCorner, screenCenter);
            upperRightAngle = MWMathHelper.ComputeAngleAgainstX(upperRightCorner, screenCenter);
            lowerLeftAngle = MWMathHelper.ComputeAngleAgainstX(lowerLeftCorner, screenCenter);
            lowerRightAngle = MWMathHelper.ComputeAngleAgainstX(lowerRightCorner, screenCenter);*/

            // Place us at the max position
            //SetAtMaxPosition();
            //Console.WriteLine(String.Format("Pre: {0}", Position.ToString()));
            CheckScreenBoundary();
            //Console.WriteLine(String.Format("Post: {0}", Position.ToString()));

            spawnScale = startSpawnScale;

            MyState = SpitterState.Spawning;
            timeSinceStart = 0.0;
            timeToNextFire = MWMathHelper.GetRandomInRange(minFiringTime, maxFiringTime);
            timeToNextFrame = timePerFrameWaiting;

            Initialized = true;
            Alive = true;
        }

        private void SetRotation()
        {
            rotation = MWMathHelper.ComputeAngleAgainstX(Orientation) + MathHelper.PiOver2;
        }

        /// <summary>
        /// Ensure that we're still inside screenboundaries.
        /// </summary>
        private void CheckScreenBoundary()
        {
            if (Position.X > upperBoundaryX)
                Position.X = upperBoundaryX;
            if (Position.X < lowerBoundaryX)
                Position.X = lowerBoundaryX;

            if (Position.Y > upperBoundaryY)
                Position.Y = upperBoundaryY;
            if (Position.Y < lowerBoundaryY)
                Position.Y = lowerBoundaryY;
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

        /* FUCK THIS SHIT
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
            //Console.WriteLine(String.Format("Pre: {0}", Position.ToString()));
            Position.X = x;
            Position.Y = y;
            //Console.WriteLine(String.Format("{0},{1} - {2}", x.ToString(), y.ToString(), Position.ToString()));
        }*/

        /// <summary>
        /// Returns a vector pointing to the origin
        /// </summary>
        private Vector2 GetVectorPointingAtOrigin()
        {
            Vector2 sc = new Vector2(
                    InstanceManager.DefaultViewport.Width / 2f,
                    InstanceManager.DefaultViewport.Height / 2f);
            return GetVectorPointingAtOrigin(sc);
        }

        /// <summary>
        /// Given a vector sc, point at it
        /// </summary>
        private Vector2 GetVectorPointingAtOrigin(Vector2 sc)
        {
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

        /// <summary>
        /// Figure out the current treadoffset
        /// </summary>
        private void ComputeShadowOffset()
        {
            // Get distance
            float distance = Vector2.Subtract(screenCenter, Position).Length();

            // Compute the size of the offset based on distance
            float size = maxShadowOffset * (distance / screenCenter.Length());

            // Aim at center of screen
            shadowOffset = Vector2.Add(screenCenter, Position);
            shadowOffset.Normalize();
            //shadowOffset = Vector2.Negate(shadowOffset);
            shadowOffset *= size;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Public Overrides
        public override bool StartOffset()
        {
            offset = Vector2.Zero;
            nearestPlayerRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            nearestPlayer = Vector2.Zero;
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Player)
            {
                // Player
                Vector2 vToPlayer = this.Position - pobj.Position;
                float len = vToPlayer.Length();
                if (len < nearestPlayerRadius)
                {
                    nearestPlayerRadius = len;
                    nearestPlayer = vToPlayer;
                }
                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em
                    return pobj.TriggerHit(this);
                }

                // Beam handling
                int temp = ((Player)pobj).IsInBeam(this);
                inBeam = false;
                //isFleeing = false;
                if (temp != 0)
                {
                    inBeam = true;
                    if (temp == -1)
                    {
                        //isFleeing = true;
                        LocalInstanceManager.Steam.AddParticles(Position, GetMyColor());
                    }
                }
                return true;
            }
            else if (pobj.MajorType == MajorPlayObjectType.Enemy)
            {
                // Enemy
                Vector2 vToEnemy = pobj.Position - this.Position;
                float len = vToEnemy.Length();
                if (len < this.Radius + pobj.Radius)
                {
                    // Too close, BTFO
                    if (len == 0f)
                    {
                        // Well, bah, we're on top of each other!
                        vToEnemy = new Vector2(
                            (float)InstanceManager.Random.NextDouble() - 0.5f,
                            (float)InstanceManager.Random.NextDouble() - 0.5f);
                    }
                    vToEnemy = Vector2.Negate(vToEnemy);
                    vToEnemy.Normalize();
                    //InstanceManager.Logger.LogEntry(String.Format("Pre {0}", offset));
                    offset += standardEnemyRepulse * vToEnemy;
                    //InstanceManager.Logger.LogEntry(String.Format("Post {0}", offset));
                }

                return true;
            }
            else
            {
                // Other

                return true;
            }
        }

        public override bool ApplyOffset()
        {
            // First, apply the player offset
            if (nearestPlayer.Length() > 0f)
            {
                // FIXME would be nice if this was more of a "turn" than a sudden jarring switch
                Orientation = Vector2.Negate(nearestPlayer); //GetVectorPointingAtOrigin(nearestPlayer);
                SetRotation();
                if (!(nearestPlayerRadius > minPlayerComfortRadiusMultiplier * Radius &&
                    nearestPlayerRadius < maxPlayerComfortRadiusMultiplier * Radius))
                {
                    // Default to attract, only repulse if too close
                    float modifier = playerAttract;
                    isFleeing = false;
                    if (nearestPlayerRadius < minPlayerComfortRadiusMultiplier * Radius)
                    {
                        // Too close, BTFO
                        modifier = playerRepluse;
                        isFleeing = true;
                    }

                    nearestPlayer += new Vector2(nearestPlayer.Y, -nearestPlayer.X);
                    //Orientation = nearestPlayer;
                    nearestPlayer.Normalize();

                    if (!isFleeing)
                        nearestPlayer = Vector2.Negate(nearestPlayer);

                    offset += modifier * nearestPlayer;
                }
            }
            /*else
            {
                // If no near player, move in previous direction
                nearestPlayer = lastDirection;

                //nearestPlayer += new Vector2(nearestPlayer.Y, -nearestPlayer.X);
                nearestPlayer.Normalize();

                offset += playerAttract * nearestPlayer;
            }*/

            // Next apply the offset permanently
            if (offset.Length() >= minMovement)
            {
                this.Position += offset;
                //lastDirection = offset;
                //Orientation = offset; // new Vector2(-offset.Y, offset.X);
                //Orientation.Normalize();
            }

            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            return true;
        }
        #endregion

        #region Private Draw Methods
        private void DrawSpawning(GameTime gameTime)
        {
            Color c = GetMyColor();
            c = new Color(c, (byte)(SpawnCrosshairPercentage * maxOpacity));
            //Console.WriteLine(SpawnCrosshairPercentage.ToString());
            InstanceManager.RenderSprite.Draw(
                textureSpawnExplode,
                Position,
                spawnExplodeCenter,
                null,
                c,
                rotation,
                spawnScale,
                0f);
        }

        private void DrawWaitingToFire(GameTime gameTime)
        {
            Color c = GetMyColor();
            // Draw shadow
            InstanceManager.RenderSprite.Draw(
                textureBase[currentFrame],
                Position + shadowOffset,
                frameCenters[currentFrame],
                null,
                shadowColor,
                rotation,
                1f,
                0f);

            // Draw base
            InstanceManager.RenderSprite.Draw(
                textureBase[currentFrame],
                Position,
                frameCenters[currentFrame],
                null,
                c,
                rotation,
                1f,
                0f);

            // Draw Outline
            InstanceManager.RenderSprite.Draw(
                textureOutline[currentFrame],
                Position,
                frameCenters[currentFrame],
                null,
                Color.White,
                rotation,
                1f,
                0f);
        }

        private void DrawFiring(GameTime gameTime)
        {
        }
        #endregion

        #region Private Update Methods
        private void UpdateSpawning(GameTime gameTime)
        {
            spawnScale += deltaSpawnScale;
            if (spawnScale < endSpawnScale)
            {
                MyState = SpitterState.WaitingToFire;
                spawnScale = endSpawnScale;
                timeSinceStart = 0.0;
                timeToNextFire = MWMathHelper.GetRandomInRange(minFiringTime, maxFiringTime);
                timeToNextFrame = timePerFrameWaiting;
            }
        }

        private void UpdateFiring(GameTime gameTime)
        {
            // For now, we just do nothing but return to waiting to fire state
            // FIXME
            spawnScale = endSpawnScale;
            timeSinceStart = 0.0;
            timeToNextFire = MWMathHelper.GetRandomInRange(minFiringTime, maxFiringTime);
            timeToNextFrame = timePerFrameWaiting;
            currentFrame = 0;
            MyState = SpitterState.WaitingToFire;
        }

        private void UpdateWaitingToFire(GameTime gameTime)
        {
            if (timeSinceStart > timeToNextFrame)
            {
                currentFrame++;
                if (currentFrame > maxWaitingFrame)
                    currentFrame = 0;
                timeToNextFrame = timeSinceStart + timePerFrameWaiting;
            }

            if (timeSinceStart > timeToNextFire)
            {
                timeSinceStart = 0.0;
                // FIXME, remove the following when we've a firing update in place
                timeToNextFire = MWMathHelper.GetRandomInRange(minFiringTime, maxFiringTime);
                timeToNextFrame = timePerFrameWaiting;
                // FIXME, uncomment the following when we've a firing update in place
                //currentFrame = 0;
                //MyState = SpitterState.Firing;
            }
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            ComputeShadowOffset();
            switch (MyState)
            {
                case SpitterState.Spawning:
                    DrawSpawning(gameTime);
                    break;
                case SpitterState.Firing:
                    DrawFiring(gameTime);
                    break;
                default:
                    // Waiting to fire
                    DrawWaitingToFire(gameTime);
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceStart += gameTime.ElapsedGameTime.TotalSeconds;

            /*if (Position.X < frameCenters[currentFrame].X
                || Position.X > InstanceManager.DefaultViewport.Width - frameCenters[currentFrame].X
                || Position.Y < frameCenters[currentFrame].Y
                || Position.Y > InstanceManager.DefaultViewport.Height - frameCenters[currentFrame].Y)
            {
                SetAtMaxPosition();
            }*/
            CheckScreenBoundary();

            switch (MyState)
            {
                case SpitterState.Spawning:
                    UpdateSpawning(gameTime);
                    break;
                case SpitterState.Firing:
                    UpdateFiring(gameTime);
                    break;
                default:
                    // Waiting to fire
                    UpdateWaitingToFire(gameTime);
                    break;
            }
        }
        #endregion
    }
}
