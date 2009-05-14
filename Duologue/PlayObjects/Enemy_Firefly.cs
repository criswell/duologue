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
    public class Enemy_Firefly : Enemy
    {
        #region Constants
        private const string filename_Body = "Enemies/spitter/spit-{0}";
        private const int frames_Body = 3;
        private const string filename_Flame = "Enemies/static-king-{0}";
        private const int frames_Flame = 4;
        private const string filename_Explode = "Audio/PlayerEffects/standard-enemy-explode";
        private const float scale_MaxFlame = 0.68f;
        private const float scale_MinFlame = 0.39f;
        private const float alpha_MaxFlame = 0.8f;
        private const float alpha_MinFlame = 0.05f;
        private const float scale_MaxBody = 1.1f;
        private const float scale_MinBody = 0.85f;
        private const float alpha_MaxBody = 0.85f;
        private const float alpha_MinBody = 0.1f;
        private const float delta_StateChange = 0.05f;

        private const float radiusMultiplier = 1.01f;

        private const float rotate_Max = MathHelper.PiOver4 * 0.01f;

        private const float outsideScreenMultiplier = 2.1f;

        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 10;

        /// <summary>
        /// The multiplier for point value tweaks based upon hitpoints
        /// </summary>
        private const int hitPointMultiplier = 2;

        private const double timePerColor = 0.1;

        #region Force interactions
        private const float standardSpeed = 2.1f;
        private const float lightAttractSpeed = 3.5f;
        private const float lightRepulseSpeed = 1.1f;

        private const float minMovement = 1.39f;
        #endregion
        #endregion

        #region Fields
        // Image related items
        private Texture2D[] texture_Body;
        private Vector2[] center_Body;
        private Texture2D[] texture_Flame;
        private Vector2[] center_Flame;
        private float[] state_Body;
        private float[] state_Flame;
        private float[] rotation_Flame;
        private Color[] bodyColor;
        private Color flameColor;
        private Color altColor;
        private int currentBodyColor;
        private double timeSinceSwitch;
        private int deltaBodyColor;

        // Sound related
        private SoundEffect sfx_Explode;
        private AudioManager audio;

        // State related
        private bool hasSpawned;

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
        public Enemy_Firefly() : base() { }

        public Enemy_Firefly(GamePlayScreenManager manager) :
            base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Firefly;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Follower;
            Initialized = false;

            // Set the RealSize by hand, set this at max
            RealSize = new Vector2(37, 40);
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
            Alive = true;
            timeSinceSwitch = 0;
            deltaBodyColor = 1;

            if (!Initialized)
                LoadAndInitialize();

            ClearFrames();
        }

        private void LoadAndInitialize()
        {
            texture_Body = new Texture2D[frames_Body];
            texture_Flame = new Texture2D[frames_Flame];
            center_Body = new Vector2[frames_Body];
            center_Flame = new Vector2[frames_Flame];

            for (int i = 0; i < frames_Body; i++)
            {
                texture_Body[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Body, i + 1));
                center_Body[i] = new Vector2(
                    texture_Body[i].Width / 2f, texture_Body[i].Height / 2f);
            }
            for (int i = 0; i < frames_Flame; i++)
            {
                texture_Flame[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Flame, i + 1));
                center_Flame[i] = new Vector2(
                    texture_Flame[i].Width / 2f, texture_Flame[i].Height / 2f);
            }

            state_Body = new float[frames_Body];
            state_Flame = new float[frames_Flame];
            rotation_Flame = new float[frames_Flame];

            bodyColor = new Color[] 
            {
                GetMyColor(ColorState.Light),
                GetMyColor(ColorState.Medium),
                GetMyColor(ColorState.Dark)
            };
            currentBodyColor = 0;

            flameColor = GetMyColor(ColorState.Dark);
            altColor = Color.WhiteSmoke;

            Radius = center_Body[0].X * radiusMultiplier;

            sfx_Explode = InstanceManager.AssetManager.LoadSoundEffect(filename_Explode);
            audio = ServiceLocator.GetService<AudioManager>();

            Initialized = true;
        }

        public override String[] GetTextureFilenames()
        {
            String[] filenames = new String[frames_Body + frames_Flame];

            int t = 0;
            for (int i = 0; i < frames_Body; i++)
            {
                filenames[t] = String.Format(filename_Body, (i + 1));
                t++;
            }

            for (int i = 0; i < frames_Flame; i++)
            {
                filenames[t] = String.Format(filename_Flame, (i + 1));
                t++;
            }

            return filenames;
        }

        public override String[] GetSFXFilenames()
        {
            return new String[]
            {
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
            if (pobj.MajorType == MajorPlayObjectType.Player)
            {
                // Player
                Vector2 vToPlayer = this.Position - pobj.Position;
                float len = vToPlayer.Length();

                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em and asplode ourselves
                    Alive = false;
                    LocalInstanceManager.EnemyExplodeSystem.AddParticles(Position, bodyColor[currentBodyColor]);
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
                        LocalInstanceManager.Steam.AddParticles(Position, bodyColor[currentBodyColor]);
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

                if (angle != 0)
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
            }

            // Check boundaries - Once we move off the screen after spawned, we just die
            if (this.Position.X < -1 * RealSize.X * outsideScreenMultiplier ||
                this.Position.Y < -1 * RealSize.Y * outsideScreenMultiplier ||
                this.Position.Y > InstanceManager.DefaultViewport.Height + RealSize.Y * outsideScreenMultiplier ||
                this.Position.X > InstanceManager.DefaultViewport.Width + RealSize.X * outsideScreenMultiplier)
            {
                if (hasSpawned)
                {
                    Alive = false;
                }
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
                    LocalInstanceManager.EnemyExplodeSystem.AddParticles(Position, bodyColor[currentBodyColor]);
                    LocalInstanceManager.EnemyExplodeSystem.AddParticles(Position, altColor);
                    LocalInstanceManager.AchievementManager.EnemyDeathCount(MyType);
                    Alive = false;
                    MyManager.TriggerPoints(((PlayerBullet)pobj).MyPlayerIndex, myPointValue + hitPointMultiplier * StartHitPoints, Position);
                    sfx_Explode.Play();
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(texture_Flame[0], flameColor, Position, 0f);
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

        private void ClearFrames()
        {
            for (int i = 0; i < frames_Body; i++)
            {
                state_Body[i] = MathHelper.Lerp(1f, 0, i / (float)frames_Body);
            }
            for (int i = 0; i < frames_Flame; i++)
            {
                state_Flame[i] = MathHelper.Lerp(1f, 0, i / (float)frames_Flame);
                rotation_Flame[i] = (float)MWMathHelper.GetRandomInRange(0f, (double)MathHelper.TwoPi);
            }
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            // Draw flames
            for (int i = 0; i < frames_Flame; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    texture_Flame[i],
                    Position,
                    center_Flame[i],
                    null,
                    new Color(flameColor, MathHelper.Lerp(alpha_MinFlame, alpha_MaxFlame, state_Flame[i])),
                    rotation_Flame[i],
                    MathHelper.Lerp(scale_MaxFlame, scale_MinFlame, state_Flame[i]),
                    0f,
                    RenderSpriteBlendMode.AddititiveTop);
            }
            // Draw body
            for (int i = 0; i < frames_Body; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    texture_Body[i],
                    Position,
                    center_Body[i],
                    null,
                    new Color(bodyColor[currentBodyColor], MathHelper.Lerp(alpha_MaxBody, alpha_MinBody, state_Body[i])),
                    0f,
                    MathHelper.Lerp(scale_MinBody, scale_MaxBody, state_Body[i]),
                    0f,
                    RenderSpriteBlendMode.AddititiveTop);
            }
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceSwitch += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceSwitch > timePerColor)
            {
                timeSinceSwitch = 0;
                currentBodyColor += deltaBodyColor;
                if (currentBodyColor >= bodyColor.Length)
                {
                    currentBodyColor = bodyColor.Length - 1;
                    deltaBodyColor = -1;
                }
                else if (currentBodyColor < 0)
                {
                    currentBodyColor = 0;
                    deltaBodyColor = 1;
                }
            }

            for (int i = 0; i < frames_Body; i++)
            {
                state_Body[i] += delta_StateChange;
                if (state_Body[i] > 1f)
                    state_Body[i] = 0f;
                else if (state_Body[i] < 0f)
                    state_Body[i] = 1f;
            }
            for (int i = 0; i < frames_Flame; i++)
            {
                state_Flame[i] += delta_StateChange;
                if (state_Flame[i] > 1f)
                {
                    state_Flame[i] = 0f;
                    rotation_Flame[i] = (float)MWMathHelper.GetRandomInRange(0f, (double)MathHelper.TwoPi);
                }
                else if (state_Flame[i] < 0f)
                {
                    state_Flame[i] = 1f;
                    rotation_Flame[i] = (float)MWMathHelper.GetRandomInRange(0f, (double)MathHelper.TwoPi);
                }
            }
        }
        #endregion
    }
}
