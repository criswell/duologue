using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Mimicware;

namespace Duologue.Audio
{

    public class IntensitySong : Song
    {
        protected int intensityStep = 0; //trick into an update the first time
        public int MaxIntensity;

        protected Dictionary<string, float>[] intensityCueVolume; //must know array size before init

        public IntensitySong(Game game, string sbname, string wbname,
            string[] cues, float[,] vols)
            : base(game, sbname, wbname) 
        {
            MaxIntensity = vols.GetLength(0);
            Tracks = new Dictionary<string, Track>();
            for (int cueNum = 0; cueNum < vols.GetLength(1); cueNum++)
            {
                Tracks.Add(cues[cueNum], new Track(cues[cueNum], vols[0, cueNum]));
            }

            intensityCueVolume = new Dictionary<string, float>[MaxIntensity];
            for (int i = 0; i < MaxIntensity; i++)
            {
                intensityCueVolume[i] = new Dictionary<string, float>();
                for (int cueNum = 0; cueNum < vols.GetLength(1); cueNum++)
                {
                    intensityCueVolume[i].Add(cues[cueNum], vols[i, cueNum]);
                }
            }
            AudioHelper.PreloadSong(this);
        }

        public override void Play()
        {
            if (!isPlaying)
            {
                base.Play();
            }
            ChangeIntensity(0);
        }

        public void FadeIn(int intensity)
        {
            if (!isPlaying)
            {
                base.Play();
            }
            ChangeIntensity(intensity - intensityStep);
        }

        public void ChangeIntensity(int amount)
        {
            int oldIntensityStep = intensityStep;
            intensityStep =
                MWMathHelper.LimitToRange(intensityStep + amount, 1, MaxIntensity);
            if (intensityStep != oldIntensityStep || amount == 0)
            {
                Tracks.Keys.ToList().ForEach(name =>
                {
                    Tracks[name].ChangeVolume(intensityCueVolume[intensityStep - 1][name]);
                });
                ChangeVolume(false);
            }
        }

        public float GetIntensityPercentage()
        {
            return (float)intensityStep / (float)MaxIntensity;
        }

        public void SetIntensityPercentage(float percentage)
        {
            //FIXME - needs to be based on variable # of intensities
            MWMathHelper.LimitToRange(percentage, 0f, 1f);
            if (percentage > 0.83f)
                intensityStep = 6;
            else if (percentage > 0.67f)
                intensityStep = 5;
            else if (percentage > 0.5f)
                intensityStep = 4;
            else if (percentage >= 0.33f)
                intensityStep = 3;
            else if (percentage >= 0.17f)
                intensityStep = 2;
            else
                intensityStep = 1;
        }

    }
}
