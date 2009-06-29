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
    public class EndCinematicScreen : DrawableGameComponent
    {
        #region Constants
        private const string fontFilename = "Fonts/inero-28";
        private const int numberOfPlayers = 15;

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
        private const float deltaTextMovementY = -0.21f;

        private const float percentageTextNeedsToMoveForNextSpawn = 1.45f;
        private const float percentageTextNeedsToMoveForNextSpawnBreak = 4.45f;

        // Time triggers and limits
        private double totalTime_TotalRun = 99.0;//64.0;
        private double totalTime_Type = 2.1;

        private double trigger_StartText = 3.41;
        private double trigger_EndText = 57.38;
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

        private string[] theText;
        private TeletypeEntry[] teletypeEntries;
        private Vector2 textStartPos;
        private int nextText = 0;
        private Teletype teletype = null;
        private Color color_Text;
        private Color color_Shadow;
        private Vector2[] offset_Shadow;
        private bool stopScrolling;

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

            theText = new String[]
            {
                Resources.EndCinematic01,
                Resources.EndCinematic02,
                Resources.EndCinematic03,
                Resources.EndCinematic04,
                Resources.EndCinematic05,
                Resources.EndCinematic06,
                Resources.EndCinematic07,
                Resources.EndCinematic08,
                Resources.EndCinematic09,
                Resources.EndCinematic10,
                Resources.EndCinematic11,
            };

            teletypeEntries = new TeletypeEntry[theText.Length];

            offset_Shadow = new Vector2[]
            {
                2f * Vector2.One,
                -2f * Vector2.One,
                Vector2.UnitX,
                Vector2.UnitY,
                10f * Vector2.One,
            };

            color_Text = Color.FloralWhite;
            color_Shadow = new Color(
                Color.Black, 0.55f);
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
                LocalInstanceManager.Background.SetBackground(0);
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
                    players[i].Aim = -Vector2.UnitX;

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
                        (float)MWMathHelper.GetRandomInRange(0, InstanceManager.DefaultViewport.Width * InstanceManager.TitleSafePercent),
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

                if (teletype == null)
                {
                    Vector2 tempSize;
                    textStartPos = new Vector2(
                        InstanceManager.DefaultViewport.Width / 2f,
                        InstanceManager.DefaultViewport.Height * InstanceManager.TitleSafePercent - font.LineSpacing);

                    teletype = ServiceLocator.GetService<Teletype>();
                    for (int i = 0; i < theText.Length; i++)
                    {
                        tempSize = font.MeasureString(theText[i]);
                        teletypeEntries[i] = new TeletypeEntry(
                            font,
                            theText[i],
                            textStartPos,
                            tempSize / 2f,
                            color_Text,
                            totalTime_Type,
                            9999.0,
                            color_Shadow,
                            offset_Shadow,
                            InstanceManager.RenderSprite);
                    }
                }

                teletype.FlushEntries();
                nextText = 0;
                stopScrolling = false;
            }
            else
            {
                if(teletype != null)
                    teletype.FlushEntries();
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
                if (players[i].Position.X < - playerSizeX && masterTimer < trigger_EndText)
                {
                    players[i].Position.X = InstanceManager.DefaultViewport.Width + offscreenStart;
                    // Make sure no overlap
                    players[i].StartOffset();
                    for (int j = 0; j < numberOfPlayers; j++)
                    {
                        if(i != j)
                            players[i].UpdateOffset(players[j]);
                    }
                    players[i].ApplyOffset();
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

            if (masterTimer > totalTime_TotalRun)
            {
                LocalInstanceManager.CurrentGameState = GameState.Credits;
            }
            else if (masterTimer > trigger_EndText)
            {
                if (stopScrolling)
                {
                    teletype.FlushEntries();
                    stopScrolling = false;
                    audio.FadeOut(SongID.Tr8or);
                }
            }
            else if (masterTimer > trigger_StartText)
            {
                if (nextText > 0)
                {
                    // Run through moving old entries
                    if (!stopScrolling)
                    {
                        for (int i = 0; i < nextText; i++)
                        {
                            teletypeEntries[i].Position.Y += deltaTextMovementY;
                        }
                    }

                    // see if time to spawn next entry
                    if (nextText == teletypeEntries.Length)
                    {
                        if (teletypeEntries[nextText - 1].Position.Y < textStartPos.Y -
                            percentageTextNeedsToMoveForNextSpawn * font.LineSpacing)
                        {
                            stopScrolling = true;
                        }
                    }
                    else if (nextText / 3f == nextText / 3)
                    {
                        if (teletypeEntries[nextText - 1].Position.Y < textStartPos.Y -
                            percentageTextNeedsToMoveForNextSpawnBreak * font.LineSpacing &&
                            nextText < teletypeEntries.Length)
                        {
                            teletype.AddEntry(teletypeEntries[nextText]);
                            nextText++;
                        }
                    }
                    else
                    {
                        if (teletypeEntries[nextText - 1].Position.Y < textStartPos.Y -
                            percentageTextNeedsToMoveForNextSpawn * font.LineSpacing &&
                            nextText < teletypeEntries.Length)
                        {
                            teletype.AddEntry(teletypeEntries[nextText]);
                            nextText++;
                        }
                    }
                }
                else
                {
                    teletype.AddEntry(teletypeEntries[nextText]);
                    nextText++;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
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
