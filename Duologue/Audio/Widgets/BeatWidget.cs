using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Duologue.Audio.Widgets
{
    public class BeatWidget
    {
        //constants for milliseconds per beat
        public const float BPM120 = 1000f * 60f/120f;
        public const float BPM140 = (3433.039f / 8.000f);
        //got 3433.039 by visual measure of one song, seems to be working
        public const float BPM170 = 1000f * 60f/170f;
        public int NumberOfBeats;
        public float lengthOfBeat;

        protected float beatTimer = 0f;
        protected int currentBeat = 0;
        protected Song parentSong;

        public BeatWidget(Song song, int beats)
        {
            parentSong = song;
            NumberOfBeats = beats;
            lengthOfBeat = (3433.039f / 8.000f);
            //FIXME: This only matches 140BPM
        }

        public BeatWidget(Song song, int beats, float beatLength)
            : this(song, beats)
        {
            lengthOfBeat = beatLength;
        }

        public double BeatPercentage()
        {
            return 0.75d + 0.25d * Math.Cos(2d * Math.PI * (beatTimer / lengthOfBeat));
        }

        public void Update(GameTime gameTime, Song song)
        {
            beatTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (beatTimer > lengthOfBeat)
            {
                if (currentBeat >= NumberOfBeats)
                {
                    currentBeat = 0;
                }
                currentBeat++;
                for (int t = 0; t < parentSong.TrackCount; t++)
                {
                    if (parentSong.Tracks[t].Enabled)
                    {
                        AudioHelper.Play(parentSong.Tracks[t].Cues[currentBeat - 1], false);
                    }
                }
                beatTimer = 0f;
            }
        }
    }
}
