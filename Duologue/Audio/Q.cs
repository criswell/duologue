using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio
{
    public class Q
    {
        public string SoundBankName;
        public string cueName;

        // this should only be set true for Cues that are set
        // to repeat (usually infinitely) in XACT.
        public bool AutoLoop;

        // this should only be set true for Cues that have had
        // volume change design done in XACT
        public bool Adjustable;

        public Q(string soundbank, string cue)
        {
            SoundBankName = soundbank;
            cueName = cue;
        }

        public void Play() { AudioHelper.Play(this); }
        public void Stop() { AudioHelper.Stop(this); }
        public void Pause() { AudioHelper.Pause(this); }
        public void Resume() { AudioHelper.Resume(this); }
    }
}
