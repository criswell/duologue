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
    public class Enemy_Ember : Enemy
    {
        #region Constants
        private const string filename_Ember = "Enemies/ember";
        private const string filename_frames = "Enemies/static-king-{0}";

        private const int numberOfFrames = 4;

        /// <summary>
        /// The minimum scale for the ember
        /// </summary>
        private const double minScale = 0.1;

        /// <summary>
        /// The maximum scale for the ember
        /// </summary>
        private const double maxScale = 0.7;

        /// <summary>
        /// The vertical offset of the ember
        /// </summary>
        private const float verticalOffset = 20f;

        /// <summary>
        /// The vertical scale for the flame
        /// </summary>
        private const float flameVerticalScale = 1.2f;

        /// <summary>
        /// The starting scale for each frame
        /// </summary>
        private const float startingScale = 0.6f;

        /// <summary>
        /// The delta scale per frame of animation
        /// </summary>
        private const float deltaScale = 0.01f;

        /// <summary>
        /// The radius multiplier (inside this point counts as a hit or colision
        /// </summary>
        private const float radiusMultiplier = 0.8f;

        /// <summary>
        /// The initial delta scale per frame
        /// </summary>
        private const float initDeltaScale = 0.1f;

        /// <summary>
        /// The minimum scale to begin a fade
        /// </summary>
        private const float minScaleToTriggerFade = 0.9f;

        /// <summary>
        /// The minimum alpha that the frame can be
        /// </summary>
        private const byte minAlpha = 45;

        /// <summary>
        /// The delta alpha per frame once we start fading
        /// </summary>
        private const int deltaAlpha = -20;

        /// <summary>
        /// The maximum starting angle (in either direction)
        /// </summary>
        private const float maxStartingAngle = MathHelper.PiOver4 / 2f;

        /// <summary>
        /// The maximum delta angle
        /// </summary>
        private const float maxDeltaAngle = MathHelper.PiOver4 * 0.005f;

        /// <summary>
        /// The maximum vertical offset
        /// </summary>
        private const double maxVerticalOffset = -4.0;

        /// <summary>
        /// The size of the outline
        /// </summary>
        private const float outlineSize = 1.05f;

        /// <summary>
        /// The time it takes to change ember colors
        /// </summary>
        private const double emberColorChangeFadeTime = 0.1;

        /// <summary>
        /// The time it takes between ember color changes
        /// </summary>
        private const double emberColorChangeDelta = 0.08;

        /// <summary>
        /// How far we can go outside the screen before we should stop
        /// </summary>
        private const float outsideScreenMultiplier = 3;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 15;

        #region Force interactions
        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        /// <summary>
        /// Attraction to the leader
        /// </summary>
        private const float leaderAttract = 5f;

        /// <summary>
        /// Repulsion from the leader
        /// </summary>
        private const float leaderRepulse = 3f;

        /// <summary>
        /// The player attract modifier
        /// </summary>
        private const float playerAttract = 2.1f;

        /// <summary>
        /// The repulsion from the player if the player gets too close
        /// this should be lower than attract so that the player can bump into them
        /// </summary>
        private const float playerRepulse = 1.5f;

        /// <summary>
        /// The rotate speed
        /// </summary>
        private const float rotateSpeed = 1.5f;

        /// <summary>
        /// The rotate speed for when we're around the leader
        /// </summary>
        private const float rotateSpeedLeader = 10f;

        /// <summary>
        /// The minimum movement required before we register motion
        /// </summary>
        private const float minMovement = 1.4f;

        /// <summary>
        /// Min number of how many of our radius size away we're comfortable with the player
        /// </summary>
        private const float minPlayerComfortRadiusMultiplier = 1.5f;

        /// <summary>
        /// Max number of our radius we're comfortable with the player
        /// </summary>
        private const float maxPlayerComfortRadiusMultiplier = 2.5f;

        /// <summary>
        /// Min number of our radius away from the leader we're comfortable with
        /// </summary>
        private const float minLeaderComfortRadiusMultiplier = 1.5f;

        /// <summary>
        /// Max number of our radius away from the leader we're comfortable with
        /// </summary>
        private const float maxLeaderComfortRadiusMultiplier = 2f;
        #endregion
        #endregion

        #region Fields
        private Texture2D ember;
        private Vector2 emberCenter;
        private StaticKingFrame[] flameFrames;
        private Vector2 flameCenter;
        private Vector2 flameScale;
        private float emberScale;
        private Vector2 emberOffset;
        private float originalRadius;

        private Color flameColor;
        private Color[] emberColors;
        private int currentEmberColor;
        private int lastEmberColor;
        private double timeSinceColorChange;

        private bool isFleeing;

        // Movement
        private Vector2 offset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;
        private Vector2 nearestLeader;
        private float nearestLeaderRadius;
        private Enemy nearestLeaderObject;
        private Vector2 lastDirection;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public Enemy_Ember(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Ember;
            MajorType = MajorPlayObjectType.Enemy;
            Initialized = false;

            // Set the RealSize by hand
            RealSize = new Vector2(100, 95);
            originalRadius = RealSize.Length() / 2f;
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
            //audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            ember = InstanceManager.AssetManager.LoadTexture2D(filename_Ember);
            flameFrames = new StaticKingFrame[numberOfFrames];

            for (int i = 0; i < numberOfFrames; i++)
            {
                flameFrames[i].Texture = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_frames, (i + 1).ToString()));
                SetupFrame(i);
                flameFrames[i].Scale = startingScale + i * initDeltaScale;
            }

            flameCenter = new Vector2(
                flameFrames[0].Texture.Width / 2f,
                flameFrames[0].Texture.Height / 2f);
            emberCenter = new Vector2(
                ember.Width / 2f,
                ember.Height / 2f);

            emberScale = (float)MWMathHelper.GetRandomInRange(minScale, maxScale);
            emberOffset = new Vector2(0, verticalOffset * emberScale);
            flameScale = new Vector2(emberScale, emberScale * flameVerticalScale);

            Radius = emberCenter.X * emberScale * radiusMultiplier;

            flameColor = GetMyColor(ColorState.Medium);
            emberColors = new Color[ColorState.NumberColorsPerPolarity];
            emberColors[0] = GetMyColor(ColorState.Light);
            emberColors[1] = GetMyColor(ColorState.Medium);
            emberColors[2] = GetMyColor(ColorState.Dark);
            currentEmberColor = 0;
            lastEmberColor = 0;
            timeSinceColorChange = 0;
            isFleeing = false;

            Initialized = true;
            Alive = true;
        }
        #endregion

        #region Private methods
        private void SetupFrame(int i)
        {
            flameFrames[i].Scale = startingScale;
            flameFrames[i].Alpha = Color.White.A;
            if (MWMathHelper.CoinToss())
            {
                flameFrames[i].Rotation = (float)MWMathHelper.GetRandomInRange(-1.0 * maxStartingAngle, 0.0);
                flameFrames[i].DeltaRotation = (float)MWMathHelper.GetRandomInRange(0.0, maxDeltaAngle);
            }
            else
            {
                flameFrames[i].Rotation = (float)MWMathHelper.GetRandomInRange(0.0, maxStartingAngle);
                flameFrames[i].DeltaRotation = (float)MWMathHelper.GetRandomInRange(-1.0 * maxDeltaAngle, 0.0);
            }
            flameFrames[i].Offset = new Vector2(
                0f, (float)MWMathHelper.GetRandomInRange(0.0, maxVerticalOffset));
        }
        #endregion

        #region Overrides
        public override bool StartOffset()
        {
            offset = Vector2.Zero;
            nearestPlayerRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            nearestPlayer = Vector2.Zero;
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
                isFleeing = false;
                if (temp != 0)
                {
                    if (temp == -1)
                    {
                        isFleeing = true;
                        LocalInstanceManager.Steam.AddParticles(Position, emberColors[currentEmberColor]);
                    }
                }
                return true;
            }
            else if (pobj.MajorType == MajorPlayObjectType.Enemy)
            {
                if (pobj.MyType == TypesOfPlayObjects.Enemy_StaticKing)
                {
                    // We only hold allegiance to the other fire guy
                    Vector2 vToLeader = this.Position - pobj.Position;
                    float len = vToLeader.Length();
                    if (len < nearestLeaderRadius)
                    {
                        nearestLeaderRadius = len;
                        nearestLeader = vToLeader;
                        nearestLeaderObject = (Enemy)pobj;
                    }
                    else if (len < this.Radius + pobj.Radius)
                    {
                        // Too close, BTFO
                        if (len == 0f)
                        {
                            // Well, bah, we're on top of each other!
                            vToLeader = new Vector2(
                                (float)InstanceManager.Random.NextDouble() - 0.5f,
                                (float)InstanceManager.Random.NextDouble() - 0.5f);
                        }
                        vToLeader = Vector2.Negate(vToLeader);
                        vToLeader.Normalize();
                        offset += standardEnemyRepulse * vToLeader;
                    }
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
            if (nearestLeader.Length() > 0f)
            {
                // The leader comes first
                if (nearestLeaderRadius >= maxLeaderComfortRadiusMultiplier * originalRadius)
                {
                    // If we're outside our comfort zone, let's get near the leader
                    nearestLeader.Normalize();

                    offset += leaderAttract * Vector2.Negate(nearestLeader);
                }
                else if (nearestLeaderRadius < maxLeaderComfortRadiusMultiplier * originalRadius &&
                    nearestLeaderRadius >= minLeaderComfortRadiusMultiplier * originalRadius)
                {
                    // Raaaah! Inside the comfort zone, <3 leader
                    nearestLeader.Normalize();

                    nearestLeader = new Vector2(nearestLeader.Y, -nearestLeader.X);

                    offset += rotateSpeedLeader * nearestLeader;
                }
                else
                {
                    // Woah, BTFO man
                    nearestLeader.Normalize();

                    offset += leaderRepulse * nearestLeader;
                }
            } else if (nearestPlayer.Length() > 0f)
            {
                // Next priority is the player
                if (nearestPlayerRadius >= maxPlayerComfortRadiusMultiplier * originalRadius)
                {
                    // If we're outside our comfort radius, bear down on the player
                    // like a mad man
                    nearestPlayer.Normalize();

                    if (!isFleeing)
                        nearestPlayer = Vector2.Negate(nearestPlayer);

                    offset += playerAttract * nearestPlayer;
                }
                else if (nearestPlayerRadius < maxPlayerComfortRadiusMultiplier * originalRadius &&
                    nearestPlayerRadius >= minPlayerComfortRadiusMultiplier * originalRadius)
                {
                    // We're in our comfort zone, we just orbit the player
                    nearestPlayer.Normalize();

                    nearestPlayer = new Vector2(nearestPlayer.Y, -nearestPlayer.X);

                    offset += rotateSpeed * nearestPlayer;
                }
                else
                {
                    // We're too close to the player for comfort, let's BTFO
                    nearestPlayer.Normalize();

                    offset += playerRepulse * nearestPlayer;
                }
            }
            else
            {
                // If no near player or leader, move in previous direction
                nearestPlayer = lastDirection;

                nearestPlayer.Normalize();

                offset += playerAttract * nearestPlayer;
            }

            // Next apply the offset permanently
            if (offset.Length() >= minMovement)
            {
                this.Position += offset;
                lastDirection = offset;
                Orientation = new Vector2(-offset.Y, offset.X);
            }

            // Check boundaries
            if (this.Position.X < -1 * RealSize.X * outsideScreenMultiplier)
                this.Position.X = -1 * RealSize.X * outsideScreenMultiplier;
            else if (this.Position.X > InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier)
                this.Position.X = InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier;

            if (this.Position.Y < -1 * RealSize.Y * outsideScreenMultiplier)
                this.Position.Y = -1 * RealSize.Y * outsideScreenMultiplier;
            else if (this.Position.Y > InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier)
                this.Position.Y = InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier;

            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.PlayerBullet)
            {
                CurrentHitPoints--;
                if (CurrentHitPoints <= 0)
                {
                    Alive = false;
                    LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                            Position, emberColors[currentEmberColor]);
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue, Position);
                    /*audio.soundEffects.PlayEffect(EffectID.BuzzDeath);*/
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(
                        flameFrames[MWMathHelper.GetRandomInRange(0, numberOfFrames)].Texture,
                        flameColor,
                        Position,
                        0f);
                    //MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myShieldPointValue, Position);
                    /*audio.soundEffects.PlayEffect(EffectID.CokeBottle);*/
                    return true;
                }
            }
            return true;
        }
        #endregion

        #region Draw/Update
        public override void Draw(GameTime gameTime)
        {
            // Draw the outline (FIXME would be lovely if we didn't have to do this in two for loops)
            for (int i = 0; i < numberOfFrames; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    flameFrames[i].Texture,
                    Position,
                    flameCenter,
                    null,
                    new Color(Color.Black, flameFrames[i].Alpha),
                    flameFrames[i].Rotation,
                    flameFrames[i].Scale * outlineSize * flameScale,
                    0f);
            }
            // Draw the vapors
            for (int i = 0; i < numberOfFrames; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    flameFrames[i].Texture,
                    Position,
                    flameCenter,
                    null,
                    new Color(flameColor, flameFrames[i].Alpha),
                    flameFrames[i].Rotation,
                    flameFrames[i].Scale * flameScale,
                    0f);
            }
            // Draw the ember
            if (lastEmberColor != currentEmberColor)
            {
                InstanceManager.RenderSprite.Draw(
                    ember,
                    Position + emberOffset,
                    emberCenter,
                    null,
                    emberColors[lastEmberColor],
                    0f,
                    emberScale,
                    0f);

                InstanceManager.RenderSprite.Draw(
                    ember,
                    Position + emberOffset,
                    emberCenter,
                    null,
                    new Color(emberColors[currentEmberColor], (float)(timeSinceColorChange / emberColorChangeFadeTime)),
                    0f,
                    emberScale,
                    0f);
            }
            else
            {
                InstanceManager.RenderSprite.Draw(
                    ember,
                    Position + emberOffset,
                    emberCenter,
                    null,
                    emberColors[currentEmberColor],
                    0f,
                    emberScale,
                    0f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceColorChange += gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < numberOfFrames; i++)
            {
                flameFrames[i].Scale += deltaScale;
                flameFrames[i].Rotation += flameFrames[i].DeltaRotation;
                if (flameFrames[i].Rotation < 0f)
                    flameFrames[i].Rotation = MathHelper.TwoPi;
                else if (flameFrames[i].Rotation > MathHelper.TwoPi)
                    flameFrames[i].Rotation = 0f;

                if (flameFrames[i].Scale > minScaleToTriggerFade)
                {
                    flameFrames[i].Alpha = (byte)(flameFrames[i].Alpha + deltaAlpha);
                    if (flameFrames[i].Alpha < minAlpha)
                        SetupFrame(i);
                }
            }

            if (lastEmberColor != currentEmberColor)
            {
                if (timeSinceColorChange >= emberColorChangeFadeTime)
                {
                    lastEmberColor = currentEmberColor;
                    timeSinceColorChange = 0;
                }
            }
            else
            {
                if (timeSinceColorChange >= emberColorChangeDelta)
                {
                    lastEmberColor = currentEmberColor;
                    currentEmberColor++;
                    if (currentEmberColor >= emberColors.Length)
                        currentEmberColor = 0;
                    timeSinceColorChange = 0;
                }
            }
        }
        #endregion
    }
}
