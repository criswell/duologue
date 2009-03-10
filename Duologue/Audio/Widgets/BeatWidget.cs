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
        public int NumberOfTracks;
        protected float beatTimer = 0f;
        protected float lengthOfBeat;
        protected int currentBeat = 0;

        public BeatWidget(int tracks, int beats)
        {
            NumberOfBeats = beats;
            NumberOfTracks = tracks;
            lengthOfBeat = (3433.039f / 8.000f);
        }

        public BeatWidget(int tracks, int beats, float beatLength)
            : this(tracks, beats)
        {
            lengthOfBeat = beatLength;
        }

        public void Update(GameTime gameTime, Song song)
        {
            if (currentBeat >= NumberOfBeats)
            {
                currentBeat = 0;
            }
            beatTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (beatTimer > lengthOfBeat)
            {
                currentBeat++;
                song.Tracks.ForEach(track => { track.PlayBeat(currentBeat); });
                beatTimer = 0f;
            }
        }

    }
}
