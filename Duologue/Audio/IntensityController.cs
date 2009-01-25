using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mimicware;

namespace Duologue.Audio
{
    class IntensityController
    {
        private static float intensity = 0.0f;
        public static float Intensity
        {
            get
            {
                return intensity;
            }
            set
            {
                intensity = MWMathHelper.LimitToRange(value, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// Set the intensity for the channel parameter
        /// </summary>
        /// <param name="channel">which intensity to set</param>
        /// <param name="intensity">float value between 0.0 and 1.0</param>
        static public void SetIntensity(int channel, float intensity)
        {
        }

        /// <summary>
        /// Increase the master or default game intensity
        /// </summary>
        /// <param name="change">float value between 0.0 and 1.0</param>
        static public void IncreaseIntensity(float change)
        {
            Intensity += change;
        }

        /// <summary>
        /// Increase the intensity for the channel parameter
        /// </summary>
        /// <param name="channel">which intensity to increase</param>
        /// <param name="intensity">float value between 0.0 and 1.0</param>
        static public void IncreaseIntensity(int channel, float intensity)
        {
        }
    }
}