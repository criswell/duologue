using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mimicware;

namespace Duologue.Audio
{
    public class IntensityEventArgs : EventArgs
    {
        public int ChangeAmount;
    }

    public delegate void IntensityEventHandler(IntensityEventArgs e);
    public class IntensityNotifier : IService
    {
        protected float intensity;
        public float Intensity //value between 0 and 1, representing a percentage
        {
            get
            {
                return intensity;
            }
            set
            {
                intensity = MWMathHelper.LimitToRange(value, 0f, 1f);
            }
        }


        public event IntensityEventHandler Changed;
        protected virtual void OnChanged(IntensityEventArgs e)
        {
            if (Changed != null)
                Changed(e);
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

        public void RequestUpdate()
        {
            ChangeIntensity(0);
        }

        public void ResetIntensity()
        {
            //if a song is playing, it will assign an intensity
            //otherwise, it will stay zero. Well, good!
            intensity = 0f;
            Detensify();
        }


    }
}