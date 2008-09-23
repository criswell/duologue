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
    /// Basic Mimicware.Fx particle system class.
    /// </summary>
    public abstract class ParticleSystem : DrawableGameComponent
    {
        #region Constants
        #endregion

        #region Fields
        /// <summary>
        /// The localGame instance
        /// </summary>
        private Game localGame;
        /// <summary>
        /// How many effects this particle system is expected to do. Will be set in the constructor.
        /// </summary>
        private int maxNumEffects;
        /// <summary>
        /// The array of particles this system will use.
        /// </summary>
        private Particle[] particles;

        /// <summary>
        /// The queue of free particles in the system.
        /// </summary>
        private Queue<Particle> freeParticles;

        /// <summary>
        /// The texture for this particle system.
        /// </summary>
        private Texture2D texture;
        /// <summary>
        /// The center of the texture
        /// </summary>
        private Vector2 center;
        #endregion

        #region Properties
        /// <summary>
        /// The game-wide asset manager.
        /// </summary>
        public AssetManager AssetManager;

        /// <summary>
        /// The game-wide render sprite instance.
        /// </summary>
        public RenderSprite RenderSprite;

        /// <summary>
        /// Returns the number of effects this particle system is expected to do. Read-only, this is set in the
        /// constructor.
        /// </summary>
        public int MaxNumEffects
        {
            get { return maxNumEffects; }
        }

        /// <summary>
        /// Returns the number of free particles.
        /// </summary>
        public int FreeParticleCount
        {
            get { return freeParticles.Count; }
        }

        #endregion

        // The following is copied (rather egregiously) from the MS-PL licensed particle system
        // sample available from Microsoft. We can use it here, we just need to release the segment
        // of code in question
        #region Items to be overriden by subclass.
        /// <summary>
        /// minNumParticles and maxNumParticles control the number of particles that are
        /// added when AddParticles is called. The number of particles will be a random
        /// number between minNumParticles and maxNumParticles.
        /// </summary>
        protected int minNumParticles;
        protected int maxNumParticles;

        /// <summary>
        /// this controls the texture that the particle system uses. It will be used as
        /// an argument to ContentManager.Load.
        /// </summary>
        protected string textureFilename;

        /// <summary>
        /// minInitialSpeed and maxInitialSpeed are used to control the initial velocity
        /// of the particles. The particle's initial speed will be a random number 
        /// between these two. The direction is determined by the function 
        /// PickRandomDirection, which can be overriden.
        /// </summary>
        protected float minInitialSpeed;
        protected float maxInitialSpeed;

        /// <summary>
        /// minAcceleration and maxAcceleration are used to control the acceleration of
        /// the particles. The particle's acceleration will be a random number between
        /// these two. By default, the direction of acceleration is the same as the
        /// direction of the initial velocity.
        /// </summary>
        protected float minAcceleration;
        protected float maxAcceleration;

        /// <summary>
        /// minRotationSpeed and maxRotationSpeed control the particles' angular
        /// velocity: the speed at which particles will rotate. Each particle's rotation
        /// speed will be a random number between minRotationSpeed and maxRotationSpeed.
        /// Use smaller numbers to make particle systems look calm and wispy, and large 
        /// numbers for more violent effects.
        /// </summary>
        protected float minRotationSpeed;
        protected float maxRotationSpeed;

        /// <summary>
        /// minLifetime and maxLifetime are used to control the lifetime. Each
        /// particle's lifetime will be a random number between these two. Lifetime
        /// is used to determine how long a particle "lasts." Also, in the base
        /// implementation of Draw, lifetime is also used to calculate alpha and scale
        /// values to avoid particles suddenly "popping" into view
        /// </summary>
        protected float minLifetime;
        protected float maxLifetime;

        /// <summary>
        /// to get some additional variance in the appearance of the particles, we give
        /// them all random scales. the scale is a value between minScale and maxScale,
        /// and is additionally affected by the particle's lifetime to avoid particles
        /// "popping" into view.
        /// </summary>
        protected float minScale;
        protected float maxScale;

        /// <summary>
        /// different effects can use different blend modes. fire and explosions work
        /// well with additive blending, for example.
        /// </summary>
        protected SpriteBlendMode spriteBlendMode;
        #endregion

        #region Constructor / Init / Load
        /// <summary>
        /// Constructs a new ParticleSystem.
        /// </summary>
        /// <param name="game">The host for this particle system. The game keeps the 
        /// content manager and sprite batch for us.</param>
        /// <param name="howManyEffects">the maximum number of particle effects that
        /// are expected on screen at once.</param>
        /// <remarks>it is tempting to set the value of howManyEffects very high.
        /// However, this value should be set to the minimum possible, because
        /// it has a large impact on the amount of memory required, and slows down the
        /// Update and Draw functions.</remarks>
        protected ParticleSystem(Game game, int maxNumEffects)
            : base(game)
        {
            this.localGame = game;
            this.maxNumEffects = maxNumEffects;
        }

        /// <summary>
        /// override the base class's Initialize to do some additional work; we want to
        /// call InitializeConstants to let subclasses set the constants that we'll use.
        /// 
        /// also, the particle array and freeParticles queue are set up here.
        /// </summary>
        public override void Initialize()
        {
            InitializeConstants();

            // calculate the total number of particles we will ever need, using the
            // max number of effects and the max number of particles per effect.
            // once these particles are allocated, they will be reused, so that
            // we don't put any pressure on the garbage collector.
            particles = new Particle[maxNumEffects * maxNumParticles];
            freeParticles = new Queue<Particle>(maxNumEffects * maxNumParticles);
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new Particle(
                    null,
                    Vector2.Zero,
                    Vector2.Zero,
                    Vector2.Zero,
                    0f,
                    Color.White,
                    0f,
                    0f,
                    0f);
                freeParticles.Enqueue(particles[i]);
            }
            base.Initialize();
        }

        /// <summary>
        /// this abstract function must be overriden by subclasses of ParticleSystem.
        /// It's here that they should set all the constants marked in the region
        /// "constants to be set by subclasses", which give each ParticleSystem its
        /// specific flavor.
        /// </summary>
        protected abstract void InitializeConstants();

        /// <summary>
        /// Override the base class LoadContent to load the texture. once it's
        /// loaded, calculate the origin.
        /// </summary>
        protected override void LoadContent()
        {
            if (AssetManager == null)
                AssetManager = InstanceManager.AssetManager;
            // make sure sub classes properly set textureFilename.
            if (string.IsNullOrEmpty(textureFilename))
            {
                string message = "textureFilename wasn't set properly, so the " +
                    "particle system doesn't know what texture to load. Make " +
                    "sure your particle system's InitializeConstants function " +
                    "properly sets textureFilename.";
                throw new InvalidOperationException(message);
            }

            if (this.AssetManager == null)
            {
                string message = "The AssetManager doesn't appear to be set. " +
                    "Please note that it must be set *before* the LoadContent() " +
                    "method is called.";
                throw new InvalidOperationException(message);
            }
            // load the texture....
            texture = AssetManager.LoadTexture2D(textureFilename);

            // ... and calculate the center. this'll be used in the draw call, we
            // always want to rotate and scale around this point.
            center.X = texture.Width / 2;
            center.Y = texture.Height / 2;

            base.LoadContent();
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// AddParticles's job is to add an effect somewhere on the screen. If there 
        /// aren't enough particles in the freeParticles queue, it will use as many as 
        /// it can. This means that if there not enough particles available, calling
        /// AddParticles will have no effect.
        /// </summary>
        /// <param name="where">where the particle effect should be created</param>
        public void AddParticles(Vector2 where, Color tint)
        {
            // the number of particles we want for this effect is a random number
            // somewhere between the two constants specified by the subclasses.
            int numParticles = MWMathHelper.GetRandomInRange(minNumParticles, maxNumParticles);

            // create that many particles, if you can.
            for (int i = 0; i < numParticles && freeParticles.Count > 0; i++)
            {
                // grab a particle from the freeParticles queue, and Initialize it.
                Particle p = freeParticles.Dequeue();
                InitializeParticle(p, where, tint);
            }
        }

        /// <summary>
        /// InitializeParticle randomizes some properties for a particle, then
        /// calls initialize on it. It can be overriden by subclasses if they 
        /// want to modify the way particles are created. For example, 
        /// SmokePlumeParticleSystem overrides this function make all particles
        /// accelerate to the right, simulating wind.
        /// </summary>
        /// <param name="p">the particle to initialize</param>
        /// <param name="where">the position on the screen that the particle should be
        /// </param>
        /// <param name="tint">The color tint for the particle</param>
        protected virtual void InitializeParticle(Particle p, Vector2 where, Color tint)
        {
            // first, call PickRandomDirection to figure out which way the particle
            // will be moving. velocity and acceleration's values will come from this.
            Vector2 direction = PickRandomDirection();

            // pick some random values for our particle
            float velocity = (float)
                MWMathHelper.GetRandomInRange(minInitialSpeed, maxInitialSpeed);
            float acceleration = (float)
                MWMathHelper.GetRandomInRange(minAcceleration, maxAcceleration);
            float lifetime = (float)
                MWMathHelper.GetRandomInRange(minLifetime, maxLifetime);
            float scale = (float)
                MWMathHelper.GetRandomInRange(minScale, maxScale);
            float rotationSpeed = (float)
                MWMathHelper.GetRandomInRange(minRotationSpeed, maxRotationSpeed);

            // then initialize it with those random values. initialize will save those,
            // and make sure it is marked as active.
            //Vector2 position, Vector2 velocity, Vector2 acceleration,
            //float lifetime, float scale, float rotationSpeed)
            /*p.Initialize(
                where, velocity * direction, acceleration * direction,
                lifetime, scale, rotationSpeed);*/

            p.Position = where;
            p.Velocity = velocity * direction;
            p.Acceleration = acceleration * direction;
            p.Lifetime = lifetime;
            p.Scale = scale;
            p.RotationSpeed = rotationSpeed;
            p.Tint = tint;
        }

        /// <summary>
        /// PickRandomDirection is used by InitializeParticles to decide which direction
        /// particles will move. The default implementation is a random vector in a
        /// circular pattern.
        /// </summary>
        protected virtual Vector2 PickRandomDirection()
        {
            float angle = (float)MWMathHelper.GetRandomInRange(0, MathHelper.TwoPi);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        #endregion

        #region Update / Draw
        /// <summary>
        /// overriden from DrawableGameComponent, Update will update all of the active
        /// particles.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // calculate dt, the change in the since the last frame. the particle
            // updates will use this value.
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // go through all of the particles...
            foreach (Particle p in particles)
            {
                
                if (p.Alive)
                {
                    // ... and if they're active, update them.
                    p.Update(dt);
                    // if that update finishes them, put them onto the free particles
                    // queue.
                    InstanceManager.Logger.LogEntry("p.alive: " + p.TimeSinceStart + " < " + p.Lifetime);
                    if (!p.Alive)
                    {
                        InstanceManager.Logger.LogEntry("ENQUEUEUEUEUUE :" + FreeParticleCount);
                        freeParticles.Enqueue(p);
                    }
                }   
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// overriden from DrawableGameComponent, Draw will use ParticleSampleGame's 
        /// sprite batch to render all of the active particles.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // tell sprite batch to begin, using the spriteBlendMode specified in
            // initializeConstants
            //game.SpriteBatch.Begin(spriteBlendMode);

            if (RenderSprite == null)
                RenderSprite = InstanceManager.RenderSprite;

            foreach (Particle p in particles)
            {
                // skip inactive particles
                if (!p.Alive)
                    continue;

                // normalized lifetime is a value from 0 to 1 and represents how far
                // a particle is through its life. 0 means it just started, .5 is half
                // way through, and 1.0 means it's just about to be finished.
                // this value will be used to calculate alpha and scale, to avoid 
                // having particles suddenly appear or disappear.
                float normalizedLifetime = p.TimeSinceStart / p.Lifetime;

                // we want particles to fade in and fade out, so we'll calculate alpha
                // to be (normalizedLifetime) * (1-normalizedLifetime). this way, when
                // normalizedLifetime is 0 or 1, alpha is 0. the maximum value is at
                // normalizedLifetime = .5, and is
                // (normalizedLifetime) * (1-normalizedLifetime)
                // (.5)                 * (1-.5)
                // .25
                // since we want the maximum alpha to be 1, not .25, we'll scale the 
                // entire equation by 4.
                float alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);
                Color color = new Color(new Vector4((float)p.Tint.R, (float)p.Tint.G, (float)p.Tint.B, alpha));

                // make particles grow as they age. they'll start at 75% of their size,
                // and increase to 100% once they're finished.
                float scale = p.Scale * (.75f + .25f * normalizedLifetime);

                //localGame.SpriteBatch.Draw(texture, p.Position, null, color,
                //    p.Rotation, center, scale, SpriteEffects.None, 0.0f);
                //InstanceManager.Logger.LogEntry(p.TimeSinceStart + " < " + p.Lifetime);
                RenderSprite.Draw(texture, p.Position, center, null, color, p.Rotation,
                    scale, 0.0f, (spriteBlendMode == SpriteBlendMode.Additive));
            }

            //game.SpriteBatch.End();

            base.Draw(gameTime);
            //InstanceManager.Logger.LogEntry("Free particles: " + FreeParticleCount);
        }
        #endregion
    }
}
