using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mimicware;

namespace Duologue.Audio.Widgets
{
    public class IntensityWidget
    {
        protected IntensityNotifier notifier;
        protected Song parentSong;
        protected float[,] intensityMap;
        protected int myIntensity; //this will ONLY be meaningful to one song

        public IntensityWidget(Song song, float [,] map)
        {
            myIntensity = 1;
            intensityMap = map;
            parentSong = song;
            notifier = ServiceLocator.GetService<IntensityNotifier>();
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);
        }

        public void UpdateIntensity(IntensityEventArgs e)
        {
            if (e.ChangeAmount > 0)
                myIntensity++;
            else
                myIntensity--;

            myIntensity = MWMathHelper.LimitToRange(myIntensity, 1, 5);
            
            for (int t = 0; t < parentSong.TrackCount; t++)
            {
                if (intensityMap[myIntensity-1, t] == Loudness.Silent)
                {
                    parentSong.Tracks[t].Enabled = false;
                }
                else
                {
                    parentSong.Tracks[t].Enabled = true;
                }   
            }
        }

        public void Detach()
        {
            notifier.Changed -= new IntensityEventHandler(UpdateIntensity);
            notifier = null;
        }


    }
}
