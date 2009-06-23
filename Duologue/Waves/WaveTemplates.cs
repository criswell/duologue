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

            //GenerateMasterList();
        }

        private void GenerateMasterList()
        {
            /*possibleMobNumbers = new List<int>(maxNumberOfMobNumbersAndLetsSayNumbersAgain);

            possibleMobNumbers.Add(4);
            possibleMobNumbers.Add(8);
            possibleMobNumbers.Add(10);
            possibleMobNumbers.Add(12);
            possibleMobNumbers.Add(20);
            possibleMobNumbers.Add(25);
            possibleMobNumbers.Add(30);
            possibleMobNumbers.Add(35);
            possibleMobNumbers.Add(40);
            possibleMobNumbers.Add(50);
            possibleMobNumbers.Add(60);
            possibleMobNumbers.Add(80);
            possibleMobNumbers.Add(100);

            templateLookup = new Dictionary<int,List<int>>();

            masterList = new List<Wavelet>();

            int tempIndex = 0;
            //List<int> tempList = new List<int>(maxNumberOfTemplatesInList);

            // The upper left corner ones
            
            Wavelet tempT = new Wavelet(4, 0);
            for (int i = 0; i < tempT.Enemies.Length; i++)
            {
                tempT.EnemyOrderTemplate[i] = 0;
                tempT.StartAngle[i] = MathHelper.Lerp(MathHelper.PiOver2, MathHelper.Pi,
                    (float)i / (float)tempT.Enemies.Length);
            }
            masterList.Add(tempT);
            //tempList.Add(tempIndex);
            tempIndex++;
             * */
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

        #region Public methods
        public Wavelet GenerateWavelet(int totalEnemies, TypesOfPlayObjects[] enemiesToUse, float maxTime)
        {
            Wavelet temp;

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

            return temp;
        }
        #endregion
    }
}
