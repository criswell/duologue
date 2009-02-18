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
        //These two arrays need to be kept "in sync"
        //That is, trackVolumes[n,tracknum] must always be relevant to cues[n]
        //cues[n] is the name of the nth cue
        //trackVolumes[n,0] is the volume of cues[n] at intensity (n+1) 0.
        //Song has a List<Track>, Track has Cuename, Volume
        protected float[,] trackVolumes;
        protected string[] cues;

        //like, Tracks = trackMap[intensityStep];
        protected Dictionary<int, List<Track>> trackMap = 
            new Dictionary<int, List<Track>>();


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
            string[] cues, float[,] vols)
            : base(game, sbname, wbname)
        {
            for (int intensity = 1; 
                intensity < vols.GetLength(0);
                intensity++)
            {
                trackMap[intensity] = new List<Track>();
                for (int cueNum = 0; cueNum < vols.GetLength(1); cueNum++)
                {
                    trackMap[intensity].Add
                        (new Track(cues[cueNum], vols[intensity, cueNum]));
                }
            }
            Tracks = trackMap[1];
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

        public void ChangeIntensity(int amount)
        {
            int oldIntensityStep = intensityStep;
            intensityStep =
                MWMathHelper.LimitToRange(intensityStep + amount, 1, trackMap.Count);
            if (intensityStep != oldIntensityStep || amount == 0)
            {
                Tracks = trackMap[intensityStep];
                AudioHelper.UpdateCues(this);
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
