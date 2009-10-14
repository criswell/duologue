#region File Description
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
// XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
// Mimicware
using Mimicware;
using Mimicware.Graphics;
using Mimicware.Manager;
using Mimicware.Debug;
using Mimicware.Fx;
// Duologue
using Duologue.ParticleEffects;
using Duologue.PlayObjects;
using Duologue.UI;
using Duologue;
using Duologue.Properties;
using Duologue.Audio;
using Duologue.Screens;
using Duologue.Waves;
#endregion

namespace Duologue.State
{
    [Serializable]
    public struct SavedGameData
    {
        public int MajorWaveNumber;
        public int Year;
        public int Month;
        public int Day;
        public int Hour;
        public int Minute;
    }

    public enum SaveGameState
    {
        Menu,
        SaveConfirm,
        Saving,
        LoadConfirm,
        Loading,
        SaveDone,
        LoadDone,
    }

    public class SaveGame : IService
    {
        #region Constants
        private const string filename_Pixel = "Mimicware/blank";
        private const string filename_ButtonX = "PlayerUI/buttonX";
        private const string filename_ButtonY = "PlayerUI/buttonY";
        private const string filename_ButtonB = "PlayerUI/buttonB";
        private const string filename_Font = "Fonts/deja-med";
        private const string filename_Blip = "Audio/PlayerEffects/menu_move_beep";

        private const string name_Survival = "Survival";
        private const string name_Campaign = "Campaign";
        private const string containerName = "Duologue";

        private const float numGradients = 20f;
        private const double totalTime_Pause = 0.45;
        #endregion

        #region Fields
        private StorageDevice storageDevice;
        private bool isDeviceSet;
        private PauseScreen pauseScreen;
        private RenderSprite render;
        private GameWaveManager gameWaveManager;
        private InputManager inputManager;
        private SoundEffect sfx_Blip;


        private Texture2D texture_Pixel;
        private Texture2D texture_ButtonX;
        private Vector2 center_ButtonX;
        private Texture2D texture_ButtonY;
        private Vector2 center_ButtonY;
        private Texture2D texture_ButtonB;
        private Vector2 center_ButtonB;
        private SpriteFont font;

        private Vector2 scale_Base;
        private Vector2 scale_WindowOutline;
        private Vector2 scale_Window;
        private Vector2 scale_SaveWindow;
        private Vector2 position_SaveWindow;
        private Vector2 screen_Center;
        private Vector2 offset_Title;
        private Vector2 offset_SaveData;
        private Vector2 offset_Load;
        private Vector2 offset_Save;
        private Vector2 offset_Cancel;
        private Vector2[] offset_Shadow;

        private SavedGameData campaignSavedGame;
        private SavedGameData survivalSavedGame;
        private SavedGameData nullSavedGame;

        private Color color_Base;
        private Color color_WindowOutline;
        private Color color_WindowStart;
        private Color color_WindowEnd;
        private Color color_SaveWindow;
        private Color color_Title;
        private Color color_SaveData;
        private Color color_Interface;
        private Color color_Shadow;

        private double timer;
        private SaveGameState currentState;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public SaveGame()
        {
            isDeviceSet = false;
            color_Base = new Color(30, 0, 23, 200);
            color_WindowStart = new Color(90, 80, 80, 255);
            color_WindowEnd = new Color(139, 129, 129, 255);
            color_WindowOutline = new Color(199, 190, 190, 255);
            color_SaveWindow = new Color(120, 120, 130, 255);
            color_Title = Color.AliceBlue;
            color_SaveData = Color.Aqua;
            color_Interface = Color.White;
            color_Shadow = new Color(45, 45, 45, 128);
            offset_Shadow = new Vector2[] { 5f * Vector2.One };

            timer = 0;
            currentState = SaveGameState.Menu;
        }

        public void LoadContent()
        {
            AssetManager assMan = InstanceManager.AssetManager;
            inputManager = InstanceManager.InputManager;

            texture_ButtonB = assMan.LoadTexture2D(filename_ButtonB);
            texture_ButtonX = assMan.LoadTexture2D(filename_ButtonX);
            texture_ButtonY = assMan.LoadTexture2D(filename_ButtonY);

            font = assMan.LoadSpriteFont(filename_Font);

            sfx_Blip = assMan.LoadSoundEffect(filename_Blip);

            center_ButtonB = new Vector2(
                0f, texture_ButtonB.Height / 2f - font.LineSpacing / 2f);
            center_ButtonX = center_ButtonB;
            center_ButtonY = center_ButtonB;

            texture_Pixel = assMan.LoadTexture2D(filename_Pixel);
            scale_Base = Vector2.Zero;

            // Set the offsets
            string[] tempText =
            {
                Resources.LoadSave_Cancel,
                Resources.LoadSave_FileFormat,
                Resources.LoadSave_Load,
                Resources.LoadSave_Save,
                String.Format(Resources.LoadSave_Title, name_Campaign)
            };

            Vector2 maxSize = Vector2.Zero;
            Vector2 tempSize;
            for (int i = 0; i < tempText.Length; i++)
            {
                tempSize = font.MeasureString(tempText[i]);
                if (tempSize.X > maxSize.X)
                    maxSize.X = tempSize.X;
                if (tempSize.Y > maxSize.Y)
                    maxSize.Y = tempSize.Y;
            }

            offset_Title = new Vector2(
                -maxSize.X / 2f, -2f * font.MeasureString(Resources.LoadSave_FileFormat).Y - 1.6f *
                font.MeasureString(Resources.LoadSave_Title).Y);

            offset_SaveData = new Vector2(
                -maxSize.X / 2f, -2f * font.MeasureString(Resources.LoadSave_FileFormat).Y);

            position_SaveWindow = new Vector2(
                offset_SaveData.X - 100f, offset_SaveData.Y - 3f);

            scale_SaveWindow = position_SaveWindow * -2f;

            offset_Load = new Vector2(
                -maxSize.X / 2f, 2.8f * font.MeasureString(Resources.LoadSave_Title).Y);

            offset_Save = offset_Load + Vector2.UnitY * texture_ButtonB.Height * 1.1f;
            offset_Cancel = offset_Save + Vector2.UnitY * texture_ButtonB.Height * 1.1f;
        }

        public void SetStorageDevice(StorageDevice device)
        {
            storageDevice = device;
            isDeviceSet = true;
            nullSavedGame.MajorWaveNumber = 0;
            campaignSavedGame = LoadSavedGame(name_Campaign);
            survivalSavedGame = LoadSavedGame(name_Survival);
        }

        private void SaveGameData(string p, SavedGameData saveData)
        {
            if (isDeviceSet && storageDevice.IsConnected)
            {
                try
                {
                    // Open a storage container.
                    StorageContainer container =
                        storageDevice.OpenContainer(containerName);

                    // Get the path of the save game.
                    string filename = Path.Combine(container.Path, p);

                    InstanceManager.Logger.LogEntry("Saving game data file:");
                    InstanceManager.Logger.LogEntry(filename);

                    // Open the file, creating it if necessary.
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
                    XmlSerializer serializer = new XmlSerializer(typeof(SavedGameData));
                    serializer.Serialize(stream, saveData);

                    // Close the file.
                    stream.Close();

                    // Dispose the container, to commit changes.
                    container.Dispose();
                }
                catch
                {
                    ServiceLocator.GetService<MainMenuScreen>().ResetStorage();
                }
            }
        }

        private SavedGameData LoadSavedGame(string p)
        {
            if (isDeviceSet && storageDevice.IsConnected)
            {
                SavedGameData temp;

                // Open a storage container.
                StorageContainer container =
                    storageDevice.OpenContainer(containerName);

                // Get the path of the save game.
                string filename = Path.Combine(container.Path, p);

                // Check to see whether the save exists.
                if (File.Exists(filename))
                {
                    InstanceManager.Logger.LogEntry("Saved game data file:");
                    InstanceManager.Logger.LogEntry(filename);

                    // Open the file.
                    FileStream stream = File.Open(filename, FileMode.OpenOrCreate,
                        FileAccess.Read);

                    // Read the data from the file.
                    XmlSerializer serializer = new XmlSerializer(typeof(SavedGameData));
                    try
                    {
                        temp = (SavedGameData)serializer.Deserialize(stream);
                    }
                    catch
                    {
                        stream.Close();
                        container.Dispose();
                        return nullSavedGame;
                    }

                    // Close the file.
                    stream.Close();

                    // Dispose the container.
                    container.Dispose();

                    return temp;
                }
                else
                {
                    container.Dispose();
                    return nullSavedGame;
                }
            }
            else
            {
                return nullSavedGame;
            }
        }
        #endregion

        #region Public methods
        public void Entrance()
        {
            currentState = SaveGameState.Menu;
            timer = 0;
            if (scale_Base == Vector2.Zero)
            {
                screen_Center = new Vector2(InstanceManager.DefaultViewport.Width / 2f, InstanceManager.DefaultViewport.Height / 2f);
                pauseScreen = ServiceLocator.GetService<PauseScreen>();
                gameWaveManager = ServiceLocator.GetService<GameWaveManager>();
                scale_Base = new Vector2(InstanceManager.DefaultViewport.Width, InstanceManager.DefaultViewport.Height);
                render = InstanceManager.RenderSprite;
                scale_WindowOutline = new Vector2(
                    InstanceManager.DefaultViewport.TitleSafeArea.Width,
                    InstanceManager.DefaultViewport.TitleSafeArea.Height);
                scale_Window = scale_WindowOutline - 4f * Vector2.One;
                gameWaveManager = ServiceLocator.GetService<GameWaveManager>();
                pauseScreen = ServiceLocator.GetService<PauseScreen>();
            }
        }

        public void Update(GameTime gameTime)
        {
            switch (currentState)
            {
                case SaveGameState.LoadConfirm:
                    if (inputManager.NewButtonPressed(Buttons.B) ||
                        inputManager.NewButtonPressed(Buttons.Back))
                    {
                        currentState = SaveGameState.Menu;
                    }
                    else if (inputManager.NewButtonPressed(Buttons.X))
                    {
                        currentState = SaveGameState.Loading;
                    }
                    break;
                case SaveGameState.Loading:
                    timer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer > totalTime_Pause)
                    {
                        timer = 0;
                        if (LocalInstanceManager.CurrentGameState == GameState.CampaignGame)
                        {
                            LocalInstanceManager.LevelSet = true;
                            LocalInstanceManager.LevelSkip = true;
                            LocalInstanceManager.NextMajorWave = campaignSavedGame.MajorWaveNumber;
                            LocalInstanceManager.NextMinorWave = 0;
                        }
                        else
                        {
                            LocalInstanceManager.LevelSet = true;
                            LocalInstanceManager.LevelSkip = true;
                            LocalInstanceManager.NextMajorWave = survivalSavedGame.MajorWaveNumber;
                            LocalInstanceManager.NextMinorWave = 0;
                        }
                        currentState = SaveGameState.LoadDone;
                    }
                    break;
                case SaveGameState.LoadDone:
                    timer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer > totalTime_Pause)
                    {
                        timer = 0;
                        pauseScreen.ReturnFromSaveScreen();
                        currentState = SaveGameState.Menu;
                    }
                    break;
                case SaveGameState.SaveConfirm:
                    if (inputManager.NewButtonPressed(Buttons.B) ||
                        inputManager.NewButtonPressed(Buttons.Back))
                    {
                        currentState = SaveGameState.Menu;
                    }
                    else if (inputManager.NewButtonPressed(Buttons.X))
                    {
                        currentState = SaveGameState.Saving;
                    }
                    break;
                case SaveGameState.Saving:
                    timer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer > totalTime_Pause)
                    {
                        timer = 0;
                        if (LocalInstanceManager.CurrentGameState == GameState.CampaignGame)
                        {
                            campaignSavedGame.MajorWaveNumber = gameWaveManager.CurrentMajorNumber;
                            DateTime now = DateTime.Now;
                            campaignSavedGame.Year = now.Year;
                            campaignSavedGame.Month = now.Month;
                            campaignSavedGame.Day = now.Day;
                            campaignSavedGame.Hour = now.Hour;
                            campaignSavedGame.Minute = now.Minute;
                            SaveGameData(name_Campaign, campaignSavedGame);
                        }
                        else
                        {
                            survivalSavedGame.MajorWaveNumber = gameWaveManager.CurrentMajorNumber;
                            DateTime now = DateTime.Now;
                            survivalSavedGame.Year = now.Year;
                            survivalSavedGame.Month = now.Month;
                            survivalSavedGame.Day = now.Day;
                            survivalSavedGame.Hour = now.Hour;
                            survivalSavedGame.Minute = now.Minute;
                            SaveGameData(name_Survival, survivalSavedGame);
                        }
                        currentState = SaveGameState.SaveDone;
                    }
                    break;
                case SaveGameState.SaveDone:
                    timer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer > totalTime_Pause)
                    {
                        timer = 0;
                        pauseScreen.ReturnFromSaveScreen();
                        currentState = SaveGameState.Menu;
                    }
                    break;
                default:
                    // Menu
                    if (inputManager.NewButtonPressed(Buttons.B) || 
                        inputManager.NewButtonPressed(Buttons.Back) ||
                        inputManager.NewButtonPressed(Buttons.Start))
                    {
                        pauseScreen.ReturnFromSaveScreen();
                    }
                    else if (inputManager.NewButtonPressed(Buttons.Y))
                    {
                        sfx_Blip.Play(0.5f, 0, 0);
                        currentState = SaveGameState.SaveConfirm;
                    }
                    else if (inputManager.NewButtonPressed(Buttons.X) &&
                        ((LocalInstanceManager.CurrentGameState == GameState.SurvivalGame &&
                        survivalSavedGame.MajorWaveNumber > 0) ||
                        (LocalInstanceManager.CurrentGameState == GameState.CampaignGame &&
                        campaignSavedGame.MajorWaveNumber > 0)))
                    {
                        sfx_Blip.Play(0.5f, 0, 0);
                        currentState = SaveGameState.LoadConfirm;
                    }
                    break;
            }
        }

        public void Draw(GameTime gameTime)
        {
            render.Draw(
                texture_Pixel,
                Vector2.Zero,
                Vector2.Zero,
                null,
                color_Base,
                0f,
                scale_Base,
                0f,
                RenderSpriteBlendMode.AbsoluteTop);

            render.Draw(
                texture_Pixel,
                screen_Center - scale_WindowOutline/2f,
                Vector2.Zero,
                null,
                color_WindowOutline,
                0f,
                scale_WindowOutline,
                0f,
                RenderSpriteBlendMode.AbsoluteTop);

            Vector2 pos = screen_Center - scale_Window / 2f;
            float stepSize = scale_Window.Y / numGradients;

            for (int i = 0; i < numGradients; i++)
            {
                render.Draw(
                    texture_Pixel,
                    pos,
                    Vector2.Zero,
                    null,
                    Color.Lerp(color_WindowStart, color_WindowEnd, (float)i/numGradients),
                    0f,
                    scale_Window.X * Vector2.UnitX + stepSize * Vector2.UnitY,
                    0f,
                    RenderSpriteBlendMode.AbsoluteTop);

                pos.Y += stepSize;
            }

            switch (currentState)
            {
                case SaveGameState.Menu:
                    Draw_Menu();
                    break;
                case SaveGameState.LoadConfirm:
                    Draw_LoadConfirm();
                    break;
                case SaveGameState.Loading:
                    Draw_CenteredText(Resources.LoadSave_Loading);
                    break;
                case SaveGameState.SaveConfirm:
                    Draw_SaveConfirm();
                    break;
                case SaveGameState.Saving:
                    Draw_CenteredText(Resources.LoadSave_Saving);
                    break;
                default:
                    // Save/Load done
                    Draw_CenteredText(Resources.LoadSave_Done);
                    break;
            }
        }

        private void Draw_LoadConfirm()
        {
            render.Draw(
                texture_Pixel,
                screen_Center + position_SaveWindow,
                Vector2.Zero,
                null,
                color_SaveWindow,
                0f,
                scale_SaveWindow,
                0f,
                RenderSpriteBlendMode.AbsoluteTop);

            render.DrawString(
                font,
                Resources.LoadSave_LoadConfirm,
                screen_Center + offset_Title,
                color_Title,
                color_Shadow,
                offset_Shadow,
                RenderSpriteBlendMode.AbsoluteTop);

            if (LocalInstanceManager.CurrentGameState == GameState.SurvivalGame &&
                survivalSavedGame.MajorWaveNumber > 0)
            {
                render.DrawString(
                    font,
                    String.Format(Resources.LoadSave_FileFormat,
                        survivalSavedGame.MajorWaveNumber,
                        survivalSavedGame.Year,
                        survivalSavedGame.Month,
                        survivalSavedGame.Day,
                        survivalSavedGame.Hour,
                        survivalSavedGame.Minute),
                    screen_Center + offset_SaveData
                     + 1.8f * font.LineSpacing * Vector2.UnitY,
                    color_SaveData,
                    color_Shadow,
                    offset_Shadow,
                    RenderSpriteBlendMode.AbsoluteTop);
            }
            else if (LocalInstanceManager.CurrentGameState == GameState.CampaignGame &&
                campaignSavedGame.MajorWaveNumber > 0)
            {
                render.DrawString(
                    font,
                    String.Format(Resources.LoadSave_FileFormat,
                        campaignSavedGame.MajorWaveNumber,
                        campaignSavedGame.Year,
                        campaignSavedGame.Month,
                        campaignSavedGame.Day,
                        campaignSavedGame.Hour,
                        campaignSavedGame.Minute),
                    screen_Center + offset_SaveData
                     + 1.8f * font.LineSpacing * Vector2.UnitY,
                    color_SaveData,
                    color_Shadow,
                    offset_Shadow,
                    RenderSpriteBlendMode.AbsoluteTop);
            }

            Draw_YesNo();
        }

        private void Draw_CenteredText(string p)
        {
            Vector2 textSize = font.MeasureString(p);
            render.DrawString(
                font,
                p,
                screen_Center,
                color_Interface,
                color_Shadow,
                1f,
                textSize / 2f,
                offset_Shadow,
                RenderSpriteBlendMode.AbsoluteTop);
        }

        private void Draw_SaveConfirm()
        {
            render.Draw(
                texture_Pixel,
                screen_Center + position_SaveWindow,
                Vector2.Zero,
                null,
                color_SaveWindow,
                0f,
                scale_SaveWindow,
                0f,
                RenderSpriteBlendMode.AbsoluteTop);

            render.DrawString(
                font,
                Resources.LoadSave_SaveConfirm,
                screen_Center + offset_Title,
                color_Title,
                color_Shadow,
                offset_Shadow,
                RenderSpriteBlendMode.AbsoluteTop);

            if (LocalInstanceManager.CurrentGameState == GameState.SurvivalGame &&
                survivalSavedGame.MajorWaveNumber > 0)
            {
                render.DrawString(
                    font,
                    Resources.LoadSave_SaveOverwrite,
                    screen_Center + offset_SaveData,
                    color_SaveData,
                    color_Shadow,
                    offset_Shadow,
                    RenderSpriteBlendMode.AbsoluteTop);
                render.DrawString(
                    font,
                    String.Format(Resources.LoadSave_FileFormat,
                        survivalSavedGame.MajorWaveNumber,
                        survivalSavedGame.Year,
                        survivalSavedGame.Month,
                        survivalSavedGame.Day,
                        survivalSavedGame.Hour,
                        survivalSavedGame.Minute),
                    screen_Center + offset_SaveData
                     + 1.8f * font.LineSpacing * Vector2.UnitY,
                    color_SaveData,
                    color_Shadow,
                    offset_Shadow,
                    RenderSpriteBlendMode.AbsoluteTop);
            }
            else if (LocalInstanceManager.CurrentGameState == GameState.CampaignGame &&
                campaignSavedGame.MajorWaveNumber > 0)
            {
                render.DrawString(
                    font,
                    Resources.LoadSave_SaveOverwrite,
                    screen_Center + offset_SaveData,
                    color_SaveData,
                    color_Shadow,
                    offset_Shadow,
                    RenderSpriteBlendMode.AbsoluteTop);
                render.DrawString(
                    font,
                    String.Format(Resources.LoadSave_FileFormat,
                        campaignSavedGame.MajorWaveNumber,
                        campaignSavedGame.Year,
                        campaignSavedGame.Month,
                        campaignSavedGame.Day,
                        campaignSavedGame.Hour,
                        campaignSavedGame.Minute),
                    screen_Center + offset_SaveData
                     + 1.8f * font.LineSpacing * Vector2.UnitY,
                    color_SaveData,
                    color_Shadow,
                    offset_Shadow,
                    RenderSpriteBlendMode.AbsoluteTop);
            }

            Draw_YesNo();
        }

        private void Draw_YesNo()
        {
            render.Draw(
                texture_ButtonX,
                screen_Center + offset_Load,
                center_ButtonX,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AbsoluteTop);

            render.DrawString(
                font,
                Resources.LoadSave_Yes,
                screen_Center + offset_Load + Vector2.UnitX * texture_ButtonX.Width,
                color_Interface,
                color_Shadow,
                offset_Shadow,
                RenderSpriteBlendMode.AbsoluteTop);

            render.Draw(
                texture_ButtonB,
                screen_Center + offset_Save,
                center_ButtonB,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AbsoluteTop);
            render.DrawString(
                font,
                Resources.LoadSave_No,
                screen_Center + offset_Save + Vector2.UnitX * texture_ButtonB.Width,
                color_Interface,
                color_Shadow,
                offset_Shadow,
                RenderSpriteBlendMode.AbsoluteTop);

        }

        private void Draw_Menu()
        {

            render.Draw(
                texture_Pixel,
                screen_Center + position_SaveWindow,
                Vector2.Zero,
                null,
                color_SaveWindow,
                0f,
                scale_SaveWindow,
                0f,
                RenderSpriteBlendMode.AbsoluteTop);

            if (LocalInstanceManager.CurrentGameState == GameState.SurvivalGame)
            {
                render.DrawString(
                    font,
                    String.Format(Resources.LoadSave_Title, name_Survival),
                    screen_Center + offset_Title,
                    color_Title,
                    color_Shadow,
                    offset_Shadow,
                    RenderSpriteBlendMode.AbsoluteTop);
                if (survivalSavedGame.MajorWaveNumber > 0)
                {
                    render.DrawString(
                        font,
                        String.Format(Resources.LoadSave_FileFormat,
                            survivalSavedGame.MajorWaveNumber,
                            survivalSavedGame.Year,
                            survivalSavedGame.Month,
                            survivalSavedGame.Day,
                            survivalSavedGame.Hour,
                            survivalSavedGame.Minute),
                        screen_Center + offset_SaveData,
                        color_SaveData,
                        color_Shadow,
                        offset_Shadow,
                        RenderSpriteBlendMode.AbsoluteTop);
                }
                else
                {
                    render.DrawString(
                        font,
                        Resources.LoadSave_NoSave,
                        screen_Center + offset_SaveData,
                        color_SaveData,
                        color_Shadow,
                        offset_Shadow,
                        RenderSpriteBlendMode.AbsoluteTop);
                }
            }
            else
            {
                render.DrawString(
                    font,
                    String.Format(Resources.LoadSave_Title, name_Campaign),
                    screen_Center + offset_Title,
                    color_Title,
                    color_Shadow,
                    offset_Shadow,
                    RenderSpriteBlendMode.AbsoluteTop);

                if (campaignSavedGame.MajorWaveNumber > 0)
                {
                    render.DrawString(
                        font,
                        String.Format(Resources.LoadSave_FileFormat,
                            campaignSavedGame.MajorWaveNumber,
                            campaignSavedGame.Year,
                            campaignSavedGame.Month,
                            campaignSavedGame.Day,
                            campaignSavedGame.Hour,
                            campaignSavedGame.Minute),
                        screen_Center + offset_SaveData,
                        color_SaveData,
                        color_Shadow,
                        offset_Shadow,
                        RenderSpriteBlendMode.AbsoluteTop);
                }
                else
                {
                    render.DrawString(
                        font,
                        Resources.LoadSave_NoSave,
                        screen_Center + offset_SaveData,
                        color_SaveData,
                        color_Shadow,
                        offset_Shadow,
                        RenderSpriteBlendMode.AbsoluteTop);
                }

            }

            if ((LocalInstanceManager.CurrentGameState == GameState.SurvivalGame &&
                survivalSavedGame.MajorWaveNumber > 0) ||
                (LocalInstanceManager.CurrentGameState == GameState.CampaignGame &&
                campaignSavedGame.MajorWaveNumber > 0))
            {
                render.Draw(
                    texture_ButtonX,
                    screen_Center + offset_Load,
                    center_ButtonX,
                    null,
                    Color.White,
                    0f,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AbsoluteTop);

                render.DrawString(
                    font,
                    Resources.LoadSave_Load,
                    screen_Center + offset_Load + Vector2.UnitX * texture_ButtonX.Width,
                    color_Interface,
                    color_Shadow,
                    offset_Shadow,
                    RenderSpriteBlendMode.AbsoluteTop);
            }

            render.Draw(
                texture_ButtonY,
                screen_Center + offset_Save,
                center_ButtonY,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AbsoluteTop);
            render.DrawString(
                font,
                Resources.LoadSave_Save,
                screen_Center + offset_Save + Vector2.UnitX * texture_ButtonY.Width,
                color_Interface,
                color_Shadow,
                offset_Shadow,
                RenderSpriteBlendMode.AbsoluteTop);

            render.Draw(
                texture_ButtonB,
                screen_Center + offset_Cancel,
                center_ButtonB,
                null,
                Color.White,
                0f, 1f, 0f,
                RenderSpriteBlendMode.AbsoluteTop);
            render.DrawString(
                font,
                Resources.LoadSave_Cancel,
                screen_Center + offset_Cancel + Vector2.UnitX * texture_ButtonB.Width,
                color_Interface,
                color_Shadow,
                offset_Shadow,
                RenderSpriteBlendMode.AbsoluteTop);
        }
        #endregion
    }
}
