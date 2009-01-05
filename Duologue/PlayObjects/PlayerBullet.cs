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
using Duologue.State;
using Duologue.Audio;
#endregion

namespace Duologue.PlayObjects
{
    public class PlayerBullet : PlayObject
    {
        #region Constants
        private const string filename_ShotBase = "shot-base";
        private const string filename_ShotHighlight = "shot-highlight-0{0}"; // FIXME, will need to fix this if more are added

        /// <summary>
        /// The number of shot highlight frames
        /// </summary>
        private const int numberOfShotHighlights = 4;

        /// <summary>
        /// The rate of change for the alpha highlights
        /// </summary>
        private const byte delta_Alpha = 5;

        /// <summary>
        /// The rate of change for the bullet
        /// </summary>
        private const float delta_BulletSpeed = 10f;
        #endregion

        #region Fields
        private Texture2D shotBase;
        private Vector2 shotBaseCenter;
        private Texture2D[] shotHighlights;

        /// <summary>
        /// The alphas for each layer of the shot highlights
        /// </summary>
        private byte[] highlightAlphas;

        /// <summary>
        /// The positive/negative multipliers for the highlight alphas
        /// </summary>
        private int[] alphaMultipliers;

        /// <summary>
        /// My associated player
        /// </summary>
        private Player myPlayer;

        /// <summary>
        /// My associate player index
        /// </summary>
        private PlayerIndex myPlayerIndex;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public PlayerBullet()
            : base()
        {
        }

        /// <summary>
        /// Called once per game to initialize the bullet and associate with respective player
        /// </summary>
        public void Initialize(Player MyPlayer)
        {
            myPlayer = MyPlayer;
            myPlayerIndex = MyPlayer.MyPlayerIndex;
            Alive = false;
            if(!Initialized)
                LoadAndInitialize();
        }

        /// <summary>
        /// Called when we load and initialize the images
        /// </summary>
        private void LoadAndInitialize()
        {
            if (!Initialized)
            {
                shotBase = AssetManager.LoadTexture2D(filename_ShotBase);
                shotBaseCenter = new Vector2(
                    shotBase.Width / 2f,
                    shotBase.Height / 2f);

                shotHighlights = new Texture2D[numberOfShotHighlights];
                highlightAlphas = new byte[numberOfShotHighlights];
                alphaMultipliers = new int[numberOfShotHighlights];
                for (int i = 0; i < numberOfShotHighlights; i++)
                {
                    shotHighlights[i] = AssetManager.LoadTexture2D(String.Format(filename_ShotHighlight, i + 1));
                    highlightAlphas[i] = 0;
                    alphaMultipliers[i] = 1;
                }
                Initialized = true;
            }
        }
        #endregion

        #region Public Overrides
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
