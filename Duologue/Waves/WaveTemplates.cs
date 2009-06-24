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
    /*public class Wavelet : Wavelet
    {
        /// <summary>
        /// Integer array deliminating the ordering of the enemies
        /// </summary>
        public int[] EnemyOrderTemplate;

        public Wavelet() : base() { }

        public Wavelet(int NumEnemies, int StartHP, ColorPolarity polarity)
            : base(NumEnemies, StartHP, polarity)
        {
            EnemyOrderTemplate = new int[NumEnemies];
            for (int i = 0; i < NumEnemies; i++)
            {
                EnemyOrderTemplate[i] = 0;
            }
        }

        public Wavelet(int NumEnemies, int StartHP)
            : base(NumEnemies, StartHP)
        {
            EnemyOrderTemplate = new int[NumEnemies];
            for (int i = 0; i < NumEnemies; i++)
            {
                EnemyOrderTemplate[i] = 0;
            }
        }
    }*/

    /*
    public struct WaveletList
    {
        public List<Wavelet> Wavelets;
    }
     */

    /// <summary>
    /// Defines various wave templates for use in survival mode
    /// </summary>
    public class WaveTemplates
    {
        #region Constants
        //private const int maxNumberOfMobNumbersAndLetsSayNumbersAgain = 20;
        //private const inf maxNumberOfTemplatesInList = 20;
        #endregion

        #region Fields
        private GameWaveManager myManager;
        //private List<Wavelet> masterList;
        //private List<int> possibleMobNumbers;
        //private Dictionary<int, List<int>> templateLookup;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the possible mob numbers
        /// </summary>
        /*public List<int> PossibleMobNumbers
        {
            get { return possibleMobNumbers; }
        }*/
        #endregion

        #region Construtor/Init
        public WaveTemplates(GameWaveManager Manager)
        {
            myManager = Manager;

        }

        #endregion

        #region Private position/delay template methods
        private Wavelet Get_UpperLeftCorner(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(MathHelper.PiOver2, MathHelper.Pi,
                    (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_UpperRightCorner(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(0, MathHelper.PiOver2,
                    (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_LowerLeftCorner(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(MathHelper.Pi, 3f * MathHelper.PiOver2,
                    (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_LowerRightCorner(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(3f * MathHelper.PiOver2, MathHelper.TwoPi,
                    (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_RightSideClockwise(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(MathHelper.PiOver4, -MathHelper.PiOver4,
                    (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_LeftSideClockwise(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(3f * MathHelper.PiOver4, 5f * MathHelper.PiOver4,
                    (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_RightSideCounterClockwise(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(-MathHelper.PiOver4, MathHelper.PiOver4,
                    (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_LeftSideCounterClockwise(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(5f * MathHelper.PiOver4, 3f * MathHelper.PiOver4,
                    (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_KittyCornerLeftRight(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                if(MWMathHelper.IsEven(i))
                    tempT.StartAngle[i] = MathHelper.Lerp(MathHelper.PiOver2, MathHelper.Pi,
                        (float)i / (float)tempT.Enemies.Length);
                else
                    tempT.StartAngle[i] = MathHelper.Lerp(3f * MathHelper.PiOver2, MathHelper.TwoPi,
                        (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_KittyCornerRightLeft(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                    tempT.StartAngle[i] = MathHelper.Lerp(MathHelper.Pi, 3f * MathHelper.PiOver2,
                        (float)i / (float)tempT.Enemies.Length);
                else
                    tempT.StartAngle[i] = MathHelper.Lerp(0, MathHelper.PiOver2,
                        (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_TopRightToLeft(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(0, MathHelper.Pi,
                        (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_TopLeftToRight(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(MathHelper.Pi, 0,
                        (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_BottomRightToLeft(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(MathHelper.TwoPi, MathHelper.Pi,
                        (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_BottomLeftToRight(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(MathHelper.Pi, MathHelper.TwoPi,
                        (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_FullClockwise(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(MathHelper.TwoPi, 0,
                        (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_FullCounterClockwise(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.StartAngle[i] = MathHelper.Lerp(0, MathHelper.TwoPi,
                        (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }

        private Wavelet Get_FullAlternating(int numMobs, float maxTime)
        {
            Wavelet tempT = new Wavelet(numMobs, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                if(MWMathHelper.IsEven(i))
                    tempT.StartAngle[i] = MathHelper.Lerp(MathHelper.TwoPi, 0,
                            (float)i / (float)tempT.Enemies.Length);
                else
                    tempT.StartAngle[i] = MathHelper.Lerp(0, MathHelper.TwoPi,
                            (float)i / (float)tempT.Enemies.Length);
                if (maxTime > 0)
                {
                    tempT.SpawnDelay[i] = (double)MathHelper.Lerp(0, maxTime,
                        (float)i / (float)tempT.Enemies.Length);
                }
            }
            return tempT;
        }
        #endregion

        #region Private enemy ordering methods
        private Wavelet Place_SimpleCycle(Wavelet wavelet, TypesOfPlayObjects[] enemiesToUse)
        {
            int poIndex = 0;
            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                wavelet.Enemies[i] = enemiesToUse[poIndex];
                poIndex++;
                if (poIndex >= enemiesToUse.Length)
                    poIndex = 0;
            }
            return wavelet;
        }
        private Wavelet Place_WaveCycle(Wavelet wavelet, TypesOfPlayObjects[] enemiesToUse)
        {
            int poIndex = 0;
            int delta = 1;
            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                wavelet.Enemies[i] = enemiesToUse[poIndex];
                poIndex += delta;
                if (poIndex >= enemiesToUse.Length)
                {
                    poIndex = enemiesToUse.Length - 1;
                    delta = -1;
                }
                else if (poIndex < 0)
                {
                    poIndex = 0;
                    delta = 1;
                }
            }
            return wavelet;
        }
        private Wavelet Place_Random(Wavelet wavelet, TypesOfPlayObjects[] enemiesToUse)
        {
            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                wavelet.Enemies[i] = enemiesToUse[MWMathHelper.GetRandomInRange(0, enemiesToUse.Length)];
            }
            return wavelet;
        }
        private Wavelet Place_Cluster(Wavelet wavelet, TypesOfPlayObjects[] enemiesToUse)
        {
            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                wavelet.Enemies[i] = enemiesToUse[(int)MathHelper.Lerp(
                    0, enemiesToUse.Length-1,
                    (float)i/(float)wavelet.Enemies.Length)];
            }
            return wavelet;
        }
        #endregion

        #region Private HP setting methods
        private Wavelet HP_SimpleCycle(Wavelet wavelet, int[] startingHPs)
        {
            int hpIndex = 0;
            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                wavelet.StartHitPoints[i] = startingHPs[hpIndex];
                hpIndex++;
                if (hpIndex >= startingHPs.Length)
                    hpIndex = 0;
            }
            return wavelet;
        }
        private Wavelet HP_WaveCycle(Wavelet wavelet, int[] startingHPs)
        {
            int hpIndex = 0;
            int delta = 1;
            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                wavelet.StartHitPoints[i] = startingHPs[hpIndex];
                hpIndex += delta;
                if (hpIndex >= startingHPs.Length)
                {
                    hpIndex = startingHPs.Length - 1;
                    delta = -1;
                }
                else if (hpIndex < 0)
                {
                    hpIndex = 0;
                    delta = 1;
                }
            }
            return wavelet;
        }
        private Wavelet HP_Random(Wavelet wavelet, int[] startingHPs)
        {
            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                wavelet.StartHitPoints[i] = startingHPs[MWMathHelper.GetRandomInRange(
                    0, startingHPs.Length)];
            }
            return wavelet;
        }
        private Wavelet HP_Cluster(Wavelet wavelet, int[] startingHPs)
        {
            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                wavelet.StartHitPoints[i] = startingHPs[(int)MathHelper.Lerp(
                    0, startingHPs.Length-1,
                    (float)i/(float)wavelet.Enemies.Length)];
            }
            return wavelet;
        }
        private Wavelet HP_RangedForward(Wavelet wavelet, int startingHP)
        {
            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                wavelet.StartHitPoints[i] = (int)MathHelper.Lerp(
                    0, startingHP,
                    (float)i/(float)wavelet.Enemies.Length);
            }
            return wavelet;
        }
        private Wavelet HP_RangedBackward(Wavelet wavelet, int startingHP)
        {
            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                wavelet.StartHitPoints[i] = (int)MathHelper.Lerp(
                    startingHP, 0,
                    (float)i / (float)wavelet.Enemies.Length);
            }
            return wavelet;
        }
        #endregion

        #region Private Colorstate setting methods
        private Wavelet Color_Random(Wavelet wavelet)
        {
            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                wavelet.ColorPolarities[i] = ColorState.RandomPolarity();
            }
            return wavelet;
        }
        private Wavelet Color_Alternating(Wavelet wavelet)
        {
            ColorPolarity p = ColorPolarity.Positive;
            ColorPolarity n = ColorPolarity.Negative;
            if (MWMathHelper.CoinToss())
            {
                p = ColorPolarity.Negative;
                n = ColorPolarity.Positive;
            }

            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                if (MWMathHelper.IsEven(i))
                    wavelet.ColorPolarities[i] = p;
                else
                    wavelet.ColorPolarities[i] = n;
            }
            return wavelet;
        }
        private Wavelet Color_HalfAndHalf(Wavelet wavelet)
        {
            ColorPolarity p = ColorPolarity.Positive;
            ColorPolarity n = ColorPolarity.Negative;
            if (MWMathHelper.CoinToss())
            {
                p = ColorPolarity.Negative;
                n = ColorPolarity.Positive;
            }

            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                if (i < wavelet.Enemies.Length/2f)
                    wavelet.ColorPolarities[i] = p;
                else
                    wavelet.ColorPolarities[i] = n;
            }
            return wavelet;
        }
        private Wavelet Color_Staggered(Wavelet wavelet)
        {
            ColorPolarity p = ColorPolarity.Positive;
            ColorPolarity n = ColorPolarity.Negative;
            if (MWMathHelper.CoinToss())
            {
                p = ColorPolarity.Negative;
                n = ColorPolarity.Positive;
            }

            int k = 0;
            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                k++;
                if (k <= 2)
                    wavelet.ColorPolarities[i] = p;
                else
                {
                    wavelet.ColorPolarities[i] = n;
                    k = 0;
                }
            }
            return wavelet;
        }
        private Wavelet Color_AllSame(Wavelet wavelet)
        {
            ColorPolarity p = ColorPolarity.Positive;
            if (MWMathHelper.CoinToss())
                p = ColorPolarity.Negative;

            for (int i = 0; i < wavelet.Enemies.Length; i++)
            {
                wavelet.ColorPolarities[i] = p;
            }
            return wavelet;
        }
        #endregion

        #region Public methods
        public Wavelet GenerateWavelet(int totalEnemies, TypesOfPlayObjects[] enemiesToUse, int[] startingHPs, float maxTime)
        {
            Wavelet temp;

            #region Wave Init
            if (totalEnemies >= 60)
            {
                #region 60+ ugliness
                switch (MWMathHelper.GetRandomInRange(0, 10))
                {
                    case 0:
                        temp = Get_BottomLeftToRight(totalEnemies, maxTime);
                        break;
                    case 1:
                        temp = Get_BottomRightToLeft(totalEnemies, maxTime);
                        break;
                    case 2:
                        temp = Get_FullAlternating(totalEnemies, maxTime);
                        break;
                    case 3:
                        temp = Get_FullClockwise(totalEnemies, maxTime);
                        break;
                    case 4:
                        temp = Get_FullCounterClockwise(totalEnemies, maxTime);
                        break;
                    case 5:
                        temp = Get_LeftSideClockwise(totalEnemies, maxTime);
                        break;
                    case 6:
                        temp = Get_LeftSideCounterClockwise(totalEnemies, maxTime);
                        break;
                    case 7:
                        temp = Get_RightSideClockwise(totalEnemies, maxTime);
                        break;
                    case 8:
                        temp = Get_TopLeftToRight(totalEnemies, maxTime);
                        break;
                    case 9:
                        temp = Get_TopRightToLeft(totalEnemies, maxTime);
                        break;
                    default:
                        temp = Get_RightSideCounterClockwise(totalEnemies, maxTime);
                        break;
                }
                #endregion
            }
            else if (totalEnemies > 20)
            {
                #region 20+ ugliness
                switch (MWMathHelper.GetRandomInRange(0, 12))
                {
                    case 0:
                        temp = Get_BottomLeftToRight(totalEnemies, maxTime);
                        break;
                    case 1:
                        temp = Get_BottomRightToLeft(totalEnemies, maxTime);
                        break;
                    case 2:
                        temp = Get_FullAlternating(totalEnemies, maxTime);
                        break;
                    case 3:
                        temp = Get_FullClockwise(totalEnemies, maxTime);
                        break;
                    case 4:
                        temp = Get_FullCounterClockwise(totalEnemies, maxTime);
                        break;
                    case 5:
                        temp = Get_KittyCornerLeftRight(totalEnemies, maxTime);
                        break;
                    case 6:
                        temp = Get_KittyCornerRightLeft(totalEnemies, maxTime);
                        break;
                    case 7:
                        temp = Get_LeftSideClockwise(totalEnemies, maxTime);
                        break;
                    case 8:
                        temp = Get_LeftSideCounterClockwise(totalEnemies, maxTime);
                        break;
                    case 9:
                        temp = Get_RightSideClockwise(totalEnemies, maxTime);
                        break;
                    case 10:
                        temp = Get_TopLeftToRight(totalEnemies, maxTime);
                        break;
                    case 11:
                        temp = Get_TopRightToLeft(totalEnemies, maxTime);
                        break;
                    default:
                        temp = Get_RightSideCounterClockwise(totalEnemies, maxTime);
                        break;
                }
                #endregion
            }
            else
            {
                #region everything else
                switch (MWMathHelper.GetRandomInRange(0, 16))
                {
                    case 0:
                        temp = Get_BottomLeftToRight(totalEnemies, maxTime);
                        break;
                    case 1:
                        temp = Get_BottomRightToLeft(totalEnemies, maxTime);
                        break;
                    case 2:
                        temp = Get_FullAlternating(totalEnemies, maxTime);
                        break;
                    case 3:
                        temp = Get_FullClockwise(totalEnemies, maxTime);
                        break;
                    case 4:
                        temp = Get_FullCounterClockwise(totalEnemies, maxTime);
                        break;
                    case 5:
                        temp = Get_KittyCornerLeftRight(totalEnemies, maxTime);
                        break;
                    case 6:
                        temp = Get_KittyCornerRightLeft(totalEnemies, maxTime);
                        break;
                    case 7:
                        temp = Get_LeftSideClockwise(totalEnemies, maxTime);
                        break;
                    case 8:
                        temp = Get_LeftSideCounterClockwise(totalEnemies, maxTime);
                        break;
                    case 9:
                        temp = Get_LowerLeftCorner(totalEnemies, maxTime);
                        break;
                    case 10:
                        temp = Get_LowerRightCorner(totalEnemies, maxTime);
                        break;
                    case 11:
                        temp = Get_RightSideClockwise(totalEnemies, maxTime);
                        break;
                    case 12:
                        temp = Get_RightSideCounterClockwise(totalEnemies, maxTime);
                        break;
                    case 13:
                        temp = Get_TopLeftToRight(totalEnemies, maxTime);
                        break;
                    case 14:
                        temp = Get_TopRightToLeft(totalEnemies, maxTime);
                        break;
                    case 15:
                        temp = Get_UpperLeftCorner(totalEnemies, maxTime);
                        break;
                    default:
                        temp = Get_UpperRightCorner(totalEnemies, maxTime);
                        break;
                }
                #endregion
            }
            #endregion

            #region Enemy placement ugliness
            switch (MWMathHelper.GetRandomInRange(0, 4))
            {
                case 0:
                    temp = Place_Cluster(temp, enemiesToUse);
                    break;
                case 1:
                    temp = Place_Random(temp, enemiesToUse);
                    break;
                case 2:
                    temp = Place_SimpleCycle(temp, enemiesToUse);
                    break;
                default:
                    temp = Place_WaveCycle(temp, enemiesToUse);
                    break;
            }
            #endregion

            #region Enemy HP ugliness
            if (startingHPs.Length < 2 && MWMathHelper.CoinToss())
            {
                if (MWMathHelper.CoinToss())
                {
                    temp = HP_RangedBackward(temp, startingHPs[0]);
                }
                else
                {
                    temp = HP_RangedForward(temp, startingHPs[0]);
                }
            }
            else
            {
                switch (MWMathHelper.GetRandomInRange(0, 4))
                {
                    case 0:
                        temp = HP_Cluster(temp, startingHPs);
                        break;
                    case 1:
                        temp = HP_Random(temp, startingHPs);
                        break;
                    case 2:
                        temp = HP_SimpleCycle(temp, startingHPs);
                        break;
                    default:
                        temp = HP_WaveCycle(temp, startingHPs);
                        break;
                }
            }
            #endregion

            #region Colorstate ugliness
            switch(MWMathHelper.GetRandomInRange(0, 5))
            {
                case 0:
                    temp = Color_Alternating(temp);
                    break;
                case 1:
                    temp = Color_HalfAndHalf(temp);
                    break;
                case 2:
                    temp = Color_Random(temp);
                    break;
                case 4:
                    temp = Color_AllSame(temp);
                    break;
                default:
                    temp = Color_Staggered(temp);
                    break;
            }
            #endregion

            return temp;
        }
        #endregion

        #region Public boss methods
        public Wavelet GenerateBoss(
            int numOfBosses, 
            int hitPointBoss,
            int hitPointMinion,
            float intensity, 
            TypesOfPlayObjects boss, 
            TypesOfPlayObjects minion)
        {
            Wavelet temp;

            temp = new Wavelet((int)(30 * intensity * numOfBosses), hitPointMinion);

            float[] startAngles = new float[numOfBosses];
            for (int i = 0; i < numOfBosses; i++)
            {
                startAngles[i] = (float)MWMathHelper.GetRandomInRange(0, MathHelper.TwoPi);
            }

            int currIndex = 0;
            for (int i = 0; i < temp.Enemies.Length; i++)
            {
                if (i <= temp.Enemies.Length - 1 - numOfBosses)
                    temp.Enemies[i] = minion;
                else
                {
                    temp.Enemies[i] = boss;
                    temp.StartHitPoints[i] = hitPointBoss;
                }

                temp.StartAngle[i] = startAngles[currIndex];
                currIndex++;
                if (currIndex >= startAngles.Length)
                    currIndex = 0;
                temp.ColorPolarities[i] = ColorState.RandomPolarity();
            }

            return temp;
        }

        public Wavelet Boss_Lahmu(int numOfBosses, int hitPoint, float intensity)
        {
            Wavelet temp;

            temp = new Wavelet(10, 0);

            return temp;
        }
        public Wavelet Boss_Moloch(int numOfBosses, int hitPoint, float intensity)
        {
            Wavelet temp;

            temp = new Wavelet(10, 0);

            return temp;
        }

        #endregion
    }
}
