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
    public struct StaticElement
    {
        public Rectangle Rect;
        public byte Alpha;
        public int DeltaAlpha;
        public RenderSpriteBlendMode BlendMode;
        public Vector2 Speed;
    }

    class Enemy_StaticGloop : Enemy
    {
        #region Constants
        private const string filename_glooplet = "Enemies/gloop/glooplet";
        private const string filename_static = "Enemies/gloop/static";
        private const string filename_gloopletDeath = "Enemies/gloop/static-death";

        private const float defaultSize = 0.8f;

        private const float radiusMultiplier = 0.8f;
        private const float outlineScale = 1.1f;
        private const float deltaRotate = 0.05f;

        private const byte shieldAlpha = 128;

        private const float deathLifetime = 0.7f;

        /// <summary>
        /// The minimum width of the square for the static
        /// </summary>
        private const int minWidth = 3;
        /// <summary>
        /// The maximum width of the square for the static
        /// </summary>
        private const int maxWidth = 11;

        /// <summary>
        /// The minimum height of the square for the static
        /// </summary>
        private const int minHeight = 3;
        /// <summary>
        /// The maximum height of the square for the static
        /// </summary>
        private const int maxHeight = 8;

        /// <summary>
        /// The minimum static square speed
        /// </summary>
        private const double minSpeed = -3f;
        /// <summary>
        /// The maximum static square speed
        /// </summary>
        private const double maxSpeed = 3f;

        /// <summary>
        /// The number of static squares
        /// </summary>
        private const int numberOfStaticSquares = 15;

        private const byte minStaticSquareAlpha = 20;
        private const byte maxStaticSquareAlpha = 245;
        private const byte deltaStaticSquareAlpha = 10;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 25;

        /// <summary>
        /// The multiplier for point value tweaks based upon hitpoints
        /// </summary>
        private const int hitPointMultiplier = 3;

        /// <summary>
        /// How far we can go outside the screen before we should stop
        /// </summary>
        private const float outsideScreenMultiplier = 3;

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
        /// The player attract modifier
        /// </summary>
        private const float playerAttract = 2.1f;

        /// <summary>
        /// The minimum movement required before we register motion
        /// </summary>
        private const float minMovement = 2f;
        #endregion
        #endregion

        #region Fields
        private Texture2D glooplet;
        private Texture2D gloopletStatic;
        private Texture2D gloopletDeath;
        private Vector2 gloopletCenter;
        private Vector2 gloopletStaticCenter;
        private Vector2 gloopletDeathCenter;

        private float deathRotation;
        private bool isFleeing;
        private bool isDying;

        private Color currentColor;

        private double timeSinceStart;

        private StaticElement[] squareElements;

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
        public Enemy_StaticGloop(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_StaticGloop;
            MajorType = MajorPlayObjectType.Enemy;
            RealSize = new Vector2(83, 83);
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
            //audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            glooplet = InstanceManager.AssetManager.LoadTexture2D(filename_glooplet);
            gloopletDeath = InstanceManager.AssetManager.LoadTexture2D(filename_gloopletDeath);
            gloopletStatic = InstanceManager.AssetManager.LoadTexture2D(filename_static);

            gloopletCenter = new Vector2(
                glooplet.Width / 2f,
                glooplet.Height / 2f);
            gloopletDeathCenter = new Vector2(
                gloopletDeath.Width / 2f,
                gloopletDeath.Height / 2f);
            gloopletStaticCenter = new Vector2(
                gloopletStatic.Width / 2f,
                gloopletStatic.Height - 1f);
            
            Radius = (glooplet.Width / 2f) * defaultSize * radiusMultiplier;

            squareElements = new StaticElement[numberOfStaticSquares];
            for (int i = 0; i < numberOfStaticSquares; i++)
            {
                squareElements[i] = GetStaticElement();
            }

            isFleeing = false;

            currentColor = GetMyColor(ColorState.Dark);

            timeSinceStart = 0.0;
            deathRotation = 0f;
            isDying = false;
            Initialized = true;
            Alive = true;
        }
        #endregion

        #region Private Methods
        private StaticElement GetStaticElement()
        {
            StaticElement e = new StaticElement();
            int w = MWMathHelper.GetRandomInRange(minWidth, maxWidth);
            int h = MWMathHelper.GetRandomInRange(minHeight, maxHeight);

            e.Rect = new Rectangle(
                MWMathHelper.GetRandomInRange(0, (int)(RealSize.X - w)),
                MWMathHelper.GetRandomInRange(0, (int)(RealSize.Y - h)),
                w, h);

            e.Speed = new Vector2(
                (float)MWMathHelper.GetRandomInRange(minSpeed, maxSpeed),
                (float)MWMathHelper.GetRandomInRange(minSpeed, maxSpeed));

            e.Alpha = (byte)MWMathHelper.GetRandomInRange(minStaticSquareAlpha, maxStaticSquareAlpha);

            e.DeltaAlpha = deltaStaticSquareAlpha;

            if (MWMathHelper.CoinToss())
                e.BlendMode = RenderSpriteBlendMode.Addititive;
            else
                e.BlendMode = RenderSpriteBlendMode.AlphaBlend;

            return e;
        }
        #endregion

        #region Public Overrides
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
            if (!isDying)
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
                    //inBeam = false;
                    isFleeing = false;
                    if (temp != 0)
                    {
                        //inBeam = true;
                        if (temp == -1)
                        {
                            isFleeing = true;
                            Color c = ColorState.Negative[ColorState.Light];
                            if (ColorPolarity == ColorPolarity.Positive)
                                c = ColorState.Positive[ColorState.Light];
                            LocalInstanceManager.Steam.AddParticles(Position, c);
                        }
                    }
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
            return true;
        }

        public override bool ApplyOffset()
        {
            if (!isDying)
            {
                if (nearestLeader.Length() > 0f)
                {
                    // The leader comes first
                    nearestLeader.Normalize();

                    offset += leaderAttract * Vector2.Negate(nearestLeader);
                    ColorPolarity = nearestLeaderObject.ColorPolarity;
                    currentColor = GetMyColor(ColorState.Dark);
                }
                else if (nearestPlayer.Length() > 0f)
                {
                    // Next priority is the player
                    nearestPlayer.Normalize();

                    if (!isFleeing)
                        nearestPlayer = Vector2.Negate(nearestPlayer);

                    offset += playerAttract * nearestPlayer;
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
                if (CurrentHitPoints <= 0 && !isDying)
                {
                    isDying = true;
                    timeSinceStart = 0.0;
                    deathRotation = (float)MWMathHelper.GetRandomInRange(0.0, (double)MathHelper.TwoPi);
                    TriggerShieldDisintegration(gloopletDeath, currentColor, Position, 0f);
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue + hitPointMultiplier * StartHitPoints, Position);
                    /*audio.soundEffects.PlayEffect(EffectID.BuzzDeath);*/
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(gloopletDeath, new Color(currentColor, shieldAlpha), Position, 0f);
                    /*audio.soundEffects.PlayEffect(EffectID.CokeBottle);*/
                    return true;
                }
            }
            return true;
        }
        #endregion

        public override void Draw(GameTime gameTime)
        {
            if (isDying)
            {
                InstanceManager.RenderSprite.Draw(
                    gloopletDeath,
                    Position,
                    gloopletDeathCenter,
                    null,
                    currentColor,
                    deathRotation,
                    defaultSize,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);
            }
            else
            {
                // Static stuff
                for (int i = 0; i < numberOfStaticSquares; i++)
                {
                }

                // Outline
                InstanceManager.RenderSprite.Draw(
                    glooplet,
                    Position,
                    gloopletCenter,
                    null,
                    Color.Black,
                    0f,
                    defaultSize * outlineScale,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);

                // Proper
            }
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
