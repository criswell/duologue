#region File Description
#endregion

#region Using Statements
// System
using System;
using System.Collections.Generic;
using System.Collections;
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
using Duologue.State;
#endregion

namespace Duologue.Waves
{
    public enum EnemyType
    {
        None,
        Buzzsaw,
    }

    /// <summary>
    /// Represents a base wavelet of enemies of a specific type, number, and specification.
    /// </summary>
    public class Wavelet
    {
        #region Constants
        /// <summary>
        /// The maximum number of parameters this wavelet can use.
        /// We really shouldn't need anything big... should we?
        /// In fact, wont 5 be "too many"? (Famous last words?)
        /// </summary>
        public int maxParams = 5;
        #endregion

        #region Fields
        private int onScreenNumber;
        #endregion

        #region Properties
        /// <summary>
        /// The enemy type to use
        /// </summary>
        public EnemyType Type;

        /// <summary>
        /// The total number of enemies to use
        /// </summary>
        public int TotalNumber;

        /// <summary>
        /// The total number of enemies to have on screen at a given time
        /// </summary>
        public int OnScreenNumber
        {
            get { return onScreenNumber; }
            set { onScreenNumber = Math.Min(value, TotalNumber); }
        }

        public Hashtable InitParams;
        #endregion

        #region Constructor
        public Wavelet()
        {
            // set to sensible defaults
            onScreenNumber = 0;
            Type = EnemyType.None;
            TotalNumber = 0;
            InitParams = new Hashtable(maxParams);
        }
        #endregion
    }
}
