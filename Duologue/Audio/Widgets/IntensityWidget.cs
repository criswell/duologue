using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio.Widgets
{
    public class IntensityWidget
    {
        protected IntensityNotifier notifier;
        protected Song parentSong;

        public IntensityWidget(Song song)
        {
            parentSong = song;
            notifier = ServiceLocator.GetService<IntensityNotifier>();
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);
        }

        public void UpdateIntensity(IntensityEventArgs e)
        {
            //parentSong.Tracks
        }

        public void Detach()
        {
            notifier.Changed -= new IntensityEventHandler(UpdateIntensity);
            notifier = null;
        }


    }
}
