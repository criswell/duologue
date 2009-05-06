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
        private const int maxNumberOfMajorWaves = 8;
        private const int numberOfWavelets = maxNumberOfMajorWaves * GameWaveManager.MaxMinorNumber;
        #endregion

        #region Fields
        /// <summary>
        /// The waves used in this game
        /// </summary>
        private GameWave[] Waves;
        #endregion

        #region Properties
        public int TotalNumberOfWavelets
        {
            get { return numberOfWavelets; }
        }

        public int TotalNumberOfMajorWaves
        {
            get { return maxNumberOfMajorWaves; }
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
            Waves = new GameWave[numberOfWavelets];

            Wavelet[] Wavelets;

            // PLEASE NOTE: If you go past the GameWaveManager.MaxMinorNumber or .MaxMajorNumber
            // your wave/wavelet will be ignored!
            // Currently MaxMinorNumber is 3 and MaxMajorNumber is 999

            #region Wave 1
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

            #region WaveDef (1-2) "Mr. Wiggles was his name..."
            #region Metadata
            Waves[GetIndex(1, 2)] = new GameWave();
            Waves[GetIndex(1, 2)].Background = 1;
            Waves[GetIndex(1, 2)].ThrobColor = Color.Red;
            Waves[GetIndex(1, 2)].ColorState = 0;
            Waves[GetIndex(1, 2)].MajorWaveNumber = 1;
            Waves[GetIndex(1, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(1, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(1, 2)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(1, 2)].ParallaxElementTop.Tint = new Color(172, 131, 22);
            Waves[GetIndex(1, 2)].ParallaxElementTop.Speed = 0.5f;
            Waves[GetIndex(1, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(1, 2)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(1, 2)].ParallaxElementBottom.Tint = new Color(131, 100, 17);
            Waves[GetIndex(1, 2)].ParallaxElementBottom.Speed = -0.5f;
            Waves[GetIndex(1, 2)].Name = "Mr. Wiggles was his name...";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(4, 0, ColorPolarity.Positive);
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
            Wavelets[1] = new Wavelet(4, 0, ColorPolarity.Negative);
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
            Wavelets[2] = new Wavelet(8, 0);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[2].Enemies.Length / 2f; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver2 / Wavelets[2].Enemies.Length / 2f
                    + MathHelper.PiOver2;
            }
            for (int i = (int)(Wavelets[2].Enemies.Length / 2f); i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver2 / Wavelets[2].Enemies.Length / 2f
                    + 5f * MathHelper.PiOver2;
            }
            #endregion

            Waves[GetIndex(1, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (1-3) "...and death was his game"
            #region Metadata
            Waves[GetIndex(1, 3)] = new GameWave();
            Waves[GetIndex(1, 3)].Background = 1;
            Waves[GetIndex(1, 3)].ThrobColor = Color.RosyBrown;
            Waves[GetIndex(1, 3)].ColorState = 0;
            Waves[GetIndex(1, 3)].MajorWaveNumber = 1;
            Waves[GetIndex(1, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(1, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(1, 3)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(1, 3)].ParallaxElementTop.Tint = new Color(172, 131, 22);
            Waves[GetIndex(1, 3)].ParallaxElementTop.Speed = 0.6f;
            Waves[GetIndex(1, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(1, 3)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(1, 3)].ParallaxElementBottom.Tint = new Color(131, 100, 17);
            Waves[GetIndex(1, 3)].ParallaxElementBottom.Speed = -0.65f;
            Waves[GetIndex(1, 3)].Name = "...and death was his game";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[0].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[0].StartAngle[i] = (float)i * MathHelper.PiOver2 / (float)Wavelets[0].Enemies.Length;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(4, 0, ColorPolarity.Negative);
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
            Wavelets[2] = new Wavelet(8, 0);
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

            Waves[GetIndex(1, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 2
            #region WaveDef (2-1) "The scouting party"
            #region Metadata
            Waves[GetIndex(2, 1)] = new GameWave();
            Waves[GetIndex(2, 1)].Background = 1;
            Waves[GetIndex(2, 1)].ThrobColor = Color.Red;
            Waves[GetIndex(2, 1)].ColorState = 0;
            Waves[GetIndex(2, 1)].MajorWaveNumber = 2;
            Waves[GetIndex(2, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(2, 1)].Name = "The scouting party";
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

                Wavelets[0].StartAngle[i] = (float)i * MathHelper.Pi/Wavelets[0].Enemies.Length +
                    MathHelper.PiOver2;
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

                Wavelets[1].StartAngle[i] = (float)i * MathHelper.Pi / Wavelets[0].Enemies.Length +
                    5f * MathHelper.PiOver2;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(10, 0);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;

                if (!MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;

                Wavelets[2].StartAngle[i] = (float)i * MathHelper.TwoPi / Wavelets[2].Enemies.Length;
            }
            #endregion

            Waves[GetIndex(2, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (2-2) "Buzzsaw backlash"
            #region Metadata
            Waves[GetIndex(2, 2)] = new GameWave();
            Waves[GetIndex(2, 2)].Background = 1;
            Waves[GetIndex(2, 2)].ThrobColor = Color.Azure;
            Waves[GetIndex(2, 2)].ColorState = 0;
            Waves[GetIndex(2, 2)].MajorWaveNumber = 2;
            Waves[GetIndex(2, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(2, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(2, 2)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(2, 2)].ParallaxElementTop.Tint = Color.Olive;
            Waves[GetIndex(2, 2)].ParallaxElementTop.Speed = 0.6f;
            Waves[GetIndex(2, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(2, 2)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(2, 2)].ParallaxElementBottom.Tint = Color.CadetBlue;
            Waves[GetIndex(2, 2)].ParallaxElementBottom.Speed = -0.65f;
            Waves[GetIndex(2, 2)].Name = "Buzzsaw backlash";
            #endregion

            // The purpose here is to try and force them to use their light

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(2, 0);
            Wavelets[0].SongID = SongID.Ultrafix;
            // Upper left corner
            Wavelets[0].Enemies[0] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].ColorPolarities[0] = ColorPolarity.Positive;
            Wavelets[0].StartAngle[0] = MathHelper.PiOver4 * 3f;
            // Lower right corner
            Wavelets[0].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].ColorPolarities[1] = ColorPolarity.Negative;
            Wavelets[0].StartAngle[1] = MathHelper.PiOver4 * 7f;
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(4, 0);
            Wavelets[1].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 + MathHelper.PiOver4;
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(8, 0);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[2].Enemies.Length/2f; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[2].StartAngle[i] = i * MathHelper.PiOver2 + MathHelper.PiOver4;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = (int)(Wavelets[2].Enemies.Length / 2f); i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[2].StartAngle[i] = i * MathHelper.PiOver2 + MathHelper.PiOver4;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
            }
            #endregion

            Waves[GetIndex(2, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (2-3) "Defensive drainage"
            #region Metadata
            Waves[GetIndex(2, 3)] = new GameWave();
            Waves[GetIndex(2, 3)].Background = 1;
            Waves[GetIndex(2, 3)].ThrobColor = Color.Azure;
            Waves[GetIndex(2, 3)].ColorState = 0;
            Waves[GetIndex(2, 3)].MajorWaveNumber = 2;
            Waves[GetIndex(2, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(2, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(2, 3)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(2, 3)].ParallaxElementTop.Tint = Color.Olive;
            Waves[GetIndex(2, 3)].ParallaxElementTop.Speed = 0.6f;
            Waves[GetIndex(2, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(2, 3)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(2, 3)].ParallaxElementBottom.Tint = Color.CadetBlue;
            Waves[GetIndex(2, 3)].ParallaxElementBottom.Speed = -0.65f;
            Waves[GetIndex(2, 3)].Name = "Defensive drainage";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(6, 0);
            Wavelets[0].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[0].Enemies.Length / 2f; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[0].StartAngle[i] = i * MathHelper.PiOver2;
                if (MWMathHelper.IsEven(i))
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
            }
            for (int i = (int)(Wavelets[0].Enemies.Length / 2f); i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[0].StartAngle[i] = i * MathHelper.PiOver2;
                if (MWMathHelper.IsEven(i))
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(6, 0);
            Wavelets[1].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[1].Enemies.Length / 2f; i++)
            {
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 + MathHelper.PiOver4;
                if (MWMathHelper.IsEven(i))
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                else
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
            }
            for (int i = (int)(Wavelets[1].Enemies.Length / 2f); i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2;
                if (MWMathHelper.IsEven(i))
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                else
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(6, 0);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[2].Enemies.Length / 2f; i++)
            {
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                Wavelets[2].StartAngle[i] = i * MathHelper.PiOver2 + MathHelper.PiOver4;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
            }
            for (int i = (int)(Wavelets[2].Enemies.Length / 2f); i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].StartAngle[i] = i * MathHelper.PiOver2 + MathHelper.PiOver4 * 0.5f;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
            }
            #endregion

            Waves[GetIndex(2, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 3 (Boss)
            #region WaveDef (3-1) "Buzzsaw lead-up"
            #region Metadata
            Waves[GetIndex(3, 1)] = new GameWave();
            Waves[GetIndex(3, 1)].Background = 1;
            Waves[GetIndex(3, 1)].ThrobColor = new Color(255, 0, 204);
            Waves[GetIndex(3, 1)].ColorState = 0;
            Waves[GetIndex(3, 1)].MajorWaveNumber = 3;
            Waves[GetIndex(3, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(3, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(3, 1)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(3, 1)].ParallaxElementTop.Tint = new Color(5, 205, 255);
            Waves[GetIndex(3, 1)].ParallaxElementTop.Speed = 0.7f;
            Waves[GetIndex(3, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(3, 1)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(3, 1)].ParallaxElementBottom.Tint = new Color(255, 199, 5);
            Waves[GetIndex(3, 1)].ParallaxElementBottom.Speed = -0.75f;
            Waves[GetIndex(3, 1)].Name = "Buzzsaw lead-up";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[0].SongID = SongID.Ultrafix;
            Wavelets[0].Enemies[0] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[0] = MathHelper.PiOver4;
            Wavelets[0].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[1] = MathHelper.Pi;
            Wavelets[0].Enemies[2] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[2] = MathHelper.PiOver2 * 3f;
            Wavelets[0].Enemies[3] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[3] = MathHelper.PiOver4 * 5f;
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(4, 0, ColorPolarity.Negative);
            Wavelets[1].SongID = SongID.Ultrafix;
            Wavelets[1].Enemies[0] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[0] = MathHelper.PiOver4;
            Wavelets[1].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[1] = 0f;
            Wavelets[1].Enemies[2] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[2] = MathHelper.PiOver2;
            Wavelets[1].Enemies[3] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[3] = MathHelper.PiOver4 * 5f;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(10, 0);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
            }
            #endregion

            Waves[GetIndex(3, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (3-2) "...something is coming"
            #region Metadata
            Waves[GetIndex(3, 2)] = new GameWave();
            Waves[GetIndex(3, 2)].Background = 1;
            Waves[GetIndex(3, 2)].ThrobColor = new Color(168, 0, 255);
            Waves[GetIndex(3, 2)].ColorState = 0;
            Waves[GetIndex(3, 2)].MajorWaveNumber = 3;
            Waves[GetIndex(3, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(3, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(3, 2)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(3, 2)].ParallaxElementTop.Tint = new Color(5, 255, 170);
            Waves[GetIndex(3, 2)].ParallaxElementTop.Speed = 0.7f;
            Waves[GetIndex(3, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(3, 2)].ParallaxElementBottom.Intensity = 3;
            Waves[GetIndex(3, 2)].ParallaxElementBottom.Tint = new Color(255, 158, 5);
            Waves[GetIndex(3, 2)].ParallaxElementBottom.Speed = -0.75f;
            Waves[GetIndex(3, 2)].Name = "...something is coming";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[0].SongID = SongID.Ultrafix;
            Wavelets[0].Enemies[0] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[0] = 3f * MathHelper.PiOver4;
            Wavelets[0].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[1] = MathHelper.Pi;
            Wavelets[0].Enemies[2] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[2] = MathHelper.PiOver2 * 3f;
            Wavelets[0].Enemies[3] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[3] = MathHelper.PiOver4 * 7f;
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(4, 0, ColorPolarity.Negative);
            Wavelets[1].SongID = SongID.Ultrafix;
            Wavelets[1].Enemies[0] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[0] = 3f * MathHelper.PiOver4;
            Wavelets[1].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[1] = MathHelper.Pi;
            Wavelets[1].Enemies[2] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[2] = MathHelper.PiOver2;
            Wavelets[1].Enemies[3] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[3] = MathHelper.PiOver4 * 7f;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(10, 0);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
            }
            #endregion

            Waves[GetIndex(3, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (3-3) "Metal Tooth attacks!
            #region Metadata
            Waves[GetIndex(3, 3)] = new GameWave();
            Waves[GetIndex(3, 3)].Background = 1;
            Waves[GetIndex(3, 3)].ThrobColor = new Color(84, 0, 255);
            Waves[GetIndex(3, 3)].ColorState = 0;
            Waves[GetIndex(3, 3)].MajorWaveNumber = 3;
            Waves[GetIndex(3, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(3, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(3, 3)].ParallaxElementTop.Intensity = 4;
            Waves[GetIndex(3, 3)].ParallaxElementTop.Tint = new Color(5, 255, 170);
            Waves[GetIndex(3, 3)].ParallaxElementTop.Speed = 0.7f;
            Waves[GetIndex(3, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(3, 3)].ParallaxElementBottom.Intensity = 4;
            Waves[GetIndex(3, 3)].ParallaxElementBottom.Tint = new Color(255, 99, 5);
            Waves[GetIndex(3, 3)].ParallaxElementBottom.Speed = -0.75f;
            Waves[GetIndex(3, 3)].Name = "Metal Tooth attacks!";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[0].SongID = SongID.Ultrafix;
            Wavelets[0].Enemies[0] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[0] = MathHelper.PiOver4;
            Wavelets[0].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[1] = MathHelper.Pi;
            Wavelets[0].Enemies[2] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[2] = MathHelper.PiOver2 * 3f;
            Wavelets[0].Enemies[3] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[3] = MathHelper.PiOver4 * 5f;
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(4, 0, ColorPolarity.Negative);
            Wavelets[1].SongID = SongID.Ultrafix;
            Wavelets[1].Enemies[0] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[0] = MathHelper.PiOver4;
            Wavelets[1].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[1] = 0f;
            Wavelets[1].Enemies[2] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[2] = MathHelper.PiOver2;
            Wavelets[1].Enemies[3] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[3] = MathHelper.PiOver4 * 5f;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(20, 0);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[2].Enemies.Length - 1; i++)
            {
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_MiniSaw;
                Wavelets[2].StartAngle[i] = 3f * MathHelper.PiOver2 + (float)i * (MathHelper.PiOver4 / (float)Wavelets[2].Enemies.Length);
            }
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 1] = ColorPolarity.Positive;
            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_MetalTooth;
            Wavelets[2].StartHitPoints[Wavelets[2].Enemies.Length - 1] = 3;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 1] = 7f * MathHelper.PiOver4;
            #endregion

            Waves[GetIndex(3, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 4
            #region WaveDef (4-1) "Into the swamps"
            #region Metadata
            Waves[GetIndex(4, 1)] = new GameWave();
            Waves[GetIndex(4, 1)].Background = 2;
            Waves[GetIndex(4, 1)].ThrobColor = new Color(121, 255, 9);
            Waves[GetIndex(4, 1)].ColorState = 2;
            Waves[GetIndex(4, 1)].MajorWaveNumber = 4;
            Waves[GetIndex(4, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(4, 1)].ParallaxElementTop = LocalInstanceManager.Background.EmptyParallaxElement;
            Waves[GetIndex(4, 1)].ParallaxElementBottom = LocalInstanceManager.Background.EmptyParallaxElement;
            Waves[GetIndex(4, 1)].Name = "Into the swamps";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(4, 0, ColorPolarity.Negative);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                Wavelets[0].StartAngle[i] = i * MathHelper.PiOver2;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 + MathHelper.PiOver4;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(8, 0);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                Wavelets[2].StartAngle[i] = i * MathHelper.PiOver2 + MathHelper.PiOver4;
            }
            #endregion

            Waves[GetIndex(4, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (4-2) "Food for maggots"
            #region Metadata
            Waves[GetIndex(4, 2)] = new GameWave();
            Waves[GetIndex(4, 2)].Background = 2;
            Waves[GetIndex(4, 2)].ThrobColor = new Color(121, 255, 9);
            Waves[GetIndex(4, 2)].ColorState = 2;
            Waves[GetIndex(4, 2)].MajorWaveNumber = 4;
            Waves[GetIndex(4, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(4, 2)].Name = "Food for maggots";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(4, 0, ColorPolarity.Negative);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                Wavelets[0].StartAngle[i] = i * MathHelper.PiOver2;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 + MathHelper.PiOver4;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(8, 0);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                Wavelets[2].StartAngle[i] = i * MathHelper.PiOver2 + MathHelper.PiOver4;
            }
            #endregion

            Waves[GetIndex(4, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (4-3) "Tupelo honey"
            #region Metadata
            Waves[GetIndex(4, 3)] = new GameWave();
            Waves[GetIndex(4, 3)].Background = 2;
            Waves[GetIndex(4, 3)].ThrobColor = new Color(121, 255, 9);
            Waves[GetIndex(4, 3)].ColorState = 2;
            Waves[GetIndex(4, 3)].MajorWaveNumber = 4;
            Waves[GetIndex(4, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(4, 3)].Name = "Tupelo honey";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(6, 0);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[0].StartAngle[i] = i * MathHelper.Pi / (float)Wavelets[0].Enemies.Length;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(6, 0);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[1].StartAngle[i] = MathHelper.Pi + i * MathHelper.Pi / (float)Wavelets[1].Enemies.Length;

            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(9, 0);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
            }
            #endregion

            Waves[GetIndex(4, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 5
            #region WaveDef (5-1) "Battle in the bog"
            #region Metadata
            Waves[GetIndex(5, 1)] = new GameWave();
            Waves[GetIndex(5, 1)].Background = 2;
            Waves[GetIndex(5, 1)].ThrobColor = new Color(121, 255, 9);
            Waves[GetIndex(5, 1)].ColorState = 2;
            Waves[GetIndex(5, 1)].MajorWaveNumber = 5;
            Waves[GetIndex(5, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(5, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(5, 1)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(5, 1)].ParallaxElementTop.Tint = new Color(116, 9, 225);
            Waves[GetIndex(5, 1)].ParallaxElementTop.Speed = 0.3f;
            Waves[GetIndex(5, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(5, 1)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(5, 1)].ParallaxElementBottom.Tint = new Color(255, 242, 95);
            Waves[GetIndex(5, 1)].ParallaxElementBottom.Speed = -0.2f;
            Waves[GetIndex(5, 1)].Name = "Battle in the bog";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(12, 0);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(16, 0);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length - 4; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)(Wavelets[1].Enemies.Length - 4);
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
            }
            for (int i = Wavelets[1].Enemies.Length - 4; i < Wavelets[1].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / 4f;
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(18, 0);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length - 6; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)(Wavelets[2].Enemies.Length - 6);
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
            }
            for (int i = Wavelets[2].Enemies.Length - 6; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 6f;
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
            }
            #endregion

            Waves[GetIndex(5, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (5-2) "War in the wetland"
            #region Metadata
            Waves[GetIndex(5, 2)] = new GameWave();
            Waves[GetIndex(5, 2)].Background = 2;
            Waves[GetIndex(5, 2)].ThrobColor = new Color(121, 255, 9);
            Waves[GetIndex(5, 2)].ColorState = 2;
            Waves[GetIndex(5, 2)].MajorWaveNumber = 5;
            Waves[GetIndex(5, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(5, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(5, 2)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(5, 2)].ParallaxElementTop.Tint = new Color(255, 242, 95);
            Waves[GetIndex(5, 2)].ParallaxElementTop.Speed = -0.3f;
            Waves[GetIndex(5, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(5, 2)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(5, 2)].ParallaxElementBottom.Tint = new Color(116, 9, 225);
            Waves[GetIndex(5, 2)].ParallaxElementBottom.Speed = 0.2f;
            Waves[GetIndex(5, 2)].Name = "War in the wetland";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(12, 0);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                }
                else
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                }
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(16, 0);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length - 4; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                }
                else
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                }
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)(Wavelets[1].Enemies.Length - 4);
            }
            for (int i = Wavelets[1].Enemies.Length - 4; i < Wavelets[1].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / 4f;
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(18, 0);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length - 6; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                }
                else
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                }
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)(Wavelets[2].Enemies.Length - 6);
            }
            for (int i = Wavelets[2].Enemies.Length - 6; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 6f;
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
            }
            #endregion

            Waves[GetIndex(5, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (5-3) "Oppugn in the peat"
            #region Metadata
            Waves[GetIndex(5, 3)] = new GameWave();
            Waves[GetIndex(5, 3)].Background = 2;
            Waves[GetIndex(5, 3)].ThrobColor = new Color(121, 255, 9);
            Waves[GetIndex(5, 3)].ColorState = 2;
            Waves[GetIndex(5, 3)].MajorWaveNumber = 5;
            Waves[GetIndex(5, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(5, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(5, 3)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(5, 3)].ParallaxElementTop.Tint = new Color(116, 9, 225);
            Waves[GetIndex(5, 3)].ParallaxElementTop.Speed = -0.3f;
            Waves[GetIndex(5, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(5, 3)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(5, 3)].ParallaxElementBottom.Tint = new Color(255, 242, 95); 
            Waves[GetIndex(5, 3)].ParallaxElementBottom.Speed = 0.2f;
            Waves[GetIndex(5, 3)].Name = "Oppugn in the peat";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(12, 0);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                }
                else
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                }
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(16, 0);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length - 6; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                }
                else
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                }
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)(Wavelets[1].Enemies.Length - 6);
            }
            for (int i = Wavelets[1].Enemies.Length - 6; i < Wavelets[1].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / 6f;
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(18, 0);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length - 6; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                }
                else
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                }
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)(Wavelets[2].Enemies.Length - 6);
            }
            for (int i = Wavelets[2].Enemies.Length - 6; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 6f;
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[2].StartHitPoints[i] = 3;
            }
            #endregion

            Waves[GetIndex(5, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 6 (Boss)
            #region WaveDef (6-1) "Land of the Gloops"
            #region Metadata
            Waves[GetIndex(6, 1)] = new GameWave();
            Waves[GetIndex(6, 1)].Background = 2;
            Waves[GetIndex(6, 1)].ThrobColor = new Color(121, 255, 9);
            Waves[GetIndex(6, 1)].ColorState = 2;
            Waves[GetIndex(6, 1)].MajorWaveNumber = 6;
            Waves[GetIndex(6, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(6, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(6, 1)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(6, 1)].ParallaxElementTop.Tint = new Color(255, 242, 95);
            Waves[GetIndex(6, 1)].ParallaxElementTop.Speed = -0.3f;
            Waves[GetIndex(6, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(6, 1)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(6, 1)].ParallaxElementBottom.Tint = new Color(116, 9, 225); 
            Waves[GetIndex(6, 1)].ParallaxElementBottom.Speed = 0.2f;
            Waves[GetIndex(6, 1)].Name = "Land of the Gloops";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(4, 0, ColorPolarity.Negative);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                Wavelets[0].StartAngle[i] = i * MathHelper.PiOver4 / 4f;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver4 / 4f + MathHelper.Pi;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(4, 0);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                Wavelets[2].StartAngle[i] = i * MathHelper.PiOver2;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
            }
            #endregion

            Waves[GetIndex(6, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (6-2) "Through the Gloop Court"
            #region Metadata
            Waves[GetIndex(6, 2)] = new GameWave();
            Waves[GetIndex(6, 2)].Background = 2;
            Waves[GetIndex(6, 2)].ThrobColor = new Color(121, 255, 9);
            Waves[GetIndex(6, 2)].ColorState = 2;
            Waves[GetIndex(6, 2)].MajorWaveNumber = 6;
            Waves[GetIndex(6, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(6, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(6, 2)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(6, 2)].ParallaxElementTop.Tint = new Color(116, 9, 225);
            Waves[GetIndex(6, 2)].ParallaxElementTop.Speed = -0.3f;
            Waves[GetIndex(6, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(6, 2)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(6, 2)].ParallaxElementBottom.Tint = new Color(255, 242, 95);
            Waves[GetIndex(6, 2)].ParallaxElementBottom.Speed = 0.2f;
            Waves[GetIndex(6, 2)].Name = "Through the Gloop Court";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(20, 0, ColorPolarity.Negative);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length - 1; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                Wavelets[0].StartAngle[i] = MathHelper.Pi;
            }
            Wavelets[0].Enemies[Wavelets[0].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[0].StartAngle[Wavelets[0].Enemies.Length - 1] = MathHelper.Pi;
            Wavelets[0].StartHitPoints[Wavelets[0].Enemies.Length - 1] = 4;
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(20, 0, ColorPolarity.Positive);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length - 1; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                Wavelets[1].StartAngle[i] = 0;
            }
            Wavelets[1].Enemies[Wavelets[1].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[1].StartAngle[Wavelets[1].Enemies.Length - 1] = 0;
            Wavelets[1].StartHitPoints[Wavelets[1].Enemies.Length - 1] = 4;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(40, 0);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].StartAngle[i] = MathHelper.Pi;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[2].StartAngle[i] = 0;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                }
            }
            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 2] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 2] = MathHelper.Pi;
            Wavelets[2].StartHitPoints[Wavelets[2].Enemies.Length - 2] = 4;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 2] = ColorPolarity.Positive;

            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 1] = 0;
            Wavelets[2].StartHitPoints[Wavelets[2].Enemies.Length - 1] = 4;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 1] = ColorPolarity.Negative;
            #endregion

            Waves[GetIndex(6, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (6-3) "King Gloop"
            #region Metadata
            Waves[GetIndex(6, 3)] = new GameWave();
            Waves[GetIndex(6, 3)].Background = 2;
            Waves[GetIndex(6, 3)].ThrobColor = new Color(121, 255, 9);
            Waves[GetIndex(6, 3)].ColorState = 2;
            Waves[GetIndex(6, 3)].MajorWaveNumber = 6;
            Waves[GetIndex(6, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(6, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(6, 3)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(6, 3)].ParallaxElementTop.Tint = new Color(255, 242, 95);
            Waves[GetIndex(6, 3)].ParallaxElementTop.Speed = -0.3f;
            Waves[GetIndex(6, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(6, 3)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(6, 3)].ParallaxElementBottom.Tint = new Color(116, 9, 225); 
            Waves[GetIndex(6, 3)].ParallaxElementBottom.Speed = 0.2f;
            Waves[GetIndex(6, 3)].Name = "King Gloop";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(25, 0);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length - 1; i++)
            {
                if (i < 20)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                    Wavelets[0].StartAngle[i] = MathHelper.Pi;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                    Wavelets[0].StartAngle[i] = i * MathHelper.PiOver4;
                    if(MWMathHelper.IsEven(i))
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                    else
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                }
            }
            Wavelets[0].Enemies[Wavelets[0].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[0].StartAngle[Wavelets[0].Enemies.Length - 1] = MathHelper.Pi;
            Wavelets[0].StartHitPoints[Wavelets[0].Enemies.Length - 1] = 2;
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(30, 0);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length - 1; i++)
            {
                if (i < 24)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                    Wavelets[1].StartAngle[i] = 0;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[1].StartAngle[i] = i * MathHelper.PiOver4;
                    if (MWMathHelper.IsEven(i))
                    {
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                        Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                    }
                    else
                    {
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                        Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                    }
                }
            }
            Wavelets[1].Enemies[Wavelets[1].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[1].StartAngle[Wavelets[1].Enemies.Length - 1] = 0;
            Wavelets[1].StartHitPoints[Wavelets[1].Enemies.Length - 1] = 2;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 0, ColorPolarity.Negative);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length - 1; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                Wavelets[2].StartAngle[i] = i * MathHelper.Pi / (float)Wavelets[2].Enemies.Length + 3f * MathHelper.PiOver4;
            }
            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_KingGloop;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 1] = 5f * MathHelper.PiOver4;
            Wavelets[2].StartHitPoints[Wavelets[2].Enemies.Length - 1] = 5;
            #endregion

            Waves[GetIndex(6, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 7
            #region WaveDef (7-1) "Into the desert"
            #region Metadata
            Waves[GetIndex(7, 1)] = new GameWave();
            Waves[GetIndex(7, 1)].Background = 4;
            Waves[GetIndex(7, 1)].ThrobColor = new Color(255, 214, 108);
            Waves[GetIndex(7, 1)].ColorState = 0;
            Waves[GetIndex(7, 1)].MajorWaveNumber = 7;
            Waves[GetIndex(7, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(7, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(7, 1)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(7, 1)].ParallaxElementTop.Tint = new Color(229, 217, 186);
            Waves[GetIndex(7, 1)].ParallaxElementTop.Speed = -0.6f;
            Waves[GetIndex(7, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(7, 1)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(7, 1)].ParallaxElementBottom.Tint = new Color(229, 217, 186);
            Waves[GetIndex(7, 1)].ParallaxElementBottom.Speed = 0.5f;
            Waves[GetIndex(7, 1)].Name = "Into the desert";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(6, 2, ColorPolarity.Negative);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(6, 2, ColorPolarity.Positive);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].Enemies.Length;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(6, 2);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                if(MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
            }
            #endregion

            Waves[GetIndex(7, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (7-2) "Spitter spatter"
            #region Metadata
            Waves[GetIndex(7, 2)] = new GameWave();
            Waves[GetIndex(7, 2)].Background = 4;
            Waves[GetIndex(7, 2)].ThrobColor = new Color(255, 214, 108);
            Waves[GetIndex(7, 2)].ColorState = 0;
            Waves[GetIndex(7, 2)].MajorWaveNumber = 7;
            Waves[GetIndex(7, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(7, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(7, 2)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(7, 2)].ParallaxElementTop.Tint = new Color(229, 217, 186);
            Waves[GetIndex(7, 2)].ParallaxElementTop.Speed = -0.65f;
            Waves[GetIndex(7, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(7, 2)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(7, 2)].ParallaxElementBottom.Tint = new Color(229, 217, 186);
            Waves[GetIndex(7, 2)].ParallaxElementBottom.Speed = 0.55f;
            Waves[GetIndex(7, 2)].Name = "Spitter spatter";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(4, 0, ColorPolarity.Negative);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(8, 0);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].Enemies.Length;
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                if(MWMathHelper.IsEven(i))
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(10, 0);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
                if (i < Wavelets[2].Enemies.Length / 3f)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;

                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
            }
            #endregion

            Waves[GetIndex(7, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (7-3) "Reinforcements"
            #region Metadata
            Waves[GetIndex(7, 3)] = new GameWave();
            Waves[GetIndex(7, 3)].Background = 4;
            Waves[GetIndex(7, 3)].ThrobColor = new Color(255, 214, 108);
            Waves[GetIndex(7, 3)].ColorState = 0;
            Waves[GetIndex(7, 3)].MajorWaveNumber = 7;
            Waves[GetIndex(7, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(7, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(7, 3)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(7, 3)].ParallaxElementTop.Tint = new Color(229, 217, 186);
            Waves[GetIndex(7, 3)].ParallaxElementTop.Speed = -0.7f;
            Waves[GetIndex(7, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(7, 3)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(7, 3)].ParallaxElementBottom.Tint = new Color(229, 217, 186);
            Waves[GetIndex(7, 3)].ParallaxElementBottom.Speed = 0.6f;
            Waves[GetIndex(7, 3)].Name = "Reinforcements";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(30, 0);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (i > Wavelets[0].Enemies.Length - 3)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                    Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / 2f;
                    Wavelets[0].StartHitPoints[i] = 1;
                    if (MWMathHelper.IsEven(i))
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                    else
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else
                {
                    Wavelets[0].StartAngle[i] = (float)InstanceManager.Random.NextDouble() * MathHelper.TwoPi;
                    Wavelets[0].ColorPolarities[i] = ColorState.RandomPolarity();
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(30, 0);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                if (i > Wavelets[1].Enemies.Length - 3)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                    Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / 2f;
                    Wavelets[1].StartHitPoints[i] = 1;
                    if (MWMathHelper.IsEven(i))
                        Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                    else
                        Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else if (i > Wavelets[1].Enemies.Length - 13)
                {
                    if (MWMathHelper.IsEven(i))
                    {
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                    }
                    else
                    {
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    }
                    Wavelets[1].ColorPolarities[i] = ColorState.RandomPolarity();
                    Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / 10f;
                }
                else
                {
                    Wavelets[1].StartAngle[i] = (float)InstanceManager.Random.NextDouble() * MathHelper.TwoPi;
                    Wavelets[1].ColorPolarities[i] = ColorState.RandomPolarity();
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(30, 0);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (i > Wavelets[2].Enemies.Length - 3)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                    Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 2f;
                    Wavelets[2].StartHitPoints[i] = 1;
                    if (MWMathHelper.IsEven(i))
                        Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                    else
                        Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else if (i > Wavelets[2].Enemies.Length - 13)
                {
                    if (MWMathHelper.IsEven(i))
                    {
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                    }
                    else
                    {
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    }
                    Wavelets[2].ColorPolarities[i] = ColorState.RandomPolarity();
                    Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 10f;
                }
                else
                {
                    Wavelets[2].StartAngle[i] = (float)InstanceManager.Random.NextDouble() * MathHelper.TwoPi;
                    Wavelets[2].ColorPolarities[i] = ColorState.RandomPolarity();
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                }
            }
            #endregion

            Waves[GetIndex(7, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 8
            #region WaveDef (8-1) "...did you see that?"
            #region Metadata
            Waves[GetIndex(8, 1)] = new GameWave();
            Waves[GetIndex(8, 1)].Background = 4;
            Waves[GetIndex(8, 1)].ThrobColor = new Color(255, 214, 108);
            Waves[GetIndex(8, 1)].ColorState = 2;
            Waves[GetIndex(8, 1)].MajorWaveNumber = 8;
            Waves[GetIndex(8, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(8, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(8, 1)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(8, 1)].ParallaxElementTop.Tint = new Color(229, 217, 186);
            Waves[GetIndex(8, 1)].ParallaxElementTop.Speed = -0.75f;
            Waves[GetIndex(8, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(8, 1)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(8, 1)].ParallaxElementBottom.Tint = new Color(229, 217, 186);
            Waves[GetIndex(8, 1)].ParallaxElementBottom.Speed = 0.65f;
            Waves[GetIndex(8, 1)].Name = "...did you see that?";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                Wavelets[0].StartAngle[i] = i * MathHelper.PiOver2 / (float)Wavelets[0].Enemies.Length + MathHelper.PiOver4;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(6, 0);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length - 2; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 / (float)(Wavelets[1].Enemies.Length - 2) + MathHelper.PiOver4;
                if(MWMathHelper.IsEven(i))
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[1].Enemies.Length - 2; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 / 2f + MathHelper.PiOver4 * 5f;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(10, 0);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length - 3; i++)
            {
                if (i < 5)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;

                Wavelets[2].StartAngle[i] = i * MathHelper.PiOver2 / (float)(Wavelets[2].Enemies.Length - 2) + MathHelper.PiOver4;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[2].Enemies.Length - 3; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].StartAngle[i] = i * MathHelper.PiOver2 / 3f + MathHelper.PiOver4 * 5f;
            }
            #endregion

            Waves[GetIndex(8, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (8-2) "...swear I saw something"
            #region Metadata
            Waves[GetIndex(8, 2)] = new GameWave();
            Waves[GetIndex(8, 2)].Background = 4;
            Waves[GetIndex(8, 2)].ThrobColor = new Color(255, 214, 108);
            Waves[GetIndex(8, 2)].ColorState = 2;
            Waves[GetIndex(8, 2)].MajorWaveNumber = 8;
            Waves[GetIndex(8, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(8, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(8, 2)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(8, 2)].ParallaxElementTop.Tint = new Color(229, 217, 186);
            Waves[GetIndex(8, 2)].ParallaxElementTop.Speed = -0.8f;
            Waves[GetIndex(8, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(8, 2)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(8, 2)].ParallaxElementBottom.Tint = new Color(229, 217, 186);
            Waves[GetIndex(8, 2)].ParallaxElementBottom.Speed = 0.7f;
            Waves[GetIndex(8, 2)].Name = "...swear I saw something";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(30, 0);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (i < 5)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                    Wavelets[0].StartHitPoints[i] = 4;
                    Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / 5f;
                    if (MWMathHelper.IsEven(i))
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                    else
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else if (i > Wavelets[0].Enemies.Length - 2)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[0].StartHitPoints[i] = 1;
                    Wavelets[0].StartAngle[i] = i * MathHelper.Pi / 2f;
                }
                else
                {
                    if (MWMathHelper.IsEven(i))
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                    else
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / 25f;
                    Wavelets[0].StartHitPoints[i] = 2;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(30, 0);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                if (i < 4)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                    Wavelets[1].StartHitPoints[i] = 4;
                    Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / 4f + MathHelper.PiOver4;
                    if (MWMathHelper.IsEven(i))
                        Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                    else
                        Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else if (i == 7)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                    Wavelets[1].StartAngle[i] = 0;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else if (i > Wavelets[1].Enemies.Length - 2)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[1].StartHitPoints[i] = 2;
                    Wavelets[1].StartAngle[i] = i * MathHelper.Pi / 2f;
                }
                else
                {
                    if (MWMathHelper.IsEven(i))
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                    else
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / 25f;
                    Wavelets[1].StartHitPoints[i] = 2;
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(40, 0);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (i < 4)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                    Wavelets[2].StartHitPoints[i] = 4;
                    Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 4f + MathHelper.PiOver4;
                    if (MWMathHelper.IsEven(i))
                        Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    else
                        Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else if (i > 4 && i < 10)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                    Wavelets[2].StartAngle[i] = 0;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else if (i > Wavelets[2].Enemies.Length - 2)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[2].StartHitPoints[i] = 1;
                    Wavelets[2].StartAngle[i] = i * MathHelper.Pi / 2f;
                }
                else
                {
                    if (MWMathHelper.IsEven(i))
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                    else
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 30f;
                    Wavelets[2].StartHitPoints[i] = 2;
                }
            }
            #endregion

            Waves[GetIndex(8, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (8-3) "Dust storm is getting bad"
            #region Metadata
            Waves[GetIndex(8, 3)] = new GameWave();
            Waves[GetIndex(8, 3)].Background = 4;
            Waves[GetIndex(8, 3)].ThrobColor = new Color(255, 214, 108);
            Waves[GetIndex(8, 3)].ColorState = 0;
            Waves[GetIndex(8, 3)].MajorWaveNumber = 8;
            Waves[GetIndex(8, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(8, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(8, 3)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(8, 3)].ParallaxElementTop.Tint = new Color(229, 217, 186);
            Waves[GetIndex(8, 3)].ParallaxElementTop.Speed = -0.85f;
            Waves[GetIndex(8, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(8, 3)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(8, 3)].ParallaxElementBottom.Tint = new Color(229, 217, 186);
            Waves[GetIndex(8, 3)].ParallaxElementBottom.Speed = 0.75f;
            Waves[GetIndex(8, 3)].Name = "Dust storm is getting bad";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(25, 0);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (i < 5)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                    Wavelets[0].StartHitPoints[i] = 1;
                    Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / 5f;
                    if (MWMathHelper.IsEven(i))
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                    else
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else if (i > Wavelets[0].Enemies.Length - 2)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[0].StartHitPoints[i] = 2;
                    Wavelets[0].StartAngle[i] = i * MathHelper.Pi / 2f;
                }
                else
                {
                    if (MWMathHelper.IsEven(i))
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                    else
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / 20f;
                    Wavelets[0].StartHitPoints[i] = 2;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(40, 0);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length/2f; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                Wavelets[1].StartAngle[i] = MathHelper.PiOver2 * 2f;
            }
            for (int i = (int)(Wavelets[1].Enemies.Length / 2f) - 1; i < Wavelets[1].Enemies.Length; i++)
            {
                if (i == Wavelets[1].Enemies.Length - 2)
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                else if (i == Wavelets[1].Enemies.Length - 1)
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                else
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                Wavelets[1].StartAngle[i] = MathHelper.PiOver2 * 2f;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(60, 0);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length / 2f; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].StartAngle[i] = MathHelper.PiOver2;
            }
            for (int i = (int)(Wavelets[2].Enemies.Length / 2f) - 1; i < Wavelets[2].Enemies.Length; i++)
            {
                if (i == Wavelets[2].Enemies.Length - 2)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                else if (i == Wavelets[2].Enemies.Length - 1)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].StartAngle[i] = MathHelper.PiOver2;
            }
            #endregion

            Waves[GetIndex(8, 1)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Commented out wave defs
            /*
            #region WaveDef (3-1) "Wave after wave..."
            #region Metadata
            Waves[GetIndex(3, 1)] = new GameWave();
            Waves[GetIndex(3, 1)].Background = 1;
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
            // Second wavelet FIXME PROBLEM HERE GET A NULL EXCEPTION FOR THIS DEF?
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
             * */
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
            //Console.Write("Requested {0}-{1} : Set to ", MajorNum, MinorNum);
            if (MinorNum > GameWaveManager.MaxMinorNumber)
                MinorNum = GameWaveManager.MaxMinorNumber;
            else if (MinorNum < 1)
                MinorNum = 1;
            int index = MajorNum * GameWaveManager.MaxMinorNumber -
                GameWaveManager.MaxMinorNumber + MinorNum - 1;
            //Console.WriteLine("{0}-{1} i={2} out of {3}", MajorNum, MinorNum, index, numberOfWaves);
            if (index >= numberOfWavelets)
                throw new WavesOutOfRangeException();
            return index;
        }
        #endregion
    }
}
