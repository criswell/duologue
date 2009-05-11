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
    public class Enemy_Flycket : Enemy
    {
        #region Constants
        private const string filename_Body = "Enemies/flycket-body";
        private const string filename_Wings = "Enemies/flycket-wings";
        private const string filename_Fire = "Enemies/flycket-fire{0}";
        private const int numberOfFireFrames = 3;
        private const string filename_Smoke = "Enemies/spitter/spit-1"; // We only use one of these
        private const int numberOfSmokeParticles = 5;
        private const string filename_Scream = "Audio/PlayerEffects/flycket-scream";
        private const float volume_Max = 0.55f;
        private const float volume_Min = 0.01f;
        private const string filename_Explode = "Audio/PlayerEffects/standard-enemy-explode";
        private const float alpha_MaxSmokeParticles = 0.55f;
        private const float alpha_MinSmokeParticles = 0.01f;
        private const float scale_MinSmokeParticle = 0.9f;
        private const float scale_MaxSmokeParticle = 2.0f;
        private const float delta_StateSmokeParticles = 0.02f;
        private const float delta_VerticalSmokeParticleOffset = -0.84f;
        private const float distance_SmokeSpawn = 8f;
        private const float delta_Rotation = MathHelper.PiOver4 / 8f;

        private const float scale_LastFire = 1.02f;

        private const float radiusMultiplier = 0.9f;

        private const float rotate_Max = MathHelper.PiOver4 * 0.0099f;
        //private const float rotate_Min = -MathHelper.PiOver4;

        private const float outsideScreenMultiplier = 2.1f;

        private const double timeToStopScreaming = 0.1f;
        private const double timeToSwitchFire = 0.1f;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 10;

        /// <summary>
        /// The multiplier for point value tweaks based upon hitpoints
        /// </summary>
        private const int hitPointMultiplier = 2;

        #region Force interactions
        private const float standardSpeed = 2.5f;
        private const float lightAttractSpeed = 3.5f;
        private const float lightRepulseSpeed = 1.75f;

        private const float minMovement = 1.5f;
        #endregion
        #endregion

        #region Fields
        // Image related items
        private Texture2D texture_Body;
        private Texture2D texture_Wings;
        private Texture2D[] texture_Fire;
        private Texture2D texture_Smoke;
        private Vector2 center_Body;
        private Vector2 center_Smoke;
        private Vector2[] position_SmokeParticles;
        private float[] state_SmokeParticles;
        private float[] rotation_SmokeParticles;
        private bool[] deltaSign_SmokeParticles;
        private Color myColor;
        private Color altColor;
        private float rotation;

        // Sound related
        private SoundEffect sfx_Scream;
        private SoundEffectInstance sfxi_Scream;
        private SoundEffect sfx_Explode;
        private AudioManager audio;

        // State related
        private bool hasSpawned;
        private bool stopThatInfernalScreamingWoman;
        private double timer_stopScreaming;
        private int currentFire;
        private int lastFire;
        private double timer_SinceFireSwitch;

        // Player tracking & movement related
        private Player trackedPlayerObject;
        private Vector2 offset;
        private Vector2 trackedPlayerVector;
        private float trackedPlayerDistance;
        private bool inBeam;
        private bool isFleeing;
        #endregion

        #region Properties
        #endregion

        #region Constructor/Init
        public Enemy_Flycket() : base() { }

        public Enemy_Flycket(GamePlayScreenManager manager) :
            base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Flycket;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Follower;
            Initialized = false;

            // Set the RealSize by hand, set this at max
            RealSize = new Vector2(90, 90);
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

            hasSpawned = false;
            stopThatInfernalScreamingWoman = false;
            timer_stopScreaming = 0.0;
            Alive = true;

            timer_SinceFireSwitch = 0.0;
            currentFire = 0;
            lastFire = 1;
            rotation = 0f;

            if (!Initialized)
                LoadAndInitialize();

            ClearSmokeParticles();
        }

        private void LoadAndInitialize()
        {
            texture_Body = InstanceManager.AssetManager.LoadTexture2D(filename_Body);
            texture_Smoke = InstanceManager.AssetManager.LoadTexture2D(filename_Smoke);
            texture_Wings = InstanceManager.AssetManager.LoadTexture2D(filename_Wings);

            texture_Fire = new Texture2D[numberOfFireFrames];
            for (int i = 0; i < numberOfFireFrames; i++)
            {
                texture_Fire[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Fire, (i + 1)));
            }

            center_Body = new Vector2(
                texture_Body.Width / 2f, texture_Body.Height / 2f);
            center_Smoke = new Vector2(
                texture_Smoke.Width / 2f, texture_Smoke.Height / 2f);

            myColor = GetMyColor(ColorState.Medium);
            altColor = Color.WhiteSmoke;

            Radius = center_Body.X * radiusMultiplier;

            sfx_Scream = InstanceManager.AssetManager.LoadSoundEffect(filename_Scream);
            sfx_Explode = InstanceManager.AssetManager.LoadSoundEffect(filename_Explode);
            audio = ServiceLocator.GetService<AudioManager>();

            // set up the smoke particles
            position_SmokeParticles = new Vector2[numberOfSmokeParticles];
            state_SmokeParticles = new float[numberOfSmokeParticles];
            rotation_SmokeParticles = new float[numberOfSmokeParticles];
            deltaSign_SmokeParticles = new bool[numberOfSmokeParticles];

            Initialized = true;
        }

        public override String[] GetTextureFilenames()
        {
            String[] filenames = new String[3 + numberOfFireFrames];

            int i;
            for (i = 0; i < numberOfFireFrames; i++)
            {
                filenames[i] = String.Format(filename_Fire, (i + 1));
            }

            filenames[i] = filename_Body;
            i++;
            filenames[i] = filename_Wings;
            i++;
            filenames[i] = filename_Smoke;

            return filenames;
        }

        public override String[] GetSFXFilenames()
        {
            return new String[]
            {
                filename_Scream,
                filename_Explode,
            };
        }
        #endregion

        #region Movement AI overrides
        public override bool StartOffset()
        {
            trackedPlayerObject = null;
            trackedPlayerVector = Vector2.Zero;
            offset = Vector2.Zero;
            if (hasSpawned)
            {
                // If we've already gotten onto the screen, we want the nearest player
                trackedPlayerDistance = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            }
            else
            {
                // If we haven't gotten onto the screen, pick the farthest player to make this fair
                trackedPlayerDistance = 0;
            }
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Player && !stopThatInfernalScreamingWoman)
            {
                // Player
                Vector2 vToPlayer = this.Position - pobj.Position;
                float len = vToPlayer.Length();

                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em and asplode ourselves
                    stopThatInfernalScreamingWoman = true;
                    LocalInstanceManager.EnemyExplodeSystem.AddParticles(Position, myColor);
                    LocalInstanceManager.EnemyExplodeSystem.AddParticles(Position, altColor);
                    return pobj.TriggerHit(this);
                }

                int temp = ((Player)pobj).IsInBeam(this);
                inBeam = false;
                isFleeing = false;
                if (temp != 0)
                {
                    inBeam = true;
                    if (temp == -1)
                    {
                        isFleeing = true;
                        LocalInstanceManager.Steam.AddParticles(Position, myColor);
                    }
                }

                if (hasSpawned)
                {
                    // If we've already gotten onto the screen, we want the nearest player
                    if (len < trackedPlayerDistance)
                    {
                        trackedPlayerDistance = len;
                        trackedPlayerObject = (Player)pobj;
                        trackedPlayerVector = vToPlayer;
                    }
                }
                else
                {
                    // If we haven't gotten onto the screen, pick the farthest player to make this fair
                    if (len > trackedPlayerDistance)
                    {
                        trackedPlayerDistance = len;
                        trackedPlayerObject = (Player)pobj;
                        trackedPlayerVector = vToPlayer;
                    }
                }
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            if (!stopThatInfernalScreamingWoman)
            {
                if (trackedPlayerObject != null)
                {
                    hasSpawned = true;
                    // We only alter our tragectory when we have a player to go after
                    trackedPlayerVector.Normalize();
                    trackedPlayerVector = Vector2.Negate(trackedPlayerVector);
                    if (Orientation == Vector2.Zero)
                    {
                        Orientation = trackedPlayerVector;
                    }
                    
                    float angle = MWMathHelper.ComputeAngleAgainstX(trackedPlayerVector, Orientation);
                    if (angle > rotate_Max)
                        angle = rotate_Max;
                    else if (angle < -rotate_Max)
                        angle = -rotate_Max;

                    if(angle != 0)
                        Orientation = Vector2.Transform(Orientation, 
                            Matrix.CreateRotationZ(angle));

                    offset += Vector2.Normalize(Orientation) * standardSpeed;

                    if (inBeam && !isFleeing)
                    {
                        offset += Vector2.Normalize(Orientation) * lightAttractSpeed;
                    }
                    else
                    {
                        offset += Vector2.Normalize(Orientation) * standardSpeed;
                    }

                    if (isFleeing)
                    {
                        offset += Vector2.Negate(Vector2.Normalize(Orientation)) * lightRepulseSpeed;
                    }
                }
                else if (hasSpawned)
                {
                    // If there's no player, but we've spawned, we keep moving in last tragectory
                    if (Orientation == Vector2.Zero)
                    {
                        // Hmmm, okay, just aim at the center of the screen
                        Orientation = GetStartingVector();
                    }
                    offset += Orientation * standardSpeed;
                }

                if (offset.Length() >= minMovement)
                {
                    Position += offset;
                    offset.Normalize();
                    Orientation = offset;
                    rotation = MWMathHelper.ComputeAngleAgainstX(Orientation) + MathHelper.PiOver2;
                }

                // Check boundaries - Once we move off the screen after spawned, we just die
                if (this.Position.X < -1 * RealSize.X * outsideScreenMultiplier ||
                    this.Position.Y < -1 * RealSize.Y * outsideScreenMultiplier ||
                    this.Position.Y > InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier ||
                    this.Position.X > InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier)
                {
                    if (hasSpawned && !stopThatInfernalScreamingWoman)
                    {
                        stopThatInfernalScreamingWoman = true;
                    }
                } /*else if (!hasSpawned &&
                    ((this.Position.X > 0 && this.Position.X < InstanceManager.DefaultViewport.Width) ||
                    (this.Position.Y > 0 && this.Position.Y < InstanceManager.DefaultViewport.Height)))
                {
                    hasSpawned = true;
                }*/
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
                    LocalInstanceManager.EnemyExplodeSystem.AddParticles(Position, altColor);
                    LocalInstanceManager.AchievementManager.EnemyDeathCount(MyType);
                    stopThatInfernalScreamingWoman = true;
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue + hitPointMultiplier * StartHitPoints, Position);
                    sfx_Explode.Play();
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(texture_Body, myColor, Position, 0f);
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

        private void ClearSmokeParticles()
        {
            for (int i = 0; i < numberOfSmokeParticles; i++)
            {
                position_SmokeParticles[i] = Vector2.Zero;
                state_SmokeParticles[i] = MathHelper.Lerp(0f, 1f, i / (float)numberOfSmokeParticles);
                rotation_SmokeParticles[i] = 0f;
                deltaSign_SmokeParticles[i] = MWMathHelper.CoinToss();
            }
        }
        #endregion

        #region Private Draw / Update
        private void DrawSmokeParticles(GameTime gameTime)
        {
            for (int i = 0; i < numberOfFireFrames; i++)
            {
                if (state_SmokeParticles[i] > 0)
                {
                    InstanceManager.RenderSprite.Draw(
                        texture_Smoke,
                        position_SmokeParticles[i],
                        center_Smoke,
                        null,
                        new Color(altColor, MathHelper.Lerp(alpha_MinSmokeParticles, alpha_MaxSmokeParticles, state_SmokeParticles[i])),
                        rotation_SmokeParticles[i],
                        MathHelper.Lerp(scale_MinSmokeParticle, scale_MaxSmokeParticle, state_SmokeParticles[i]),
                        0f,
                        RenderSpriteBlendMode.AlphaBlendTop);
                }
            }
        }

        private void UpdateSmokeParticles(GameTime gameTime)
        {
            for (int i = 0; i < numberOfSmokeParticles; i++)
            {
                state_SmokeParticles[i] -= delta_StateSmokeParticles;
                if (state_SmokeParticles[i] < 0f)
                {
                    if (stopThatInfernalScreamingWoman)
                        state_SmokeParticles[i] = 0f;
                    else
                    {
                        state_SmokeParticles[i] = 1f;
                        deltaSign_SmokeParticles[i] = MWMathHelper.CoinToss();
                        position_SmokeParticles[i] = Position + Vector2.Normalize(Vector2.Negate(Orientation)) * distance_SmokeSpawn;
                    }
                }
                else
                {
                    position_SmokeParticles[i] += Vector2.UnitY * delta_VerticalSmokeParticleOffset;
                    if(deltaSign_SmokeParticles[i])
                        rotation_SmokeParticles[i] += delta_Rotation;
                    else
                        rotation_SmokeParticles[i] -= delta_Rotation;

                    if (rotation_SmokeParticles[i] < 0)
                        rotation_SmokeParticles[i] = MathHelper.TwoPi;
                    else if (rotation_SmokeParticles[i] > MathHelper.TwoPi)
                        rotation_SmokeParticles[i] = 0;
                }
            }
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            if (!stopThatInfernalScreamingWoman)
            {
                // Draw the fire first
                InstanceManager.RenderSprite.Draw(
                    texture_Fire[currentFire],
                    Position,
                    center_Body,
                    null,
                    Color.White,
                    rotation,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);
                InstanceManager.RenderSprite.Draw(
                    texture_Fire[lastFire],
                    Position,
                    center_Body,
                    null,
                    new Color(Color.White, MathHelper.Lerp(0, 1f, (float)(timer_SinceFireSwitch / timeToSwitchFire))),
                    rotation,
                    scale_LastFire,
                    0f,
                    RenderSpriteBlendMode.Addititive);
                // Draw the wings and base
                InstanceManager.RenderSprite.Draw(
                    texture_Wings,
                    Position,
                    center_Body,
                    null,
                    Color.White,
                    rotation,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);
                InstanceManager.RenderSprite.Draw(
                    texture_Body,
                    Position,
                    center_Body,
                    null,
                    myColor,
                    rotation,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);
            }

            DrawSmokeParticles(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (stopThatInfernalScreamingWoman)
            {
                // We're dying here, need to shut off the screaming
                timer_stopScreaming += gameTime.ElapsedGameTime.TotalSeconds;
                if (timer_stopScreaming > timeToStopScreaming)
                {
                    Alive = false;
                    hasSpawned = false;
                    stopThatInfernalScreamingWoman = false;
                    timer_stopScreaming = 0.0;
                    ClearSmokeParticles();
                    try
                    {
                        sfxi_Scream.Stop();
                    }
                    catch { }
                }
                else
                {
                    float vol = MathHelper.Lerp(volume_Min, volume_Max, (float)(timer_stopScreaming / timeToStopScreaming));
                    if (sfxi_Scream == null)
                    {
                        sfxi_Scream = sfx_Scream.Play(vol);
                    }
                    else
                    {
                        sfxi_Scream.Volume = vol;
                        if (sfxi_Scream.State != SoundState.Playing)
                            sfxi_Scream.Play();
                    }
                }
            }
            else
            {
                timer_SinceFireSwitch += gameTime.ElapsedGameTime.TotalSeconds;
                if (timer_SinceFireSwitch > timeToSwitchFire)
                {
                    timer_SinceFireSwitch = 0.0;
                    lastFire = currentFire;
                    currentFire++;
                    if (currentFire >= numberOfFireFrames)
                        currentFire = 0;
                }
                // Proceed as normal
                if (hasSpawned)
                {
                    if (sfxi_Scream == null)
                        sfxi_Scream = sfx_Scream.Play(volume_Max);

                    if (sfxi_Scream.State != SoundState.Playing)
                        sfxi_Scream.Play();

                    // Adjust the scream according to where we are on the screen
                    sfxi_Scream.Pan = MathHelper.Lerp(1f, -1f,
                        ((float)InstanceManager.DefaultViewport.Width - Position.X) / (float)InstanceManager.DefaultViewport.Width);
                }

                UpdateSmokeParticles(gameTime);
            }
        }
        #endregion
    }
}
