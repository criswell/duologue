#region File Description
#endregion

#region Using Statements
// System
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
using Mimicware.Manager;
using Mimicware.Graphics;
// Duologue
using Duologue;
using Duologue.Properties;
using Duologue.Screens;
#endregion

namespace Duologue.PlayObjects
{
    /// <summary>
    /// The various player color schemes
    /// </summary>
    public class PlayerColors
    {
        #region Constants
        private const int numColors = 2;

        /// <summary>
        /// The light version of the color
        /// </summary>
        public const int Light = 0;

        /// <summary>
        /// The medium version of the color
        /// </summary>
        public const int Medium = 1;

        /// <summary>
        /// The dark version of the color
        /// FIXME: Currently no one supports this really
        /// </summary>
        public const int Dark = 1;
        #endregion

        #region Fields
        private static PlayerColors[] theColors;
        #endregion

        #region Properties
        /// <summary>
        /// The colors of this player
        /// </summary>
        public Color[] Colors;

        /// <summary>
        /// The name of this color scheme
        /// </summary>
        public string Name;
        #endregion

        #region Constructor
        private PlayerColors()
        {
            Colors = new Color[numColors];
        }
        #endregion

        #region Get method
        public static PlayerColors[] GetPlayerColors()
        {
            if (theColors == null)
            {
                theColors = new PlayerColors[InputManager.MaxInputs];

                // Wahoo, more slop!

                // Melon for player #1
                theColors[0] = new PlayerColors();
                theColors[0].Name = "Melon";
                theColors[0].Colors[0] = Color.Honeydew;
                theColors[0].Colors[1] = Color.PaleGreen;

                // Reddish for player #2
                theColors[1] = new PlayerColors();
                theColors[1].Name = "Rose";
                theColors[1].Colors[0] = Color.MistyRose;
                theColors[1].Colors[1] = Color.PeachPuff;

                // Bluish for player #3
                theColors[2] = new PlayerColors();
                theColors[2].Name = "Blue";
                theColors[2].Colors[0] = Color.AliceBlue;
                theColors[2].Colors[1] = Color.LightSkyBlue;

                // Yellow for player #4
                theColors[3] = new PlayerColors();
                theColors[3].Name = "Yellow";
                theColors[3].Colors[0] = Color.LightGoldenrodYellow;
                theColors[3].Colors[1] = new Color(new Vector3(255f, 221f,62f));
            }

            return theColors;
        }
        #endregion
    }
}
