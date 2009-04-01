using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace Mimicware
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GamePadHelper : Microsoft.Xna.Framework.GameComponent
    {
        protected PlayerIndex player;
        protected float gamePadTimer = 0f;
        protected float durationInMs = 0f;

        protected float chirpStepTimer = 0f;
        protected float chirpChange = 0f;
        protected float chirpAmount = 0f;
        protected bool chirping = false;
        protected bool waitingForPad = false;
        protected const float chirpStepTime = 50f; //milliseconds

        public GamePadHelper(Game game, PlayerIndex index)
            : base(game)
        {
            player = index;
            game.Components.Add(this);
            // TODO: Construct any child components here
        }

        public void ChirpIt(float milliseconds, float startSpeed, float endSpeed)
        {
            durationInMs = milliseconds;
            chirpChange = (endSpeed - startSpeed) * chirpStepTime / durationInMs;
            chirpAmount = startSpeed;
            chirping = true;
            Enabled = true;
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (chirping)
            {
                GamePad.SetVibration(player, chirpAmount, chirpAmount);
                chirpStepTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                gamePadTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (chirpStepTimer > chirpStepTime)
                {
                    chirpAmount += chirpChange;
                    GamePad.SetVibration(player, chirpAmount, chirpAmount);
                    chirpStepTimer = 0;
                }
                if (gamePadTimer > durationInMs)
                {
                    // If the gamepad isn't ready to shut vibration off, we need to
                    // keep trying until it is
                    waitingForPad = !GamePad.SetVibration(player, 0f, 0f);
                    chirping = false;
                    gamePadTimer = 0f;
                    durationInMs = 0f;
                }
            } 
            else if(waitingForPad)
            {
                if (GamePad.SetVibration(player, 0f, 0f))
                {
                    waitingForPad = false;
                    Enabled = false;
                }
            }
            base.Update(gameTime);
        }
    }
}