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

    public class Music
    {
        //Select Menu Song constants
        private const string selectMenuWaves = "Content\\Audio\\SelectMenu.xwb";
        private const string selectMenuSounds = "Content\\Audio\\SelectMenu.xsb";
        private const string selectMenuCue = "nicStage_gso";

        //Beat Effects Song constants
        private const string beatEffectsWaves = "Content\\Audio\\Intensity.xwb";
        private const string beatEffectsSounds = "Content\\Audio\\Intensity.xsb";
        private const string Intensity1 = "beat";
        private const string Intensity2 = "bass";
        private const string Intensity3 = "bassplus";
        private const string Intensity4 = "organ";
        private const string Intensity5 = "guitar";

        //Land of Sand Song constants
        private const string landOfSandWaves = "Content\\Audio\\LandOfSand.xwb";
        private const string landOfSandSounds = "Content\\Audio\\LandOfSand.xsb";
        private const string LoSBassDrum = "BassDrum";
        private const string LoSHiHat = "HiHat";
        private const string LoSBassSynth = "BassSynth";
        private const string LoSStabs = "Stabs";
        private const string LoSMelody = "Melody";
        private const string LoSAccent = "Accent";
        private const string LoSToms = "Toms";

        private AudioManager notifier;
        private static Dictionary<SongID, Song> songMap = new Dictionary<SongID, Song>();
        public Song SelectSong;
        public IntensitySong LandOfSand;
        public IntensitySong BeatEffects;
        private BeatEngine beatEngine;

        public Music(AudioManager manager)
        {
            notifier = manager;
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);

            beatEngine = new BeatEngine(manager.Game);

            float on = Loudness.Normal;
            float off = Loudness.Silent;

            SelectSong = new Song(notifier.Game, selectMenuSounds, selectMenuWaves,
              new List<string> { selectMenuCue });

            List<string> BECues = new List<string> {Intensity1, Intensity2,
                Intensity3, Intensity4, Intensity5 };
            float[,] BEvolumes = {
                                  {on, off, off, off, off},
                                  {on, on, off, off, off},
                                  {on, on, on, off, off},
                                  {on, on, on, on, off},
                                  {on, on, on, on, on}
                              };

            string[] BEcueOrder = { Intensity1, Intensity2, Intensity3, 
                                      Intensity4, Intensity5 };

            BeatEffects = new IntensitySong(notifier.Game, beatEffectsSounds,
                beatEffectsWaves, BECues, BEvolumes);


            List<string> LoSCues = new List<string> {LoSBassDrum, LoSHiHat,
                LoSBassSynth, LoSStabs, LoSMelody, LoSAccent, LoSToms};

            float[,] volumes = {
                                  {on, on, off, off, off, off, off},
                                  {on, on, on, off, off, off, off},
                                  {on, on, on, on, off, off, off},
                                  {on, on, off, on, on, off, off},
                                  {on, on, off, on, on, on, off},
                                  {on, off, off, on, on, on, on},
                              };
            string[] cueOrder = {LoSBassDrum, LoSHiHat, LoSBassSynth, LoSStabs, 
                                  LoSMelody, LoSAccent, LoSToms};

            LandOfSand = new IntensitySong(notifier.Game, landOfSandSounds,
                landOfSandWaves, LoSCues, volumes);

            manager.Game.Components.Add(SelectSong);
            manager.Game.Components.Add(BeatEffects);
            manager.Game.Components.Add(LandOfSand);
        }

        public void UpdateIntensity(IntensityEventArgs e)
        {
            LandOfSand.ChangeIntensity(e.ChangeAmount);
            BeatEffects.ChangeIntensity(e.ChangeAmount);
        }

        public void Detach()
        {
            notifier.Changed -= new IntensityEventHandler(UpdateIntensity);
            notifier = null;
        }

    }
}