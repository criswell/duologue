using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio
{
    public interface IIntensitySong : ISong
    {
        void Play(float percentVolume, float fadeinTime, float percentIntensity);
        void SetIntensity(float percentage);
        float GetIntensity();
        void ChangeIntensity(float percentage);
    }

/*
    public class ExampleIntensitySong : MainMenuMusic, IIntensitySong
    {
        private float intensity;

        public void Play(float percentVolume, float fadeinTime, float percentIntensity) 
        {
            this.SetIntensity(percentIntensity);
            this.Play(percentVolume, fadeinTime);
        }

        public void SetIntensity(float percentage)
        {
            if (percentage < 0.0f)
                percentage = 0.0f;
            else if (percentage > 1.0f)
                percentage = 1.0f;
            intensity = percentage;
        }

        public float GetIntensity()
        { 
            return intensity;
        }

        public void ChangeIntensity(float percentage) 
        {
            percentage = intensity + percentage;
            this.SetIntensity(percentage);
        }

    }
*/
}
