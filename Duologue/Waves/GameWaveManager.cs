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
using Duologue.Audio;
using Duologue.Properties;
using Duologue.Screens;
using Duologue.UI;
using Duologue.PlayObjects;
using Duologue.Waves;
using Duologue.State;
#endregion

namespace Duologue.Waves
{
    /// <summary>
    /// The manager of the game waves
    /// </summary>
    public class GameWaveManager
    {
        #region Constants
        /// <summary>
        /// The maximum number of gamewaves
        /// </summary>
        public const int MaxNumOfGameWaves = 100;

        /// <summary>
        /// The maximum number for minor wave numbers
        /// </summary>
        public const int MaxMinorNumber = 10;

        /// <summary>
        /// The maxium number for major wave numbers
        /// </summary>
        public const int MaxMajorNumber = 999;
        #endregion

        #region Fields
        private WaveDefinitions waveDef;
        #endregion

        #region Properties
        /// <summary>
        /// The current wave's Major Number
        /// </summary>
        public int CurrentMajorNumber;

        /// <summary>
        /// The current wave's Minor number
        /// </summary>
        public int CurrentMinorNumber;

        #endregion

        #region Constructor / Init
        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="maxNumOfGameWaves"></param>
        public GameWaveManager()//int? maxNumOfGameWaves)
        {
            waveDef = new WaveDefinitions();

            // Sensible defaults
            CurrentMajorNumber = 0;
            CurrentMinorNumber = 0;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get the next wave
        /// </summary>
        private GameWave GetWave(int lastMajorNumber, int lastMinorNumber)
        {
            int[] k = IncrementWaveNumbers(lastMajorNumber, lastMinorNumber);
            lastMajorNumber = k[0];
            lastMinorNumber = k[1];
            return waveDef.GetWave(lastMinorNumber, lastMinorNumber);
        }

        private int[] IncrementWaveNumbers(int lastMajorNo, int lastMinorNo)
        {
            int[] k = new int[2];
            lastMinorNo++;
            if (lastMinorNo > MaxMinorNumber)
            {
                lastMinorNo = 0;
                lastMajorNo++;
                if (lastMajorNo > MaxMajorNumber)
                {
                    lastMajorNo = 0;
                    // FIXME: Need an achievement here, I think
                }
            }
            k[0] = lastMajorNo;
            k[1] = lastMinorNo;

            return k;
        }

        /// <summary>
        /// Generate a random wave
        /// </summary>
        /// <returns>A random game wave</returns>
        private GameWave GenerateRandomWave(int lastMajorWaveNo, int lastMinorWaveNo)
        {
            int[] k = IncrementWaveNumbers(lastMajorWaveNo, lastMinorWaveNo);
            lastMajorWaveNo = k[0];
            lastMinorWaveNo = k[1];

            // Get a random color state
            ColorState[] theStates = ColorState.GetColorStates();

            GameWave thisWave = new GameWave(Resources.GameScreen_InfiniteWave,
                InstanceManager.Random.Next(LocalInstanceManager.Background.NumBackgrounds),
                MWMathHelper.GetRandomInRange(0, ColorState.numberOfColorStates),
                lastMajorWaveNo,
                lastMinorWaveNo);

            // Okay, this is gonna get fugly... sorry folks
            // FIXME: If you feel adventurous enough
            // .
            // Okay, we'll divide up every ten levels, beyond 100 they just repeat
            /*if (lastMajorWaveNo < 10)
            {

            }*/

            // ERE I AM JH
            int NumWavelets = 1;
            int NumEnemies = 22;
            thisWave.CurrentWavelet = 0;
            thisWave.Wavelets = new Wavelet[NumWavelets];
            int hitsToKillEnemy = 0;
            thisWave.Wavelets[thisWave.CurrentWavelet] =
                new Wavelet(NumEnemies, hitsToKillEnemy);

            if (MWMathHelper.CoinToss())
                thisWave.Wavelets[thisWave.CurrentWavelet].SongID = SongID.Intensity;
            else
                thisWave.Wavelets[thisWave.CurrentWavelet].SongID = SongID.LandOfSand;

            /*thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[0] = TypesOfPlayObjects.Enemy_Wiggles;
            thisWave.Wavelet[thisWave.CurrentWavelet].StartAngle[0] = MathHelper.PiOver2;

            thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            thisWave.Wavelet[thisWave.CurrentWavelet].StartAngle[1] = MathHelper.Pi;*/
            for (int i = 0; i < thisWave.NumEnemies; i++)
            {
                /*if ((float)i / 2f == i / 2)
                    thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                else
                    thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Buzzsaw;*/
                thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Spitter;
                //thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                //thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_StaticGloop;
                thisWave.Wavelets[thisWave.CurrentWavelet].StartAngle[i] = (float)InstanceManager.Random.NextDouble() * MathHelper.TwoPi;
                thisWave.Wavelets[thisWave.CurrentWavelet].ColorPolarities[i] = ColorState.RandomPolarity();
            }
            //thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[80] = TypesOfPlayObjects.Enemy_KingGloop;
            /*thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[20] = TypesOfPlayObjects.Enemy_StaticKing;
            thisWave.Wavelets[thisWave.CurrentWavelet].StartAngle[20] = MathHelper.TwoPi;

            thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[21] = TypesOfPlayObjects.Enemy_KingGloop;
            //thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[81] = TypesOfPlayObjects.Enemy_StaticKing;
            thisWave.Wavelets[thisWave.CurrentWavelet].StartAngle[21] = MathHelper.Pi;*/

            return thisWave;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the next wave
        /// </summary>
        /// <returns></returns>
        public GameWave GetNextWave()
        {
            GameWave temp;
            if (LocalInstanceManager.CurrentGameState == Duologue.State.GameState.InfinityGame)
            {
                temp = GenerateRandomWave(CurrentMajorNumber, CurrentMinorNumber);
                CurrentMajorNumber = temp.MajorWaveNumber;
                CurrentMinorNumber = temp.MinorWaveNumber;
            }
            else
            {
                temp = GetWave(CurrentMajorNumber, CurrentMinorNumber);
                CurrentMajorNumber = temp.MajorWaveNumber;
                CurrentMinorNumber = temp.MinorWaveNumber;
            }
            return temp;
        }
        #endregion
    }
}
