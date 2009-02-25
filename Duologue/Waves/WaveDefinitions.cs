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
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.Audio;
#endregion

namespace Duologue.Waves
{
    /// <summary>
    /// For now, we'll just be storing these in memory. At a later date, we likely will
    /// want these to be stored on disk
    /// </summary>
    public class WaveDefinitions
    {
        #region Constants
        private int numberOfWaves = 20;
        #endregion

        #region Fields
        /// <summary>
        /// The waves used in this game
        /// </summary>
        private GameWave[] Waves;
        #endregion

        #region Properties
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
            Waves[GetIndex(1, 1)] = new GameWave();
            Waves[GetIndex(1, 1)].Background = 0;
            Waves[GetIndex(1, 1)].ColorState = 0;
            Waves[GetIndex(1, 1)].MajorWaveNumber = 1;
            Waves[GetIndex(1, 1)].MinorWaveNumber = 1;
            Waves[GetIndex(1, 1)].Name = "The LZ is hot!";

            Wavelets = new Wavelet[4];
            // First wavelet
            Wavelets[0] = new Wavelet(2, 0);
            Wavelets[0].SongID = SongID.Intensity;
            Wavelets[0].Enemies[0] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[0] = 0f;
            Wavelets[0].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[1] = MathHelper.Pi * 1.5f;
            // Second wavelet
            Wavelets[1] = new Wavelet(2, 0);
            Wavelets[1].SongID = SongID.Intensity;
            Wavelets[1].Enemies[0] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[0] = MathHelper.Pi;
            Wavelets[1].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[1].StartAngle[1] = MathHelper.PiOver2;
            // Third wavelet
            Wavelets[2] = new Wavelet(4, 0);
            Wavelets[2].SongID = SongID.Intensity;
            Wavelets[2].Enemies[0] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[0] = 0f;
            Wavelets[2].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[1] = MathHelper.Pi * 1.5f;
            Wavelets[2].Enemies[2] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[2] = MathHelper.Pi;
            Wavelets[2].Enemies[3] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[3] = MathHelper.PiOver2;
            // Fourth wavelet
            Wavelets[2] = new Wavelet(8, 0);
            Wavelets[2].SongID = SongID.Intensity;
            Wavelets[2].Enemies[0] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[0] = 0f;
            Wavelets[2].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[1] = MathHelper.Pi * 1.5f;
            Wavelets[2].Enemies[2] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[2] = MathHelper.Pi;
            Wavelets[2].Enemies[3] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[3] = MathHelper.PiOver2;
            Wavelets[2].Enemies[4] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[4] = MathHelper.PiOver4;
            Wavelets[2].Enemies[5] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[5] = MathHelper.PiOver2 + MathHelper.PiOver4;
            Wavelets[2].Enemies[6] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[6] = MathHelper.Pi + MathHelper.PiOver4;
            Wavelets[2].Enemies[7] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[2].StartAngle[7] = MathHelper.Pi * 1.5f + MathHelper.PiOver4;

            Waves[GetIndex(1, 1)].Wavelets = Wavelets;
            #endregion

            #region WaveDef (1-2) "The scouting party"
            Waves[GetIndex(1, 2)] = new GameWave();
            Waves[GetIndex(1, 2)].Background = 0;
            Waves[GetIndex(1, 2)].ColorState = 0;
            Waves[GetIndex(1, 2)].MajorWaveNumber = 1;
            Waves[GetIndex(1, 2)].MinorWaveNumber = 1;
            Waves[GetIndex(1, 2)].Name = "The LZ is hot!";

            Wavelets = new Wavelet[4];
            // First wavelet
            Wavelets[0] = new Wavelet(2, 0);
            Wavelets[0].SongID = SongID.Intensity;
            Wavelets[0].Enemies[0] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[0] = 0f;
            Wavelets[0].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            Wavelets[0].StartAngle[1] = MathHelper.Pi * 1.5f;
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
            return Waves[GetIndex(MajorNum, MinorNum)];
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
                index = numberOfWaves - 1;
            return index;
        }
        #endregion
    }
}
