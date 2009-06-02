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
        SpawningEye,
        EyeDying,
        GeneralDying,
    }

    public enum MolochEyeState
    {
        Open,
        Closing,
        Opening,
    }

    public struct ExplosionElement
    {
        public bool IsGloopType;
        public double Timer;
        public Color Color;
        public Vector2 Position;
        public float Rotation;
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
        private const int numberOfBlobsInShaft = 10;

        private const string filename_GloopDeath = "Enemies/gloop/glooplet-death";
        private const float minScale_GloopDeath = 0.5f;
        private const float maxScale_GloopDeath = 1.2f;
        private const float minAlpha_GloopDeath = 0.3f;
        private const float maxAlpha_GloopDeath = 0.9f;
        private const string filename_BulletHits = "bullet-hit-0{0}";
        private const int frames_BulletHits = 5;
        private const int numberOfActiveExplosion = 15;
        private const int numberOfDeathCoughExplosions = 60;

        private const string filename_TubeExplode = "Audio/PlayerEffects/splat-explode";
        private const float volume_TubeExplode = 1f;
        private const string filename_EyeBallWobble = "Audio/PlayerEffects/end-boss-wobble";
        private const float volume_EyeBallWobble = 0.85f;
        private const string filename_EndBoom = "Audio/PlayerEffects/big-badda-boom";
        private const float volume_EndBoom = 0.85f;

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

        private const float radius_TubeGuy = 90f;
        private const float offset_TubeGuy = 95f;
        private const float offset_Asplosions = 10f;
        private const int numberOfExplosionsPerBlob = 5;
        private const int numberOfShotsPerTube = 3;

        //private const double totalTime_SpinnerColorChange = 1.02;
        //private const double totalTime_BodyColorChange = 2.51;
        private const double totalTime_EyeBallBlinkTick = 0.1;
        private const double totalTime_EyeBallOpen = 1.5;
        private const double totalTime_TubeRotationRampUp = 2.51;
        private const double totalTime_TubeAnimationTick = 0.5;
        private const double totalTime_EyeStareOrbit = 2.54;
        private const double totalTime_TubeDeath = 1.1;
        private const double totalTime_EyeSpawn = 2.981;
        private const double totalTime_SteadyState = 6.5;
        private const double totalTime_MovingState = 5.342;
        private const double totalTime_EyeDying = 6.781;
        private const double totalTime_EyeDyingMove = 2.121;
        private const double totalTime_GeneralDeath = 7.123;
        private const double totalTime_Explosion = 0.81;

        private const int chanceOfExternalExposion = 5;

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
        private const float minOffsetLength_EyeBall = 225f;

        private const float maxOffsetLength_EyeBall = 450f;
        private const float radius_Eyeball = 50f;
        /// <summary>
        /// The offset length of the pupil from center of eyeball
        /// </summary>
        private const float offsetLength_Pupil = 22f;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPoints = 25;

        private const int eyeBallHitPoints = 40;

        private const float offscreenMovementMultiplier = 0.35f;
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
        private Texture2D texture_GloopDeath;
        private Texture2D[] texture_BulletHit;
        private Vector2 center_GloopDeath;
        private Vector2 center_BulletHit;
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
        private Enemy_MolochPart molochPart_Eye;
        private float currentOffsetLength_EyeBall;
        private RenderSpriteBlendMode currentBlendMode;
        private ExplosionElement[] activeExplosions;

        // Relation to player stuff
        private Vector2 vectorToNearestPlayer;
        private Player nearestPlayer;
        private Vector2 centerOfScreen;

        // State stuff
        private MolochState currentState;
        //private double timer_SpinnerColorChange;
        private double timer_EyeBall;
        private int currentEyeFrame;
        private MolochEyeState currentEyeState;
        private double timer_TubeRotation;
        private bool tubeRotationRampUp;
        private double timer_EyeStare;
        private bool isEyeEngaged;
        private double timer_EyeStateChange;
        private double timer_GeneralState;
        private Vector2 position_Last;
        private Vector2 position_Next;
        private int currentUpperLimit;

        // Audio stuff
        private AudioManager audio;
        private SoundEffect sfx_TubeExplode;
        private SoundEffectInstance sfxi_TubeExplode;
        private SoundEffect sfx_EyeBallWobble;
        private SoundEffectInstance sfxi_EyeBallWobble;
        private SoundEffect sfx_EndBoom;
        private SoundEffectInstance sfxi_EndBoom;
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
            //MyEnemyType = EnemyType.Leader;
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
            Position = Vector2.Zero - RealSize.Length() * Vector2.One;
                /*new Vector2(
                    (float)InstanceManager.DefaultViewport.Width, 0);*/
            // Set next position

            // Set state
            currentState = MolochState.Moving;
            timer_GeneralState = 0;
            currentUpperLimit = numberOfBlobsInShaft;
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
            currentBlendMode = RenderSpriteBlendMode.AlphaBlendTop;
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
            SetNextDestination();

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

                LocalInstanceManager.Enemies[i] = new Enemy_MolochPart(
                    MyManager,
                    this,
                    i,
                    radius_TubeGuy);
                LocalInstanceManager.Enemies[i].Initialize(
                    GetPartPosition(i),
                    Vector2.Zero,
                    ColorState,
                    tubes[i].ColorPolarity,
                    StartHitPoints);
            }

            delta_CurrentTubeRotation = minDelta_TubeRotation;
            timer_TubeRotation = 0;
            tubeRotationRampUp = true;

            // Set up spinner information
            rotation_Spinner = 0;
            size_Spinner = maxScale_Spinner;
            color_Spinner = MWMathHelper.GetRandomInRange(0, colorArray_TasteTheRainbow.Length);
            //timer_SpinnerColorChange = 0;

            // Set up a default eye stuff
            vectorToNearestPlayer = Vector2.Zero;
            nearestPlayer = null;
            SetEyeOffsets();
            color_Pupil = MWMathHelper.GetRandomInRange(0, colorArray_TasteTheRainbow.Length);
            polarity_EyeBall = ColorPolarity.Positive;
            timer_EyeBall = 0;
            currentEyeFrame = 0;
            currentEyeState = MolochEyeState.Open;
            currentOffsetLength_EyeBall = minOffsetLength_EyeBall;
            timer_EyeStare = 0;
            isEyeEngaged = false;
            molochPart_Eye = new Enemy_MolochPart(
                MyManager,
                this,
                -1,
                radius_Eyeball);
            timer_EyeStateChange = 0;

            // Set up the explosion stuff
            texture_GloopDeath = InstanceManager.AssetManager.LoadTexture2D(filename_GloopDeath);
            center_GloopDeath = new Vector2(
                texture_GloopDeath.Width / 2f, texture_GloopDeath.Height / 2f);
            texture_BulletHit = new Texture2D[frames_BulletHits];
            for (int i = 0; i < frames_BulletHits; i++)
            {
                texture_BulletHit[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_BulletHits, (i + 1).ToString()));
            }
            center_BulletHit = new Vector2(
                texture_BulletHit[0].Width / 2f, texture_BulletHit[0].Height / 2f);

            activeExplosions = new ExplosionElement[numberOfActiveExplosion];
            for (int i = 0; i < numberOfActiveExplosion; i++)
            {
                activeExplosions[i].IsGloopType = MWMathHelper.CoinToss();
                activeExplosions[i].Position = (float)MWMathHelper.GetRandomInRange(0, (double)Radius) *
                    GetRandomUnitVector();
                activeExplosions[i].Timer = MWMathHelper.GetRandomInRange(0, totalTime_Explosion);
                activeExplosions[i].Color = GetRandomColor();
                activeExplosions[i].Rotation = (float)MWMathHelper.GetRandomInRange(0, (double)MathHelper.TwoPi);
            }

            // Load audio things
            sfx_TubeExplode = InstanceManager.AssetManager.LoadSoundEffect(filename_TubeExplode);
            sfxi_TubeExplode = null;
            sfx_EyeBallWobble = InstanceManager.AssetManager.LoadSoundEffect(filename_EyeBallWobble);
            sfxi_EyeBallWobble = null;
            sfx_EndBoom = InstanceManager.AssetManager.LoadSoundEffect(filename_EndBoom);
            sfxi_EndBoom = null;

            Alive = true;
            Initialized = true;
        }

        public override String[] GetSFXFilenames()
        {
            return new String[]
            {
                filename_TubeExplode,
                filename_EyeBallWobble,
            };
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

        public override void CleanUp()
        {
            try
            {
                sfxi_EndBoom.Stop();
            }
            catch { }
            try
            {
                sfxi_EyeBallWobble.Stop();
            }
            catch { }
            try
            {
                sfxi_TubeExplode.Stop();
            }
            catch { }
            base.CleanUp();
        }
        #endregion

        #region Private methods
        private void SpawnTubeBabby(int index)
        {
            Vector2 pos = GetPartPosition(index);
            int num = 0;
            if ((pos.X >= 0 && pos.X <= InstanceManager.DefaultViewport.Width) &&
                (pos.Y >= 0 && pos.Y <= InstanceManager.DefaultViewport.Height))
            {
                // We only proceed if we are on screen
                for (int i = 0; i < LocalInstanceManager.CurrentNumberEnemies; i++)
                {
                    if (!LocalInstanceManager.Enemies[i].Alive)
                    {
                        LocalInstanceManager.Enemies[i] = new Enemy_Firefly(MyManager);
                        LocalInstanceManager.Enemies[i].Initialize(
                            pos,
                            Vector2.Zero,
                            ColorState,
                            tubes[index].ColorPolarity,
                            1);
                        num++;
                        if(num >= numberOfShotsPerTube)
                            break;
                    }
                }
            }
        }

        private void SpawnEyeBabby()
        {
        }
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
            Vector2 temp_offset_Eye = offset_Eye * currentOffsetLength_EyeBall;
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
                offset_Pupil = nearestPlayer.Position - (Position + temp_offset_Eye);
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

        private Vector2 GetRandomUnitVector()
        {
            Vector2 temp = new Vector2(
                (float)MWMathHelper.GetRandomInRange(-1.0, 1.0),
                (float)MWMathHelper.GetRandomInRange(-1.0, 1.0));
            temp.Normalize();
            return temp;
        }

        private Color GetRandomColor()
        {
            Color c;
            if (MWMathHelper.CoinToss())
            {
                ColorPolarity p;
                if (MWMathHelper.CoinToss())
                    p = ColorPolarity.Negative;
                else
                    p = ColorPolarity.Positive;

                switch (MWMathHelper.GetRandomInRange(0, 3))
                {
                    case 0:
                        c = GetMyColor(ColorState.Dark, p);
                        break;
                    case 1:
                        c = GetMyColor(ColorState.Medium, p);
                        break;
                    default:
                        c = GetMyColor(ColorState.Light, p);
                        break;
                }
            }
            else
            {
                c = colorArray_TasteTheRainbow[MWMathHelper.GetRandomInRange(
                    0, colorArray_TasteTheRainbow.Length)];
            }
            return c;
        }

        private void KickOffRandomExplosion()
        {
            Vector2 temp = centerOfScreen - Position;
            temp.Normalize();
            temp = (float)MWMathHelper.GetRandomInRange((double)Radius/2.0, (double)Radius) *
                    MWMathHelper.RotateVectorByRadians(
                    temp,
                    (float)MWMathHelper.GetRandomInRange((double)(-MathHelper.PiOver2), (double)MathHelper.PiOver2));
            if (MWMathHelper.CoinToss())
            {
                LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                    Position + temp,
                    GetRandomColor());
            }
            else
            {
                LocalInstanceManager.EnemySplatterSystem.AddParticles(
                    Position + temp,
                    GetRandomColor());
            }
        }

        /// <summary>
        /// Figure out where we're going next
        /// </summary>
        private void SetNextDestination()
        {
            position_Last = Position;

            // Next position is dependent upon where we currently are
            if (Position.X <= -RealSize.X*offscreenMovementMultiplier || Position.X >= InstanceManager.DefaultViewport.Width + RealSize.X*offscreenMovementMultiplier ||
                Position.Y <= -RealSize.Y*offscreenMovementMultiplier || Position.Y >= InstanceManager.DefaultViewport.Height + RealSize.Y *offscreenMovementMultiplier)
            {
                // We're off the screen, we can go nearly anywhere
                InstanceManager.Logger.LogEntry("Enemy_Moloch.SetNextDestination(): Offscreen to onscreen");
                // Let's stick to corners
                switch (MWMathHelper.GetRandomInRange(0, 4))
                {
                    case 0:
                        // Upper left corner
                        position_Next = Vector2.Zero;
                        break;
                    case 1:
                        // Upper right corner
                        position_Next = new Vector2(
                            InstanceManager.DefaultViewport.Width, 0);
                        break;
                    case 2:
                        // Lower right corner
                        position_Next = new Vector2(
                            InstanceManager.DefaultViewport.Width, InstanceManager.DefaultViewport.Height);
                        break;
                    default:
                        // Lower left corner
                        position_Next = new Vector2(
                            0, InstanceManager.DefaultViewport.Height);
                        break;
                }
                position_Last = position_Next - centerOfScreen;
                position_Last.Normalize();
                position_Last = position_Next + position_Last * RealSize.Length() * offscreenMovementMultiplier;
            }
            else if (MWMathHelper.GetRandomInRange(0, 3) == 1)
            {
                // aim off screen
                InstanceManager.Logger.LogEntry("Enemy_Moloch.SetNextDestination(): Aim offscreen, location known");
                position_Next = position_Last - centerOfScreen;
                position_Next.Normalize();
                position_Next = position_Last + position_Next * RealSize.Length() * offscreenMovementMultiplier;
            }
            else
            {
                // aim for another corner
                if (((Position.X <= 2 && Position.X >= -2) 
                    || (Position.X >= InstanceManager.DefaultViewport.Width - 2 && Position.X <= InstanceManager.DefaultViewport.Width + 2))
                    && Position.Y >= 0 && Position.Y <= InstanceManager.DefaultViewport.Height)
                {
                    float x = (float)MWMathHelper.GetRandomInRange(0, 5);
                    if (Position.X >= InstanceManager.DefaultViewport.Width - 2)
                        x = InstanceManager.DefaultViewport.Width - x;
                    // We're on a side
                    if (Position.Y >= InstanceManager.DefaultViewport.Height/2f)
                    {
                        InstanceManager.Logger.LogEntry("Enemy_Moloch.SetNextDestination(): Side, moving up");
                        // Aim for top corner
                        position_Next = new Vector2(x, 0);
                    }
                    else
                    {
                        InstanceManager.Logger.LogEntry("Enemy_Moloch.SetNextDestination(): Side, moving down");
                        // Aim for bottom corner
                        position_Next = new Vector2(x, InstanceManager.DefaultViewport.Height);
                    }
                }
                else if (((Position.Y <= 2 && Position.Y >=-2)
                    || (Position.Y >= InstanceManager.DefaultViewport.Height - 2 && Position.Y <= InstanceManager.DefaultViewport.Height + 2))
                    && Position.X >= 0 && Position.X <= InstanceManager.DefaultViewport.Width)
                {
                    float y = (float)MWMathHelper.GetRandomInRange(0, 5);
                    if (Position.Y >= InstanceManager.DefaultViewport.Height - 2)
                        y = InstanceManager.DefaultViewport.Height - y;
                    // We're on top or bottom
                    if (Position.X <= InstanceManager.DefaultViewport.Width/2f)
                    {
                        InstanceManager.Logger.LogEntry("Enemy_Moloch.SetNextDestination(): Top/Bot, moving right");
                        // Aim for right
                        position_Next = new Vector2(InstanceManager.DefaultViewport.Width, y);
                    }
                    else
                    {
                        InstanceManager.Logger.LogEntry("Enemy_Moloch.SetNextDestination(): Top/Bot, moving left");
                        // Aim for left
                        position_Next = new Vector2(0, y);
                    }
                }
                else
                {
                    // Dude, fuck, I don't know where we are...
                    // Aim at nearest offscreen and GTFO
                    InstanceManager.Logger.LogEntry("Enemy_Moloch.SetNextDestination(): Location unknown");
                    position_Next = position_Last - centerOfScreen;
                    position_Next.Normalize();
                    position_Next = position_Last + position_Next * RealSize.Length() * offscreenMovementMultiplier;
                }
            }
        }
        #endregion

        #region Movement overrides
        public override bool StartOffset()
        {
            vectorToNearestPlayer = Vector2.One * 3f * InstanceManager.DefaultViewport.Width;
            nearestPlayer = null;
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Player)
            {
                // Player
                Vector2 vToPlayer = this.Position - pobj.Position;
                float len = vToPlayer.Length();
                if (len < vectorToNearestPlayer.Length())
                {
                    vectorToNearestPlayer = vToPlayer;
                    nearestPlayer = (Player)pobj;
                }
                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em
                    return pobj.TriggerHit(this);
                }

                // Beam handling
                int temp = ((Player)pobj).IsInBeam(this);
                //inBeam = false;
                if (temp != 0)
                {
                    //inBeam = true;
                    if (temp == -1)
                    {
                        LocalInstanceManager.Steam.AddParticles(Position, GetMyColor(ColorState.Light));
                    }
                }
            }
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

        #region Public methods
        public Vector2 GetPartPosition(int index)
        {
            if (index >= 0)
                return Position + tubes[index].Offset + offset_TubeGuy * Vector2.Normalize(tubes[index].Offset);
            else
            {
                return Position + currentOffsetLength_EyeBall * offset_Eye;
            }
        }

        public void TriggerPartDeath(int index)
        {
            if (index >= 0)
            {
                if (tubes[index].Alive)
                {
                    tubes[index].Alive = false;
                    tubes[index].Timer = 0;
                    if (sfxi_TubeExplode == null)
                    {
                        try
                        {
                            sfxi_TubeExplode = sfx_TubeExplode.Play(volume_TubeExplode);
                        }
                        catch { }
                    }
                    else if (sfxi_TubeExplode.State == SoundState.Stopped ||
                             sfxi_TubeExplode.State == SoundState.Paused)
                    {
                        try
                        {
                            sfxi_TubeExplode.Play();
                        }
                        catch { }
                    }
                }
            }
            else
            {
                // We're dealing with the eye, should trigger death
                currentState = MolochState.EyeDying;
                timer_GeneralState = 0;
                // Splat the eyeball
                LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                    Position + currentOffsetLength_EyeBall * offset_Eye + offset_Asplosions * GetRandomUnitVector(),
                    colorArray_TasteTheRainbow[color_Pupil]);
                LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                    Position + currentOffsetLength_EyeBall * offset_Eye + offset_Asplosions * GetRandomUnitVector(),
                    GetMyColor(ColorState.Medium, polarity_EyeBall));
                LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                    Position + currentOffsetLength_EyeBall * offset_Eye + offset_Asplosions * GetRandomUnitVector(),
                    GetMyColor(ColorState.Light, polarity_EyeBall));
                LocalInstanceManager.EnemySplatterSystem.AddParticles(
                    Position + currentOffsetLength_EyeBall * offset_Eye + offset_Asplosions * GetRandomUnitVector(),
                    GetMyColor(ColorState.Medium, polarity_EyeBall));
                LocalInstanceManager.EnemySplatterSystem.AddParticles(
                    Position + currentOffsetLength_EyeBall * offset_Eye + offset_Asplosions * GetRandomUnitVector(),
                    colorArray_TasteTheRainbow[color_Pupil]);
                LocalInstanceManager.EnemySplatterSystem.AddParticles(
                    Position + currentOffsetLength_EyeBall * offset_Eye + offset_Asplosions * GetRandomUnitVector(),
                    GetMyColor(ColorState.Dark, polarity_EyeBall));
                // Boundary checking, if we were killed while offscreen, even a little bit,
                // we need to get back on screen
                position_Last = Position;
                position_Next = Position;
                float tempOffsetX = (float)InstanceManager.DefaultViewport.Width - InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent;
                float tempOffsetY = (float)InstanceManager.DefaultViewport.Height - InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent;
                if (Position.X < tempOffsetX)
                    position_Next.X = tempOffsetX;
                else if (Position.X > InstanceManager.DefaultViewport.Width - tempOffsetX)
                    position_Next.X = InstanceManager.DefaultViewport.Width - tempOffsetX;

                if (Position.Y < tempOffsetY)
                    position_Next.Y = tempOffsetY;
                else if (Position.Y > InstanceManager.DefaultViewport.Height - tempOffsetY)
                    position_Next.Y = InstanceManager.DefaultViewport.Height - tempOffsetY;

                InstanceManager.Logger.LogEntry(String.Format(
                    "Death-Last: {0}", position_Last.ToString()));
                InstanceManager.Logger.LogEntry(String.Format(
                    "Death-Next: {0}", position_Next.ToString()));
            }
        }
        #endregion

        #region Draw/Update
        public override void Draw(GameTime gameTime)
        {
            // Draw the tubes
            for (int i = 0; i < tubes.Length; i++)
            {
                #region Tube Draw
                if (tubes[i].Alive)
                {
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
                        currentBlendMode);
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
                        currentBlendMode);
                    InstanceManager.RenderSprite.Draw(
                        tubeFrames[tubes[i].CurrentFrame].Upper,
                        Position + tubes[i].Offset,
                        tubeFrames[tubes[i].CurrentFrame].Center,
                        null,
                        GetMyColor(ColorState.Light, tubes[i].ColorPolarity),
                        tubes[i].Rotation,
                        1f,
                        0f,
                        currentBlendMode);
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
                        currentBlendMode);
                }
                else
                {
                    // Draw the base
                    InstanceManager.RenderSprite.Draw(
                        tubeDead.Base,
                        Position + tubes[i].Offset,
                        tubeDead.Center,
                        null,
                        GetMyColor(ColorState.Dark, tubes[i].ColorPolarity),
                        tubes[i].Rotation,
                        1f,
                        0f,
                        currentBlendMode);
                    // Draw the layers
                    InstanceManager.RenderSprite.Draw(
                        tubeDead.Lower,
                        Position + tubes[i].Offset,
                        tubeDead.Center,
                        null,
                        GetMyColor(ColorState.Medium, tubes[i].ColorPolarity),
                        tubes[i].Rotation,
                        1f,
                        0f,
                        currentBlendMode);
                    InstanceManager.RenderSprite.Draw(
                        tubeDead.Upper,
                        Position + tubes[i].Offset,
                        tubeDead.Center,
                        null,
                        GetMyColor(ColorState.Light, tubes[i].ColorPolarity),
                        tubes[i].Rotation,
                        1f,
                        0f,
                        currentBlendMode);
                    // Draw the outline
                    InstanceManager.RenderSprite.Draw(
                        tubeDead.Outline,
                        Position + tubes[i].Offset,
                        tubeDead.Center,
                        null,
                        Color.White,
                        tubes[i].Rotation,
                        1f,
                        0f,
                        currentBlendMode);
                }
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
                currentBlendMode);

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
                    currentBlendMode);
            }

            #region Draw the eye
            // Draw the shaft
            if (currentState != MolochState.GeneralDying)
            {
                int upperLimit = numberOfBlobsInShaft;
                if (currentState == MolochState.EyeDying)
                {
                    upperLimit = (int)MathHelper.Lerp((float)numberOfBlobsInShaft, 0, (float)(timer_GeneralState / totalTime_EyeDying));
                    //Console.WriteLine("{0}, {1}", upperLimit, (float)(timer_EyeStateChange / totalTime_EyeDying));
                }
                for (int i = 0; i < upperLimit; i++)
                {
                    // Draw the outline
                    InstanceManager.RenderSprite.Draw(
                        texture_Blob,
                        Position + MathHelper.Lerp(0, currentOffsetLength_EyeBall, (float)i / (float)numberOfBlobsInShaft) * offset_Eye,
                        center_Blob,
                        null,
                        Color.Black,
                        0f,
                        scale_BlobOutline,
                        0f,
                        currentBlendMode);
                    // Draw the blob
                    InstanceManager.RenderSprite.Draw(
                        texture_Blob,
                        Position + MathHelper.Lerp(0, currentOffsetLength_EyeBall, (float)i / (float)numberOfBlobsInShaft) * offset_Eye,
                        center_Blob,
                        null,
                        GetMyColor(ColorState.Dark, polarity_EyeBall),
                        0f,
                        scale_Blob,
                        0f,
                        currentBlendMode);
                    // Draw the highlight
                    InstanceManager.RenderSprite.Draw(
                        texture_BlobHighlight,
                        Position + MathHelper.Lerp(0, currentOffsetLength_EyeBall, (float)i / (float)numberOfBlobsInShaft) * offset_Eye +
                        offsetY_BlobHighlight * Vector2.UnitY,
                        center_BlobHighlight,
                        null,
                        Color.White,
                        0f,
                        scale_Blob,
                        0f,
                        currentBlendMode);
                }
                if (upperLimit < numberOfBlobsInShaft)
                {
                    for (int i = upperLimit; i < currentUpperLimit; i++)
                    {
                        for (int t = 0; t < numberOfExplosionsPerBlob; t++)
                        {
                            Color c = GetMyColor(ColorState.Medium, polarity_EyeBall);
                            if (MWMathHelper.CoinToss())
                                c = colorArray_TasteTheRainbow[color_Pupil];

                            if (MWMathHelper.CoinToss())
                                LocalInstanceManager.EnemySplatterSystem.AddParticles(
                                    Position + MathHelper.Lerp(0, currentOffsetLength_EyeBall, (float)i / (float)numberOfBlobsInShaft) * offset_Eye + offset_Asplosions * GetRandomUnitVector(),
                                    c);
                            else
                                LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                                    Position + MathHelper.Lerp(0, currentOffsetLength_EyeBall, (float)i / (float)numberOfBlobsInShaft) * offset_Eye + offset_Asplosions * GetRandomUnitVector(),
                                    c);
                        }
                    }
                    currentUpperLimit = upperLimit;
                }
                if (currentState != MolochState.EyeDying)
                {
                    // Draw the base
                    InstanceManager.RenderSprite.Draw(
                        eyes[currentEyeFrame].Base,
                        Position + currentOffsetLength_EyeBall * offset_Eye,
                        center_Eye,
                        null,
                        Color.White,
                        0f,//rotation_Eye,
                        1f,
                        0f,
                        currentBlendMode);
                    // Draw the pupil
                    InstanceManager.RenderSprite.Draw(
                        texture_EyePupil,
                        Position + currentOffsetLength_EyeBall * offset_Eye + offset_Pupil,
                        center_Pupil,
                        null,
                        colorArray_TasteTheRainbow[color_Pupil],
                        0f,
                        1f,
                        0f,
                        currentBlendMode);
                    // Draw the layers
                    InstanceManager.RenderSprite.Draw(
                        eyes[currentEyeFrame].ShadeLower,
                        Position + currentOffsetLength_EyeBall * offset_Eye,
                        center_Eye,
                        null,
                        GetMyColor(ColorState.Light, polarity_EyeBall),
                        0f,//rotation_Eye,
                        1f,
                        0f,
                        currentBlendMode);
                    InstanceManager.RenderSprite.Draw(
                        eyes[currentEyeFrame].ShadeMiddle,
                        Position + currentOffsetLength_EyeBall * offset_Eye,
                        center_Eye,
                        null,
                        GetMyColor(ColorState.Medium, polarity_EyeBall),
                        0f,//rotation_Eye,
                        1f,
                        0f,
                        currentBlendMode);
                    InstanceManager.RenderSprite.Draw(
                        eyes[currentEyeFrame].ShadeUpper,
                        Position + currentOffsetLength_EyeBall * offset_Eye,
                        center_Eye,
                        null,
                        GetMyColor(ColorState.Dark, polarity_EyeBall),
                        0f,//rotation_Eye,
                        1f,
                        0f,
                        currentBlendMode);
                    // Draw the outline
                    InstanceManager.RenderSprite.Draw(
                        eyes[currentEyeFrame].Outline,
                        Position + currentOffsetLength_EyeBall * offset_Eye,
                        center_Eye,
                        null,
                        Color.White,
                        0f,//rotation_Eye,
                        1f,
                        0f,
                        currentBlendMode);
                }
            }
            #endregion

            #region End Explosions
            if (currentState == MolochState.EyeDying || currentState == MolochState.GeneralDying)
            {
                for (int i = 0; i < numberOfActiveExplosion; i++)
                {
                    if (activeExplosions[i].IsGloopType)
                    {
                        InstanceManager.RenderSprite.Draw(
                            texture_GloopDeath,
                            Position + activeExplosions[i].Position,
                            center_GloopDeath,
                            null,
                            new Color(activeExplosions[i].Color, MathHelper.Lerp(
                               maxAlpha_GloopDeath, minAlpha_GloopDeath,
                               (float)(activeExplosions[i].Timer / totalTime_Explosion))),
                            activeExplosions[i].Rotation,
                            MathHelper.Lerp(
                                minScale_GloopDeath, maxScale_GloopDeath,
                                (float)(activeExplosions[i].Timer / totalTime_Explosion)),
                            0f,
                            currentBlendMode);
                    }
                    else
                    {
                        InstanceManager.RenderSprite.Draw(
                            texture_BulletHit[(int)MathHelper.Lerp(0, frames_BulletHits-1,
                                (float)(activeExplosions[i].Timer / totalTime_Explosion))],
                            Position + activeExplosions[i].Position,
                            center_BulletHit,
                            null,
                            activeExplosions[i].Color,
                            activeExplosions[i].Rotation,
                            1f,
                            0f,
                            currentBlendMode);
                    }
                }
            }
            #endregion
        }

        public override void Update(GameTime gameTime)
        {
            double delta = gameTime.ElapsedGameTime.TotalSeconds;
            float bp = MathHelper.Clamp(((float)ServiceLocator.GetService<AudioManager>().BeatPercentage() - 0.5f) / 0.5f, 0, 1f);
            // Spinner
            size_Spinner = MathHelper.Lerp(minScale_Spinner, maxScale_Spinner, bp);
            rotation_Spinner += delta_SpinnerRotation;

            UpdateBody(delta);
            UpdateEye(delta);
            UpdateTubes(delta);

            // Deal with the state
            switch (currentState)
            {
                case MolochState.SpawningEye:
                    timer_EyeStateChange += delta;
                    if (timer_EyeStateChange > totalTime_EyeSpawn)
                    {
                        currentState = MolochState.Steady;
                        timer_EyeStateChange = 0;
                        currentOffsetLength_EyeBall = maxOffsetLength_EyeBall;
                    }
                    else
                    {
                        currentOffsetLength_EyeBall = MathHelper.Lerp(
                            minOffsetLength_EyeBall, maxOffsetLength_EyeBall, (float)(timer_EyeStateChange / totalTime_EyeSpawn));
                    }
                    break;
                case MolochState.EyeDying:
                    timer_GeneralState += delta;
                    if (timer_GeneralState > totalTime_EyeDying)
                    {
                        currentState = MolochState.GeneralDying;
                        LocalInstanceManager.AchievementManager.EnemyDeathCount(MyType);
                        timer_GeneralState = 0;
                        Position = position_Next;
                        InstanceManager.Logger.LogEntry(String.Format(
                            "Enemy_Moloch>GeneralDying {0}", Position.ToString()));
                    }
                    else
                    {
                        if (timer_GeneralState <= totalTime_EyeDyingMove && position_Next != position_Last)
                        {
                            Position = new Vector2(
                                MathHelper.Lerp(position_Last.X, position_Next.X, (float)(timer_GeneralState / totalTime_EyeDyingMove)),
                                MathHelper.Lerp(position_Last.Y, position_Next.Y, (float)(timer_GeneralState / totalTime_EyeDyingMove)));
                        }
                        else
                        {
                            Position = position_Next;
                        }
                    }
                    if (sfxi_EndBoom == null)
                    {
                        try
                        {
                            sfxi_EndBoom = sfx_EndBoom.Play(volume_EndBoom);
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            if (sfxi_EndBoom.State == SoundState.Paused ||
                                sfxi_EndBoom.State == SoundState.Stopped)
                            {
                                sfxi_EndBoom.Play();
                            }
                        }
                        catch { }
                    }
                    if (sfxi_EyeBallWobble != null)
                    {
                        if (sfxi_EyeBallWobble.State == SoundState.Playing)
                            sfxi_EyeBallWobble.Stop();
                    }
                    UpdateExplosions(delta);
                    break;
                case MolochState.GeneralDying:
                    timer_GeneralState += delta;
                    if (sfxi_EndBoom == null)
                    {
                        try
                        {
                            sfxi_EndBoom = sfx_EndBoom.Play(volume_EndBoom);
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            if (sfxi_EndBoom.State == SoundState.Paused ||
                                sfxi_EndBoom.State == SoundState.Stopped)
                            {
                                sfxi_EndBoom.Play();
                            }
                        }
                        catch { }
                    }
                    if (sfxi_EyeBallWobble != null)
                    {
                        if (sfxi_EyeBallWobble.State == SoundState.Playing)
                            sfxi_EyeBallWobble.Stop();
                    }
                    if (timer_GeneralState > totalTime_GeneralDeath)
                    {
                        timer_GeneralState = 0;
                        Alive = false;
                        for (int i = 0; i < numberOfDeathCoughExplosions; i++)
                            KickOffRandomExplosion();
                        try
                        {
                            sfxi_EndBoom.Stop();
                        }
                        catch { }
                        try
                        {
                            sfxi_EyeBallWobble.Stop();
                        }
                        catch { }
                        try
                        {
                            sfxi_TubeExplode.Stop();
                        }
                        catch { }
                        audio.PlayEffect(EffectID.PlayerExplosion);
                    }
                    UpdateExplosions(delta);
                    break;
                case MolochState.Moving:
                    // Reminder: We need to check if we're off screen at the final destination
                    // if so, then we need to immediately get a new destination and remain
                    // at moving
                    timer_GeneralState += delta;
                    if (timer_GeneralState <= totalTime_MovingState)
                    {
                        Position = new Vector2(
                            MathHelper.Lerp(position_Last.X, position_Next.X, (float)(timer_GeneralState / totalTime_MovingState)),
                            MathHelper.Lerp(position_Last.Y, position_Next.Y, (float)(timer_GeneralState / totalTime_MovingState)));
                    }
                    else
                    {
                        Position = position_Next;
                        timer_GeneralState = 0;
                        if (Position.X <= -RealSize.X * offscreenMovementMultiplier || Position.X >= InstanceManager.DefaultViewport.Width + RealSize.X * offscreenMovementMultiplier ||
                            Position.Y <= -RealSize.Y * offscreenMovementMultiplier || Position.Y >= InstanceManager.DefaultViewport.Height + RealSize.Y * offscreenMovementMultiplier)
                        {
                            // offscreen, start again
                            SetNextDestination();
                        }
                        else
                        {
                            InstanceManager.Logger.LogEntry("Enemy_Moloch>Setting steady state");
                            currentState = MolochState.Steady;
                        }
                    }
                    break;
                default:
                    // Steady
                    timer_GeneralState += delta;
                    if (timer_GeneralState > totalTime_SteadyState)
                    {
                        timer_GeneralState = 0;
                        SetNextDestination();
                        InstanceManager.Logger.LogEntry("Enemy_Moloch>Setting moving state");
                        currentState = MolochState.Moving;
                    }
                    break;
            }

        }
        #endregion

        #region Private update methods
        private void UpdateExplosions(double delta)
        {
            for (int i = 0; i < numberOfActiveExplosion; i++)
            {
                activeExplosions[i].Timer += delta;
                if (activeExplosions[i].Timer > totalTime_Explosion)
                {
                    activeExplosions[i].Timer = 0;
                    activeExplosions[i].IsGloopType = MWMathHelper.CoinToss();
                    activeExplosions[i].Position = centerOfScreen - Position;
                    activeExplosions[i].Position.Normalize();
                    activeExplosions[i].Position = (float)MWMathHelper.GetRandomInRange((double)Radius/2.0, (double)Radius) *
                        MWMathHelper.RotateVectorByRadians(
                        activeExplosions[i].Position,
                        (float)MWMathHelper.GetRandomInRange((double)(-MathHelper.PiOver2), (double)MathHelper.PiOver2));
                    activeExplosions[i].Color = GetRandomColor();
                    activeExplosions[i].Rotation = (float)MWMathHelper.GetRandomInRange(0, (double)MathHelper.TwoPi);
                }
            }

            if (MWMathHelper.GetRandomInRange(0, chanceOfExternalExposion) == 1)
            {
                KickOffRandomExplosion();
            }
        }

        private void UpdateTubes(double delta)
        {
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

            int temp_NumTubesUp = 0;
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
                if (tubes[i].Alive)
                {
                    temp_NumTubesUp++;
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
                            SpawnTubeBabby(i);
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
                else
                {
                    if (tubes[i].Timer < totalTime_TubeDeath)
                    {
                        tubes[i].Timer += delta;
                        Color c;
                        switch (MWMathHelper.GetRandomInRange(0, 4))
                        {
                            case 0:
                                c = GetMyColor(ColorState.Dark, tubes[i].ColorPolarity);
                                break;
                            case 1:
                                c = GetMyColor(ColorState.Medium, tubes[i].ColorPolarity);
                                break;
                            case 2:
                                c = GetMyColor(ColorState.Light, tubes[i].ColorPolarity);
                                break;
                            default:
                                c = colorArray_TasteTheRainbow[MWMathHelper.GetRandomInRange(0, colorArray_TasteTheRainbow.Length)];
                                break;
                        }
                        if (MWMathHelper.CoinToss())
                        {
                            LocalInstanceManager.EnemySplatterSystem.AddParticles(
                                GetPartPosition(i) + new Vector2(
                                    (float)MWMathHelper.GetRandomInRange(-radius_TubeGuy, radius_TubeGuy),
                                    (float)MWMathHelper.GetRandomInRange(-radius_TubeGuy, radius_TubeGuy)),
                                c);
                        }
                        else
                        {
                            LocalInstanceManager.EnemyExplodeSystem.AddParticles(
                                GetPartPosition(i) + new Vector2(
                                    (float)MWMathHelper.GetRandomInRange(-radius_TubeGuy, radius_TubeGuy),
                                    (float)MWMathHelper.GetRandomInRange(-radius_TubeGuy, radius_TubeGuy)),
                                c);
                        }
                    }
                }
            }

            if (temp_NumTubesUp == 0 && !isEyeEngaged)
            {
                // Well shit my bricks and call me Mary, the tubes are dead
                isEyeEngaged = true;
                // Spawn ourselves an eye part
                molochPart_Eye.Initialize(
                    Position + currentOffsetLength_EyeBall * offset_Eye,
                    Vector2.Zero,
                    ColorState,
                    polarity_EyeBall,
                    eyeBallHitPoints);
                LocalInstanceManager.Enemies[0].CleanUp();
                LocalInstanceManager.Enemies[0] = molochPart_Eye;
                timer_EyeStateChange = 0;
                currentBlendMode = RenderSpriteBlendMode.AlphaBlend;
                currentState = MolochState.SpawningEye;
            }
        }

        private void UpdateEye(double delta)
        {
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
                            if (isEyeEngaged)
                            {
                                LocalInstanceManager.Enemies[0].ColorPolarity = polarity_EyeBall;
                            }
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
            if (isEyeEngaged)
            {
                if (sfxi_EyeBallWobble == null)
                {
                    try
                    {
                        sfxi_EyeBallWobble = sfx_EyeBallWobble.Play(volume_EyeBallWobble);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        if (sfxi_EyeBallWobble.State == SoundState.Paused ||
                            sfxi_EyeBallWobble.State == SoundState.Stopped)
                        {
                            sfxi_EyeBallWobble.Play();
                        }
                    }
                    catch
                    { }
                }
            }
        }

        private void UpdateBody(double delta)
        {
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
        }
        #endregion
    }
}
