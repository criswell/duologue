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
    public enum SongID { SelectMenu, Intensity, Wiggles }
    //the rule is: one sound bank = one group of effects = one EffectsGroupID
    //public enum EffectsGroupID { Player }

    //keep from having to tweak floats and add levels in many places
    public struct Loudness
    {
        public const float Silent = 0.0f;
        public const float Full = 999.0f;
    }


    public class Music
    {
        private AudioManager notifier;

        public const string SelectMenuWB = "Content\\Audio\\SelectMenu.xwb";
        public const string SelectMenuSB = "Content\\Audio\\SelectMenu.xsb";
        public const string SelectMenuCue = "nicStage_gso";

        public const string IntensityWB = "Content\\Audio\\Intensity.xwb";
        public const string IntensitySB = "Content\\Audio\\Intensity.xsb";
        public const string Intensity1 = "beat";
        public const string Intensity2 = "bass";
        public const string Intensity3 = "bassplus";
        public const string Intensity4 = "organ";
        public const string Intensity5 = "guitar";

        public const string WigglesSoundBank = "You gonna get an exception!";

        private static Dictionary<SongID, string> soundBankMap = new Dictionary<SongID, string>();

        private static Dictionary<SongID, Song> songMap = new Dictionary<SongID, Song>();

        private BeatEffectsSong beatSong;
        private SelectMenuSong selectSong;

        public Music(AudioManager arg)
        {
            notifier = arg;
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);

            beatSong = new BeatEffectsSong();
            selectSong = new SelectMenuSong();

            songMap.Add(SongID.Intensity, beatSong);
            songMap.Add(SongID.SelectMenu, selectSong);

            List<string> selectMenuCues = new List<string> 
            {
                SelectMenuCue
            };
            AudioHelper.AddBank(SelectMenuSB, SelectMenuWB, selectMenuCues);

            List<string> intensityCues = new List<string>
            {
                Intensity1, Intensity2, Intensity3, Intensity4, Intensity5
            };
            AudioHelper.AddBank(IntensitySB, IntensityWB, intensityCues);

            soundBankMap.Add(SongID.Intensity, IntensitySB);
            soundBankMap.Add(SongID.SelectMenu, SelectMenuSB);
            soundBankMap.Add(SongID.Wiggles, WigglesSoundBank);

            soundBankMap = new Dictionary<SongID, string> {
                {SongID.SelectMenu, SelectMenuSB}, 
                {SongID.Intensity, IntensitySB}, 
                {SongID.Wiggles, WigglesSoundBank}
            };
        }

        public void PlaySong(SongID ID)
        {
            songMap[ID].Play();
        }

        public void UpdateSong(SongID ID)
        {
        }

        public void StopSong(SongID ID)
        {
            songMap[ID].Stop();
        }

        public void UpdateIntensity(object sender, EventArgs e)
        {
            beatSong.SetIntensity(notifier.Intensity);
            //walk through a list of playing intensity songs and notify them!
        }

        public void Detach()
        {
            notifier.Changed -= new IntensityEventHandler(UpdateIntensity);
            notifier = null;
        }

    }
}