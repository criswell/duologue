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
using Mimicware;
using Mimicware.Manager;
using Mimicware.Graphics;
// Duologue
using Duologue;
using Duologue.Properties;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.Audio;
using Duologue.State;
#endregion

namespace Duologue.Waves
{
    [Serializable()]
    public class WavesOutOfRangeException : System.Exception
    {
        public WavesOutOfRangeException() { }
        public WavesOutOfRangeException(string message) { }
        public WavesOutOfRangeException(string message, System.Exception inner) { }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        /*protected WavesOutOfRangeException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }*/
    }

    /// <summary>
    /// For now, we'll just be storing these in memory. At a later date, we likely will
    /// want these to be stored on disk
    /// </summary>
    public class WaveDefinitions
    {
        #region Constants
        private const int numberOfMinorWaves = 3;
        private const int numberOfMajorWaves = 2;
        private const int numberOfWaves = numberOfMajorWaves * numberOfMinorWaves;
        #endregion

        #region Fields
        /// <summary>
        /// The waves used in this game
        /// </summary>
        private GameWave[] Waves;
        #endregion

        #region Properties
        public int TotalNumberOfWaves
        {
            get { return numberOfWaves; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs the wave def object
        /// </summary>
        public WaveDefinitions()
        {
            // For now, this is all in-memory, we may eventually want to dump this crap to a file
            // FIXME
            Waves = new GameWave[numberOfWaves];

            Wavelet[] Wavelets;

            // PLEASE NOTE: If you go past the GameWaveManager.MaxMinorNumber or .MaxMajorNumber
            // your wave/wavelet will be ignored!
            // Currently MaxMinorNumber is 3 and MaxMajorNumber is 999

            #region WaveDef (1-1) "The LZ is hot!"
            #region Metadata
            Waves[GetIndex(1, 1)] = new GameWave();
            Waves[GetIndex(1, 1)].Background = 1;
            Waves[GetIndex(1, 1)].ThrobColor = Color.Red;
            Waves[GetIndex(1, 1)].ColorState = 0;
            Waves[GetIndex(1, 1)].MajorWaveNumber = 1;
            Waves[GetIndex(1, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(1, 1)].Name = "The LZ is hot!";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet - Buzzsaws bottom
            #region
            Wavelets[0] = new Wavelet(4, 0, ColorPolarity.Negative);
            Wavelets[0].SongID = SongID.Ultrafix;
            for (int i = 0; i < 4; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[0].StartAngle[i] = (float)i * MathHelper.PiOver2;
            }
            #endregion
            // Second wavelet - Buzzsaws top
            #region
            Wavelets[1] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[1].SongID = SongID.Ultrafix;
            for (int i = 0; i < 4; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[1].StartAngle[i] = (float)i * MathHelper.PiOver2 + MathHelper.PiOver4;
            }
            #endregion
            // Third wavelet - Mixed buzzsaws all around
            #region
            Wavelets[2] = new Wavelet(8, 0);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < 8; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver4;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
            }
            #endregion

            Waves[GetIndex(1, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (1-2) "The scouting party"
            #region Metadata
            Waves[GetIndex(1, 2)] = new GameWave();
            Waves[GetIndex(1, 2)].Background = 1;
            Waves[GetIndex(1, 2)].ThrobColor = Color.Red;
            Waves[GetIndex(1, 2)].ColorState = 0;
            Waves[GetIndex(1, 2)].MajorWaveNumber = 1;
            Waves[GetIndex(1, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(1, 2)].Name = "The scouting party";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(8, 0);
            Wavelets[0].SongID = SongID.Ultrafix;
            for (int i = 0; i < 8; i++)
            {
                if(MWMathHelper.IsEven(i))
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                else
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;

                if (MWMathHelper.IsEven(i))
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;

                Wavelets[0].StartAngle[i] = (float)i * MathHelper.PiOver4;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(8, 0);
            Wavelets[1].SongID = SongID.Ultrafix;
            for (int i = 0; i < 8; i++)
            {
                if (MWMathHelper.IsEven(i))
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                else
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;

                if (!MWMathHelper.IsEven(i))
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;

                Wavelets[1].StartAngle[i] = (float)i * MathHelper.PiOver4 + MathHelper.PiOver4 / 2f; ;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(16, 0);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < 16; i++)
            {
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;

                if (!MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;

                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver4 + MathHelper.PiOver4 / 4f;
            }
            #endregion

            Waves[GetIndex(1, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (1-3) "Wave after wave..."
            #region Metadata
            Waves[GetIndex(1, 3)] = new GameWave();
            Waves[GetIndex(1, 3)].Background = 1;
            Waves[GetIndex(1, 3)].ThrobColor = Color.Red;
            Waves[GetIndex(1, 3)].ColorState = 0;
            Waves[GetIndex(1, 3)].MajorWaveNumber = 1;
            Waves[GetIndex(1, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(1, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(1, 3)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(1, 3)].ParallaxElementTop.Tint = new Color(172, 131, 22);
            Waves[GetIndex(1, 3)].ParallaxElementTop.Speed = 0.6f;
            Waves[GetIndex(1, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(1, 3)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(1, 3)].ParallaxElementBottom.Tint = new Color(172, 131, 22);
            Waves[GetIndex(1, 3)].ParallaxElementBottom.Speed = -0.6f;
            Waves[GetIndex(1, 3)].Name = "Wave after wave...";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(10, 0);
            Wavelets[0].SongID = SongID.Ultrafix;
            for (int i = 0; i < 10; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[0].StartAngle[i] = (float)i * MathHelper.TwoPi / 10f;
                Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(8, 0);
            Wavelets[1].SongID = SongID.Ultrafix;
            for (int i = 0; i < 8; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[1].StartAngle[i] = (float)i * MathHelper.TwoPi / 8f;
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(12, 0);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < 12; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.TwoPi / 12f;
            }
            #endregion

            Waves[GetIndex(1, 3)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (2-1) "Mr. Wiggles was his name..."
            #region Metadata
            Waves[GetIndex(2, 1)] = new GameWave();
            Waves[GetIndex(2, 1)].Background = 1;
            Waves[GetIndex(2, 1)].ThrobColor = Color.Red;
            Waves[GetIndex(2, 1)].ColorState = 0;
            Waves[GetIndex(2, 1)].MajorWaveNumber = 2;
            Waves[GetIndex(2, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(2, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(2, 1)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(2, 1)].ParallaxElementTop.Tint = new Color(172, 131, 22);
            Waves[GetIndex(2, 1)].ParallaxElementTop.Speed = 0.6f;
            Waves[GetIndex(2, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(2, 1)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(2, 1)].ParallaxElementBottom.Tint = new Color(131, 100, 17);
            Waves[GetIndex(2, 1)].ParallaxElementBottom.Speed = -0.6f;
            Waves[GetIndex(2, 1)].Name = "Mr. Wiggles was his name...";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(8, 0, ColorPolarity.Positive);
            Wavelets[0].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[0].StartAngle[i] = (float)i * MathHelper.PiOver2 / (float)Wavelets[0].Enemies.Length
                    + MathHelper.PiOver2;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(8, 0, ColorPolarity.Negative);
            Wavelets[1].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[1].StartAngle[i] = (float)i * MathHelper.PiOver2 / (float)Wavelets[1].Enemies.Length
                    + 5f * MathHelper.PiOver2;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(16, 0);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[2].Enemies.Length/2f; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver2 / Wavelets[2].Enemies.Length/2f
                    + MathHelper.PiOver2;
            }
            for (int i = (int)(Wavelets[2].Enemies.Length / 2f); i < Wavelets[2].Enemies.Length ; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver2 / Wavelets[2].Enemies.Length / 2f
                    + 5f * MathHelper.PiOver2;
            }
            #endregion

            Waves[GetIndex(2, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (2-2) "...and death was his game"
            #region Metadata
            Waves[GetIndex(2, 2)] = new GameWave();
            Waves[GetIndex(2, 2)].Background = 1;
            Waves[GetIndex(2, 2)].ThrobColor = Color.RosyBrown;
            Waves[GetIndex(2, 2)].ColorState = 0;
            Waves[GetIndex(2, 2)].MajorWaveNumber = 2;
            Waves[GetIndex(2, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(2, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(2, 2)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(2, 2)].ParallaxElementTop.Tint = new Color(172, 131, 22);
            Waves[GetIndex(2, 2)].ParallaxElementTop.Speed = 0.7f;
            Waves[GetIndex(2, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(2, 2)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(2, 2)].ParallaxElementBottom.Tint = new Color(131, 100, 17);
            Waves[GetIndex(2, 2)].ParallaxElementBottom.Speed = -0.65f;
            Waves[GetIndex(2, 2)].Name = "...and death was his game";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(8, 0, ColorPolarity.Positive);
            Wavelets[0].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[0].StartAngle[i] = (float)i * MathHelper.PiOver2 / (float)Wavelets[0].Enemies.Length;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(8, 0, ColorPolarity.Negative);
            Wavelets[1].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[1].StartAngle[i] = (float)i * MathHelper.PiOver2 / (float)Wavelets[1].Enemies.Length +
                    MathHelper.TwoPi;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(16, 0);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[2].Enemies.Length / 2f; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver2 / Wavelets[2].Enemies.Length / 2f;
            }
            for (int i = (int)(Wavelets[2].Enemies.Length / 2f); i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver2 / Wavelets[2].Enemies.Length / 2f
                    + MathHelper.TwoPi;
            }
            #endregion

            Waves[GetIndex(2, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (2-3) "Reinforcements arive.."
            #region Metadata
            Waves[GetIndex(2, 3)] = new GameWave();
            Waves[GetIndex(2, 3)].Background = 1;
            Waves[GetIndex(2, 3)].ThrobColor = Color.Tomato;
            Waves[GetIndex(2, 3)].ColorState = 0;
            Waves[GetIndex(2, 3)].MajorWaveNumber = 2;
            Waves[GetIndex(2, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(2, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(2, 3)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(2, 3)].ParallaxElementTop.Tint = new Color(172, 131, 22);
            Waves[GetIndex(2, 3)].ParallaxElementTop.Speed = 0.9f;
            Waves[GetIndex(2, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(2, 3)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(2, 3)].ParallaxElementBottom.Tint = new Color(172, 131, 22);
            Waves[GetIndex(2, 3)].ParallaxElementBottom.Speed = -0.9f;
            Waves[GetIndex(2, 3)].Name = "Reinforcements arive..";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(16, 2);
            Wavelets[0].SongID = SongID.Ultrafix;
            for (int i = 0; i < 8; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[0].StartAngle[i] = (float)i * MathHelper.TwoPi / 16f;
                Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
            }
            for (int i = 8; i < 16; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[0].StartAngle[i] = (float)i * MathHelper.TwoPi / 16f;
                Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(16, 2);
            Wavelets[1].SongID = SongID.Ultrafix;
            for (int i = 0; i < 8; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[1].StartAngle[i] = (float)i * MathHelper.TwoPi / 16f;
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
            }
            for (int i = 8; i < 16; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[1].StartAngle[i] = (float)i * MathHelper.TwoPi / 16f;
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(16, 2);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < 16; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.TwoPi / 16f;
            }
            #endregion

            Waves[GetIndex(2, 3)].Wavelets = Wavelets;
            #endregion
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the wave specified by major and minor numbers
        /// </summary>
        /// <param name="MajorNum">The major number of the wave</param>
        /// <param name="MinorNum">The minor number of the wave. Must be 1-4. If greater than 4, or less than 1, will be capped.</param>
        /// <returns>The gamewave specified by the major and minor numbers</returns>
        public GameWave GetWave(int MajorNum, int MinorNum)
        {
            try
            {
                return Waves[GetIndex(MajorNum, MinorNum)];
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Given a major and minor number for a game wave, return the index for it
        /// </summary>
        private int GetIndex(int MajorNum, int MinorNum)
        {
            if (MinorNum > GameWaveManager.MaxMinorNumber - 1)
                MinorNum = GameWaveManager.MaxMinorNumber - 1;
            else if (MinorNum < 1)
                MinorNum = 1;
            int index = (MajorNum * (GameWaveManager.MaxMinorNumber - 1)) -
                GameWaveManager.MaxMinorNumber + MinorNum;
            if (index >= numberOfWaves)
                throw new WavesOutOfRangeException();
            return index;
        }
        #endregion
    }
}
