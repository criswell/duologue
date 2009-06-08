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

        public void PlayBeat(int beat)
        {
            //Manipulations by the IntensityWidget (if any) are reacted to here!
            if (Enabled)
            {
                Cues[beat - 1].Play();
            }
        }

        public void ChangeVolume(float newVol)
        {
            Volume = newVol;
            VolumeChanging = true;
            //if (null != Fade)
            {
                //Fade.ChangeVolume(newVol, stop);
            }
        }

        /*
        public void FadeIn(float vol)
        {
            SetFade(VolumePresets.Quiet, VolumePresets.Normal, MILLISECONDS_FADE); //FIXME
            Fade.FadeIn();
        }
         */

        public void Play()
        {
            for (int q = 0; q < QCount; q++)
            {
                Cues[q].Play();
            }
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