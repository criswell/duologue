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

        private const int numberOfWalkingFrames = 8;
        #endregion

        #region Fields
        private Texture2D[] baseFrames;
        private Texture2D[] outlineFrames;
        private Vector2[] walkingCenters;
        private int currentFrame;

        private float rotation;
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

            // load the base frames
            for (int i = 1; i <= numberOfWalkingFrames; i++)
            {
                baseFrames[i - 1] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_base, i.ToString()));
                outlineFrames[i - 1] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_outline, i.ToString()));

                walkingCenters[i - 1] = new Vector2(
                    baseFrames[i - 1].Width / 2f,
                    baseFrames[i - 1].Height / 2f);
            }

            // load the death frames FIXME TODO

            // We want a random starting frame, otherwise everyone will look "the same"
            currentFrame = MWMathHelper.GetRandomInRange(0, numberOfWalkingFrames);

            CurrentState = WigglesState.Walking;

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
            Color c;
            switch (ColorPolarity)
            {
                case ColorPolarity.Negative:
                    c = ColorState.Negative[ColorState.Light];
                    break;
                default:
                    c = ColorState.Positive[ColorState.Light];
                    break;
            }
            
            // Draw base
            InstanceManager.RenderSprite.Draw(
                baseFrames[currentFrame],
                Position,
                walkingCenters[currentFrame],
                null,
                c,
                rotation,
                1f,
                1f); // FIXME ERE I AM JH, should be determined elsewhere

        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
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
