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

        // Sound related
        private SoundEffect sfx_Scream;
        private SoundEffectInstance sfxi_Scream;
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
            Orientation = GetStartingVector();
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            if (hitPoints == null)
            {
                hitPoints = 0;
            }
            StartHitPoints = (int)hitPoints;
            CurrentHitPoints = (int)hitPoints;

            if (!Initialized)
                LoadAndInitialize();
        }

        private Vector2 GetStartingVector()
        {
            throw new NotImplementedException();
        }

        private void LoadAndInitialize()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            throw new NotImplementedException();
        }

        public override bool ApplyOffset()
        {
            throw new NotImplementedException();
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
