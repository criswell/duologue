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

        protected int steps;
        protected float stepAmount;
        protected Song parentSong;
        protected float updateTimer = 0f;

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
            Volume = VolumePresets.Full; //Full is really the truth, right? Until someone calls a change!
            Volume = VolumePresets.Quiet; //But that causes FadeIn to fade from full down to
                                            //the target volume, which sounds messed up
        }

        protected void SetTimingVars(int milliseconds)
        {
            milliseconds = MWMathHelper.LimitToRange(
                milliseconds, AudioConstants.MIN_VOL_CHANGE_MS, AudioConstants.MAX_VOL_CHANGE_MS);

            steps = milliseconds / AudioConstants.VOL_CHANGE_UPDATE_MS;

            steps = MWMathHelper.LimitToRange(
                steps, AudioConstants.MIN_VOL_CHANGE_STEPS, AudioConstants.MAX_VOL_CHANGE_STEPS); 

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

            //this comparison has given me anguish when I magically set the volume
            //for a particular song at the initial Volume set in init() above
            //Can't I just do the code inside this conditional regardless?
            
            if (newVol != Volume)
            {
                StartVolume = MWMathHelper.LimitToRange(
                    Volume, AudioConstants.MIN_XACT_VOL, AudioConstants.MAX_XACT_VOL);
                EndVolume = MWMathHelper.LimitToRange(
                    newVol, AudioConstants.MIN_XACT_VOL, AudioConstants.MAX_XACT_VOL);
                SetTimingVars(milliseconds);
                VolumeChanging = true;
                StopAfterChange = stop;
            }
            else
            {
                SetTimingVars(AudioConstants.WTF);
            }
        }

        public void FadeIn(float targetVolume)
        {
            ChangeVolume(targetVolume, AudioConstants.FADE_IN_TIME, false);
        }

        public void FadeOut()
        {
            ChangeVolume(VolumePresets.Quiet, AudioConstants.FADE_OUT_TIME, true);
        }

        public void Update(GameTime gameTime, Song song)
        {
            if (VolumeChanging)
            {
                updateTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (updateTimer > AudioConstants.VOL_CHANGE_UPDATE_MS)
                {
                    VolumeChanging = false;
                    Volume += stepAmount;

                    string message = "Change volume " + 
                        parentSong.SoundBankName + " " + Volume.ToString();
                    Debug.WriteLine(message);

                    //this is bookkeeping only: so if you check the Volume property
                    //on a Track, it will have the correct value. It should go away somehow.
                    //Edit: nuh uh. I commented this out, and it broke the hell out of it,
                    //so ignore what I said about it going away!
                    for (int t = 0; t < song.TrackCount; t++)
                        song.Tracks[t].ChangeVolume(Volume);

                    VolumeChanging =
                        (((StartVolume > EndVolume) && (Volume > EndVolume)) ||
                        ((StartVolume < EndVolume) && (Volume < EndVolume)));

                    AudioHelper.UpdateCues(song);
                    if (!VolumeChanging && StopAfterChange)
                    {
                        song.Stop();
                        StopAfterChange = false;
                    }
                    updateTimer = 0f;
                }
            }
        }
    }
}