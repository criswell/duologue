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
    /*public enum MolochIntroState
    {
        InitialWait,
        MovingInTentacles,
        WiggleTentaclesWait,
        MovingInEyeBall,
        Speech,
        Intense,
        Exit,
    }*/

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

        private const string filename_EyePupil = "Enemies/gloop/king-gloop-eye";
        private const string filename_EyeShaftBlob = "Enemies/gloop/glooplet";
        private const string filename_EyeShaftHighlight = "Enemies/gloop/glooplet-highlight";
        private const float scale_Blob = 0.8f;
        private const float offsetY_BlobHighlight = -10f;
        private const float scale_BlobOutline = 1.05f;
        private const int numberOfBlobsInShaft = 10;
        private const float max_BlobSize = 2.1f;
        private const float min_BlobSize = 0.08f;
        private const float min_ShadowOffset = 5f;
        private const float max_ShadowOffset = 50f;

        /// <summary>
        /// The offset length of the eyeball from center of body
        /// </summary>
        private const float offsetLength = 350f;

        private const float maxOrbit_X = 80f;
        private const float maxOrbit_Y = 70f;
        private const float multiplierOrbit_X = 3f;
        private const float multiplierOrbit_Y = 2f;

        private const double totalTime_Orbit = 2.54;
        private const double totalTime_ColorChange = 1.51;

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

        private const double totalTime_TentacleMove = 2.15;
        #endregion

        #region Fields
        private EyeBallFrame[] eyes;
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
            int? hitPoints)
        {
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            Alive = true;
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
        #endregion

        #region Motion overrides
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
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < tentacles.Length; i++)
            {
                DrawTentacle(tentacles[i]);
            }
        }

        public override void Update(GameTime gameTime)
        {
            double delta = gameTime.ElapsedGameTime.TotalSeconds;
            mainTimer += delta;
            if (mainTimer > totalTime_StartToFinish)
            {
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
        }
        #endregion
    }
}
