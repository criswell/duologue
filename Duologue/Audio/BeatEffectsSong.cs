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
    public class BeatEffectsSong : Song, IIntensitySong
    {
        private const int NUMBER_OF_TRACKS = 5;
        private int intensityStep = 1;

        public const string waveBank = "Content\\Audio\\Intensity.xwb";
        public const string soundBank = "Content\\Audio\\Intensity.xsb";

        public const string Intensity1 = "beat";
        public const string Intensity2 = "bass";
        public const string Intensity3 = "bassplus";
        public const string Intensity4 = "organ";
        public const string Intensity5 = "guitar";


        // This is the place where we keep the per intensity, per track volume targets
        static private Dictionary<int, Dictionary<string, float>> volumeMatrix =
            new Dictionary<int, Dictionary<string, float>>();

        public BeatEffectsSong() : base()
        {
            this.WaveBankName = waveBank;
            this.SoundBankName = soundBank;
            this.Tracks = new List<Track> {
                new Track(Intensity1, Loudness.Full),
                new Track(Intensity2, Loudness.Full),
                new Track(Intensity3, Loudness.Full),
                new Track(Intensity4, Loudness.Full),
                new Track(Intensity5, Loudness.Full)
            };

            List<string> cues = new List<string>();
            Tracks.ForEach(delegate(Track track) { cues.Add(track.CueName); });
            AudioHelper.AddBank(SoundBankName, WaveBankName, cues);

            this.intensityStep = 1;

            Dictionary<string, float> one = new Dictionary<string, float> {
                { Intensity1, Loudness.Full },
                { Intensity2, Loudness.Silent },
                { Intensity3, Loudness.Silent },
                { Intensity4, Loudness.Silent },
                { Intensity5, Loudness.Silent }
            };
            Dictionary<string, float> two = new Dictionary<string, float> {
                { Intensity1, Loudness.Full },
                { Intensity2, Loudness.Full },
                { Intensity3, Loudness.Silent },
                { Intensity4, Loudness.Silent },
                { Intensity5, Loudness.Silent }
            };
            Dictionary<string, float> three = new Dictionary<string, float> {
                { Intensity1, Loudness.Full },
                { Intensity2, Loudness.Full },
                { Intensity3, Loudness.Full },
                { Intensity4, Loudness.Silent },
                { Intensity5, Loudness.Silent }
            };
            Dictionary<string, float> four = new Dictionary<string, float> {
                { Intensity1, Loudness.Full },
                { Intensity2, Loudness.Full },
                { Intensity3, Loudness.Full },
                { Intensity4, Loudness.Full },
                { Intensity5, Loudness.Silent }
            };
            Dictionary<string, float> five = new Dictionary<string, float> {
                { Intensity1, Loudness.Full },
                { Intensity2, Loudness.Full },
                { Intensity3, Loudness.Full },
                { Intensity4, Loudness.Full },
                { Intensity5, Loudness.Full }
            };
            volumeMatrix.Add(1, one);
            volumeMatrix.Add(2, two);
            volumeMatrix.Add(3, three);
            volumeMatrix.Add(4, four);
            volumeMatrix.Add(5, five);
        }

        public override void Play()
        {
            AudioHelper.PlayCues(SoundBankName, PlayType.Nonstop);
            SetIntensity(0.0f);
        }

        public override void Stop()
        {
            AudioHelper.StopCues(SoundBankName);
        }

        public void SetIntensity(float intensity)
        {
            //fuckin' thing sucks. Do it live!
            if (intensity >= 1.0f)
                intensityStep = 5;
            else if (intensity >= 0.8f)
                intensityStep = 4;
            else if (intensity >= 0.6f)
                intensityStep = 3;
            else if (intensity >= 0.4f)
                intensityStep = 2;
            else
                intensityStep = 1;
            AudioHelper.UpdateCues(SoundBankName, volumeMatrix[intensityStep]);
        }

    }
}