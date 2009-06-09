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
//using Scurvy.Media.VideoModel;
//using Scurvy.Media;
#endregion

namespace Duologue.Screens
{
    public class CreditsScreen : DrawableGameComponent
    {
        #region Constants
        private const string fontFilename = "Fonts/inero-50";
        private const string vidFilename = "Content/cred";
        #endregion

        #region Fields
        private CreditsScreenManager myManager;
        private SpriteFont font;
        private DuologueGame localGame;

        private Vector2 pos;
        private AudioManager audio;
        //private ContentManager contentMangler;
        //private Video vid;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public CreditsScreen(Game game, CreditsScreenManager manager)
            : base(game)
        {
            localGame = (DuologueGame)game;
            myManager = manager;
            //FFFFFFFFFFFFFFFFFFFFFFFFUUUUUUUUUUUUUUUUUUUUUUUUUUU
            //contentMangler = new VideoContentManager(game.Services);
        }

        public override void Initialize()
        {
            audio = ServiceLocator.GetService<AudioManager>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = InstanceManager.AssetManager.LoadSpriteFont(fontFilename);
            pos = new Vector2(400, 400);

            //FFFFFFFFFFFFFFFFFFFFFFFFUUUUUUUUUUUUUUUUUUUUUUUUUUU
            //vid = contentMangler.Load<Video>(vidFilename);
            //vid.Loop = false;

            base.LoadContent();
        }
        #endregion

        #region Update / Draw
        public override void Update(GameTime gameTime)
        {
            //FFFFFFFFFFFFFFFFFFFFFFFFUUUUUUUUUUUUUUUUUUUUUUUUUUU
            //vid.Update();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            InstanceManager.RenderSprite.DrawString(
                font, "Placeholder for credits", pos, Color.Azure);

            //FFFFFFFFFFFFFFFFFFFFFFFFUUUUUUUUUUUUUUUUUUUUUUUUUUU
            //if (vid.IsPlaying)
            //{
            //    localGame.spriteBatch.Begin();
            //    localGame.spriteBatch.Draw(vid.CurrentTexture, new Vector2(10, 10), Color.White);
            //    localGame.spriteBatch.End();
            //}

            base.Draw(gameTime);
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (null != audio)
            {
                if (Enabled)
                {
                    //FFFFFFFFFFFFFFFFFFFFFFFFUUUUUUUUUUUUUUUUUUUUUUUUUUU
                    //vid.Play();
                    
                    if (audio.SongIsPaused(SongID.Credits))
                    {
                        audio.ResumeSong(SongID.Credits);
                    }
                    else
                    {
                        //audio.PlaySong(SongID.Credits);
                        audio.FadeIn(SongID.Credits);
                    }
                    
                }
                else
                {
                    //FFFFFFFFFFFFFFFFFFFFFFFFUUUUUUUUUUUUUUUUUUUUUUUUUUU
                    //vid.Stop();
                    audio.PauseSong(SongID.Credits);
                }
            }
            base.OnEnabledChanged(sender, args);
        }
        #endregion
    }
}