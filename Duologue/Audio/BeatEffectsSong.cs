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

        // This is the place where we keep the per intensity, per track volume targets
        static private Dictionary<int, Dictionary<string, float>> volumeMatrix =
            new Dictionary<int, Dictionary<string, float>>();

        public BeatEffectsSong() : base()
        {
            this.intensityStep = 1;
            WaveBankName = Music.IntensityWB;
            SoundBankName = Music.IntensitySB;

            Dictionary<string, float> one = new Dictionary<string, float> {
                { Music.Intensity1, Loudness.Full },
                { Music.Intensity2, Loudness.Silent },
                { Music.Intensity3, Loudness.Silent },
                { Music.Intensity4, Loudness.Silent },
                { Music.Intensity5, Loudness.Silent }
            };
            Dictionary<string, float> two = new Dictionary<string, float> {
                { Music.Intensity1, Loudness.Full },
                { Music.Intensity2, Loudness.Full },
                { Music.Intensity3, Loudness.Silent },
                { Music.Intensity4, Loudness.Silent },
                { Music.Intensity5, Loudness.Silent }
            };
            Dictionary<string, float> three = new Dictionary<string, float> {
                { Music.Intensity1, Loudness.Full },
                { Music.Intensity2, Loudness.Full },
                { Music.Intensity3, Loudness.Full },
                { Music.Intensity4, Loudness.Silent },
                { Music.Intensity5, Loudness.Silent }
            };
            Dictionary<string, float> four = new Dictionary<string, float> {
                { Music.Intensity1, Loudness.Full },
                { Music.Intensity2, Loudness.Full },
                { Music.Intensity3, Loudness.Full },
                { Music.Intensity4, Loudness.Full },
                { Music.Intensity5, Loudness.Silent }
            };
            Dictionary<string, float> five = new Dictionary<string, float> {
                { Music.Intensity1, Loudness.Full },
                { Music.Intensity2, Loudness.Full },
                { Music.Intensity3, Loudness.Full },
                { Music.Intensity4, Loudness.Full },
                { Music.Intensity5, Loudness.Full }
            };
            volumeMatrix.Add(1, one);
            volumeMatrix.Add(2, two);
            volumeMatrix.Add(3, three);
            volumeMatrix.Add(4, four);
            volumeMatrix.Add(5, five);
        }

        public override void Play()
        {
            AudioHelper.PlayCues(Music.IntensitySB, PlayType.Nonstop);
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
            AudioHelper.UpdateCues(Music.IntensitySB, volumeMatrix[intensityStep]);
        }

    }
}