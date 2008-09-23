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
    /// <summary>
    /// Steam for when the enemies get hit by light.
    /// </summary>
    public class Steam : ParticleSystem
    {
        public Steam(Game game, int howManyEffects)
            : base(game, howManyEffects)
        {
        }

        /// <summary>
        /// Set up the constants
        /// </summary>
        protected override void InitializeConstants()
        {
            textureFilename = "steam";

            minInitialSpeed = 20;
            maxInitialSpeed = 100;

            // No accelleration, just rises
            minAcceleration = 0;
            maxAcceleration = 0;

            // long lifetime, this can be changed to create thinner or thicker smoke.
            // tweak minNumParticles and maxNumParticles to complement the effect.
            minLifetime = 1.0f;
            maxLifetime = 2.0f;

            minScale = .5f;
            maxScale = 1.0f;

            minNumParticles = 7;
            maxNumParticles = 15;

            // rotate slowly, we want a fairly relaxed effect
            minRotationSpeed = -MathHelper.PiOver4 / 2.0f;
            maxRotationSpeed = MathHelper.PiOver4 / 2.0f;

            spriteBlendMode = SpriteBlendMode.AlphaBlend;

//            DrawOrder = AlphaBlendDrawOrder;
        }

        /// <summary>
        /// PickRandomDirection is overriden so that we can make the particles always 
        /// move have an initial velocity pointing up.
        /// </summary>
        /// <returns>a random direction which points basically up.</returns>
        protected override Vector2 PickRandomDirection()
        {
            // Point the particles somewhere between 80 and 100 degrees.
            // tweak this to make the smoke have more or less spread.
            float radians = (float)MWMathHelper.GetRandomInRange(
                MathHelper.ToRadians(80), MathHelper.ToRadians(100));

            Vector2 direction = Vector2.Zero;
            // from the unit circle, cosine is the x coordinate and sine is the
            // y coordinate. We're negating y because on the screen increasing y moves
            // down the monitor.
            direction.X = (float)Math.Cos(radians);
            direction.Y = -(float)Math.Sin(radians);
            return direction;
        }

        /// <summary>
        /// InitializeParticle is overridden to add the appearance of wind.
        /// </summary>
        /// <param name="p">the particle to set up</param>
        /// <param name="where">where the particle should be placed</param>
        protected override void InitializeParticle(Particle p, Vector2 where, Color tint)
        {
            base.InitializeParticle(p, where, tint);

            // the base is mostly good, but we want to simulate a little bit of wind
            // heading to the right.
            p.Acceleration.X += MWMathHelper.GetRandomInRange(10, 50);
        }
    }
}
