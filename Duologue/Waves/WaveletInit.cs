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
                    switch (LocalInstanceManager.CurrentGameWave.Wavelet[LocalInstanceManager.CurrentGameWave.CurrentWavelet].Enemies[i])
                    {
                        case TypesOfPlayObjects.Enemy_Buzzsaw:
                            Init_Buzzsaw(i, manager);
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

        #region Buzzsaw
        private static void Init_Buzzsaw(int i, GamePlayScreenManager manager)
        {
            LocalInstanceManager.Enemies[i] = new Enemy_Buzzsaw(manager);
        }
        #endregion

        #endregion
    }
}
