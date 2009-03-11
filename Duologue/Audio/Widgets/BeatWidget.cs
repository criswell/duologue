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
        protected Song parentSong;

        public BeatWidget(Song song, int tracks, int beats)
        {
            parentSong = song;
            NumberOfBeats = beats;
            NumberOfTracks = tracks;
            lengthOfBeat = (3433.039f / 8.000f);
        }

        public BeatWidget(Song song, int tracks, int beats, float beatLength)
            : this(song, tracks, beats)
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

                for (int t = 0; t < parentSong.TrackCount; t++)
                {
                    if (parentSong.Tracks[t].Enabled)
                    {
                        parentSong.Tracks[t].Cues[currentBeat-1].Play();
                    }
                }
                beatTimer = 0f;
            }
        }

    }
}
