#region File Description
#endregion

#region Using statements
using System;
using System.Collections.Generic;
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

        private const int possibleAchievements = 20;
        private const float lifetime = 0.001f;
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
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public AchievementManager(Game game)
            : base(game)
        {
            achievements = new Achievement[possibleAchievements];
            orderedAchievementList = new List<int>(possibleAchievements);
            unlockedYetToDisplay = new Queue<Achievement>(possibleAchievements);
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

            GenerateAchievements();

            base.LoadContent();
        }

        public void InitSaveGame(StorageDevice device)
        {
        }
        #endregion

        #region Private Methods
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

        /// <summary>
        /// Internal- called when an achievement is unlocked
        /// </summary>
        /// <param name="i">The achievement enum</param>
        private void UnlockAchievement(Achievements i)
        {
            int j = (int)i;
            if (!achievements[j].Unlocked)
            {
                achievements[j].Displayed = false;
                achievements[j].Unlocked = true;
                unlockedYetToDisplay.Enqueue(achievements[j]);
            }
        }
        #endregion

        #region Achievement calls
        /// <summary>
        /// Unlock "Rolled Score" achievement
        /// </summary>
        public void AchievementRolledScore()
        {
            UnlockAchievement(Achievements.HeavyRoller);
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
                Vector2 size = font.MeasureString(currentDisplayed.Name);
                if (centerPos == Vector2.Zero)
                {
                    centerPos = new Vector2(
                        InstanceManager.DefaultViewport.Width / 2f - size.X/2f,
                        (float)InstanceManager.DefaultViewport.Height - 2 * size.Y);
                }
                render.DrawString(font,
                    currentDisplayed.Name,
                    centerPos,
                    Color.White);
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}