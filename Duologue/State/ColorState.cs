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
    /// The color polarity
    /// </summary>
    public enum ColorPolarity
    {
        /// <summary>
        /// The positive color polarity
        /// </summary>
        Positive,
        /// <summary>
        /// The negative color polarity
        /// </summary>
        Negative
    }
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
        public const int numberColorsPerPolarity = 3;
        public const int numberOfColorStates = 3;

        /// <summary>
        /// Light version of the color
        /// </summary>
        public const int Light = 0;

        /// <summary>
        /// Medium version of the color
        /// </summary>
        public const int Medium = 1;

        /// <summary>
        /// Dark version of the color
        /// FIXME: no one currently supports this
        /// </summary>
        public const int Dark = 2;
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
                theStates[0].Positive[Light] = new Color(0, 204, 255); // 100
                theStates[0].Positive[Medium] = new Color(0, 168, 210); // 85
                theStates[0].Positive[Dark] = new Color(0, 124, 156); // 75

                theStates[0].NegativeName = "Orange";
                theStates[0].Negative[Light] = new Color(255, 174, 0);
                theStates[0].Negative[Medium] = new Color(217, 148, 0);
                theStates[0].Negative[Dark] = new Color(191, 130, 0);

                // Green/Red
                theStates[1] = new ColorState();
                theStates[1].PositiveName = "Green";
                theStates[1].Positive[Light] = new Color(0, 255, 186);
                theStates[1].Positive[Medium] = new Color(0, 217, 158);
                theStates[1].Positive[Dark] = new Color(0, 191, 140);

                theStates[1].NegativeName = "Red";
                theStates[1].Negative[Light] = new Color(255, 0, 66);
                theStates[1].Negative[Medium] = new Color(217, 0, 56);
                theStates[1].Negative[Dark] = new Color(191, 0, 49);

                // Yellow/Violet
                theStates[2] = new ColorState();
                theStates[2].PositiveName = "Yellow";
                theStates[2].Positive[Light] = new Color(234, 255, 0);
                theStates[2].Positive[Medium] = new Color(199, 217, 0);
                theStates[2].Positive[Dark] = new Color(175, 191, 0);

                theStates[2].NegativeName = "Violet";
                theStates[2].Negative[Light] = new Color(138, 0, 255);
                theStates[2].Negative[Medium] = new Color(117, 0, 217);
                theStates[2].Negative[Dark] = new Color(103, 0, 191);
            }
            return theStates;
        }

        /// <summary>
        /// Given two colors, determine if they are the same in a colorstate
        /// </summary>
        /// <param name="color">First color</param>
        /// <param name="color_2">Second color</param>
        /// <returns>True/Fase</returns>
        internal bool SameColor(Color color, Color color_2)
        {
            return ((color == Positive[0] || color == Positive[1]) &&
                (color_2 == Positive[0] || color_2 == Positive[1])) ||
                ((color == Negative[0] || color == Negative[1]) &&
                (color_2 == Negative[0] || color_2 == Negative[1]));
        }

        /// <summary>
        /// Returns a random color polarity
        /// </summary>
        /// <returns>The random color polarity</returns>
        internal static ColorPolarity RandomPolarity()
        {
            if (InstanceManager.Random.Next(2) == 0)
            {
                return ColorPolarity.Negative;
            }
            return ColorPolarity.Positive;
        }
        #endregion
    }
}
