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
    public class Enemy_Moloch : Enemy
    {
        #region Constants
        private string filename_Body = "Enemies/end/end-boss-body-{0}";
        private int frames_Body = 3;
        
        private string filename_EyeBall = "Enemies/end/end-boss-eye{0}-eyeball";
        private string filename_EyeOutline = "Enemies/end/end-boss-eye{0}-outline";
        private string filename_EyeShadeLower = "Enemies/end/end-boss-eye{0}-shade-lower";
        private string filename_EyeShadeMiddle = "Enemies/end/end-boss-eye{0}-shade-middle";
        private string filename_EyeShadeUpper = "Enemies/end/end-boss-eye{0}-shade-upper";
        private int frames_Eye = 5;

        private string filename_Spinner = "Enemies/end/end-boss-spinner";

        private string filename_TubeBase = "Enemies/end/end-boss-tube{0}-base";
        private string filename_TubeOutline = "Enemies/end/end-boss-tube{0}-outline";
        private string filename_TubeShadeLower = "Enemies/end/end-boss-tube{0}-shade-lower";
        private string filename_TubeShadeUpper = "Enemies/end/end-boss-tube{0}-shade-upper";
        private int frames_Tube = 4;

        private const string filename_EyePupil = "Enemies/gloop/king-gloop-eye";

        /// <summary>
        /// This is both the minimum number of hit points it is possible for this boss to have
        /// as well as the step-size for each additional hitpoint requested.
        /// E.g., if you request this boss have "2" HP, then he will *really* get "2 x realHitPointMultiplier" HP
        /// </summary>
        private const int realHitPointMultiplier = 75;
        #endregion

        #region Fields
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
            RealSize = new Vector2(700, 700);
        }

        public override void Initialize(
            Vector2 startPos, 
            Vector2 startOrientation, 
            ColorState currentColorState, 
            ColorPolarity startColorPolarity, 
            int? hitPoints)
        {
            // We say "fuck the requested starting pos"
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
            throw new NotImplementedException();
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
