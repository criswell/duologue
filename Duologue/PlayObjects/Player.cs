#region File Description
#endregion

#region Using Statements
// System
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
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
using Duologue.Audio;
#endregion

namespace Duologue.PlayObjects
{
    public enum PlayerState
    {
        Spawning,
        GettingReady,
        Alive,
        Dying,
        Dead,
    }
    /// <summary>
    /// The play object defining the game player.
    /// </summary>
    public class Player : PlayObject
    {
        #region Constants
        // Filenames
        private const string filename_playerBase = "player-base";
        private const string filename_playerCannon = "player-cannon";
        private const string filename_playerLight = "player-light";
        private const string filename_playerUIbase = "PlayerUI/P{0}-base";
        private const string filename_playerUIroot = "PlayerUI/player-root";
        private const string filename_spawnCrosshairs = "ship-spawn";
        private const string filename_beam = "beam";
        private const string filename_beamBase = "beam-base";
        private const string filename_tread = "{0:tread00}";
        private const string filename_shine = "shine{0:00}";
        private const string filename_treadDestroy = "{0:tread-destroy00}";
        private const string filename_playerDestroy = "player-destroy";
        private const int treadDestroyFrames = 2;

        // PlayerUI items
        private const float startSpawnScale = 5f;
        private const float endSpawnScale = 1f;
        private const float deltaSpawnScale = -0.1f;
        private const float deltaSpawnRotation = 0.01f;
        private const float maxOpacity = 255f;
        private const float tankBlinkTransparency = 128f;
        private const float minUITransparency = 50f;
        private const float maxUITransparency = 192f;
        private const int numTimesBlink = 10;
        private const float maxBlinkTimer = 0.25f;
        private const float vibrationTimeOnDeath = 500f;

        private const byte alphaIfOtherPlayers = 150;

        /// <summary>
        /// The time it takes to finish death sequence
        /// </summary>
        private const float deathTime = 4f;

        // Attraction/Repulsion force strengths
        private const float repulsionFromOtherPlayers = 5f;

        /// <summary>
        /// The minimum distance a player must move for an offset to register
        /// </summary>
        private const float minPlayerOffsetMovement = 3f;

        /// <summary>
        /// The time between bullet shots
        /// </summary>
        private const float timeBetwenShots = 0.15f;
        #endregion

        #region Fields
        // Is this player object initalized?
        //private bool Initialized;
        private PlayerState state;

        // sprite objects defining the player
        private SpriteObject playerBase;
        private SpriteObject playerCannon;
        private SpriteObject playerLight;
        private SpriteObject playerDestroy;

        // The beam
        private SpriteObject beam;
        private SpriteObject beamBase;
        private int numPlayers;
        //private SpriteObject shot;

        // Tread items
        private const int treadFrames = 2;
        private Texture2D[] playerTreads;
        private Texture2D[] treadDestroy;
        private Vector2 treadCenter;
        private Vector2 treadDestroyCenter;
        private int currentTread;
        private int treadTimer;
        private const int maxTreadTimer = 50;
        private Vector2 treadOffset;
        private const float maxTreadOffset = 10f;

        // Things relating to the "shine" or glimmer on the player
        private const int shineFrames = 4;
        private Texture2D[] playerShines;
        private Vector2 shineCenter;
        private int currentShine;
        private int shineTimer;
        private const int maxShineTimer = 20;

        // Relating to the player UI elements
        private Texture2D spawnCrosshairs;
        private float spawnScale;
        private float spawnRotation;
        private float spawnCalc_m = 1f / (endSpawnScale - startSpawnScale);
        private float spawnCalc_h = -1f*startSpawnScale / (endSpawnScale - startSpawnScale);
        private Vector2 spawnCenter;
        private Texture2D playerUIroot;
        private Texture2D playerUIbase;
        private Vector2 playerUICenter;
        //private bool blinkOn;
        private float blinkTimer;
        private int blinksSinceStart;
        private float deathTimer;
        private Vector2 playerUIOffset;
        private int myPlayerIndexNum;
        private float shotTimer;

        // FIXME, dude, got me
        private bool lightIsNegative;

        // The last position, offset and screen center
        private Vector2 lastPosition;
        private Vector2 screenCenter;
        private Vector2 offset;

        // The beam arc and radius
        private const float beamRadius = 400f;
        private float beamArcMin;
        private float beamArcMax;
        private float piOver6 = MathHelper.Pi / 6f;
        private ColorState colorState;

        // Audio
        private AudioManager audio;
        private GamePadHelper myPadHelper;
        #endregion

        #region Properties
        /// <summary>
        /// Read only property that tells whether or not the light beam is negative
        /// </summary>
        public bool LightIsNegative
        {
            get { return lightIsNegative; }
        }
        /// <summary>
        /// Returns a percentage complete for spawn crosshair
        /// </summary>
        public float SpawnCrosshairPercentage
        {
            get
            {
                float p = spawnCalc_m * spawnScale + spawnCalc_h;
                //Console.WriteLine(p);
                if (p > 1f)
                    return 1f;
                else if (p < 0f)
                    return 0f;
                else
                    return p;
            }
        }

        /// <summary>
        /// Determines if this player is active or not (e.g., if there is a controller
        /// connected that controls this player)
        /// </summary>
        public bool Active;

        /// <summary>
        /// Set to true if you want to ignore the screen boundaries
        /// (e.g., for cinematics)
        /// </summary>
        public bool IgnoreScreenBoundaries = false;

        /// <summary>
        /// The current state of this player
        /// </summary>
        public PlayerState State
        {
            get { return state; }
        }

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

        /// <summary>
        /// The current rotation of the tread
        /// </summary>
        public float TreadRotation;

        /// <summary>
        /// The shot object
        /// </summary>
        //public PlayerShot Shot;

        /// <summary>
        /// The current colorstate
        /// </summary>
        public ColorState ColorState
        {
            get { return colorState; }
            set
            {
                colorState = value;
                /*playerCannon.Tint = ColorState.Positive[ColorState.Dark];
                playerLight.Tint = ColorState.Negative[ColorState.Medium];
                shot.Tint = ColorState.Positive[ColorState.Light];
                beam.Tint = ColorState.Negative[ColorState.Dark];*/
                SetColors();
            }
        }

        /// <summary>
        /// The player color
        /// </summary>
        public PlayerColors PlayerColor;

        /// <summary>
        /// The player index associated with this player
        /// </summary>
        public PlayerIndex MyPlayerIndex;

        /// <summary>
        /// The SignedInGamer associated with this player
        /// </summary>
        public SignedInGamer MyGamer;

        /// <summary>
        /// The GamerProfile associated with this player. We store this
        /// separately from SignedInGamer because we reserve the right to
        /// later on add in newtork gamer support, and if that ever happens
        /// we need to have an instant way to get at the profile.
        /// </summary>
        public GamerProfile MyProfile;
        #endregion

        #region Constructor / Init / Load
        /// <summary>
        /// Default constructor
        /// </summary>
        public Player()
            : base()
        {
            MyType = TypesOfPlayObjects.Player;
            MajorType = MajorPlayObjectType.Player;
            Initialized = false;
            audio = ServiceLocator.GetService<AudioManager>();
        }

        /// <summary>
        /// Initialize the player object. Called every time the player needs to be initialized
        /// </summary>
        /// <param name="playerColor">Color of this player</param>
        /// <param name="playerIndex">Player's index</param>
        /// <param name="signedInGamer">Signed in gamer</param>
        /// <param name="gamerProfile">Gamer profile associated with the player</param>
        /// <param name="currentColorState">The current color state</param>
        /// <param name="startPos">The starting position of the player</param>
        /// <param name="orientation">The starting orientation of the player</param>
        /// <param name="numOfPlayers">The number of players this game</param>
        public void Initialize(
            PlayerColors playerColor,
            PlayerIndex playerIndex,
            SignedInGamer signedInGamer,
            GamerProfile gamerProfile,
            ColorState currentColorState,
            Vector2 startPos,
            Vector2 orientation,
            int numOfPlayers)
        {
            // Set up the player data
            PlayerColor = playerColor;
            MyPlayerIndex = playerIndex;
            myPlayerIndexNum = (int)MyPlayerIndex;
            MyGamer = signedInGamer;
            MyProfile = gamerProfile;
            Position = startPos;
            lastPosition = startPos;
            colorState = currentColorState;
            numPlayers = numOfPlayers;

            myPadHelper = LocalInstanceManager.GamePadHelpers[myPlayerIndexNum];

            // Set up the orientation related items
            lightIsNegative = true;
            Orientation = orientation;
            Aim = Vector2.Negate(Orientation);
            CaclulateRotations();
            treadOffset = Vector2.Zero;

            // Set up the timers
            treadTimer = 0;
            shineTimer = 0;
            shotTimer = 0f;
            beamArcMax = 0f;
            beamArcMin = 0f;

            LoadAndInitialize();

            state = PlayerState.Dead;
            spawnScale = startSpawnScale;
            spawnRotation = 0f;

            Initialized = true;
        }
        /// <summary>
        /// Long and tedious initialize function
        /// </summary>
        private void LoadAndInitialize()
        {
            if (AssetManager == null)
                AssetManager = InstanceManager.AssetManager;

            #region Load player objects
            playerBase = new SpriteObject(
                AssetManager.LoadTexture2D(filename_playerBase),
                Position,
                new Vector2(
                    AssetManager.LoadTexture2D(filename_playerBase).Width / 2f,
                    AssetManager.LoadTexture2D(filename_playerBase).Height / 2f),
                null,
                PlayerColor.Colors[PlayerColors.Light],
                0f,
                1f,
                0.5f);

            playerCannon = new SpriteObject(
                AssetManager.LoadTexture2D(filename_playerCannon),
                Position,
                new Vector2(
                    AssetManager.LoadTexture2D(filename_playerCannon).Width / 2f,
                    AssetManager.LoadTexture2D(filename_playerCannon).Height / 2f),
                null,
                ColorState.Positive[ColorState.Dark],
                0f,
                1f,
                0.4f);

            playerLight = new SpriteObject(
                AssetManager.LoadTexture2D(filename_playerLight),
                Position,
                new Vector2(
                    AssetManager.LoadTexture2D(filename_playerLight).Width / 2f,
                    AssetManager.LoadTexture2D(filename_playerLight).Height / 2f),
                null,
                ColorState.Negative[ColorState.Medium],
                0f,
                1f,
                0.4f);

            // Load projectile object
            /*shot = new SpriteObject(
                AssetManager.LoadTexture2D(filename_shot),
                Vector2.Zero,
                new Vector2(
                    AssetManager.LoadTexture2D(filename_shot).Width / 2f,
                    AssetManager.LoadTexture2D(filename_shot).Height / 2f),
                null,
                ColorState.Positive[ColorState.Light],
                0f,
                1f,
                1f);

            shot.Alive = false;*/

            // Load beam object
            Vector2 beamCenter = new Vector2(747f, 197.5f);
            beam = new SpriteObject(
                AssetManager.LoadTexture2D(filename_beam),
                Position,
                beamCenter,
                null,
                ColorState.Negative[ColorState.Dark],
                0f,
                1f,
                1f);

            beamBase = new SpriteObject(
                AssetManager.LoadTexture2D(filename_beamBase),
                Position,
                beamCenter,
                null,
                ColorState.Negative[ColorState.Light],
                0f,
                1f,
                1f);

            // Load the destroy items
            playerDestroy = new SpriteObject(
                AssetManager.LoadTexture2D(filename_playerDestroy),
                Vector2.Zero,
                new Vector2(
                    AssetManager.LoadTexture2D(filename_playerDestroy).Width/2f,
                    AssetManager.LoadTexture2D(filename_playerDestroy).Height/2f),
                null,
                PlayerColor.Colors[PlayerColors.Light],
                0f,
                1f,
                0.5f);
            #endregion Wow, that was a lot, wasn't it?

            SetColors();

            // Set up the treads and shines
            playerTreads = new Texture2D[treadFrames];
            for (int i = 0; i < treadFrames; i++)
            {
                playerTreads[i] = AssetManager.LoadTexture2D(String.Format(filename_tread, treadFrames-i));
            }
            currentTread = 0;
            treadCenter = new Vector2(
                playerTreads[currentTread].Width / 2f,
                playerTreads[currentTread].Height / 2f);

            treadDestroy = new Texture2D[treadDestroyFrames];
            for (int i = 0; i < treadDestroyFrames; i++)
            {
                treadDestroy[i] = AssetManager.LoadTexture2D(String.Format(filename_treadDestroy, treadDestroyFrames - i));
            }
            treadDestroyCenter = new Vector2(
                treadDestroy[currentTread].Width / 2f,
                treadDestroy[currentTread].Height / 2f);

            playerShines = new Texture2D[shineFrames];
            for (int i = 0; i < shineFrames; i++)
            {
                playerShines[i] = AssetManager.LoadTexture2D(String.Format(filename_shine, i+1));
            }
            currentShine = 0;
            shineCenter = new Vector2(
                playerShines[currentShine].Width / 2f,
                playerShines[currentShine].Height / 2f);

            // Player UI items
            spawnCrosshairs = AssetManager.LoadTexture2D(filename_spawnCrosshairs);
            spawnCenter = new Vector2(spawnCrosshairs.Width / 2f, spawnCrosshairs.Height / 2f);
            playerUIroot = AssetManager.LoadTexture2D(filename_playerUIroot);
            playerUIbase = AssetManager.LoadTexture2D(String.Format(filename_playerUIbase, (int)MyPlayerIndex +1));
            playerUICenter = new Vector2(
                playerUIbase.Width / 2f,
                playerUIbase.Height / 2f);
            playerUIOffset = new Vector2(
                0f,
                -1f * (playerUIroot.Height / 2f + playerBase.Texture.Height / 2f));

            // Set the radius
            Radius = playerBase.Texture.Height / 2f;
            if (playerBase.Texture.Width > playerBase.Texture.Height)
                Radius = playerBase.Texture.Width / 2f;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Ensure that we're still inside screenboundaries.
        /// </summary>
        private void CheckScreenBoundary()
        {
            if (!IgnoreScreenBoundaries)
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
        }

        /// <summary>
        /// Calculate the various rotations, should be called once per frame
        /// </summary>
        private void CaclulateRotations()
        {
            // The base is easy because we can arg it up- the base is a circle with
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

        /// <summary>
        /// Set the color of the player's utilities (light and gun)
        /// This should be called at the start of the object's life,
        /// and any other time the ColorState changes.
        /// </summary>
        private void SetColors()
        {
            if (lightIsNegative)
            {
                playerLight.Tint = colorState.Negative[ColorState.Medium];
                beam.Tint = colorState.Negative[ColorState.Dark];
                beamBase.Tint = colorState.Negative[ColorState.Light];

                playerCannon.Tint = colorState.Positive[ColorState.Dark];
                //shot.Tint = colorState.Positive[ColorState.Light];
            }
            else
            {
                playerLight.Tint = colorState.Positive[ColorState.Medium];
                beam.Tint = colorState.Positive[ColorState.Dark];
                beamBase.Tint = colorState.Positive[ColorState.Light];

                playerCannon.Tint = colorState.Negative[ColorState.Dark];
                //shot.Tint = colorState.Negative[ColorState.Light];
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
            if (state == PlayerState.Alive && shotTimer >= timeBetwenShots)
            {
                Vector2 startPos = Position + Vector2.Normalize(Aim) * Radius;
                shotTimer = 0f;

                for (int i = 0; i < LocalInstanceManager.MaxNumberOfBulletsPerPlayer; i++)
                {
                    if (!LocalInstanceManager.Bullets[myPlayerIndexNum][i].Alive)
                    {
                        LocalInstanceManager.Bullets[myPlayerIndexNum][i].Fire(Aim, startPos);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Given a position and color, will determine if it is in the beam, and if it is complementary or opposite
        /// </summary>
        /// <param name="eobj">The player object in question</param>
        /// <returns>Returns 0 if not in beam. -1 if in beam and opposite colors. +1 if in beam and complimentary colors.</returns>
        internal int IsInBeam(Enemy eobj)
        {
            int retval = 0;
            // Check if in-beam
            Vector2 distance = eobj.Position - Position;
            if (Math.Abs(distance.Length()) < beamRadius)
            {
                // We're close enough... inside the arc?
                float rotation = MWMathHelper.ComputeAngleAgainstX(distance);
                if (rotation > beamArcMin && rotation < beamArcMax)
                {
                    // In the beam
                    // Check if complimentary color
                    Color color;
                    switch (eobj.ColorPolarity)
                    {
                        case ColorPolarity.Positive:
                            color = eobj.ColorState.Positive[ColorState.Light];
                            break;
                        default:
                            color = eobj.ColorState.Negative[ColorState.Light];
                            break;
                    }
                    if (ColorState.SameColor(color, playerLight.Tint))
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

        /// <summary>
        /// Spawns the player
        /// </summary>
        public void Spawn()
        {
            state = PlayerState.Spawning;
        }

        /// <summary>
        /// Call when the player just needs to be instantly alive
        /// WARNING: Only use when you know what this does!
        /// </summary>
        public void SetAlive()
        {
            state = PlayerState.Alive;
            Alive = true;
        }

        /// <summary>
        /// Set the player state as dead
        /// WARNING: Only use when you know what this does!
        /// </summary>
        public void SetDead()
        {
            state = PlayerState.Dead;
            Alive = false;
        }
        #endregion

        #region Draw / Update
        /// <summary>
        /// Draw the player object
        /// </summary>
        /// <param name="gameTime">Gametime</param>
        public override void Draw(GameTime gameTime)
        {
            if (RenderSprite == null)
                RenderSprite = InstanceManager.RenderSprite;

            switch (state)
            {
                case PlayerState.Spawning:
                    DrawSpawning(gameTime);
                    break;
                case PlayerState.Alive:
                    DrawAlive(gameTime);
                    break;
                case PlayerState.GettingReady:
                    DrawGettingReady(gameTime);
                    break;
                case PlayerState.Dying:
                    DrawDying(gameTime);
                    break;
                default:
                    // Player is dead, do nothing
                    break;
            }
        }


        /// <summary>
        /// Called once per frame
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            shotTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (shotTimer >= timeBetwenShots)
                shotTimer = timeBetwenShots;
            switch (state)
            {
                case PlayerState.Alive:
                    UpdateAlive(gameTime);
                    break;
                case PlayerState.Spawning:
                    UpdateSpawning(gameTime);
                    break;
                case PlayerState.GettingReady:
                    UpdateGettingReady(gameTime);
                    break;
                case PlayerState.Dying:
                    UpdateDying(gameTime);
                    break;
                default:
                    // Player is dead
                    break;
            }
        }
        #endregion

        #region Public Overrides
        public override string[] GetTextureFilenames()
        {
            /* The player object is such a bear to initialize, and yet its
             * assets get loaded only once per game, that we're going to be
             * lazy here and not properly pre-cache all of the player assets.
             * Since the player is the only thing doing this, I think we
             * should be fine. -Sam
             */
            return new String[]
            {
                filename_playerCannon,
                filename_playerBase,
                filename_playerLight
            };
        }
        /// <summary>
        /// Call to set the AssetManager
        /// </summary>
        /// <param name="manager">The AssetManager</param>
        public override void SetAssetManager(AssetManager manager)
        {
            base.SetAssetManager(manager);
        }

        /// <summary>
        /// Call to set the graphics device
        /// </summary>
        /// <param name="device">The GraphicsDevice</param>
        public override void SetGraphicsDevice(GraphicsDevice device)
        {
            base.SetGraphicsDevice(device);
        }

        /// <summary>
        /// Call to set the render sprite
        /// </summary>
        /// <param name="render">Do you really need to ask?</param>
        public override void SetRenderSprite(RenderSprite render)
        {
            base.SetRenderSprite(render);
        }

        public override bool StartOffset()
        {
            offset = Vector2.Zero;
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MyType == TypesOfPlayObjects.Player)
            {
                Vector2 vToOtherPlayer = this.Position - pobj.Position;
                if (vToOtherPlayer.Length() < pobj.Radius + this.Radius)
                {
                    // Too close, BTFO
                    vToOtherPlayer.Normalize();
                    offset += repulsionFromOtherPlayers * Vector2.Negate(vToOtherPlayer);
                }
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            if(offset.Length() > minPlayerOffsetMovement)
                Position += offset;
            return true;
        }

        /// <summary>
        /// Called when we collide with a play object
        /// </summary>
        /// <param name="pobj">The playobject we've collided with</param>
        /// <returns>True if we've got more lives, or false if we're out of lives</returns>
        public override bool TriggerHit(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Enemy)
            {
                // Trigger a player explosion ring & explosion effects
                LocalInstanceManager.PlayerRing.AddRing(this.Position, this.PlayerColor.Colors[PlayerColors.Light]);
                LocalInstanceManager.PlayerExplosion.AddParticles(this.Position, this.PlayerColor.Colors[PlayerColors.Light]);
                LocalInstanceManager.PlayerSmoke.AddParticles(this.Position, Color.White);

                // Should trigger other explosions here
                myPadHelper.ChirpIt(vibrationTimeOnDeath, 0f, 1f);
                audio.PlayEffect(EffectID.PlayerExplosion);
                
                // Set the graphical items
                state = PlayerState.Dying;
                currentTread = InstanceManager.Random.Next(treadDestroyFrames);
                deathTimer = 0f;

                this.Active = LocalInstanceManager.Scores[(int)MyPlayerIndex].LoseLife();
                return this.Active;
            }
            return true;
        }
        #endregion

        #region Private Update Methods
        /// <summary>
        /// Update called when dying
        /// </summary>
        private void UpdateDying(GameTime gameTime)
        {
            deathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (deathTimer > deathTime)
            {
                deathTimer = deathTime;
                state = PlayerState.Dead;
            }
        }

        /// <summary>
        /// Update when the player is getting ready to play
        /// </summary>
        private void UpdateGettingReady(GameTime gameTime)
        {
            blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (blinkTimer > maxBlinkTimer)
            {
                //blinkOn = !blinkOn;
                blinksSinceStart++;
                blinkTimer = 0f;
                if (blinksSinceStart > numTimesBlink)
                {
                    state = PlayerState.Alive;
                }
            }
        }

        /// <summary>
        /// Update when the player is alive
        /// </summary>
        private void UpdateAlive(GameTime gameTime)
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
                InstanceManager.Logger.LogEntry(String.Format("Player {0} spawned", MyPlayerIndex.ToString()));
            }
            ComputeTreadOffset();
            lastPosition = Position;
        }

        /// <summary>
        /// Update when the player is spawning
        /// </summary>
        private void UpdateSpawning(GameTime gameTime)
        {
            spawnRotation += deltaSpawnRotation;
            if (spawnRotation > MathHelper.TwoPi)
                spawnRotation = 0f;
            else if (spawnRotation < 0f)
                spawnRotation = MathHelper.TwoPi;
            spawnScale += deltaSpawnScale;
            if (spawnScale < endSpawnScale)
            {
                state = PlayerState.GettingReady;
                spawnScale = endSpawnScale;
                //blinkOn = true;
                blinksSinceStart = 0;
                blinkTimer = 0f;
            }
        }
        #endregion

        #region Private Draw Methods
        /// <summary>
        /// Draw the player dying
        /// </summary>
        private void DrawDying(GameTime gameTime)
        {
            CaclulateRotations();
            CheckScreenBoundary();

            Color c = playerBase.Tint;

            if (deathTimer > deathTime * 0.5f)
            {
                // We only fade out past halfway point
                float alpha = 1f - deathTimer / deathTime;
                //InstanceManager.Logger.LogEntry(String.Format("fade alpha: {0}", alpha.ToString()));
                c = new Color(
                    playerBase.Tint,
                    alpha);

            }

            // Treads
            RenderSprite.Draw(
                treadDestroy[currentTread],
                Position + treadOffset,
                treadDestroyCenter,
                null,
                c,
                TreadRotation,
                1f,
                0f);

            // Base
            RenderSprite.Draw(
                playerDestroy.Texture,
                Position,
                playerDestroy.Center,
                null,
                c,
                BaseRotation,
                1f,
                0f);
        }

        /// <summary>
        /// Draw the player getting read
        /// </summary>
        private void DrawGettingReady(GameTime gameTime)
        {
            CaclulateRotations();
            CheckScreenBoundary();

            DrawPlayerTank((byte)tankBlinkTransparency);
            DrawPlayerCannon((byte)tankBlinkTransparency);

            float uiTrans = (blinkTimer / maxBlinkTimer) * maxUITransparency + minUITransparency;
            float lightTrans = maxUITransparency - minUITransparency*(blinkTimer / maxBlinkTimer);

            /*if (blinkOn)
            {
                uiTrans = (blinkTimer / maxBlinkTimer) * tankBlinkTransparency;
                //lightTrans = (blinkTimer / maxBlinkTimer) * maxUITransparency;
            }*/
            Color c1 = new Color(
                PlayerColor.Colors[PlayerColors.Dark].R,
                PlayerColor.Colors[PlayerColors.Dark].G,
                PlayerColor.Colors[PlayerColors.Dark].B,
                (byte)uiTrans);
            Color c2 = new Color(
                PlayerColor.Colors[PlayerColors.Light].R,
                PlayerColor.Colors[PlayerColors.Light].G,
                PlayerColor.Colors[PlayerColors.Light].B,
                (byte)uiTrans);
            Vector2 pos = Position + playerUIOffset;
            RenderSprite.Draw(
                playerUIroot,
                pos,
                playerUICenter,
                null,
                c1,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
            RenderSprite.Draw(
                playerUIbase,
                pos,
                playerUICenter,
                null,
                c2,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
            DrawPlayerLight((byte)lightTrans);
        }

        private void DrawPlayerCannon(byte? tankTransparency)
        {
            Color c = playerCannon.Tint;

            if (tankTransparency != null)
                c = new Color(
                    playerCannon.Tint.R,
                    playerCannon.Tint.G,
                    playerCannon.Tint.B,
                    (byte)tankTransparency);

            // Cannon
            RenderSprite.Draw(
                playerCannon.Texture,
                Position,
                playerCannon.Center,
                null,
                c,
                CannonRotation,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        private void DrawPlayerTank(byte? tankTransparency)
        {
            Color c = playerBase.Tint;

            if (tankTransparency != null)
                c = new Color(
                    playerBase.Tint.R,
                    playerBase.Tint.G,
                    playerBase.Tint.B,
                    (byte)tankTransparency);

            // Treads
            RenderSprite.Draw(
                playerTreads[currentTread],
                Position + treadOffset,
                treadCenter,
                null,
                c,
                TreadRotation,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);


            // Base
            RenderSprite.Draw(
                playerBase.Texture,
                Position,
                playerBase.Center,
                null,
                c,
                BaseRotation,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            // Shine
            RenderSprite.Draw(
                playerShines[currentShine],
                Position,
                shineCenter,
                null,
                c,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        /// <summary>
        /// Draw the player light beam with optional transparency
        /// </summary>
        private void DrawPlayerLight(byte? transparency)
        {
            Color c1 = playerLight.Tint;
            Color c2 = beam.Tint;
            Color c3 = beamBase.Tint;
            if (transparency != null)
            {
                c1 = new Color(
                    c1.R,
                    c1.G,
                    c1.B,
                    (byte)transparency);
                c2 = new Color(
                    c2.R,
                    c2.G,
                    c2.B,
                    (byte)transparency);
                c3 = new Color(
                    c3.R,
                    c3.G,
                    c3.B,
                    (byte)transparency);
            }

            // Light
            RenderSprite.Draw(
                playerLight.Texture,
                Position,
                playerLight.Center,
                null,
                c1,
                LightRotation,
                1f,
                0.5f,
                RenderSpriteBlendMode.Addititive);

            // Lightbeam
            RenderSprite.Draw(
                beam.Texture,
                Position,
                beam.Center,
                null,
                c2,
                BeamRotation,
                1f,
                0.5f,
                RenderSpriteBlendMode.Addititive);

            RenderSprite.Draw(
                beamBase.Texture,
                Position,
                beamBase.Center,
                null,
                c3,
                BeamRotation,
                1f,
                0.5f,
                RenderSpriteBlendMode.Addititive);
        }

        /// <summary>
        /// Draw the player when alive
        /// </summary>
        private void DrawAlive(GameTime gameTime)
        {
            CaclulateRotations();
            CheckScreenBoundary();

            DrawPlayerTank(null);

            if (numPlayers <= 1)
                DrawPlayerLight(null);
            else
                DrawPlayerLight(alphaIfOtherPlayers);

            DrawPlayerCannon(null);

        }

        /// <summary>
        /// Draw the player spawning
        /// </summary>
        private void DrawSpawning(GameTime gameTime)
        {
            Color c = new Color(
                playerBase.Tint.R,
                playerBase.Tint.G,
                playerBase.Tint.B,
                (byte)(SpawnCrosshairPercentage * maxOpacity));
            RenderSprite.Draw(
                spawnCrosshairs,
                Position,
                spawnCenter,
                null,
                c,
                spawnRotation,
                spawnScale,
                0.5f);
        }
        #endregion
    }
}
