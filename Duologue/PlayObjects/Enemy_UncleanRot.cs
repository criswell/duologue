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
    public enum RotState
    {
        Steady,
        FadeIn,
        FullOn,
        FadeOut,
        ScreamIn,
        TongueRoll,
        Scream,
        ScreamOut
    }

    public class Enemy_UncleanRot : Enemy
    {
        #region Constants
        // Filenames
        private const string filename_Base = "Enemies/unclean_rot/base-{0}";
        private const string filename_Outline = "Enemies/unclean_rot/outline_{0}";
        private const string filename_OutlineTongue = "Enemies/unclean_rot/outline-tongue-{0}";
        private const string filename_Skullcap = "Enemies/unclean_rot/skullcap-{0}";
        private const string filename_Static = "Enemies/unclean_rot/static-{0:00}";

        private const int numFrames_Body = 9;
        private const int numFrames_Tongue = 6;
        private const int numFrames_Static = 10;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 5;

        /// <summary>
        /// The time it takes to fade in
        /// </summary>
        private const double deltaTime_FadeIn = 2f;

        /// <summary>
        /// The time it takes to fade out
        /// </summary>
        private const double deltaTime_FadeOut = 2f;
        #endregion

        #region Fields
        private Texture2D[] texture_Base;
        private Texture2D[] texture_Outline;
        private Texture2D[] texture_OutlineTongue;
        private Texture2D[] texture_Skullcap;
        private Texture2D[] texture_Static;

        private Color[] color_Steady;
        private Color[] color_CurrentColors;
        private Color[] color_Static;
        private int currentStaticColor;

        private Vector2 center_Body;
        private Vector2 center_Static;

        private int currentFrame_Body;
        private int currentFrame_Static;
        private int currentFrame_Tongue;

        private RotState currentState;

        private double timeSinceLastSwitch;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public Enemy_UncleanRot(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_UncleanRot;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Leader;
            RealSize = new Vector2(300, 280);
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
            texture_Base = new Texture2D[numFrames_Body];
            texture_Outline = new Texture2D[numFrames_Body];
            texture_Skullcap = new Texture2D[numFrames_Body];
            texture_OutlineTongue = new Texture2D[numFrames_Tongue];
            texture_Static = new Texture2D[numFrames_Static];

            for (int i = 0; i < numFrames_Body; i++)
            {
                texture_Base[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Base, (i + 1).ToString()));
                texture_Outline[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Outline, (i + 1).ToString()));
                texture_Skullcap[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Skullcap, (i + 1).ToString()));
            }

            for (int i = 0; i < numFrames_Tongue; i++)
            {
                texture_OutlineTongue[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_OutlineTongue, (i + 1).ToString()));
            }

            center_Body = new Vector2(
                80f, texture_Base[0].Height / 2f);

            for (int i = 0; i < numFrames_Static; i++)
            {
                texture_Static[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Static, (i + 1).ToString()));
            }

            center_Static = new Vector2(
                texture_Static[0].Width / 2f, texture_Static[0].Height / 2f);

            // Init the colors
            color_CurrentColors = new Color[ColorState.NumberColorsPerPolarity];
            color_CurrentColors[ColorState.Light] = GetMyColor(ColorState.Light);
            color_CurrentColors[ColorState.Medium] = GetMyColor(ColorState.Medium);
            color_CurrentColors[ColorState.Dark] = GetMyColor(ColorState.Dark);

            color_Steady = new Color[2];
            color_Steady[0] = Color.LightSlateGray;
            color_Steady[1] = Color.SteelBlue;

            color_Static = new Color[4];
            color_Static[0] = new Color(Color.Ivory, 200);
            color_Static[1] = new Color(Color.White, 128);
            color_Static[2] = new Color(Color.Thistle, 175);
            color_Static[3] = new Color(Color.Silver, 225);
            currentStaticColor = 0;

            // Init the basic variables
            currentFrame_Body = 0;
            currentFrame_Static = 0;
            currentFrame_Tongue = 0;
            currentState = RotState.Steady;
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

        #region Private Draw methods
        private void Draw_NormalFace(GameTime gameTime)
        {
            InstanceManager.RenderSprite.Draw(
                texture_Outline[currentFrame_Body],
                Position,
                center_Body,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        private void Draw_Steady(GameTime gameTime)
        {
            InstanceManager.RenderSprite.Draw(
                texture_Base[currentFrame_Body],
                Position,
                center_Body,
                null,
                color_Steady[0],
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            InstanceManager.RenderSprite.Draw(
                texture_Skullcap[currentFrame_Body],
                Position,
                center_Body,
                null,
                color_Steady[1],
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        private void Draw_FadeIn(GameTime gameTime)
        {
            InstanceManager.RenderSprite.Draw(
                texture_Base[currentFrame_Body],
                Position,
                center_Body,
                null,
                new Color(color_CurrentColors[ColorState.Medium],
                    (float)(timeSinceLastSwitch/deltaTime_FadeIn)),
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            InstanceManager.RenderSprite.Draw(
                texture_Skullcap[currentFrame_Body],
                Position,
                center_Body,
                null,
                new Color(color_CurrentColors[ColorState.Dark],
                    (float)(timeSinceLastSwitch / deltaTime_FadeIn)),
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }
        #endregion

        #region Draw/ Update
        public override void Draw(GameTime gameTime)
        {
            // Draw the static
            InstanceManager.RenderSprite.Draw(
                texture_Static[currentFrame_Static],
                Position,
                center_Static,
                null,
                color_Static[currentStaticColor],
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlend);

            switch (currentState)
            {
                case RotState.FadeIn:
                    Draw_Steady(gameTime);
                    Draw_FadeIn(gameTime);
                    Draw_NormalFace(gameTime);
                    break;
                default:
                    Draw_Steady(gameTime);
                    Draw_NormalFace(gameTime);
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
