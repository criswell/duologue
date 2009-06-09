using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public float Volume = VolumePresets.Normal;

        //This number should be in a fairly tight window - smaller numbers make a fade tax the CPU more,
        //larger numbers compromise the smoothness of the audio transition
        public const int UPDATE_MILLISECONDS = 50;
        //A "client" command only dictates the duration of the volume transition,
        //The rest is calculated by this class

        //or maybe we don't let the client dictate that...
        public const int FADE_IN_TIME = 1300;
        public const int FADE_OUT_TIME = 500;

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
            init();
            parentSong = song;
        }

        //allow for re-use of this widget over the life of a song
        protected void init()
        {
            StartVolume = VolumePresets.Quiet;
            Volume = VolumePresets.Quiet;
        }

        protected void SetTimingVars(int milliseconds)
        {
            milliseconds = MWMathHelper.LimitToRange(milliseconds, 1, 5000);
            steps = milliseconds / UPDATE_MILLISECONDS;
            steps = MWMathHelper.LimitToRange(steps, 1, 1000); 
            //left room in case UPDATE_MILLISECONDS is changed
            stepAmount = (EndVolume - StartVolume) / steps;
        }

        public void ChangeVolume(float newVol, int milliseconds, bool stop)
        {
            if (milliseconds == 0) //immediate mode
            {
                for (int t = 0; t < parentSong.TrackCount; t++)
                    parentSong.Tracks[t].ChangeVolume(newVol);
                Volume = newVol;
            }

            if (newVol != Volume)
            {
                StartVolume = MWMathHelper.LimitToRange(Volume, 0f, 100f);
                EndVolume = MWMathHelper.LimitToRange(newVol, 0f, 100f);
                SetTimingVars(milliseconds);
                VolumeChanging = true;
                StopAfterChange = stop;
            }
            else
            {
                SetTimingVars(5000);
            }
        }

        public void FadeIn(float targetVolume)
        {
            ChangeVolume(targetVolume, FADE_IN_TIME, false);
        }

        public void FadeOut()
        {
            ChangeVolume(VolumePresets.Quiet, FADE_OUT_TIME, true);
        }

        public void Update(GameTime gameTime, Song song)
        {
            if (VolumeChanging)
            {
                if (gameTime.TotalRealTime.TotalMilliseconds - previousVolChangeTime >
                    UPDATE_MILLISECONDS)
                {
                    previousVolChangeTime = gameTime.TotalRealTime.TotalMilliseconds;
                    VolumeChanging = false;
                    Volume += stepAmount;

                    string message = "Change volume " + 
                        parentSong.SoundBankName + " " + Volume.ToString();
                    Debug.WriteLine(message);

                    //this is bookkeeping only: so if you check the Volume property
                    //on a Track, it will have the correct value. It should go away somehow.
                    //for (int t = 0; t < song.TrackCount; t++)
                    //    song.Tracks[t].ChangeVolume(Volume);

                    VolumeChanging =
                        (((StartVolume > EndVolume) && (Volume > EndVolume)) ||
                        ((StartVolume < EndVolume) && (Volume < EndVolume)));

                    AudioHelper.UpdateCues(song);
                    if (!VolumeChanging && StopAfterChange)
                    {
                        song.Stop();
                        StopAfterChange = false;
                    }

                }
            }
        }
    }
}