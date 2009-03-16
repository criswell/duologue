using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio
{
    public class Q
    {
        public string SoundBankName;
        public string CueName;

        // this should only be set true for Cues that are set
        // to repeat (usually infinitely) in XACT.
        public bool AutoLoop;

        public Q(string soundbank, string cue)
        {
            SoundBankName = soundbank;
            CueName = cue;
        }

        public void Play() { AudioHelper.Play(this, true); }
        public void Stop() { AudioHelper.Stop(this); }
        public void Pause() { AudioHelper.Pause(this); }
        public void Resume() { AudioHelper.Resume(this); }
        public void ChangeVolume(float vol)
        {
           
        }
    }
}
