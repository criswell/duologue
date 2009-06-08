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
        private const int maxNumberOfMajorWaves = 18;
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
                    + 3f * MathHelper.PiOver2;
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
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver2 / (Wavelets[2].Enemies.Length / 2f)
                    + MathHelper.PiOver2;
            }
            for (int i = (int)(Wavelets[2].Enemies.Length / 2f); i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver2 / (Wavelets[2].Enemies.Length / 2f)
                    + 3f * MathHelper.PiOver2;
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
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver2 / (Wavelets[2].Enemies.Length / 2f);
            }
            for (int i = (int)(Wavelets[2].Enemies.Length / 2f); i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver2 / (Wavelets[2].Enemies.Length / 2f)
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
                    3f * MathHelper.PiOver2;
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
            Wavelets[0].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                Wavelets[0].StartAngle[i] = i * MathHelper.PiOver2;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[1].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 + MathHelper.PiOver4;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(8, 0);
            Wavelets[2].SongID = SongID.WinOne;
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
            Wavelets[0].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                Wavelets[0].StartAngle[i] = i * MathHelper.PiOver2;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[1].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 + MathHelper.PiOver4;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(8, 0);
            Wavelets[2].SongID = SongID.WinOne;
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
            Wavelets[0].SongID = SongID.WinOne;
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
            Wavelets[1].SongID = SongID.WinOne;
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
            Wavelets[2].SongID = SongID.WinOne;
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
            Wavelets[0].SongID = SongID.WinOne;
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
            Wavelets[1].SongID = SongID.WinOne;
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
            Wavelets[2].SongID = SongID.WinOne;
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
            Wavelets[0].SongID = SongID.WinOne;
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
            Wavelets[1].SongID = SongID.WinOne;
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
            Wavelets[2].SongID = SongID.WinOne;
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
            Wavelets[0].SongID = SongID.WinOne;
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
            Wavelets[1].SongID = SongID.WinOne;
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
            Wavelets[2].SongID = SongID.WinOne;
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
            Wavelets[0].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                Wavelets[0].StartAngle[i] = i * MathHelper.PiOver4 / 4f;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[1].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver4 / 4f + MathHelper.Pi;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(4, 0);
            Wavelets[2].SongID = SongID.WinOne;
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
            Wavelets[0].SongID = SongID.WinOne;
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
            Wavelets[1].SongID = SongID.WinOne;
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
            Wavelets[2].SongID = SongID.WinOne;
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
            Wavelets[2].StartHitPoints[Wavelets[2].Enemies.Length - 2] = 6;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 2] = ColorPolarity.Positive;

            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 1] = 0;
            Wavelets[2].StartHitPoints[Wavelets[2].Enemies.Length - 1] = 6;
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
            Wavelets[0].SongID = SongID.WinOne;
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
            Wavelets[1].SongID = SongID.WinOne;
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
            Wavelets[2].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[2].Enemies.Length - 1; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                Wavelets[2].StartAngle[i] = 5f * MathHelper.PiOver4;//i * MathHelper.Pi / (float)Wavelets[2].Enemies.Length + 3f * MathHelper.PiOver4;
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
                    Wavelets[0].StartHitPoints[i] = 2;
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
                if (i > Wavelets[1].Enemies.Length - 2)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                    Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / 2f;
                    Wavelets[1].StartHitPoints[i] = 0;
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
                if (i > Wavelets[2].Enemies.Length - 2)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                    Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 2f;
                    Wavelets[2].StartHitPoints[i] = 0;
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
            Wavelets[0] = new Wavelet(2, 0, ColorPolarity.Positive);
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
            Wavelets[0] = new Wavelet(20, 0);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (i < 5)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                    Wavelets[0].StartHitPoints[i] = 2;
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
                    Wavelets[0].StartHitPoints[i] = 0;
                    Wavelets[0].StartAngle[i] = i * MathHelper.Pi / 2f;
                }
                else
                {
                    if (MWMathHelper.IsEven(i))
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                    else
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / 15f;
                    Wavelets[0].StartHitPoints[i] = 1;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(25, 0);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                if (i < 4)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                    Wavelets[1].StartHitPoints[i] = 2;
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
                    Wavelets[1].StartHitPoints[i] = 0;
                    Wavelets[1].StartAngle[i] = i * MathHelper.Pi / 2f;
                }
                else
                {
                    if (MWMathHelper.IsEven(i))
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                    else
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / 20f;
                    Wavelets[1].StartHitPoints[i] = 2;
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(30, 0);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (i < 4)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                    Wavelets[2].StartHitPoints[i] = 2;
                    Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 4f + MathHelper.PiOver4;
                    if (MWMathHelper.IsEven(i))
                        Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    else
                        Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else if (i > 4 && i < 8)
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
                    Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 25f;
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
                    Wavelets[0].StartHitPoints[i] = 0;
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
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / 20f;
                    Wavelets[0].StartHitPoints[i] = 0;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(30, 0);
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
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[1].StartHitPoints[i] = 5;
                }
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
            Wavelets[2] = new Wavelet(35, 0);
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
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[2].StartHitPoints[i] = 5;
                }
                else if (i == Wavelets[2].Enemies.Length - 1)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].StartAngle[i] = MathHelper.PiOver2;
            }
            #endregion

            Waves[GetIndex(8, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 9 (Boss)
            #region WaveDef (9-1) "Alien haboob"
            #region Metadata
            Waves[GetIndex(9, 1)] = new GameWave();
            Waves[GetIndex(9, 1)].Background = 4;
            Waves[GetIndex(9, 1)].ThrobColor = new Color(255, 214, 108);
            Waves[GetIndex(9, 1)].ColorState = 0;
            Waves[GetIndex(9, 1)].MajorWaveNumber = 9;
            Waves[GetIndex(9, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(9, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(9, 1)].ParallaxElementTop.Intensity = 4;
            Waves[GetIndex(9, 1)].ParallaxElementTop.Tint = new Color(229, 217, 186);
            Waves[GetIndex(9, 1)].ParallaxElementTop.Speed = -0.9f;
            Waves[GetIndex(9, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(9, 1)].ParallaxElementBottom.Intensity = 4;
            Waves[GetIndex(9, 1)].ParallaxElementBottom.Tint = new Color(229, 217, 186);
            Waves[GetIndex(9, 1)].ParallaxElementBottom.Speed = 0.8f;
            Waves[GetIndex(9, 1)].Name = "Alien haboob";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(50, 0);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (i == Wavelets[0].Enemies.Length - 2)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[0].StartAngle[i] = 0;
                    Wavelets[0].StartHitPoints[i] = 5;
                }
                else if (i == Wavelets[0].Enemies.Length - 1)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[0].StartAngle[i] = MathHelper.Pi;
                    Wavelets[0].StartHitPoints[i] = 5;
                }
                else
                {
                    if (MWMathHelper.IsEven(i))
                        Wavelets[0].StartAngle[i] = MathHelper.Pi;
                    else
                        Wavelets[0].StartAngle[i] = 0;
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(50, 0);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                if (i == Wavelets[1].Enemies.Length - 4)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[1].StartAngle[i] = 3f * MathHelper.PiOver4;
                    Wavelets[1].StartHitPoints[i] = 5;
                }
                else if (i == Wavelets[1].Enemies.Length - 3)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[1].StartAngle[i] = 5f * MathHelper.PiOver4;
                    Wavelets[1].StartHitPoints[i] = 5;
                }
                else if (i == Wavelets[1].Enemies.Length - 2)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[1].StartAngle[i] = 7f * MathHelper.PiOver4;
                    Wavelets[1].StartHitPoints[i] = 5;
                }
                else if (i == Wavelets[1].Enemies.Length - 1)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[1].StartAngle[i] = MathHelper.PiOver4;
                    Wavelets[1].StartHitPoints[i] = 5;
                }
                else
                {
                    if (MWMathHelper.IsEven(i))
                        Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 / 20f + 3f * MathHelper.PiOver4;
                    else
                        Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 / 20f + 7f * MathHelper.PiOver4;
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 0);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (i == Wavelets[2].Enemies.Length - 4)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[2].StartAngle[i] = 0;
                    Wavelets[2].StartHitPoints[i] = 5;
                }
                else if (i == Wavelets[2].Enemies.Length - 3)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[2].StartAngle[i] = MathHelper.PiOver2;
                    Wavelets[2].StartHitPoints[i] = 5;
                }
                else if (i == Wavelets[2].Enemies.Length - 2)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[2].StartAngle[i] = MathHelper.Pi;
                    Wavelets[2].StartHitPoints[i] = 5;
                }
                else if (i == Wavelets[2].Enemies.Length - 1)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[2].StartAngle[i] = 3f * MathHelper.PiOver2;
                    Wavelets[2].StartHitPoints[i] = 5;
                }
                else
                {
                    Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / Wavelets[2].Enemies.Length;
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
            }
            #endregion

            Waves[GetIndex(9, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (9-2) "Eye of the storm"
            #region Metadata
            Waves[GetIndex(9, 2)] = new GameWave();
            Waves[GetIndex(9, 2)].Background = 4;
            Waves[GetIndex(9, 2)].ThrobColor = new Color(255, 214, 108);
            Waves[GetIndex(9, 2)].ColorState = 2;
            Waves[GetIndex(9, 2)].MajorWaveNumber = 9;
            Waves[GetIndex(9, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(9, 2)].ParallaxElementTop = LocalInstanceManager.Background.EmptyParallaxElement;
            Waves[GetIndex(9, 2)].ParallaxElementBottom = LocalInstanceManager.Background.EmptyParallaxElement;
            Waves[GetIndex(9, 2)].Name = "Eye of the storm";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(3, 0, ColorPolarity.Positive);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                Wavelets[0].StartAngle[i] = i * MathHelper.PiOver2 / (float)Wavelets[0].Enemies.Length + MathHelper.PiOver4;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(3, 0, ColorPolarity.Negative);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 / (float)Wavelets[1].Enemies.Length + 5f * MathHelper.PiOver4;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(4, 0, ColorPolarity.Positive);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
            }
            #endregion

            Waves[GetIndex(9, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (9-3) "Unclean Rot"
            #region Metadata
            Waves[GetIndex(9, 3)] = new GameWave();
            Waves[GetIndex(9, 3)].Background = 4;
            Waves[GetIndex(9, 3)].ThrobColor = new Color(255, 214, 108);
            Waves[GetIndex(9, 3)].ColorState = 0;
            Waves[GetIndex(9, 3)].MajorWaveNumber = 9;
            Waves[GetIndex(9, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(9, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(9, 3)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(9, 3)].ParallaxElementTop.Tint = new Color(229, 217, 186);
            Waves[GetIndex(9, 3)].ParallaxElementTop.Speed = 0.7f;
            Waves[GetIndex(9, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(9, 3)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(9, 3)].ParallaxElementBottom.Tint = new Color(229, 217, 186);
            Waves[GetIndex(9, 3)].ParallaxElementBottom.Speed = -0.65f;
            Waves[GetIndex(9, 3)].Name = "Unclean Rot";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(4, 0);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(4, 0);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(60, 2, ColorPolarity.Positive);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                Wavelets[2].StartAngle[i] = MathHelper.Pi;
            }
            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_UncleanRot;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 1] = 5f * MathHelper.PiOver4;
            Wavelets[2].StartHitPoints[Wavelets[2].Enemies.Length - 1] = 4;
            #endregion

            Waves[GetIndex(9, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 10
            #region WaveDef (10-1) "AnnMoebic Dysentery"
            #region Metadata
            Waves[GetIndex(10, 1)] = new GameWave();
            Waves[GetIndex(10, 1)].Background = 3;
            Waves[GetIndex(10, 1)].ThrobColor = new Color(119, 203, 191);
            Waves[GetIndex(10, 1)].ColorState = 0;
            Waves[GetIndex(10, 1)].MajorWaveNumber = 10;
            Waves[GetIndex(10, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(10, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(10, 1)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(10, 1)].ParallaxElementTop.Tint = new Color(170, 255, 170);
            Waves[GetIndex(10, 1)].ParallaxElementTop.Speed = 0.4f;
            Waves[GetIndex(10, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(10, 1)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(10, 1)].ParallaxElementBottom.Tint = new Color(170, 255, 170);
            Waves[GetIndex(10, 1)].ParallaxElementBottom.Speed = 0.3f;
            Waves[GetIndex(10, 1)].Name = "AnnMoebic Dysentery";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(10, 0, ColorPolarity.Negative);
            Wavelets[0].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[0].StartAngle[i] = 0;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(20, 0, ColorPolarity.Positive);
            Wavelets[1].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[1].StartAngle[i] = 0;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(40, 0);
            Wavelets[2].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[2].StartAngle[i] = 0;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
            }
            #endregion

            Waves[GetIndex(10, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (10-2) "Benthopelagic Bestiary"
            #region Metadata
            Waves[GetIndex(10, 2)] = new GameWave();
            Waves[GetIndex(10, 2)].Background = 3;
            Waves[GetIndex(10, 2)].ThrobColor = new Color(119, 203, 191);
            Waves[GetIndex(10, 2)].ColorState = 2;
            Waves[GetIndex(10, 2)].MajorWaveNumber = 10;
            Waves[GetIndex(10, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(10, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(10, 2)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(10, 2)].ParallaxElementTop.Tint = new Color(203, 238, 169);
            Waves[GetIndex(10, 2)].ParallaxElementTop.Speed = 0.4f;
            Waves[GetIndex(10, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(10, 2)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(10, 2)].ParallaxElementBottom.Tint = new Color(203, 238, 169);
            Waves[GetIndex(10, 2)].ParallaxElementBottom.Speed = 0.3f;
            Waves[GetIndex(10, 2)].Name = "Benthopelagic Bestiary";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(40, 0);
            Wavelets[0].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / Wavelets[0].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;

                if (i / 4f == i / 4)
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                else
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(40, 0);
            Wavelets[1].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / Wavelets[1].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;

                if (i / 4f == i / 4)
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                else
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(40, 0);
            Wavelets[2].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / Wavelets[2].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;

                if (i >= Wavelets[2].Enemies.Length-2)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                else if (i / 4f == i / 4)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                else if (i / 5f == i / 5)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
            }
            #endregion

            Waves[GetIndex(10, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (10-3) "Attenuated Turbidity"
            #region Metadata
            Waves[GetIndex(10, 3)] = new GameWave();
            Waves[GetIndex(10, 3)].Background = 3;
            Waves[GetIndex(10, 3)].ThrobColor = new Color(119, 203, 191);
            Waves[GetIndex(10, 3)].ColorState = 2;
            Waves[GetIndex(10, 3)].MajorWaveNumber = 10;
            Waves[GetIndex(10, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(10, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(10, 3)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(10, 3)].ParallaxElementTop.Tint = new Color(203, 238, 169);
            Waves[GetIndex(10, 3)].ParallaxElementTop.Speed = 0.4f;
            Waves[GetIndex(10, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(10, 3)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(10, 3)].ParallaxElementBottom.Tint = new Color(203, 238, 169);
            Waves[GetIndex(10, 3)].ParallaxElementBottom.Speed = 0.3f;
            Waves[GetIndex(10, 3)].Name = "Attenuated Turbidity";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(40, 1, ColorPolarity.Negative);
            Wavelets[0].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[0].StartAngle[i] = 0;
            }
            Wavelets[0].Enemies[Wavelets[0].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[0].StartHitPoints[Wavelets[0].Enemies.Length - 1] = 15;
            Wavelets[0].StartAngle[Wavelets[0].Enemies.Length - 1] = MathHelper.PiOver2 * 3f;
            Wavelets[0].ColorPolarities[Wavelets[0].Enemies.Length - 1] = ColorPolarity.Positive;
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(40, 0);
            Wavelets[1].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / Wavelets[1].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;

                if (i / 4f == i / 4)
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                else if (i / 3f == i / 3)
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                else
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
            }
            Wavelets[1].Enemies[Wavelets[1].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[1].StartHitPoints[Wavelets[0].Enemies.Length - 1] = 15;
            Wavelets[1].StartAngle[Wavelets[1].Enemies.Length - 1] = MathHelper.Pi;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(40, 0);
            Wavelets[2].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / Wavelets[2].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;

                if (i >= Wavelets[2].Enemies.Length - 2) {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[2].StartHitPoints[i] = 15;
                }
                else if (i / 4f == i / 4)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                else if (i / 3f == i / 3)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                else if (i / 5f == i / 5)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
            }
            #endregion

            Waves[GetIndex(10, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 11
            #region Wave (11-1) "Okeanos Salinity"
            #region Metadata
            Waves[GetIndex(11, 1)] = new GameWave();
            Waves[GetIndex(11, 1)].Background = 3;
            Waves[GetIndex(11, 1)].ThrobColor = new Color(119, 203, 191);
            Waves[GetIndex(11, 1)].ColorState = 0;
            Waves[GetIndex(11, 1)].MajorWaveNumber = 11;
            Waves[GetIndex(11, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(11, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(11, 1)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(11, 1)].ParallaxElementTop.Tint = new Color(83, 167, 0);
            Waves[GetIndex(11, 1)].ParallaxElementTop.Speed = 0.4f;
            Waves[GetIndex(11, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(11, 1)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(11, 1)].ParallaxElementBottom.Tint = new Color(83, 167, 0);
            Waves[GetIndex(11, 1)].ParallaxElementBottom.Speed = 0.3f;
            Waves[GetIndex(11, 1)].Name = "Okeanos Salinity";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(50, 1);
            Wavelets[0].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[0].Enemies.Length - 20; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[0].StartAngle[i] = 0;
                if (MWMathHelper.IsEven(i))
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[0].Enemies.Length - 20; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / 20f;
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                if ((Wavelets[0].StartAngle[i] < MathHelper.PiOver4 || Wavelets[0].StartAngle[i] > 7f * MathHelper.PiOver4) ||
                    (Wavelets[0].StartAngle[i] >= 3f * MathHelper.PiOver4 && Wavelets[0].StartAngle[i] < 5f * MathHelper.PiOver4))
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                }

                if ((Wavelets[0].Enemies.Length - i) / 4f == (Wavelets[0].Enemies.Length - i) / 4)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[0].StartHitPoints[i] = 15;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(50, 1);
            Wavelets[1].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[1].Enemies.Length - 20; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[1].StartAngle[i] = 0;
                if (MWMathHelper.IsEven(i))
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[1].Enemies.Length - 20; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / 20f;
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                if ((Wavelets[1].StartAngle[i] < MathHelper.PiOver4 || Wavelets[1].StartAngle[i] > 7f * MathHelper.PiOver4) ||
                    (Wavelets[1].StartAngle[i] >= 3f * MathHelper.PiOver4 && Wavelets[1].StartAngle[i] < 5f * MathHelper.PiOver4))
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }

                if ((Wavelets[1].Enemies.Length - i) / 4f == (Wavelets[1].Enemies.Length - i) / 4)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[1].StartHitPoints[i] = 15;
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 1);
            Wavelets[2].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[2].Enemies.Length - 20; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[2].StartAngle[i] = 0;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[2].Enemies.Length - 20; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 20f;
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                if (Wavelets[2].StartAngle[i] < 3f * MathHelper.PiOver4 || Wavelets[2].StartAngle[i] > 7f * MathHelper.PiOver4)
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else 
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                }

                if ((Wavelets[2].Enemies.Length - i) / 4f == (Wavelets[2].Enemies.Length - i) / 4)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[2].StartHitPoints[i] = 15;
                }
            }
            #endregion

            Waves[GetIndex(11, 1)].Wavelets = Wavelets;
            #endregion

            #region Wave (11-2) "Nereus and Doris"
            #region Metadata
            Waves[GetIndex(11, 2)] = new GameWave();
            Waves[GetIndex(11, 2)].Background = 3;
            Waves[GetIndex(11, 2)].ThrobColor = new Color(119, 203, 191);
            Waves[GetIndex(11, 2)].ColorState = 0;
            Waves[GetIndex(11, 2)].MajorWaveNumber = 11;
            Waves[GetIndex(11, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(11, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(11, 2)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(11, 2)].ParallaxElementTop.Tint = new Color(83, 167, 0);
            Waves[GetIndex(11, 2)].ParallaxElementTop.Speed = 0.4f;
            Waves[GetIndex(11, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(11, 2)].ParallaxElementBottom.Intensity = 3;
            Waves[GetIndex(11, 2)].ParallaxElementBottom.Tint = new Color(83, 167, 0);
            Waves[GetIndex(11, 2)].ParallaxElementBottom.Speed = 0.3f;
            Waves[GetIndex(11, 2)].Name = "Nereus and Doris";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(50, 0);
            Wavelets[0].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[0].Enemies.Length - 10; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[0].StartAngle[i] = 0;
                if (MWMathHelper.IsEven(i))
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[0].Enemies.Length - 10; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / 20f;
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                if ((Wavelets[0].StartAngle[i] < MathHelper.PiOver4 || Wavelets[0].StartAngle[i] > 7f * MathHelper.PiOver4) ||
                    (Wavelets[0].StartAngle[i] >= 3f * MathHelper.PiOver4 && Wavelets[0].StartAngle[i] < 5f * MathHelper.PiOver4))
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(50, 0);
            Wavelets[1].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[1].Enemies.Length - 10; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[1].StartAngle[i] = 0;
                if (MWMathHelper.IsEven(i))
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[1].Enemies.Length - 10; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / 20f;
                if (MWMathHelper.IsEven(i))
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                else
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                if ((Wavelets[1].StartAngle[i] < MathHelper.PiOver4 || Wavelets[1].StartAngle[i] > 7f * MathHelper.PiOver4) ||
                    (Wavelets[1].StartAngle[i] >= 3f * MathHelper.PiOver4 && Wavelets[1].StartAngle[i] < 5f * MathHelper.PiOver4))
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }

                if ((Wavelets[1].Enemies.Length - i) / 4f == (Wavelets[1].Enemies.Length - i) / 4)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 0);
            Wavelets[2].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[2].Enemies.Length - 20; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[2].StartAngle[i] = 0;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[2].Enemies.Length - 20; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 20f;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                if (Wavelets[2].StartAngle[i] < 3f * MathHelper.PiOver4 || Wavelets[2].StartAngle[i] > 7f * MathHelper.PiOver4)
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                }

                if ((Wavelets[2].Enemies.Length - i) / 4f == (Wavelets[2].Enemies.Length - i) / 4)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                }
            }
            #endregion

            Waves[GetIndex(11, 2)].Wavelets = Wavelets;
            #endregion

            #region Wave (11-3) "Thetis, daughter of prophecy?"
            #region Metadata
            Waves[GetIndex(11, 3)] = new GameWave();
            Waves[GetIndex(11, 3)].Background = 3;
            Waves[GetIndex(11, 3)].ThrobColor = new Color(119, 203, 191);
            Waves[GetIndex(11, 3)].ColorState = 2;
            Waves[GetIndex(11, 3)].MajorWaveNumber = 11;
            Waves[GetIndex(11, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(11, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(11, 3)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(11, 3)].ParallaxElementTop.Tint = new Color(83, 167, 0);
            Waves[GetIndex(11, 3)].ParallaxElementTop.Speed = 0.4f;
            Waves[GetIndex(11, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(11, 3)].ParallaxElementBottom.Intensity = 3;
            Waves[GetIndex(11, 3)].ParallaxElementBottom.Tint = new Color(83, 167, 0);
            Waves[GetIndex(11, 3)].ParallaxElementBottom.Speed = 0.3f;
            Waves[GetIndex(11, 3)].Name = "Thetis, daughter of prophecy?";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(50, 1);
            Wavelets[0].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[0].StartAngle[i] = 0;
                Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
            }
            Wavelets[0].Enemies[Wavelets[0].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_KingGloop;
            Wavelets[0].ColorPolarities[Wavelets[0].Enemies.Length - 1] = ColorPolarity.Negative;
            Wavelets[0].StartHitPoints[Wavelets[0].Enemies.Length - 1] = 10;
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(50, 1);
            Wavelets[1].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[1].StartAngle[i] = MathHelper.Pi;
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
            }
            Wavelets[1].Enemies[Wavelets[1].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_KingGloop;
            Wavelets[1].ColorPolarities[Wavelets[1].Enemies.Length - 1] = ColorPolarity.Positive;
            Wavelets[1].StartHitPoints[Wavelets[1].Enemies.Length - 1] = 10;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 1);
            Wavelets[2].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[2].StartAngle[i] = 0;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[2].Enemies.Length - 4; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_KingGloop;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 4f;
                Wavelets[2].StartHitPoints[i] = 10;
            }
            #endregion

            Waves[GetIndex(11, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 12 (Boss)
            #region Wave (12-1) "Peleus and Thetis begat Achilles"
            #region Metadata
            Waves[GetIndex(12, 1)] = new GameWave();
            Waves[GetIndex(12, 1)].Background = 3;
            Waves[GetIndex(12, 1)].ThrobColor = new Color(119, 203, 191);
            Waves[GetIndex(12, 1)].ColorState = 2;
            Waves[GetIndex(12, 1)].MajorWaveNumber = 12;
            Waves[GetIndex(12, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(12, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(12, 1)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(12, 1)].ParallaxElementTop.Tint = new Color(0, 167, 83);
            Waves[GetIndex(12, 1)].ParallaxElementTop.Speed = 0.5f;
            Waves[GetIndex(12, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(12, 1)].ParallaxElementBottom.Intensity = 3;
            Waves[GetIndex(12, 1)].ParallaxElementBottom.Tint = new Color(0, 167, 83);
            Waves[GetIndex(12, 1)].ParallaxElementBottom.Speed = 0.5f;
            Waves[GetIndex(12, 1)].Name = "Peleus and Thetis begat Achilles";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(50, 2);
            Wavelets[0].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[0].StartAngle[i] = 0;
                Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
            }
            for (int i = Wavelets[0].Enemies.Length - 4; i < Wavelets[0].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                    Wavelets[0].StartHitPoints[i] = 10;
                }
                else
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                    Wavelets[0].StartHitPoints[i] = 1;
                }
                Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / 4f;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(50, 2);
            Wavelets[1].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[1].StartAngle[i] = MathHelper.TwoPi;
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
            }
            Wavelets[1].Enemies[Wavelets[1].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_KingGloop;
            Wavelets[1].ColorPolarities[Wavelets[1].Enemies.Length - 1] = ColorPolarity.Positive;
            Wavelets[1].StartHitPoints[Wavelets[1].Enemies.Length - 1] = 20;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 2);
            Wavelets[2].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[2].StartAngle[i] = 0;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[2].Enemies.Length - 4; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                }
                else
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                }
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / 4f;
                Wavelets[2].StartHitPoints[i] = 1;
            }
            #endregion

            Waves[GetIndex(12, 1)].Wavelets = Wavelets;
            #endregion

            #region Wave (12-2) "Enter ProtoNora"
            #region Metadata
            Waves[GetIndex(12, 2)] = new GameWave();
            Waves[GetIndex(12, 2)].Background = 3;
            Waves[GetIndex(12, 2)].ThrobColor = new Color(119, 203, 191);
            Waves[GetIndex(12, 2)].ColorState = 2;
            Waves[GetIndex(12, 2)].MajorWaveNumber = 12;
            Waves[GetIndex(12, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(12, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(12, 2)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(12, 2)].ParallaxElementTop.Tint = new Color(0, 112, 55);
            Waves[GetIndex(12, 2)].ParallaxElementTop.Speed = 0.5f;
            Waves[GetIndex(12, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(12, 2)].ParallaxElementBottom.Intensity = 3;
            Waves[GetIndex(12, 2)].ParallaxElementBottom.Tint = new Color(0, 112, 55);
            Waves[GetIndex(12, 2)].ParallaxElementBottom.Speed = 0.5f;
            Waves[GetIndex(12, 2)].Name = "Enter ProtoNora";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(50, 0);
            Wavelets[0].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[0].StartAngle[i] = 0;
                Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
            }
            Wavelets[0].Enemies[Wavelets[0].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_ProtoNora;
            Wavelets[0].StartHitPoints[Wavelets[0].Enemies.Length - 1] = 4;
            Wavelets[0].ColorPolarities[Wavelets[0].Enemies.Length - 1] = ColorPolarity.Negative;
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(50, 2);
            Wavelets[1].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[1].StartAngle[i] = MathHelper.TwoPi;
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
            }
            Wavelets[1].Enemies[Wavelets[1].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_ProtoNora;
            Wavelets[1].ColorPolarities[Wavelets[1].Enemies.Length - 1] = ColorPolarity.Positive;
            Wavelets[1].StartHitPoints[Wavelets[1].Enemies.Length - 1] = 5;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 0);
            Wavelets[2].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[2].StartAngle[i] = 0;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[2].Enemies.Length - 3; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_ProtoNora;
                Wavelets[2].StartAngle[i] = i * 3f * MathHelper.PiOver4 / 3f;
                Wavelets[2].StartHitPoints[i] = 5;
            }
            #endregion

            Waves[GetIndex(12, 2)].Wavelets = Wavelets;
            #endregion

            #region Wave (12-3) "ProtoNora Infection"
            #region Metadata
            Waves[GetIndex(12, 3)] = new GameWave();
            Waves[GetIndex(12, 3)].Background = 3;
            Waves[GetIndex(12, 3)].ThrobColor = new Color(119, 203, 191);
            Waves[GetIndex(12, 3)].ColorState = 2;
            Waves[GetIndex(12, 3)].MajorWaveNumber = 12;
            Waves[GetIndex(12, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(12, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(12, 3)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(12, 3)].ParallaxElementTop.Tint = new Color(0, 34, 112);
            Waves[GetIndex(12, 3)].ParallaxElementTop.Speed = 0.5f;
            Waves[GetIndex(12, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(12, 3)].ParallaxElementBottom.Intensity = 3;
            Waves[GetIndex(12, 3)].ParallaxElementBottom.Tint = new Color(0, 34, 122);
            Waves[GetIndex(12, 3)].ParallaxElementBottom.Speed = 0.5f;
            Waves[GetIndex(12, 3)].Name = "ProtoNora Infection";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(50, 0);
            Wavelets[0].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[0].StartAngle[i] = 0;
                Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
            }
            for (int i = Wavelets[0].Enemies.Length - 3; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_ProtoNora;
                Wavelets[0].StartAngle[i] = i * 3f * MathHelper.PiOver4 / 3f + MathHelper.Pi;
                Wavelets[0].StartHitPoints[i] = 5;
                Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(50, 2);
            Wavelets[1].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[1].StartAngle[i] = MathHelper.TwoPi;
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[1].Enemies.Length - 3; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_ProtoNora;
                Wavelets[1].StartAngle[i] = i * 3f * MathHelper.PiOver4 / 3f + MathHelper.PiOver4 * 5f;
                Wavelets[1].StartHitPoints[i] = 5;
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 2);
            Wavelets[2].SongID = SongID.Ultrafix; // FIXME DIFFERENT MUSIC?
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_AnnMoeba;
                Wavelets[2].StartAngle[i] = 0;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
            }
            for (int i = Wavelets[2].Enemies.Length - 5; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_ProtoNora;
                Wavelets[2].StartAngle[i] = i * 3f * MathHelper.PiOver4 / 3f;
                Wavelets[2].StartHitPoints[i] = 5;
            }
            #endregion

            Waves[GetIndex(12, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 13 - Flambi used
            #region WaveDef (13-1) "Rampant volcanism"
            #region Metadata
            Waves[GetIndex(13, 1)] = new GameWave();
            Waves[GetIndex(13, 1)].Background = 0;
            Waves[GetIndex(13, 1)].ThrobColor = new Color(255, 0, 162);
            Waves[GetIndex(13, 1)].ColorState = 1;
            Waves[GetIndex(13, 1)].MajorWaveNumber = 13;
            Waves[GetIndex(13, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(13, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(13, 1)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(13, 1)].ParallaxElementTop.Tint = new Color(255, 240, 0);
            Waves[GetIndex(13, 1)].ParallaxElementTop.Speed = -0.4f;
            Waves[GetIndex(13, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(13, 1)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(13, 1)].ParallaxElementBottom.Tint = new Color(255, 240, 0);
            Waves[GetIndex(13, 1)].ParallaxElementBottom.Speed = 0.3f;
            Waves[GetIndex(13, 1)].Name = "Rampant volcanism";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(40, 0);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
            }

            for (int i = 1; i <= 4; i++)
            {
                Wavelets[0].Enemies[Wavelets[0].Enemies.Length - i] = TypesOfPlayObjects.Enemy_Firefly;
                Wavelets[0].ColorPolarities[Wavelets[0].Enemies.Length - i] = ColorPolarity.Negative;
                Wavelets[0].StartAngle[Wavelets[0].Enemies.Length - i] = i * MathHelper.TwoPi / 4f;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(40, 0);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].Enemies.Length;
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
            }

            for (int i = 1; i <= 4; i++)
            {
                if (i == 4)
                {
                    Wavelets[1].Enemies[Wavelets[1].Enemies.Length - i] = TypesOfPlayObjects.Enemy_Flambi;
                }
                else
                {
                    Wavelets[1].Enemies[Wavelets[1].Enemies.Length - i] = TypesOfPlayObjects.Enemy_Firefly;
                }
                Wavelets[1].ColorPolarities[Wavelets[1].Enemies.Length - i] = ColorPolarity.Negative;
                Wavelets[1].StartAngle[Wavelets[1].Enemies.Length - i] = i * MathHelper.TwoPi / 4f;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(40, 0);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
            }

            for (int i = 1; i <= 4; i++)
            {
                if (i == 4)
                {
                    Wavelets[2].Enemies[Wavelets[2].Enemies.Length - i] = TypesOfPlayObjects.Enemy_Flambi;
                }
                else
                {
                    Wavelets[2].Enemies[Wavelets[2].Enemies.Length - i] = TypesOfPlayObjects.Enemy_Firefly;
                }
                Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - i] = ColorPolarity.Negative;
                Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - i] = i * MathHelper.TwoPi / 4f;
            }
            #endregion

            Waves[GetIndex(13, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (13-2) "Campus Martius"
            #region Metadata
            Waves[GetIndex(13, 2)] = new GameWave();
            Waves[GetIndex(13, 2)].Background = 0;
            Waves[GetIndex(13, 2)].ThrobColor = new Color(255, 0, 162);
            Waves[GetIndex(13, 2)].ColorState = 1;
            Waves[GetIndex(13, 2)].MajorWaveNumber = 13;
            Waves[GetIndex(13, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(13, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(13, 2)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(13, 2)].ParallaxElementTop.Tint = new Color(255, 138, 0);
            Waves[GetIndex(13, 2)].ParallaxElementTop.Speed = -0.5f;
            Waves[GetIndex(13, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(13, 2)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(13, 2)].ParallaxElementBottom.Tint = new Color(255, 138, 0);
            Waves[GetIndex(13, 2)].ParallaxElementBottom.Speed = 0.4f;
            Waves[GetIndex(13, 2)].Name = "Campus Martius";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(30, 0, ColorPolarity.Negative);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                Wavelets[0].StartAngle[i] = i * MathHelper.Pi / (float)Wavelets[0].Enemies.Length;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(30, 0, ColorPolarity.Positive);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                Wavelets[1].StartAngle[i] = i * MathHelper.Pi / (float)Wavelets[1].Enemies.Length + MathHelper.Pi;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(60, 2);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
            }
            #endregion

            Waves[GetIndex(13, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (13-3) "Cult of Helios"
            #region Metadata
            Waves[GetIndex(13, 3)] = new GameWave();
            Waves[GetIndex(13, 3)].Background = 0;
            Waves[GetIndex(13, 3)].ThrobColor = new Color(255, 0, 162);
            Waves[GetIndex(13, 3)].ColorState = 1;
            Waves[GetIndex(13, 3)].MajorWaveNumber = 13;
            Waves[GetIndex(13, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(13, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(13, 3)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(13, 3)].ParallaxElementTop.Tint = new Color(255, 138, 0);
            Waves[GetIndex(13, 3)].ParallaxElementTop.Speed = -0.6f;
            Waves[GetIndex(13, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(13, 3)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(13, 3)].ParallaxElementBottom.Tint = new Color(255, 138, 0);
            Waves[GetIndex(13, 3)].ParallaxElementBottom.Speed = 0.5f;
            Waves[GetIndex(13, 3)].Name = "Cult of Helios";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(60, 2);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                }
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(60, 2);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;

                if (i >= 0 && i <= Wavelets[1].Enemies.Length / 2f)
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                else if (i >= Wavelets[1].Enemies.Length - 10 && i <= Wavelets[1].Enemies.Length - 5)
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                else if (i > Wavelets[1].Enemies.Length - 5)
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                else
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                Wavelets[1].StartAngle[i] = i * 3f * MathHelper.PiOver4 / (float)Wavelets[1].Enemies.Length + MathHelper.Pi;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(60, 2);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                }
                else
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                    if (i >= Wavelets[2].Enemies.Length - 4)
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                    else if (i >= Wavelets[2].Enemies.Length - 10)
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                    else if (i >= Wavelets[2].Enemies.Length - 15)
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                    else
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                }
            }
            #endregion

            Waves[GetIndex(13, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 14 (Boss) - Flambi used
            #region WaveDef (14-1) "Screamin' Meanies"
            #region Metadata
            Waves[GetIndex(14, 1)] = new GameWave();
            Waves[GetIndex(14, 1)].Background = 0;
            Waves[GetIndex(14, 1)].ThrobColor = new Color(255, 0, 162);
            Waves[GetIndex(14, 1)].ColorState = 1;
            Waves[GetIndex(14, 1)].MajorWaveNumber = 14;
            Waves[GetIndex(14, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(14, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(14, 1)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(14, 1)].ParallaxElementTop.Tint = new Color(255, 66, 0);
            Waves[GetIndex(14, 1)].ParallaxElementTop.Speed = -0.6f;
            Waves[GetIndex(14, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(14, 1)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(14, 1)].ParallaxElementBottom.Tint = new Color(255, 138, 0);
            Waves[GetIndex(14, 1)].ParallaxElementBottom.Speed = 0.5f;
            Waves[GetIndex(14, 1)].Name = "Screamin' Meanies";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(10, 0);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if(MWMathHelper.IsEven(i))
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Flycket;
                else
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                Wavelets[0].ColorPolarities[i] = ColorState.RandomPolarity();
                Wavelets[0].StartAngle[i] = i * MathHelper.PiOver2 / (float)Wavelets[0].Enemies.Length + MathHelper.PiOver2;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(15, 0);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                if(MWMathHelper.IsEven(i)) 
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Flycket;
                    Wavelets[1].ColorPolarities[i] = ColorState.RandomPolarity();
                } else 
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }
                Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 / (float)Wavelets[1].Enemies.Length + MathHelper.PiOver2 * 3f;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(20, 0);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    if (i > 12)
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Flycket;
                    else
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Firefly;
                    Wavelets[2].ColorPolarities[i] = ColorState.RandomPolarity();
                }
                else
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                }
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
            }
            #endregion

            Waves[GetIndex(14, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (14-2) "Phaeton, rediscovered"
            #region Metadata
            Waves[GetIndex(14, 2)] = new GameWave();
            Waves[GetIndex(14, 2)].Background = 0;
            Waves[GetIndex(14, 2)].ThrobColor = new Color(255, 0, 162);
            Waves[GetIndex(14, 2)].ColorState = 1;
            Waves[GetIndex(14, 2)].MajorWaveNumber = 14;
            Waves[GetIndex(14, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(14, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(14, 2)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(14, 2)].ParallaxElementTop.Tint = new Color(255, 66, 0);
            Waves[GetIndex(14, 2)].ParallaxElementTop.Speed = -0.6f;
            Waves[GetIndex(14, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(14, 2)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(14, 2)].ParallaxElementBottom.Tint = new Color(255, 138, 0);
            Waves[GetIndex(14, 2)].ParallaxElementBottom.Speed = 0.5f;
            Waves[GetIndex(14, 2)].Name = "Phaeton, rediscovered";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(30, 0);
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[0].StartHitPoints[i] = 2;
                }
                else
                {
                    if (i / 3f == i / 3)
                    {
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                    }
                    else if (i / 4f == i / 4)
                    {
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Firefly;
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    }
                    else if (i >= Wavelets[0].Enemies.Length - 4)
                    {
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                        Wavelets[0].StartHitPoints[i] = 2;
                    }
                    else
                    {
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                    }
                }
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(40, 0);
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;

                if (i <= 15)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Firefly;
                }
                else if (i == 16)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Flambi;
                }
                else if (i > 16 && i < 25)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                }
                else if (i >= 25 && i < 33)
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                }
                else
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(40, 2);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;

                if (i / 3f == i / 3)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Firefly;
                }
                else if (i / 5f == i / 5)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Flycket;
                }
                else if (i > Wavelets[2].Enemies.Length - 3)
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Flambi;
                }
                else
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                }
            }
            #endregion

            Waves[GetIndex(14, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (14-3) "Pyre"
            #region Metadata
            Waves[GetIndex(14, 3)] = new GameWave();
            Waves[GetIndex(14, 3)].Background = 0;
            Waves[GetIndex(14, 3)].ThrobColor = new Color(255, 0, 162);
            Waves[GetIndex(14, 3)].ColorState = 1;
            Waves[GetIndex(14, 3)].MajorWaveNumber = 14;
            Waves[GetIndex(14, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(14, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(14, 3)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(14, 3)].ParallaxElementTop.Tint = new Color(255, 66, 0);
            Waves[GetIndex(14, 3)].ParallaxElementTop.Speed = -0.7f;
            Waves[GetIndex(14, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(14, 3)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(14, 3)].ParallaxElementBottom.Tint = new Color(255, 0, 36);
            Waves[GetIndex(14, 3)].ParallaxElementBottom.Speed = 0.6f;
            Waves[GetIndex(14, 3)].Name = "Pyre";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(20, 0, ColorPolarity.Negative); // From the right
            Wavelets[0].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.Pi / (float)Wavelets[0].Enemies.Length + 3f * MathHelper.PiOver2;
                if (MWMathHelper.IsEven(i))
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Firefly;
                else
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Flycket;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(20, 0, ColorPolarity.Positive);  // From the left
            Wavelets[1].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.Pi / (float)Wavelets[1].Enemies.Length + MathHelper.PiOver2;
                if (MWMathHelper.IsEven(i))
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Firefly;
                else
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Flycket;

                if (i == 10)
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Flambi;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 2);
            Wavelets[2].SongID = SongID.Dance8ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length / 2; i++) // From the right
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                Wavelets[2].StartAngle[i] = i * MathHelper.Pi / 25f + 3f * MathHelper.PiOver2;
            }
            for (int i = Wavelets[2].Enemies.Length / 2; i < Wavelets[2].Enemies.Length; i++) // From the left
            {
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].StartAngle[i] = i * MathHelper.Pi / 25f + MathHelper.PiOver2;
            }
            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 2] = TypesOfPlayObjects.Enemy_Pyre;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 2] = ColorPolarity.Negative;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 2] = 0;
            Wavelets[2].StartHitPoints[Wavelets[2].Enemies.Length - 2] = 5;

            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_Pyre;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 1] = ColorPolarity.Positive;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 1] = MathHelper.Pi;
            Wavelets[2].StartHitPoints[Wavelets[2].Enemies.Length - 1] = 5;
            #endregion

            Waves[GetIndex(14, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 15
            #region WaveDef (15-1) "Anunnaki alchemist"
            #region Metadata
            Waves[GetIndex(15, 1)] = new GameWave();
            Waves[GetIndex(15, 1)].Background = 1;
            Waves[GetIndex(15, 1)].ThrobColor = new Color(255, 0, 222);
            Waves[GetIndex(15, 1)].ColorState = 2;
            Waves[GetIndex(15, 1)].MajorWaveNumber = 15;
            Waves[GetIndex(15, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(15, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(15, 1)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(15, 1)].ParallaxElementTop.Tint = new Color(96, 46, 64);
            Waves[GetIndex(15, 1)].ParallaxElementTop.Speed = -0.3f;
            Waves[GetIndex(15, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(15, 1)].ParallaxElementBottom.Intensity = 3;
            Waves[GetIndex(15, 1)].ParallaxElementBottom.Tint = new Color(96, 181, 64);
            Waves[GetIndex(15, 1)].ParallaxElementBottom.Speed = 0.7f;
            Waves[GetIndex(15, 1)].Name = "Anunnaki alchemist";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(30, 3);
            Wavelets[0].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                }
                else
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                }

                if (i < Wavelets[0].Enemies.Length / 2f)
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(30, 3);
            Wavelets[1].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                }
                else
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                }

                if (i < Wavelets[1].Enemies.Length / 2f)
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(30, 3);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
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
            }
            #endregion

            Waves[GetIndex(15, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (15-2) "Ninhursag's silk spinners"
            #region Metadata
            Waves[GetIndex(15, 2)] = new GameWave();
            Waves[GetIndex(15, 2)].Background = 1;
            Waves[GetIndex(15, 2)].ThrobColor = new Color(255, 0, 222);
            Waves[GetIndex(15, 2)].ColorState = 2;
            Waves[GetIndex(15, 2)].MajorWaveNumber = 15;
            Waves[GetIndex(15, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(15, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(15, 2)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(15, 2)].ParallaxElementTop.Tint = new Color(96, 181, 64);
            Waves[GetIndex(15, 2)].ParallaxElementTop.Speed = -0.3f;
            Waves[GetIndex(15, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(15, 2)].ParallaxElementBottom.Intensity = 3;
            Waves[GetIndex(15, 2)].ParallaxElementBottom.Tint = new Color(96, 46, 64);
            Waves[GetIndex(15, 2)].ParallaxElementBottom.Speed = 0.7f;
            Waves[GetIndex(15, 2)].Name = "Ninhursag's silk spinners";
            #endregion

            Wavelets = new Wavelet[3];
            // Wavelets
            #region
            for (int i = 0; i < Wavelets.Length; i++)
            {
                Wavelets[i] = new Wavelet(20 + i * 20, 3);
                Wavelets[i].SongID = SongID.Ultrafix;
                for (int j = 0; j < Wavelets[i].Enemies.Length; j++)
                {
                    Wavelets[i].Enemies[j] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    Wavelets[i].StartAngle[j] = j * MathHelper.TwoPi / (float)Wavelets[i].Enemies.Length;
                    if (MWMathHelper.IsEven(j))
                    {
                        if (MWMathHelper.IsEven(i))
                            Wavelets[i].ColorPolarities[j] = ColorPolarity.Positive;
                        else
                            Wavelets[i].ColorPolarities[j] = ColorPolarity.Negative;
                    }
                    else
                    {
                        if (!MWMathHelper.IsEven(i))
                            Wavelets[i].ColorPolarities[j] = ColorPolarity.Positive;
                        else
                            Wavelets[i].ColorPolarities[j] = ColorPolarity.Negative;
                    }
                }
            }            
            #endregion

            Waves[GetIndex(15, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (15-3) "Ninmah, Nintu and Nin-ki"
            #region Metadata
            Waves[GetIndex(15, 3)] = new GameWave();
            Waves[GetIndex(15, 3)].Background = 1;
            Waves[GetIndex(15, 3)].ThrobColor = new Color(255, 0, 222);
            Waves[GetIndex(15, 3)].ColorState = 1;
            Waves[GetIndex(15, 3)].MajorWaveNumber = 15;
            Waves[GetIndex(15, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(15, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(15, 3)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(15, 3)].ParallaxElementTop.Tint = new Color(96, 46, 64);
            Waves[GetIndex(15, 3)].ParallaxElementTop.Speed = -0.6f;
            Waves[GetIndex(15, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(15, 3)].ParallaxElementBottom.Intensity = 3;
            Waves[GetIndex(15, 3)].ParallaxElementBottom.Tint = new Color(96, 181, 64);
            Waves[GetIndex(15, 3)].ParallaxElementBottom.Speed = 1.4f;
            Waves[GetIndex(15, 3)].Name = "Ninmah, Nintu and Nin-ki";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(40, 3);
            Wavelets[0].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
                }
                else
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_MiniSaw;
                    Wavelets[0].StartAngle[i] = MathHelper.Pi;
                }
                Wavelets[0].ColorPolarities[i] = ColorState.RandomPolarity();
            }
            Wavelets[0].Enemies[Wavelets[0].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_MetalTooth;
            Wavelets[0].StartAngle[Wavelets[0].Enemies.Length - 1] = MathHelper.Pi;
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(40, 3);
            Wavelets[1].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].Enemies.Length;
                }
                else
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_MiniSaw;
                    if (i > Wavelets[1].Enemies.Length / 2f)
                        Wavelets[1].StartAngle[i] = MathHelper.Pi;
                    else
                        Wavelets[1].StartAngle[i] = 0;
                }
                Wavelets[1].ColorPolarities[i] = ColorState.RandomPolarity();
            }
            Wavelets[1].Enemies[Wavelets[1].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_MetalTooth;
            Wavelets[1].StartAngle[Wavelets[1].Enemies.Length - 1] = MathHelper.Pi;
            Wavelets[1].Enemies[Wavelets[1].Enemies.Length - 2] = TypesOfPlayObjects.Enemy_MetalTooth;
            Wavelets[1].StartAngle[Wavelets[1].Enemies.Length - 2] = 0;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 3);
            Wavelets[2].SongID = SongID.Ultrafix;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (i > Wavelets[2].Enemies.Length - 5)
                {
                    Wavelets[2].StartAngle[i] = (Wavelets[2].Enemies.Length - i) * MathHelper.TwoPi/4f;
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_MetalTooth;
                    if (MWMathHelper.IsEven(i))
                        Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    else
                        Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
                    if (MWMathHelper.IsEven(i))
                    {
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                    }
                    else
                    {
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_MiniSaw;
                    }
                    Wavelets[2].ColorPolarities[i] = ColorState.RandomPolarity();
                }
            }
            #endregion

            Waves[GetIndex(15, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 16
            #region Wavedef (16-1) "Abzu remnants"
            #region Metadata
            Waves[GetIndex(16, 1)] = new GameWave();
            Waves[GetIndex(16, 1)].Background = 3;
            Waves[GetIndex(16, 1)].ThrobColor = new Color(255, 212, 255);
            Waves[GetIndex(16, 1)].ColorState = 0;
            Waves[GetIndex(16, 1)].MajorWaveNumber = 16;
            Waves[GetIndex(16, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(16, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(16, 1)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(16, 1)].ParallaxElementTop.Tint = new Color(255, 181, 126);
            Waves[GetIndex(16, 1)].ParallaxElementTop.Speed = 0.2f;
            Waves[GetIndex(16, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(16, 1)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(16, 1)].ParallaxElementBottom.Tint = new Color(255, 181, 126);
            Waves[GetIndex(16, 1)].ParallaxElementBottom.Speed = 0.2f;
            Waves[GetIndex(16, 1)].Name = "Abzu remnants";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(30, 2);
            Wavelets[0].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (i > Wavelets[0].Enemies.Length - 4)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                    Wavelets[0].StartAngle[i] = 0;
                    Wavelets[0].ColorPolarities[i] = ColorState.RandomPolarity();
                }
                else
                {
                    Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
                    if (MWMathHelper.IsEven(i))
                    {
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    }
                    else
                    {
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Firefly;
                        Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                    }
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(40, 2);
            Wavelets[1].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].Enemies.Length;
                if (i > Wavelets[1].Enemies.Length / 2f)
                {
                    if (MWMathHelper.IsEven(i))
                    {
                        Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                    }
                    else
                    {
                        Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                    }
                }
                else
                {
                    if (MWMathHelper.IsEven(i))
                    {
                        Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                    }
                    else
                    {
                        Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Firefly;
                    }
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 3, ColorPolarity.Positive);
            Wavelets[2].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (i > Wavelets[2].Enemies.Length - 3)
                {
                    Wavelets[2].StartAngle[i] = (Wavelets[2].Enemies.Length - i) * MathHelper.Pi;
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spawner;
                }
                else
                {
                    Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)(Wavelets[2].Enemies.Length - 3);
                    if (MWMathHelper.IsEven(i))
                    {
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                    }
                    else
                    {
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                    }
                }
            }
            #endregion

            Waves[GetIndex(16, 1)].Wavelets = Wavelets;
            #endregion

            #region Wavedef (16-2) "What Isimud beheld"
            #region Metadata
            Waves[GetIndex(16, 2)] = new GameWave();
            Waves[GetIndex(16, 2)].Background = 2;
            Waves[GetIndex(16, 2)].ThrobColor = new Color(255, 212, 255);
            Waves[GetIndex(16, 2)].ColorState = 0;
            Waves[GetIndex(16, 2)].MajorWaveNumber = 16;
            Waves[GetIndex(16, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(16, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(16, 2)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(16, 2)].ParallaxElementTop.Tint = new Color(255, 181, 126);
            Waves[GetIndex(16, 2)].ParallaxElementTop.Speed = 0.2f;
            Waves[GetIndex(16, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(16, 2)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(16, 2)].ParallaxElementBottom.Tint = new Color(255, 181, 126);
            Waves[GetIndex(16, 2)].ParallaxElementBottom.Speed = 0.2f;
            Waves[GetIndex(16, 2)].Name = "What Isimud beheld";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(30, 0);
            Wavelets[0].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
                Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                if(Wavelets[0].StartAngle[i] >= MathHelper.PiOver2 && Wavelets[0].StartAngle[i] <= MathHelper.PiOver2 *3f)
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(40, 0);
            Wavelets[1].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].Enemies.Length;
                Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                if (Wavelets[1].StartAngle[i] >= 0 && Wavelets[1].StartAngle[i] <= MathHelper.Pi)
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(35, 2);
            Wavelets[2].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
                if (i > Wavelets[2].Enemies.Length - 2)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Flambi;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;

                if (Wavelets[2].StartAngle[i] >= 0 && Wavelets[2].StartAngle[i] <= MathHelper.Pi)
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
            }
            #endregion

            Waves[GetIndex(16, 2)].Wavelets = Wavelets;
            #endregion

            #region Wavedef (16-3) "Apsu and Tiamat"
            #region Metadata
            Waves[GetIndex(16, 3)] = new GameWave();
            Waves[GetIndex(16, 3)].Background = 3;
            Waves[GetIndex(16, 3)].ThrobColor = new Color(136, 181, 216);
            Waves[GetIndex(16, 3)].ColorState = 2;
            Waves[GetIndex(16, 3)].MajorWaveNumber = 16;
            Waves[GetIndex(16, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(16, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(16, 3)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(16, 3)].ParallaxElementTop.Tint = new Color(136, 181, 126);
            Waves[GetIndex(16, 3)].ParallaxElementTop.Speed = 0.2f;
            Waves[GetIndex(16, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(16, 3)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(16, 3)].ParallaxElementBottom.Tint = new Color(136, 181, 126);
            Waves[GetIndex(16, 3)].ParallaxElementBottom.Speed = 0.2f;
            Waves[GetIndex(16, 3)].Name = "Apsu and Tiamat";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(30, 0);
            Wavelets[0].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.Pi / Wavelets[0].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(30, 0);
            Wavelets[1].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.Pi / Wavelets[1].Enemies.Length + MathHelper.Pi;
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }
                else
                {
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 2);
            Wavelets[2].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[2].StartAngle[i] = i * MathHelper.Pi / (float)Wavelets[2].Enemies.Length + 7f * MathHelper.PiOver4;
                }
                else
                {
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[2].StartAngle[i] = i * MathHelper.Pi / (float)Wavelets[2].Enemies.Length + 3f * MathHelper.PiOver4;
                }
            }
            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 2] = TypesOfPlayObjects.Enemy_KingGloop;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 2] = ColorPolarity.Positive;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 2] = MathHelper.Pi;

            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_Pyre;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 1] = ColorPolarity.Negative;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 1] = 0;
            #endregion

            Waves[GetIndex(16, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 17
            #region Wavedef (17-1) "Dainn alive"
            #region Metadata
            Waves[GetIndex(17, 1)] = new GameWave();
            Waves[GetIndex(17, 1)].Background = 4;
            Waves[GetIndex(17, 1)].ThrobColor = new Color(209, 216, 136);
            Waves[GetIndex(17, 1)].ColorState = 1;
            Waves[GetIndex(17, 1)].MajorWaveNumber = 17;
            Waves[GetIndex(17, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(17, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(17, 1)].ParallaxElementTop.Intensity = 4;
            Waves[GetIndex(17, 1)].ParallaxElementTop.Tint = new Color(111, 112, 96);
            Waves[GetIndex(17, 1)].ParallaxElementTop.Speed = 0.5f;
            Waves[GetIndex(17, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(17, 1)].ParallaxElementBottom.Intensity = 4;
            Waves[GetIndex(17, 1)].ParallaxElementBottom.Tint = new Color(111, 112, 193);
            Waves[GetIndex(17, 1)].ParallaxElementBottom.Speed = -0.5f;
            Waves[GetIndex(17, 1)].Name = "Dainn alive";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(40, 0);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;

                if (i / 10f == i / 10)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                }
                else
                {
                    if (MWMathHelper.IsEven(i))
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                    else
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(40, 2);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].Enemies.Length;
                if (Wavelets[1].StartAngle[i] > MathHelper.PiOver4 && Wavelets[1].StartAngle[i] < 3f * MathHelper.PiOver2)
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }
                if (i > Wavelets[1].Enemies.Length - 4)
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Flycket;
                else if (MWMathHelper.IsEven(i))
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                else
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
            }
            Wavelets[1].Enemies[Wavelets[1].Enemies.Length - 2] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[1].StartAngle[Wavelets[1].Enemies.Length - 2] = 0;
            Wavelets[1].Enemies[Wavelets[1].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[1].StartAngle[Wavelets[1].Enemies.Length - 1] = MathHelper.Pi;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(40, 3);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
                if (i / 4f == i / 4)
                {
                    if (i >= 10)
                    {
                        if (Wavelets[2].ColorPolarities[i - 10] == ColorPolarity.Negative)
                            Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                        else
                            Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    }
                    else
                    {
                        Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    }
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                }
                else
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    if (MWMathHelper.IsEven(i))
                    {
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                    }
                    else
                    {
                        Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                    }
                }
            }
            #endregion

            Waves[GetIndex(17, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (17-2) "Dvalinn calms Duneyrr and Durapror"
            #region Metadata
            Waves[GetIndex(17, 2)] = new GameWave();
            Waves[GetIndex(17, 2)].Background = 1;
            Waves[GetIndex(17, 2)].ThrobColor = new Color(209, 216, 136);
            Waves[GetIndex(17, 2)].ColorState = 1;
            Waves[GetIndex(17, 2)].MajorWaveNumber = 17;
            Waves[GetIndex(17, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(17, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(17, 2)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(17, 2)].ParallaxElementTop.Tint = new Color(111, 112, 193);
            Waves[GetIndex(17, 2)].ParallaxElementTop.Speed = 0.7f;
            Waves[GetIndex(17, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(17, 2)].ParallaxElementBottom.Intensity = 3;
            Waves[GetIndex(17, 2)].ParallaxElementBottom.Tint = new Color(111, 112, 96);
            Waves[GetIndex(17, 2)].ParallaxElementBottom.Speed = -0.7f;
            Waves[GetIndex(17, 2)].Name = "Dvalinn calms Duneyrr and Durapror";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(40, 3);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * 3f * MathHelper.PiOver2 / (float)Wavelets[0].Enemies.Length - MathHelper.PiOver4;
                if (MWMathHelper.IsEven(i))
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;

                if (i / 4f == i / 4)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;
                }
                else if (i /5f == i / 5f)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                }
                else
                {
                    if (MWMathHelper.IsEven(i))
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_MiniSaw;
                    else
                        Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(40, 3);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                if (i < Wavelets[1].Enemies.Length / 2f)
                {
                    Wavelets[1].StartAngle[i] = 0;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                    if (MWMathHelper.IsEven(i))
                    {
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                        Wavelets[1].StartHitPoints[i] = 0;
                    }
                    else
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_MiniSaw;
                }
                else
                {
                    Wavelets[1].StartAngle[i] = MathHelper.Pi;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                    if (MWMathHelper.IsEven(i))
                    {
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                        Wavelets[1].StartHitPoints[i] = 0;
                    }
                    else
                        Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                }
            }
            Wavelets[1].Enemies[Wavelets[1].Enemies.Length - 2] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[1].ColorPolarities[Wavelets[1].Enemies.Length - 2] = ColorPolarity.Negative;
            Wavelets[1].StartAngle[Wavelets[1].Enemies.Length - 2] = MathHelper.Pi;
            Wavelets[1].Enemies[Wavelets[1].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[1].ColorPolarities[Wavelets[1].Enemies.Length - 1] = ColorPolarity.Positive;
            Wavelets[1].StartAngle[Wavelets[1].Enemies.Length - 1] = 0;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(40, 3);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
            }
            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 2] = TypesOfPlayObjects.Enemy_MetalTooth;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 2] = ColorPolarity.Negative;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 2] = 3f * MathHelper.PiOver4;
            
            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_MetalTooth;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 1] = ColorPolarity.Positive;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 1] = 7f * MathHelper.PiOver4;
            
            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 3] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 3] = ColorPolarity.Negative;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 3] = MathHelper.PiOver4;
            Wavelets[2].StartHitPoints[Wavelets[2].Enemies.Length - 3] = 0;

            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 4] = TypesOfPlayObjects.Enemy_GloopPrince;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 4] = ColorPolarity.Positive;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 4] = 5f * MathHelper.PiOver4;
            Wavelets[2].StartHitPoints[Wavelets[2].Enemies.Length - 4] = 0;
            #endregion

            Waves[GetIndex(17, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (17-3) "Loving the Alien"
            #region Metadata
            Waves[GetIndex(17, 3)] = new GameWave();
            Waves[GetIndex(17, 3)].Background = 0;
            Waves[GetIndex(17, 3)].ThrobColor = new Color(209, 216, 136);
            Waves[GetIndex(17, 3)].ColorState = 1;
            Waves[GetIndex(17, 3)].MajorWaveNumber = 17;
            Waves[GetIndex(17, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(17, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(17, 3)].ParallaxElementTop.Intensity = 1;
            Waves[GetIndex(17, 3)].ParallaxElementTop.Tint = new Color(111, 112, 193);
            Waves[GetIndex(17, 3)].ParallaxElementTop.Speed = 1.2f;
            Waves[GetIndex(17, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(17, 3)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(17, 3)].ParallaxElementBottom.Tint = new Color(111, 112, 96);
            Waves[GetIndex(17, 3)].ParallaxElementBottom.Speed = -1.2f;
            Waves[GetIndex(17, 3)].Name = "Loving the Alien";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(40, 0);
            Wavelets[0].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].StartAngle.Length;
                if (i / 4f == i / 4)
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Flycket;
                }
                else
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(40, 0);
            Wavelets[1].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].StartAngle.Length + MathHelper.PiOver4;
                if (i / 4f == i / 4)
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Flycket;
                }
                else
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(50, 3);
            Wavelets[2].SongID = SongID.LandOfSand16ths;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].StartAngle[i] = 0;
                }
                else
                {
                    Wavelets[2].StartAngle[i] = MathHelper.Pi;
                }
            }
            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 2] = TypesOfPlayObjects.Enemy_UncleanRot;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 2] = ColorPolarity.Positive;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 2] = 0;

            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_UncleanRot;
            Wavelets[2].ColorPolarities[Wavelets[2].Enemies.Length - 1] = ColorPolarity.Negative;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 1] = MathHelper.Pi;
            #endregion

            Waves[GetIndex(17, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 18
            #region Wavedef (18-1) "Plane of Ginnungagap"
            #region Metadata
            Waves[GetIndex(18, 1)] = new GameWave();
            Waves[GetIndex(18, 1)].Background = 4;
            Waves[GetIndex(18, 1)].ThrobColor = new Color(255, 154, 95);
            Waves[GetIndex(18, 1)].ColorState = 2;
            Waves[GetIndex(18, 1)].MajorWaveNumber = 18;
            Waves[GetIndex(18, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(18, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(18, 1)].ParallaxElementTop.Intensity = 4;
            Waves[GetIndex(18, 1)].ParallaxElementTop.Tint = new Color(39, 173, 173);
            Waves[GetIndex(18, 1)].ParallaxElementTop.Speed = 2.1f;
            Waves[GetIndex(18, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(18, 1)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(18, 1)].ParallaxElementBottom.Tint = new Color(234, 149, 209);
            Waves[GetIndex(18, 1)].ParallaxElementBottom.Speed = -2.2f;
            Waves[GetIndex(18, 1)].Name = "Plane of Ginnungagap";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(20, 0);
            Wavelets[0].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
                if ((Wavelets[0].StartAngle[i] >= 0 && Wavelets[0].StartAngle[i] <= MathHelper.PiOver4) ||
                    (Wavelets[0].StartAngle[i] >= MathHelper.Pi && Wavelets[0].StartAngle[i] <= 3f * MathHelper.PiOver2))
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                }
                else
                {
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                }
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(20, 2);
            Wavelets[1].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].Enemies.Length;
                if ((Wavelets[1].StartAngle[i] >= 0 && Wavelets[1].StartAngle[i] <= MathHelper.PiOver4) ||
                    (Wavelets[1].StartAngle[i] >= MathHelper.Pi && Wavelets[1].StartAngle[i] <= 3f * MathHelper.PiOver2))
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                }
                else
                {
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(25, 2);
            Wavelets[2].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                if (i / 3f == i / 3)
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                else
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
            }
            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_MetalTooth;
            Wavelets[2].StartAngle[Wavelets[2].Enemies.Length - 1] = MathHelper.PiOver2;
            Wavelets[2].StartHitPoints[Wavelets[2].Enemies.Length - 1] = 4;
            #endregion

            Waves[GetIndex(18, 1)].Wavelets = Wavelets;
            #endregion

            #region Wavedef (18-2) "Omphalos"
            #region Metadata
            Waves[GetIndex(18, 2)] = new GameWave();
            Waves[GetIndex(18, 2)].Background = 4;
            Waves[GetIndex(18, 2)].ThrobColor = new Color(255, 154, 95);
            Waves[GetIndex(18, 2)].ColorState = 1;
            Waves[GetIndex(18, 2)].MajorWaveNumber = 18;
            Waves[GetIndex(18, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(18, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(18, 2)].ParallaxElementTop.Intensity = 3;
            Waves[GetIndex(18, 2)].ParallaxElementTop.Tint = new Color(39, 173, 173);
            Waves[GetIndex(18, 2)].ParallaxElementTop.Speed = 2.1f;
            Waves[GetIndex(18, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(18, 2)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(18, 2)].ParallaxElementBottom.Tint = new Color(234, 149, 209);
            Waves[GetIndex(18, 2)].ParallaxElementBottom.Speed = -2.2f;
            Waves[GetIndex(18, 2)].Name = "Omphalos";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(32, 2);
            Wavelets[0].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;

                if (i > (Wavelets[0].Enemies.Length - 2) / 2f)
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_MiniSaw;
                    Wavelets[0].StartAngle[i] = MathHelper.PiOver4;
                }
                else
                {
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                    Wavelets[0].StartAngle[i] = MathHelper.PiOver4 * 5f;
                }
            }
            Wavelets[0].Enemies[Wavelets[0].Enemies.Length - 2] = TypesOfPlayObjects.Enemy_MetalTooth;
            Wavelets[0].StartAngle[Wavelets[0].Enemies.Length - 2] = MathHelper.PiOver4;
            Wavelets[0].StartHitPoints[Wavelets[0].Enemies.Length - 2] = 3;

            Wavelets[0].Enemies[Wavelets[0].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_Pyre;
            Wavelets[0].StartAngle[Wavelets[0].Enemies.Length - 1] = MathHelper.PiOver4 * 5f;
            Wavelets[0].StartHitPoints[Wavelets[0].Enemies.Length - 1] = 3;

            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(50, 5);
            Wavelets[1].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                if (i < Wavelets[1].Enemies.Length - 4)
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                else
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_GloopPrince;

                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 / (float)Wavelets[1].Enemies.Length;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;
                }
                else
                {
                    Wavelets[1].StartAngle[i] = i * MathHelper.PiOver2 / (float)Wavelets[1].Enemies.Length + MathHelper.Pi;
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                }
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(30, 5);
            Wavelets[2].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[2].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Negative;
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Maggot;
                }
                else
                {
                    Wavelets[2].ColorPolarities[i] = ColorPolarity.Positive;
                    Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                }
            }
            #endregion

            Waves[GetIndex(18, 2)].Wavelets = Wavelets;
            #endregion

            #region Wavedef (18-3) "Lahmu, the gatekeeper" (muddy)
            #region Metadata
            Waves[GetIndex(18, 3)] = new GameWave();
            Waves[GetIndex(18, 3)].Background = 1;
            Waves[GetIndex(18, 3)].ThrobColor = new Color(255, 154, 95);
            Waves[GetIndex(18, 3)].ColorState = 0;
            Waves[GetIndex(18, 3)].MajorWaveNumber = 18;
            Waves[GetIndex(18, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(18, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(18, 3)].ParallaxElementTop.Intensity = 2;
            Waves[GetIndex(18, 3)].ParallaxElementTop.Tint = new Color(234, 149, 209);
            Waves[GetIndex(18, 3)].ParallaxElementTop.Speed = 2.1f;
            Waves[GetIndex(18, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(18, 3)].ParallaxElementBottom.Intensity = 1;
            Waves[GetIndex(18, 3)].ParallaxElementBottom.Tint = new Color(39, 173, 173);
            Waves[GetIndex(18, 3)].ParallaxElementBottom.Speed = -2.2f;
            Waves[GetIndex(18, 3)].Name = "Lahmu, the gatekeeper";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(12, 2);
            Wavelets[0].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[0].Enemies.Length; i++)
            {
                Wavelets[0].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[0].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Positive;
                else
                    Wavelets[0].ColorPolarities[i] = ColorPolarity.Negative;

                if (i < Wavelets[0].Enemies.Length / 2f)
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                else
                    Wavelets[0].Enemies[i] = TypesOfPlayObjects.Enemy_Firefly;
            }
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(12, 2);
            Wavelets[1].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[1].Enemies.Length; i++)
            {
                Wavelets[1].StartAngle[i] = i * MathHelper.TwoPi / (float)Wavelets[1].Enemies.Length;
                if (MWMathHelper.IsEven(i))
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Negative;
                else
                    Wavelets[1].ColorPolarities[i] = ColorPolarity.Positive;

                if (i < Wavelets[1].Enemies.Length / 2f)
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                else
                    Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
            }
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(10, 2, ColorPolarity.Negative);
            Wavelets[2].SongID = SongID.WinOne;
            for (int i = 0; i < Wavelets[2].Enemies.Length; i++)
            {
                Wavelets[2].StartAngle[i] = 0;
                Wavelets[2].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
            }
            Wavelets[2].Enemies[Wavelets[2].Enemies.Length - 1] = TypesOfPlayObjects.Enemy_Lahmu;
            #endregion

            Waves[GetIndex(18, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            /*
            #region Wave 19
            #region Wavedef (19-1) "Deny me and be doomed"
            #region Metadata
            Waves[GetIndex(19, 1)] = new GameWave();
            Waves[GetIndex(19, 1)].Background = 4;
            Waves[GetIndex(19, 1)].ThrobColor = new Color(255, 154, 95);
            Waves[GetIndex(19, 1)].ColorState = 2;
            Waves[GetIndex(19, 1)].MajorWaveNumber = 19;
            Waves[GetIndex(19, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(19, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(19, 1)].ParallaxElementTop.Intensity = 4;
            Waves[GetIndex(19, 1)].ParallaxElementTop.Tint = new Color(39, 173, 173);
            Waves[GetIndex(19, 1)].ParallaxElementTop.Speed = 2.1f;
            Waves[GetIndex(19, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(19, 1)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(19, 1)].ParallaxElementBottom.Tint = new Color(234, 149, 209);
            Waves[GetIndex(19, 1)].ParallaxElementBottom.Speed = -2.2f;
            Waves[GetIndex(19, 1)].Name = "Deny me and be doomed";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            #endregion
            // Second wavelet
            #region
            #endregion
            // Third wavelet
            #region
            #endregion

            Waves[GetIndex(19, 1)].Wavelets = Wavelets;
            #endregion

            #region Wavedef (19-2) "Tamsin Gnosis"
            #region Metadata
            Waves[GetIndex(19, 2)] = new GameWave();
            Waves[GetIndex(19, 2)].Background = 4;
            Waves[GetIndex(19, 2)].ThrobColor = new Color(255, 154, 95);
            Waves[GetIndex(19, 2)].ColorState = 2;
            Waves[GetIndex(19, 2)].MajorWaveNumber = 19;
            Waves[GetIndex(19, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(19, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(19, 2)].ParallaxElementTop.Intensity = 4;
            Waves[GetIndex(19, 2)].ParallaxElementTop.Tint = new Color(39, 173, 173);
            Waves[GetIndex(19, 2)].ParallaxElementTop.Speed = 2.1f;
            Waves[GetIndex(19, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(19, 2)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(19, 2)].ParallaxElementBottom.Tint = new Color(234, 149, 209);
            Waves[GetIndex(19, 2)].ParallaxElementBottom.Speed = -2.2f;
            Waves[GetIndex(19, 2)].Name = "Tamsin Gnosis";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            #endregion
            // Second wavelet
            #region
            #endregion
            // Third wavelet
            #region
            #endregion

            Waves[GetIndex(19, 2)].Wavelets = Wavelets;
            #endregion

            #region Wavedef (19-3) "Center of the Abzu" (water/swamp)
            #region Metadata
            Waves[GetIndex(19, 3)] = new GameWave();
            Waves[GetIndex(19, 3)].Background = 4;
            Waves[GetIndex(19, 3)].ThrobColor = new Color(255, 154, 95);
            Waves[GetIndex(19, 3)].ColorState = 2;
            Waves[GetIndex(19, 3)].MajorWaveNumber = 19;
            Waves[GetIndex(19, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(19, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(19, 3)].ParallaxElementTop.Intensity = 4;
            Waves[GetIndex(19, 3)].ParallaxElementTop.Tint = new Color(39, 173, 173);
            Waves[GetIndex(19, 3)].ParallaxElementTop.Speed = 2.1f;
            Waves[GetIndex(19, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(19, 3)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(19, 3)].ParallaxElementBottom.Tint = new Color(234, 149, 209);
            Waves[GetIndex(19, 3)].ParallaxElementBottom.Speed = -2.2f;
            Waves[GetIndex(19, 3)].Name = "Center of the Abzu";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            #endregion
            // Second wavelet
            #region
            #endregion
            // Third wavelet
            #region
            #endregion

            Waves[GetIndex(19, 3)].Wavelets = Wavelets;
            #endregion
            #endregion

            #region Wave 20 (END-GAME)
            #region Wavedef (20-1) "End times"
            #region Metadata
            Waves[GetIndex(20, 1)] = new GameWave();
            Waves[GetIndex(20, 1)].Background = 4;
            Waves[GetIndex(20, 1)].ThrobColor = new Color(255, 154, 95);
            Waves[GetIndex(20, 1)].ColorState = 2;
            Waves[GetIndex(20, 1)].MajorWaveNumber = 20;
            Waves[GetIndex(20, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(20, 1)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(20, 1)].ParallaxElementTop.Intensity = 4;
            Waves[GetIndex(20, 1)].ParallaxElementTop.Tint = new Color(39, 173, 173);
            Waves[GetIndex(20, 1)].ParallaxElementTop.Speed = 2.1f;
            Waves[GetIndex(20, 1)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(20, 1)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(20, 1)].ParallaxElementBottom.Tint = new Color(234, 149, 209);
            Waves[GetIndex(20, 1)].ParallaxElementBottom.Speed = -2.2f;
            Waves[GetIndex(20, 1)].Name = "End times";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            #endregion
            // Second wavelet
            #region
            #endregion
            // Third wavelet
            #region
            #endregion

            Waves[GetIndex(20, 1)].Wavelets = Wavelets;
            #endregion

            #region Wavedef (20-2) "Evening Star"
            #region Metadata
            Waves[GetIndex(20, 2)] = new GameWave();
            Waves[GetIndex(20, 2)].Background = 4;
            Waves[GetIndex(20, 2)].ThrobColor = new Color(255, 154, 95);
            Waves[GetIndex(20, 2)].ColorState = 2;
            Waves[GetIndex(20, 2)].MajorWaveNumber = 20;
            Waves[GetIndex(20, 2)].MinorWaveNumber = 2;
            Waves[GetIndex(20, 2)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(20, 2)].ParallaxElementTop.Intensity = 4;
            Waves[GetIndex(20, 2)].ParallaxElementTop.Tint = new Color(39, 173, 173);
            Waves[GetIndex(20, 2)].ParallaxElementTop.Speed = 2.1f;
            Waves[GetIndex(20, 2)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(20, 2)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(20, 2)].ParallaxElementBottom.Tint = new Color(234, 149, 209);
            Waves[GetIndex(20, 2)].ParallaxElementBottom.Speed = -2.2f;
            Waves[GetIndex(20, 2)].Name = "Evening Star";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            #endregion
            // Second wavelet
            #region
            #endregion
            // Third wavelet
            #region
            #endregion

            Waves[GetIndex(20, 2)].Wavelets = Wavelets;
            #endregion

            #region Wavedef (20-3) "Moloch whose name is the Mind!"
            #region Metadata
            Waves[GetIndex(20, 3)] = new GameWave();
            Waves[GetIndex(20, 3)].Background = 4;
            Waves[GetIndex(20, 3)].ThrobColor = new Color(255, 154, 95);
            Waves[GetIndex(20, 3)].ColorState = 2;
            Waves[GetIndex(20, 3)].MajorWaveNumber = 20;
            Waves[GetIndex(20, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(20, 3)].ParallaxElementTop = new ParallaxElement();
            Waves[GetIndex(20, 3)].ParallaxElementTop.Intensity = 4;
            Waves[GetIndex(20, 3)].ParallaxElementTop.Tint = new Color(39, 173, 173);
            Waves[GetIndex(20, 3)].ParallaxElementTop.Speed = 2.1f;
            Waves[GetIndex(20, 3)].ParallaxElementBottom = new ParallaxElement();
            Waves[GetIndex(20, 3)].ParallaxElementBottom.Intensity = 2;
            Waves[GetIndex(20, 3)].ParallaxElementBottom.Tint = new Color(234, 149, 209);
            Waves[GetIndex(20, 3)].ParallaxElementBottom.Speed = -2.2f;
            Waves[GetIndex(20, 3)].Name = "Moloch whose name is the Mind!";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            #endregion
            // Second wavelet
            #region
            #endregion
            // Third wavelet
            #region
            #endregion

            Waves[GetIndex(20, 3)].Wavelets = Wavelets;
            #endregion
            #endregion
             * */
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
