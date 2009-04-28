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
    public class Enemy_MetalTooth :Enemy
    {
        #region Constants
        private const string filename_Base = "Enemies/MetalTooth-base";
        private const string filename_Blades = "Enemies/MetalTooth-blades";
        private const string filename_Shine = "Enemies/MetalTooth-shine";

        // Deltas
        private const float delta_Rotation = 0.01f;
        private const float delta_ShineOffsetX = -0f;
        private const float delta_ShineOffsetY = -12f;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 10;

        /// <summary>
        /// The hit point array
        /// </summary>
        static readonly byte[] hitPointArray = new byte[] 
        {
            (byte)128, 
            (byte)160, 
            (byte)192, 
            (byte)224, 
            (byte)255
        };
        #endregion

        #region Properties
        #endregion

        #region Fields
        // Textures and related stuff
        private Texture2D texture_Base;
        private Texture2D texture_Blades;
        private Texture2D texture_Shine;

        private Vector2 offset_Shine;
        private Vector2 center_Base;
        private Vector2 center_Blades;
        private Vector2 center_Shine;

        private Color[] myColor;
        private int currentColor;

        // Animation stuff
        private float rotation_Blades;
        private float rotation_Eye;
        private double timeSinceSwitch;

        // Audio stuff
        private AudioManager audio;
        #endregion

        #region Constructor / Init
        public Enemy_MetalTooth() : base() { }

        public Enemy_MetalTooth(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_MetalTooth;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Leader;
            Initialized = false;
            offset_Shine = new Vector2(delta_ShineOffsetX, delta_ShineOffsetY);

            // Set the RealSize by hand
            RealSize = new Vector2(255, 260);
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
            if (hitPoints == null || (int)hitPoints == 0)
            {
                hitPoints = 1;
            }
            StartHitPoints = (int)hitPoints * realHitPointMultiplier;
            CurrentHitPoints = (int)hitPoints * realHitPointMultiplier;
            audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        public override String[] GetTextureFilenames()
        {
            return new String[]
            {
                filename_Base,
                filename_Blades,
                filename_Shine,
            };
        }

        public void LoadAndInitialize()
        {
            texture_Base = InstanceManager.AssetManager.LoadTexture2D(filename_Base);
            texture_Blades = InstanceManager.AssetManager.LoadTexture2D(filename_Blades);
            texture_Shine = InstanceManager.AssetManager.LoadTexture2D(filename_Shine);

            center_Base = new Vector2(
                texture_Base.Width * 0.5f, texture_Base.Height * 0.5f);
            center_Blades = new Vector2(
                texture_Blades.Width * 0.5f, texture_Blades.Height * 0.5f);
            center_Shine = new Vector2(
                texture_Shine.Width * 0.5f, texture_Shine.Height * 0.5f);

            myColor = new Color[]
            {
                GetMyColor(ColorState.Dark),
                GetMyColor(ColorState.Medium),
                GetMyColor(ColorState.Light)
            };

            currentColor = 0;

            rotation_Blades = 0f;
            rotation_Eye = 0f;
            timeSinceSwitch = 0.0;

            Initialized = true;
            Alive = true;
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

        #region Draw/Update
        public override void Draw(GameTime gameTime)
        {
            Color s = myColor[currentColor];
            int alphaIndex = (int)MathHelper.Lerp(0, myColor.Length - 1, ((float)CurrentHitPoints / (float)StartHitPoints));
            
            // Draw blades
            InstanceManager.RenderSprite.Draw(
                texture_Blades,
                Position,
                center_Blades,
                null,
                Color.White,
                rotation_Blades,
                1f,
                0f);

            // Draw base
            InstanceManager.RenderSprite.Draw(
                texture_Base,
                Position,
                center_Base,
                null,
                myColor[currentColor],
                rotation_Eye,
                1f,
                0f);

            // Draw shine
            InstanceManager.RenderSprite.Draw(
                texture_Shine,
                Position + offset_Shine,
                center_Shine,
                null,
                myColor[currentColor],
                0f,
                1f,
                0f);
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceSwitch += gameTime.ElapsedGameTime.TotalSeconds;

            rotation_Blades += delta_Rotation;
            if (rotation_Blades > MathHelper.TwoPi)
                rotation_Blades = 0f;
            else if (rotation_Blades < 0f)
                rotation_Blades = MathHelper.TwoPi;
        }
        #endregion
    }
}
