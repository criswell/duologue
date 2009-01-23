using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio
{
    public interface IIntensitySong : ISong
    {
        //void Play(float percentVolume, float fadeinTime, float percentIntensity);
        void SetIntensity(float percentage);
        //float GetIntensity();
        //void ChangeIntensity(float percentage);
    }
}
