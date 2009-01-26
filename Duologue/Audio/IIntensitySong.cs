using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio
{
    public interface IIntensitySong : ISong
    {
        void ChangeIntensity(int amount);
        void SetIntensityPercentage(float percentage);
        float GetIntensityPercentage();
    }
}
