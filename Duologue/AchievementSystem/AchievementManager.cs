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
#endregion

namespace Duologue.AchievementSystem
{
    public enum PossibleMedals
    {
        HeavyRoller,
        Kilokillage,
        Exterminator,
        TourOfDuty,
        BFF,
        WetFeet,
        Experienced,
        NoEndInSight,
        KeyParty,
        Seriously,
    }

    public enum MedalCaseState
    {
        InitialPause,
        InitialFadeIn,
        ButtonFadeIn,
        Steady,
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AchievementManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string filename_Font = "Fonts/inero-small";
        private const string filename_FontTitle = "Fonts/inero-50";
        private const string filename_FontMedalName = "Fonts/inero-40";
        private const string filename_FontMedalDesc = "Fonts/inero-28";
        private const string filename_Background = "Medals/background";
        private const string filename_CaseForeground = "Medals/medal-case-background";
        private const string filename_CaseBackgrounds = "Medals/medal-case-l{0}";
        private const int number_CaseBackgrounds = 2;
        private const string filename_CaseUI = "Medals/medal-case-ui";
        private const string filename_BFF = "Medals/bff";
        private const string filename_Experienced = "Medals/experienced";
        private const string filename_Exterminator = "Medals/exterminator";
        private const string filename_HeavyRoller = "Medals/heavy_roller";
        private const string filename_KeyParty = "Medals/key_party";
        private const string filename_Kilokillage = "Medals/kilokillage";
        private const string filename_NoEndInSight = "Medals/no_end_in_sight";
        private const string filename_Seriously = "Medals/seriously";
        private const string filename_TourOfDuty = "Medals/tour_of_duty";
        private const string filename_WetFeet = "Medals/wet_feet";
        private const string filename_GreyExtension = "-grey";
        private const string filename_SavedData = "medal_data.sav";

        private const string filename_MenuMoveBeep = "Audio/PlayerEffects/menu_move_beep";

        private const string containerName = "Duologue";

        private const int possibleAchievements = 10;
        private const float lifetime = 3.5f;

        private const float iconVerticalSize = 80f;

        private const float horizSpacing = 8f;
        private const float vertSpacing = 5f;

        private const float spacing_HorizTextIcon = 20f;
        private const float spacing_VertTextBottomWindow = 100f;
        private const float spacing_VertTextToText = 15f;

        private const float timePercent_FadeIn = 0.15f;
        private const float timePercent_FadeOut = 0.9f;

        private const float minSize = 0.7f;

        private const float delta_LayerOffset = 0.32416f;
        private const float multiplyer_LayerOffset = 2.4f;

        private const float offsetX_MedalStart = 20f;
        private const float offsetY_MedalStart = 175f;

        private const float offsetX_UI = 435f + 7f;
        private const float offsetY_UI = 326.5f + 7f;

        private const float offsetY_MedalCaseTitle = -2f;

        private const float offsetX_MedalCaseIcon = -35f;
        private const float offsetY_MedalCaseIcon = -35f;
        private const float offset_IconShadow = 8f;

        private const float offsetY_Stats = -60f;

        private const int numberMedalsWide = 3;
        private const int numberMedalsHigh = 4;

        private const float offsetY_Medals = -300f;

        private const double time_FadeIn = 0.2;
        private const double time_Pause = 0.5;
        private const double time_PerButtonFade = 0.1;

        private const float volume_MenuBeep = 0.25f;

        #region Achievement Constants
        private const int number_Kilokillage = 1000;
        private const int number_Seriously = 39516;
        /// <summary>
        /// Past this point, we stop counting
        /// </summary>
        private const int number_MaxEnemiesKilled = 39999;
        #endregion
        #endregion

        #region Fields
        private SpriteFont font_MedalDisplay;
        private SpriteFont font_Title;
        private SpriteFont font_MedalName;
        private SpriteFont font_MedalDesc;
        private RenderSprite render;
        private Achievement[] achievements;
        private Queue<Achievement> unlockedYetToDisplay;
        private Vector2 centerPos;
        private float timeSinceStart;
        private Achievement currentDisplayed;
        private List<int> orderedAchievementList;
        private StorageDevice storageDevice;
        private bool storageDeviceIsSet;
        private AchievementData achievementData;
        private bool dataLoaded;
        private Texture2D texture_Background;
        private Texture2D texture_CaseForeground;
        private Texture2D[] texture_CaseBackgrounds;
        private float[] offset_CaseBackgrounds;
        private Texture2D texture_CaseUI;
        private Vector2 pos_ScreenCenter;
        private Vector2 pos_PositionUI;
        private Vector2 pos_MedalsStart;

        private MedalCaseState currentState;
        private double timer_MedalScreen;
        private int currentSelection;

        /// <summary>
        /// Since every play object in the game is not a destructable enemy,
        /// we have to have a lookup table for converting them to the proper
        /// index.
        /// </summary>
        private int[] enemyObjectLookupTable;
        private int maxNumEnemies;
        private int dataVersion;

        private float alpha_Achievement;
        private float size_Achievement;
        private Color color_Text;
        private Color color_TextLocked;
        private Color color_Border;
        private Color color_BorderLocked;
        private Color color_BorderSelected;
        private Vector2 textSize;
        private Vector2 borderSize;
        private float imageSize;

        private bool medalCaseScreen;

        // The following have to do with various UI elements on the medal case
        private Color color_Title;
        private Color color_MedalName;
        private Color color_MedalDesc;
        private Color color_Shadow;
        private Color color_Locked;
        private Color color_Stats;
        private Color color_ShadowStats;
        private Vector2 pos_FullMedalIcon;
        private Vector2 pos_Title;
        private Vector2 pos_MedalName;
        private Vector2 pos_MedalDesc;
        private Vector2 pos_Stats;
        private Vector2[] offset_Shadow;
        private Vector2[] offset_ShadowSmaller;

        private int numberOfEnemyTypesKilled;

        private SoundEffect sfx_MenuMove;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the current medal list
        /// </summary>
        public Achievement[] Medals
        {
            get { return achievements; }
        }
        #endregion

        #region Constructor / Init / Load
        public AchievementManager(Game game)
            : base(game)
        {
            achievements = new Achievement[possibleAchievements];
            orderedAchievementList = new List<int>(possibleAchievements);
            unlockedYetToDisplay = new Queue<Achievement>(possibleAchievements);
            storageDeviceIsSet = false;
            dataVersion = 9;
            alpha_Achievement = 1f;
            size_Achievement = 1f;
            color_Text = Color.Bisque;
            color_TextLocked = Color.Tomato;
            color_Border = Color.LightCyan;
            color_BorderLocked = Color.DarkSlateGray;
            color_BorderSelected = Color.OrangeRed;
            color_Title = Color.White;
            color_Shadow = new Color(Color.Black, 128);
            color_MedalName = Color.OldLace;
            color_MedalDesc = Color.LightCyan;
            color_Locked = Color.SaddleBrown;
            color_Stats = Color.DarkCyan;
            color_ShadowStats = Color.Cornsilk;
            offset_Shadow = new Vector2[]
            {
                Vector2.One,
                -1 * Vector2.One,
                new Vector2(-1,1),
                new Vector2(1,-1)
            };

            offset_ShadowSmaller = new Vector2[]
            {
                Vector2.One
            };
            medalCaseScreen = false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            timeSinceStart = 0f;
            centerPos = Vector2.Zero;

            base.Initialize();
        }
        protected override void LoadContent()
        {
            font_MedalDisplay = InstanceManager.AssetManager.LoadSpriteFont(filename_Font);
            font_Title = InstanceManager.AssetManager.LoadSpriteFont(filename_FontTitle);
            font_MedalName = InstanceManager.AssetManager.LoadSpriteFont(filename_FontMedalName);
            font_MedalDesc = InstanceManager.AssetManager.LoadSpriteFont(filename_FontMedalDesc);

            // Dear Glen, Sorry for the InstanceManager usage, this will go away next game, Love Sam
            sfx_MenuMove = InstanceManager.AssetManager.LoadSoundEffect(filename_MenuMoveBeep);

            texture_Background = InstanceManager.AssetManager.LoadTexture2D(filename_Background);
            texture_CaseForeground = InstanceManager.AssetManager.LoadTexture2D(filename_CaseForeground);
            texture_CaseUI = InstanceManager.AssetManager.LoadTexture2D(filename_CaseUI);

            texture_CaseBackgrounds = new Texture2D[number_CaseBackgrounds];
            offset_CaseBackgrounds = new float[number_CaseBackgrounds];
            for (int i = 0; i < number_CaseBackgrounds; i++)
            {
                texture_CaseBackgrounds[i] = InstanceManager.AssetManager.LoadTexture2D(
                    String.Format(filename_CaseBackgrounds, i));
                offset_CaseBackgrounds[i] = 0;
            }

            GenerateEnemyList();
            GenerateAchievements();

            // set the text size
            Vector2 temp;
            textSize = Vector2.Zero;
            for (int i = 0; i < achievements.Length; i++)
            {
                temp = font_MedalDisplay.MeasureString(achievements[i].Name);
                if (temp.X > textSize.X)
                    textSize = temp;
            }

            pos_ScreenCenter = Vector2.Zero;

            base.LoadContent();
        }

        public void InitStorageData(StorageDevice device)
        {
            storageDevice = device;
            storageDeviceIsSet = true;
            if (!Guide.IsTrialMode)
            {
                // Try to load the game
                if (!LoadAchievementData())
                {
                    // Let's creat a new save data
                    achievementData = new AchievementData();
                    achievementData.EnemyTypesKilled = new bool[maxNumEnemies];
                    for (int i = 0; i < maxNumEnemies; i++)
                    {
                        achievementData.EnemyTypesKilled[i] = false;
                    }
                    achievementData.MedalEarned = new bool[possibleAchievements];
                    for (int i = 0; i < possibleAchievements; i++)
                    {
                        achievementData.MedalEarned[i] = false;
                    }
                    achievementData.NumberOfEnemiesKilled = 0;

                    achievementData.DataVersion = dataVersion;

                    achievementData.NumberOfGamesPlayed = 0;

                    SaveAchievementData();
                    dataLoaded = true;
                }

                SyncUpAchievementDataAfterLoad();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Checks to see if the "menu down" controll was triggered
        /// </summary>
        private bool IsMenuDown()
        {
            bool pressed = false;

            for (int i = 0; i < InstanceManager.InputManager.CurrentGamePadStates.Length; i++)
            {
                if (InstanceManager.InputManager.CurrentKeyboardStates[i].IsKeyDown(Keys.Down) &&
                    InstanceManager.InputManager.LastKeyboardStates[i].IsKeyUp(Keys.Down))
                {
                    pressed = true;
                    break;
                }
                if ((InstanceManager.InputManager.CurrentGamePadStates[i].DPad.Down == ButtonState.Pressed &&
                    InstanceManager.InputManager.LastGamePadStates[i].DPad.Down == ButtonState.Released) ||
                   (InstanceManager.InputManager.CurrentGamePadStates[i].ThumbSticks.Left.Y < 0 &&
                    InstanceManager.InputManager.LastGamePadStates[i].ThumbSticks.Left.Y >= 0))
                {
                    pressed = true;
                    break;
                }
            }
            return pressed;
        }

        /// <summary>
        /// Checks to see if we wanna move left
        /// </summary>
        private bool IsMenuLeft()
        {
            bool pressed = false;

            for (int i = 0; i < InstanceManager.InputManager.CurrentGamePadStates.Length; i++)
            {
                if (InstanceManager.InputManager.CurrentKeyboardStates[i].IsKeyDown(Keys.Left) &&
                    InstanceManager.InputManager.LastKeyboardStates[i].IsKeyUp(Keys.Left))
                {
                    pressed = true;
                    break;
                }
                if ((InstanceManager.InputManager.CurrentGamePadStates[i].DPad.Left == ButtonState.Pressed &&
                    InstanceManager.InputManager.LastGamePadStates[i].DPad.Left == ButtonState.Released) ||
                   (InstanceManager.InputManager.CurrentGamePadStates[i].ThumbSticks.Left.X < 0 &&
                    InstanceManager.InputManager.LastGamePadStates[i].ThumbSticks.Left.X >= 0))
                {
                    pressed = true;
                    break;
                }
            }
            return pressed;
        }

        /// <summary>
        /// Checks to see if we wanna move left
        /// </summary>
        private bool IsMenuRight()
        {
            bool pressed = false;

            for (int i = 0; i < InstanceManager.InputManager.CurrentGamePadStates.Length; i++)
            {
                if (InstanceManager.InputManager.CurrentKeyboardStates[i].IsKeyDown(Keys.Right) &&
                    InstanceManager.InputManager.LastKeyboardStates[i].IsKeyUp(Keys.Right))
                {
                    pressed = true;
                    break;
                }
                if ((InstanceManager.InputManager.CurrentGamePadStates[i].DPad.Right == ButtonState.Pressed &&
                    InstanceManager.InputManager.LastGamePadStates[i].DPad.Right == ButtonState.Released) ||
                   (InstanceManager.InputManager.CurrentGamePadStates[i].ThumbSticks.Left.X > 0 &&
                    InstanceManager.InputManager.LastGamePadStates[i].ThumbSticks.Left.X <= 0))
                {
                    pressed = true;
                    break;
                }
            }
            return pressed;
        }

        /// <summary>
        /// Checks to see if the "menu up" control was triggered
        /// </summary>
        private bool IsMenuUp()
        {
            bool pressed = false;

            for (int i = 0; i < InstanceManager.InputManager.CurrentGamePadStates.Length; i++)
            {
                if (InstanceManager.InputManager.CurrentKeyboardStates[i].IsKeyDown(Keys.Up) &&
                    InstanceManager.InputManager.LastKeyboardStates[i].IsKeyUp(Keys.Up))
                {
                    pressed = true;
                    break;
                }
                if ((InstanceManager.InputManager.CurrentGamePadStates[i].DPad.Up == ButtonState.Pressed &&
                    InstanceManager.InputManager.LastGamePadStates[i].DPad.Up == ButtonState.Released) ||
                   (InstanceManager.InputManager.CurrentGamePadStates[i].ThumbSticks.Left.Y > 0 &&
                    InstanceManager.InputManager.LastGamePadStates[i].ThumbSticks.Left.Y <= 0))
                {
                    pressed = true;
                    break;
                }
            }
            return pressed;
        }

        private void SetPositions()
        {
            imageSize = iconVerticalSize / (float)achievements[0].Icon.Height;
            borderSize = new Vector2(
                (textSize.X + achievements[0].Icon.Width * imageSize + 3f * horizSpacing) / (float)texture_Background.Width,
                (achievements[0].Icon.Height * imageSize + 2f * vertSpacing) / (float)texture_Background.Height);

            pos_ScreenCenter = new Vector2(
                InstanceManager.DefaultViewport.Width / 2f,
                InstanceManager.DefaultViewport.Height / 2f);
            pos_PositionUI = new Vector2(
                pos_ScreenCenter.X - offsetX_UI,
                pos_ScreenCenter.Y - offsetY_UI);
            pos_MedalsStart = new Vector2(
                pos_PositionUI.X + offsetX_MedalStart,
                pos_PositionUI.Y + offsetY_MedalStart);

            Vector2 temp = font_Title.MeasureString(Resources.MedalCase_Title);
            pos_Title = new Vector2(
                pos_ScreenCenter.X - temp.X,
                pos_PositionUI.Y + offsetY_MedalCaseTitle);

            pos_FullMedalIcon = new Vector2(
                pos_ScreenCenter.X + offsetX_UI + offsetX_MedalCaseIcon,
                pos_ScreenCenter.Y + offsetY_UI + offsetY_MedalCaseIcon);

            pos_Stats = new Vector2(
                pos_MedalsStart.X,
                pos_ScreenCenter.Y + offsetY_UI + offsetY_Stats);

            pos_MedalName = new Vector2(
                pos_FullMedalIcon.X - achievements[0].IconGrey.Width - spacing_HorizTextIcon,
                pos_FullMedalIcon.Y - font_MedalName.LineSpacing - spacing_VertTextBottomWindow - spacing_VertTextToText - font_MedalDisplay.LineSpacing);
            pos_MedalDesc = new Vector2(
                pos_MedalName.X,
                pos_MedalName.Y + font_MedalName.LineSpacing + spacing_VertTextToText);
        }

        private void SyncUpAchievementDataAfterLoad()
        {
            for (int i = 0; i < possibleAchievements; i++)
            {
                achievements[i].Unlocked = achievementData.MedalEarned[i];
            }
        }

        private void SyncUpAchievementDataBeforeSave()
        {
            for (int i = 0; i < possibleAchievements; i++)
            {
                achievementData.MedalEarned[i] = achievements[i].Unlocked;
            }
        }

        private bool LoadAchievementData()
        {
            // Open a storage container.
            StorageContainer container =
                storageDevice.OpenContainer(containerName);

            // Get the path of the save game.
            string filename = Path.Combine(container.Path, filename_SavedData);

            // Check to see whether the save exists.
            if (File.Exists(filename))
            {
                InstanceManager.Logger.LogEntry("Medal data file:");
                InstanceManager.Logger.LogEntry(filename);

                // Open the file.
                FileStream stream = File.Open(filename, FileMode.OpenOrCreate,
                    FileAccess.Read);

                // Read the data from the file.
                XmlSerializer serializer = new XmlSerializer(typeof(AchievementData));
                try
                {
                    achievementData = (AchievementData)serializer.Deserialize(stream);
                }
                catch
                {
                    stream.Close();
                    container.Dispose();
                    dataLoaded = false;
                    return false;
                }

                // Close the file.
                stream.Close();

                // Dispose the container.
                container.Dispose();
                dataLoaded = true;
                if (achievementData.DataVersion == dataVersion)
                    return true;
                else
                    return false; // Yeah, so we nuke their medals... should only happen during development
            }
            else
            {
                container.Dispose();
                return false;
            }
        }

        private void SaveAchievementData()
        {
            // Open a storage container.
            StorageContainer container =
                storageDevice.OpenContainer(containerName);

            // Get the path of the save game.
            string filename = Path.Combine(container.Path, filename_SavedData);

            InstanceManager.Logger.LogEntry("Medal data file:");
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
            XmlSerializer serializer = new XmlSerializer(typeof(AchievementData));
            serializer.Serialize(stream, achievementData);

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();
        }

        private void GenerateEnemyList()
        {
            enemyObjectLookupTable = new int[25]; // Probably should be bigger than possible enemies to allow for growth

            // We need to only populate this list with the destructable enemy types
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_AnnMoeba] = 0;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Buzzsaw] = 1;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Ember] = 2;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Gloop] = 3;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_KingGloop] = 4;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Maggot] = 5;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Mirthworm] = 6;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_ProtoNora] = 7;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Pyre] = 8;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Roggles] = 9;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Spawner] = 10;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Spitter] = 11;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_StaticGloop] = 12;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_UncleanRot] = 13;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Wiggles] = 14;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_MetalTooth] = 15;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_MiniSaw] = 16;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_GloopPrince] = 17;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Flycket] = 18;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Flambi] = 19;
            enemyObjectLookupTable[(int)TypesOfPlayObjects.Enemy_Firefly] = 20;

            maxNumEnemies = 21;
        }

        private void GenerateAchievements()
        {
            // Rolled Score
            achievements[(int)PossibleMedals.HeavyRoller] = new Achievement(
                Resources.Medal_Name_HeavyRoller,
                Resources.Medal_Desc_HeavyRoller);
            achievements[(int)PossibleMedals.HeavyRoller].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_HeavyRoller);
            achievements[(int)PossibleMedals.HeavyRoller].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_HeavyRoller + filename_GreyExtension);
            achievements[(int)PossibleMedals.HeavyRoller].Weight = 1;
            orderedAchievementList.Add((int)PossibleMedals.HeavyRoller);

            // Kilokillage
            achievements[(int)PossibleMedals.Kilokillage] = new Achievement(
                Resources.Medal_Name_Kilokillage,
                Resources.Medal_Desc_Kilokillage);
            achievements[(int)PossibleMedals.Kilokillage].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_Kilokillage);
            achievements[(int)PossibleMedals.Kilokillage].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_Kilokillage + filename_GreyExtension);
            achievements[(int)PossibleMedals.Kilokillage].Weight = 2;
            orderedAchievementList.Add((int)PossibleMedals.Kilokillage);

            // Exterminator
            achievements[(int)PossibleMedals.Exterminator] = new Achievement(
                Resources.Medal_Name_Exterminator,
                Resources.Medal_Desc_Exterminator);
            achievements[(int)PossibleMedals.Exterminator].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_Exterminator);
            achievements[(int)PossibleMedals.Exterminator].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_Exterminator + filename_GreyExtension);
            achievements[(int)PossibleMedals.Exterminator].Weight = 3;
            orderedAchievementList.Add((int)PossibleMedals.Exterminator);

            // Tour of duty
            achievements[(int)PossibleMedals.TourOfDuty] = new Achievement(
                Resources.Medal_Name_TourOfDuty,
                Resources.Medal_Desc_TourOfDuty);
            achievements[(int)PossibleMedals.TourOfDuty].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_TourOfDuty);
            achievements[(int)PossibleMedals.TourOfDuty].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_TourOfDuty + filename_GreyExtension);
            achievements[(int)PossibleMedals.TourOfDuty].Weight = 4;
            orderedAchievementList.Add((int)PossibleMedals.TourOfDuty);

            // BFF
            achievements[(int)PossibleMedals.BFF] = new Achievement(
                Resources.Medal_Name_BFF,
                Resources.Medal_Desc_BFF);
            achievements[(int)PossibleMedals.BFF].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_BFF);
            achievements[(int)PossibleMedals.BFF].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_BFF + filename_GreyExtension);
            achievements[(int)PossibleMedals.BFF].Weight = 5;
            orderedAchievementList.Add((int)PossibleMedals.BFF);

            // Wet feet
            achievements[(int)PossibleMedals.WetFeet] = new Achievement(
                Resources.Medal_Name_WetFeet,
                Resources.Medal_Desc_WetFeet);
            achievements[(int)PossibleMedals.WetFeet].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_WetFeet);
            achievements[(int)PossibleMedals.WetFeet].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_WetFeet + filename_GreyExtension);
            achievements[(int)PossibleMedals.WetFeet].Weight = 6;
            orderedAchievementList.Add((int)PossibleMedals.WetFeet);

            // Experienced
            achievements[(int)PossibleMedals.Experienced] = new Achievement(
                Resources.Medal_Name_Experienced,
                Resources.Medal_Desc_Experienced);
            achievements[(int)PossibleMedals.Experienced].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_Experienced);
            achievements[(int)PossibleMedals.Experienced].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_Experienced + filename_GreyExtension);
            achievements[(int)PossibleMedals.Experienced].Weight = 7;
            orderedAchievementList.Add((int)PossibleMedals.Experienced);

            // No end in sight
            achievements[(int)PossibleMedals.NoEndInSight] = new Achievement(
                Resources.Medal_Name_NoEndInSight,
                Resources.Medal_Desc_NoEndInSight);
            achievements[(int)PossibleMedals.NoEndInSight].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_NoEndInSight);
            achievements[(int)PossibleMedals.NoEndInSight].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_NoEndInSight + filename_GreyExtension);
            achievements[(int)PossibleMedals.NoEndInSight].Weight = 8;
            orderedAchievementList.Add((int)PossibleMedals.NoEndInSight);

            // Key party
            achievements[(int)PossibleMedals.KeyParty] = new Achievement(
                Resources.Medal_Name_KeyParty,
                Resources.Medal_Desc_KeyParty);
            achievements[(int)PossibleMedals.KeyParty].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_KeyParty);
            achievements[(int)PossibleMedals.KeyParty].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_KeyParty + filename_GreyExtension);
            achievements[(int)PossibleMedals.KeyParty].Weight = 9;
            orderedAchievementList.Add((int)PossibleMedals.KeyParty);

            // Seriously?
            achievements[(int)PossibleMedals.Seriously] = new Achievement(
                Resources.Medal_Name_Seriously,
                Resources.Medal_Desc_Seriously);
            achievements[(int)PossibleMedals.Seriously].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_Seriously);
            achievements[(int)PossibleMedals.Seriously].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_Seriously + filename_GreyExtension);
            achievements[(int)PossibleMedals.Seriously].Weight = 10;
            orderedAchievementList.Add((int)PossibleMedals.Seriously);
        }
        #endregion

        #region Achievement calls
        /// <summary>
        /// Called when an achievement is unlocked
        /// </summary>
        /// <param name="i">The achievement enum</param>
        public void UnlockAchievement(PossibleMedals i)
        {
            if (!Guide.IsTrialMode)
            {
                int j = (int)i;
                if (!achievements[j].Unlocked)
                {
                    achievements[j].Displayed = false;
                    achievements[j].Unlocked = true;
                    unlockedYetToDisplay.Enqueue(achievements[j]);
                    this.Enabled = true;
                    this.Visible = true;
                    InstanceManager.Logger.LogEntry(String.Format(
                        "Unlocked medal: {0}", i.ToString()));
                }
            }
        }
        #endregion

        #region Other Public Methods
        /// <summary>
        /// Enable the medal screen
        /// </summary>
        public void EnableMedalScreen()
        {
            medalCaseScreen = true;
            currentState = MedalCaseState.InitialPause;
            timer_MedalScreen = 0;
            currentSelection = 0;
            alpha_Achievement = 1f;
            size_Achievement = 1f;
            // Compute the number of enemies killed
            numberOfEnemyTypesKilled = 0;
            for (int i = 0; i < maxNumEnemies; i++)
            {
                if (achievementData.EnemyTypesKilled[i])
                    numberOfEnemyTypesKilled++;
            }
        }

        /// <summary>
        /// Disable the medal screen
        /// </summary>
        public void DisableMedalScreen()
        {
            medalCaseScreen = false;
        }

        /// <summary>
        /// Call at the end of the game to save the achievement storage data
        /// </summary>
        public void SaveStorageData()
        {
            if (!Guide.IsTrialMode && storageDeviceIsSet && dataLoaded)
            {
                SyncUpAchievementDataBeforeSave();
                SaveAchievementData();
            }
        }

        /// <summary>
        /// Call this at the begining of a game to determine if we should display
        /// the tutorial or not.
        /// </summary>
        /// <param name="timesPlayedBeforeTutorialStops">How many times we should play the game before we stop
        /// showing the tutorial</param>
        /// <remarks>
        /// This is effectively equivalent for checking the following
        ///   (numberOfTimesWevePlayed <= timesPlayedBeforeTutorialStops)
        /// </remarks>
        public bool DisplayTutorial(int timesPlayedBeforeTutorialStops)
        {
            achievementData.NumberOfGamesPlayed++;
            if (achievementData.NumberOfGamesPlayed >= int.MaxValue - 100)
            {
                // OMG WALLHAX
                achievementData.NumberOfGamesPlayed = int.MaxValue - 100;
            }
            return (achievementData.NumberOfGamesPlayed <= timesPlayedBeforeTutorialStops);
        }

        /// <summary>
        /// Called when an enemy has been killed
        /// </summary>
        /// <param name="po">The enemy type that was killed</param>
        public void EnemyDeathCount(TypesOfPlayObjects po)
        {
            if (!Guide.IsTrialMode && dataLoaded)
            {
                achievementData.NumberOfEnemiesKilled++;
                if (achievementData.NumberOfEnemiesKilled > number_MaxEnemiesKilled)
                    achievementData.NumberOfEnemiesKilled = number_MaxEnemiesKilled;

                if (!achievementData.MedalEarned[(int)PossibleMedals.Kilokillage]
                    && achievementData.NumberOfEnemiesKilled >= number_Kilokillage)
                {
                    UnlockAchievement(PossibleMedals.Kilokillage);
                }

                if (!achievementData.MedalEarned[(int)PossibleMedals.Seriously]
                    && achievementData.NumberOfEnemiesKilled >= number_Seriously)
                {
                    UnlockAchievement(PossibleMedals.Seriously);
                }
                if (!achievementData.EnemyTypesKilled[enemyObjectLookupTable[(int)po]])
                {
                    achievementData.EnemyTypesKilled[enemyObjectLookupTable[(int)po]] = true;
                    // See if that was all we were lacking
                    int enemyCount = 0;
                    for (int i = 0; i < maxNumEnemies; i++)
                    {
                        if (achievementData.EnemyTypesKilled[i])
                            enemyCount++;
                    }
                    if (enemyCount >= maxNumEnemies)
                        UnlockAchievement(PossibleMedals.Exterminator);
                }
            }
        }

        /// <summary>
        /// Draw a medal on the screen
        /// </summary>
        /// <param name="medal">The medal to display</param>
        /// <param name="position">The position of the medal. X will be the middle of the medal, Y will be the upper limit</param>
        public void DrawMedal(Achievement medal, Vector2 position)
        {
            DrawMedal(medal, position, false);
        }

        /// <summary>
        /// Draw a medal on the screen
        /// </summary>
        /// <param name="medal">The medal to display</param>
        /// <param name="position">The position of the medal. X will be the middle of the medal, Y will be the upper limit</param>
        /// <param name="isSelected">True if the medal has been selected</param>
        public void DrawMedal(Achievement medal, Vector2 position, bool isSelected)
        {
            imageSize = iconVerticalSize / (float)medal.Icon.Height;

            centerPos = new Vector2(
                position.X - (float)texture_Background.Width * borderSize.X / 2f,
                position.Y - (float)texture_Background.Height * borderSize.Y);

            // Draw border
            if (isSelected)
            {
                render.Draw(
                    texture_Background,
                    centerPos,
                    Vector2.Zero,
                    null,
                    new Color(color_BorderSelected, alpha_Achievement),
                    0f,
                    borderSize * size_Achievement,
                    0f,
                    RenderSpriteBlendMode.AbsoluteTop);
            }
            else if (medal.Unlocked)
            {
                render.Draw(
                    texture_Background,
                    centerPos,
                    Vector2.Zero,
                    null,
                    new Color(color_Border, alpha_Achievement),
                    0f,
                    borderSize * size_Achievement,
                    0f,
                    RenderSpriteBlendMode.AbsoluteTop);
            }
            else
            {
                render.Draw(
                    texture_Background,
                    centerPos,
                    Vector2.Zero,
                    null,
                    new Color(color_BorderLocked, alpha_Achievement),
                    0f,
                    borderSize * size_Achievement,
                    0f,
                    RenderSpriteBlendMode.AbsoluteTop);
            }

            // Draw icon
            if (medal.Unlocked)
            {
                render.Draw(
                    medal.Icon,
                    centerPos
                        + Vector2.UnitX * horizSpacing
                        + Vector2.UnitY * vertSpacing,
                    Vector2.Zero,
                    null,
                    new Color(Color.White, alpha_Achievement),
                    0,
                    size_Achievement * imageSize,
                    0,
                    RenderSpriteBlendMode.AbsoluteTop);
                // Draw text
                render.DrawString(font_MedalDisplay,
                    medal.Name,
                    centerPos
                        + Vector2.UnitX * (2f * horizSpacing + (float)medal.Icon.Width * size_Achievement * imageSize)
                        + Vector2.UnitY * (vertSpacing + (float)medal.Icon.Height * size_Achievement * imageSize * 0.5f - textSize.Y / 2f),
                    new Color(color_Text, alpha_Achievement),
                    Vector2.One * size_Achievement,
                    Vector2.Zero,
                    RenderSpriteBlendMode.AbsoluteTop);
            }
            else
            {
                render.Draw(
                    medal.IconGrey,
                    centerPos
                        + Vector2.UnitX * horizSpacing
                        + Vector2.UnitY * vertSpacing,
                    Vector2.Zero,
                    null,
                    new Color(Color.White, alpha_Achievement),
                    0,
                    size_Achievement * imageSize,
                    0,
                    RenderSpriteBlendMode.AbsoluteTop);
                // Draw text
                render.DrawString(font_MedalDisplay,
                    medal.Name,
                    centerPos
                        + Vector2.UnitX * (2f * horizSpacing + (float)medal.Icon.Width * size_Achievement * imageSize)
                        + Vector2.UnitY * (vertSpacing + (float)medal.Icon.Height * size_Achievement * imageSize * 0.5f - textSize.Y / 2f),
                    new Color(color_TextLocked, alpha_Achievement),
                    Vector2.One * size_Achievement,
                    Vector2.Zero,
                    RenderSpriteBlendMode.AbsoluteTop);
            }
        }
        #endregion

        #region Update / Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (medalCaseScreen)
                UpdateCaseScreen(gameTime);
            if (currentDisplayed != null)
            {
                timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timeSinceStart > lifetime)
                {
                    currentDisplayed.Displayed = true;
                    currentDisplayed = null;
                    timeSinceStart = 0;
                }
                else
                {
                    if (timeSinceStart / lifetime < timePercent_FadeIn)
                    {
                        alpha_Achievement = (timeSinceStart / lifetime) / timePercent_FadeIn;
                        size_Achievement = (1f - minSize) * (timeSinceStart / lifetime) / timePercent_FadeIn + minSize;
                    }
                    else if (timeSinceStart / lifetime > timePercent_FadeOut)
                    {
                        alpha_Achievement = 1f - (timeSinceStart / lifetime);
                        size_Achievement = 1f;
                    }
                    else
                    {
                        size_Achievement = 1f;
                        alpha_Achievement = 1f;
                    }
                }
            }
            else if (unlockedYetToDisplay.Count > 0)
            {
                currentDisplayed = unlockedYetToDisplay.Dequeue();
                timeSinceStart = 0f;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (render == null)
                render = InstanceManager.RenderSprite;

            if (pos_ScreenCenter == Vector2.Zero)
                SetPositions();

            if (medalCaseScreen)
                DrawCaseScreen(gameTime);

            if (currentDisplayed != null && currentDisplayed.Displayed == false)
            {
                DrawMedal(
                    currentDisplayed,
                    new Vector2(
                        InstanceManager.DefaultViewport.Width / 2f,
                        (float)InstanceManager.DefaultViewport.TitleSafeArea.Bottom));
           }
            base.Draw(gameTime);
        }
        #endregion

        #region Private update/draw
        private void UpdateCaseScreen(GameTime gameTime)
        {
            timer_MedalScreen += gameTime.ElapsedGameTime.TotalSeconds;

            //Console.WriteLine(timer_MedalScreen);

            switch (currentState)
            {
                case MedalCaseState.InitialFadeIn:
                    if (timer_MedalScreen > time_FadeIn)
                    {
                        timer_MedalScreen = 0;
                        currentState = MedalCaseState.ButtonFadeIn;
                    }
                    break;
                case MedalCaseState.ButtonFadeIn:
                    if (timer_MedalScreen > time_PerButtonFade)
                    {
                        timer_MedalScreen = 0;
                        currentSelection++;
                        if (currentSelection > achievements.Length - 1)
                        {
                            currentSelection = 0;
                            currentState = MedalCaseState.Steady;
                        }
                    }
                    break;
                case MedalCaseState.InitialPause:
                    if (timer_MedalScreen > time_Pause)
                    {
                        timer_MedalScreen = 0;
                        currentSelection = 0;
                        currentState = MedalCaseState.InitialFadeIn;
                    }
                    break;
                default:
                    break;
            }

            for (int i = 0; i < number_CaseBackgrounds; i++)
            {
                offset_CaseBackgrounds[i] += delta_LayerOffset * multiplyer_LayerOffset * (i + 1);
                if (offset_CaseBackgrounds[i] > texture_CaseBackgrounds[i].Width)
                    offset_CaseBackgrounds[i] = 0;
                else if (offset_CaseBackgrounds[i] < 0)
                    offset_CaseBackgrounds[i] = (float)texture_CaseBackgrounds[i].Width;
            }

            int lastSelection = currentSelection;
            if (InstanceManager.InputManager.NewButtonPressed(Buttons.Back) ||
                InstanceManager.InputManager.NewButtonPressed(Buttons.B))
            {
                LocalInstanceManager.CurrentGameState = LocalInstanceManager.NextGameState;
            }
            else if (IsMenuUp())
            {
                currentSelection -= numberMedalsWide;
            }
            else if (IsMenuDown())
            {
                currentSelection += numberMedalsWide;
            }
            else if (IsMenuLeft())
            {
                currentSelection--;
            }
            else if (IsMenuRight())
            {
                currentSelection++;
            }

            if (currentSelection < 0)
                currentSelection = 0;
            else if (currentSelection > achievements.Length - 1)
                currentSelection = achievements.Length - 1;

            if (lastSelection != currentSelection)
                sfx_MenuMove.Play(volume_MenuBeep);
        }

        private void DrawCaseScreen(GameTime gameTime)
        {
            for (int i = 0; i < number_CaseBackgrounds; i++)
            {
                render.Draw(
                    texture_CaseBackgrounds[i],
                    Vector2.Zero,
                    Vector2.UnitX * offset_CaseBackgrounds[i],
                    null,
                    Color.White,
                    0f,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);

                render.Draw(
                    texture_CaseBackgrounds[i],
                    Vector2.UnitX * ((float)texture_CaseBackgrounds[i].Width - offset_CaseBackgrounds[i]),
                    Vector2.Zero,
                    null,
                    Color.White,
                    0f,
                    1f,
                    0f,
                    RenderSpriteBlendMode.AlphaBlend);
            }
            render.Draw(
                texture_CaseForeground,
                Vector2.Zero,
                Vector2.Zero,
                null,
                Color.White,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.Multiplicative);

            float x = 0;
            float y = 0;
            int w = 1;

            switch (currentState)
            {
                case MedalCaseState.InitialFadeIn:
                    DrawUIBase(new Color(Color.White,
                            (float)timer_MedalScreen / (float)time_FadeIn));
                    break;
                case MedalCaseState.ButtonFadeIn:
                    DrawUIBase(Color.White);
                    for (int i = 0; i < currentSelection; i++)
                    {

                        alpha_Achievement = 1f;
                        DrawMedal(achievements[i],
                            pos_MedalsStart + Vector2.UnitX * x + 
                            Vector2.UnitY * y + 
                            Vector2.UnitX * borderSize.X * (float)texture_Background.Width / 2f);
                        x += borderSize.X * (float)texture_Background.Width + horizSpacing;
                        w++;
                        if (w > numberMedalsWide)
                        {
                            x = 0;
                            w = 1;
                            y += borderSize.Y * (float)texture_Background.Height + vertSpacing;
                        }
                    }
                    alpha_Achievement = (float)timer_MedalScreen / (float)time_PerButtonFade;
                    DrawMedal(achievements[currentSelection],
                        pos_MedalsStart + Vector2.UnitX * x + 
                        Vector2.UnitY * y + 
                        Vector2.UnitX * borderSize.X * (float)texture_Background.Width / 2f);
                    break;
                case MedalCaseState.InitialPause:
                    break;
                default:
                    alpha_Achievement = 1f;
                    for (int i = 0; i < achievements.Length; i++)
                    {

                        DrawMedal(achievements[i],
                            pos_MedalsStart + Vector2.UnitX * x +
                            Vector2.UnitY * y +
                            Vector2.UnitX * borderSize.X * (float)texture_Background.Width / 2f,
                            currentSelection == i);
                        x += borderSize.X * (float)texture_Background.Width + horizSpacing;
                        w++;
                        if (w > numberMedalsWide)
                        {
                            x = 0;
                            w = 1;
                            y += borderSize.Y * (float)texture_Background.Height + vertSpacing;
                        }
                    }
                    DrawUIBase(Color.White);
                    // Draw the title
                    render.DrawString(
                        font_Title,
                        Resources.MedalCase_Title,
                        pos_Title,
                        color_Title,
                        color_Shadow,
                        offset_Shadow,
                        RenderSpriteBlendMode.AbsoluteTop);
                    // Draw the icon
                    render.Draw(
                        achievements[currentSelection].IconGrey,
                        pos_FullMedalIcon + Vector2.One * offset_IconShadow,
                        new Vector2(
                            achievements[currentSelection].IconGrey.Width,
                            achievements[currentSelection].IconGrey.Height),
                        null,
                        color_Shadow,
                        0f,
                        1f,
                        0f,
                        RenderSpriteBlendMode.AbsoluteTop);
                    if (achievements[currentSelection].Unlocked)
                    {
                        render.Draw(
                            achievements[currentSelection].Icon,
                            pos_FullMedalIcon,
                            new Vector2(
                                achievements[currentSelection].Icon.Width,
                                achievements[currentSelection].Icon.Height),
                            null,
                            Color.White,
                            0f,
                            1f,
                            0f,
                            RenderSpriteBlendMode.AbsoluteTop);
                    }
                    else
                    {
                        render.Draw(
                            achievements[currentSelection].IconGrey,
                            pos_FullMedalIcon,
                            new Vector2(
                                achievements[currentSelection].IconGrey.Width,
                                achievements[currentSelection].IconGrey.Height),
                            null,
                            Color.White,
                            0f,
                            1f,
                            0f,
                            RenderSpriteBlendMode.AbsoluteTop);
                    }
                    // Draw the text
                    Vector2 temp = font_MedalName.MeasureString(achievements[currentSelection].Name);
                    render.DrawString(
                        font_MedalName,
                        achievements[currentSelection].Name,
                        pos_MedalName - Vector2.UnitX * temp.X,
                        color_MedalName,
                        color_Shadow,
                        offset_Shadow,
                        RenderSpriteBlendMode.AbsoluteTop);
                    temp = font_MedalDesc.MeasureString(achievements[currentSelection].Description);
                    render.DrawString(
                        font_MedalDesc,
                        achievements[currentSelection].Description,
                        pos_MedalDesc - Vector2.UnitX * temp.X,
                        color_MedalDesc,
                        color_Shadow,
                        offset_Shadow,
                        RenderSpriteBlendMode.AbsoluteTop);
                    if (Guide.IsTrialMode)
                    {
                        temp = font_MedalDesc.MeasureString(Resources.MedalCase_TrialMode);
                        render.DrawString(
                            font_MedalDisplay,
                            Resources.MedalCase_TrialMode,
                            pos_MedalDesc - Vector2.UnitX * temp.X + Vector2.UnitY * (font_MedalDesc.LineSpacing + spacing_VertTextToText),
                            color_Locked,
                            RenderSpriteBlendMode.AbsoluteTop);
                    }
                    else if (!achievements[currentSelection].Unlocked)
                    {
                        temp = font_MedalDesc.MeasureString(Resources.MedalCase_Locked);
                        render.DrawString(
                            font_MedalDisplay,
                            Resources.MedalCase_Locked,
                            pos_MedalDesc - Vector2.UnitX * temp.X + Vector2.UnitY * (font_MedalDesc.LineSpacing + spacing_VertTextToText),
                            color_Locked,
                            RenderSpriteBlendMode.AbsoluteTop);
                    }
                    // Draw the stats
                    if (achievementData.NumberOfEnemiesKilled < number_MaxEnemiesKilled)
                    {
                        render.DrawString(
                            font_MedalDisplay,
                            String.Format(Resources.MedalCase_Stats,
                                numberOfEnemyTypesKilled,
                                maxNumEnemies,
                                achievementData.NumberOfEnemiesKilled),
                            pos_Stats,
                            color_Stats,
                            color_ShadowStats,
                            offset_ShadowSmaller,
                            RenderSpriteBlendMode.AbsoluteTop);
                    }
                    else
                    {
                        // Woot easter egg!
                        render.DrawString(
                            font_MedalDisplay,
                            String.Format(Resources.MedalCase_Stats, 
                                numberOfEnemyTypesKilled,
                                maxNumEnemies,
                                Resources.MedalCase_Potato),
                            pos_Stats,
                            color_Stats,
                            color_ShadowStats,
                            offset_ShadowSmaller,
                            RenderSpriteBlendMode.AbsoluteTop);

                    }
                    break;
            }
        }

        private void DrawUIBase(Color color)
        {
            render.Draw(
                texture_CaseUI,
                pos_PositionUI,
                Vector2.Zero,
                null,
                color,
                0f,
                1f,
                0f,
                RenderSpriteBlendMode.AlphaBlendTop);
        }
        #endregion
    }
}