#region File Description
#endregion

#region Using Statements
// System
using System;
using System.Collections.Generic;
// FIXME, will we need this later on?
using System.IO;
using System.Xml.Serialization;
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
    [Serializable]
    public struct CreditPage
    {
        public string ImageFilename;
        public string[] Headers;
        public string[] Content;
    }

    public class CreditsScreen : DrawableGameComponent
    {
        #region Constants
        //private const string fontFilename = "Fonts/inero-50";
        private const int maxNumOfPages = 20;

        private const double timePerPage = 4.0;
        #endregion

        #region Fields
        private CreditsScreenManager myManager;
        //private SpriteFont font;
        private DuologueGame localGame;

        private AudioManager audio;

        /// <summary>
        /// List of pages
        /// </summary>
        private List<CreditPage> thePages;
        private int currentPage;

        private double timer_Page;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public CreditsScreen(Game game, CreditsScreenManager manager)
            : base(game)
        {
            localGame = (DuologueGame)game;
            myManager = manager;
        }

        public override void Initialize()
        {
            audio = ServiceLocator.GetService<AudioManager>();

            thePages = new List<CreditPage>(maxNumOfPages);

            // BAH, fucking hate having this in code, but I'll FIXME this
            // next game
            #region Parade of Ugliness
            CreditPage tempPage = new CreditPage();

            // First page
            tempPage.ImageFilename = null;
            tempPage.Headers = new string[]
            {
                "Duologue"
            };
            tempPage.Content = new string[]
            {
                "Copyright (c) 2009 Funavision Electronic Entertainment"
            };

            thePages.Add(tempPage);
            tempPage = new CreditPage();

            // Second page
            tempPage.ImageFilename = "credits/criswellious";
            tempPage.Headers = new string[]
            {
                "Sam Hart",
                "Gamertag: Criswellious",
                "Studio Head (CVTL FTW)",
            };
            tempPage.Content = new string[]
            {
                "Game creator",
                "Lead developer",
                "Game artwork"
            };

            thePages.Add(tempPage);
            tempPage = new CreditPage();

            // Third page
            tempPage.ImageFilename = "credits/doctorcal";
            tempPage.Headers = new string[]
            {
                "Glen Smith",
                "Gamertag: DoctorCal",
                "Audio Rockstar",
            };
            tempPage.Content = new string[]
            {
                "Audio engine developer",
                "Game music",
                "Secondary development",
            };

            thePages.Add(tempPage);
            tempPage = new CreditPage();

            // Fourth page
            tempPage.ImageFilename = "credits/schultmc";
            tempPage.Headers = new string[]
            {
                "Michael Schultheiss",
                "Gamertag: schultmc",
                "Infrastructure Guru",
            };
            tempPage.Content = new string[]
            {
                "Infrastructure management",
                "Lead testing",
            };

            thePages.Add(tempPage);
            tempPage = new CreditPage();

            // Fifth page
            tempPage.ImageFilename = "credits/testers";
            tempPage.Headers = new string[]
            {
                "Testers",
            };
            tempPage.Content = new string[]
            {
                "Jake Tabke (RavenclawX)",
                "Jessica Hart (Jesness)",
                "Justin Lebreck (Raprot)",
                "Melanie Smith (melanieeeeeee)",
            };

            thePages.Add(tempPage);
            tempPage = new CreditPage();

            // Sixth page
            tempPage.ImageFilename = null;
            tempPage.Headers = new string[]
            {
                "Special thanks",
            };
            tempPage.Content = new string[]
            {
                "Blah, blah, blah",
            };

            thePages.Add(tempPage);
            #endregion

            ResetAll();

            base.Initialize();
        }

        public void ResetAll()
        {
            timer_Page = 0;
            currentPage = 0;
        }

        protected override void LoadContent()
        {
            //font = InstanceManager.AssetManager.LoadSpriteFont(fontFilename);
            //pos = new Vector2(400, 400);

            base.LoadContent();
        }
        #endregion

        #region Public methods
        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (null != audio)
            {
                if (Enabled)
                {
                    if (audio.SongIsPaused(SongID.Credits))
                    {
                        audio.ResumeSong(SongID.Credits);
                    }
                    else
                    {
                        audio.FadeIn(SongID.Credits);
                    }

                }
                else
                {
                    audio.PauseSong(SongID.Credits);
                }
            }
            base.OnEnabledChanged(sender, args);
        }
        #endregion

        #region Update / Draw
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        #endregion
    }
}