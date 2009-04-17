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
    public enum Achievements
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

        private const string containerName = "Duologue";

        private const int possibleAchievements = 10;
        private const float lifetime = 0.033f;

        private const float iconVerticalSize = 80f;

        private const float horizSpacing = 8f;
        private const float vertSpacing = 5f;

        private const float timePercent_FadeIn = 0.15f;
        private const float timePercent_FadeOut = 0.9f;

        private const float minSize = 0.7f;

        private const float delta_LayerOffset = 0.32416f;
        private const float multiplyer_LayerOffset = 2.4f;

        private const float offsetX_MedalStart = 20f;
        private const float offsetY_MedalStart = 175f;

        private const float offsetX_UI = 435f + 7f;
        private const float offsetY_UI = 326.5f + 7f;

        private const int numberMedalsWide = 3;
        private const int numberMedalsHigh = 4;

        private const float offsetY_Medals = -300f;

        private const double time_FadeIn = 0.2;
        private const double time_Pause = 0.5;
        private const double time_PerButtonFade = 0.1;

        #region Achievement Constants
        private const int number_Kilokillage = 1000;
        private const int number_Seriously = 39516;
        #endregion
        #endregion

        #region Fields
        private SpriteFont font;
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
        private Color color_Border;
        private Color color_BorderLocked;
        private Color color_BorderSelected;
        private Vector2 textSize;
        private Vector2 borderSize;
        private float imageSize;

        private bool medalCaseScreen;
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
            dataVersion = 2;
            alpha_Achievement = 1f;
            size_Achievement = 1f;
            color_Text = Color.Bisque;
            color_Border = Color.White;
            color_BorderLocked = Color.DarkSlateGray;
            color_BorderSelected = Color.OrangeRed;
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
            font = InstanceManager.AssetManager.LoadSpriteFont(filename_Font);
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
                temp = font.MeasureString(achievements[i].Name);
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

                    SaveAchievementData();
                    dataLoaded = true;
                }

                SyncUpAchievementDataAfterLoad();
            }
        }
        #endregion

        #region Private Methods
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
                /*
                pos_ScreenCenter.X - 
                (numberMedalsWide * borderSize.X * (float)texture_Background.Width + (numberMedalsWide - 1) * horizSpacing) / 2f,
                pos_ScreenCenter.Y - 
                (numberMedalsHigh * borderSize.Y * (float)texture_Background.Height + (numberMedalsHigh - 1) * vertSpacing) / 2f + offsetY_Medals);
                 */
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

            InstanceManager.Logger.LogEntry(filename);

            // Open the file, creating it if necessary.
            FileStream stream = File.Open(filename, FileMode.OpenOrCreate);

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

            maxNumEnemies = 15;
        }

        private void GenerateAchievements()
        {
            // Rolled Score
            achievements[(int)Achievements.HeavyRoller] = new Achievement(
                Resources.Medal_Name_HeavyRoller,
                Resources.Medal_Desc_HeavyRoller);
            achievements[(int)Achievements.HeavyRoller].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_HeavyRoller);
            achievements[(int)Achievements.HeavyRoller].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_HeavyRoller + filename_GreyExtension);
            achievements[(int)Achievements.HeavyRoller].Weight = 1;
            orderedAchievementList.Add((int)Achievements.HeavyRoller);

            // Kilokillage
            achievements[(int)Achievements.Kilokillage] = new Achievement(
                Resources.Medal_Name_Kilokillage,
                Resources.Medal_Desc_Kilokillage);
            achievements[(int)Achievements.Kilokillage].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_Kilokillage);
            achievements[(int)Achievements.Kilokillage].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_Kilokillage + filename_GreyExtension);
            achievements[(int)Achievements.Kilokillage].Weight = 2;
            orderedAchievementList.Add((int)Achievements.Kilokillage);

            // Exterminator
            achievements[(int)Achievements.Exterminator] = new Achievement(
                Resources.Medal_Name_Exterminator,
                Resources.Medal_Desc_Exterminator);
            achievements[(int)Achievements.Exterminator].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_Exterminator);
            achievements[(int)Achievements.Exterminator].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_Exterminator + filename_GreyExtension);
            achievements[(int)Achievements.Exterminator].Weight = 3;
            orderedAchievementList.Add((int)Achievements.Exterminator);

            // Tour of duty
            achievements[(int)Achievements.TourOfDuty] = new Achievement(
                Resources.Medal_Name_TourOfDuty,
                Resources.Medal_Desc_TourOfDuty);
            achievements[(int)Achievements.TourOfDuty].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_TourOfDuty);
            achievements[(int)Achievements.TourOfDuty].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_TourOfDuty + filename_GreyExtension);
            achievements[(int)Achievements.TourOfDuty].Weight = 4;
            orderedAchievementList.Add((int)Achievements.TourOfDuty);

            // BFF
            achievements[(int)Achievements.BFF] = new Achievement(
                Resources.Medal_Name_BFF,
                Resources.Medal_Desc_BFF);
            achievements[(int)Achievements.BFF].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_BFF);
            achievements[(int)Achievements.BFF].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_BFF + filename_GreyExtension);
            achievements[(int)Achievements.BFF].Weight = 5;
            orderedAchievementList.Add((int)Achievements.BFF);

            // Wet feet
            achievements[(int)Achievements.WetFeet] = new Achievement(
                Resources.Medal_Name_WetFeet,
                Resources.Medal_Desc_WetFeet);
            achievements[(int)Achievements.WetFeet].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_WetFeet);
            achievements[(int)Achievements.WetFeet].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_WetFeet + filename_GreyExtension);
            achievements[(int)Achievements.WetFeet].Weight = 6;
            orderedAchievementList.Add((int)Achievements.WetFeet);

            // Experienced
            achievements[(int)Achievements.Experienced] = new Achievement(
                Resources.Medal_Name_Experienced,
                Resources.Medal_Desc_Experienced);
            achievements[(int)Achievements.Experienced].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_Experienced);
            achievements[(int)Achievements.Experienced].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_Experienced + filename_GreyExtension);
            achievements[(int)Achievements.Experienced].Weight = 7;
            orderedAchievementList.Add((int)Achievements.Experienced);

            // No end in sight
            achievements[(int)Achievements.NoEndInSight] = new Achievement(
                Resources.Medal_Name_NoEndInSight,
                Resources.Medal_Desc_NoEndInSight);
            achievements[(int)Achievements.NoEndInSight].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_NoEndInSight);
            achievements[(int)Achievements.NoEndInSight].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_NoEndInSight + filename_GreyExtension);
            achievements[(int)Achievements.NoEndInSight].Weight = 8;
            orderedAchievementList.Add((int)Achievements.NoEndInSight);

            // Key party
            achievements[(int)Achievements.KeyParty] = new Achievement(
                Resources.Medal_Name_KeyParty,
                Resources.Medal_Desc_KeyParty);
            achievements[(int)Achievements.KeyParty].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_KeyParty);
            achievements[(int)Achievements.KeyParty].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_KeyParty + filename_GreyExtension);
            achievements[(int)Achievements.KeyParty].Weight = 9;
            orderedAchievementList.Add((int)Achievements.KeyParty);

            // Seriously?
            achievements[(int)Achievements.Seriously] = new Achievement(
                Resources.Medal_Name_Seriously,
                Resources.Medal_Desc_Seriously);
            achievements[(int)Achievements.Seriously].Icon =
                InstanceManager.AssetManager.LoadTexture2D(filename_Seriously);
            achievements[(int)Achievements.Seriously].IconGrey =
                InstanceManager.AssetManager.LoadTexture2D(filename_Seriously + filename_GreyExtension);
            achievements[(int)Achievements.Seriously].Weight = 10;
            orderedAchievementList.Add((int)Achievements.Seriously);
        }
        #endregion

        #region Achievement calls
        /// <summary>
        /// Called when an achievement is unlocked
        /// </summary>
        /// <param name="i">The achievement enum</param>
        public void UnlockAchievement(Achievements i)
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
        /// Called when an enemy has been killed
        /// </summary>
        /// <param name="po">The enemy type that was killed</param>
        public void EnemyDeathCount(TypesOfPlayObjects po)
        {
            if (!Guide.IsTrialMode && dataLoaded)
            {
                achievementData.NumberOfEnemiesKilled++;
                if (!achievementData.MedalEarned[(int)Achievements.Kilokillage]
                    && achievementData.NumberOfEnemiesKilled >= number_Kilokillage)
                {
                    UnlockAchievement(Achievements.Kilokillage);
                }

                if (!achievementData.MedalEarned[(int)Achievements.Seriously]
                    && achievementData.NumberOfEnemiesKilled >= number_Seriously)
                {
                    UnlockAchievement(Achievements.Seriously);
                }
                if (!achievementData.EnemyTypesKilled[enemyObjectLookupTable[(int)po]])
                {
                    achievementData.EnemyTypesKilled[enemyObjectLookupTable[(int)po]] = true;
                    // See if that was all we were lacking
                    int enemyCount = 0;
                    for (int i = 0; i < maxNumEnemies; i++)
                    {
                        if (!achievementData.EnemyTypesKilled[i])
                            enemyCount++;
                    }
                    if (enemyCount > 0)
                        UnlockAchievement(Achievements.Exterminator);
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


            // Draw text
            render.DrawString(font,
                medal.Name,
                centerPos
                    + Vector2.UnitX * (2f * horizSpacing + (float)medal.Icon.Width * size_Achievement * imageSize)
                    + Vector2.UnitY * (vertSpacing + (float)medal.Icon.Height * size_Achievement * imageSize * 0.5f - textSize.Y / 2f),
                new Color(color_Text, alpha_Achievement),
                Vector2.One * size_Achievement,
                Vector2.Zero,
                RenderSpriteBlendMode.AbsoluteTop);

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
                timeSinceStart += (float)gameTime.ElapsedRealTime.TotalSeconds;
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

            if (InstanceManager.InputManager.NewButtonPressed(Buttons.Back))
            {
                LocalInstanceManager.CurrentGameState = LocalInstanceManager.NextGameState;
            }
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