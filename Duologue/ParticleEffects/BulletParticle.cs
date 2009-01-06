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
    public class BulletParticle : ParticleSystem
    {
        public BulletParticle(Game game, int howManyEffects)
            : base(game, howManyEffects)
        {
        }

        protected override void InitializeConstants()
        {
            textureFilename = "shot-highlight-01";

            // high initial speed with lots of variance.  make the values closer
            // together to have more consistently circular explosions.
            minInitialSpeed = 40;
            maxInitialSpeed = 50;

            // doesn't matter what these values are set to, acceleration is tweaked in
            // the override of InitializeParticle.
            minAcceleration = 0;
            maxAcceleration = 0;

            // explosions should be relatively short lived
            minLifetime = .5f;
            maxLifetime = 1.0f;

            minScale = .3f;
            maxScale = 1.0f;

            minNumParticles = 20;
            maxNumParticles = 25;

            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

            // additive blending is very good at creating fiery effects.
            spriteBlendMode = RenderSpriteBlendMode.Addititive;

            //DrawOrder = AdditiveDrawOrder;
        }

        protected override void InitializeParticle(Particle p, Vector2 where, Color tint)
        {
            base.InitializeParticle(p, where, tint);

            p.Acceleration = -p.Velocity / p.Lifetime;
        }
    }
}
