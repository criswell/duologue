using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Mimicware;

namespace Duologue.Audio
{

    public class IntensityHelper
    {
        public int Step = 0;
        public int Max;
        public Dictionary<string, float>[] CueVolume; //must know array size before init

        public IntensityHelper(string[] cues, float[,] vols)
        {
            Max = vols.GetLength(0);
            CueVolume = new Dictionary<string, float>[Max];
            for (int i = 0; i < Max; i++)
            {
                CueVolume[i] = new Dictionary<string, float>();
                for (int cueNum = 0; cueNum < vols.GetLength(1); cueNum++)
                {
                    CueVolume[i].Add(cues[cueNum], vols[i, cueNum]);
                }
            }
        }
    }

    public class IntensitySong : Song
    {
        public IntensityHelper Intensity;

        public IntensitySong(Game game, string sbname, string wbname)
            : base(game, sbname, wbname)
        {
        }

        /*
        public IntensitySong(Game game, string sbname, string wbname,
            string[] cues, float[,] vols)
            : this(game, sbname, wbname)
        {
            for (int cueNum = 0; cueNum < vols.GetLength(1); cueNum++)
            {
                Tracks.Add(cues[cueNum], new Track(cues[cueNum], vols[0, cueNum]));
            }
            AudioHelper.Preload(this);
            Intensity = new IntensityHelper(cues, vols);
        }

        public IntensitySong(Game game, string sbname, string wbname,
            string[][] cues, float[,] vols)
            : this(game, sbname, wbname)
        {
            for (int track = 0; track < cues.Length; track++)
            {
                Tracks.Add(new Track(sbname, track));
            }
            AudioHelper.Preload(this);
            Intensity = new IntensityHelper(cues, vols);
        }
         */

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
            MWMathHelper.LimitToRange(intensity, 1, Intensity.Max);
            ChangeIntensity(intensity - Intensity.Step);
        }

        public void ChangeIntensity(int amount)
        {
            int oldIntensityStep = Intensity.Step;
            Intensity.Step =
                MWMathHelper.LimitToRange(Intensity.Step + amount, 1, Intensity.Max);
            //if ( ( (Intensity.Step != oldIntensityStep) || amount == 0 ) && !this.volumeChanging )
            if (!volumeChanging)
            {
                Tracks.ForEach(track =>
                {
                    track.ChangeVolume(Intensity.CueVolume[Intensity.Step - 1][""]);
                    //Tracks[name].ChangeVolume(Intensity.CueVolume[Intensity.Step - 1][name]);
                });
                ChangeVolume(false);
            }
        }

        public float GetIntensityPercentage()
        {
            return (float)Intensity.Step / (float)Intensity.Max;
        }

        public void SetIntensityPercentage(float percentage)
        {
            Intensity.Step = GetIntensityStepFromPercent(percentage);
        }

        public int GetIntensityStepFromPercent(float percent)
        {
            MWMathHelper.LimitToRange(percent, 0f, 1f);
            float segment = 1f / (float)Intensity.Max;
            float estimateStep = percent / segment;
            int step = (int)estimateStep;
            MWMathHelper.LimitToRange(step, 1, Intensity.Max);
            return step;
        }

    }
}
