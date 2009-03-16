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
        protected bool[,] intensityMap;  //TrackVolume = intensityMap[myIntensity,tracknumber]
        protected int myIntensity;
        protected int maxIntensity;

        public IntensityWidget(Song song, bool [,] map)
        {
            myIntensity = 1;
            myIntensity = (int)(maxIntensity * ServiceLocator.GetService<IntensityNotifier>().Intensity);
            intensityMap = map;
            maxIntensity = intensityMap.GetLength(0);
            parentSong = song;
            Attach();
        }

        /// <summary>
        /// UpdateIntensity is a callback which marks tracks as enabled or disabled based
        /// on the definition map in the song, and the current Intensity.
        /// Note that the actual update to play the correct cues always occurs elsewhere.
        /// </summary>
        /// <param name="e"></param>
        public void UpdateIntensity(IntensityEventArgs e)
        {
            if (e.ChangeAmount > 0)
                myIntensity++;
            else
                myIntensity--;

            myIntensity = MWMathHelper.LimitToRange(myIntensity, 1, maxIntensity);

            for (int t = 0; t < parentSong.TrackCount; t++)
            {
                parentSong.Tracks[t].Enabled = intensityMap[myIntensity - 1, t];
            }
        }

        public void Attach()
        {
            notifier = ServiceLocator.GetService<IntensityNotifier>();
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);
        }

        public void Detach()
        {
            notifier.Changed -= new IntensityEventHandler(UpdateIntensity);
            notifier = null;
        }
    }
}