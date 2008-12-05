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
using Duologue.Screens;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.UI;
#endregion

namespace Duologue.Screens
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GamePlayLoop : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const float playerMovementModifier_X = 2f;
        private const float playerMovementModifier_Y = -2f;
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public GamePlayLoop(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        #endregion


        #region Update / Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            #region Player stuff
            // First, run through the players, doing their stuff
            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                Player p = LocalInstanceManager.Players[i];
                if (p.Active)
                {
                    if (p.State == PlayerState.Alive ||
                       p.State == PlayerState.GettingReady)
                    {
                        // Update player position
                        p.Position.X += 
                            InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Left.X
                            * playerMovementModifier_X;
                        p.Position.Y +=
                            InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Left.Y
                            * playerMovementModifier_Y;

                        // Update player's orientation
                        if (InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Left.X != 0 ||
                           InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Left.Y != 0)
                        {
                            p.Orientation.X =
                                InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Left.X;
                            p.Orientation.Y = -1f *
                                InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Left.Y;
                        }

                        // Update player's aim
                        if (InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Right.X != 0 ||
                            InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Right.Y != 0)
                        {
                            p.Aim.X =
                                InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Right.X;
                            p.Aim.Y = -1f *
                                InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].ThumbSticks.Right.Y;
                            p.Fire();
                        }

                        // Button handling
                        if (InstanceManager.InputManager.CurrentGamePadStates[(int)p.MyPlayerIndex].Triggers.Left > 0 &&
                            InstanceManager.InputManager.LastGamePadStates[(int)p.MyPlayerIndex].Triggers.Left == 0)
                        {
                            // Swap color
                            p.SwapColors();
                        }

                        // Now, make sure no one is stepping on eachother
                        bool dumb;
                        dumb = p.StartOffset();
                        // Yeah, not efficient... but we have very low n in O(n^2)
                        for (int j = 0; j < InputManager.MaxInputs; j++)
                        {
                            if (j != i)
                            {
                                if (LocalInstanceManager.Players[j].Active)
                                {
                                    dumb = p.UpdateOffset(LocalInstanceManager.Players[i]);
                                }
                            }
                        }
                        dumb = p.ApplyOffset();
                    }
                        
                    p.Update(gameTime);
                }
            }
            #endregion Player Stuff

            #region Enemy Stuff

            #endregion Enemy Stuff

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // First, run through the players
            for (int i = 0; i < InputManager.MaxInputs; i++)
            {
                Player p = LocalInstanceManager.Players[i];
                if (p.Active)
                {
                    p.Draw(gameTime);
                }
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}