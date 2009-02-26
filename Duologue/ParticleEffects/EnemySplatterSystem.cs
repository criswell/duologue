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
    public class EnemySplatterSystem : ParticleSystem
    {
        public EnemySplatterSystem(Game game, int howManyEffects)
            : base(game, howManyEffects)
        {
        }

        protected override void InitializeConstants()
        {
            textureFilename = "Enemies/splat-particle";

            // high initial speed with lots of variance.  make the values closer
            // together to have more consistently circular explosions.
            minInitialSpeed = 10;
            maxInitialSpeed = 25;

            minAcceleration = 1;
            maxAcceleration = 10;

            // explosions should be relatively short lived
            minLifetime = .5f;
            maxLifetime = 1.0f;

            minScale = 0.25f;
            maxScale = 2.0f;

            minNumParticles = 10;
            maxNumParticles = 15;

            minRotationSpeed = MathHelper.PiOver4 / 4f; ;
            maxRotationSpeed = MathHelper.PiOver4 / 2f ;

            // additive blending is very good at creating fiery effects.
            spriteBlendMode = RenderSpriteBlendMode.AlphaBlend;

            //DrawOrder = AdditiveDrawOrder;
        }

        /*protected override void InitializeParticle(Particle p, Vector2 where, Color tint)
        {
            base.InitializeParticle(p, where, tint);

            p.Acceleration = -p.Velocity / p.Lifetime;
        }*/
    }
}
