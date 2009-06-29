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

        private const int numberOfLavas = 13;

        private const int minColorValue = 99;
        private const int maxColorValue = 238;

        private const float colorPercent_Medium = 75f;
        private const float colorPercent_Dark = 50f;

        private const float offscreenStart = 671f + 62f;
        private const float playerSizeY = 64f;
        private const float playerSizeX = 62f;

        private const int minNumOffset = 2;
        private const int maxNumOffset = 6;

        private const float minHorizOffset = 3.2f;
        private const float maxHorizOffset = 5.4f;

        private const int chanceDeltaChange = 45;

        private const float minAimDelta = -MathHelper.PiOver4 * 0.02f;
        private const float maxAimDelta = MathHelper.PiOver4 * 0.02f;

        private const float deltaPlayerMovementX = -2.7f;

        // Time triggers and limits
        private double time_TotalRunTime = 40.0;
        #endregion

        #region Fields
        private EndCinematicScreenManager myManager;
        private SpriteFont font;

        private Vector2 pos;
        private AudioManager audio;

        private Player[] players;
        private PlayerColors[] playerColors;
        private float[] playerAimDelta;
        
        private bool infiniteModeResults;
        private float maxInitialOffset = -1;
        private float minY = -1;
        private float maxY = -1;

        private BKG_LavaBurp[] lavas;

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
            lavas = new BKG_LavaBurp[numberOfLavas];
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
                if(maxInitialOffset <= 0)
                {
                    maxInitialOffset = 0.5f * InstanceManager.DefaultViewport.TitleSafeArea.Height / playerSizeY;
                }
                players = new Player[numberOfPlayers];
                playerAimDelta = new float[numberOfPlayers];
                ColorState[] tempC = ColorState.GetColorStates();
                int playerColorIndex = MWMathHelper.GetRandomInRange(0, numberOfPlayers);
                minY = InstanceManager.DefaultViewport.Height -
                    InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent;
                maxY = InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent;
                float currentX = InstanceManager.DefaultViewport.Width + offscreenStart;
                float currentY = playerSizeY * (float)MWMathHelper.GetRandomInRange(0, maxInitialOffset) +
                    minY;
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    players[i] = new Player();
                    players[i].Initialize(
                        playerColors[playerColorIndex],
                        PlayerIndex.One,
                        null,
                        null,
                        tempC[MWMathHelper.GetRandomInRange(0, tempC.Length)],
                        new Vector2(
                            currentX + (float)MWMathHelper.GetRandomInRange(-1.0, 1.0) * playerSizeX,
                            currentY),
                        -Vector2.UnitX,
                        4);
                    /*Console.WriteLine(String.Format(
                        "P[{0}]: {1}", i.ToString(), players[i].Position.ToString()));*/

                    players[i].IgnoreScreenBoundaries = true;
                    players[i].SetAssetManager(InstanceManager.AssetManager);
                    players[i].SetGraphicsDevice(InstanceManager.GraphicsDevice);
                    players[i].SetRenderSprite(InstanceManager.RenderSprite);
                    players[i].SetAlive();

                    float tempOffset = (float)MWMathHelper.GetRandomInRange(minNumOffset, maxNumOffset);

                    currentY += playerSizeY * tempOffset;
                    if (currentY > maxY)
                    {
                        currentY = minY + playerSizeY * tempOffset;
                        currentX += playerSizeX * (float)MWMathHelper.GetRandomInRange(minHorizOffset, maxHorizOffset);
                    }

                    playerColorIndex++;
                    if (playerColorIndex >= numberOfPlayers)
                        playerColorIndex = 0;

                    playerAimDelta[i] = (float)MWMathHelper.GetRandomInRange(minAimDelta, maxAimDelta);
                }

                // init lavas
                for (int i = 0; i < numberOfLavas; i++)
                {
                    lavas[i] = new BKG_LavaBurp();
                    lavas[i].Initialize(new Vector2(
                        (float)MWMathHelper.GetRandomInRange(0, InstanceManager.DefaultViewport.Width),
                        (float)MWMathHelper.GetRandomInRange(minY, maxY)));
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
                }
                players[i].Aim = MWMathHelper.RotateVectorByRadians(players[i].Aim, playerAimDelta[i]);
                if (MWMathHelper.GetRandomInRange(0, chanceDeltaChange) == 1)
                {
                    playerAimDelta[i] = (float)MWMathHelper.GetRandomInRange(minAimDelta, maxAimDelta);
                }

                players[i].Update(gameTime);
            }

            for (int i = 0; i < numberOfLavas; i++)
            {
                lavas[i].Update(gameTime);
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
            for (int i = 0; i < numberOfLavas; i++)
            {
                lavas[i].Draw(gameTime);
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}
