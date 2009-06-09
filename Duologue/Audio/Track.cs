using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Duologue.Audio.Widgets;

namespace Duologue.Audio
{
    public class Track
    {
        public string SoundbankName;
        public Q[] Cues;
        public float Volume = VolumePresets.Normal;
        public bool VolumeChanging;
        public bool Enabled = true;
        public int QCount;

        public Track()
        {
        }

        public Track(string soundbank, string[] argCues)
            : this()
        {
            QCount = argCues.Length;
            Cues = new Q[QCount];
            SoundbankName = soundbank;
            for (int q = 0; q < QCount; q++)
            {
                Q newQ = new Q(SoundbankName, argCues[q]);
                Cues[q] = newQ;

            }
        }

        public void ChangeVolume(float newVol)
        {
            Volume = newVol;
            VolumeChanging = true;
        }

        public void Pause()
        {
            for (int q = 0; q < QCount; q++)
            {
                Cues[q].Pause();
            }
        }

        public void Resume()
        {
            for (int q = 0; q < QCount; q++)
            {
                Cues[q].Resume();
            }
        }

        public void Stop()
        {
            for (int q = 0; q < QCount; q++)
            {
                Cues[q].Stop();
            }
        }
    }
}