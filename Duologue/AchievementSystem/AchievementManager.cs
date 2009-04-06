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

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AchievementManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const string filename_Font = "Fonts/inero-28";
        private const string filename_Background = "Medals/background";
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
        private const float lifetime = 0.001f;

        private float iconVerticalSize = 80f;

        private float horizSpacing = 8f;
        private float vertSpacing = 5f;

        private float timePercent_FadeIn = 0.08f;
        private float timePercent_FadeOut = 0.85f;

        private float minSize = 0.7f;

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
            alpha_Achievement = 0;
            size_Achievement = 0;
            color_Text = Color.Bisque;
            color_Border = Color.SlateBlue;
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

            GenerateEnemyList();
            GenerateAchievements();

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
                Console.WriteLine(filename);

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

            Console.WriteLine(filename);

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
                }
            }
        }
        #endregion

        #region Other Public Methods
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
        #endregion

        #region Update / Draw
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (currentDisplayed != null)
            {
                timeSinceStart += (float)gameTime.ElapsedRealTime.TotalSeconds;
                if (timeSinceStart > lifetime)
                {
                    currentDisplayed.Displayed = true;
                    currentDisplayed = null;
                    size_Achievement = 0f;
                    alpha_Achievement = 0f;
                }
                else if (timeSinceStart / lifetime < timePercent_FadeIn)
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

            if (currentDisplayed != null)
            {
                Vector2 textSize = font.MeasureString(currentDisplayed.Name);
                float imageSize = iconVerticalSize / (float)currentDisplayed.Icon.Height;
                Vector2 borderSize = new Vector2(
                    (textSize.X + currentDisplayed.Icon.Width * imageSize + 3f * horizSpacing) / (float)texture_Background.Width,
                    (currentDisplayed.Icon.Height * imageSize + 2f * vertSpacing) / (float)texture_Background.Height);

                if (centerPos == Vector2.Zero)
                {
                    centerPos = new Vector2(
                        InstanceManager.DefaultViewport.Width / 2f - borderSize.X/2f,
                        (float)InstanceManager.DefaultViewport.TitleSafeArea.Bottom - borderSize.Y);
                }
                // Draw border
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
                    

                // Draw text
                render.DrawString(font,
                    currentDisplayed.Name,
                    centerPos
                        + Vector2.UnitX * (2f * horizSpacing + (float)currentDisplayed.Icon.Width * iconVerticalSize)
                        + Vector2.UnitY * vertSpacing,
                    new Color(color_Text, alpha_Achievement),
                    Vector2.One * size_Achievement,
                    Vector2.Zero,
                    RenderSpriteBlendMode.AbsoluteTop);

                // Draw icon
                render.Draw(
                    currentDisplayed.Icon,
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
            base.Draw(gameTime);
        }
        #endregion
    }
}