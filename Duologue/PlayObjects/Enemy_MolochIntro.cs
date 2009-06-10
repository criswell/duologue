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
        #endregion

        #region Fields
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
            throw new NotImplementedException();
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
