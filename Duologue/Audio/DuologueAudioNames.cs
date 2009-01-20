using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Duologue.Audio
{
    public static class DuologueAudioNames
    {
        
        // FIXME maybe some of this should be divided up among Music and SoundEffects...
        public const string engine = "Content\\Audio\\Duologue.xgs";
        public static void LoadAudio(Game param_game)
        {
            AudioManager am = new AudioManager(param_game, engine);
            Music.LoadAudio(param_game);
            SoundEffects.LoadAudio(param_game);
            am.Initialize();
        }
    }
}