using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Mimicware.Graphics;
using Mimicware.Manager;

namespace Duologue.State
{
    /// <summary>
    /// The color state defines which colors are in use in a given wave/level. It defines the following
    /// things:
    /// 1) The positive color
    /// 2) The negative color
    /// 3) The next color state in the sequence
    /// 
    /// The color state sequence will be of a Tetradic color scheme (see color theory).
    /// </summary>
    /// <remarks>
    /// One thing to note here is that the postive and negative demarkations are completely
    /// arbitrary.
    /// </remarks>
    public class ColorState
    {
        #region Constants
        public const int numberColorsPerPolarity = 2;
        public const int numberOfColorStates = 3;
        #endregion

        #region Fields
        private static ColorState[] theStates;
        #endregion

        #region Properties
        /// <summary>
        /// This color state's positive colors
        /// </summary>
        public Color[] Positive;
        /// <summary>
        /// This color state's negative colors
        /// </summary>
        public Color[] Negative;

        /// <summary>
        /// The name of the positive color
        /// </summary>
        public string PositiveName;

        /// <summary>
        /// The name of the negative color
        /// </summary>
        public string NegativeName;
        #endregion

        #region Constructor
        private ColorState()
        {
            Positive = new Color[numberColorsPerPolarity];
            Negative = new Color[numberColorsPerPolarity];
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Use this to get the color states availavble
        /// </summary>
        /// <returns>An array of the color states</returns>
        public static ColorState[] GetColorStates()
        {
            if (theStates == null) {
                theStates = new ColorState[numberOfColorStates];
                
                // Sloppy? Maybe... whatever

                // Blue/Orange
                theStates[0] = new ColorState();
                theStates[0].PositiveName = "Blue";
                theStates[0].Positive[0] = new Color((byte)0, (byte)64, (byte)110);
                theStates[0].Positive[1] = new Color((byte)33, (byte)0, (byte)79);
                theStates[0].NegativeName = "Orange";
                theStates[0].Negative[0] = new Color((byte)255, (byte)7, (byte)0);
                theStates[0].Negative[1] = new Color((byte)253, (byte)128, (byte)0);

                // Green/Red
                theStates[1] = new ColorState();
                theStates[1].PositiveName = "Green";
                theStates[1].Positive[0] = new Color((byte)63, (byte)185, (byte)0);
                theStates[1].Positive[1] = new Color((byte)0, (byte)64, (byte)110);
                theStates[1].NegativeName = "Red";
                theStates[1].Negative[0] = new Color((byte)88, (byte)21, (byte)64);
                theStates[1].Negative[1] = new Color((byte)255, (byte)7, (byte)0);

                // Yellow/Violet
                theStates[2] = new ColorState();
                theStates[2].PositiveName = "Yellow";
                theStates[2].Positive[0] = new Color((byte)254, (byte)249, (byte)0);
                theStates[2].Positive[1] = new Color((byte)63, (byte)185, (byte)0);
                theStates[2].NegativeName = "Violet";
                theStates[2].Negative[0] = new Color((byte)111, (byte)0, (byte)128);
                theStates[2].Negative[1] = new Color((byte)88, (byte)21, (byte)64);
            }
            return theStates;
        }
        #endregion
    }
}