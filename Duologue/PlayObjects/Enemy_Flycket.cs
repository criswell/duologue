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
        private const float volume_Max = 0.65f;
        private const float volume_Min = 0.01f;
        private const float alpha_MaxSmokeParticles = 0.75f;
        private const float alpha_MinSmokeParticles = 0.01f;
        private const float scale_MinSmokeParticle = 0.9f;
        private const float scale_MaxSmokeParticle = 2.0f;
        private const float delta_ScaleSmokeParticles = 0.04f;

        private const float radiusMultiplier = 0.9f;

        #region Force interactions
        private const float 
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
        private float[] scale_SmokeParticles;
        private float[] alpha_SmokeParticles;
        private Color myColor;
        private Color altColor;

        // Sound related
        private SoundEffect sfx_Scream;
        private SoundEffectInstance sfxi_Scream;

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
        public Enemy_Flycket() : base() { }

        public Enemy_Flycket(GamePlayScreenManager manager) :
            base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Flycket;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Follower;
            Initialized = false;

            // Set the RealSize by hand
            RealSize = new Vector2(46, 90);
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

            if (!Initialized)
                LoadAndInitialize();
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

            i++;
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
            if (trackedPlayerObject != null)
            {
                // We only alter our tragectory when we have a player to go after

            }
            else if (hasSpawned)
            {
                // If there's no player, but we've spawned, we keep moving in last tragectory
            }
            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
