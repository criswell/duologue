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
        private const float medal_KeyParty = 0.5f;

        private const int default_StartingMajorNum = 1;
        private const int default_StartingMinorNum = 0;

        private const int numberOfWavesPerColorStateChange = 3;
        private const int min_NumberOfWavesToSwitchItems = 1;
        private const int max_NumberOfWavesToSwitchItems = 8;

        private const float min_NumberOfEnemies = 4f;
        private const float max_NumberOfEnemies = 80f;

        private const int min_EnemyRandomJitter = 0;
        private const int max_EnemyRandomJitter = 5;

        private const float min_HPBaseline = 0f;
        private const float max_HPBaseline = 5f;

        private const float max_TotalHP = 10f;

        private const float min_EnemyDelay = 0.5f;
        // How's this for a mind-fuck?
        private const float maxMin_EnemyDelay = 25.1f;
        private const float maxMax_EnemyDelay = 40.1f;

        #region Boss/Minion HP max and mins for random waves
        // Gloops
        private const float HP_GloopPrinceMin = 2f;
        private const float HP_GloopPrinceMax = 10f;
        private const float HP_GloopKingMin = 2f;
        private const float HP_GloopKingMax = 15f;
        private const float HP_GloopMin = 0f;
        private const float HP_GloopMax = 6f;
        // Fire stuffs
        private const float HP_PyreMin = 7f;
        private const float HP_PyreMax = 15f;
        private const float HP_EmberMin = 1f;
        private const float HP_EmberMax = 15f;
        // ProtoNora stuffs
        private const float HP_ProtoNoraMin = 2f;
        private const float HP_ProtoNoraMax = 4f;
        private const float HP_AnnMoebaMin = 3f;
        private const float HP_AnnMoebaMax = 5f;
        // MetalTooth stuffs
        private const float HP_MetalToothMin = 2f;
        private const float HP_MetalToothMax = 4f;
        private const float HP_MiniSawMin = 0f;
        private const float HP_MiniSawMax = 1f;
        // Unclean Rot stuff
        private const float HP_UncleanRotMin = 4f;
        private const float HP_UncleanRotMax = 10f;
        private const float HP_StaticGloopMin = 4f;
        private const float HP_StaticGloopMax = 9f;
        // Percentage limits
        private const float bossHP_LowerLimit = 0.3f;
        private const float bossHP_UpperLimit = 1f;
        private const float intensity_LowerLimit = 0.5f;
        private const float intensity_UpperLimit = 1.1f;
        // Lahmu data
        private const float minNumberLahmu = 1;
        private const float maxNumberLahmu = 3;
        private const float HP_LahmuMin = 1;
        private const float HP_LahmuMax = 3;
        #endregion
        #endregion

        #region Fields
        private WaveDefinitions waveDef;
        private int nextMajorNumToSwitchBackgroundOn;
        private int countdownToSwitchBackgroundElements;
        private int countdownToSwitchSongs;
        private int currentBackground;
        private int currentColorState;
        private int wavesSinceColorStateChange;
        private ParallaxElement currentParallaxElementTop;
        private ParallaxElement currentParallaxElementBottom;
        private Color throbColor;

        private SongID currentSong;
        private SongID[] possibleSongs = new SongID[]
        {
            SongID.Dance8ths,
            SongID.LandOfSand16ths,
            SongID.SecondChance,
            SongID.Ultrafix,
            SongID.WinOne,
        };

        private WaveTemplates waveTemplates;

        private TypesOfPlayObjects[][] enemyClusters;
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
            nextMajorNumToSwitchBackgroundOn = 0;
            wavesSinceColorStateChange = numberOfWavesPerColorStateChange;
            countdownToSwitchBackgroundElements = 0;
            countdownToSwitchSongs = 0;

            waveTemplates = new WaveTemplates(this);

            // Sensible defaults
            CurrentMajorNumber = default_StartingMajorNum;
            CurrentMinorNumber = default_StartingMinorNum;

            // Set up the possible enemy clusters
            enemyClusters = new TypesOfPlayObjects[][]
            {
                new TypesOfPlayObjects[]
                // Tier 1
                {
                    TypesOfPlayObjects.Enemy_Buzzsaw,
                    TypesOfPlayObjects.Enemy_Maggot,
                    TypesOfPlayObjects.Enemy_Mirthworm,
                    TypesOfPlayObjects.Enemy_Wiggles,
                },
                new TypesOfPlayObjects[]
                // Tier 2
                {
                    TypesOfPlayObjects.Enemy_Buzzsaw,
                    TypesOfPlayObjects.Enemy_Maggot,
                    TypesOfPlayObjects.Enemy_Mirthworm,
                    TypesOfPlayObjects.Enemy_Wiggles,
                    TypesOfPlayObjects.Enemy_AnnMoeba,
                    TypesOfPlayObjects.Enemy_Ember,
                    TypesOfPlayObjects.Enemy_Spitter,
                },
                new TypesOfPlayObjects[]
                // Tier 3
                {
                    TypesOfPlayObjects.Enemy_Buzzsaw,
                    TypesOfPlayObjects.Enemy_Maggot,
                    TypesOfPlayObjects.Enemy_Mirthworm,
                    TypesOfPlayObjects.Enemy_Wiggles,
                    TypesOfPlayObjects.Enemy_AnnMoeba,
                    TypesOfPlayObjects.Enemy_Ember,
                    TypesOfPlayObjects.Enemy_Spitter,
                    TypesOfPlayObjects.Enemy_Gloop,
                    TypesOfPlayObjects.Enemy_StaticGloop,
                    TypesOfPlayObjects.Enemy_Spawner,
                },
                new TypesOfPlayObjects[]
                // Tier 4
                {
                    TypesOfPlayObjects.Enemy_Buzzsaw,
                    TypesOfPlayObjects.Enemy_Maggot,
                    TypesOfPlayObjects.Enemy_Mirthworm,
                    TypesOfPlayObjects.Enemy_Wiggles,
                    TypesOfPlayObjects.Enemy_AnnMoeba,
                    TypesOfPlayObjects.Enemy_Ember,
                    TypesOfPlayObjects.Enemy_Spitter,
                    TypesOfPlayObjects.Enemy_Gloop,
                    TypesOfPlayObjects.Enemy_StaticGloop,
                    TypesOfPlayObjects.Enemy_Spawner,
                    TypesOfPlayObjects.Enemy_Firefly,
                    TypesOfPlayObjects.Enemy_Flycket,
                },
            };
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

            // Figure out number of players
            int players = 0;
            for (int i = 0; i < LocalInstanceManager.MaxNumberOfPlayers; i++)
            {
                if (LocalInstanceManager.Players[i].Active)
                    players++;
            }

            if ((float)lastMajorNumber / (float)waveDef.TotalNumberOfMajorWaves > medal_WetFeet)
            {
                LocalInstanceManager.AchievementManager.UnlockAchievement(PossibleMedals.WetFeet);
            }
            if ((float)lastMajorNumber / (float)waveDef.TotalNumberOfMajorWaves > medal_Experienced)
            {
                LocalInstanceManager.AchievementManager.UnlockAchievement(PossibleMedals.Experienced);
            }
            if ((float)lastMajorNumber / (float)waveDef.TotalNumberOfMajorWaves > medal_KeyParty &&
                players > 3)
            {
                LocalInstanceManager.AchievementManager.UnlockAchievement(PossibleMedals.KeyParty);
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
                if (players > 1)
                    LocalInstanceManager.AchievementManager.UnlockAchievement(PossibleMedals.BFF);
                throw;
            }
        }

        private int[] IncrementWaveNumbers(int lastMajorNo, int lastMinorNo)
        {
            int[] k = new int[2];
            lastMinorNo++;
            if (lastMinorNo > MaxMinorNumber)
            {
                lastMinorNo = 1;
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

            // Get a color state
            wavesSinceColorStateChange++;
            if (wavesSinceColorStateChange > numberOfWavesPerColorStateChange)
                currentColorState =
                    MWMathHelper.GetRandomInRange(0, ColorState.NumberOfColorStates);

            // Handle background changes
            if (lastMajorWaveNo >= nextMajorNumToSwitchBackgroundOn)
            {
                currentBackground = InstanceManager.Random.Next(LocalInstanceManager.Background.NumBackgrounds + 1);
                nextMajorNumToSwitchBackgroundOn = lastMajorWaveNo +
                    MWMathHelper.GetRandomInRange(
                        min_NumberOfWavesToSwitchItems,
                        max_NumberOfWavesToSwitchItems);

                if (nextMajorNumToSwitchBackgroundOn > MaxMajorNumber)
                    nextMajorNumToSwitchBackgroundOn = 0;
            }

            GameWave thisWave = new GameWave(Resources.GameScreen_SurvivalWave,
                currentBackground,
                currentColorState,
                lastMajorWaveNo,
                lastMinorWaveNo);

            countdownToSwitchBackgroundElements--;
            if (countdownToSwitchBackgroundElements < 0)
            {
                countdownToSwitchBackgroundElements = MWMathHelper.GetRandomInRange(
                    min_NumberOfWavesToSwitchItems,
                    max_NumberOfWavesToSwitchItems);

                // Randomize the background and parallax elements
                throbColor = new Color(
                    (byte)MWMathHelper.GetRandomInRange(0, 255),
                    (byte)MWMathHelper.GetRandomInRange(0, 255),
                    (byte)MWMathHelper.GetRandomInRange(0, 255));

                currentParallaxElementTop.Intensity = MWMathHelper.GetRandomInRange(0, 6);
                currentParallaxElementTop.Speed = (float)MWMathHelper.GetRandomInRange(-5.0, 5.0);
                if (currentParallaxElementTop.Speed == 0)
                    currentParallaxElementTop.Speed = 0.1f;
                currentParallaxElementTop.Tint = new Color(
                    (byte)MWMathHelper.GetRandomInRange(0, 255),
                    (byte)MWMathHelper.GetRandomInRange(0, 255),
                    (byte)MWMathHelper.GetRandomInRange(0, 255),
                    (byte)MWMathHelper.GetRandomInRange(50, 255));

                currentParallaxElementBottom.Intensity = MWMathHelper.GetRandomInRange(0, 5);
                if (MWMathHelper.CoinToss())
                {
                    currentParallaxElementBottom.Speed = currentParallaxElementTop.Speed;
                    currentParallaxElementBottom.Tint = currentParallaxElementTop.Tint;
                }
                else
                {
                    currentParallaxElementBottom.Speed = (float)MWMathHelper.GetRandomInRange(-5.0, 5.0);
                    if (currentParallaxElementBottom.Speed == 0)
                        currentParallaxElementBottom.Speed = 0.1f;
                    currentParallaxElementBottom.Tint = new Color(
                        (byte)MWMathHelper.GetRandomInRange(0, 255),
                        (byte)MWMathHelper.GetRandomInRange(0, 255),
                        (byte)MWMathHelper.GetRandomInRange(0, 255),
                        (byte)MWMathHelper.GetRandomInRange(50, 255));
                }
            }

            thisWave.ThrobColor = throbColor;
            thisWave.ParallaxElementBottom = currentParallaxElementBottom;
            thisWave.ParallaxElementTop = currentParallaxElementTop;

            // Possible beat engine songs
            // Dance8ths, LandOfSand16ths, 
            // Ultrafix, WinOne, SecondChance
            // SecondChance - Small number of levels, good for boss battles?
            // WinOne - Good number of levels, synth guitar, breath, good for long usage
            // Ultrafix - Decent number of levels, hammer "ping", highly repetative
            // Dance8ths - Small number of levels, guitar rock track, highly repetitive
            // LandOfSand16ths - Kind of 80s sound synth piano, good number of levels
            countdownToSwitchSongs--;
            if (countdownToSwitchSongs < 0)
            {
                currentSong = possibleSongs[MWMathHelper.GetRandomInRange(
                    0, possibleSongs.Length)];

                countdownToSwitchSongs = MWMathHelper.GetRandomInRange(
                    min_NumberOfWavesToSwitchItems,
                    max_NumberOfWavesToSwitchItems);
            }

            // Begin constructing the wavelet
            thisWave.CurrentWavelet = 0;
            thisWave.Wavelets = new Wavelet[1];

            // Get the relative wave num
            int relativeWaveNum = lastMajorWaveNo;
            for(int i = 900; i > 100; i = i -100)
            {
                if (relativeWaveNum > i)
                {
                    relativeWaveNum = relativeWaveNum - i;
                    break;
                }
            }

            // Figure out if we're creating a boss level or not
            if (relativeWaveNum / 10 == relativeWaveNum / 10f && lastMinorWaveNo == MaxMinorNumber)
            {
                // Fighting boss
                int numOfBosses = 1;
                if (relativeWaveNum >= 50 && relativeWaveNum < 80)
                {
                    numOfBosses = MWMathHelper.GetRandomInRange(2, 4);
                }

                // HP boss (determined by relwaveno, real wave no)
                // realwaveno determines max of percentage
                // relwaveno determines percentage up to max
                float percentBossHP;

                float maxPerc = MathHelper.Lerp(
                    bossHP_LowerLimit,
                    bossHP_UpperLimit,
                    (float)lastMajorWaveNo/(float)MaxMajorNumber);

                percentBossHP = MathHelper.Lerp(0, maxPerc,
                    (float)relativeWaveNum / 100f);

                // HP Minion (determined by relwavno)
                float percentMinionHP = (float)relativeWaveNum / 100f;

                // intensity (determined by relwavno, real wave no)
                float intensity;

                maxPerc = MathHelper.Lerp(
                    intensity_LowerLimit,
                    intensity_UpperLimit,
                    (float)lastMajorWaveNo / (float)MaxMajorNumber);

                intensity = MathHelper.Lerp(0, maxPerc,
                    (float)relativeWaveNum / 100f);

                if (relativeWaveNum <= 80)
                {
                    switch (MWMathHelper.GetRandomInRange(0, 6))
                    {
                        case 0:
                            // Gloop Prince
                            thisWave.Wavelets[0] = waveTemplates.GenerateBoss(
                                numOfBosses,
                                (int)MathHelper.Lerp(
                                    HP_GloopPrinceMin,
                                    HP_GloopPrinceMax,
                                    percentBossHP),
                                (int)MathHelper.Lerp(
                                    HP_GloopMin,
                                    HP_GloopMax,
                                    percentMinionHP),
                                intensity,
                                TypesOfPlayObjects.Enemy_GloopPrince,
                                TypesOfPlayObjects.Enemy_Gloop);
                            break;
                        case 1:
                            // Gloop king
                            thisWave.Wavelets[0] = waveTemplates.GenerateBoss(
                                numOfBosses,
                                (int)MathHelper.Lerp(
                                    HP_GloopKingMin,
                                    HP_GloopKingMax,
                                    percentBossHP),
                                (int)MathHelper.Lerp(
                                    HP_GloopMin,
                                    HP_GloopMax,
                                    percentMinionHP),
                                intensity,
                                TypesOfPlayObjects.Enemy_KingGloop,
                                TypesOfPlayObjects.Enemy_Gloop);
                            break;
                        case 2:
                            // Metal tooth
                            thisWave.Wavelets[0] = waveTemplates.GenerateBoss(
                                numOfBosses,
                                (int)MathHelper.Lerp(
                                    HP_MetalToothMin,
                                    HP_MetalToothMax,
                                    percentBossHP),
                                (int)MathHelper.Lerp(
                                    HP_MiniSawMin,
                                    HP_MiniSawMax,
                                    percentMinionHP),
                                intensity,
                                TypesOfPlayObjects.Enemy_MetalTooth,
                                TypesOfPlayObjects.Enemy_MiniSaw);
                            break;
                        case 3:
                            // Unclean Rot
                            thisWave.Wavelets[0] = waveTemplates.GenerateBoss(
                                numOfBosses,
                                (int)MathHelper.Lerp(
                                    HP_UncleanRotMin,
                                    HP_UncleanRotMax,
                                    percentBossHP),
                                (int)MathHelper.Lerp(
                                    HP_StaticGloopMin,
                                    HP_StaticGloopMax,
                                    percentMinionHP),
                                intensity,
                                TypesOfPlayObjects.Enemy_UncleanRot,
                                TypesOfPlayObjects.Enemy_StaticGloop);
                            break;
                        case 4:
                            // Pyre
                            thisWave.Wavelets[0] = waveTemplates.GenerateBoss(
                                numOfBosses,
                                (int)MathHelper.Lerp(
                                    HP_PyreMin,
                                    HP_PyreMax,
                                    percentBossHP),
                                (int)MathHelper.Lerp(
                                    HP_EmberMin,
                                    HP_EmberMax,
                                    percentMinionHP),
                                intensity,
                                TypesOfPlayObjects.Enemy_Pyre,
                                TypesOfPlayObjects.Enemy_Ember);
                            break;
                        default:
                            // ProtoNora
                            thisWave.Wavelets[0] = waveTemplates.GenerateBoss(
                                numOfBosses,
                                (int)MathHelper.Lerp(
                                    HP_ProtoNoraMin,
                                    HP_ProtoNoraMax,
                                    percentBossHP),
                                (int)MathHelper.Lerp(
                                    HP_AnnMoebaMin,
                                    HP_AnnMoebaMax,
                                    percentMinionHP),
                                intensity,
                                TypesOfPlayObjects.Enemy_ProtoNora,
                                TypesOfPlayObjects.Enemy_AnnMoeba);
                            break;
                    }
                }
                else if (relativeWaveNum == 90)
                {
                    // Lahmu
                    // Determine num of bosses
                    int numBosses = (int)maxNumberLahmu;
                    if (lastMajorWaveNo < 500)
                    {
                        numBosses = (int)MathHelper.Lerp(
                            minNumberLahmu,
                            maxNumberLahmu,
                            (float)lastMajorWaveNo / 500f);
                    }

                    // Determine HP
                    int bossHP = (int)MathHelper.Lerp(
                        HP_LahmuMin,
                        HP_LahmuMax,
                        (float)lastMajorWaveNo / (float)MaxMajorNumber);

                    // Determine delay
                    float delay = 0;
                    if (numBosses > 1)
                        delay = 8f * numBosses;

                    thisWave.Wavelets[0] = waveTemplates.GenerateBoss(
                        numBosses,
                        bossHP,
                        delay,
                        TypesOfPlayObjects.Enemy_Lahmu);
                }
                else
                {
                    // Moloch
                    // Determine HP
                }
            }
            else
            {
                // Figure out how many enemies we should be fighting
                int numOfEnemies = (int)MathHelper.Lerp(
                    min_NumberOfEnemies,
                    max_NumberOfEnemies,
                    (float)relativeWaveNum/100f) + 
                    MWMathHelper.GetRandomInRange(
                        min_EnemyRandomJitter, max_EnemyRandomJitter);

                // Figure out if we want a max starting HP
                int[] maxStartingHPs;
                if (MWMathHelper.CoinToss())
                {
                    // Uniform HP
                    maxStartingHPs = new int[]
                    {
                        (int)MathHelper.Lerp(
                            MathHelper.Lerp(min_HPBaseline, max_HPBaseline,
                                (float)lastMajorWaveNo/(float)MaxMajorNumber),
                            max_TotalHP,
                            (float)relativeWaveNum/100f),
                    };
                }
                else
                {
                    // Some array of HP
                    int start = (int)MathHelper.Lerp(min_HPBaseline, max_HPBaseline,
                        (float)lastMajorWaveNo/(float)MaxMajorNumber);
                    int end = (int)MathHelper.Lerp(start, max_TotalHP,
                        (float)relativeWaveNum/100f);
                    maxStartingHPs = new int[end - start + 1];
                    int index = 0;
                    for (int i = start; i <= end; i++)
                    {
                        maxStartingHPs[index] = i;
                        index++;
                    }
                }

                // Figure out if we want a max delay
                float maxDelay = 0;
                if (numOfEnemies > 15)
                {
                    maxDelay = MathHelper.Lerp(min_EnemyDelay,
                        (float)MWMathHelper.GetRandomInRange(maxMin_EnemyDelay, maxMax_EnemyDelay),
                        (float)(numOfEnemies-15) / (float)(max_NumberOfEnemies-15));
                }

                // Figure out the enemies we should use
                TypesOfPlayObjects[] enemiesToUse;
                int enemyTierToUse = (int)MathHelper.Lerp(-1, enemyClusters.Length - 2,
                    (float)relativeWaveNum / 100f) + 1;
                int numberOfEnemyTypesToUse = MWMathHelper.GetRandomInRange(1,
                    (int)MathHelper.Min(numOfEnemies, enemyClusters[enemyTierToUse].Length));
                int enemyIndex = MWMathHelper.GetRandomInRange(0, enemyClusters[enemyTierToUse].Length - 1);

                enemiesToUse = new TypesOfPlayObjects[numberOfEnemyTypesToUse];
                for (int i = 0; i < numberOfEnemyTypesToUse; i++)
                {
                    enemiesToUse[i] = enemyClusters[enemyTierToUse][enemyIndex];
                    enemyIndex++;
                    if (enemyIndex >= enemyClusters[enemyTierToUse].Length)
                        enemyIndex = 0;
                }

                // Generate the wavelet(s)
                thisWave.Wavelets[0] = waveTemplates.GenerateWavelet(
                    numOfEnemies,
                    enemiesToUse,
                    maxStartingHPs,
                    maxDelay);
            }

            thisWave.Wavelets[0].SongID = currentSong;
            /*

            int hitsToKillEnemy = 0;
            thisWave.Wavelets[thisWave.CurrentWavelet] =
                new Wavelet(NumEnemies, hitsToKillEnemy);
            thisWave.Wavelets[1] = new Wavelet(NumEnemies, hitsToKillEnemy);


            /*thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[0] = TypesOfPlayObjects.Enemy_Wiggles;
            thisWave.Wavelet[thisWave.CurrentWavelet].StartAngle[0] = MathHelper.PiOver2;

            thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[1] = TypesOfPlayObjects.Enemy_Buzzsaw;
            thisWave.Wavelet[thisWave.CurrentWavelet].StartAngle[1] = MathHelper.Pi;* /

            
            for (int i = 0; i < thisWave.NumEnemies; i++)
            {
                /*if ((float)i / 2f == i / 2)
                    thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                else
                    thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;* /
                //thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Ember;
                //thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Gloop;
                //if(MWMathHelper.IsEven(i))
                    //thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Wiggles;
                //else
                    thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                thisWave.Wavelets[thisWave.CurrentWavelet].StartAngle[i] = (float)InstanceManager.Random.NextDouble() * MathHelper.TwoPi;
                thisWave.Wavelets[thisWave.CurrentWavelet].ColorPolarities[i] = ColorState.RandomPolarity();

                thisWave.Wavelets[1].Enemies[i] = TypesOfPlayObjects.Enemy_Placeholder;
                thisWave.Wavelets[1].StartAngle[i] = (float)InstanceManager.Random.NextDouble() * MathHelper.TwoPi;
                thisWave.Wavelets[1].ColorPolarities[i] = ColorState.RandomPolarity();

            }
            //thisWave.Wavelet[thisWave.CurrentWavelet].Enemies[80] = TypesOfPlayObjects.Enemy_KingGloop;
            /*thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[59] = TypesOfPlayObjects.Enemy_Pyre;
            thisWave.Wavelets[thisWave.CurrentWavelet].StartAngle[59] = MathHelper.TwoPi;* /

            thisWave.Wavelets[thisWave.CurrentWavelet].Enemies[thisWave.NumEnemies - 1] = TypesOfPlayObjects.Enemy_MolochIntro;//enemiesToSpawn[currentEnemyIndex];
            thisWave.Wavelets[thisWave.CurrentWavelet].StartAngle[thisWave.NumEnemies - 1] = MathHelper.Pi;
            thisWave.Wavelets[thisWave.CurrentWavelet].StartHitPoints[thisWave.NumEnemies - 1] = 0;

            thisWave.Wavelets[1].Enemies[thisWave.NumEnemies - 1] = TypesOfPlayObjects.Enemy_Moloch;//enemiesToSpawn[currentEnemyIndex];
            thisWave.Wavelets[1].StartAngle[thisWave.NumEnemies - 1] = MathHelper.Pi;
            thisWave.Wavelets[1].StartHitPoints[thisWave.NumEnemies - 1] = 0;

            /*currentEnemyIndex++;
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
            Reset(default_StartingMajorNum, default_StartingMinorNum);
        }

        /// <summary>
        /// Reset the game back to a major and minor number specified
        /// </summary>
        public void Reset(int Major, int Minor)
        {
            CurrentMajorNumber = Major;
            CurrentMinorNumber = Minor;
            nextMajorNumToSwitchBackgroundOn = 0;
            wavesSinceColorStateChange = numberOfWavesPerColorStateChange;
            countdownToSwitchBackgroundElements = 0;
            countdownToSwitchSongs = 0;
        }

        /// <summary>
        /// Get the next wave
        /// </summary>
        /// <returns></returns>
        public GameWave GetNextWave()
        {
            GameWave temp;
            if (LocalInstanceManager.CurrentGameState == GameState.SurvivalGame)
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
