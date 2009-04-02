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
    public enum SpawnerState
    {
        Moving,
        Fleeing,
        FlareUp,
        FlareDown,
        Scanning,
    }
    public class Enemy_Spawner : Enemy
    {
        #region Constants
        private const string filename_Flare = "Enemies/spawner/flare";
        private const string filename_Base = "Enemies/spawner/spawner-base";
        private const string filename_Lights = "Enemies/spawner/spawner-lights{0}";

        private const int numOfLightLevels = 3;

        private const float scale_InnerRing = 0.68f;

        private const float delta_InnerRingNormal = MathHelper.PiOver4 * -0.2f;
        private const float delta_InnerRingFlee = MathHelper.PiOver4;
        private const float delta_OuterRingNormal = MathHelper.PiOver4 * 0.1f;
        private const float delta_OuterRingFlee = MathHelper.PiOver4 * -0.7f;

        private const float maxSpeed = 4f;
        private const float accel = 0.25f;

        private const double maxFleeDistanceMultiplier = 7.0;
        private const double minFleeDistanceMultiplier = 3.0;
        private const float radiusMultiplier = 0.78f;
        #endregion

        #region Fields
        private Texture2D texture_Flare;
        private Texture2D texture_Base;
        private Texture2D[] texture_Lights;

        private Vector2 center_Flare;
        private Vector2 center_Base;

        private float rotation_InnerRing;
        private float rotation_OuterRing;

        private SpawnerState currentState;

        private Vector2 nextPosition;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public Enemy_Spawner() : base() { }

        public Enemy_Spawner(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Spawner;
            MajorType = MajorPlayObjectType.Enemy;
            Initialized = false;

            // Set the RealSize by hand
            RealSize = new Vector2(110, 110);
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
            texture_Flare = InstanceManager.AssetManager.LoadTexture2D(filename_Flare);
            texture_Base = InstanceManager.AssetManager.LoadTexture2D(filename_Base);
            texture_Lights = new Texture2D[numOfLightLevels];

            for (int i = 0; i < numOfLightLevels; i++)
            {
                texture_Lights[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Lights, (i + 1).ToString()));
            }

            center_Base = new Vector2(
                texture_Base.Width / 2f, texture_Base.Height / 2f);
            center_Flare = new Vector2(
                texture_Flare.Width / 2f, texture_Flare.Height / 2f);

            rotation_InnerRing = (float)MWMathHelper.GetRandomInRange(0, MathHelper.TwoPi);
            rotation_OuterRing = (float)MWMathHelper.GetRandomInRange(0, MathHelper.TwoPi);

            Radius = RealSize.Length() * radiusMultiplier;

            currentState = SpawnerState.Moving;

            GetNextPosition(Vector2.Zero);

            Alive = true;
            Initialized = true;
        }

        public override String[] GetFilenames()
        {
            String[] filenames = new String[numOfLightLevels + 2];

            for (int i = 0; i < numOfLightLevels; i++)
            {
                filenames[i] = String.Format(filename_Lights, (i+1).ToString());
            }

            filenames[filenames.Length - 2] = filename_Base;
            filenames[filenames.Length - 1] = filename_Flare;

            return filenames;
        }
        #endregion

        #region Private methods
        private void GetNextPosition(Vector2 vector2)
        {
            if (vector2 == Vector2.Zero)
            {
                nextPosition = new Vector2(
                    (float)MWMathHelper.GetRandomInRange(
                        (double)InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent,
                        (double)InstanceManager.DefaultViewport.Width - (double)InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent),
                    (float)MWMathHelper.GetRandomInRange(
                        (double)InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent,
                        (double)InstanceManager.DefaultViewport.Height - (double)InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent));
            }
            else
            {
                vector2.Normalize();
                float distance = (float)MWMathHelper.GetRandomInRange(
                    minFleeDistanceMultiplier * Radius, maxFleeDistanceMultiplier * Radius);

                nextPosition = Position + distance * vector2;
            }
        }
        #endregion

        #region Public overrides
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
