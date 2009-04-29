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
//using Mimicware.Manager;
//using Mimicware.Graphics;
// Duologue
using Duologue;
//using Duologue.Properties;
using Duologue.Screens;
//using Duologue.UI;
using Duologue.PlayObjects;
using Duologue.Waves;
//using Duologue.State;
#endregion

namespace Duologue.Waves
{
    /// <summary>
    /// Wave Init routine that will simply be too big to include sanely elsewhere
    /// </summary>
    public static class WaveletInit
    {
        #region Initialize
        /// <summary>
        /// Initialize the wavelet
        /// </summary>
        public static bool Initialize(GamePlayScreenManager manager)
        {
            if (LocalInstanceManager.CurrentGameWave.CurrentWavelet < LocalInstanceManager.CurrentGameWave.NumWavelets)
            {
                LocalInstanceManager.Enemies = new Enemy[LocalInstanceManager.CurrentGameWave.NumEnemies];
                LocalInstanceManager.CurrentNumberEnemies = LocalInstanceManager.CurrentGameWave.NumEnemies;
                for (int i = 0; i < LocalInstanceManager.CurrentGameWave.NumEnemies; i++)
                {
                    switch (LocalInstanceManager.CurrentGameWave.Wavelets[LocalInstanceManager.CurrentGameWave.CurrentWavelet].Enemies[i])
                    {
                        case TypesOfPlayObjects.Enemy_Buzzsaw:
                            Init_Buzzsaw(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_Wiggles:
                            Init_Wiggles(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_Spitter:
                            Init_Spitter(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_Gloop:
                            Init_Gloop(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_KingGloop:
                            Init_KingGloop(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_StaticGloop:
                            Init_StaticGloop(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_Pyre:
                            Init_Pyre(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_Ember:
                            Init_Ember(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_UncleanRot:
                            Init_UncleanRot(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_Mirthworm:
                            Init_Mirthworm(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_AnnMoeba:
                            Init_AnnMoeba(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_ProtoNora:
                            Init_ProtoNora(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_Maggot:
                            Init_Maggot(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_Roggles:
                            Init_Roggles(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_Spawner:
                            Init_Spawner(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_Placeholder:
                            Init_Placeholder(i, manager);
                            break;
                        case TypesOfPlayObjects.Enemy_MetalTooth:
                            Init_MetalTooth(i, manager);
                            break;
                        default:
                            // Squat, for now
                            break;
                    }
                }
                
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Enemy inits
        private static void Init_MetalTooth(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_MetalTooth(manager);
        }

        private static void Init_Placeholder(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_Placeholder(manager);
        }

        private static void Init_Spawner(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_Spawner(manager);
        }

        private static void Init_Roggles(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_Roggles(manager);
        }

        private static void Init_Maggot(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_Maggot(manager);
        }

        private static void Init_ProtoNora(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_ProtoNora(manager);
        }

        private static void Init_AnnMoeba(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_AnnMoeba(manager);
        }

        private static void Init_Mirthworm(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_Mirthworm(manager);
        }

        private static void Init_UncleanRot(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_UncleanRot(manager);
        }

        private static void Init_Ember(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_Ember(manager);
        }

        private static void Init_Pyre(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_Pyre(manager);
        }

        private static void Init_Buzzsaw(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_Buzzsaw(manager);
        }

        private static void Init_Wiggles(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_Wiggles(manager);
        }

        private static void Init_Spitter(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_Spitter(manager);
        }

        private static void Init_Gloop(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_Gloop(manager);
        }

        private static void Init_KingGloop(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_GloopKing(manager);
        }

        private static void Init_StaticGloop(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_StaticGloop(manager);
        }
        #endregion
    }
}
