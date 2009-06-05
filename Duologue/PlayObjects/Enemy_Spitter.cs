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
using Duologue.Audio;
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

        private const string filename_SpitAttack = "Audio/PlayerEffects/spit-attack";
        private const float volume_SpitAttack = 0.15f;
        private const string filename_Explode = "Audio/PlayerEffects/splat-explode-short";
        private const float volume_Explode = 0.35f;

        private const float spitSpeed = 3.5f;
        private const float minSplatterSpeed = spitSpeed / 2f;
        private const float maxSplatterSpeed = spitSpeed;

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
        /// The time per frame of animation while we're firing
        /// </summary>
        private const double timePerFrameFiring = 0.1;

        /// <summary>
        /// The inclusive upper bounds for the animation frames we do when just standing around waiting
        /// </summary>
        private const int maxWaitingFrame = 1;

        private const double minFiringTime = 2.0;
        private const double maxFiringTime = 4.0;

        /// <summary>
        /// The maximum spit dropping alpha
        /// </summary>
        private const float maxSpitDroppingAlpha = 0.4f;

        /// <summary>
        /// The minimum splatter radius change
        /// </summary>
        private const double minSplatterAngleChange = (double)MathHelper.PiOver4/2.0;

        /// <summary>
        /// The maximum splatter radius change
        /// </summary>
        private const double maxSplatterAngleChange = (double)MathHelper.PiOver2;

        /// <summary>
        /// The time it takes to splatter
        /// </summary>
        private const double totalSplatterTime = 0.4;

        /// <summary>
        /// The minimum starting size of spit droppings
        /// </summary>
        private const double minStartingSpitDroppingSize = 0.8;

        /// <summary>
        /// The maximum starting size of spit droppings
        /// </summary>
        private const double maxStartingSpitDroppingSize = 1.5;

        /// <summary>
        /// The ending spit dropping size
        /// </summary>
        private const float endingSpitDroppingSize = 0.1f;

        /// <summary>
        /// The number of frames for a given spit dropping to evaporate (and thus clear said dropping for reuse)
        /// </summary>
        private const int numberOfFramesForSpitDroppingEvaporate = 10;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 40;

        /// <summary>
        /// The multiplier for point value tweaks based upon hitpoints
        /// </summary>
        private const int hitPointMultiplier = 2;

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

        /// <summary>
        /// The position of the spit
        /// </summary>
        private Vector2 spitPosition;
        /// <summary>
        /// The radius of the spit
        /// </summary>
        private float spitRadius;
        /// <summary>
        /// Vector pointing where the spit is going
        /// </summary>
        private Vector2 spitDirection;
        /// <summary>
        /// Spit rotations per frame
        /// </summary>
        private float[] spitRotation;
        /// <summary>
        /// Array containing positions of the current spit droppings
        /// </summary>
        private Vector2[] spitDroppingPositions;
        /// <summary>
        /// After we splat, we want some blips to go off in various directions
        /// </summary>
        private Vector2[] spitDroppingDirections;
        /// <summary>
        /// Array containing the sizes of the current spit droppings
        /// </summary>
        private float[] spitDroppingSizes;
        /// <summary>
        /// Array containing the size deltas for the spit droppings
        /// </summary>
        private float[] spitDroppingSizeDeltas;
        /// <summary>
        /// The frames used for the droppings
        /// </summary>
        private int[] spitDroppingFrames;
        /// <summary>
        /// The starting position of the spit
        /// </summary>
        private Vector2 spitStartPosition;
        /// <summary>
        /// The ending position of the spit
        /// </summary>
        private Vector2 spitEndPosition;
        /// <summary>
        /// The length the spit will travel
        /// </summary>
        private float spitTravelLength;
        /// <summary>
        /// Whether the spit is alive
        /// </summary>
        private bool spitAlive;
        /// <summary>
        /// True if the spit is at its destination and is splatting
        /// </summary>
        private bool spitSplatting;
        /// <summary>
        /// The time since the splatter started
        /// </summary>
        private double splatterTime;
        /// <summary>
        /// Array containing the current spit alphas
        /// </summary>
        private byte[] currentSpitAlphas;
        /// <summary>
        /// Array containing the current spit alpha deltas
        /// </summary>
        private int[] spitAlphaDeltas;

        private Vector2 screenCenter;

        private int currentFrame;
        /// <summary>
        /// The direction (1, or -1) our animation is going
        /// </summary>
        private int animationDirection;

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
        private PlayObject nearestPlayerObject;

        private bool isFleeing;

        private AudioManager audio;
        private SoundEffect sfx_SpitAttack;
        private SoundEffectInstance sfxi_SpitAttack;
        private SoundEffect sfx_Explode;
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
        /// <summary>
        /// The empty constructor for pre-caching
        /// </summary>
        public Enemy_Spitter() : base() { }

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
            audio = ServiceLocator.GetService<AudioManager>();

            spitAlive = false;
            spitSplatting = false;

            LoadAndInitialize();
        }

        public override String[] GetSFXFilenames()
        {
            return new String[]
            {
                filename_SpitAttack
            };
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
            spitRotation = new float[maxSpitFrames];

            // Spit droppings
            spitDroppingFrames = new int[maxSpitFrames];
            spitDroppingPositions = new Vector2[maxSpitFrames];
            spitDroppingSizeDeltas = new float[maxSpitFrames];
            spitDroppingSizes = new float[maxSpitFrames];
            spitDroppingDirections = new Vector2[maxSpitFrames];

            // Audio
            sfx_SpitAttack = InstanceManager.AssetManager.LoadSoundEffect(filename_SpitAttack);
            sfxi_SpitAttack = null;
            sfx_Explode = InstanceManager.AssetManager.LoadSoundEffect(filename_Explode);

            // Load the animation frames
            for (int i = 0; i < maxAnimationFrames; i++)
            {
                textureBase[i] = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_base, (i + 1).ToString()));
                textureOutline[i] = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_outline, (i + 1).ToString()));
                frameCenters[i] = new Vector2(textureOutline[i].Width / 2f, textureOutline[i].Height / 2f);
            }

            spitRadius = 0f;

            // Load the spit frames
            for (int i = 0; i < maxSpitFrames; i++)
            {
                textureSpit[i] = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_spit, (i + 1).ToString()));
                spitCenters[i] = new Vector2(textureSpit[i].Width / 2f, textureSpit[i].Height / 2f);
                if (spitCenters[i].Length() > spitRadius)
                    spitRadius = spitCenters[i].Length();
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

            CheckScreenBoundary();

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

        /// <summary>
        /// Fire spit at the player
        /// </summary>
        private void FireSpit(GameTime gameTime)
        {
            // Set our starting and ending positional data
            spitStartPosition = Position + Vector2.Normalize(Orientation) * (Radius + spitRadius);
            spitEndPosition = nearestPlayerObject.Position + Vector2.Normalize(Orientation) * Radius;
            spitTravelLength = (spitStartPosition - spitEndPosition).Length();
            spitDirection = Vector2.Negate(Vector2.Normalize(spitStartPosition - spitEndPosition));
            spitPosition = spitStartPosition;

            // Set up our alphas
            GenerateSpitAlphas();
            for (int i = 0; i < maxSpitFrames; i++)
            {
                GenerateSpitDropping(i, MWMathHelper.GetRandomInRange(0, maxSpitFrames), spitPosition);
                spitRotation[i] = (float)MWMathHelper.GetRandomInRange(0.0, (double)MathHelper.TwoPi);
            }

            spitSplatting = false;
            spitAlive = true;
            if (sfxi_SpitAttack == null)
            {
                try
                {
                    sfxi_SpitAttack = sfx_SpitAttack.Play(volume_SpitAttack);
                }
                catch { }
            }
            else
            {
                try
                {
                    sfxi_SpitAttack.Play();
                }
                catch { }
            }
        }

        /// <summary>
        /// Generates a new spit dropping
        /// </summary>
        /// <param name="i">The index of the spit dropping</param>
        /// <param name="p">The index of the spit frame to use</param>
        /// <param name="position">The position of this spit dropping</param>
        private void GenerateSpitDropping(int i, int p, Vector2 position)
        {
            spitDroppingFrames[i] = p;
            spitDroppingPositions[i] = position;
            spitDroppingSizes[i] = (float)MWMathHelper.GetRandomInRange(minStartingSpitDroppingSize, maxStartingSpitDroppingSize);
            spitDroppingSizeDeltas[i] =
                (spitDroppingSizes[i] - endingSpitDroppingSize) / numberOfFramesForSpitDroppingEvaporate;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Public Overrides
        public override string[] GetTextureFilenames()
        {
            String[] filenames = new String[2 * maxAnimationFrames + maxSpitFrames + 1];

            int t = 0;
            filenames[t] = filename_spawnExplode;
            t++;

            for (int i = 0; i < maxAnimationFrames; i++)
            {
                filenames[t] = String.Format(filename_base, (i+1).ToString());
                t++;
                filenames[t] = String.Format(filename_outline, (i+1).ToString());
                t++;
            }

            for (int i = 0; i < maxSpitFrames; i++)
            {
                filenames[t] = String.Format(filename_spit, (i+1).ToString());
                t++;
            }

            return filenames;
        }

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
                    nearestPlayerObject = pobj;
                }
                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em
                    if(MyState != SpitterState.Spawning)
                        return pobj.TriggerHit(this);
                }

                // Beam handling
                if (((Player)pobj).IsInBeam(this) == -1)
                {
                    //isFleeing = true;
                    LocalInstanceManager.Steam.AddParticles(Position, GetMyColor());
                }

                // Spit handling
                if (spitAlive && !spitSplatting)
                {
                    vToPlayer = spitPosition - pobj.Position;
                    if (vToPlayer.Length() < spitRadius + pobj.Radius)
                    {
                        return pobj.TriggerHit(this);
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
                Orientation = Vector2.Negate(nearestPlayer);
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
                    nearestPlayer.Normalize();

                    if (!isFleeing)
                        nearestPlayer = Vector2.Negate(nearestPlayer);

                    offset += modifier * nearestPlayer;
                }
            }

            // Next apply the offset permanently
            if (offset.Length() >= minMovement)
            {
                this.Position += offset;
            }

            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.PlayerBullet && MyState != SpitterState.Spawning)
            {
                Color c = GetMyColor();

                CurrentHitPoints--;
                if (CurrentHitPoints <= 0)
                {
                    LocalInstanceManager.EnemySplatterSystem.AddParticles(Position, c);
                    Alive = false;
                    LocalInstanceManager.AchievementManager.EnemyDeathCount(MyType);
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue + hitPointMultiplier * StartHitPoints, Position);
                    sfx_Explode.Play(volume_Explode);
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(textureSpawnExplode, c, Position, 0f);
                    audio.PlayEffect(EffectID.CokeBottle);
                    return true;
                }
            }
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

        private void DrawStandard(GameTime gameTime)
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

        private void DrawSpit(GameTime gameTime)
        {
            if(spitAlive)
                if(!spitSplatting)
                {
                    for (int i = 0; i < maxSpitFrames; i++)
                    {
                        // Start with the spit itself
                        InstanceManager.RenderSprite.Draw(
                            textureSpit[i],
                            spitPosition,
                            spitCenters[i],
                            null,
                            new Color(Color.White, currentSpitAlphas[i]),
                            spitRotation[i],
                            1f,
                            0f,
                            RenderSpriteBlendMode.AlphaBlend);

                        float j = maxSpitDroppingAlpha * (spitDroppingSizes[i] / ((float)maxStartingSpitDroppingSize - endingSpitDroppingSize));
                        if(j < 0)
                            j = 0f;
                        else if(j > 1)
                            j = 1.0f;
                        // Next, do the droppings
                        InstanceManager.RenderSprite.Draw(
                            textureSpit[spitDroppingFrames[i]],
                            spitDroppingPositions[i],
                            spitCenters[spitDroppingFrames[i]],
                            null,
                            new Color(Color.White, j),
                            spitRotation[i],
                            spitDroppingSizes[i],
                            0f,
                            RenderSpriteBlendMode.Addititive);
                    }
                } else
                {
                    for (int i = 0; i < maxSpitFrames; i++)
                    {
                        float k = (float)(splatterTime / totalSplatterTime);
                        if (k < 0)
                            k = 0f;
                        else if (k > 1)
                            k = 1.0f;

                        float alpha = maxSpitDroppingAlpha * k;
                        float size = (float)maxStartingSpitDroppingSize -
                            k * ((float)maxStartingSpitDroppingSize - endingSpitDroppingSize);

                        // Next, do the droppings
                        InstanceManager.RenderSprite.Draw(
                            textureSpit[spitDroppingFrames[i]],
                            spitDroppingPositions[i],
                            spitCenters[spitDroppingFrames[i]],
                            null,
                            new Color(Color.White, alpha),
                            spitRotation[i],
                            size,
                            0f,
                            RenderSpriteBlendMode.Addititive);

                    }
                }

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
            if (timeSinceStart > timeToNextFrame)
            {
                timeToNextFrame = timeSinceStart + timePerFrameFiring;
                currentFrame += animationDirection;
                if (currentFrame >= maxAnimationFrames)
                {
                    currentFrame = maxAnimationFrames - 1;
                    animationDirection = -1;
                    if(nearestPlayerObject != null)
                        FireSpit(gameTime);
                }
                else if (currentFrame < 0)
                {
                    timeSinceStart = 0.0;
                    timeToNextFire = MWMathHelper.GetRandomInRange(minFiringTime, maxFiringTime);
                    timeToNextFrame = timePerFrameWaiting;
                    currentFrame = 0;
                    MyState = SpitterState.WaitingToFire;
                }
            }
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

            if (timeSinceStart > timeToNextFire && ! spitAlive && nearestPlayerObject != null)
            {
                timeSinceStart = 0.0;
                timeToNextFrame = timePerFrameFiring;
                currentFrame = 0;
                animationDirection = 1;
                MyState = SpitterState.Firing;
            }
        }


        private void SpitUpdate(GameTime gameTime)
        {
            if (!spitSplatting)
            {
                // Move the spit
                spitPosition += spitSpeed * spitDirection;

                // See if we're at our target or not
                if ((spitStartPosition - spitPosition).Length() >= spitTravelLength)
                {
                    spitSplatting = true;
                    splatterTime = 0.0;
                    for (int i = 0; i < maxSpitFrames; i++)
                    {
                        GenerateSpitDropping(i, MWMathHelper.GetRandomInRange(0, maxSpitFrames), spitPosition);
                        spitRotation[i] = (float)MWMathHelper.GetRandomInRange(0.0, (double)MathHelper.TwoPi);
                        float k = (float)MWMathHelper.GetRandomInRange(minSplatterAngleChange, maxSplatterAngleChange);
                        if(MWMathHelper.CoinToss())
                            k = k * -1;
                        spitDroppingDirections[i] =
                            MWMathHelper.RotateVectorByRadians(spitDirection, k);
                        spitDroppingDirections[i].Normalize();
                        spitDroppingDirections[i] = spitDroppingDirections[i] *
                            (float)MWMathHelper.GetRandomInRange(minSplatterSpeed, maxSplatterSpeed);
                    }
                }
                else
                {
                    // Update the droppings and alphas
                    for (int i = 0; i < maxSpitFrames; i++)
                    {
                        // Handle the main spit glob's alphas
                        int j = currentSpitAlphas[i] + spitAlphaDeltas[i];
                        if (j < lowerSpitAlpha)
                            j = lowerSpitAlpha;
                        else if (j > upperSpitAlpha)
                            j = upperSpitAlpha;
                        currentSpitAlphas[i] = (byte)j;

                        // Handle the droplets
                        spitDroppingSizes[i] -= spitDroppingSizeDeltas[i];
                        if (spitDroppingSizes[i] < endingSpitDroppingSize)
                            GenerateSpitDropping(i, MWMathHelper.GetRandomInRange(0, maxSpitFrames), spitPosition);
                    }
                }
            }
            else
            {
                // We're splatting
                splatterTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (splatterTime > totalSplatterTime)
                {
                    spitSplatting = false;
                    spitAlive = false;
                }
                else
                {
                    for (int i = 0; i < maxSpitFrames; i++)
                    {
                        spitDroppingPositions[i] += spitDroppingDirections[i];
                    }
                }
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
                default:
                    DrawStandard(gameTime);
                    break;
            }
            DrawSpit(gameTime);
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

            // Update the spit droppings
            if (spitAlive)
            {
                SpitUpdate(gameTime);
            }

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
