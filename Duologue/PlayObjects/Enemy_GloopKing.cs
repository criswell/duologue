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
    public class Enemy_GloopKing : Enemy
    {
        #region Constants
        private const string filename_kingBase = "Enemies/gloop/king-gloop-base";
        private const string filename_kingEye = "Enemies/gloop/king-gloop-eye";
        private const string filename_kingBody = "Enemies/gloop/king-gloop-{0}";
        private const string filename_kingDeath = "Enemies/gloop/king-gloop-death-{0}";

        private const int bodyFrames = 7;
        private const int deathFrames = 5;

        /// <summary>
        /// The vertical middle of the king's base. This is offset from the true middle because the king is
        /// not strictly viewed from the top-down, but rather from a top-side-down viewpoint.
        /// </summary>
        private const float kingBaseVerticalMiddle = 82f;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 5;

        /// <summary>
        /// A pre-defined radius (since we can't really get this from the image
        /// </summary>
        private const float definedRadius = 70f;
        #endregion

        #region Fields
        private Texture2D kingBase;
        private Texture2D kingEye;
        private Texture2D[] kingBody;
        private Texture2D[] kingDeath;

        private Vector2 kingCenter;
        private Vector2 eyeCenter;
        private Color currentColor;
        private bool isFleeing;
        private bool isDying;
        #endregion

        #region Properties
        #endregion

        #region Construct/ Init
        public Enemy_GloopKing(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_KingGloop;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Leader;
            RealSize = new Vector2(192, 158);
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
                hitPoints = 1;
            }
            StartHitPoints = (int)hitPoints * realHitPointMultiplier;
            CurrentHitPoints = (int)hitPoints * realHitPointMultiplier;
            //audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            kingBase = InstanceManager.AssetManager.LoadTexture2D(filename_kingBase);
            kingCenter = new Vector2(
                kingBase.Width / 2f,
                kingBaseVerticalMiddle);
            kingEye = InstanceManager.AssetManager.LoadTexture2D(filename_kingEye);
            eyeCenter = new Vector2(
                kingEye.Width / 2f,
                kingEye.Height / 2f);

            kingBody = new Texture2D[bodyFrames];
            kingDeath = new Texture2D[deathFrames];

            for (int i = 0; i < bodyFrames; i++)
            {
                kingBody[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_kingBody, (i+1).ToString()));
            }

            for (int i = 0; i < deathFrames; i++)
            {
                kingDeath[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_kingDeath, (i + 1).ToString()));
            }

            Radius = definedRadius;

            isFleeing = false;

            currentColor = GetMyColor();

            isDying = false;
            Initialized = true;
            Alive = true;

        }
        #endregion

        #region Public Overrides
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
