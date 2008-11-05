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
#endregion

namespace Duologue.PlayObjects
{
    public enum PlayerState
    {
        Spawning,
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
        private const string filename_shot = "shot";
        private const string filename_playerUIbase = "PlayerUI/P{0}-base";
        private const string filename_playerUIroot = "PlayerUI/player-root";
        private const string filename_spawnCrosshairs = "ship-spawn";
        private const string filename_beam = "beam";
        private const string filename_beamBase = "beam-base";
        private const string filename_tread = "{0:tread00}";
        private const string filename_shine = "shine{0:00}";
        #endregion

        #region Fields
        // Is this player object initalized?
        private bool Initialized;
        private PlayerState state;

        // sprite objects defining the player
        private SpriteObject playerBase;
        private SpriteObject playerCannon;
        private SpriteObject playerLight;

        // The beam and shot
        private SpriteObject beam;
        private SpriteObject beamBase;
        private SpriteObject shot;

        // Tread items
        private const int treadFrames = 2;
        private Texture2D[] playerTreads;
        private Vector2 treadCenter;
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
        private Texture2D playerUIroot;
        private Texture2D playerUIbase;

        // FIXME, dude, got me
        private bool lightIsNegative;

        // The last position, and screen center
        private Vector2 lastPosition;
        private Vector2 screenCenter;

        // The beam arc and radius
        private const float beamRadius = 400f;
        private float beamArcMin;
        private float beamArcMax;
        private float piOver6 = MathHelper.Pi / 6f;
        #endregion

        #region Properties
        /// <summary>
        /// Determines if this player is active or not (e.g., if there is a controller
        /// connected that controls this player)
        /// </summary>
        public bool Active;

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
        public ColorState colorState;

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
            Initialized = false;
        }

        /// <summary>
        /// Initialize the player object. Called every time the player needs to be initializeds
        /// </summary>
        /// <param name="playerColor">Color of this player</param>
        /// <param name="playerIndex">Player's index</param>
        /// <param name="signedInGamer">Signed in gamer</param>
        /// <param name="gamerProfile">Gamer profile associated with the player</param>
        /// <param name="currentColorState">The current color state</param>
        /// <param name="startPos">The starting position of the player</param>
        public void Initialize(
            PlayerColors playerColor,
            PlayerIndex playerIndex,
            SignedInGamer signedInGamer,
            GamerProfile gamerProfile,
            ColorState currentColorState,
            Vector2 startPos)
        {
            // Set up the player data
            PlayerColor = playerColor;
            MyPlayerIndex = playerIndex;
            MyGamer = signedInGamer;
            MyProfile = gamerProfile;
            Position = startPos;
            lastPosition = startPos;
            colorState = currentColorState;

            // Set up the orientation related items
            lightIsNegative = true;
            Orientation = Vector2.UnitX;
            Aim = Vector2.Negate(Orientation);
            CaclulateRotations();
            treadOffset = Vector2.Zero;

            // Set up the timers
            treadTimer = 0;
            shineTimer = 0;
            beamArcMax = 0f;
            beamArcMin = 0f;

            LoadAndInitialize();

            state = PlayerState.Dead;

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
                colorState.Positive[ColorState.Dark],
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
                colorState.Negative[ColorState.Medium],
                0f,
                1f,
                0.4f);

            // Load projectile object
            shot = new SpriteObject(
                AssetManager.LoadTexture2D(filename_shot),
                Vector2.Zero,
                new Vector2(
                    AssetManager.LoadTexture2D(filename_shot).Width / 2f,
                    AssetManager.LoadTexture2D(filename_shot).Height / 2f),
                null,
                colorState.Positive[ColorState.Light],
                0f,
                1f,
                1f);

            shot.Alive = false;

            // Load beam object
            Vector2 beamCenter = new Vector2(747f, 197.5f);
            beam = new SpriteObject(
                AssetManager.LoadTexture2D(filename_beam),
                Position,
                beamCenter,
                null,
                colorState.Negative[ColorState.Dark],
                0f,
                1f,
                1f);

            beamBase = new SpriteObject(
                AssetManager.LoadTexture2D(filename_beamBase),
                Position,
                beamCenter,
                null,
                colorState.Negative[ColorState.Light],
                0f,
                1f,
                1f);
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

            playerShines = new Texture2D[shineFrames];
            for (int i = 0; i < shineFrames; i++)
            {
                playerShines[i] = AssetManager.LoadTexture2D(String.Format(filename_shine, i+1));
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
                shot.Tint = colorState.Positive[ColorState.Light];
            }
            else
            {
                playerLight.Tint = colorState.Positive[ColorState.Medium];
                beam.Tint = colorState.Positive[ColorState.Dark];
                beamBase.Tint = colorState.Positive[ColorState.Light];

                playerCannon.Tint = colorState.Negative[ColorState.Dark];
                shot.Tint = colorState.Negative[ColorState.Light];
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
            //Shot.Fire(Aim, playerCannon.Tint, startPos);
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

        /// <summary>
        /// Spawns the player
        /// </summary>
        public void Spawn()
        {
            state = PlayerState.Spawning;
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

            switch (state)
            {
                case PlayerState.Spawning:
                    DrawSpawning();
                    break;
                case PlayerState.Alive:
                    DrawAlive();
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
        }
        #endregion

        #region Public Overrides
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
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool ApplyOffset()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region Private Draw Methods
        /// <summary>
        /// Draw the player when alive
        /// </summary>
        private void DrawAlive()
        {
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
                RenderSpriteBlendMode.Addititive);

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
                RenderSpriteBlendMode.Addititive);

            RenderSprite.Draw(
                beamBase.Texture,
                Position,
                beamBase.Center,
                null,
                beamBase.Tint,
                BeamRotation,
                1f,
                0.5f,
                RenderSpriteBlendMode.Addititive);

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

        }

        /// <summary>
        /// Draw the player spawning
        /// </summary>
        private void DrawSpawning()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}
