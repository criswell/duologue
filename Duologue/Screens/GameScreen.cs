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
using Duologue.Properties;
#endregion

namespace Duologue.Screens
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public abstract class GameScreen : Microsoft.Xna.Framework.GameComponent
    {
        #region Fields
        /// <summary>
        /// The components I am responsible for
        /// </summary>
        protected List<DrawableGameComponent> MyComponents;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public GameScreen(Game game)
            : base(game)
        {
            MyComponents = new List<DrawableGameComponent>();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            InitializeConstants();

            base.Initialize();
        }
        #endregion

        #region Abstract  & protected methods
        protected abstract void InitializeConstants();

        protected virtual void InitAll()
        {
            foreach (DrawableGameComponent comp in MyComponents)
            {
                comp.Initialize();
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Enable or disable the components I own
        /// </summary>
        /// <param name="t">T/F</param>
        public virtual void SetEnable(bool t)
        {
            foreach(DrawableGameComponent comp in MyComponents)
            {
                comp.Enabled = t;
            }
        }

        /// <summary>
        /// Make visible or invisible the components I own
        /// </summary>
        /// <param name="t">T/F</param>
        public virtual void SetVisible(bool t)
        {
            foreach (DrawableGameComponent comp in MyComponents)
                comp.Visible = t;
        }

        /// <summary>
        /// Call to re-init all components I own
        /// </summary>
        public virtual void ReInitAll()
        {
            foreach (DrawableGameComponent comp in MyComponents)
                comp.Initialize();
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
        #endregion
    }
}