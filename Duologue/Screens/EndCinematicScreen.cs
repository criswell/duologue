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
        private const string fontFilename = "Fonts/inero-40";
        private const int numberOfPlayers = 20;

        private const int minColorValue = 144;
        private const int maxColorValue = 238;

        private const float colorPercent_Medium = 75f;
        private const float colorPercent_Dark = 50f;

        private const float offscreenStart = 771f + 62f;
        private const float playerSizeY = 64f;
        private const float playerSizeX = 62f;

        private const int minNumOffset = 2;
        private const int maxNumOffset = 6;

        private const float deltaPlayerMovementX = -4f;

        // Time triggers and limits
        private double time_TotalRunTime = 20.0;
        #endregion

        #region Fields
        private EndCinematicScreenManager myManager;
        private SpriteFont font;

        private Vector2 pos;
        private AudioManager audio;

        private Player[] players;
        private PlayerColors[] playerColors;

        private bool infiniteModeResults;
        private float numberOfVertShips = -1;

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
            playerColors = new PlayerColors[numberOfPlayers];
            Color tempColor;
            for (int i = 0; i < numberOfPlayers; i++)
            {
                tempColor = new Color(
                    (byte)MWMathHelper.GetRandomInRange(minColorValue, maxColorValue),
                    (byte)MWMathHelper.GetRandomInRange(minColorValue, maxColorValue),
                    (byte)MWMathHelper.GetRandomInRange(minColorValue, maxColorValue));

                playerColors[i] = PlayerColors.GeneratePlayerColor(
                    i.ToString(),
                    tempColor,
                    FadeColorByPercent(tempColor, colorPercent_Medium),
                    FadeColorByPercent(tempColor, colorPercent_Dark));
            }
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

        #region Private methods
        private Color FadeColorByPercent(Color tempColor, float colorPercent)
        {
            return new Color(
                (byte)(tempColor.R * colorPercent),
                (byte)(tempColor.G * colorPercent),
                (byte)(tempColor.B * colorPercent));
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
                if(numberOfVertShips <= 0)
                {
                    numberOfVertShips = InstanceManager.DefaultViewport.Height / playerSizeY;
                }
                players = new Player[numberOfPlayers];
                ColorState[] tempC = ColorState.GetColorStates();
                int playerColorIndex = MWMathHelper.GetRandomInRange(0, numberOfPlayers);
                float currentX = InstanceManager.DefaultViewport.Width + offscreenStart;
                float currentY = playerSizeY * (float)MWMathHelper.GetRandomInRange(0, numberOfVertShips);
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    players[i] = new Player();
                    players[i].Initialize(
                        playerColors[playerColorIndex],
                        PlayerIndex.One,
                        null,
                        null,
                        tempC[MWMathHelper.GetRandomInRange(0, tempC.Length)],
                        new Vector2(currentX, currentY),
                        -Vector2.UnitX,
                        4);

                    players[i].IgnoreScreenBoundaries = true;
                    players[i].SetAssetManager(InstanceManager.AssetManager);
                    players[i].SetGraphicsDevice(InstanceManager.GraphicsDevice);
                    players[i].SetRenderSprite(InstanceManager.RenderSprite);
                    players[i].SetAlive();

                    currentY += playerSizeY * (float)MWMathHelper.GetRandomInRange(minNumOffset, maxNumOffset);
                    if (currentY > InstanceManager.DefaultViewport.Height)
                    {
                        currentY = InstanceManager.DefaultViewport.Height - currentY;
                        currentX += playerSizeX;
                    }
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

            for (int i = 0; i < numberOfPlayers; i++)
            {
                players[i].Position += Vector2.UnitX * deltaPlayerMovementX;
                if (players[i].Position.X < - playerSizeX)
                {
                    players[i].Position.X = InstanceManager.DefaultViewport.Width + offscreenStart;
                    players[i].Update(gameTime);
                }
            }

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

            for (int i = 0; i < numberOfPlayers; i++)
            {
                players[i].Draw(gameTime);
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}
