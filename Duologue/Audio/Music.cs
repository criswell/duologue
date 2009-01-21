using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

/// <summary>
/// Every song used in the game must have its assets identified and initialized here.
/// Also a convenience class. Provides no-arg methods that invoke AudioHelper generics
/// </summary>
namespace Duologue.Audio
{
    //the rule is: one sound bank = one song = one SongID
    public enum SongID { Intro, First, Wiggles }
    //the rule is: one sound bank = one group of effects = one EffectsGroupID
    //public enum EffectsGroupID { Player }

    class Music
    {

        public const string IntroWaveBank = "Content\\Audio\\OneFileMusic.xwb";
        public const string IntroSoundBank = "Content\\Audio\\OneFileMusic.xsb";
        public const string IntroCue = "nicStage_gso";

        public const string FirstWaveBank = "Content\\Audio\\Flick3r_Dance.xwb";
        public const string FirstSoundBank = "Content\\Audio\\Flick3r_Dance.xsb";
        public const string First1 = "beat";
        public const string First2 = "bass";
        public const string First3 = "bassplus";
        public const string First4 = "organ";
        public const string First5 = "guitar";

        public const string WigglesSoundBank = "You gonna get an exception!";

        private static Dictionary<SongID, string> soundBankMap = new Dictionary<SongID, string>();

        public Music()
        {
        }

        public static void init(Game param_game)
        {
            List<string> introCues = new List<string> 
            {
                IntroCue
            };
            AudioHelper.AddBank(IntroSoundBank, IntroWaveBank, introCues);

            List<string> firstCues = new List<string>
            {
                First1, First2, First3, First4, First5
            };
            AudioHelper.AddBank(FirstSoundBank, FirstWaveBank, firstCues);

            soundBankMap.Add(SongID.First, FirstSoundBank);
            soundBankMap.Add(SongID.Intro, IntroSoundBank);
            soundBankMap.Add(SongID.Wiggles, WigglesSoundBank);

            soundBankMap = new Dictionary<SongID, string> {
                {SongID.Intro, IntroSoundBank}, 
                {SongID.First, FirstSoundBank}, 
                {SongID.Wiggles, WigglesSoundBank}
            };
            /*
             * hmmmmmmmm
            Song introSong = new Song();
            introSong.SoundBankName = introSoundBankName;
            introSong.WaveBankName = introWaveBankName;
            introSong.Tracks = new List<Track> { { new Track(introCue, 1.0f) } };
            AudioHelper.AddSong(SongID.Intro, introSong);

            Song buzzsawSong = new Song();
            buzzsawSong.SoundBankName = buzzsawSoundBank;
            buzzsawSong.WaveBankName = buzzsawWaveBank;
            Track track1 = new Track(beatName, 999.0f);
            Track track2 = new Track(bassName, 0.0f);
            Track track3 = new Track(bassplusName, 0.0f);
            Track track4 = new Track(organName, 0.0f);
            Track track5 = new Track(guitarName, 0.0f);
            buzzsawSong.Tracks = new List<Track> { 
            { track1 }, { track2 }, { track3 }, { track4 }, { track5 } };
             */
        }

        public static void PlaySong(SongID ID)
        {
            AudioHelper.PlayCues(soundBankMap[ID], PlayType.Nonstop);
        }

        public static void StopSong(SongID ID)
        {
            AudioHelper.StopCues(soundBankMap[ID]);
        }
    }
}
