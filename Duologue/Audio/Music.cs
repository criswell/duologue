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

    class Music
    {

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

        public Music()
        {
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

        public static void PlaySong(SongID ID)
        {
            AudioHelper.PlayCues(soundBankMap[ID], PlayType.Nonstop);
        }

        public static void UpdateSong(SongID ID)
        {
        }

        public static void StopSong(SongID ID)
        {
            AudioHelper.StopCues(soundBankMap[ID]);
        }
    }
}
