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

            #endregion

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //font = InstanceManager.AssetManager.LoadSpriteFont(fontFilename);
            //pos = new Vector2(400, 400);

            CreditPage tempPage;

            

            base.LoadContent();
        }
        #endregion

        #region Public methods
        public void TestDataFormat(StorageDevice device)
        {
            Console.WriteLine("HEY BITCH FIX THIS FUCKER! CreditsScreen.cs");
            CreditPage tempPage = new CreditPage();

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

            // Open a storage container.
            StorageContainer container =
                device.OpenContainer("Duologue");

            // Get the path of the save game.
            string filename = Path.Combine(container.Path, "credittest");
            FileStream stream;
            if (File.Exists(filename))
            {
                stream = File.Open(filename, FileMode.Truncate);
            }
            else
            {
                stream = File.Open(filename, FileMode.CreateNew);
            }

            // Convert the object to XML data and put it in the stream.
            XmlSerializer serializer = new XmlSerializer(typeof(CreditPage));
            serializer.Serialize(stream, tempPage);

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();
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
    }
}