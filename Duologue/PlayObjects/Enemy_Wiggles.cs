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
    public class Enemy_Wiggles : Enemy
    {
        public enum WigglesState
        {
            Walking,
            Running,
            Dying
        }
        #region Constants
        private const string filename_base = "Enemies/wiggles/0{0}-base"; // FIXME, silliness
        private const string filename_outline = "Enemies/wiggles/0{0}-outline"; // FIXME, silliness
        private const string filename_invertOutline = "Enemies/wiggles/0{0}-invert-outline"; // Bah, who cares

        private const int numberOfWalkingFrames = 8;

        /// <summary>
        /// The time per frame while we're walking
        /// </summary>
        private const double timePerFrameWalking = 0.25;

        /// <summary>
        /// The time per frame while we're running
        /// </summary>
        private const double timePerFrameRunning = 0.1;

        /// <summary>
        /// Our speed when we're just randomly walking around
        /// </summary>
        private const float walkingSpeed = 1.5f;

        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;

        /// <summary>
        /// The minimum movement required before we register motion
        /// </summary>
        private const float minMovement = 1.5f;
        #endregion

        #region Fields
        private Texture2D[] baseFrames;
        private Texture2D[] outlineFrames;
        private Texture2D[] invertOutlineFrames;
        private Vector2[] walkingCenters;
        private int currentFrame;

        private float rotation;

        private float baseLayer;
        private float outlineLayer;

        private double timeSinceStart;
        #endregion

        #region Properties
        /// <summary>
        /// The current state for the animation of Mr. Wiggles
        /// </summary>
        public WigglesState CurrentState;
        #endregion

        #region Constructor / Init
        public Enemy_Wiggles(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Wiggles;
            MajorType = MajorPlayObjectType.Enemy;
            baseLayer = LocalInstanceManager.BlitLayer_EnemyBase;
            outlineLayer = LocalInstanceManager.BlitLayer_EnemyBase - 0.1f;
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
            //Orientation = startOrientation;
            // We want to start out in a random direction
            Orientation = new Vector2(
                (float)MWMathHelper.GetRandomInRange(-1, 1),
                (float)MWMathHelper.GetRandomInRange(-1, 1));
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            rotation = MWMathHelper.ComputeAngleAgainstX(Orientation);
            if (hitPoints == null)
            {
                hitPoints = 0;
            }
            StartHitPoints = (int)hitPoints;
            CurrentHitPoints = (int)hitPoints;
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            // Textures
            baseFrames = new Texture2D[numberOfWalkingFrames];
            outlineFrames = new Texture2D[numberOfWalkingFrames];
            walkingCenters = new Vector2[numberOfWalkingFrames];
            invertOutlineFrames = new Texture2D[numberOfWalkingFrames];

            // load the base frames
            for (int i = 1; i <= numberOfWalkingFrames; i++)
            {
                baseFrames[i - 1] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_base, i.ToString()));
                outlineFrames[i - 1] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_outline, i.ToString()));
                invertOutlineFrames[i - 1] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_invertOutline, i.ToString()));

                walkingCenters[i - 1] = new Vector2(
                    baseFrames[i - 1].Width / 2f,
                    baseFrames[i - 1].Height / 2f);
            }

            // load the death frames FIXME TODO

            // We want a random starting frame, otherwise everyone will look "the same"
            currentFrame = MWMathHelper.GetRandomInRange(0, numberOfWalkingFrames);

            CurrentState = WigglesState.Walking;

            timeSinceStart = 0;

            Initialized = true;
            Alive = true;
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            Color c = ColorState.Negative[ColorState.Light];
            if(ColorPolarity == ColorPolarity.Positive)
                c = ColorState.Positive[ColorState.Light];
            
            // Draw base
            InstanceManager.RenderSprite.Draw(
                baseFrames[currentFrame],
                Position,
                walkingCenters[currentFrame],
                null,
                c,
                rotation,
                1f,
                baseLayer);

            // Draw Outline
            InstanceManager.RenderSprite.Draw(
                outlineFrames[currentFrame],
                Position,
                walkingCenters[currentFrame],
                null,
                Color.White,
                rotation,
                1f,
                outlineLayer);
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceStart += gameTime.ElapsedGameTime.TotalSeconds;

            switch(CurrentState)
            {
                case WigglesState.Walking:
                    if (timeSinceStart > timePerFrameWalking)
                    {
                        currentFrame++;
                        timeSinceStart = 0;
                    }
                    break;
                case WigglesState.Running:
                    if (timeSinceStart > timePerFrameRunning)
                    {
                        currentFrame++;
                        timeSinceStart = 0;
                    }
                    break;
                default:
                    break;
            }
            if (currentFrame >= numberOfWalkingFrames)
                currentFrame = 0;

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
    }
}
