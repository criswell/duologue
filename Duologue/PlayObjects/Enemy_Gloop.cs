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
using Duologue.Audio;
using Duologue.Screens;
#endregion

namespace Duologue.PlayObjects
{
    public class Enemy_Gloop : Enemy
    {
        #region Constants
        private const string filename_glooplet = "Enemies/gloop/glooplet";
        private const string filename_gloopletHighlight = "Enemies/gloop/glooplet-highlight";
        private const string filename_gloopletDeath = "Enemies/gloop/glooplet-death";

        private const string filename_bubbles = "Audio/Gloop/bubbles1";

        private const string filename_Pop = "Audio/PlayerEffects/pop";
        private const float volume_Pop = 0.25f;

        private const double minSize = 0.5;
        private const double maxSize = 1.0;

        private const float radiusMultiplier = 0.8f;
        private const float outlineScale = 1.1f;
        private const float deltaRotate = 0.05f;

        private const byte maxHighlightAlpha = 245;
        private const byte minHighlightAlpha = 20;
        private const byte shieldAlpha = 128;
        private const int defaultHighlightAlphaDelta = 10;
        private double highlightTimer = 0.04;

        /// <summary>
        /// The min and max bubble volume
        /// </summary>
        private const double minBubbleVolume = 0.01;
        private const double maxBubbleVolume = 0.04;

        private const float deathLifetime = 0.7f;

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
        private Texture2D gloopletHighlight;
        private Texture2D gloopletDeath;
        private Vector2 gloopletCenter;
        private Vector2 gloopletHighlightCenter;
        private Vector2 gloopletDeathCenter;

        private float scale;
        private float deathRotation;
        private bool isFleeing;
        private bool isDying;

        private Color currentColor;
        private byte highlightAlpha;
        private int highlightAlphaDelta;

        private double timeSinceStart;

        // Movement
        private Vector2 offset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;
        private Vector2 nearestLeader;
        private float nearestLeaderRadius;
        private Enemy nearestLeaderObject;
        private Vector2 lastDirection;

        // Sound
        private SoundEffect sfx_Bubble;
        private SoundEffectInstance sfxi_Bubble;
        private float volume_CurrentBubble;
        private AudioManager audio;
        private SoundEffect sfx_Pop;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        /// <summary>
        /// The empty constructor for pre-caching
        /// </summary>
        public Enemy_Gloop() : base() { }

        public Enemy_Gloop(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Gloop;
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
            int? hitPoints,
            double spawnDelay)
        {
            Position = startPos;
            Orientation = startOrientation;
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
            glooplet = InstanceManager.AssetManager.LoadTexture2D(filename_glooplet);
            gloopletDeath = InstanceManager.AssetManager.LoadTexture2D(filename_gloopletDeath);
            gloopletHighlight = InstanceManager.AssetManager.LoadTexture2D(filename_gloopletHighlight);

            gloopletCenter = new Vector2(
                glooplet.Width / 2f,
                glooplet.Height / 2f);
            gloopletDeathCenter = new Vector2(
                gloopletDeath.Width / 2f,
                gloopletDeath.Height / 2f);
            gloopletHighlightCenter = new Vector2(
                gloopletHighlight.Width / 2f,
                gloopletHighlight.Height - 1f);

            sfx_Bubble = InstanceManager.AssetManager.LoadSoundEffect(filename_bubbles);
            volume_CurrentBubble = (float)minBubbleVolume;
            sfxi_Bubble = null;
            //sfxi_Bubble = sfx_Bubble.Play(volume_CurrentBubble);
            sfx_Pop = InstanceManager.AssetManager.LoadSoundEffect(filename_Pop);

            scale = (float)MWMathHelper.GetRandomInRange(minSize, maxSize);

            Radius = (glooplet.Width/2f) * scale * radiusMultiplier;

            isFleeing = false;

            currentColor = GetMyColor(ColorState.Dark);
            highlightAlpha = (byte)MWMathHelper.GetRandomInRange(minHighlightAlpha, maxHighlightAlpha);
            highlightAlphaDelta = defaultHighlightAlphaDelta;
            if (MWMathHelper.CoinToss())
                highlightAlphaDelta = highlightAlphaDelta * -1;

            timeSinceStart = 0.0;
            deathRotation = 0f;
            isDying = false;
            Initialized = true;
            Alive = true;
        }

        public override void CleanUp()
        {
            try
            {
                sfxi_Bubble.Stop();
            }
            catch { }
            base.CleanUp();
        }
        #endregion

        #region Public Overrides
        public override String[] GetTextureFilenames()
        {
            return new String[]
            {
                filename_glooplet,
                filename_gloopletDeath,
                filename_gloopletHighlight
            };
        }

        public override String[] GetSFXFilenames()
        {
            return new String[]
            {
                filename_bubbles,
                filename_Pop
            };
        }

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
                    isFleeing = false;
                    if (Position.X > 0 && Position.X < InstanceManager.DefaultViewport.Width &&
                    Position.Y > 0 && Position.Y < InstanceManager.DefaultViewport.Height)
                    {
                        // We only want to handle this stuff if we're onscreen
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
                    }
                    return true;
                }
                else if (pobj.MajorType == MajorPlayObjectType.Enemy)
                {
                    if (((Enemy)pobj).MyEnemyType == EnemyType.Leader && pobj.MyType != TypesOfPlayObjects.Enemy_Pyre)
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
                    try
                    {
                        sfxi_Bubble.Stop();
                    }
                    catch { }
                    sfx_Pop.Play(volume_Pop);

                    LocalInstanceManager.AchievementManager.EnemyDeathCount(MyType);
                    timeSinceStart = 0.0;
                    deathRotation = (float)MWMathHelper.GetRandomInRange(0.0, (double)MathHelper.TwoPi);
                    TriggerShieldDisintegration(gloopletDeath, currentColor, Position, 0f);
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue + hitPointMultiplier * StartHitPoints, Position);
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(gloopletDeath, new Color(currentColor, shieldAlpha), Position, 0f);
                   /*audio.soundEffects.PlayEffect(EffectID.CokeBottle);*/
                    audio.PlayEffect(EffectID.CokeBottle);
                    return true;
                }
            }
            return true;
        }
        #endregion

        #region Draw / Update
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
                    scale,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);
            }
            else
            {
                // Outline
                InstanceManager.RenderSprite.Draw(
                    glooplet,
                    Position,
                    gloopletCenter,
                    null,
                    Color.Black,
                    0f,
                    scale * outlineScale,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);

                // Proper
                InstanceManager.RenderSprite.Draw(
                    glooplet,
                    Position,
                    gloopletCenter,
                    null,
                    currentColor,
                    0f,
                    scale,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);

                // Highlight
                InstanceManager.RenderSprite.Draw(
                    gloopletHighlight,
                    Position,
                    gloopletHighlightCenter,
                    null,
                    new Color(Color.White, highlightAlpha),
                    0f,
                    scale,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
            }
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceStart += gameTime.ElapsedGameTime.TotalSeconds;

            if (isDying)
            {
                if (timeSinceStart > deathLifetime)
                {
                    Alive = false;
                }
                deathRotation += deltaRotate;
            }
            else
            {
                if (timeSinceStart > highlightTimer)
                {
                    timeSinceStart = 0.0;
                    highlightAlpha = (byte)((int)highlightAlpha + highlightAlphaDelta);
                    if (highlightAlpha > maxHighlightAlpha)
                    {
                        highlightAlpha = maxHighlightAlpha;
                        highlightAlphaDelta = defaultHighlightAlphaDelta * -1;
                    }
                    else if (highlightAlpha < minHighlightAlpha)
                    {
                        highlightAlpha = minHighlightAlpha;
                        highlightAlphaDelta = defaultHighlightAlphaDelta;
                    }
                }
                if (sfxi_Bubble == null)
                {
                    sfxi_Bubble = sfx_Bubble.Play(volume_CurrentBubble);
                }
                else if (sfxi_Bubble.State == SoundState.Stopped || sfxi_Bubble.State == SoundState.Paused)
                {
                    volume_CurrentBubble = (float)MWMathHelper.GetRandomInRange(minBubbleVolume, maxBubbleVolume);
                    sfxi_Bubble.Volume = volume_CurrentBubble;
                    sfxi_Bubble.Resume();
                }
            }
        }
        #endregion
    }
}
