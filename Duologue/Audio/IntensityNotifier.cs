using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio
{
    public class IntensityEventArgs : EventArgs
    {
        public int ChangeAmount;
    }

    public delegate void IntensityEventHandler(IntensityEventArgs e);
    public class IntensityNotifier : IService
    {
        public event IntensityEventHandler Changed;
        protected virtual void OnChanged(IntensityEventArgs e)
        {
            if (Changed != null)
                Changed(e);
        }

        public float GetIntensity(SongID ID)
        {
            try
            {
                AudioManager am = ServiceLocator.GetService<AudioManager>();
                //WTF. The current song should be the trusted repository for the global
                //intensity. That shit needs to be stored HERE!
                if (am.SongIsPlaying(ID))
                {
                    //return songMap[ID].GetIntensityPercentage();
                    return 1f;
                }
            }
            catch (Exception e)
            {
            }
            return 0f;
        }

        public void SetIntensity(SongID ID, float percentage)
        {
            try
            {
                //songMap[ID].SetIntensityPercentage(percentage);
            }
            catch (Exception e)
            {
            }
        }

        public void Intensify()
        {
            ChangeIntensity(1);
        }

        protected void ChangeIntensity(int amount)
        {
            IntensityEventArgs e = new IntensityEventArgs();
            e.ChangeAmount = amount;
            OnChanged(e);
        }

        public void Detensify()
        {
            ChangeIntensity(-1);
        }
    }
}