using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Mimicware;

namespace Duologue.Audio
{
    public interface IIntensitySong : ISong
    {
        void ChangeIntensity(int amount);
        void SetIntensityPercentage(float percentage);
        float GetIntensityPercentage();
    }

    public class IntensitySong : Song, IIntensitySong
    {
        private int intensityStep = 0; //trick into an update the first time

        // This is the place where we keep the per intensity, per track volume targets
        //              (intensity)      (Cue name) (Volume)
        private Dictionary<int, List<Track>> trackMap = 
            new Dictionary<int, List<Track>>();

        private float[,] trackVolumes;
        private string[] cues;

        public IntensitySong(Game game, string sbname, string wbname)
            : base(game, sbname, wbname)
        {
        }

        public IntensitySong(Game game, string sbname, string wbname, List<string> cueList)
            : base(game, sbname, wbname, cueList)
        {
            int i = 0;
            cues = new string[cueList.Count];
            cueList.ForEach(cue =>
                {
                    this.cues[i] = cue;
                    i++;
                });
        }

        public IntensitySong(Game game, string sbname, string wbname, 
            List<string> cues, float[,] vols)
            : this(game, sbname, wbname, cues)
        {
            trackVolumes = vols;
        }

        public override void Play()
        {
            base.Play();
            ChangeIntensity(0);
        }

        public void ChangeIntensity(int amount)
        {
            int oldIntensityStep = intensityStep;
            intensityStep =
                MWMathHelper.LimitToRange(intensityStep + amount, 1, trackVolumes.GetLength(0));
            if (intensityStep != oldIntensityStep || amount == 0)
            {
                float[] vols = new float[trackVolumes.GetLength(1)];
                for (int i = 0; i < trackVolumes.GetLength(0); i++)
                {
                    vols[i] = trackVolumes[intensityStep - 1, i];
                }

                AudioHelper.UpdateCues(SoundBankName, cues, vols);
            }
        }


        public float GetIntensityPercentage()
        {
            return (float)intensityStep / (float)trackVolumes.GetLength(0);
        }

        public void SetIntensityPercentage(float percentage)
        {
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
