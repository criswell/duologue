﻿#region File Description
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

        private const double minTimeBetweenParticles = 0.05;
        private const double maxtimeBetweenParticles = 0.2;

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

        /// <summary>
        /// The draw layer depth
        /// </summary>
        private const float layerDepth = 1.0f;
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

        /// <summary>
        /// The orientation (or aim) of the bullet
        /// </summary>
        private Vector2 orientation;

        /// <summary>
        /// My current color state
        /// </summary>
        private ColorState currentColorState;

        /// <summary>
        /// True if our color state is positive
        /// </summary>
        private bool isPositive;

        private Color baseColor;
        private Color highlightColor;

        private float timeSinceStart;
        private float currentHighlightLimit;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public PlayerBullet()
            : base()
        {
            MyType = TypesOfPlayObjects.PlayerBullet;
            MajorType = MajorPlayObjectType.PlayerBullet;
            Initialized = false;
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
                Radius = shotBaseCenter.X;
                if (shotBaseCenter.Y > Radius)
                    Radius = shotBaseCenter.Y;

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
            // For now, this does nothing (might always do nothing)
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Enemy)
            {
                Vector2 vToEnemy = this.Position - pobj.Position;
                float len = vToEnemy.Length();
                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, trigger a hit?
                    Color c = ((Enemy)pobj).ColorState.Positive[ColorState.Light];
                    if(((Enemy)pobj).ColorPolarity == ColorPolarity.Negative)
                        c = ((Enemy)pobj).ColorState.Negative[ColorState.Light];
                    if (!currentColorState.SameColor(baseColor, c))
                    {
                        return TriggerHit(pobj);
                    }
                    else
                    {
                        // We just get absorbed
                        Alive = false;
                    }
                    return true;
                }
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            // For now, this does nothing (might always do nothing)
            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Enemy)
            {
                Alive = false;
                pobj.TriggerHit(this);
            }
            return Alive;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Called when this bullet has been fired
        /// </summary>
        /// <param name="Aim">The direction we're firing in (doesn't need to be normalized)</param>
        /// <param name="StartPos">The starting position</param>
        public void Fire(Vector2 Aim, Vector2 StartPos)
        {
            Position = StartPos;
            orientation = Aim;
            orientation.Normalize();
            Alive = true;
            timeSinceStart = 0f;
            currentHighlightLimit = (float)MWMathHelper.GetRandomInRange(minTimeBetweenParticles, maxtimeBetweenParticles);
            for (int i = 0; i < numberOfShotHighlights; i++)
            {
                highlightAlphas[i] = (byte)InstanceManager.Random.Next(255);
                if (InstanceManager.Random.Next(2) == 0)
                    alphaMultipliers[i] = -1;
                else
                    alphaMultipliers[i] = 1;
            }

            // Get the color stuff
            currentColorState = myPlayer.ColorState;
            isPositive = myPlayer.LightIsNegative;
            if (isPositive)
            {
                baseColor = currentColorState.Positive[ColorState.Light];
                highlightColor = currentColorState.Positive[ColorState.Dark];
            }
            else
            {
                baseColor = currentColorState.Negative[ColorState.Light];
                highlightColor = currentColorState.Negative[ColorState.Dark];
            }
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            RenderSprite.Draw(
                shotBase,
                Position,
                shotBaseCenter,
                null,
                baseColor,
                0f,
                1f,
                layerDepth,
                RenderSpriteBlendMode.Addititive);

            Color c;
            for (int i = 0; i < numberOfShotHighlights; i++)
            {
                c = new Color(highlightColor, highlightAlphas[i]);
                RenderSprite.Draw(
                    shotHighlights[i],
                    Position,
                    shotBaseCenter,
                    null,
                    c,
                    0f,
                    1f,
                    layerDepth,
                    RenderSpriteBlendMode.Addititive);
            }
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceStart > currentHighlightLimit)
            {
                LocalInstanceManager.BulletParticle.AddParticles(Position, highlightColor);
                currentHighlightLimit = (float)MWMathHelper.GetRandomInRange(minTimeBetweenParticles, maxtimeBetweenParticles);
                timeSinceStart = 0f;
            }

            // Update the position
            Position += delta_BulletSpeed * orientation;

            // If we're offscreen, we're gone
            if (Position.X < 0 - Radius || Position.X > InstanceManager.DefaultViewport.Width + Radius ||
                Position.Y < 0 - Radius || Position.Y > InstanceManager.DefaultViewport.Height + Radius)
                Alive = false;

            if (Alive)
            {
                for (int i = 0; i < numberOfShotHighlights; i++)
                {
                    if (highlightAlphas[i] > 254 - delta_Alpha)
                    {
                        alphaMultipliers[i] = -1;
                    }
                    else if (highlightAlphas[i] < 1 + delta_Alpha)
                    {
                        alphaMultipliers[i] = 1;
                    }
                    highlightAlphas[i] = (byte)(highlightAlphas[i] + delta_Alpha * alphaMultipliers[i]);
                }
            }
        }
        #endregion
    }
}