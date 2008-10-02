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

namespace Duologue.AchievementManager
{
    public enum Achievements
    {
        RolledScore,
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AchievementManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        private const int possibleAchievements = 20;
        private const float lifetime = 1f;
        #endregion

        #region Fields
        private SpriteFont font;
        private RenderSprite render;
        private Achievement[] achievements;
        private Queue<Achievement> unlockedYetToDisplay;
        private Vector2 centerPos;
        private float timeSinceStart;
        private Achievement currentDisplayed;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init / Load
        public AchievementManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            achievements = new Achievement[possibleAchievements];
            unlockedYetToDisplay = new Queue<Achievement>(possibleAchievements);
            timeSinceStart = 0f;
            centerPos = Vector2.Zero;
            GenerateAchievements();

            base.Initialize();
        }
        protected override void LoadContent()
        {
            font = InstanceManager.AssetManager.LoadSpriteFont("inero-28");
            base.LoadContent();
        }
        #endregion

        #region Private Methods
        private void GenerateAchievements()
        {
            // FIXME
            // When we do this "for real" we want to pre-populate this with achievements
            // the player already has

            // Rolled Score
            achievements[(int)Achievements.RolledScore] = new Achievement(
                Resources.Achievement_Name_RolledScore,
                Resources.Achievement_Desc_RolledScore,
                int.Parse(Resources.Achievement_Points_RolledScore));
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
            UnlockAchievement(Achievements.RolledScore);
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
                        InstanceManager.DefaultViewport.Width / 2f,
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