using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Mimicware;

namespace Duologue.Audio
{
    /// <summary>
    /// 
    /// </summary>
    public class LandOfSandSong : Song, IIntensitySong
    {
        private const int MAX_INTENSITY = 6;
        private int intensityStep = 1;

        public const string wave = "Content\\Audio\\LandOfSand.xwb";
        public const string sound = "Content\\Audio\\LandOfSand.xsb";

        public const string BassDrum = "BassDrum";
        public const string HiHat = "HiHat";
        public const string BassSynth = "BassSynth";
        public const string Stabs = "Stabs";
        public const string Melody = "Melody";
        public const string Accent = "Accent";
        public const string Toms = "Toms";

        // This is the place where we keep the per intensity, per track volume targets
        static private Dictionary<int, Dictionary<string, float>> volumeMatrix =
            new Dictionary<int, Dictionary<string, float>>();

        public LandOfSandSong(Game game)
            : base(game, sound, wave)
        {
            this.Tracks = new List<Track> {
                new Track(BassDrum, Loudness.Full),
                new Track(HiHat, Loudness.Full),
                new Track(BassSynth, Loudness.Full),
                new Track(Stabs, Loudness.Full),
                new Track(Melody, Loudness.Full),
                new Track(Accent, Loudness.Full),
                new Track(Toms, Loudness.Full)
            };

            List<string> cues = new List<string>();
            Tracks.ForEach(delegate(Track track) { cues.Add(track.CueName); });
            AudioHelper.AddBank(SoundBankName, WaveBankName, cues);

            this.intensityStep = 1;

            Dictionary<string, float> one = new Dictionary<string, float> {
                { BassDrum, Loudness.Full },
                { HiHat, Loudness.Full },
                { BassSynth, Loudness.Silent },
                { Stabs, Loudness.Silent },
                { Melody, Loudness.Silent },
                { Accent, Loudness.Silent },
                { Toms, Loudness.Silent }
            };
            Dictionary<string, float> two = new Dictionary<string, float> {
                { BassDrum, Loudness.Full },
                { HiHat, Loudness.Full },
                { BassSynth, Loudness.Full },
                { Stabs, Loudness.Silent },
                { Melody, Loudness.Silent },
                { Accent, Loudness.Silent },
                { Toms, Loudness.Silent }
            };
            Dictionary<string, float> three = new Dictionary<string, float> {
                { BassDrum, Loudness.Full },
                { HiHat, Loudness.Full },
                { BassSynth, Loudness.Full },
                { Stabs, Loudness.Full },
                { Melody, Loudness.Silent },
                { Accent, Loudness.Silent },
                { Toms, Loudness.Silent }
            };
            Dictionary<string, float> four = new Dictionary<string, float> {
                { BassDrum, Loudness.Full },
                { HiHat, Loudness.Full },
                { BassSynth, Loudness.Silent },
                { Stabs, Loudness.Full },
                { Melody, Loudness.Full },
                { Accent, Loudness.Silent },
                { Toms, Loudness.Silent }
            };
            Dictionary<string, float> five = new Dictionary<string, float> {
                { BassDrum, Loudness.Full },
                { HiHat, Loudness.Full },
                { BassSynth, Loudness.Silent },
                { Stabs, Loudness.Full },
                { Melody, Loudness.Full },
                { Accent, Loudness.Full },
                { Toms, Loudness.Silent }
            };
            Dictionary<string, float> six = new Dictionary<string, float> {
                { BassDrum, Loudness.Full },
                { HiHat, Loudness.Silent },
                { BassSynth, Loudness.Silent },
                { Stabs, Loudness.Full },
                { Melody, Loudness.Full },
                { Accent, Loudness.Full },
                { Toms, Loudness.Full }
            };
            volumeMatrix.Add(1, one);
            volumeMatrix.Add(2, two);
            volumeMatrix.Add(3, three);
            volumeMatrix.Add(4, four);
            volumeMatrix.Add(5, five);
            volumeMatrix.Add(6, six);
        }

        public override void Play()
        {
            base.Play();
            //AudioHelper.PlayCues(SoundBankName, PlayType.Nonstop);
            AudioHelper.UpdateCues(SoundBankName, volumeMatrix[intensityStep]);
            //IsPlaying = true;
        }

        public void ChangeIntensity(int amount)
        {
            int oldIntensityStep = intensityStep;
            intensityStep =
                MWMathHelper.LimitToRange(intensityStep + amount, 1, MAX_INTENSITY);
            if (intensityStep != oldIntensityStep)
            {
                AudioHelper.UpdateCues(SoundBankName, volumeMatrix[intensityStep]);
            }
        }


        public float GetIntensityPercentage()
        {
            return (float)intensityStep / (float)MAX_INTENSITY;
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