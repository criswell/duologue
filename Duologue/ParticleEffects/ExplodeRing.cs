#region File Description
#endregion

#region Using statements
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
using Mimicware.Debug;
using Mimicware.Fx;
#endregion

namespace Duologue.ParticleEffects
{
    public class ExplodeRing : Particle
    {
        #region Properties
        public float ScaleSpeed;
        #endregion

        #region Constructor
        public ExplodeRing(
            Texture2D texture2D,
            Vector2 texturePosition,
            Vector2 textureCenter,
            Vector2 acceleration,
            float lifetime,
            Color textureTint,
            float rotationSpeed,
            float textureScale,
            float textureLayer)
            :
            base(
            texture2D,
            texturePosition,
            textureCenter,
            acceleration,
            lifetime,
            textureTint,
            rotationSpeed,
            textureScale,
            textureLayer)
        {
            ScaleSpeed = 0.5f;
        }
        #endregion

        #region Update
        /// <summary>
        /// Update is to be called once per frame by the particle system
        /// </summary>
        /// <param name="dt">Delta time</param>
        public override void Update(float dt)
        {
            base.Update(dt);
            this.Scale += ScaleSpeed;
        }
        #endregion
    }

}
