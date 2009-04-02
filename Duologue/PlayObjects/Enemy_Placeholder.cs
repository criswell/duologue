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
    class Enemy_Placeholder : Enemy
    {
        public Enemy_Placeholder() { }

        public Enemy_Placeholder(GamePlayScreenManager manager)
            : base(manager)
        {
            MyType = TypesOfPlayObjects.Enemy_Placeholder;
            MajorType = MajorPlayObjectType.Enemy;
            Initialized = false;

            // Set the RealSize by hand
            RealSize = Vector2.Zero;
            Radius = 0f;
            Initialized = false;
            Alive = false;
        }

        public override void Initialize(
            Vector2 startPos, 
            Vector2 startOrientation, 
            ColorState currentColorState, 
            ColorPolarity startColorPolarity,
            int? hitPoints)
        {
            // I'm just a placeholder, so I'm never alive
            Alive = false;
        }

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

        public override String[] GetFilenames()
        {
            return new String[] { };
        }

        public override bool TriggerHit(PlayObject pobj)
        {
            return true;
        }

        public override void Draw(GameTime gameTime)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
