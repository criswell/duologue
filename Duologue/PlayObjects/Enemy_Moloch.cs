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
    public struct MolochBodyElement
    {
        public Texture2D Texture;
        public float Rotation;
        public float DeltaRotation;
        public float Alpha;
        public float DeltaAlpha;
        public bool DeltaAlphaDirection;
        public int Color;
        public double TimerColorChange;
    }

    public struct EyeBallFrame
    {
        public Texture2D Base;
        public Texture2D Outline;
        public Texture2D ShadeLower;
        public Texture2D ShadeMiddle;
        public Texture2D ShadeUpper;
    }

    public struct TubeFrame
    {
        public Texture2D Base;
        public Texture2D Outline;
        public Texture2D Lower;
        public Texture2D Upper;
        public Vector2 Center;
    }

    public struct TubeGuy
    {
        public int CurrentFrame;
        public float Angle;
        public Vector2 Offset;
        public float Rotation;
        public bool Alive;
        public double Timer;
        public int HitPoints;
        public ColorPolarity ColorPolarity;
    }

    public enum MolochState
    {
        Moving,
        Steady,
    }

    public class Enemy_Moloch : Enemy
    {
        #region Constants
        private const string filename_Body = "Enemies/end/end-boss-body-{0}";
        private const int frames_Body = 3;
        
        private const string filename_EyeBall = "Enemies/end/end-boss-eye{0}-eyeball";
        private const string filename_EyeOutline = "Enemies/end/end-boss-eye{0}-outline";
        private const string filename_EyeShadeLower = "Enemies/end/end-boss-eye{0}-shade-lower";
        private const string filename_EyeShadeMiddle = "Enemies/end/end-boss-eye{0}-shade-middle";
        private const string filename_EyeShadeUpper = "Enemies/end/end-boss-eye{0}-shade-upper";
        private const int frames_Eye = 5;

        private const string filename_Spinner = "Enemies/end/end-boss-spinner";

        private const string filename_TubeBase = "Enemies/end/end-boss-tube{0}-base";
        private const string filename_TubeOutline = "Enemies/end/end-boss-tube{0}-outline";
        private const string filename_TubeShadeLower = "Enemies/end/end-boss-tube{0}-shade-lower";
        private const string filename_TubeShadeUpper = "Enemies/end/end-boss-tube{0}-shade-upper";
        private const int frames_Tube = 4;
        private const string filename_TubeDeadBase = "Enemies/end/end-boss-tube-dead-base";
        private const string filename_TubeDeadOutline = "Enemies/end/end-boss-tube-dead-outline";
        private const string filename_TubeDeadShadeLower = "Enemies/end/end-boss-tube-dead-shade-lower";
        private const string filename_TubeDeadShadeUpper = "Enemies/end/end-boss-tube-dead-shade-upper";
        private const float offset_TubeVerticalCenter = -10f;

        private const string filename_EyePupil = "Enemies/gloop/king-gloop-eye";

        private const float delta_BodyRotation = MathHelper.PiOver4 * 0.4f;
        private const float delta_SpinnerRotation = MathHelper.PiOver4 * 0.1f;

        private const float minAlpha = 0.1f;
        private const float maxAlpha = 1.0f;
        private const double minDeltaAlpha = 0.005;
        private const double maxDeltaAlpha = 0.04;

        private const double totalTime_SpinnerColorChange = 1.02;
        private const double totalTime_BodyColorChange = 1.51;

        /// <summary>
        /// The offset length of the tube base
        /// </summary>
        private const float offset_Tube = 350f;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPoints = 75;
        #endregion

        #region Fields
        // Image and animation related things
        private MolochBodyElement[] body;
        private EyeBallFrame[] eyes;
        private TubeFrame[] tubeFrames;
        private TubeFrame tubeDead;
        private Texture2D texture_Spinner;
        private Texture2D texture_EyePupil;
        private Vector2 center_Body;
        private Vector2 center_Eye;
        private Vector2 center_Spinner;
        private Vector2 center_Pupil;
        /// <summary>
        /// The array of tube guys, contains the frame information, position, etc.
        /// </summary>
        private TubeGuy[] tubes;
        private float rotation_Spinner;
        private float alpha_Spinner;
        private float size_Spinner;
        private Color[] colorArray_TasteTheRainbow;
        private int color_Spinner;
        private Vector2 offset_Eye;
        private Vector2 offset_Pupil;
        private float rotation_Eye;

        // Relation to player stuff
        private Vector2 vectorToNearestPlayer;
        private Player nearestPlayer;
        private Vector2 centerOfScreen;

        // State stuff
        private MolochState currentState;
        private double timer_SpinnerColorChange;

        // Audio stuff
        private AudioManager audio;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public Enemy_Moloch() : base() { }

        public Enemy_Moloch(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Moloch;
            MajorType = MajorPlayObjectType.Enemy;
            MyEnemyType = EnemyType.Leader;
            Initialized = false;

            // Set the RealSize by hand, set this at max
            RealSize = new Vector2(1048, 1048);
        }

        public override void Initialize(
            Vector2 startPos, 
            Vector2 startOrientation, 
            ColorState currentColorState, 
            ColorPolarity startColorPolarity, 
            int? hitPoints)
        {
            // Starting variables are all ignored

            // Set Position
            Position = Vector2.Zero - 0.5f * RealSize;
            // Set next position

            // Set state
            currentState = MolochState.Moving;
            // Set color
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            // Set hitpoints
            StartHitPoints = realHitPoints;
            CurrentHitPoints = realHitPoints;
            audio = ServiceLocator.GetService<AudioManager>();
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            // Set up the color array that will be used for all of the non-tub stuffs
            colorArray_TasteTheRainbow = new Color[]
            {
                new Color(146, 203, 80),
                new Color(76, 56, 80),
                new Color(195, 94, 80),
                new Color(22, 74, 32),
                new Color(137, 22, 128),
                new Color(8, 140, 128),
                new Color(183,172,182),
                new Color(255,135,39),
                new Color(153,100,167),
            };

            centerOfScreen = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f, InstanceManager.DefaultViewport.Height / 2f);

            body = new MolochBodyElement[frames_Body];
            eyes = new EyeBallFrame[frames_Eye];
            tubeFrames = new TubeFrame[frames_Tube];

            texture_EyePupil = InstanceManager.AssetManager.LoadTexture2D(filename_EyePupil);
            center_Pupil = new Vector2(
                texture_EyePupil.Width / 2f, texture_EyePupil.Height / 2f);
            texture_Spinner = InstanceManager.AssetManager.LoadTexture2D(filename_Spinner);
            center_Spinner = new Vector2(
                texture_Spinner.Width / 2f, texture_Spinner.Height / 2f);

            for (int i = 0; i < frames_Body; i++)
            {
                body[i].Texture = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_Body, i.ToString()));
                body[i].Rotation = 0;
                body[i].DeltaRotation = (1 + i) * delta_BodyRotation;
                body[i].Alpha = MathHelper.Lerp(minAlpha, maxAlpha, i/(float)frames_Body);
                body[i].DeltaAlpha = (float)MWMathHelper.GetRandomInRange(minDeltaAlpha, maxDeltaAlpha);
                body[i].DeltaAlphaDirection = MWMathHelper.CoinToss();
                body[i].Color = MWMathHelper.GetRandomInRange(0, colorArray_TasteTheRainbow.Length);
                body[i].TimerColorChange = MWMathHelper.GetRandomInRange(0, totalTime_BodyColorChange);
            }
            center_Body = new Vector2(
                body[0].Texture.Width / 2f, body[0].Texture.Height / 2f);

            Radius = RealSize.X / 2f;

            for (int i = 0; i < frames_Eye; i++)
            {
                eyes[i].Base = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_EyeBall, i.ToString()));
                eyes[i].Outline = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_EyeOutline, i.ToString()));
                eyes[i].ShadeLower = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_EyeShadeLower, i.ToString()));
                eyes[i].ShadeMiddle = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_EyeShadeMiddle, i.ToString()));
                eyes[i].ShadeUpper = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_EyeShadeUpper, i.ToString()));
            }
            center_Eye = new Vector2(
                eyes[0].Base.Width / 2f, eyes[0].Base.Height / 2f);

            for (int i = 0; i < frames_Tube; i++)
            {
                tubeFrames[i].Base = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_TubeBase, i.ToString()));
                tubeFrames[i].Outline = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_TubeOutline, i.ToString()));
                tubeFrames[i].Lower = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_TubeShadeLower, i.ToString()));
                tubeFrames[i].Upper = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_TubeShadeUpper, i.ToString()));
                tubeFrames[i].Center = new Vector2(
                    tubeFrames[i].Base.Width / 2f, (float)tubeFrames[i].Base.Height + offset_TubeVerticalCenter);
            }

            tubeDead.Base = InstanceManager.AssetManager.LoadTexture2D(filename_TubeDeadBase);
            tubeDead.Outline = InstanceManager.AssetManager.LoadTexture2D(filename_TubeDeadOutline);
            tubeDead.Lower = InstanceManager.AssetManager.LoadTexture2D(filename_TubeDeadShadeLower);
            tubeDead.Upper = InstanceManager.AssetManager.LoadTexture2D(filename_TubeDeadShadeUpper);
            tubeDead.Center = new Vector2(
                tubeDead.Base.Width / 2f, (float)tubeDead.Base.Height + offset_TubeVerticalCenter);

            float[] tempOffsets = new float[]
            {
                0,
                MathHelper.PiOver4,
                MathHelper.PiOver2,
                3f * MathHelper.PiOver4,
                MathHelper.Pi,
                5f * MathHelper.PiOver4,
                3f * MathHelper.PiOver2,
                7f * MathHelper.PiOver4
            };
            tubes = new TubeGuy[tempOffsets.Length];
            for (int i = 0; i < tempOffsets.Length; i++)
            {
                tubes[i].Alive = true;
                tubes[i].CurrentFrame = MWMathHelper.GetRandomInRange(0, frames_Tube);
                tubes[i].Angle = tempOffsets[i];
                tubes[i].Offset = GetTubeOffset((double)tempOffsets[i]);
                tubes[i].Rotation = GetTubeRotation(tempOffsets[i]);
                tubes[i].HitPoints = StartHitPoints;
                if(MWMathHelper.IsEven(i))
                    tubes[i].ColorPolarity = ColorPolarity.Negative;
                else
                    tubes[i].ColorPolarity = ColorPolarity.Positive;
                tubes[i].Timer = 0;
            }

            // Set up spinner information
            rotation_Spinner = 0;
            size_Spinner = 1f;
            alpha_Spinner = maxAlpha;
            color_Spinner = MWMathHelper.GetRandomInRange(0, colorArray_TasteTheRainbow.Length);
            timer_SpinnerColorChange = 0;

            Alive = true;
            Initialized = true;
        }

        public override String[] GetTextureFilenames()
        {
            String[] filenames = new String[frames_Body + 5 * frames_Eye + 4 * frames_Tube + 6];

            int i = 0;
            filenames[i] = filename_EyePupil;
            i++;
            filenames[i] = filename_Spinner;
            i++;

            filenames[i] = filename_TubeDeadBase;
            i++;
            filenames[i] = filename_TubeDeadOutline;
            i++;
            filenames[i] = filename_TubeDeadShadeLower;
            i++;
            filenames[i] = filename_TubeDeadShadeUpper;
            i++;

            for (int t = 0; t < frames_Body; t++)
            {
                filenames[i] = String.Format(filename_Body, t.ToString());
                i++;
            }

            for (int t = 0; t < frames_Eye; t++)
            {
                filenames[i] = String.Format(filename_EyeBall, t.ToString());
                i++;
                filenames[i] = String.Format(filename_EyeOutline, t.ToString());
                i++;
                filenames[i] = String.Format(filename_EyeShadeLower, t.ToString());
                i++;
                filenames[i] = String.Format(filename_EyeShadeMiddle, t.ToString());
                i++;
                filenames[i] = String.Format(filename_EyeShadeUpper, t.ToString());
                i++;
            }

            for (int t = 0; t < frames_Tube; t++)
            {
                filenames[i] = String.Format(filename_TubeBase, t.ToString());
                i++;
                filenames[i] = String.Format(filename_TubeOutline, t.ToString());
                i++;
                filenames[i] = String.Format(filename_TubeShadeLower, t.ToString());
                i++;
                filenames[i] = String.Format(filename_TubeShadeUpper, t.ToString());
                i++;
            }

            return filenames;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Given an angle, will get an offset for the tube
        /// </summary>
        private Vector2 GetTubeOffset(double angle)
        {
            Vector2 offset = Vector2.Zero;

            offset.X = offset_Tube * (float)Math.Cos(angle);
            offset.Y = -offset_Tube * (float)Math.Sin(angle);

            return offset;
        }

        /// <summary>
        /// Given an angle, will get a rotation for the tube such
        /// that it will always be facing away from the center
        /// </summary>
        private float GetTubeRotation(float p)
        {
            return MathHelper.PiOver2 - p;
        }
        #endregion

        #region Movement overrides
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
            // Draw the tubes
            for (int i = 0; i < tubes.Length; i++)
            {
                #region Tube Draw
                // Draw the base
                InstanceManager.RenderSprite.Draw(
                    tubeFrames[tubes[i].CurrentFrame].Base,
                    Position + tubes[i].Offset,
                    tubeFrames[tubes[i].CurrentFrame].Center,
                    null,
                    GetMyColor(ColorState.Dark, tubes[i].ColorPolarity),
                    tubes[i].Rotation,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
                // Draw the layers
                InstanceManager.RenderSprite.Draw(
                    tubeFrames[tubes[i].CurrentFrame].Lower,
                    Position + tubes[i].Offset,
                    tubeFrames[tubes[i].CurrentFrame].Center,
                    null,
                    GetMyColor(ColorState.Medium, tubes[i].ColorPolarity),
                    tubes[i].Rotation,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
                InstanceManager.RenderSprite.Draw(
                    tubeFrames[tubes[i].CurrentFrame].Upper,
                    Position + tubes[i].Offset,
                    tubeFrames[tubes[i].CurrentFrame].Center,
                    null,
                    GetMyColor(ColorState.Light, tubes[i].ColorPolarity),
                    tubes[i].Rotation,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
                // Draw the outline
                InstanceManager.RenderSprite.Draw(
                    tubeFrames[tubes[i].CurrentFrame].Outline,
                    Position + tubes[i].Offset,
                    tubeFrames[tubes[i].CurrentFrame].Center,
                    null,
                    Color.White,
                    tubes[i].Rotation,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
                #endregion
            }

            // Draw the spinner
            InstanceManager.RenderSprite.Draw(
                texture_Spinner,
                Position,
                center_Spinner,
                null,
                new Color(colorArray_TasteTheRainbow[color_Spinner], alpha_Spinner),
                rotation_Spinner,
                size_Spinner,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);

            // Draw the body
            for (int i = 0; i < body.Length; i++)
            {
                InstanceManager.RenderSprite.Draw(
                    body[i].Texture,
                    Position,
                    center_Body,
                    null,
                    new Color(colorArray_TasteTheRainbow[body[i].Color], body[i].Alpha),
                    body[i].Rotation,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
            }

            // Draw the eye
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
