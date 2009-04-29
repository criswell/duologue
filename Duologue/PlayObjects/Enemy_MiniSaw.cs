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
    public class Enemy_MiniSaw : Enemy
    {
        #region Constants
        private const string filename_blades = "Enemies/buzzsaw-blades";
        private const string filename_glooplet = "Enemies/gloop/glooplet";
        private const string filename_gloopletHighlight = "Enemies/gloop/glooplet-highlight";

        private const float scale_Blades = 0.75f;
        private const float scale_Gloop = 0.35f;
        private const double fadeInTime = 0.85f;

        private const float bladeRotateSpeed = -0.1f;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 5;

        /// <summary>
        /// The multiplier for point value tweaks based upon hitpoints
        /// </summary>
        private const int hitPointMultiplier = 2;

        /// <summary>
        /// How far we can go outside the screen before we should stop
        /// </summary>
        private const float outsideScreenMultiplier = 3.85f;

        #region Force Interactions
        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        /// <summary>
        /// The minimum distance I'm comfortable with being to the leader
        /// </summary>
        private const float leaderDistanceMinMultiplier = 1.25f;
        /// <summary>
        /// The maximum distance I'm comfortable with being to the leader
        /// </summary>
        private const float leaderDistanceMaxMultiplier = 1.9f;

        /// <summary>
        /// Attraction to the leader (from a distance)
        /// </summary>
        private const float leaderAttractFromDistance = 5f;

        /// <summary>
        /// The rotate speed for when we're around the leader
        /// </summary>
        private const float rotateSpeedLeader = 10f;

        /// <summary>
        /// The standard wandering speed
        /// </summary>
        private const float standardWanderSpeed = 5f;
        #endregion
        #endregion

        #region Fields
        // Texture & related
        private Texture2D texture_Blades;
        private Texture2D texture_Gloop;
        private Texture2D texture_Highlight;
        private Vector2 center_Blades;
        private Vector2 center_Gloop;
        private Vector2 center_Highlight;
        private Color myColor;
        private Color altExplosionColor;
        
        // Animation related
        private float rotation_Blades;
        private bool spawning;
        private float scale_CurrentScale;
        private double timer;

        // Movement related
        private Vector2 offset;
        private Vector2 nearestLeader;
        private float nearestLeaderRadius;
        private Enemy nearestLeaderObject;
        private bool upspin;

        // Audio stuff
        private AudioManager audio;
        #endregion

        #region Constructor / Init
        public Enemy_MiniSaw() : base() { }

        public Enemy_MiniSaw(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_MiniSaw;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Follower;
            Initialized = false;
            spawning = false;

            altExplosionColor = Color.WhiteSmoke;

            // Set the RealSize by hand
            RealSize = new Vector2(64, 65);
        }

        public override void Initialize(
            Vector2 startPos, 
            Vector2 startOrientation, 
            ColorState currentColorState, 
            ColorPolarity startColorPolarity, 
            int? hitPoints)
        {
            Position = startPos;
            Orientation = GetStartingVector();
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            if (hitPoints == null)
            {
                hitPoints = 0;
            }
            StartHitPoints = (int)hitPoints;
            CurrentHitPoints = (int)hitPoints;

            // The mini saw is different from its big brother,
            // it can be re-initialized after death multiple times
            // thus, we really want to do stuff we'd normally put
            // in LoadAndInitialize() here.
            rotation_Blades = 0f;
            spawning = true;
            Alive = true;
            scale_CurrentScale = 0f;
            timer = 0.0;
            upspin = false;
            myColor = GetMyColor(ColorState.Medium);
            ClearInnerTriggers();

            if(!Initialized)
                LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            texture_Blades = InstanceManager.AssetManager.LoadTexture2D(filename_blades);
            texture_Gloop = InstanceManager.AssetManager.LoadTexture2D(filename_glooplet);
            texture_Highlight = InstanceManager.AssetManager.LoadTexture2D(filename_gloopletHighlight);

            center_Blades = new Vector2(
                texture_Blades.Width / 2f, texture_Blades.Height / 2f);
            center_Gloop = new Vector2(
                texture_Gloop.Width / 2f, texture_Gloop.Height / 2f);
            center_Highlight = new Vector2(
                texture_Highlight.Width / 2f, texture_Highlight.Height / 2f);

            Radius = center_Blades.X * scale_Blades;

            audio = ServiceLocator.GetService<AudioManager>();

            Initialized = true;
        }

        public override string[] GetTextureFilenames()
        {
            return new String[]
            {
                filename_blades,
                filename_glooplet,
                filename_gloopletHighlight,
            };
        }
        #endregion

        #region Private methods
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
        #endregion

        #region Movement AI
        public override bool StartOffset()
        {
            offset = Vector2.Zero;
            nearestLeaderRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            nearestLeader = Vector2.Zero;
            nearestLeaderObject = null;
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Player)
            {
                // Player
                Vector2 vToPlayer = this.Position - pobj.Position;
                float len = vToPlayer.Length();

                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em
                    return pobj.TriggerHit(this);
                }

                // Beam handling
                int temp = ((Player)pobj).IsInBeam(this);
                return true;
            }
            else if (pobj.MajorType == MajorPlayObjectType.Enemy)
            {
                if (((Enemy)pobj).MyEnemyType == EnemyType.Leader)
                {
                    // Leader
                    Vector2 vToLeader = this.Position - pobj.Position;
                    float len = vToLeader.Length();
                    if (len < nearestLeaderRadius)
                    {
                        nearestLeaderRadius = len;
                        nearestLeader = vToLeader;
                        nearestLeaderObject = (Enemy)pobj;
                    }
                    else /*if (len < this.Radius + pobj.Radius)
                    {*/
                        // Too close, BTFO
                        if (len == 0f)
                        {
                            // Well, bah, we're on top of each other!
                            vToLeader = new Vector2(
                                (float)InstanceManager.Random.NextDouble() - 0.5f,
                                (float)InstanceManager.Random.NextDouble() - 0.5f);
                        }/*
                        vToLeader = Vector2.Negate(vToLeader);
                        vToLeader.Normalize();
                        offset += standardEnemyRepulse * vToLeader;
                    }*/
                }
                else
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
                        offset += standardEnemyRepulse * vToEnemy;
                    }
                }
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            if (nearestLeader.Length() > 0f)
            {
                // Check if we're too far away
                if (nearestLeaderRadius >= leaderDistanceMaxMultiplier * nearestLeaderObject.Radius)
                {
                    // Too far away, let's get closer
                    upspin = false;
                    nearestLeader.Normalize();
                    offset += leaderAttractFromDistance * Vector2.Negate(nearestLeader);
                }
                else if (nearestLeaderRadius < leaderDistanceMinMultiplier * nearestLeaderObject.Radius)
                {
                    // Too close, let's GTFO
                    upspin = true;
                    nearestLeader.Normalize();
                    offset += standardEnemyRepulse * nearestLeader;
                }
                else if (nearestLeaderRadius < leaderDistanceMaxMultiplier * nearestLeaderObject.Radius &&
                    nearestLeaderRadius >= leaderDistanceMinMultiplier * nearestLeaderObject.Radius)
                {
                    // Sweet spot, let's move around here
                    Vector2 temp = new Vector2(nearestLeader.Y, -nearestLeader.X);
                    temp.Normalize();

                    if (!upspin)
                        nearestLeader = Vector2.Negate(nearestLeader);
                    nearestLeader.Normalize();
                    nearestLeader += temp;

                    offset += rotateSpeedLeader * nearestLeader;
                }
            }
            else
            {
                // If not near leader, move according to orientation
                nearestLeader = Orientation;

                nearestLeader.Normalize();

                offset += standardWanderSpeed * nearestLeader;
            }
            this.Position += offset;
            Orientation = offset;

            // Check boundaries - Once we move off the screen after spawned, we just die
            if ((this.Position.X < -1 * RealSize.X * outsideScreenMultiplier ||
                this.Position.Y < -1 * RealSize.Y * outsideScreenMultiplier ||
                this.Position.Y > InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier ||
                this.Position.X > InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier) &&
                !spawning)
            {
                Alive = false;
                return false;
            }

            return true;
        }


        public override bool TriggerHit(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.PlayerBullet)
            {
                CurrentHitPoints--;
                if (CurrentHitPoints <= 0)
                {
                    LocalInstanceManager.EnemyExplodeSystem.AddParticles(Position, myColor);
                    LocalInstanceManager.EnemyExplodeSystem.AddParticles(Position, altExplosionColor);
                    LocalInstanceManager.AchievementManager.EnemyDeathCount(MyType);
                    Alive = false;
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue + hitPointMultiplier * StartHitPoints, Position);
                    audio.PlayEffect(EffectID.BuzzDeath);
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(texture_Gloop, myColor, Position, 0f);
                    audio.PlayEffect(EffectID.CokeBottle);
                    return true;
                }
            }
            return true;
        }
        #endregion

        #region Update / Draw
        public override void Draw(GameTime gameTime)
        {
            // Draw the blades
            InstanceManager.RenderSprite.Draw(
                texture_Blades,
                Position,
                center_Blades,
                null,
                Color.White,
                rotation_Blades,
                scale_Blades * scale_CurrentScale,
                0f);

            // Draw the base
            InstanceManager.RenderSprite.Draw(
                texture_Gloop,
                Position,
                center_Gloop,
                null,
                myColor,
                0f,
                scale_Gloop * scale_CurrentScale,
                0f);

            // Draw the shine
            InstanceManager.RenderSprite.Draw(
                texture_Highlight,
                Position,
                center_Highlight,
                null,
                Color.White,
                0f,
                scale_Gloop * scale_CurrentScale,
                0f);
        }

        public override void Update(GameTime gameTime)
        {
            if (spawning)
            {
                if (timer < fadeInTime)
                {
                    scale_CurrentScale = (float)(timer / fadeInTime);
                    timer += gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    spawning = false;
                    scale_CurrentScale = 1f;
                    timer = fadeInTime;
                }
            }

            rotation_Blades += bladeRotateSpeed;
            if (rotation_Blades > MathHelper.TwoPi)
                rotation_Blades = 0f;
            else if (rotation_Blades < 0f)
                rotation_Blades = MathHelper.TwoPi;
        }
        #endregion
    }
}
