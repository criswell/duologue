﻿#region File Description
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
#endregion

namespace Duologue.Waves
{
    /// <summary>
    /// For now, we'll just be storing these in memory. At a later date, we likely will
    /// want these to be stored on disk
    /// </summary>
    public class WaveDefinitions
    {
        /// <summary>
        /// The waves used in this game
        /// </summary>
        public List<GameWave> Waves;
    }
}
