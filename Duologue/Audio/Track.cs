using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio
{
    public class Track
    {
        public string SoundbankName;
        public List<Q> cues;
        public float Volume = Loudness.Normal;

        // this should only be set true for Cues that are set
        // to repeat (usually infinitely) in XACT.
        //public bool AutoLoop;

        //things relevant to songs with volume fades
        public VolumeChangeWidget Fade;

        protected int MILLISECONDS_FADE = 500;

        //things relevant to "beat" songs, i.e. songs which we need
        // to be able to know the timing of audible events

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

        public bool VolumeChanging()
        {
            if (null != Fade)
            {
                return Fade.VolumeChanging;
            }
            else
            {
                return false;
            }
        }

        public void ChangeVolume(float newVol)
        {
            if (null != Fade)
            {
                Fade.ChangeVolume(newVol);
            }
        }

        public void IncrementFade()
        {
            if (null != Fade)
            {
                Volume = Fade.IncrementFade();
            }
        }

        public void SetFade(float start, float end, int mS)
        {
            Fade = new VolumeChangeWidget(start, end, mS);
        }

        public void FadeIn(float vol)
        {
            SetFade(Loudness.Quiet, Loudness.Normal, MILLISECONDS_FADE); //FIXME
            Fade.FadeIn(vol);
        }

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
