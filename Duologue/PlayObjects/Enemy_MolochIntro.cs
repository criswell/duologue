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
using Mimicware.Fx;
// Duologue
using Duologue.Audio;
using Duologue.State;
using Duologue.Screens;
using Duologue.Properties;
#endregion


namespace Duologue.PlayObjects
{
    public struct Tentacle
    {
        public Vector2 StartPosition;
        public Vector2 EndPosition;
        public double Timer_Orientated; // EAT IT, DOCTORCAL
        public double Timer_ColorChange;
        public double Timer_Movement;
        public int LastColor;
        public int NextColor;
    }

    public class Enemy_MolochIntro : Enemy
    {
        #region Constants
        private const string filename_EyeBall = "Enemies/end/end-boss-eye{0}-eyeball";
        private const string filename_EyeOutline = "Enemies/end/end-boss-eye{0}-outline";
        private const string filename_EyeShadeLower = "Enemies/end/end-boss-eye{0}-shade-lower";
        private const string filename_EyeShadeMiddle = "Enemies/end/end-boss-eye{0}-shade-middle";
        private const string filename_EyeShadeUpper = "Enemies/end/end-boss-eye{0}-shade-upper";
        private const int frames_Eye = 5;

        private const string filename_Font = "Fonts/inero-40";

        private const string filename_EyePupil = "Enemies/gloop/king-gloop-eye";
        private const string filename_EyeShaftBlob = "Enemies/gloop/glooplet";
        private const string filename_EyeShaftHighlight = "Enemies/gloop/glooplet-highlight";
        private const float scale_Blob = 0.8f;
        private const float offsetY_BlobHighlight = -10f;
        private const float scale_BlobOutline = 1.05f;
        private const float scale_EyeBlobOutline = 0.82f;
        private const int numberOfBlobsInShaft = 10;
        private const float max_BlobSize = 2.1f;
        private const float min_BlobSize = 0.08f;
        private const float min_ShadowOffset = 5f;
        private const float max_ShadowOffset = 50f;

        /// <summary>
        /// The offset length of the eyeball from center of body
        /// </summary>
        private const float offsetLength = 350f;

        private const float offsetLength_EyeBall = 250f;

        private const float maxOrbit_X = 80f;
        private const float maxOrbit_Y = 70f;
        private const float multiplierOrbit_X = 3f;
        private const float multiplierOrbit_Y = 2f;

        private const double totalTime_Orbit = 2.54;
        private const double totalTime_ColorChange = 1.51;
        private const double totalTime_EyeStareOrbit = 2.54;
        private const double totalTime_EyeBallBlinkTick = 0.1;
        private const double totalTime_EyeBallOpen = 1.5;

        private const double totalTime_TextOnScreen1 = 13.98;
        private const double totalTime_Type = 3.55;
        private const double totalTime_TextOnScreen2 = 10.43;

        /// <summary>
        /// The time, from start to finish, for this intro to be alive
        /// </summary>
        private const double totalTime_StartToFinish = 50.987;
        private const double timeTrigger_AllLeave = 42.011;
        /// <summary>
        /// The roll-in times for each tentacle
        /// </summary>
        private const double timeTrigger_RollInOne = 8.325;
        private const double timeTrigger_RollInTwo = 10.567;
        private const double timeTrigger_RollInThree = 12.89;
        private const double timeTrigger_RollInFour = 15.04;
        private const double timeTrigger_RollInFive = 17.293;//16.731;
        private const double timeTrigger_RollInSix = 19.353;

        private const double timeTrigger_EyeBallMoveIn = 25.042;
        private const double timeTrigger_Talk1 = 27.538;
        private const double timeTrigger_Talk2 = 31.088;

        private const double totalTime_TentacleMove = 2.15;
        /// <summary>
        /// The offset length of the pupil from center of eyeball
        /// </summary>
        private const float offsetLength_Pupil = 22f;
        #endregion

        #region Fields
        private EyeBallFrame[] eyes;
        private SpriteFont font;
        private Texture2D texture_Blob;
        private Texture2D texture_BlobHighlight;
        private Texture2D texture_EyePupil;
        private Vector2 center_Pupil;
        private Vector2 center_Blob;
        private Vector2 center_BlobHighlight;
        private Vector2 center_Eye;

        private Color[] colorArray_TasteTheRainbow;
        private Color color_Shadow;

        private Tentacle[] tentacles;

        private Vector2 centerOfScreen;

        private double mainTimer;
        private double[] timeTriggers;

        private Vector2 offset_Eye;
        private Vector2 offset_Pupil;
        private Vector2 vectorToNearestPlayer;
        private Player nearestPlayer;
        private int color_NextEye;
        private int color_LastEye;
        private double timer_EyeColorChange;
        private double timer_EyeStare;
        private int currentEyeFrame;
        private double timer_EyeBall;
        private MolochEyeState currentEyeState;
        private Vector2 startPos;
        private Vector2 endPos;
        private double timer_EyeMove;
        private Teletype teletype;
        private bool shotText1;
        private bool shotText2;
        private TeletypeEntry entry1;
        private TeletypeEntry entry2;
        #endregion

        #region Constructor / Init
        public Enemy_MolochIntro() : base() { }

        public Enemy_MolochIntro(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_MolochIntro;
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
            int? hitPoints,
            double spawnDelay)
        {
            // No pausing during cinematics
            LocalInstanceManager.CanPause = false;
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            SpawnTimeDelay = spawnDelay;
            SpawnTimer = 0;
            if (Guide.IsTrialMode)
                Alive = false; // Skip on trial mode
            else
                Alive = true;
            teletype = ServiceLocator.GetService<Teletype>();
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
            timeTriggers = new double[]
            {
                timeTrigger_RollInOne,
                timeTrigger_RollInTwo,
                timeTrigger_RollInThree,
                timeTrigger_RollInFour,
                timeTrigger_RollInFive,
                timeTrigger_RollInSix
            };

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

            color_Shadow = new Color(Color.Black, 125);

            centerOfScreen = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f, InstanceManager.DefaultViewport.Height / 2f);

            eyes = new EyeBallFrame[frames_Eye];
            texture_EyePupil = InstanceManager.AssetManager.LoadTexture2D(filename_EyePupil);
            center_Pupil = new Vector2(
                texture_EyePupil.Width / 2f, texture_EyePupil.Height / 2f);
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

            Vector2[] tempPos = new Vector2[]
            {
                Vector2.Zero,
                new Vector2(InstanceManager.DefaultViewport.Width, 0),
                new Vector2(0, InstanceManager.DefaultViewport.Height),
                new Vector2(InstanceManager.DefaultViewport.Width, InstanceManager.DefaultViewport.Height),
                new Vector2(-20f, centerOfScreen.Y),
                new Vector2(20 + InstanceManager.DefaultViewport.Width, centerOfScreen.Y),
                //new Vector2(centerOfScreen.X, InstanceManager.DefaultViewport.Height) // YOU GON GIT RAPED
            };

            tentacles = new Tentacle[tempPos.Length];
            Vector2 tempOffset;

            for (int i = 0; i < tentacles.Length; i++)
            {
                tentacles[i].EndPosition = tempPos[i];
                tempOffset = tempPos[i] - centerOfScreen;
                tempOffset.Normalize();
                tentacles[i].StartPosition = tempPos[i] + offsetLength * tempOffset; // + center_Blob.X)
                tentacles[i].Timer_Orientated = MWMathHelper.GetRandomInRange(0, totalTime_Orbit);
                tentacles[i].Timer_ColorChange = MWMathHelper.GetRandomInRange(0, totalTime_ColorChange);
                tentacles[i].Timer_Movement = 0;
                int t = MWMathHelper.GetRandomInRange(0, colorArray_TasteTheRainbow.Length);
                tentacles[i].LastColor = t;
                if (t < colorArray_TasteTheRainbow.Length - 1)
                    tentacles[i].NextColor = t + 1;
                else
                    tentacles[i].NextColor = 0;
            }

            mainTimer = 0;

            // Set up a default eye stuff
            vectorToNearestPlayer = Vector2.Zero;
            nearestPlayer = null;
            SetEyeOffsets();
            color_LastEye = 0;
            color_NextEye = 1;
            timer_EyeColorChange = 0;
            timer_EyeStare = 0;
            currentEyeFrame = 0;
            timer_EyeBall = 0;
            currentEyeState = MolochEyeState.Open;
            endPos = new Vector2(
                centerOfScreen.X, 0);
            startPos = endPos - Vector2.UnitY * (offsetLength_EyeBall + center_Eye.Y);
            Position = startPos;
            timer_EyeMove = 0;

            // Set up text stuff
            font = InstanceManager.AssetManager.LoadSpriteFont(filename_Font);
            Vector2 tempSize = font.MeasureString(Resources.Boss_Moloch1);
            Vector2[] shadowOffset = new Vector2[] 
            {
                new Vector2(-1f, -1f),
                new Vector2(1f, 1f),
                new Vector2(0, 1f),
                new Vector2(1f, 0),
                new Vector2(10f, 10f),
            };
            entry1 = new TeletypeEntry(
                font,
                Resources.Boss_Moloch1,
                centerOfScreen - Vector2.UnitY * tempSize.Y/2f,
                new Vector2(tempSize.X/2f, tempSize.Y/2f),
                new Color(248, 233, 218),
                totalTime_Type,
                totalTime_TextOnScreen1,
                new Color(37, 9, 44, 170),
                shadowOffset,
                InstanceManager.RenderSprite);

            tempSize = font.MeasureString(Resources.Boss_Moloch2);
            entry2 = new TeletypeEntry(
                font,
                Resources.Boss_Moloch2,
                centerOfScreen + Vector2.UnitY * tempSize.Y/2f,
                new Vector2(tempSize.X / 2f, tempSize.Y / 2f),
                new Color(248, 233, 218),
                totalTime_Type,
                totalTime_TextOnScreen2,
                new Color(37, 9, 44, 170),
                shadowOffset,
                InstanceManager.RenderSprite);

            shotText1 = false;
            shotText2 = false;

            Initialized = true;
        }

        public override string[] GetTextureFilenames()
        {
            String[] filenames = new String[5 * frames_Eye + 1];

            int i = 0;
            filenames[i] = filename_EyePupil;
            i++;

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

            return filenames;
        }

        public override void CleanUp()
        {
            // Reset the pause
            LocalInstanceManager.CanPause = true;
            base.CleanUp();
        }
        #endregion

        #region Private methods
        public Vector2 AimAtWiggle(Vector2 curPos, float percentage)
        {
            // Lissajous curve
            Vector2 localOffset = new Vector2(
                maxOrbit_X * (float)Math.Sin(multiplierOrbit_X * MathHelper.Lerp(0, MathHelper.TwoPi, percentage)),
                maxOrbit_Y * (float)Math.Sin(multiplierOrbit_Y * MathHelper.Lerp(0, MathHelper.TwoPi, percentage)));

            localOffset = centerOfScreen + localOffset - curPos;
            localOffset.Normalize();
            return localOffset;
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
                offset_Pupil = nearestPlayer.Position - (Position + temp_offset_Eye);
            }
            offset_Pupil.Normalize();
            offset_Pupil = offset_Pupil * offsetLength_Pupil;
        }
        #endregion

        #region Motion overrides
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

        #region Private Draw
        private void DrawTentacle(Tentacle tentacle)
        {
            Vector2 orientated = AimAtWiggle(tentacle.EndPosition,
                (float)(tentacle.Timer_Orientated / totalTime_Orbit));

            Color tempColor;
            float tempSize;
            float tempOffset;
            Vector2 tempPos = new Vector2(
                    MathHelper.Lerp(
                        tentacle.StartPosition.X,
                        tentacle.EndPosition.X,
                        (float)(tentacle.Timer_Movement / totalTime_TentacleMove)),
                    MathHelper.Lerp(
                        tentacle.StartPosition.Y,
                        tentacle.EndPosition.Y,
                        (float)(tentacle.Timer_Movement / totalTime_TentacleMove)));

            for (int i = numberOfBlobsInShaft - 1; i >= 0; i--)
            {
                tempSize = MathHelper.Lerp(max_BlobSize, min_BlobSize, (float)i / (float)numberOfBlobsInShaft);
                tempOffset = MathHelper.Lerp(0, offsetLength, (float)i / (float)numberOfBlobsInShaft);
                // Draw outline
                InstanceManager.RenderSprite.Draw(
                    texture_Blob,
                    tempPos + orientated * tempOffset,
                    center_Blob,
                    null,
                    Color.Black,
                    0f,
                    tempSize,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);

                // Compute color
                tempColor = new Color(
                    (byte)MathHelper.Lerp(
                        (float)colorArray_TasteTheRainbow[tentacle.LastColor].R,
                        (float)colorArray_TasteTheRainbow[tentacle.NextColor].R, (float)(tentacle.Timer_ColorChange / totalTime_ColorChange)),
                    (byte)MathHelper.Lerp(
                        (float)colorArray_TasteTheRainbow[tentacle.LastColor].G,
                        (float)colorArray_TasteTheRainbow[tentacle.NextColor].G, (float)(tentacle.Timer_ColorChange / totalTime_ColorChange)),
                    (byte)MathHelper.Lerp(
                        (float)colorArray_TasteTheRainbow[tentacle.LastColor].B,
                        (float)colorArray_TasteTheRainbow[tentacle.NextColor].B, (float)(tentacle.Timer_ColorChange / totalTime_ColorChange)));

                // Draw body
                InstanceManager.RenderSprite.Draw(
                    texture_Blob,
                    tempPos + orientated * tempOffset,
                    center_Blob,
                    null,
                    tempColor,
                    0f,
                    tempSize,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);

                // Draw highlight
                InstanceManager.RenderSprite.Draw(
                    texture_BlobHighlight,
                    tempPos + orientated * tempOffset
                        + offsetY_BlobHighlight * tempSize * Vector2.UnitY,
                    center_BlobHighlight,
                    null,
                    Color.White,
                    0f,
                    tempSize,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);

                // Draw shadow
                InstanceManager.RenderSprite.Draw(
                    texture_Blob,
                    tempPos + orientated * tempOffset + Vector2.UnitY *
                        MathHelper.Lerp(min_ShadowOffset, max_ShadowOffset, (float)i / (float)numberOfBlobsInShaft),
                    center_Blob,
                    null,
                    color_Shadow,
                    0f,
                    tempSize,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);
                    
            }
        }

        private void DrawEye()
        {
            // Compute color
            Color tempColor = new Color(
                (byte)MathHelper.Lerp(
                    (float)colorArray_TasteTheRainbow[color_LastEye].R,
                    (float)colorArray_TasteTheRainbow[color_NextEye].R, (float)(timer_EyeColorChange / totalTime_ColorChange)),
                (byte)MathHelper.Lerp(
                    (float)colorArray_TasteTheRainbow[color_LastEye].G,
                    (float)colorArray_TasteTheRainbow[color_NextEye].G, (float)(timer_EyeColorChange / totalTime_ColorChange)),
                (byte)MathHelper.Lerp(
                    (float)colorArray_TasteTheRainbow[color_LastEye].B,
                    (float)colorArray_TasteTheRainbow[color_NextEye].B, (float)(timer_EyeColorChange / totalTime_ColorChange)));

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
                    scale_EyeBlobOutline,
                    0f,
                    RenderSpriteBlendMode.AlphaBlendTop);
                // Draw the blob
                InstanceManager.RenderSprite.Draw(
                    texture_Blob,
                    Position + MathHelper.Lerp(0, offsetLength_EyeBall, (float)i / (float)numberOfBlobsInShaft) * offset_Eye,
                    center_Blob,
                    null,
                    tempColor,
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
                tempColor,
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
                tempColor,
                0f,//rotation_Eye,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
            InstanceManager.RenderSprite.Draw(
                eyes[currentEyeFrame].ShadeMiddle,
                Position + offsetLength_EyeBall * offset_Eye,
                center_Eye,
                null,
                tempColor,
                0f,//rotation_Eye,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
            InstanceManager.RenderSprite.Draw(
                eyes[currentEyeFrame].ShadeUpper,
                Position + offsetLength_EyeBall * offset_Eye,
                center_Eye,
                null,
                tempColor,
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
        #endregion

        #region Private update
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
            timer_EyeColorChange += delta;
            if (timer_EyeColorChange > totalTime_ColorChange)
            {
                timer_EyeColorChange -= totalTime_ColorChange;
                color_LastEye = color_NextEye;
                color_NextEye++;
                if (color_NextEye >= colorArray_TasteTheRainbow.Length)
                    color_NextEye = 0;
            }
            SetEyeOffsets();

            // Eye position
            Position = new Vector2(
                    MathHelper.Lerp(
                        startPos.X,
                        endPos.X,
                        (float)(timer_EyeMove / totalTime_TentacleMove)),
                    MathHelper.Lerp(
                        startPos.Y,
                        endPos.Y,
                        (float)(timer_EyeMove / totalTime_TentacleMove)));
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < tentacles.Length; i++)
            {
                DrawTentacle(tentacles[i]);
            }
            DrawEye();
        }

        public override void Update(GameTime gameTime)
        {
            double delta = gameTime.ElapsedGameTime.TotalSeconds;
            UpdateEye(delta);
            mainTimer += delta;
            if (mainTimer > totalTime_StartToFinish)
            {
                // Reset pause ability
                LocalInstanceManager.CanPause = true;
                Alive = false;
            }

            for (int i = 0; i < tentacles.Length; i++)
            {
                // Color change
                tentacles[i].Timer_ColorChange += delta;
                if (tentacles[i].Timer_ColorChange > totalTime_ColorChange)
                {
                    tentacles[i].Timer_ColorChange = 0;
                    tentacles[i].LastColor = tentacles[i].NextColor;
                    tentacles[i].NextColor++;
                    if (tentacles[i].NextColor >= colorArray_TasteTheRainbow.Length)
                        tentacles[i].NextColor = 0;
                }

                // Orientation change
                tentacles[i].Timer_Orientated += delta;
                if (tentacles[i].Timer_Orientated > totalTime_Orbit)
                {
                    tentacles[i].Timer_Orientated -= totalTime_Orbit;
                }

                // Movement
                if (mainTimer < timeTrigger_AllLeave)
                {
                    if (mainTimer > timeTriggers[i] && tentacles[i].Timer_Movement < totalTime_TentacleMove)
                    {
                        tentacles[i].Timer_Movement += delta;
                        if (tentacles[i].Timer_Movement > totalTime_TentacleMove)
                            tentacles[i].Timer_Movement = totalTime_TentacleMove;
                    }
                }
                else
                {
                    tentacles[i].Timer_Movement -= delta;
                    if (tentacles[i].Timer_Movement < 0)
                        tentacles[i].Timer_Movement = 0;
                }
            }

            if (mainTimer < timeTrigger_AllLeave)
            {
                if (mainTimer > timeTrigger_EyeBallMoveIn && timer_EyeMove < totalTime_TentacleMove)
                {
                    timer_EyeMove += delta;
                    if (timer_EyeMove > totalTime_TentacleMove)
                        timer_EyeMove = totalTime_TentacleMove;
                }
            }
            else
            {
                timer_EyeMove -= delta;
                if (timer_EyeMove < 0)
                    timer_EyeMove = 0;
            }

            if (mainTimer > timeTrigger_Talk1 && !shotText1)
            {
                shotText1 = teletype.AddEntry(entry1);
            }
            else if (mainTimer > timeTrigger_Talk2 && !shotText2)
            {
                shotText2 = teletype.AddEntry(entry2);
            }
        }
        #endregion
    }
}
