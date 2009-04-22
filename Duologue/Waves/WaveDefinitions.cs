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
        private int numberOfWaves = 3;
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

            #region WaveDef (1-1) "The LZ is hot!"
            #region Metadata
            Waves[GetIndex(1, 1)] = new GameWave();
            Waves[GetIndex(1, 1)].Background = 0;
            Waves[GetIndex(1, 1)].ThrobColor = Color.Tomato;
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
            Waves[GetIndex(1, 2)].Background = 4;
            Waves[GetIndex(1, 2)].ThrobColor = Color.SaddleBrown;
            Waves[GetIndex(1, 2)].ColorState = 1;
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
            Wavelets[1] = new Wavelet(8, 2);
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
            Wavelets[2] = new Wavelet(16, 3);
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

                Wavelets[2].StartAngle[i] = (float)i * MathHelper.PiOver4 + MathHelper.PiOver4 / 4f; ;
            }
            #endregion

            Waves[GetIndex(1, 2)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (1-3) "Bringing in the big guns"
            #region Metadata
            Waves[GetIndex(1, 3)] = new GameWave();
            Waves[GetIndex(1, 3)].Background = 2;
            Waves[GetIndex(1, 3)].ThrobColor = Color.SeaGreen;
            Waves[GetIndex(1, 3)].ColorState = 2;
            Waves[GetIndex(1, 3)].MajorWaveNumber = 1;
            Waves[GetIndex(1, 3)].MinorWaveNumber = 3;
            Waves[GetIndex(1, 3)].Name = "Bringing in the big guns";
            #endregion

            Wavelets = new Wavelet[3];
            // First wavelet
            #region
            Wavelets[0] = new Wavelet(2, 0);
            Wavelets[0].SongID = SongID.LandOfSand16ths;

            Wavelets[0].Enemies[0] = TypesOfPlayObjects.Enemy_Spitter;
            Wavelets[0].StartAngle[0] = MathHelper.PiOver4;
            Wavelets[0].ColorPolarities[0] = ColorPolarity.Positive;

            Wavelets[0].Enemies[1] = TypesOfPlayObjects.Enemy_Spitter;
            Wavelets[0].StartAngle[1] = MathHelper.Pi + MathHelper.PiOver4;
            Wavelets[0].ColorPolarities[1] = ColorPolarity.Negative;
            #endregion
            // Second wavelet
            #region
            Wavelets[1] = new Wavelet(6, 0);
            Wavelets[1].SongID = SongID.LandOfSand16ths;

            Wavelets[1].Enemies[0] = TypesOfPlayObjects.Enemy_Spitter;
            Wavelets[1].StartAngle[0] = MathHelper.PiOver4 + MathHelper.PiOver2;
            Wavelets[1].ColorPolarities[0] = ColorPolarity.Positive;

            Wavelets[1].Enemies[1] = TypesOfPlayObjects.Enemy_Spitter;
            Wavelets[1].StartAngle[1] = MathHelper.TwoPi - MathHelper.PiOver4;
            Wavelets[1].ColorPolarities[1] = ColorPolarity.Negative;

            Wavelets[1].Enemies[2] = TypesOfPlayObjects.Enemy_Wiggles;
            Wavelets[1].StartAngle[2] = MathHelper.PiOver4;
            Wavelets[1].ColorPolarities[2] = ColorPolarity.Positive;

            Wavelets[1].Enemies[3] = TypesOfPlayObjects.Enemy_Wiggles;
            Wavelets[1].StartAngle[3] = MathHelper.Pi + MathHelper.PiOver4;
            Wavelets[1].ColorPolarities[3] = ColorPolarity.Negative;

            Wavelets[1].Enemies[4] = TypesOfPlayObjects.Enemy_Spitter;
            Wavelets[1].StartAngle[4] = MathHelper.Pi;
            Wavelets[1].ColorPolarities[4] = ColorPolarity.Negative;

            Wavelets[1].Enemies[5] = TypesOfPlayObjects.Enemy_Spitter;
            Wavelets[1].StartAngle[5] = MathHelper.TwoPi;
            Wavelets[1].ColorPolarities[5] = ColorPolarity.Positive;
            #endregion
            // Third wavelet
            #region
            Wavelets[2] = new Wavelet(10, 2);
            Wavelets[2].SongID = SongID.LandOfSand16ths;

            Wavelets[2].Enemies[0] = TypesOfPlayObjects.Enemy_Spitter;
            Wavelets[2].StartAngle[0] = MathHelper.PiOver2;
            Wavelets[2].ColorPolarities[0] = ColorPolarity.Negative;

            Wavelets[2].Enemies[1] = TypesOfPlayObjects.Enemy_Spitter;
            Wavelets[2].StartAngle[1] = MathHelper.Pi - MathHelper.PiOver2;
            Wavelets[2].ColorPolarities[1] = ColorPolarity.Positive;

            Wavelets[2].Enemies[2] = TypesOfPlayObjects.Enemy_Wiggles;
            Wavelets[2].StartAngle[2] = MathHelper.Pi;
            Wavelets[2].ColorPolarities[2] = ColorPolarity.Positive;

            Wavelets[2].Enemies[3] = TypesOfPlayObjects.Enemy_Wiggles;
            Wavelets[2].StartAngle[3] = 0;
            Wavelets[2].ColorPolarities[3] = ColorPolarity.Negative;

            Wavelets[2].Enemies[4] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[4] = MathHelper.PiOver4;
            Wavelets[2].ColorPolarities[4] = ColorPolarity.Positive;

            Wavelets[2].Enemies[5] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[5] = MathHelper.Pi + MathHelper.PiOver4;
            Wavelets[2].ColorPolarities[5] = ColorPolarity.Negative;

            Wavelets[2].Enemies[6] = TypesOfPlayObjects.Enemy_Spitter;
            Wavelets[2].StartAngle[6] = 0;
            Wavelets[2].ColorPolarities[6] = ColorPolarity.Negative;

            Wavelets[2].Enemies[7] = TypesOfPlayObjects.Enemy_Spitter;
            Wavelets[2].StartAngle[7] = MathHelper.Pi - MathHelper.PiOver4;
            Wavelets[2].ColorPolarities[7] = ColorPolarity.Positive;

            Wavelets[2].Enemies[8] = TypesOfPlayObjects.Enemy_Spitter;
            Wavelets[2].StartAngle[8] = 0;
            Wavelets[2].ColorPolarities[8] = ColorPolarity.Negative;

            Wavelets[2].Enemies[9] = TypesOfPlayObjects.Enemy_Spitter;
            Wavelets[2].StartAngle[9] = MathHelper.Pi - MathHelper.PiOver4;
            Wavelets[2].ColorPolarities[9] = ColorPolarity.Positive;
            #endregion

            Waves[GetIndex(1, 3)].Wavelets = Wavelets;
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
            if (MinorNum > 4)
                MinorNum = 4;
            else if (MinorNum < 1)
                MinorNum = 1;
            int index = (MajorNum * 4) - 5 + MinorNum;
            if (index >= numberOfWaves)
                throw new WavesOutOfRangeException();
            return index;
        }
        #endregion
    }
}
