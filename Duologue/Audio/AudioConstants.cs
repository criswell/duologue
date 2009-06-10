using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio
{

    //Keep from having to tweak floats and add levels in many places
    //Stupidly, these values are from 0f to 100f.
    //I swear to god, I ended up with that from following an MS tutorial for an XACT-based project
    //In fact, I've never tried creating a map of 0 to 1 in XACT.
    public struct VolumePresets
    {
        public const float Silent = 0f;
        public const float Quiet = 40f;
        public const float Normal = 80f;
        public const float Full = 100f;

        public const float SelectMenu = 82f;
        public const float Credits = 80f;
        public const float Tr8or = 79f; //Medals Display

    }

    class AudioConstants
    {
        //this is the proportional volume control for *all* in-game music (hell, maybe even out of game)
        public const float MusicCategoryDefaultVolume = 0.5f;
        //these are the limits that AudioHelper uses to clamp the above volume. Change if you really need to.
        //It's supposed to be, I think, 1.0f = 6db
        public const float MIN_MUSIC_CAT_VOL = 0f;
        public const float MAX_MUSIC_CAT_VOL = 2f;

        //XACT stuff - you can't change these unless you actually change the XACT project!
        public const float MIN_XACT_VOL = 0f;
        public const float MAX_XACT_VOL = 100f;

        //This is the number of milliseconds between volume changes in a "fade".
        //Smaller numbers tax the CPU more, larger numbers may make it choppy
        public const int VOL_CHANGE_UPDATE_MS = 5;

        //Default values for volume changes in milliseconds
        public const int FADE_IN_TIME = 100;
        public const int FADE_OUT_TIME = 100;

        //More volume change constants
        public const int MIN_VOL_CHANGE_MS = 1;
        public const int MAX_VOL_CHANGE_MS = 5000;
        public const int MIN_VOL_CHANGE_STEPS = 1;
        public const int MAX_VOL_CHANGE_STEPS = 1000;

        //length of silence to avoid the big loud blast before fading in
        public const int PREFADE_MUTE_MS = 10;


        //constants for control of scoring on "close to the beat"
        public const double MAX_BEAT_SCORE = 1d; //change this one
        public const double MIN_BEAT_SCORE = 0.5d; //and this one
        //and these will update for the calculating function
        public const double MEDIAN_BEAT_SCORE = (MAX_BEAT_SCORE + MIN_BEAT_SCORE) / 2;
        public const double BEAT_SCORE_DEV = MAX_BEAT_SCORE - MEDIAN_BEAT_SCORE;

        //constants for milliseconds per beat
        public const float BPM120 = 1000f * 60f / 120f;
        public const float BPM140 = (3433.039f / 8.000f);
        //got 3433.039 by visual measure of one song, seems to be working
        public const float BPM170 = 1000f * 60f / 170f;

        public const int WTF = 5000;

    }
}
