using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Mimicware;

namespace Duologue.Audio
{
    //keep from having to tweak floats and add levels in many places
    public struct Loudness
    {
        public const float Silent = 0.0f;
        public const float Full = 999.0f;
    }


    /// <summary>
    /// 
    /// </summary>
    public class BeatEffectsSong : Song
    {
        private const int NUMBER_OF_TRACKS = 5;
        private AudioManager notifier;
        private int intensityStep = 1;

        // This is the place where we keep the per intensity, per track volume targets
        static private Dictionary<int, Dictionary<string, float>> volumeMatrix =
            new Dictionary<int, Dictionary<string, float>>();

        public BeatEffectsSong(AudioManager arg) : base()
        {
            notifier = arg;
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);

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

        public void UpdateIntensity(object sender, EventArgs e)
        {
            intensityStep = Convert.ToInt32((notifier.Intensity * 4.0f) + 0.5f);//FIXME - right int type?
            AudioHelper.UpdateCues(Music.IntensitySB, volumeMatrix[intensityStep]);
        }

        public void Detach()
        {
            notifier.Changed -= new IntensityEventHandler(UpdateIntensity);
            notifier = null;
        }


    }
}