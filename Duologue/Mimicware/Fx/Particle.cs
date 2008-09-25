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
#endregion

namespace Mimicware.Fx
{
    /// <summary>
    /// The basic Mimicware particle unit. Many of these individual units will be combined to make
    /// a larger particle effect.
    /// </summary>
    public class Particle : DrawableObject
    {
        #region Constants
        #endregion

        #region Fields
        #endregion

        #region Properties
        /// <summary>
        /// Velocity of the particle
        /// </summary>
        public Vector2 Velocity;
        /// <summary>
        /// Acceleration of the particle
        /// </summary>
        public Vector2 Acceleration;
        /// <summary>
        /// The total lifetime of the particle
        /// </summary>
        public float Lifetime;
        /// <summary>
        /// The time since this object was initialized
        /// </summary>
        public float TimeSinceStart;
        /// <summary>
        /// The speed of rotation of this particle
        /// </summary>
        public float RotationSpeed;
        /// <summary>
        /// Override the base Alive
        /// </summary>
        public override bool Alive
        {
            get { return TimeSinceStart < Lifetime; }
        }
        public float Rotation;
        public float Scale;
        #endregion

        #region Constructor / Init
        public Particle(
            Texture2D texture2D,
            Vector2 texturePosition,
            Vector2 textureCenter,
            Vector2 acceleration,
            float lifetime,
            Color textureTint,
            float rotationSpeed,
            float textureScale,
            float textureLayer) : 
            base(
            texturePosition,
            textureCenter,
            textureTint)
        {
            //Text = null;
            this.Acceleration = acceleration;
            this.Lifetime = lifetime;
            this.RotationSpeed = rotationSpeed;
            this.TimeSinceStart = 0.0f;
            this.Rotation = (float)MWMathHelper.GetRandomInRange(0, MathHelper.TwoPi);
        }
        #endregion

        #region Update
        /// <summary>
        /// Update is to be called once per frame by the particle system
        /// </summary>
        /// <param name="dt">Delta time</param>
        public virtual void  Update(float dt)
        {
            Velocity += Acceleration * dt;
            Position += Velocity * dt;
            Rotation += RotationSpeed * dt;
            TimeSinceStart += dt;
        }
        #endregion
    }
}
