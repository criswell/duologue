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
        protected bool[,] intensityMap;  //TrackVolume = intensityMap[intensity,tracknumber]
        protected int intensity;
        protected int maxIntensity;
        protected bool attached = false;

        public int Intensity
        {
            get 
            {
                return intensity;
            }
            set 
            {
                intensity = MWMathHelper.LimitToRange(value, 1, maxIntensity);
            }
        }

        public IntensityWidget(Song song, bool [,] map)
        {
            parentSong = song;
            intensityMap = map;
            maxIntensity = intensityMap.GetLength(0);
            SetIntensity(ServiceLocator.GetService<IntensityNotifier>().Intensity);
            Attach();
        }

        public void SetIntensity(float percent)
        {
            intensity = (int)(maxIntensity * percent);
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
                intensity++;
            else if (e.ChangeAmount < 0)
                intensity--;

            intensity = MWMathHelper.LimitToRange(intensity, 1, maxIntensity);

            for (int t = 0; t < parentSong.TrackCount; t++)
            {
                parentSong.Tracks[t].Enabled = intensityMap[intensity - 1, t];
            }
            ServiceLocator.GetService<IntensityNotifier>().Intensity = 
                ((float)intensity) / ((float)maxIntensity);

        }

        public void Attach()
        {
            if (!attached)
            {
                attached = true;
                notifier = ServiceLocator.GetService<IntensityNotifier>();
                notifier.Changed += new IntensityEventHandler(UpdateIntensity);
            }
            else
            {
                throw new Exception("Already Attached!");
            }
        }

        public void Detach()
        {
            if (attached)
            {
                attached = false;
                notifier.Changed -= new IntensityEventHandler(UpdateIntensity);
                notifier = null;
            }
            else
            {
                throw new Exception("Not attached so I can not Detach!");
            }

        }
    }
}