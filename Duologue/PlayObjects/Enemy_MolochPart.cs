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
        #endregion

        #region Fields
        private int parentIndex;
        private Enemy_Moloch myMaster;
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
            Initialized = true;
            Alive = true;
        }

        public override string[] GetTextureFilenames()
        {
            throw new NotImplementedException();
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
