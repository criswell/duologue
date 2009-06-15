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
using Mimicware.Fx;
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

    public enum CreditState
    {
        SetNext,
        MoveIn,
        Steady,
        MoveOut,
    }

    public class CreditsScreen : DrawableGameComponent
    {
        #region Constants
        private const string filename_FontHeaderOne = "Fonts/inero-50";
        private const string filename_FontHeaderTwo = "Fonts/inero-40";
        private const string filename_FontContent = "Fonts/inero-28";
        private const string filename_Blank = "Mimicware/blank";
        private const int maxNumOfPages = 20;

        private const double totalTime_MoveIn = 0.5;
        private const double totalTime_Steady = 4.0;
        private const double totalTime_MoveOut = 0.5;
        private const double totalTime_Type = 0.35;
        private const double totalTime_TextOnScreen = totalTime_MoveIn + totalTime_Steady + totalTime_MoveOut * .5;

        private const float offsetHeader = 40f;
        private const float standardWidthOfTexture = 250f;
        private const float offsetShadow = 9.76f;
        #endregion

        #region Fields
        private CreditsScreenManager myManager;
        private DuologueGame localGame;

        private AudioManager audio;

        /// <summary>
        /// List of pages
        /// </summary>
        private List<CreditPage> thePages;
        private int currentPage;

        private double timer_Page;

        private SpriteFont font_HeaderOne;
        private SpriteFont font_HeaderTwo;
        private SpriteFont font_Content;

        private CreditState currentState;

        private Texture2D texture_Current;
        private Vector2 center;
        private Vector2 pos_Texture;
        private Vector2 pos_StartText;
        private Texture2D texture_Blank;
        private Color color_Text;
        private Color color_Shadow;
        private Vector2[] shadow;

        private Teletype teletype;
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
            currentState = CreditState.SetNext;
            pos_StartText = Vector2.Zero;
            pos_Texture = Vector2.Zero;
        }

        protected override void LoadContent()
        {
            font_HeaderOne = InstanceManager.AssetManager.LoadSpriteFont(filename_FontHeaderOne);
            font_HeaderTwo = InstanceManager.AssetManager.LoadSpriteFont(filename_FontHeaderTwo);
            font_Content = InstanceManager.AssetManager.LoadSpriteFont(filename_FontContent);

            texture_Blank = InstanceManager.AssetManager.LoadTexture2D(filename_Blank);

            color_Text = new Color(212, 189, 255);
            color_Shadow = new Color(Color.Black, 165);
            shadow = new Vector2[]
            {
                offsetShadow * Vector2.One,
                offsetShadow * Vector2.UnitY,
                offsetShadow * Vector2.UnitX
            };

            teletype = null;

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
            if(pos_Texture == Vector2.Zero)
                pos_Texture = new Vector2(
                    InstanceManager.DefaultViewport.TitleSafeArea.Right - standardWidthOfTexture/2f,
                    InstanceManager.DefaultViewport.Height/2f);
            if(pos_StartText == Vector2.Zero)
                pos_StartText = new Vector2(
                    InstanceManager.DefaultViewport.TitleSafeArea.X,
                    InstanceManager.DefaultViewport.TitleSafeArea.Y);
            
            if (teletype == null)
                teletype = ServiceLocator.GetService<Teletype>();

            timer_Page += gameTime.ElapsedGameTime.TotalSeconds;
            switch (currentState)
            {
                case CreditState.MoveIn:
                    break;
                case CreditState.MoveOut:
                    break;
                case CreditState.SetNext:
                    // Verify we have another to get
                    if (currentPage >= thePages.Count)
                        myManager.QuitScreen();
                    else
                    {
                        // Load next image
                        if (thePages[currentPage].ImageFilename != null)
                        {
                            texture_Current =
                                InstanceManager.AssetManager.LoadTexture2D(thePages[currentPage].ImageFilename);
                        }
                        else
                        {
                            texture_Current = texture_Blank;
                        }
                        center = new Vector2(
                            texture_Current.Width / 2f, texture_Current.Height / 2f);

                        // Reset the timer
                        timer_Page = 0;

                        // Queue up the teletype texts
                        TeletypeEntry tempEntry;
                        Vector2 tempPos = Vector2.Zero;
                        for (int i = 0; i < thePages[currentPage].Headers.Length; i++)
                        {
                            tempEntry = new TeletypeEntry(
                                font_HeaderTwo,
                                thePages[currentPage].Headers[i],
                                tempPos + pos_StartText,
                                color_Text,
                                totalTime_Type,
                                totalTime_TextOnScreen,
                                color_Shadow,
                                shadow,
                                InstanceManager.RenderSprite);

                            if (i == 0)
                            {
                                tempEntry.Font = font_HeaderOne;
                                tempPos += Vector2.UnitY * font_HeaderOne.LineSpacing;
                            }
                            else
                            {
                                tempPos += Vector2.UnitY * font_HeaderTwo.LineSpacing;
                            }

                            teletype.AddEntry(tempEntry);
                        }
                        tempPos += Vector2.UnitY * offsetHeader;

                        for (int i = 0; i < thePages[currentPage].Content.Length; i++)
                        {
                            tempEntry = new TeletypeEntry(
                                font_Content,
                                thePages[currentPage].Content[i],
                                tempPos + pos_StartText,
                                color_Text,
                                totalTime_Type,
                                totalTime_TextOnScreen,
                                color_Shadow,
                                shadow,
                                InstanceManager.RenderSprite);
                            teletype.AddEntry(tempEntry);
                            tempPos += Vector2.UnitY * font_Content.LineSpacing;
                        }
                        currentState = CreditState.MoveIn;
                    }

                    break;
                default:
                    // Steady
                    break;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        #endregion
    }
}