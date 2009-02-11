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
    class Enemy_Gloop : Enemy
    {
        #region Constants
        private const string filename_glooplet = "Enemies/gloop/glooplet";
        private const string filename_gloopletDeath = "Enemies/gloop/glooplet-death";

        private const double minSize = 0.5;
        private const double maxSize = 1.1;

        private const float radiusMultiplier = 0.8f;
        private const float outlineScale = 1.1f;

        #region Force interactions
        /// <summary>
        /// Standard repulsion of the enemy ships when too close
        /// </summary>
        private const float standardEnemyRepulse = 5f;
        #endregion
        #endregion

        #region Fields
        private Texture2D glooplet;
        private Texture2D gloopletDeath;
        private Vector2 gloopletCenter;
        private Vector2 gloopletDeathCenter;

        private float scale;
        private bool isFleeing;

        private Color currentColor;

        // Movement
        private Vector2 offset;
        private Vector2 nearestPlayer;
        private float nearestPlayerRadius;
        private Vector2 nearestLeader;
        private float nearestLeaderRadius;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
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
            glooplet = InstanceManager.AssetManager.LoadTexture2D(filename_glooplet);
            gloopletDeath = InstanceManager.AssetManager.LoadTexture2D(filename_gloopletDeath);

            gloopletCenter = new Vector2(
                glooplet.Width / 2f,
                glooplet.Height / 2f);
            gloopletDeathCenter = new Vector2(
                gloopletDeath.Width / 2f,
                gloopletDeath.Height / 2f);

            scale = (float)MWMathHelper.GetRandomInRange(minSize, maxSize);

            Radius = glooplet.Width * scale * radiusMultiplier;

            isFleeing = false;

            currentColor = GetMyColor();

            Initialized = true;
            Alive = true;
        }
        #endregion

        #region Public Overrides
        public override bool StartOffset()
        {
            offset = Vector2.Zero;
            nearestPlayerRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            nearestPlayer = Vector2.Zero;
            nearestLeaderRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            nearestLeader = Vector2.Zero;
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
                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em
                    return pobj.TriggerHit(this);
                }

                // Beam handling
                int temp = ((Player)pobj).IsInBeam(this);
                //inBeam = false;
                isFleeing = false;
                if (temp != 0)
                {
                    //inBeam = true;
                    if (temp == -1)
                    {
                        isFleeing = true;
                        Color c = ColorState.Negative[ColorState.Light];
                        if(ColorPolarity == ColorPolarity.Positive)
                            c = ColorState.Positive[ColorState.Light];
                        LocalInstanceManager.Steam.AddParticles(Position, c);
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
                    //InstanceManager.Logger.LogEntry(String.Format("Pre {0}", offset));
                    offset += standardEnemyRepulse * vToEnemy;
                    //InstanceManager.Logger.LogEntry(String.Format("Post {0}", offset));
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
        }

        public override void Update(GameTime gameTime)
        {
            // TODO
        }
        #endregion
    }
}
