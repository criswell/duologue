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
        private const int numColors = 3;

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
        public const int Dark = 2;
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
                theColors[0].Colors[PlayerColors.Light] = Color.LightGreen;
                theColors[0].Colors[PlayerColors.Medium] = Color.PaleGreen;
                theColors[0].Colors[PlayerColors.Dark] = Color.Green;

                // Reddish for player #2
                theColors[1] = new PlayerColors();
                theColors[1].Name = "Rose";
                theColors[1].Colors[PlayerColors.Light] = Color.Pink;
                theColors[1].Colors[PlayerColors.Medium] = Color.PeachPuff;
                theColors[1].Colors[PlayerColors.Dark] = Color.LightSalmon;

                // Bluish for player #3
                theColors[2] = new PlayerColors();
                theColors[2].Name = "Blue";
                theColors[2].Colors[PlayerColors.Light] = Color.AliceBlue;
                theColors[2].Colors[PlayerColors.Medium] = Color.LightSkyBlue;
                theColors[2].Colors[PlayerColors.Dark] = Color.LightSteelBlue;

                // Yellow for player #4
                theColors[3] = new PlayerColors();
                theColors[3].Name = "Yellow";
                theColors[3].Colors[PlayerColors.Light] = Color.LightGoldenrodYellow;
                theColors[3].Colors[PlayerColors.Medium] = new Color(new Vector3(255f, 221f,62f));
                theColors[3].Colors[PlayerColors.Dark] = Color.Yellow;
            }

            return theColors;
        }
        #endregion
    }
}
