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
        public List<Q> cues;
        public float Volume = Loudness.Normal;
        public bool VolumeChanging;

        //things relevant to songs with volume fades
        //public VolumeChangeWidget Fade;
        //protected int MILLISECONDS_FADE = 500;

        public Track()
        {
            cues = new List<Q>();
        }

        public Track(string soundbank, string[] argCues)
            : this()
        {
            SoundbankName = soundbank;
            foreach (string argCue in argCues)
            {
                Q newQ = new Q(SoundbankName, argCue);
                this.cues.Add(newQ);
            };
        }

        public void PlayBeat(int beat)
        {
            cues[beat - 1].Play();
        }

        /*
        public void ChangeVolume(float newVol, bool stop)
        {
            if (null != Fade)
            {
                Fade.ChangeVolume(newVol, stop);
            }
        }

        public void FadeIn(float vol)
        {
            SetFade(Loudness.Quiet, Loudness.Normal, MILLISECONDS_FADE); //FIXME
            Fade.FadeIn();
        }
         */

        public void Play()
        {
            cues.ForEach(q =>
            {
                q.Play();
            });
        }

        public void Stop()
        {
            cues.ForEach(q =>
            {
                q.Stop();
            });
        }
    }
}