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
using Duologue.Audio; // BAH! IService and SL need to be in Mimicware space FIXME
#endregion

namespace Mimicware.Fx
{
    /// <summary>
    /// An entry object for the teletype
    /// </summary>
    public class TeletypeEntry
    {
        #region Constants
        private const float maxAlpha = 0.75f;
        private const float minAlpha = 0f;
        #endregion

        #region Fields
        private double masterTimer = 0;
        private bool alive = false;
        #endregion

        #region Properties
        /// <summary>
        /// The text to be displayed
        /// </summary>
        public String Text;
        /// <summary>
        /// The font to be used
        /// </summary>
        public SpriteFont Font;
        /// <summary>
        /// The position of this text
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// The center of the text which defines where position is applied on this text
        /// </summary>
        public Vector2 Center = Vector2.Zero;
        /// <summary>
        /// The color of the text
        /// </summary>
        public Color Color;
        /// <summary>
        /// The time it takes for this entry to type on screen
        /// </summary>
        public double TimeToType;
        /// <summary>
        /// The total time this entry will be on screen
        /// </summary>
        public double TotalTimeOnScreen;
        /// <summary>
        /// If you use a shadow for this text, this will be the color
        /// </summary>
        public Color ShadowColor;
        /// <summary>
        /// Shadow offsets. If non-null, will be used.
        /// </summary>
        public Vector2[] ShadowOffset = null;
        /// <summary>
        /// The rendersprite instance to use
        /// </summary>
        public RenderSprite RenderSprite;
        /// <summary>
        /// Are we alive or not
        /// </summary>
        public bool Alive
        { get { return alive; } }
        /// <summary>
        /// FIXME: For now, we're hard coded to the craziness for Duologue, but in
        /// future iterations we want this to be MUCH more general.
        /// </summary>
        public RenderSpriteBlendMode RenderMode = RenderSpriteBlendMode.AlphaBlendTop;
        #endregion

        #region Constructor
        /// <summary>
        /// Empty constructor
        /// </summary>
        public TeletypeEntry()
        {
            alive = false;
        }
        /// <summary>
        /// Creates a teletype entry object
        /// </summary>
        /// <param name="renderSprite">The rendersprite instance to use</param>
        public TeletypeEntry(RenderSprite renderSprite)
        {
            RenderSprite = renderSprite;
            alive = true;
        }

        /// <summary>
        /// Creates a teletype entry object
        /// </summary>
        /// <param name="font">The font to use</param>
        /// <param name="text">The text to display</param>
        /// <param name="position">The position of the text</param>
        /// <param name="color">The color of the text</param>
        /// <param name="timeToType">The time it takes to type the entry</param>
        /// <param name="totalTimeOnScreen">The total time we will be on screen</param>
        /// <param name="renderSprite">The rendersprite instance to use</param>
        public TeletypeEntry(
            SpriteFont font,
            String text,
            Vector2 position,
            Color color,
            double timeToType,
            double totalTimeOnScreen,
            RenderSprite renderSprite)
        {
            Font = font;
            Text = text;
            Position = position;
            Color = color;
            TimeToType = timeToType;
            TotalTimeOnScreen = totalTimeOnScreen;
            RenderSprite = renderSprite;
            alive = true;
        }

        /// <summary>
        /// Creates a teletype entry object
        /// </summary>
        /// <param name="font">The font to use</param>
        /// <param name="text">The text to display</param>
        /// <param name="position">The position of the text</param>
        /// <param name="color">The color of the text</param>
        /// <param name="timeToType">The time it takes to type the entry</param>
        /// <param name="totalTimeOnScreen">The total time we will be on screen</param>
        /// <param name="renderSprite">The rendersprite instance to use</param>
        /// <param name="center">The center of the text</param>
        public TeletypeEntry(
            SpriteFont font,
            String text,
            Vector2 position,
            Vector2 center,
            Color color,
            double timeToType,
            double totalTimeOnScreen,
            RenderSprite renderSprite)
        {
            Font = font;
            Text = text;
            Position = position;
            Center = center;
            Color = color;
            TimeToType = timeToType;
            TotalTimeOnScreen = totalTimeOnScreen;
            RenderSprite = renderSprite;
            alive = true;
        }

        /// <summary>
        /// Creates a teletype entry object
        /// </summary>
        /// <param name="font">The font to use</param>
        /// <param name="text">The text to display</param>
        /// <param name="position">The position of the text</param>
        /// <param name="color">The color of the text</param>
        /// <param name="timeToType">The time it takes to type the entry</param>
        /// <param name="totalTimeOnScreen">The total time we will be on screen</param>
        /// <param name="renderSprite">The rendersprite instance to use</param>
        /// <param name="shadowColor">The Shadow color to use</param>
        /// <param name="shadowOffset">Array containing shadow offsets</param>
        public TeletypeEntry(
            SpriteFont font,
            String text,
            Vector2 position,
            Color color,
            double timeToType,
            double totalTimeOnScreen,
            Color shadowColor,
            Vector2[] shadowOffset,
            RenderSprite renderSprite)
        {
            Font = font;
            Text = text;
            Position = position;
            Color = color;
            TimeToType = timeToType;
            TotalTimeOnScreen = totalTimeOnScreen;
            RenderSprite = renderSprite;
            ShadowColor = shadowColor;
            ShadowOffset = shadowOffset;
            alive = true;
        }

        /// <summary>
        /// Creates a teletype entry object
        /// </summary>
        /// <param name="font">The font to use</param>
        /// <param name="text">The text to display</param>
        /// <param name="position">The position of the text</param>
        /// <param name="color">The color of the text</param>
        /// <param name="timeToType">The time it takes to type the entry</param>
        /// <param name="totalTimeOnScreen">The total time we will be on screen</param>
        /// <param name="renderSprite">The rendersprite instance to use</param>
        /// <param name="shadowColor">The Shadow color to use</param>
        /// <param name="shadowOffset">Array containing shadow offsets</param>
        /// <param name="center">The Center of the text</param>
        public TeletypeEntry(
            SpriteFont font,
            String text,
            Vector2 position,
            Vector2 center,
            Color color,
            double timeToType,
            double totalTimeOnScreen,
            Color shadowColor,
            Vector2[] shadowOffset,
            RenderSprite renderSprite)
        {
            Font = font;
            Text = text;
            Position = position;
            Center = center;
            Color = color;
            TimeToType = timeToType;
            TotalTimeOnScreen = totalTimeOnScreen;
            RenderSprite = renderSprite;
            ShadowColor = shadowColor;
            ShadowOffset = shadowOffset;
            alive = true;
        }
        #endregion

        #region Draw / Update
        public void Update(GameTime gameTime)
        {
            masterTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (masterTimer > TotalTimeOnScreen)
            {
                masterTimer = TotalTimeOnScreen;
                alive = false;
            }
        }

        public void Draw(GameTime gameTime)
        {
            int charToTypeTo;
            if (masterTimer <= TimeToType)
                charToTypeTo = (int)MathHelper.Lerp(0, Text.Length, (float)(masterTimer / TimeToType));
            else
                charToTypeTo = Text.Length;

            // Draw the done bits
            if (ShadowOffset == null)
            {
                RenderSprite.DrawString(
                    Font,
                    Text.Substring(0, charToTypeTo),
                    Position,
                    Color,
                    Vector2.One,
                    Center,
                    RenderMode);
            }
            else
            {
                RenderSprite.DrawString(
                    Font,
                    Text.Substring(0, charToTypeTo),
                    Position,
                    Color,
                    ShadowColor,
                    1f,
                    Center,
                    ShadowOffset,
                    RenderMode);
            }
        }
        #endregion
    }

    public class Teletype : DrawableGameComponent, IService
    {
        #region Constants
        private const int maxTeletypeEntries = 10;
        #endregion

        #region Fields
        private Game myGame;
        private RenderSprite rsprite;

        private TeletypeEntry[] entries;
        #endregion

        #region Properties
        #endregion

        #region Constructor / Init
        public Teletype(Game game, RenderSprite rendersprite)
            : base(game)
        {
            rsprite = rendersprite;
            myGame = game;
            Enabled = false;
            Visible = false;
        }

        public override void Initialize()
        {
            entries = new TeletypeEntry[maxTeletypeEntries];
            base.Initialize();
        }
        #endregion

        #region Public methods
        public bool AddEntry(TeletypeEntry entry)
        {
            for (int i = 0; i < maxTeletypeEntries; i++)
            {
                if (entries[i] == null || !entries[i].Alive)
                {
                    entries[i] = entry;
                    Enabled = true;
                    Visible = true;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Draw / Update
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion
    }
}
