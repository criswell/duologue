#region File Description
#endregion

#region Using Statements
// System
using System;
using System.Collections.Generic;
// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
// Mimicware
using Mimicware.Manager;
using Mimicware.Graphics;
using Mimicware;
// Duologue
using Duologue;
using Duologue.Audio;
using Duologue.State;
using Duologue.Properties;
using Duologue.Screens;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.UI;
#endregion

namespace Duologue.Screens
{
    public class EndCinematicScreen : DrawableGameComponent
    {
        #region Constants
        private const string fontFilename = "Fonts/inero-50";
        private const int numberOfPlayers = 20;

        // Time triggers and limits
        private double time_TotalRunTime = 20.0;
        #endregion

        #region Fields
        private EndCinematicScreenManager myManager;
        private SpriteFont font;

        private Vector2 pos;
        private AudioManager audio;

        private Player[] players;

        private bool infiniteModeResults;

        // Timer stuff
        private double masterTimer;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public EndCinematicScreen(Game game, EndCinematicScreenManager manager)
            : base(game)
        {
            myManager = manager;
            infiniteModeResults = false;
            masterTimer = 0;
        }

        protected override void LoadContent()
        {
            font = InstanceManager.AssetManager.LoadSpriteFont(fontFilename);
            pos = new Vector2(400, 400);
            base.LoadContent();
        }

        public override void Initialize()
        {
            audio = ServiceLocator.GetService<AudioManager>();
            base.Initialize();
        }
        #endregion

        #region Public methods
        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (null != audio)
            {
                if (Enabled)
                {
                    if (audio.SongIsPaused(SongID.Tr8or))
                    {
                        audio.ResumeSong(SongID.Tr8or);
                    }
                    else
                    {
                        audio.FadeIn(SongID.Tr8or);
                    }

                }
                else
                {
                    audio.PauseSong(SongID.Tr8or);
                }
            }

            if (!Enabled && players != null)
            {
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    players[i].CleanUp();
                }
                players = null;
            }
            else if (Enabled && players == null)
            {
                players = new Player[numberOfPlayers];
                ColorState[] tempC = ColorState.GetColorStates();
                PlayerColors[] tempP = PlayerColors.GetPlayerColors();
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    players[i] = new Player();
                    //players[i].Initialize(
                        //PlayerColors.
                }
            }

            if (Enabled)
            {
                if (LocalInstanceManager.LastGameState == GameState.InfiniteGame)
                    infiniteModeResults = true;
                else
                    infiniteModeResults = false;

                masterTimer = 0;
            }
            base.OnEnabledChanged(sender, args);
        }
        #endregion

        #region Update / Draw
        public override void Update(GameTime gameTime)
        {
            masterTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (masterTimer > time_TotalRunTime)
            {
                LocalInstanceManager.CurrentGameState = GameState.Credits;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            InstanceManager.RenderSprite.DrawString(
                font,
                "Placeholder for cinematics",
                pos,
                Color.Azure);
            base.Draw(gameTime);
        }
        #endregion
    }
}
