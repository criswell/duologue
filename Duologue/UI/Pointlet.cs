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
using Mimicware.Manager;
using Mimicware.Graphics;
// Duologue
using Duologue;
//using Duologue.State;
//using Duologue.PlayObjects;
#endregion


namespace Duologue.UI
{
    public class Pointlet : DrawableObject
    {
        #region Constants
        private const float assumedFPS = 60f;
        #endregion

        #region Fields
        private int points;
        private Rectangle finalRect;
        private float lifetime;
        private Vector2 velocity;
        private float timer;
        #endregion

        #region Properties
        /// <summary>
        /// Used to get the current point value of this pointlet
        /// </summary>
        public int Points
        {
            get { return points; }
        }

        /// <summary>
        /// Determines if the pointlet is alive or not
        /// </summary>
        public override bool Alive
        {
            get
            {
                if (finalRect.Contains((Int32)Position.X, (Int32)Position.Y) ||
                    timer > lifetime)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Used to get the expected lifetime of this current pointlet
        /// </summary>
        public float Lifetime
        {
            get { return lifetime; }
        }

        /// <summary>
        /// Gets or sets the final rectangle
        /// </summary>
        public Rectangle FinalRect
        {
            get { return finalRect; }
            set { finalRect = value; }
        }
        #endregion

        #region Constructor / Init
        /// <summary>
        /// Default, empty constructor. This sets the pointlet at dead (not alive).
        /// </summary>
        /// <param name="pos">The position</param>
        /// <param name="tint">The tint</param>
        public Pointlet(Vector2 pos, Color tint)
            : base(pos, Vector2.Zero, tint)
        {
            finalRect = new Rectangle((int)pos.X, (int)pos.Y, 1,1);
            lifetime = 0f;
            velocity = Vector2.Zero;
        }

        /// <summary>
        /// Construct a new pointlet based on initial values
        /// </summary>
        /// <param name="position">The position of the pointlet at start</param>
        /// <param name="tint">The tint of the pointlet</param>
        /// <param name="pointValue">The point value for the pointlet</param>
        /// <param name="destRectangle">The destination rectangle for the pointlet</param>
        /// <param name="lifeTime">How long it should take the pointlet to reach the destination in seconds (assuming 60 FPS)</param>
        public Pointlet(
            Vector2 position,
            Color tint,
            int pointValue,
            Rectangle destRectangle,
            float lifeTime)
            : base(position, Vector2.Zero, tint)
        {
            Initialize(position, tint, pointValue, destRectangle, lifeTime);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Called when you need to compute the velocity
        /// </summary>
        private void ComputeVelocity()
        {
            // Aim at the rectangle first
            Vector2 vector2rect = new Vector2((float)finalRect.X + finalRect.Width/2f, (float)finalRect.Y + finalRect.Height/2f) - Position;
            float distanceToRect = vector2rect.Length();
            vector2rect.Normalize();

            // Now, figure out how much we need to move per frame to get there in lifetime
            float deltaFrames = lifetime * assumedFPS;
            velocity = distanceToRect / deltaFrames * vector2rect;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize (or re-initialize) a pointlet
        /// </summary>
        /// <param name="position">The position of the pointlet at start</param>
        /// <param name="tint">The tint of the pointlet</param>
        /// <param name="pointValue">The point value for the pointlet</param>
        /// <param name="destRectangle">The destination rectangle for the pointlet</param>
        /// <param name="lifeTime">How long it should take the pointlet to reach the destination in seconds (assuming 60 FPS)</param>
        public void Initialize(Vector2 position, Color tint, int pointValue, Rectangle destRectangle, float lifeTime)
        {
            Position = position;
            Tint = tint;
            points = pointValue;
            finalRect = destRectangle;
            lifetime = lifeTime;
            timer = 0;

            ComputeVelocity();
        }
        #endregion

        #region Update
        /// <summary>
        /// Called once per frame to update the pointlet
        /// </summary>
        /// <param name="dt">Delta time</param>
        public void Update(float dt)
        {
            timer += dt;
            Position += velocity;
        }
        #endregion
    }
}
