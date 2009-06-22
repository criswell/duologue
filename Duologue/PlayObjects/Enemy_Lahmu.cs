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
    public enum LahmuState
    {
        Spawning,
        Moving,
        FreakOut,
        Death,
    }

    public class Enemy_Lahmu : Enemy
    {
        #region Constants
        private const string filename_Body = "Enemies/end/tent-body{0}";
        private const string filename_Outline = "Enemies/end/tent-out{0}";
        private const int frames_Tentacle = 3;
        private const string filename_EyeBase = "Enemies/gloop/prince-gloop-base";
        private const string filename_EyeBody = "Enemies/gloop/prince-gloop-body";
        private const string filename_EyePupil = "Enemies/gloop/king-gloop-eye";
        private const string filename_Flame = "Enemies/static-king-{0}";
        private const string filename_gloopletHighlight = "Enemies/gloop/glooplet-highlight";
        private const int frames_Flame = 4;
        private const string filename_SquidWalk = "Audio/PlayerEffects/squid-walk";
        private const string filename_BigBaddaBoom = "Audio/PlayerEffects/big-badda-boom";

        private const float volume_SquidWalk = 0.95f;
        private const float volume_Boom = 1f;

        private const double time_Spawning = 1f;
        private const double time_Moving = 4f;
        private const double time_FreakOut = 0.5f;
        private const double time_FreakBlip = 0.08f;
        private const double time_DeathBlip = 1.5f;
        private const double time_TotalDeath = 8f;

        private const double shieldCoolOffTime = 0.2;

        private const double time_TentacleRotate = 0.05;
        private const double time_TentacleAnimation = 0.1;

        /// <summary>
        /// Need to make it so that lahmu isn't as deadly when he first spawns
        /// </summary>
        private const double time_SpawnToDeadly = 3.1;

        /// <summary>
        /// This is the multiplier applied to my radius that determines how far away
        /// to spawn a babby
        /// </summary>
        private const float spawnDistanceMultiplier = 0.5f;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 1000;

        /// <summary>
        /// The multiplier for point value tweaks based upon hitpoints
        /// </summary>
        private const int hitPointMultiplier = 2;

        private const int myShieldPointValue = 2;

        private const float radiusMultiplier = 0.8f;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 50;

        /// <summary>
        /// The scale of the pupil
        /// </summary>
        private const float scale_eyePupil = 0.9f;

        private const float max_ScaleFlame = 1.1f;
        private const float min_ScaleFlame = 0.01f;
        private const int numberOfFlames = 10;
        private const float delta_SpawnFlameState = 0.07f;

        /// <summary>
        /// The max offset of the pupil
        /// </summary>
        private const float scale_eyeOffset = 20f;

        private const float verticalOffsetHighlight = -30f;
        private const float verticalOffsetFlameCenter = 30f;
        private const double minOffsetLengthExplosions = 35.0;
        private const double maxOffsetLengthExplosions = 75.0;

        /// <summary>
        /// How far we can go outside the screen before we should stop
        /// </summary>
        private const float outsideScreenMultiplier = 0.01f;

        #region Force interactions
        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        /// <summary>
        /// Min number of how many of our radius size away we're comfortable with the player
        /// </summary>
        private const float minPlayerComfortRadiusMultiplier = 3.4f;

        /// <summary>
        /// Max number of our radius we're comfortable with the player
        /// </summary>
        private const float maxPlayerComfortRadiusMultiplier = 5.1f;

        /// <summary>
        /// The player attract modifier
        /// </summary>
        private const float playerAttract = 2.1f;

        /// <summary>
        /// The minimum movement required before we register motion
        /// </summary>
        private const float minMovement = 2f;

        /// <summary>
        /// The repulsion from the player if the player gets too close
        /// this should be lower than attract so that the player can bump into them
        /// </summary>
        private const float playerRepulse = 1.5f;

        /// <summary>
        /// The rotate speed
        /// </summary>
        private const float rotateSpeed = 0.8f;

        /// <summary>
        /// The rotate speed during death
        /// </summary>
        private const float deathRotateSpeed = 1.6f;
        #endregion
        #endregion

        #region Fields
        // Images and animation stuff
        private Texture2D[] texture_Body;
        private Texture2D[] texture_Outline;
        private Texture2D[] texture_Flame;
        private Texture2D texture_EyeBase;
        private Texture2D texture_EyeBody;
        private Texture2D texture_EyePupil;
        private Texture2D texture_Highlight;
        private Vector2[] center_Body;
        private Vector2 center_Flame;
        private Vector2 center_EyeBase;
        private Vector2 center_EyeBody;
        private Vector2 center_EyePupil;
        private Vector2 center_Highlight;
        private float[] rotation_Tentacle;
        private float[] delta_RotationTentacle;
        private int[] currentFrame_Tentacles;
        private int[] delta_CurrentTentacleFrame;
        private Vector2 offset_eye;
        private Color[] eyeColor;
        private Color deathEyeColor;
        private int currentEyeColor;
        private Color[] currentLayerColors;
        private float[] offset_Explosions;
        private RenderSpriteBlendMode currentBlendMode;

        // State information
        private LahmuState currentState;
        private double timeSinceStateChange;
        private float[] spawnStateFlames;
        private float[] rotation_Flames;
        private int[] currentFlameFrame;
        private double timerFreakBlip;
        private double shieldCoolOff;
        private double timeSinceTentacleRotate;
        private double timeSinceTentacleWiggle;
        private double timeBeforeDangerous;

        // Movement stuff
        private Vector2 offset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;
        private bool isFleeing;

        // Audio stuff
        private AudioManager audio;
        private SoundEffect sfx_SquidWalk;
        private SoundEffectInstance sfxi_SquidWalk;
        private SoundEffect sfx_Boom;
        private SoundEffectInstance sfxi_Boom;
        #endregion

        #region Constructor / Init
        public Enemy_Lahmu() : base() { }

        public Enemy_Lahmu(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Lahmu;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Leader;
            Initialized = false;

            // Set the RealSize by hand, set this at max
            RealSize = new Vector2(284, 224);
        }

        public override void Initialize(
            Vector2 startPos, 
            Vector2 startOrientation, 
            ColorState currentColorState, 
            ColorPolarity startColorPolarity, 
            int? hitPoints,
            double spawnDelay)
        {
            // We say "fuck the requested starting pos"
            Position = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f, InstanceManager.DefaultViewport.Height / 2f);

            if (startOrientation == Vector2.Zero)
                Orientation = GetStartingVector();
            else
                Orientation = startOrientation;
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            SpawnTimeDelay = spawnDelay;
            SpawnTimer = 0;
            if (hitPoints == null || (int)hitPoints == 0)
            {
                hitPoints = 1;
            }
            StartHitPoints = (int)hitPoints * realHitPointMultiplier;
            CurrentHitPoints = (int)hitPoints * realHitPointMultiplier;
            audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            texture_EyeBase = InstanceManager.AssetManager.LoadTexture2D(filename_EyeBase);
            texture_EyeBody = InstanceManager.AssetManager.LoadTexture2D(filename_EyeBody);
            texture_EyePupil = InstanceManager.AssetManager.LoadTexture2D(filename_EyePupil);
            texture_Highlight = InstanceManager.AssetManager.LoadTexture2D(filename_gloopletHighlight);
            center_EyeBase = new Vector2(
                texture_EyeBase.Width / 2f, texture_EyeBase.Height / 2f);
            center_EyeBody = new Vector2(
                texture_EyeBody.Width / 2f, texture_EyeBody.Height / 2f);
            center_EyePupil = new Vector2(
                texture_EyePupil.Width / 2f, texture_EyePupil.Height / 2f);
            center_Highlight = new Vector2(
                texture_Highlight.Width / 2f, texture_Highlight.Height / 2f);

            texture_Flame = new Texture2D[frames_Flame];
            for (int i = 0; i < frames_Flame; i++)
            {
                texture_Flame[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Flame, (i + 1)));
            }
            center_Flame = new Vector2(
                texture_Flame[0].Width / 2f, texture_Flame[0].Height / 2f + verticalOffsetFlameCenter);

            texture_Outline = new Texture2D[frames_Tentacle];
            texture_Body = new Texture2D[frames_Tentacle];
            for (int i = 0; i < frames_Tentacle; i++)
            {
                texture_Body[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Body, (i + 1)));
                texture_Outline[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Outline, (i + 1)));
            }
            center_Body = new Vector2[]
            {
                new Vector2(137f, 117f),
                new Vector2(130f, 107f),
                new Vector2(139f, 91f)
            };

            currentFrame_Tentacles = new int[]
            {
                0, 1, 2
            };

            delta_RotationTentacle = new float[]
            {
                MathHelper.PiOver4 * 0.01f, MathHelper.PiOver4 * 0.05f, MathHelper.PiOver4 * 0.1f
            };

            delta_CurrentTentacleFrame = new int[]
            {
                1, 1, 1
            };

            Radius = radiusMultiplier * texture_Body[0].Width / 2f;

            rotation_Tentacle = new float[]
            {
                0,
                MathHelper.PiOver4,
                MathHelper.PiOver2
            };

            offset_eye = Vector2.Zero;

            eyeColor = new Color[]
            {
                new Color(160, 138,29),
                new Color(95, 208, 228),
                new Color(249, 85, 161),
                new Color(49, 200, 76),
            };
            currentEyeColor = 0;
            offset_eye = Vector2.Zero;
            SetCurrentColors();

            deathEyeColor = new Color(203, 0, 0);

            offset_Explosions = new float[]
            {
                0f,
                MathHelper.PiOver4 + MathHelper.PiOver4/2f,
                MathHelper.PiOver4 * 3f - MathHelper.PiOver4/2f,
                MathHelper.Pi,
                MathHelper.PiOver4 * 5f + MathHelper.PiOver4/2f,
                MathHelper.PiOver4 * 7f - MathHelper.PiOver4/2f
            };

            currentBlendMode = RenderSpriteBlendMode.AlphaBlendTop;

            sfx_SquidWalk = InstanceManager.AssetManager.LoadSoundEffect(filename_SquidWalk);
            sfxi_SquidWalk = null;
            sfx_Boom = InstanceManager.AssetManager.LoadSoundEffect(filename_BigBaddaBoom);
            sfxi_Boom = null;

            // Set up state stuff
            currentState = LahmuState.Spawning;
            timeSinceStateChange = 0;
            timeSinceTentacleRotate = 0;
            timeSinceTentacleWiggle = 0;
            timeBeforeDangerous = 0;
            shieldCoolOff = 0;
            rotation_Flames = new float[numberOfFlames];
            spawnStateFlames = new float[numberOfFlames];
            currentFlameFrame = new int[numberOfFlames];
            for (int i = 0; i < numberOfFlames; i++)
            {
                rotation_Flames[i] = (float)MWMathHelper.GetRandomInRange(0.0, (double)MathHelper.TwoPi);
                spawnStateFlames[i] = (float)MWMathHelper.GetRandomInRange(0, 1.0);
                currentFlameFrame[i] = MWMathHelper.GetRandomInRange(0, texture_Flame.Length);
            }

            Initialized = true;
            Alive = true;
        }

        public override String[] GetTextureFilenames()
        {
            String[] filenames = new String[frames_Tentacle * 2 + 4 + frames_Flame];
            int i = 0;
            for (int t = 0; t < frames_Tentacle; t++)
            {
                filenames[i] = String.Format(filename_Body, (t + 1));
                i++;
                filenames[i] = String.Format(filename_Outline, (t + 1));
                i++;
            }
            for (int t = 0; t < frames_Flame; t++)
            {
                filenames[i] = String.Format(filename_Flame, (t + 1));
                i++;
            }
            filenames[i] = filename_EyeBase;
            i++;
            filenames[i] = filename_EyeBody;
            i++;
            filenames[i] = filename_EyePupil;
            i++;
            filenames[i] = filename_gloopletHighlight;
            return filenames;
        }

        public override void CleanUp()
        {
            try
            {
                sfxi_SquidWalk.Stop();
            }
            catch { }
            base.CleanUp();
        }
        #endregion

        #region Public movement overrides
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
                if (len < this.Radius + pobj.Radius && timeBeforeDangerous < time_SpawnToDeadly)
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
                        LocalInstanceManager.Steam.AddParticles(Position, currentLayerColors[0]);
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
                    offset += standardEnemyRepulse * vToEnemy;
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
            if (nearestPlayer != Vector2.Zero && nearestPlayer.Length() > 0f)
            {
                offset_eye = Vector2.Negate(nearestPlayer);
                offset_eye.Normalize();
                offset_eye = offset_eye * scale_eyeOffset;
                // 1st priority is the player
                if (nearestPlayerRadius >= maxPlayerComfortRadiusMultiplier * Radius)
                {
                    // If we're outside our comfort radius, bear down on the player
                    // like a mad man
                    nearestPlayer.Normalize();

                    if (!isFleeing)
                        nearestPlayer = Vector2.Negate(nearestPlayer);

                    offset += playerAttract * nearestPlayer;
                }
                else if (nearestPlayerRadius < maxPlayerComfortRadiusMultiplier * Radius &&
                    nearestPlayerRadius >= minPlayerComfortRadiusMultiplier * Radius)
                {
                    // We're in our comfort zone, we just orbit the player
                    nearestPlayer.Normalize();

                    nearestPlayer = new Vector2(nearestPlayer.Y, -nearestPlayer.X);

                    if(currentState != LahmuState.Death)
                        offset += rotateSpeed * nearestPlayer;
                    else
                        offset += deathRotateSpeed * nearestPlayer;
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
                nearestPlayer = Orientation;

                nearestPlayer.Normalize();

                offset += playerAttract * nearestPlayer;

                offset_eye = Vector2.Zero;
            }

            this.Position += offset;
            Orientation = offset;
            Orientation.Normalize();

            if (offset_eye.Y < 0 && currentState != LahmuState.Death)
                offset_eye.Y = 0;

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
            if (pobj.MajorType == MajorPlayObjectType.PlayerBullet
                && shieldCoolOff >= shieldCoolOffTime && currentState != LahmuState.Death)
            {
                CurrentHitPoints--;
                shieldCoolOff = 0;
                if (CurrentHitPoints <= 0)
                {
                    try
                    {
                        sfxi_SquidWalk.Stop();
                    }
                    catch { }
                    try
                    {
                        sfxi_Boom = sfx_Boom.Play(volume_Boom);
                    }
                    catch { }
                    //Alive = false;
                    //LocalInstanceManager.AchievementManager.EnemyDeathCount(MyType);
                    //LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                            //Position, currentLayerColors[1]);
                    //LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                        //Position, eyeColor[currentEyeColor]);
                    timerFreakBlip = time_DeathBlip;
                    timeSinceStateChange = 0;
                    currentBlendMode = RenderSpriteBlendMode.AlphaBlend;
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue, Position);

                    currentState = LahmuState.Death;
                    return true;
                }
                else
                {
                    TriggerShieldDisintegration(
                        texture_Flame[MWMathHelper.GetRandomInRange(0, texture_Flame.Length)],
                        currentLayerColors[1],
                        Position,
                        0f);
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myShieldPointValue, Position);
                    /*audio.soundEffects.PlayEffect(EffectID.CokeBottle);*/
                    audio.PlayEffect(EffectID.CokeBottle);
                    return true;
                }
            }
            return true;
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

        private void SetCurrentColors()
        {
            currentLayerColors = new Color[]
            {
                GetMyColor(ColorState.Dark),
                GetMyColor(ColorState.Medium),
                GetMyColor(ColorState.Light)
            };
        }

        private Vector2 GetBabbySpawnPosition()
        {
            Vector2 temp;

            temp = new Vector2(
                (float)MWMathHelper.GetRandomInRange(-1.0, 1.0),
                (float)MWMathHelper.GetRandomInRange(-1.0, 1.0));
            temp.Normalize();
            temp = temp * spawnDistanceMultiplier * Radius;

            return temp + Position;
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            float scale = 1f;
            if (currentState == LahmuState.Spawning)
            {
                scale = (float)(timeSinceStateChange / time_Spawning);
                // Draw the spawning flames
                for (int i = 0; i < numberOfFlames; i++)
                {
                    InstanceManager.RenderSprite.Draw(
                        texture_Flame[currentFlameFrame[i]],
                        Position,
                        center_Flame,
                        null,
                        new Color(currentLayerColors[currentLayerColors.Length - 1], MathHelper.Lerp(0, 0.7f, spawnStateFlames[i])),
                        rotation_Flames[i],
                        MathHelper.Lerp(max_ScaleFlame, min_ScaleFlame, spawnStateFlames[i]),
                        0f,
                        RenderSpriteBlendMode.AddititiveTop);
                }
            }

            // Draw tenticles
            for (int i = 0; i < currentFrame_Tentacles.Length; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    texture_Body[currentFrame_Tentacles[i]],
                    Position,
                    center_Body[currentFrame_Tentacles[i]],
                    null,
                    currentLayerColors[i],
                    rotation_Tentacle[i],
                    scale,
                    0f,
                    currentBlendMode);
                InstanceManager.RenderSprite.Draw(
                    texture_Outline[currentFrame_Tentacles[i]],
                    Position,
                    center_Body[currentFrame_Tentacles[i]],
                    null,
                    Color.White,
                    rotation_Tentacle[i],
                    scale,
                    0f,
                    currentBlendMode);
            }

            // Eye base
            if (currentState != LahmuState.Death)
            {
                InstanceManager.RenderSprite.Draw(
                    texture_EyeBase,
                    Position,
                    center_EyeBase,
                    null,
                    Color.White,
                    0f,
                    scale,
                    0f,
                    currentBlendMode);
            }
            else
            {
                InstanceManager.RenderSprite.Draw(
                    texture_EyeBase,
                    Position,
                    center_EyeBase,
                    null,
                    deathEyeColor,
                    0f,
                    scale,
                    0f,
                    currentBlendMode);
            }

            // Pupil
            InstanceManager.RenderSprite.Draw(
                texture_EyePupil,
                Position + scale * offset_eye,
                center_EyePupil,
                null,
                eyeColor[currentEyeColor],
                0f,
                scale_eyePupil * scale,
                0f,
                currentBlendMode);

            // Body
            if (currentState != LahmuState.Death)
            {
                InstanceManager.RenderSprite.Draw(
                    texture_EyeBody,
                    Position,
                    center_EyeBody,
                    null,
                    currentLayerColors[currentLayerColors.Length - 1],
                    0f,
                    scale,
                    0f,
                    currentBlendMode);

                // Highlight
                InstanceManager.RenderSprite.Draw(
                    texture_Highlight,
                    Position + Vector2.UnitY * verticalOffsetHighlight * scale,
                    center_Highlight,
                    null,
                    Color.White,
                    0f,
                    scale,
                    0f,
                    currentBlendMode);
            }
        }

        public override void Update(GameTime gameTime)
        {
            double timePassed = gameTime.ElapsedGameTime.TotalSeconds;
            
            if (timeBeforeDangerous <= time_SpawnToDeadly)
                timeBeforeDangerous += timePassed;

            shieldCoolOff += timePassed;
            if (shieldCoolOff > shieldCoolOffTime)
                shieldCoolOff = shieldCoolOffTime;

            timeSinceStateChange += timePassed;
            if (currentState == LahmuState.Death)
            {
                if (sfxi_Boom == null)
                {
                    try
                    {
                        sfxi_Boom = sfx_Boom.Play(volume_Boom);
                    }
                    catch { }
                }
                else if (sfxi_Boom.State == SoundState.Stopped ||
                         sfxi_Boom.State == SoundState.Paused)
                {
                    try
                    {
                        sfxi_Boom.Play();
                    }
                    catch { }
                }

                if (timeSinceStateChange > time_TotalDeath)
                {
                    timeSinceStateChange = 0;
                    try
                    {
                        sfxi_Boom.Stop();
                    }
                    catch { }
                    try
                    {
                        sfxi_SquidWalk.Stop();
                    }
                    catch { }
                    Alive = false;
                    LocalInstanceManager.AchievementManager.EnemyDeathCount(MyType);
                }
                else
                {
                    timerFreakBlip += timePassed;
                    if (timerFreakBlip > time_DeathBlip)
                    {
                        if (ColorPolarity == ColorPolarity.Negative)
                            ColorPolarity = ColorPolarity.Positive;
                        else
                            ColorPolarity = ColorPolarity.Negative;
                        timerFreakBlip = 0;
                        SetCurrentColors();
                        currentEyeColor++;
                        if (currentEyeColor >= eyeColor.Length)
                            currentEyeColor = 0;
                        // Fire off some explosions
                        float additionalRotation = (float)MWMathHelper.GetRandomInRange(0, (double)MathHelper.TwoPi);
                        Color tempColor;
                        for (int i = 0; i < offset_Explosions.Length; i++)
                        {
                            if (MWMathHelper.CoinToss())
                                tempColor = eyeColor[currentEyeColor];
                            else
                                tempColor = currentLayerColors[MWMathHelper.GetRandomInRange(0, currentLayerColors.Length)];

                            if (MWMathHelper.CoinToss())
                            {
                                LocalInstanceManager.EnemySplatterSystem.AddParticles(
                                    Position + MWMathHelper.RotateVectorByRadians(
                                       (float)MWMathHelper.GetRandomInRange(minOffsetLengthExplosions, maxOffsetLengthExplosions) *
                                       Vector2.UnitX, additionalRotation + offset_Explosions[i]),
                                    tempColor);
                            }
                            else
                            {
                                LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                                    Position + MWMathHelper.RotateVectorByRadians(
                                       (float)MWMathHelper.GetRandomInRange(minOffsetLengthExplosions, maxOffsetLengthExplosions) *
                                       Vector2.UnitX, additionalRotation + offset_Explosions[i]),
                                    tempColor);
                            }
                        }
                    }
                }
            } 
            else if (currentState == LahmuState.Spawning)
            {
                if (timeSinceStateChange > time_Spawning)
                {
                    timeSinceStateChange = 0;
                    currentState = LahmuState.Moving;
                }
                else
                {
                    for (int i = 0; i < numberOfFlames; i++)
                    {
                        spawnStateFlames[i] += delta_SpawnFlameState;
                        if (spawnStateFlames[i] > 1)
                        {
                            spawnStateFlames[i] = 0;
                            rotation_Flames[i] = (float)MWMathHelper.GetRandomInRange(0.0, (double)MathHelper.TwoPi);
                            currentFlameFrame[i] = MWMathHelper.GetRandomInRange(0, texture_Flame.Length);
                        }
                    }
                }
            }
            else if (currentState == LahmuState.FreakOut)
            {
                if (timeSinceStateChange > time_FreakOut)
                {
                    timeSinceStateChange = 0;
                    if (ColorPolarity == ColorPolarity.Negative)
                        ColorPolarity = ColorPolarity.Positive;
                    else
                        ColorPolarity = ColorPolarity.Negative;
                    timerFreakBlip = 0;
                    SetCurrentColors();
                    currentState = LahmuState.Moving;
                }
                else
                {
                    timerFreakBlip += timePassed;
                    if (timerFreakBlip > time_FreakBlip)
                    {
                        if (ColorPolarity == ColorPolarity.Negative)
                            ColorPolarity = ColorPolarity.Positive;
                        else
                            ColorPolarity = ColorPolarity.Negative;
                        timerFreakBlip = 0;
                        SetCurrentColors();
                        currentEyeColor++;
                        if (currentEyeColor >= eyeColor.Length)
                            currentEyeColor = 0;
                        // Run through the other enemy objects, looking for dead ones
                        if (offset_eye != Vector2.Zero)
                        {
                            for (int i = 0; i < LocalInstanceManager.CurrentNumberEnemies; i++)
                            {
                                if (!LocalInstanceManager.Enemies[i].Alive)
                                {
                                    if (MWMathHelper.CoinToss())
                                        LocalInstanceManager.Enemies[i] =
                                            new Enemy_Flycket(MyManager);
                                    else
                                        LocalInstanceManager.Enemies[i] =
                                            new Enemy_Firefly(MyManager);

                                    LocalInstanceManager.Enemies[i].Initialize(
                                        GetBabbySpawnPosition(),
                                        Vector2.Zero,
                                        ColorState,
                                        ColorPolarity,
                                        (int)(StartHitPoints / (float)realHitPointMultiplier),
                                        0);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (sfxi_SquidWalk == null)
                {
                    try
                    {
                        sfxi_SquidWalk = sfx_SquidWalk.Play(volume_SquidWalk);
                    }
                    catch { }
                }
                else if (sfxi_SquidWalk.State == SoundState.Stopped ||
                    sfxi_SquidWalk.State == SoundState.Paused)
                {
                    try
                    {
                        sfxi_SquidWalk.Play();
                    }
                    catch { }
                }

                if (timeSinceStateChange > time_Moving)
                {
                    timeSinceStateChange = 0;
                    timerFreakBlip = time_FreakBlip;
                    currentState = LahmuState.FreakOut;
                }
            }

            // Update the tentacle rotations
            timeSinceTentacleRotate += timePassed;
            if (timeSinceTentacleRotate > time_TentacleRotate)
            {
                for (int i = 0; i < currentFrame_Tentacles.Length; i++)
                {
                    rotation_Tentacle[i] += delta_RotationTentacle[i];
                    if (rotation_Tentacle[i] > MathHelper.TwoPi)
                        rotation_Tentacle[i] -= MathHelper.TwoPi;
                    else if (rotation_Tentacle[i] < 0)
                        rotation_Tentacle[i] += MathHelper.TwoPi;
                }
                timeSinceTentacleRotate = 0;
            }
            timeSinceTentacleWiggle += timePassed;
            if (timeSinceTentacleWiggle > time_TentacleAnimation)
            {
                for (int i = 0; i < currentFrame_Tentacles.Length; i++)
                {
                    currentFrame_Tentacles[i] += delta_CurrentTentacleFrame[i];
                    if (currentFrame_Tentacles[i] >= frames_Tentacle)
                    {
                        currentFrame_Tentacles[i] = frames_Tentacle - 2;
                        delta_CurrentTentacleFrame[i] = -1;
                    }
                    else if (currentFrame_Tentacles[i] < 0)
                    {
                        currentFrame_Tentacles[i] = 1;
                        delta_CurrentTentacleFrame[i] = 1;
                    }
                }
                timeSinceTentacleWiggle = 0;
            }
        }
        #endregion
    }
}
