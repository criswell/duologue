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
        //public double TimerColorChange;
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
        public bool MoveOut;
    }

    public enum MolochState
    {
        Moving,
        Steady,
    }

    public enum MolochEyeState
    {
        Open,
        Closing,
        Opening,
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
        private const string filename_EyeShaftBlob = "Enemies/gloop/glooplet";
        private const string filename_EyeShaftHighlight = "Enemies/gloop/glooplet-highlight";
        private const float scale_Blob = 0.8f;
        private const float offsetY_BlobHighlight = -10f;
        private const float scale_BlobOutline = 0.82f;
        private const int numberOfBlobsInShaft = 8;

        private const float delta_BodyRotation = MathHelper.PiOver4 * 0.005f;
        private const float delta_SpinnerRotation = MathHelper.PiOver4 * 0.01f;

        private const float minAlpha_Body = 0.1f;
        private const float maxAlpha_Body = 0.91f;
        private const double minDeltaAlpha_Body = 0.0005;
        private const double maxDeltaAlpha_Body = 0.004;

        private const float alpha_Spinner = 0.5f;
        private const float maxScale_Spinner = 1.0f;
        private const float minScale_Spinner = 0.7889f;

        private const float minDelta_TubeRotation = MathHelper.PiOver4 * 0.001f;
        private const float maxDelta_TubeRotation = MathHelper.PiOver4 * 0.03f;

        private const double totalTime_SpinnerColorChange = 1.02;
        //private const double totalTime_BodyColorChange = 2.51;
        private const double totalTime_EyeBallBlinkTick = 0.1;
        private const double totalTime_EyeBallOpen = 1.5;
        private const double totalTime_TubeRotationRampUp = 2.51;
        private const double totalTime_TubeAnimationTick = 0.5;
        private const double totalTime_EyeStareOrbit = 2.54;

        private const float maxOrbit_X = 80f;
        private const float maxOrbit_Y = 70f;
        private const float multiplierOrbit_X = 3f;
        private const float multiplierOrbit_Y = 2f;

        private const float radiusOfBody = 350f;

        /// <summary>
        /// The offset length of the tube base
        /// </summary>
        private const float offsetLength_Tube = 350f;

        /// <summary>
        /// The offset length of the eyeball from center of body
        /// </summary>
        private const float offsetLength_EyeBall = 225f;
        /// <summary>
        /// The offset length of the pupil from center of eyeball
        /// </summary>
        private const float offsetLength_Pupil = 22f;

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
        private Texture2D texture_Blob;
        private Texture2D texture_BlobHighlight;
        private Vector2 center_Body;
        private Vector2 center_Eye;
        private Vector2 center_Spinner;
        private Vector2 center_Pupil;
        private Vector2 center_Blob;
        private Vector2 center_BlobHighlight;
        /// <summary>
        /// The array of tube guys, contains the frame information, position, etc.
        /// </summary>
        private TubeGuy[] tubes;
        private float delta_CurrentTubeRotation;
        private float rotation_Spinner;
        private float size_Spinner;
        private Color[] colorArray_TasteTheRainbow;
        private int color_Spinner;
        private Vector2 offset_Eye;
        private Vector2 offset_Pupil;
        //private float rotation_Eye;
        private int color_Pupil;
        private ColorPolarity polarity_EyeBall;

        // Relation to player stuff
        private Vector2 vectorToNearestPlayer;
        private Player nearestPlayer;
        private Vector2 centerOfScreen;

        // State stuff
        private MolochState currentState;
        private double timer_SpinnerColorChange;
        private double timer_EyeBall;
        private int currentEyeFrame;
        private MolochEyeState currentEyeState;
        private double timer_TubeRotation;
        private bool tubeRotationRampUp;
        private double timer_EyeStare;

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
            // Perform pre-checks
            if (LocalInstanceManager.CurrentNumberEnemies < 30)
                throw new Exception("MOLOCH NEEDS MOAR FODDER! (In order for Moloch to work, we need a minimum of 30 enemies in the enemy list. Note, you'll probably want them all Placeholders.)");

            // Starting variables are all ignored

            // Set Position
            Position = //Vector2.Zero;// -0.5f * RealSize;
                new Vector2(
                    (float)InstanceManager.DefaultViewport.Width, 0);
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
                body[i].Alpha = MathHelper.Lerp(minAlpha_Body, maxAlpha_Body, i/(float)frames_Body);
                body[i].DeltaAlpha = (float)MWMathHelper.GetRandomInRange(minDeltaAlpha_Body, maxDeltaAlpha_Body);
                body[i].DeltaAlphaDirection = MWMathHelper.CoinToss();
                body[i].Color = MWMathHelper.GetRandomInRange(0, colorArray_TasteTheRainbow.Length);
                //body[i].TimerColorChange = MWMathHelper.GetRandomInRange(0, totalTime_BodyColorChange);
            }
            center_Body = new Vector2(
                body[0].Texture.Width / 2f, body[0].Texture.Height / 2f);

            Radius = radiusOfBody;

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

            texture_Blob = InstanceManager.AssetManager.LoadTexture2D(filename_EyeShaftBlob);
            texture_BlobHighlight = InstanceManager.AssetManager.LoadTexture2D(filename_EyeShaftHighlight);
            center_Blob = new Vector2(
                texture_Blob.Width / 2f, texture_Blob.Height / 2f);
            center_BlobHighlight = new Vector2(
                texture_BlobHighlight.Width / 2f, texture_BlobHighlight.Height / 2f);

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
                tubes[i].MoveOut = true;
            }

            delta_CurrentTubeRotation = minDelta_TubeRotation;
            timer_TubeRotation = 0;
            tubeRotationRampUp = true;

            // Set up spinner information
            rotation_Spinner = 0;
            size_Spinner = maxScale_Spinner;
            color_Spinner = MWMathHelper.GetRandomInRange(0, colorArray_TasteTheRainbow.Length);
            timer_SpinnerColorChange = 0;

            // Set up a default eye stuff
            vectorToNearestPlayer = Vector2.Zero;
            nearestPlayer = null;
            SetEyeOffsets();
            color_Pupil = MWMathHelper.GetRandomInRange(0, colorArray_TasteTheRainbow.Length);
            polarity_EyeBall = ColorPolarity.Positive;
            timer_EyeBall = 0;
            currentEyeFrame = 0;
            currentEyeState = MolochEyeState.Open;
            timer_EyeStare = 0;

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
        /// Will set the current offsets for the eyeball and pupil
        /// </summary>
        private void SetEyeOffsets()
        {
            // Lissajous curve for what he's looking at
            Vector2 localOffset = new Vector2(
                maxOrbit_X * (float)Math.Sin(multiplierOrbit_X * MathHelper.Lerp(0, MathHelper.TwoPi, (float)(timer_EyeStare / totalTime_EyeStareOrbit))),
                maxOrbit_Y * (float)Math.Sin(multiplierOrbit_Y * MathHelper.Lerp(0, MathHelper.TwoPi, (float)(timer_EyeStare / totalTime_EyeStareOrbit))));
            // Place eye ball with relation to center
            offset_Eye = centerOfScreen + localOffset - Position;
            offset_Eye.Normalize();
            Vector2 temp_offset_Eye = offset_Eye * offsetLength_EyeBall;
            //rotation_Eye = MWMathHelper.ComputeAngleAgainstX(offset_Eye) - MathHelper.PiOver4;

            // Aim the pupil
            if (nearestPlayer == null || vectorToNearestPlayer == Vector2.Zero)
            {
                // Aim at the center of screen
                offset_Pupil = centerOfScreen - (Position + temp_offset_Eye);
            }
            else
            {
                // Aim at the player
                offset_Pupil = vectorToNearestPlayer - (Position + temp_offset_Eye);
            }
            offset_Pupil.Normalize();
            offset_Pupil = offset_Pupil * offsetLength_Pupil;
        }

        /// <summary>
        /// Given an angle, will get an offset for the tube
        /// </summary>
        private Vector2 GetTubeOffset(double angle)
        {
            Vector2 offset = Vector2.Zero;

            offset.X = offsetLength_Tube * (float)Math.Cos(angle);
            offset.Y = -offsetLength_Tube * (float)Math.Sin(angle);

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
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            return true;
        }

        public override bool ApplyOffset()
        {
            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            return true;
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

            #region Draw the eye
            // Draw the shaft
            for (int i = 0; i < numberOfBlobsInShaft; i++)
            {
                // Draw the outline
                InstanceManager.RenderSprite.Draw(
                    texture_Blob,
                    Position + MathHelper.Lerp(0, offsetLength_EyeBall, (float)i / (float)numberOfBlobsInShaft) * offset_Eye,
                    center_Blob,
                    null,
                    Color.Black,
                    0f,
                    scale_BlobOutline,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
                // Draw the blob
                InstanceManager.RenderSprite.Draw(
                    texture_Blob,
                    Position + MathHelper.Lerp(0, offsetLength_EyeBall, (float)i / (float)numberOfBlobsInShaft) * offset_Eye,
                    center_Blob,
                    null,
                    GetMyColor(ColorState.Dark, polarity_EyeBall),
                    0f,
                    scale_Blob,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);                
                // Draw the highlight
                InstanceManager.RenderSprite.Draw(
                    texture_BlobHighlight,
                    Position + MathHelper.Lerp(0, offsetLength_EyeBall, (float)i / (float)numberOfBlobsInShaft) * offset_Eye +
                    offsetY_BlobHighlight * Vector2.UnitY,
                    center_BlobHighlight,
                    null,
                    Color.White,
                    0f,
                    scale_Blob,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
            }
            // Draw the base
            InstanceManager.RenderSprite.Draw(
                eyes[currentEyeFrame].Base,
                Position + offsetLength_EyeBall * offset_Eye,
                center_Eye,
                null,
                Color.White,
                0f,//rotation_Eye,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
            // Draw the pupil
            InstanceManager.RenderSprite.Draw(
                texture_EyePupil,
                Position + offsetLength_EyeBall * offset_Eye + offset_Pupil,
                center_Pupil,
                null,
                colorArray_TasteTheRainbow[color_Pupil],
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
            // Draw the layers
            InstanceManager.RenderSprite.Draw(
                eyes[currentEyeFrame].ShadeLower,
                Position + offsetLength_EyeBall * offset_Eye,
                center_Eye,
                null,
                GetMyColor(ColorState.Light, polarity_EyeBall),
                0f,//rotation_Eye,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
            InstanceManager.RenderSprite.Draw(
                eyes[currentEyeFrame].ShadeMiddle,
                Position + offsetLength_EyeBall * offset_Eye,
                center_Eye,
                null,
                GetMyColor(ColorState.Medium, polarity_EyeBall),
                0f,//rotation_Eye,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
            InstanceManager.RenderSprite.Draw(
                eyes[currentEyeFrame].ShadeUpper,
                Position + offsetLength_EyeBall *  offset_Eye,
                center_Eye,
                null,
                GetMyColor(ColorState.Dark, polarity_EyeBall),
                0f,//rotation_Eye,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
            // Draw the outline
            InstanceManager.RenderSprite.Draw(
                eyes[currentEyeFrame].Outline,
                Position + offsetLength_EyeBall * offset_Eye,
                center_Eye,
                null,
                Color.White,
                0f,//rotation_Eye,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);            
            #endregion

        }

        public override void Update(GameTime gameTime)
        {
            double delta = gameTime.ElapsedGameTime.TotalSeconds;
            #region Visual updates
            float bp = MathHelper.Clamp(((float)ServiceLocator.GetService<AudioManager>().BeatPercentage() - 0.5f) / 0.5f, 0, 1f);
            // Spinner
            size_Spinner = MathHelper.Lerp(minScale_Spinner, maxScale_Spinner, bp);
            rotation_Spinner += delta_SpinnerRotation;

            // Body
            for (int i = 0; i < body.Length; i++)
            {
                //body[i].TimerColorChange += delta;
                //if (body[i].TimerColorChange > totalTime_BodyColorChange)
                //{
                    //body[i].TimerColorChange = 0;
                    //body[i].Color = MWMathHelper.GetRandomInRange(0, colorArray_TasteTheRainbow.Length);
                //}
                body[i].Rotation += body[i].DeltaRotation;
                if (body[i].Rotation > MathHelper.TwoPi)
                    body[i].Rotation -= MathHelper.TwoPi;
                if (body[i].Rotation < 0)
                    body[i].Rotation = MathHelper.TwoPi + body[i].Rotation;

                if (body[i].DeltaAlphaDirection)
                    body[i].Alpha += body[i].DeltaAlpha;
                else
                    body[i].Alpha -= body[i].DeltaAlpha;

                if (body[i].Alpha > maxAlpha_Body)
                {
                    body[i].Alpha = maxAlpha_Body;
                    body[i].DeltaAlphaDirection = !body[i].DeltaAlphaDirection;
                }
                else if (body[i].Alpha < minAlpha_Body)
                {
                    body[i].Alpha = minAlpha_Body;
                    body[i].Color = MWMathHelper.GetRandomInRange(0, colorArray_TasteTheRainbow.Length);
                    body[i].DeltaAlphaDirection = !body[i].DeltaAlphaDirection;
                }
            }
            // Eyeball
            timer_EyeBall += delta;
            switch (currentEyeState)
            {
                case MolochEyeState.Opening:
                    if (timer_EyeBall > totalTime_EyeBallBlinkTick)
                    {
                        timer_EyeBall = 0;
                        currentEyeFrame--;
                        if (currentEyeFrame < 0)
                        {
                            currentEyeFrame = 0;
                            currentEyeState = MolochEyeState.Open;
                        }
                    }
                    break;
                case MolochEyeState.Closing:
                    if (timer_EyeBall > totalTime_EyeBallBlinkTick)
                    {
                        timer_EyeBall = 0;
                        currentEyeFrame++;
                        if (currentEyeFrame >= eyes.Length)
                        {
                            currentEyeFrame = eyes.Length - 1;
                            if (polarity_EyeBall == ColorPolarity.Negative)
                                polarity_EyeBall = ColorPolarity.Positive;
                            else
                                polarity_EyeBall = ColorPolarity.Negative;
                            color_Pupil++;
                            if (color_Pupil >= colorArray_TasteTheRainbow.Length)
                                color_Pupil = 0;
                            currentEyeState = MolochEyeState.Opening;
                        }
                    }
                    break;
                default:
                    // Default is open
                    if (timer_EyeBall > totalTime_EyeBallOpen)
                    {
                        timer_EyeBall = 0;
                        currentEyeState = MolochEyeState.Closing;
                    }
                    break;
            }
            timer_EyeStare += delta;
            if (timer_EyeStare > totalTime_EyeStareOrbit)
                timer_EyeStare -= totalTime_EyeStareOrbit;
            SetEyeOffsets();
            // Tube rotation
            timer_TubeRotation += delta;
            if (timer_TubeRotation > totalTime_TubeRotationRampUp)
            {
                timer_TubeRotation = 0;
                tubeRotationRampUp = !tubeRotationRampUp;
            }

            if (tubeRotationRampUp)
                delta_CurrentTubeRotation = MathHelper.Lerp(
                    minDelta_TubeRotation,
                    maxDelta_TubeRotation,
                    (float)(timer_TubeRotation / totalTime_TubeRotationRampUp));
            else
                delta_CurrentTubeRotation = MathHelper.Lerp(
                    maxDelta_TubeRotation,
                    minDelta_TubeRotation,
                    (float)(timer_TubeRotation / totalTime_TubeRotationRampUp));

            for (int i = 0; i < tubes.Length; i++)
            {
                tubes[i].Angle += delta_CurrentTubeRotation;
                if (tubes[i].Angle > MathHelper.TwoPi)
                {
                    tubes[i].Angle -= MathHelper.TwoPi;
                }
                else if (tubes[i].Angle < 0)
                {
                    tubes[i].Angle += MathHelper.TwoPi;
                }
                tubes[i].Offset = GetTubeOffset((double)tubes[i].Angle);
                tubes[i].Rotation = GetTubeRotation(tubes[i].Angle);
                tubes[i].Timer += delta;
                if (tubes[i].Timer > totalTime_TubeAnimationTick)
                {
                    tubes[i].Timer = 0;
                    if (tubes[i].MoveOut)
                        tubes[i].CurrentFrame++;
                    else
                        tubes[i].CurrentFrame--;
                    if (tubes[i].CurrentFrame >= tubeFrames.Length)
                    {
                        // FIXME should fire here
                        tubes[i].MoveOut = false;
                        tubes[i].CurrentFrame = tubeFrames.Length - 1;
                    }
                    else if (tubes[i].CurrentFrame < 0)
                    {
                        tubes[i].MoveOut = true;
                        tubes[i].CurrentFrame = 0;
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}
