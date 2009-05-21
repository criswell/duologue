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
    public enum LahmuState
    {
        Spawning,
        Moving,
        FreakOut,
    }

    public class Enemy_Lahmu : Enemy
    {
        #region Constants
        private const string filename_Body = "Enemies/end/tent-body{0}";
        private const string filename_Outline = "Enemies/end/tent-out{0}";
        private const int frames_Tenticles = 3;
        private const string filename_EyeBase = "Enemies/gloop/prince-gloop-base";
        private const string filename_EyeBody = "Enemies/gloop/prince-gloop-body";
        private const string filename_EyePupil = "Enemies/gloop/king-gloop-eye";
        private const string filename_Flame = "Enemies/static-king-{0}";
        private const string filename_gloopletHighlight = "Enemies/gloop/glooplet-highlight";
        private const int frames_Flame = 4;

        /// <summary>
        /// The multiplier for point value tweaks based upon hitpoints
        /// </summary>
        private const int hitPointMultiplier = 2;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 2;

        /// <summary>
        /// The scale of the pupil
        /// </summary>
        private const float scale_eyePupil = 0.9f;

        /// <summary>
        /// The max offset of the pupil
        /// </summary>
        private const float scale_eyeOffset = 20f;

        private const float verticalOffsetHighlight = -30f;
        #endregion

        #region Fields
        // Images and animation stuff
        private Texture2D[] texture_Body;
        private Texture2D[] texture_Outline;
        private Texture2D[] texture_Flame;
        private Texture2D texture_EyeBase;
        private Texture2D texture_EyeBody;
        private Texture2D texture_EyePupil;
        private Texture2D texture_Highlight;
        private Vector2[] center_Body;
        private Vector2 center_Flame;
        private Vector2 center_EyeBase;
        private Vector2 center_EyeBody;
        private Vector2 center_EyePupil;
        private Vector2 center_Highlight;
        private float[] rotation_Tenticle;
        private int[] currentFrame_Tenticles;
        private float rotation;
        private Vector2 offset_eye;
        private Color[] eyeColor;
        private int currentEyeColor;
        private Color[] currentLayerColors;

        // State information
        private LahmuState currentState;
        private double timeSinceStateChange;
        #endregion

        #region Constructor / Init
        public Enemy_Lahmu() : base() { }

        public Enemy_Lahmu(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Lahmu;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Leader;
            Initialized = false;

            // Set the RealSize by hand, set this at max
            RealSize = new Vector2(284, 224);
        }

        public override void Initialize(
            Vector2 startPos, 
            Vector2 startOrientation, 
            ColorState currentColorState, 
            ColorPolarity startColorPolarity, 
            int? hitPoints)
        {
            // We say "fuck the requested starting pos"
            Position = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f, InstanceManager.DefaultViewport.Height / 2f);

            Orientation = startOrientation;
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            if (hitPoints == null || (int)hitPoints == 0)
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
            texture_EyeBase = InstanceManager.AssetManager.LoadTexture2D(filename_EyeBase);
            texture_EyeBody = InstanceManager.AssetManager.LoadTexture2D(filename_EyeBody);
            texture_EyePupil = InstanceManager.AssetManager.LoadTexture2D(filename_EyePupil);
            texture_Highlight = InstanceManager.AssetManager.LoadTexture2D(filename_gloopletHighlight);
            center_EyeBase = new Vector2(
                texture_EyeBase.Width / 2f, texture_EyeBase.Height / 2f);
            center_EyeBody = new Vector2(
                texture_EyeBody.Width / 2f, texture_EyeBody.Height / 2f);
            center_EyePupil = new Vector2(
                texture_EyePupil.Width / 2f, texture_EyePupil.Height / 2f);
            center_Highlight = new Vector2(
                texture_Highlight.Width / 2f, texture_Highlight.Height / 2f);

            texture_Flame = new Texture2D[frames_Flame];
            for (int i = 0; i < frames_Flame; i++)
            {
                texture_Flame[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Flame, (i + 1)));
            }
            center_Flame = new Vector2(
                texture_Flame[0].Width / 2f, texture_Flame[0].Height / 2f);

            texture_Outline = new Texture2D[frames_Tenticles];
            texture_Body = new Texture2D[frames_Tenticles];
            for (int i = 0; i < frames_Tenticles; i++)
            {
                texture_Body[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Body, (i + 1)));
                texture_Outline[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_Outline, (i + 1)));
            }
            center_Body = new Vector2[]
            {
                new Vector2(137f, 117f),
                new Vector2(130f, 107f),
                new Vector2(139f, 91f)
            };

            currentFrame_Tenticles = new int[]
            {
                0, 1, 2
            };

            rotation_Tenticle = new float[]
            {
                0,
                MathHelper.PiOver4,
                MathHelper.PiOver2
            };

            rotation = 0;
            offset_eye = Vector2.Zero;

            eyeColor = new Color[]
            {
                new Color(160, 138,29),
                new Color(95, 208, 228),
                new Color(249, 85, 161),
                new Color(49, 200, 76),
            };
            currentEyeColor = 0;

            SetCurrentColors();

            // Set up state stuff
            currentState = LahmuState.Spawning;
            timeSinceStateChange = 0;

            Initialized = true;
            Alive = true;
        }

        public override String[] GetTextureFilenames()
        {
            String[] filenames = new String[frames_Tenticles * 2 + 4 + frames_Flame];
            int i = 0;
            for (int t = 0; t < frames_Tenticles; t++)
            {
                filenames[i] = String.Format(filename_Body, (t + 1));
                i++;
                filenames[i] = String.Format(filename_Outline, (t + 1));
                i++;
            }
            for (int t = 0; t < frames_Flame; t++)
            {
                filenames[i] = String.Format(filename_Flame, (t + 1));
                i++;
            }
            filenames[i] = filename_EyeBase;
            i++;
            filenames[i] = filename_EyeBody;
            i++;
            filenames[i] = filename_EyePupil;
            i++;
            filenames[i] = filename_gloopletHighlight;
            return filenames;
        }
        #endregion

        #region Public movement overrides
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

        #region Private methods
        private void SetCurrentColors()
        {
            currentLayerColors = new Color[]
            {
                GetMyColor(ColorState.Dark),
                GetMyColor(ColorState.Medium),
                GetMyColor(ColorState.Light)
            };
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            // Draw tenticles
            for (int i = 0; i < currentFrame_Tenticles.Length; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    texture_Body[currentFrame_Tenticles[i]],
                    Position,
                    center_Body[currentFrame_Tenticles[i]],
                    null,
                    currentLayerColors[i],
                    rotation + rotation_Tenticle[i],
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
                InstanceManager.RenderSprite.Draw(
                    texture_Outline[currentFrame_Tenticles[i]],
                    Position,
                    center_Body[currentFrame_Tenticles[i]],
                    null,
                    Color.White,
                    rotation + rotation_Tenticle[i],
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
            }

            // Eye base
            InstanceManager.RenderSprite.Draw(
                texture_EyeBase,
                Position,
                center_EyeBase,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            // Pupil
            InstanceManager.RenderSprite.Draw(
                texture_EyeBody,
                Position + offset_eye,
                center_EyeBody,
                null,
                eyeColor[currentEyeColor],
                0f,
                scale_eyePupil,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            // Body
            InstanceManager.RenderSprite.Draw(
                texture_EyeBody,
                Position,
                center_EyeBody,
                null,
                currentLayerColors[currentLayerColors.Length - 1],
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            // Highlight
            InstanceManager.RenderSprite.Draw(
                texture_Highlight,
                Position + Vector2.UnitY * verticalOffsetHighlight,
                center_Highlight,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
