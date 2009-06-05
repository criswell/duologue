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
using Duologue.AchievementSystem;
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
        public const int MaxMinorNumber = 3;

        /// <summary>
        /// The maxium number for major wave numbers
        /// </summary>
        public const int MaxMajorNumber = 999;

        private const float medal_WetFeet = 0.25f;
        private const float medal_Experienced = 0.5f;
        #endregion

        #region Fields
        private WaveDefinitions waveDef;

        // DELME - this is just here for testing the kill-everyone achievement
        private int currentEnemyIndex = 0;
        private TypesOfPlayObjects[] enemiesToSpawn = new TypesOfPlayObjects[]
        {
            TypesOfPlayObjects.Enemy_Moloch,
            TypesOfPlayObjects.Enemy_AnnMoeba,
            TypesOfPlayObjects.Enemy_Buzzsaw,
            TypesOfPlayObjects.Enemy_Ember,
            TypesOfPlayObjects.Enemy_Firefly,
            TypesOfPlayObjects.Enemy_Flambi,
            TypesOfPlayObjects.Enemy_Flycket,
            TypesOfPlayObjects.Enemy_Gloop,
            TypesOfPlayObjects.Enemy_GloopPrince,
            TypesOfPlayObjects.Enemy_KingGloop,
            TypesOfPlayObjects.Enemy_Lahmu,
            TypesOfPlayObjects.Enemy_MetalTooth,
            TypesOfPlayObjects.Enemy_MiniSaw,
            TypesOfPlayObjects.Enemy_Mirthworm,
            TypesOfPlayObjects.Enemy_ProtoNora,
            TypesOfPlayObjects.Enemy_Pyre,
            TypesOfPlayObjects.Enemy_Roggles,
            TypesOfPlayObjects.Enemy_Spawner,
            TypesOfPlayObjects.Enemy_Spitter,
            TypesOfPlayObjects.Enemy_StaticGloop,
            TypesOfPlayObjects.Enemy_UncleanRot,
            TypesOfPlayObjects.Enemy_Wiggles,
        };
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
            CurrentMajorNumber = 1;
            CurrentMinorNumber = 0;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get the next wave
        /// </summary>
        private GameWave GetWave(int lastMajorNumber, int lastMinorNumber)
        {
            InstanceManager.Logger.LogEntry(
                String.Format("Current progress: {0}/{1}={2}", lastMajorNumber, waveDef.TotalNumberOfMajorWaves,
                    (float)lastMajorNumber / (float)waveDef.TotalNumberOfMajorWaves));

            if ((float)lastMajorNumber / (float)waveDef.TotalNumberOfMajorWaves > medal_WetFeet)
            {
                LocalInstanceManager.AchievementManager.UnlockAchievement(PossibleMedals.WetFeet);
            }
            if ((float)lastMajorNumber / (float)waveDef.TotalNumberOfMajorWaves > medal_Experienced)
            {
                LocalInstanceManager.AchievementManager.UnlockAchievement(PossibleMedals.Experienced);
            }

            int[] k = IncrementWaveNumbers(lastMajorNumber, lastMinorNumber);
            GameWave temp;
            try
            {
                temp = waveDef.GetWave(k[0], k[1]);
                lastMajorNumber = temp.MajorWaveNumber; //k[0];
                lastMinorNumber = temp.MinorWaveNumber; // k[1];
                return temp;
            }
            catch
            {
                LocalInstanceManager.AchievementManager.UnlockAchievement(PossibleMedals.TourOfDuty);
                throw;
            }
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
                MWMathHelper.GetRandomInRange(0, ColorState.NumberOfColorStates),
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
            int NumEnemies = 30;
            thisWave.CurrentWavelet = 0;
            thisWave.Wavelets = new Wavelet[NumWavelets];
            int hitsToKillEnemy = 0;
            thisWave.Wavelets[thisWave.CurrentWavelet] =
                new Wavelet(NumEnemies, hitsToKillEnemy);

            //if (MWMathHelper.CoinToss())
                thisWave.Wavelets[thisWave.CurrentWavelet].SongID = SongID.Dance8ths;
            //else
                //thisWave.Wavelets[thisWave.CurrentWavelet].SongID = SongID.SuperbowlIntro;

            // Randomize the background and parallax elements
            thisWave.Background = MWMathHelper.GetRandomInRange(0, LocalInstanceManager.Background.NumBackgrounds + 1);
            thisWave.ThrobColor = new Color(
                (byte)MWMathHelper.GetRandomInRange(0, 255),
                (byte)MWMathHelper.GetRandomInRange(0, 255),
                (byte)MWMathHelper.GetRandomInRange(0, 255));
            
            thisWave.ParallaxElementTop.Intensity = MWMathHelper.GetRandomInRange(0, 6);
            thisWave.ParallaxElementTop.Speed = (float)MWMathHelper.GetRandomInRange(-5.0, 5.0);
            if (thisWave.ParallaxElementTop.Speed == 0)
                thisWave.ParallaxElementTop.Speed = 0.1f;
            thisWave.ParallaxElementTop.Tint = new Color(
                (byte)MWMathHelper.GetRandomInRange(0, 255),
                (byte)MWMathHelper.GetRandomInRange(0, 255),
                (byte)MWMathHelper.GetRandomInRange(0, 255),
                (byte)MWMathHelper.GetRandomInRange(50, 255));

            thisWave.ParallaxElementBottom.Intensity = MWMathHelper.GetRandomInRange(0, 5);
            if (MWMathHelper.CoinToss())
            {
                thisWave.ParallaxElementBottom.Speed = thisWave.ParallaxElementTop.Speed;
                thisWave.ParallaxElementBottom.Tint = thisWave.ParallaxElementTop.Tint;
            }
            else
            {
                thisWave.ParallaxElementBottom.Speed = (float)MWMathHelper.GetRandomInRange(-5.0, 5.0);
                if (thisWave.ParallaxElementBottom.Speed == 0)
                    thisWave.ParallaxElementBottom.Speed = 0.1f;
                thisWave.ParallaxElementBottom.Tint = new Color(
                    (byte)MWMathHelper.GetRandomInRange(0, 255),
                    (byte)MWMathHelper.GetRandomInRange(0, 255),
                    (byte)MWMathHelper.GetRandomInRange(0, 255),
                    (byte)MWMathHelper.GetRandomInRange(50, 255));

            }


            /*thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[0] = TypesOfPlayObjects.Enemy_Wiggles;
            thisWave.Wavelet[thisWave.CurrentWavelet].StartAngle[0] = MathHelper.PiOver2;

            thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            thisWave.Wavelet[thisWave.CurrentWavelet].StartAngle[1] = MathHelper.Pi;*/
            for (int i = 0; i < thisWave.NumEnemies; i++)
            {
                /*if ((float)i / 2f == i / 2)
                    thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                else
                    thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;*/
                //thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                //thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                //if(MWMathHelper.IsEven(i))
                    //thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                //else
                    thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Mirthworm;
                thisWave.Wavelets[thisWave.CurrentWavelet].StartAngle[i] = (float)InstanceManager.Random.NextDouble() * MathHelper.TwoPi;
                thisWave.Wavelets[thisWave.CurrentWavelet].ColorPolarities[i] = ColorState.RandomPolarity();
            }
            //thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[80] = TypesOfPlayObjects.Enemy_KingGloop;
            /*thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[59] = TypesOfPlayObjects.Enemy_Pyre;
            thisWave.Wavelets[thisWave.CurrentWavelet].StartAngle[59] = MathHelper.TwoPi;*/

            /*thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[thisWave.NumEnemies - 1] = enemiesToSpawn[currentEnemyIndex];
            thisWave.Wavelets[thisWave.CurrentWavelet].StartAngle[thisWave.NumEnemies - 1] = MathHelper.Pi;
            thisWave.Wavelets[thisWave.CurrentWavelet].StartHitPoints[thisWave.NumEnemies - 1] = 0;

            currentEnemyIndex++;
            if (currentEnemyIndex >= enemiesToSpawn.Length)
                currentEnemyIndex = 0;*/

            //thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[thisWave.NumEnemies - 2] = TypesOfPlayObjects.Enemy_Spawner;
            //thisWave.Wavelets[thisWave.CurrentWavelet].StartAngle[thisWave.NumEnemies - 2] = 0;
            //thisWave.Wavelets[thisWave.CurrentWavelet].StartHitPoints[thisWave.NumEnemies - 2] = 2;

            /*
            thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[39] = TypesOfPlayObjects.Enemy_ProtoNora;
            thisWave.Wavelets[thisWave.CurrentWavelet].StartHitPoints[39] = 2;
            thisWave.Wavelets[thisWave.CurrentWavelet].ColorPolarities[39] = ColorPolarity.Positive;
            thisWave.Wavelets[thisWave.CurrentWavelet].StartAngle[39] = MathHelper.Pi;

            thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[38] = TypesOfPlayObjects.Enemy_ProtoNora;
            thisWave.Wavelets[thisWave.CurrentWavelet].StartHitPoints[38] = 2;
            thisWave.Wavelets[thisWave.CurrentWavelet].ColorPolarities[38] = ColorPolarity.Negative;
            thisWave.Wavelets[thisWave.CurrentWavelet].StartAngle[38] = MathHelper.TwoPi;
            */

            return thisWave;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reset the game wave back to 1-1
        /// </summary>
        public void Reset()
        {
            Reset(1, 0);
        }

        /// <summary>
        /// Reset the game back to a major and minor number specified
        /// </summary>
        public void Reset(int Major, int Minor)
        {
            CurrentMajorNumber = Major;
            CurrentMinorNumber = Minor;
        }

        /// <summary>
        /// Get the next wave
        /// </summary>
        /// <returns></returns>
        public GameWave GetNextWave()
        {
            GameWave temp;
            if (LocalInstanceManager.CurrentGameState == GameState.InfinityGame)
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
