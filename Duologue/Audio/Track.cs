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

        public Track[] CreateTrackArray(string[,] arrangement)
        {
            int TrackCount = arrangement.GetLength(0);
            int cueCount = arrangement.GetLength(1);
            Track[] Tracks = new Track[TrackCount];

            for (int track = 0; track < TrackCount; track++)
            {
                string[] row = new string[cueCount];
                for (int q = 0; q < cueCount; q++)
                {
                    row[q] = arrangement[track, q];
                }
                Tracks[track] = new Track(SoundbankName, row);
            }
            return Tracks;
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