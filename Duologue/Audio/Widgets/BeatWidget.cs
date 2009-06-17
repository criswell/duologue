using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Duologue.Audio.Widgets
{
    public class BeatWidget
    {
        public int NumberOfBeats;
        protected float lengthOfBeat;

        protected float beatTimer = 0f;
        protected int currentBeat = 0;
        protected Song parentSong;

        public BeatWidget(Song song, int beats, float beatLength)
        {
            parentSong = song;
            NumberOfBeats = beats;
            lengthOfBeat = beatLength;
        }

        public void Reset()
        {
            currentBeat = 0;
        }

        public double BeatPercentage()
        {
            return AudioConstants.MEDIAN_BEAT_SCORE + AudioConstants.BEAT_SCORE_DEV *
                Math.Cos(2d * Math.PI * (beatTimer / lengthOfBeat));
            //return 0.75d + 0.25d * Math.Cos(2d * Math.PI * (beatTimer / lengthOfBeat));
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
