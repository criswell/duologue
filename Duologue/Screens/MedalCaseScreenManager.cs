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
    public class MedalCaseScreenManager : GameScreen
    {
        #region Constants
        #endregion

        #region Fields
        private AudioManager audio;

        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public MedalCaseScreenManager(Game game)
            : base(game)
        {
        }

        protected override void InitializeConstants()
        {
            // Nothing to see here, go away
        }
        #endregion

        #region Public methods
        public override void ScreenEntrance(GameTime gameTime)
        {
            // Turn off background
            LocalInstanceManager.Background.Enabled = false;
            LocalInstanceManager.Background.Visible = false;

            // Turn on achievement manager's medal screen
            LocalInstanceManager.AchievementManager.EnableMedalScreen();
            audio = ServiceLocator.GetService<AudioManager>();
            if (audio.SongIsPaused(SongID.Tr8or))
            {
                audio.ResumeSong(SongID.Tr8or);
            }
            else
            {
                audio.FadeIn(SongID.Tr8or);
            }

            base.ScreenEntrance(gameTime);
        }

        public override void ScreenExit(GameTime gameTime)
        {
            // Turn off achievement manager's medal screen
            LocalInstanceManager.AchievementManager.DisableMedalScreen();

            // Turn on background
            LocalInstanceManager.Background.Enabled = true;
            LocalInstanceManager.Background.Visible = true;
            //audio.PauseSong(SongID.Tr8or);
            audio.StopSong(SongID.Tr8or);

            base.ScreenExit(gameTime);
        }
        #endregion

        #region Update
        #endregion
    }
}
