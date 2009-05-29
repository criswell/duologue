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
    }

    public struct TubeGuy
    {
        public int CurrentFrame;
        public float Rotation;
        public Vector2 Offset;
        public bool Alive;
        public double Timer;
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
        private const float offset_TubeVerticalCenter = -10f;

        private const string filename_EyePupil = "Enemies/gloop/king-gloop-eye";

        private const float delta_BodyRotation = MathHelper.PiOver4 * 0.4f;

        private const float minAlpha = 0.1f;
        private const float maxAlpha = 1.0f;
        private const double minDeltaAlpha = 0.005;
        private const double maxDeltaAlpha = 0.04;

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 75;
        #endregion

        #region Fields
        // Image and animation related things
        private MolochBodyElement[] body;
        private EyeBallFrame[] eyes;
        private TubeFrame[] tubes;
        private Texture2D texture_Spinner;
        private Texture2D texture_EyePupil;
        private Vector2 center_Body;
        private Vector2 center_Eye;
        private Vector2 center_Spinner;
        private Vector2 center_Tube;
        private Vector2 center_Pupil;

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
            // We say "fuck the requested starting pos"

            // FIXME set position

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

        private void LoadAndInitialize()
        {
            body = new MolochBodyElement[frames_Body];
            eyes = new EyeBallFrame[frames_Eye];
            tubes = new TubeFrame[frames_Tube];

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
            }
            center_Body = new Vector2(
                body[0].Texture.Width / 2f, body[0].Texture.Height / 2f);

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
                tubes[i].Base = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_TubeBase, i.ToString()));
                tubes[i].Outline = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_TubeOutline, i.ToString()));
                tubes[i].Lower = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_TubeShadeLower, i.ToString()));
                tubes[i].Upper = InstanceManager.AssetManager.LoadTexture2D(String.Format(filename_TubeShadeUpper, i.ToString()));
            }
            center_Tube = new Vector2(
                tubes[0].Base.Width / 2f, (float)tubes[0].Base.Height + offset_TubeVerticalCenter);


            Alive = true;
            Initialized = true;
        }

        public override String[] GetTextureFilenames()
        {
            String[] filenames = new String[frames_Body + 5 * frames_Eye + 4 * frames_Tube + 2];

            int i = 0;
            filenames[i] = filename_EyePupil;
            i++;
            filenames[i] = filename_Spinner;
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
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
