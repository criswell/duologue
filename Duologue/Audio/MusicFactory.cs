using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

/// <summary>
/// Every song used in the game must have an instance initialized here.
/// Also a convenience class. Provides no-arg methods that invoke AudioHelper generics
/// </summary>
namespace Duologue.Audio
{

    public class MusicFactory
    {
        //Select Menu Song constants
        protected const string selectMenuWaves = "Content\\Audio\\SelectMenu.xwb";
        protected const string selectMenuSounds = "Content\\Audio\\SelectMenu.xsb";
        protected const string selectMenuCue = "nicStage_gso";

        //Beat Effects Song constants
        protected const string beatEffectsWaves = "Content\\Audio\\Intensity.xwb";
        protected const string beatEffectsSounds = "Content\\Audio\\Intensity.xsb";
        protected const string Intensity1 = "beat";
        protected const string Intensity2 = "bass";
        protected const string Intensity3 = "bassplus";
        protected const string Intensity4 = "organ";
        protected const string Intensity5 = "guitar";

        //Land of Sand Song constants
        protected const string landOfSandWaves = "Content\\Audio\\LandOfSand.xwb";
        protected const string landOfSandSounds = "Content\\Audio\\LandOfSand.xsb";
        protected const string LoSBassDrum = "BassDrum";
        protected const string LoSHiHat = "HiHat";
        protected const string LoSBassSynth = "BassSynth";
        protected const string LoSStabs = "Stabs";
        protected const string LoSMelody = "Melody";
        protected const string LoSAccent = "Accent";
        protected const string LoSToms = "Toms";

        //Dance8ths constants
        protected const string dance8thsWaves = "Content\\Audio\\Dance8ths.xwb";
        protected const string dance8thsSounds = "Content\\Audio\\Dance8ths.xsb";
        protected const string D8Bass8th = "bass8th";
        protected const string D8bassplus1 = "bassplus-1";
        protected const string D8bassplus2 = "bassplus-2";
        protected const string D8bassplus3 = "bassplus-3";
        protected const string D8bassplus4 = "bassplus-4";
        protected const string D8bassplus5 = "bassplus-5";
        protected const string D8bassplus6 = "bassplus-6";
        protected const string D8bassplus7 = "bassplus-7";
        protected const string D8bassplus8 = "bassplus-8";
        protected const string D8guitar1 = "guitar-1";
        protected const string D8guitar2 = "guitar-2";
        protected const string D8guitar3 = "guitar-3";
        protected const string D8guitar4 = "guitar-4";
        protected const string D8guitar5 = "guitar-5";
        protected const string D8guitar6 = "guitar-6";
        protected const string D8guitar7 = "guitar-7";
        protected const string D8guitar8 = "guitar-8";
        protected const string D8organ1 = "organ-1";
        protected const string D8organ2 = "organ-2";
        protected const string D8organ3 = "organ-3";
        protected const string D8organ4 = "organ-4";
        protected const string D8organ5 = "organ-5";
        protected const string D8organ6 = "organ-6";
        protected const string D8organ7 = "organ-7";
        protected const string D8organ8 = "organ-8";

        protected AudioManager notifier;
        protected static Dictionary<SongID, Song> songMap = new Dictionary<SongID, Song>();
        public Song SelectSong;
        public Song LandOfSand;
        public Song BeatEffects;
        public Song Dance8ths;

        public MusicFactory(AudioManager manager)
        {
            //general use
            notifier = manager;
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);

            //SelectSong
            string[] selectArr = { selectMenuCue };
            SelectSong = new Song(notifier.Game, selectMenuSounds, selectMenuWaves, selectArr);

            //BeatEffectsSong aka IntensitySong aka Dance
            float on = Loudness.Normal;
            float off = Loudness.Quiet;

            string[,] BEarr = { {Intensity1}, {Intensity2}, {Intensity3}, 
                                   {Intensity4}, {Intensity5} };

            float[,] BEvolumes = {
                                  {on, off, off, off, off},
                                  {on, on, off, off, off},
                                  {on, on, on, off, off},
                                  {on, on, on, on, off},
                                  {on, on, on, on, on}
                              };

            BeatEffects = new Song(notifier.Game, beatEffectsSounds, 
                beatEffectsWaves, BEarr, BEvolumes);

            //LandOfSand, the second IntensitySong
            string[,] LoStracks = {
                   {LoSBassDrum}, {LoSHiHat}, {LoSBassSynth}, {LoSStabs}, 
                   {LoSMelody}, {LoSAccent}, {LoSToms} };

            float[,] LoSvolumes = {
                                  {on, on, off, off, off, off, off},
                                  {on, on, on, off, off, off, off},
                                  {on, on, on, on, off, off, off},
                                  {on, on, off, on, on, off, off},
                                  {on, on, off, on, on, on, off},
                                  {on, off, off, on, on, on, on},
                              };

            LandOfSand = new Song(notifier.Game, landOfSandSounds,
                landOfSandWaves, LoStracks, LoSvolumes);

            string[,] D8arr = { 
                        {D8Bass8th, D8Bass8th, D8Bass8th, D8Bass8th, 
                            D8Bass8th, D8Bass8th, D8Bass8th, D8Bass8th},
                        {D8bassplus1, D8bassplus2, D8bassplus3, D8bassplus4, 
                            D8bassplus5, D8bassplus6, D8bassplus7, D8bassplus8},
                        {D8organ1, D8organ2, D8organ3, D8organ4, 
                            D8organ5, D8organ6, D8organ7, D8organ8},
                        {D8guitar1, D8guitar2, D8guitar3, D8guitar4, 
                            D8guitar5, D8guitar6, D8guitar7, D8guitar8}
                                };

            //Dance8ths, an attempt to slice each track of the BeatSong into 8 segments ("beats") that we
            //fire every time they play, instead of relying on the wav being
            //infinite repeat
            string[,] D8arrangement = {
                            {D8Bass8th, D8Bass8th, D8Bass8th, D8Bass8th, 
                                D8Bass8th, D8Bass8th, D8Bass8th, D8Bass8th},
                            {D8bassplus1, D8bassplus2, D8bassplus3, D8bassplus4, 
                                D8bassplus5, D8bassplus6, D8bassplus7, D8bassplus8},
                            {D8organ1, D8organ2, D8organ3, D8organ4, 
                                D8organ5, D8organ6, D8organ7, D8organ8},
                            {D8guitar1, D8guitar2, D8guitar3, D8guitar4, 
                                D8guitar5, D8guitar6, D8guitar7, D8guitar8}
                            };

            //this intensity volume map is per track, not per Q
            float[,] D8volumes = {
                                  {on, off, off, off, off},
                                  {on, on, off, off, off},
                                  {on, on, on, off, off},
                                  {on, on, on, on, off},
                                  {on, on, on, on, on}
                              };
            List<string> D8cues = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!D8cues.Contains(D8arrangement[i,j]))
                    {
                        D8cues.Add(D8arrangement[i,j]);
                    }
                }
            }
            Dance8ths = new Song(notifier.Game, dance8thsSounds, dance8thsWaves,
                D8arr, D8volumes);
            Dance8ths.IsBeatSong = true;

            //Addition of songs to Game Components
            manager.Game.Components.Add(SelectSong);
            manager.Game.Components.Add(BeatEffects);
            manager.Game.Components.Add(LandOfSand);
            manager.Game.Components.Add(Dance8ths);
        }

        public void UpdateIntensity(IntensityEventArgs e)
        {
            //LandOfSand.ChangeIntensity(e.ChangeAmount);
            //BeatEffects.ChangeIntensity(e.ChangeAmount);
            //Dance8ths.ChangeIntensity(e.ChangeAmount); FIXME
        }

        public void Detach()
        {
            notifier.Changed -= new IntensityEventHandler(UpdateIntensity);
            notifier = null;
        }

    }
}