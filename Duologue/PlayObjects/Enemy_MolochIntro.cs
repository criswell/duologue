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
    public enum MolochIntroState
    {
        InitialWait,
        MovingInTentacles,
        WiggleTentaclesWait,
        MovingInEyeBall,
        Speech,
        Intense,
        Exit,
    }

    public struct Tentacle
    {
        public Vector2 Position;
        public Vector2 Orientation;
        public double Timer_Orientated; // EAT IT, DOCTORCAL
        public double Timer_ColorChange;
        public Color LastColor;
        public Color NextColor;
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
        private const float scale_BlobOutline = 0.82f;
        private const int numberOfBlobsInShaft = 10;

        //private const double totalTime_InitialWait = 

        private const float maxOrbit_X = 80f;
        private const float maxOrbit_Y = 70f;
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

        private Tentacle[] tentacles;

        private Vector2 centerOfScreen;
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
            LoadAndInitialize();
        }

        private void LoadAndInitialize()
        {
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
        public Vector2 AimAtWiggle(float multiplier_X, float multiplier_Y, Vector2 curPos)
        {
            // Lissajous curve for what he's looking at
            Vector2 localOffset = new Vector2(
                maxOrbit_X * (float)Math.Sin(multiplier_X * MathHelper.Lerp(0, MathHelper.TwoPi, (float)(timer_EyeStare / totalTime_EyeStareOrbit))),
                maxOrbit_Y * (float)Math.Sin(multiplier_Y * MathHelper.Lerp(0, MathHelper.TwoPi, (float)(timer_EyeStare / totalTime_EyeStareOrbit))));
            // Place eye ball with relation to center
            localOffset = centerOfScreen + localOffset - curPos;
            localOffset.Normalize();
            return localOffset;
        }
        #endregion

        #region Motion overrides
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

        #region Draw / Update
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
