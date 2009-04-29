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
    public class Enemy_MiniSaw : Enemy
    {
        #region Constants
        private const string filename_blades = "Enemies/buzzsaw-blades";
        private const string filename_glooplet = "Enemies/gloop/glooplet";
        private const string filename_gloopletHighlight = "Enemies/gloop/glooplet-highlight";

        private const float scale_Blades = 0.75f;
        private const float scale_Gloop = 0.45f;
        #endregion

        #region Fields
        // Texture & related
        private Texture2D texture_Blades;
        private Texture2D texture_Gloop;
        private Texture2D texture_Highlight;
        private Vector2 center_Blades;
        private Vector2 center_Gloop;
        
        // Animation related
        private float rotation_Blades;
        private bool spawning;
        private float scale_CurrentScale;

        // Movement related
        private Vector2 offset;
        private Vector2 nearestLeader;
        private float nearestLeaderRadius;
        private Enemy nearestLeaderObject;
        #endregion

        #region Constructor / Init
        public Enemy_MiniSaw() : base() { }

        public Enemy_MiniSaw(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_MiniSaw;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Follower;
            Initialized = false;
            spawning = false;

            // Set the RealSize by hand
            RealSize = new Vector2(64, 65);
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

            // The mini saw is different from its big brother,
            // it can be re-initialized after death multiple times
            // thus, we really want to do stuff we'd normally put
            // in LoadAndInitialize() here.
            rotation_Blades = 0f;
            spawning = true;
            Alive = true;
            scale_CurrentScale = 0f;

            if(!Initialized)
                LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            texture_Blades = InstanceManager.AssetManager.LoadTexture2D(filename_blades);
            texture_Gloop = InstanceManager.AssetManager.LoadTexture2D(filename_glooplet);
            texture_Highlight = InstanceManager.AssetManager.LoadTexture2D(filename_gloopletHighlight);

            center_Blades = new Vector2(
                texture_Blades.Width / 2f, texture_Blades.Height / 2f);
            center_Gloop = new Vector2(
                texture_Gloop.Width / 2f, texture_Gloop.Height / 2f);

            Radius = center_Blades.X * scale_Blades;

            Initialized = true;
        }

        public override string[] GetTextureFilenames()
        {
            return new String[]
            {
                filename_blades,
                filename_glooplet,
                filename_gloopletHighlight,
            };
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
        #endregion

        #region Movement AI
        public override bool StartOffset()
        {
            offset = Vector2.Zero;
            nearestLeaderRadius = 3 * InstanceManager.DefaultViewport.Width; // Feh, good enough
            nearestLeader = Vector2.Zero;
            nearestLeaderObject = null;
            return true;
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

        #region Update / Draw
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
