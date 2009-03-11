using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Duologue.Audio;
using Mimicware;

namespace Duologue.Audio.Widgets
{
    public class VolumeChangeWidget
    {
        //Most recently commanded Volume
        public float Volume;

        //This number should be in a fairly tight window - smaller numbers make a fade tax the CPU more,
        //larger numbers compromise the smoothness of the audio transition
        public const int UPDATE_MILLISECONDS = 50;
        //A "client" command only dictates the duration of the volume transition,
        //The rest is calculated by this class

        protected int steps;
        protected float stepAmount;
        protected double previousVolChangeTime;
        protected Song parentSong;

        public float StartVolume; //prefer read-only, set in c'tor
        public float EndVolume; //prefer read-only, set in c'tor
        public bool VolumeChanging = false; //prefer read-only, set in c'tor
        public bool StopAfterChange;

        public VolumeChangeWidget(Song song)
        {
            parentSong = song;
        }

        //allow for re-use of this widget over the life of a song
        protected void init()
        {
        }

        protected void SetTimingVars(int milliseconds)
        {
            milliseconds = MWMathHelper.LimitToRange(milliseconds, 1, 5000);
            steps = milliseconds / UPDATE_MILLISECONDS;
            stepAmount = (EndVolume - StartVolume) / steps;
        }

        public void ChangeVolume(float newVol, int milliseconds, bool stop)
        {
            if (newVol != Volume)
            {
                StartVolume = MWMathHelper.LimitToRange(Volume, 0f, 1f);
                EndVolume = MWMathHelper.LimitToRange(newVol, 0f, 1f);
                SetTimingVars(milliseconds);
                VolumeChanging = true;
                StopAfterChange = stop;
            }
        }

        public void FadeIn(int milliseconds)
        {
            ChangeVolume(Loudness.Normal, milliseconds, false);
        }

        public void FadeIn(float volume, int milliseconds)
        {
            ChangeVolume(volume, milliseconds, false);
        }

        public void FadeOut(int milliseconds)
        {
            ChangeVolume(Loudness.Silent, milliseconds, true);
        }

        protected float IncrementFade()
        {
            if (VolumeChanging)
            {
                Volume += (EndVolume - StartVolume) / (float)steps;
                if (((StartVolume > EndVolume) && !(Volume > EndVolume)) ||
                    ((StartVolume < EndVolume) && !(EndVolume > Volume)))
                {
                    VolumeChanging = false;
                }
            }
            return Volume;
        }

        public void Update(GameTime gameTime, Song song)
        {
            if (gameTime.TotalRealTime.TotalMilliseconds - previousVolChangeTime > 
                UPDATE_MILLISECONDS)
            {
                previousVolChangeTime = gameTime.TotalRealTime.TotalMilliseconds;
                VolumeChanging = false;

                for (int t = 0; t < song.TrackCount; t++)
                {
                    for (int q = 0; q < song.Tracks[t].QCount; q++)
                    {
                        song.Tracks[t].Cues[q].ChangeVolume(IncrementFade());
                    }
                    VolumeChanging |= song.Tracks[t].VolumeChanging;
                }

                AudioHelper.UpdateCues(song);
                if (!VolumeChanging && StopAfterChange)
                {
                    song.Stop();
                }
            }
        }

    }
}