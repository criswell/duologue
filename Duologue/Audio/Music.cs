using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

/// <summary>
/// Every song used in the game must have its assets identified and initialized here.
/// Also a convenience class. Provides no-arg methods that invoke AudioManager generics
/// </summary>
namespace Duologue.Audio
{
    //the rule is: one sound bank = one song = one SongID
    public enum SongID { Intro, Buzzsaw, Wiggles }

    class Music
    {

        public const string introWaveBankName = "Content\\Audio\\OneFileMusic.xwb";
        public const string introSoundBankName = "Content\\Audio\\OneFileMusic.xsb";
        public const string introCue = "nicStage_gso";

        public const string buzzsawWaveBank = "Content\\Audio\\Flick3r_Dance.xwb";
        public const string buzzsawSoundBank = "Content\\Audio\\Flick3r_Dance.xsb";
        public const string beatName = "beat";
        public const string bassName = "bass";
        public const string bassplusName = "bassplus";
        public const string organName = "organ";
        public const string guitarName = "guitar";

        private static bool OMG_RU_REDY_YET = false;

        public Music()
        {
            if (!OMG_RU_REDY_YET)
            {
                init();
            }
        }

        private static void init()
        {
            Song introSong = new Song();
            introSong.SoundBankName = introSoundBankName;
            introSong.WaveBankName = introWaveBankName;
            introSong.Tracks = new List<Track> { { new Track(introCue, 1.0f) } };
            AudioManager.AddSong(SongID.Intro, introSong);

            Song buzzsawSong = new Song();
            buzzsawSong.SoundBankName = buzzsawSoundBank;
            buzzsawSong.WaveBankName = buzzsawWaveBank;
            Track track1 = new Track(beatName, 1.0f);
            Track track2 = new Track(bassName, 0.0f);
            Track track3 = new Track(bassplusName, 0.0f);
            Track track4 = new Track(organName, 0.0f);
            Track track5 = new Track(guitarName, 0.0f);
            buzzsawSong.Tracks = new List<Track> { 
            { track1 }, { track2 }, { track3 }, { track4 }, { track5 } };
            AudioManager.AddSong(SongID.Buzzsaw, buzzsawSong);

            OMG_RU_REDY_YET = true;
        }

        public static void PlaySong(SongID ID)
        {
            if (!OMG_RU_REDY_YET)
            {
                init();
            }
            AudioManager.PlaySong(ID);
        }

        public static void StopSong(SongID ID)
        {
            if (!OMG_RU_REDY_YET)
            {
                init();
            }
            AudioManager.StopSong(ID);
        }

        public static void LoadAudio(Game param_game)
        {
            if (!OMG_RU_REDY_YET)
            {
                init();
            }
        }
    }
}
