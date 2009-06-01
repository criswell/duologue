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
    class Enemy_MolochPart : Enemy
    {
        #region Constants
        private const string filename_Glooplet = "Enemies/gloop/glooplet-death";

        private const double shieldCoolOffTime = 0.4;
        /// <summary>
        /// The point value I would be if I were hit at perfect beat
        /// </summary>
        private const int myPointValue = 100;

        private const int myShieldValue = 15;
        #endregion

        #region Fields
        private int parentIndex;
        private Enemy_Moloch myMaster;
        private Texture2D texture_Glooplet;

        private double shieldCoolOff;
        private AudioManager audio;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public Enemy_MolochPart() : base() { }

        public Enemy_MolochPart(GamePlayScreenManager manager)
            : base(manager)
        {
            // Yeah, we shouldn't be added by the WaveInit class
            throw new NotImplementedException();
        }

        public Enemy_MolochPart(GamePlayScreenManager manager,
            Enemy_Moloch master,
            int myIndex,
            float myRadius)
            : base(manager)
        {
            parentIndex = myIndex;
            MyType = TypesOfPlayObjects.Enemy_MolochPart;
            MajorType = MajorPlayObjectType.Enemy;
            RealSize = new Vector2(myRadius*2f, myRadius*2f);
            Radius = myRadius;
            Initialized = false;
            Alive = false;
            myMaster = master;
        }

        public override void Initialize(
            Vector2 startPos, 
            Vector2 startOrientation, 
            ColorState currentColorState, 
            ColorPolarity startColorPolarity, 
            int? hitPoints)
        {
            Position = startPos;
            Orientation = startOrientation;
            ColorState = currentColorState;
            ColorPolarity = startColorPolarity;
            if (hitPoints == null || (int)hitPoints == 0)
            {
                hitPoints = 1;
            }
            StartHitPoints = (int)hitPoints;
            CurrentHitPoints = (int)hitPoints;
            texture_Glooplet = InstanceManager.AssetManager.LoadTexture2D(filename_Glooplet);
            shieldCoolOff = 0;
            audio = ServiceLocator.GetService<AudioManager>();
            Initialized = true;
            Alive = true;
        }

        public override String[] GetTextureFilenames()
        {
            return new String[]
            {
                filename_Glooplet,
            };
        }
        #endregion

        #region Movement overrides
        public override bool StartOffset()
        {
            Position = myMaster.GetTubePosition(parentIndex);
            return true;
        }

        public override bool UpdateOffset(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.Player)
            {
                // Player
                Vector2 vToPlayer = this.Position - pobj.Position;
                float len = vToPlayer.Length();

                if (len < this.Radius + pobj.Radius)
                {
                    // We're on them, kill em
                    return pobj.TriggerHit(this);
                }
            }
            return true;
        }

        public override bool ApplyOffset()
        {
            // Nothing. We just follow the stuff from our master
            return true;
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            if (pobj.MajorType == MajorPlayObjectType.PlayerBullet &&
                shieldCoolOff >= shieldCoolOffTime)
            {
                CurrentHitPoints--;
                shieldCoolOff = 0;
                if (CurrentHitPoints <= 0)
                {
                    MyManager.TriggerPoints(
                        ((PlayerBullet)pobj).MyPlayerIndex,
                        myPointValue,
                        Position);
                    myMaster.TriggerTubeDeath(parentIndex);
                    Alive = false;
                    return false;
                }
                else
                {
                    TriggerShieldDisintegration(texture_Glooplet,
                        GetMyColor(ColorState.Light),
                        Position,
                        (float)MWMathHelper.GetRandomInRange(0, (double)MathHelper.TwoPi));
                    MyManager.TriggerPoints(
                        ((PlayerBullet)pobj).MyPlayerIndex,
                        myShieldValue,
                        Position);
                    //audio.soundEffects.PlayEffect(EffectID.CokeBottle);
                    audio.PlayEffect(EffectID.CokeBottle);
                }
            }
            return true;
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            // Nada
        }

        public override void Update(GameTime gameTime)
        {
            shieldCoolOff += gameTime.ElapsedGameTime.TotalSeconds;
            if (shieldCoolOff > shieldCoolOffTime)
                shieldCoolOff = shieldCoolOffTime;
        }
        #endregion
    }
}
