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
using Duologue.Audio;
using Duologue.State;
using Duologue.Screens;
#endregion

namespace Duologue.PlayObjects
{
    class Enemy_Maggot : Enemy
    {
        #region Constants
        private const string filename_Glooplet = "Enemies/gloop/glooplet";
        private const string filename_Highlight = "Enemies/gloop/glooplet-highlight";
        private const string filename_Death = "Enemies/gloop/glooplet-death";
        private const string filename_Bubble = "Enemies/iridescent_bubble";

        private const string filename_SplatExplode = "Audio/PlayerEffects/splat-explode";
        private const float volume_Splat = 0.25f;

        private const int numberOfWormBits = 4;

        private const float maxSize = 0.6f;
        private const float minSize = 0.08f;
        private const float deltaSize = 0.015f;
        private const float bubbleScale = 0.35f;

        private const float lightScale = 0.6f;

        private const float lightVerticalOffset = -10f;

        private const float radiusScale = 0.65f;

        private const float outsideScreenMultiplier = 2f;

        private const double timeBetweenTurns = 1.1;
        private const double timeBetweenSpawns = 0.35;

        private const double timeBetweenDeaths = 0.25;

        private const double minTurnAngle = -1f * MathHelper.PiOver4;
        private const double maxTurnAngle = MathHelper.PiOver4;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 25;

        /// <summary>
        /// The multiplier for point value tweaks based upon hitpoints
        /// </summary>
        private const int hitPointMultiplier = 3;

        #region Forces and interactions
        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        private const float speed = 1.3f;

        private const float offScreenSpeed = 3.4f;
        #endregion
        #endregion

        #region Properties
        #endregion

        #region Fields
        private Texture2D texture_Glooplet;
        private Texture2D texture_Highlight;
        private Texture2D texture_Death;
        private Texture2D texture_Bubble;

        private Vector2 center_Glooplet;
        private Vector2 center_Highlight;
        private Vector2 lightOffset;
        private Vector2 center_Bubble;
        private Vector2 bubbleOffset;

        private float rotation_Bubble;

        private Color color_Dark;
        private Color color_Light;
        private Color color_Bubble;

        private WormBit[] myBits;

        private double timeSinceSwitch;
        private double timeSinceSpawn;

        private bool dying;

        private int nextDead;

        private Vector2 offset;

        // Sound stuff
        private SoundEffect sfx_Explode;
        private AudioManager audio;
        #endregion

        #region Constructor/Init
        /// <summary>
        /// The empty constructor for pre-caching
        /// </summary>
        public Enemy_Maggot() : base() { }

        public Enemy_Maggot(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Maggot;
            MajorType = MajorPlayObjectType.Enemy;
            Initialized = false;

            // Set the RealSize by hand
            RealSize = new Vector2(83, 83);
            Initialized = false;
            Alive = false;
            OffscreenArrow = true;
        }

        public override void Initialize(
            Vector2 startPos,
            Vector2 startOrientation,
            ColorState currentColorState,
            ColorPolarity startColorPolarity,
            int? hitPoints,
            double spawnDelay)
        {
            Position = startPos;
            if (startOrientation == Vector2.Zero)
            {
                Orientation = GetStartingVector();
            }
            else
            {
                Orientation = startOrientation;
            }
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            SpawnTimeDelay = spawnDelay;
            SpawnTimer = 0;
            if (hitPoints == null)
            {
                hitPoints = 0;
            }
            StartHitPoints = (int)hitPoints;
            CurrentHitPoints = (int)hitPoints;
            audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            texture_Glooplet = InstanceManager.AssetManager.LoadTexture2D(filename_Glooplet);
            texture_Highlight = InstanceManager.AssetManager.LoadTexture2D(filename_Highlight);
            texture_Death = InstanceManager.AssetManager.LoadTexture2D(filename_Death);
            texture_Bubble = InstanceManager.AssetManager.LoadTexture2D(filename_Bubble);

            center_Glooplet = new Vector2(
                texture_Glooplet.Width / 2f, texture_Glooplet.Height / 2f);
            center_Highlight = new Vector2(
                texture_Highlight.Width / 2f, texture_Highlight.Height / 2f);
            center_Bubble = new Vector2(
                texture_Bubble.Width / 2f, texture_Bubble.Height / 2f);

            sfx_Explode = InstanceManager.AssetManager.LoadSoundEffect(filename_SplatExplode);

            rotation_Bubble = (float)MWMathHelper.GetRandomInRange(0, MathHelper.TwoPi);

            color_Dark = GetMyColor(ColorState.Medium);
            color_Light = GetMyColor(ColorState.Light);
            color_Bubble = new Color(Color.SeaGreen, 0.5f);

            bubbleOffset = Vector2.Zero;

            myBits = new WormBit[numberOfWormBits];
            for (int i = 0; i < numberOfWormBits; i++)
            {
                myBits[i].Size = 0;
            }

            Radius = radiusScale * (RealSize.Length() / 2f);

            lightOffset = new Vector2(0f, lightVerticalOffset);
            timeSinceSwitch = MWMathHelper.GetRandomInRange(0, timeBetweenTurns);
            timeSinceSpawn = 0;
            dying = false;

            Alive = true;
            Initialized = true;
        }
        #endregion

        #region Private Methods
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

        private bool OnScreen()
        {
            return (Position.X > 0 && Position.Y > 0 &&
                Position.X < InstanceManager.DefaultViewport.Width &&
                Position.Y < InstanceManager.DefaultViewport.Height);
        }

        private Vector2 RandomJitter(double lower, double upper)
        {
            return new Vector2(
                (float)MWMathHelper.GetRandomInRange(lower, upper),
                (float)MWMathHelper.GetRandomInRange(lower, upper));
        }
        #endregion

        #region Public overrides
        public override string[] GetTextureFilenames()
        {
            return new String[]
            {
                filename_Death,
                filename_Glooplet,
                filename_Highlight
            };
        }

        public override string[] GetSFXFilenames()
        {
            return new String[]
            {
                filename_SplatExplode
            };
        }
        public override bool StartOffset()
        {
            offset = Vector2.Zero;
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Player && !dying)
            {
                // Player
                Vector2 vToPlayer = this.Position - pobj.Position;
                float len = vToPlayer.Length();
                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em
                    return pobj.TriggerHit(this);
                }
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            if (!dying)
            {
                Orientation.Normalize();

                if (OnScreen())
                    offset += Orientation * speed;
                else
                    offset += Orientation * offScreenSpeed;

                this.Position += offset;
                Orientation = offset;

                // Check boundaries
                if (this.Position.X < -1 * RealSize.X * outsideScreenMultiplier)
                {
                    this.Position.X = -1 * RealSize.X * outsideScreenMultiplier;
                    Orientation = GetVectorPointingAtOrigin();
                }
                else if (this.Position.X > InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier)
                {
                    this.Position.X = InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier;
                    Orientation = GetVectorPointingAtOrigin();
                }

                if (this.Position.Y < -1 * RealSize.Y * outsideScreenMultiplier)
                {
                    this.Position.Y = -1 * RealSize.Y * outsideScreenMultiplier;
                    Orientation = GetVectorPointingAtOrigin();
                }
                else if (this.Position.Y > InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier)
                {
                    this.Position.Y = InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier;
                    Orientation = GetVectorPointingAtOrigin();
                }
            }

            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.PlayerBullet && !dying)
            {
                CurrentHitPoints--;
                if (CurrentHitPoints <= 0)
                {
                    dying = true;
                    sfx_Explode.Play(volume_Splat);
                    timeSinceSwitch = timeBetweenDeaths;
                    nextDead = numberOfWormBits - 1;
                    MyManager.TriggerPoints(
                        ((PlayerBullet)pobj).MyPlayerIndex,
                        myPointValue + hitPointMultiplier * StartHitPoints,
                        Position);
                    //audio.soundEffects.PlayEffect(EffectID.BuzzDeath);
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(texture_Death, color_Dark, Position, 0f);
                    //audio.soundEffects.PlayEffect(EffectID.CokeBottle);
                    audio.PlayEffect(EffectID.CokeBottle);
                }
            }
            return true;
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            // Draw the shadows first
            for (int i = 0; i < numberOfWormBits; i++)
            {
                if (myBits[i].Size > 0)
                    InstanceManager.RenderSprite.Draw(
                        texture_Glooplet,
                        myBits[i].Position,
                        center_Glooplet,
                        null,
                        Color.Black,
                        0f,
                        myBits[i].Size,
                        0f,
                        RenderSpriteBlendMode.AlphaBlend);
            }

            // Draw the orbs
            for (int i = 0; i < numberOfWormBits; i++)
            {
                if (myBits[i].Size > 0)
                {
                    InstanceManager.RenderSprite.Draw(
                        texture_Glooplet,
                        myBits[i].Position,
                        center_Glooplet,
                        null,
                        color_Dark,
                        0f,
                        myBits[i].Size,
                        0f,
                        RenderSpriteBlendMode.AlphaBlend);

                    InstanceManager.RenderSprite.Draw(
                        texture_Glooplet,
                        myBits[i].Position + lightScale * lightOffset,
                        center_Glooplet,
                        null,
                        color_Light,
                        0f,
                        myBits[i].Size * lightScale,
                        0f,
                        RenderSpriteBlendMode.AlphaBlend);

                    InstanceManager.RenderSprite.Draw(
                        texture_Highlight,
                        myBits[i].Position,
                        center_Highlight,
                        null,
                        Color.White,
                        0f,
                        myBits[i].Size,
                        0f,
                        RenderSpriteBlendMode.AlphaBlend);
                }
            }

            // Draw bubble
            InstanceManager.RenderSprite.Draw(
                texture_Bubble,
                Position + bubbleOffset,
                center_Bubble,
                null,
                color_Bubble,
                rotation_Bubble,
                bubbleScale,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        public override void Update(GameTime gameTime)
        {
            if (SpawnTimerElapsed)
            {
                timeSinceSwitch += gameTime.ElapsedGameTime.TotalSeconds;
                timeSinceSpawn += gameTime.ElapsedGameTime.TotalSeconds;

                if (dying)
                {
                    if (timeSinceSwitch > timeBetweenDeaths)
                    {
                        if (nextDead >= 0)
                        {
                            timeSinceSwitch = 0;
                            myBits[nextDead].Size = -1f;
                            LocalInstanceManager.EnemySplatterSystem.AddParticles(
                                myBits[nextDead].Position,
                                color_Dark);
                            nextDead--;
                        }
                        else
                        {
                            Alive = false;
                            //sfx_Explode.Play(volume_Splat);
                            LocalInstanceManager.AchievementManager.EnemyDeathCount(MyType);
                        }
                    }
                }
                else
                {
                    bool spawn = false;

                    if (timeSinceSwitch > timeBetweenTurns)
                    {
                        // Turn randomly
                        if (OnScreen())
                            Orientation = MWMathHelper.RotateVectorByRadians(Orientation,
                                (float)MWMathHelper.GetRandomInRange(minTurnAngle, maxTurnAngle));
                        else
                            Orientation = GetStartingVector();
                        timeSinceSwitch = 0;
                    }

                    if (timeSinceSpawn > timeBetweenSpawns)
                    {
                        timeSinceSpawn = 0;
                        spawn = true;
                    }

                    for (int i = 1; i < numberOfWormBits; i++)
                    {
                        myBits[i].Position += RandomJitter(-2.0, 2);
                        if (myBits[i].Grow)
                        {
                            myBits[i].Size += deltaSize;
                        }
                        else
                        {
                            myBits[i].Size -= deltaSize;
                        }

                        if (myBits[i].Size < minSize)
                        {
                            if (spawn)
                            {
                                myBits[i] = myBits[0];
                                spawn = false;
                            }
                            else
                            {
                                myBits[i].Size = minSize;
                            }
                        }
                        else if (myBits[i].Size > maxSize)
                        {
                            myBits[i].Size = maxSize;
                            myBits[i].Grow = false;
                        }
                    }

                    myBits[0].Position = Position + RandomJitter(-2.0, 2.0);
                    myBits[0].Size = maxSize;
                    myBits[0].Grow = false;

                    bubbleOffset = RandomJitter(-1.0, 1.0);
                }
            }
            else
            {
                SpawnTimer += gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        #endregion
    }
}
