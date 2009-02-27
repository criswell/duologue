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

        protected AudioManager notifier;
        protected static Dictionary<SongID, Song> songMap = new Dictionary<SongID, Song>();
        public Song SelectSong;
        public IntensitySong LandOfSand;
        public IntensitySong BeatEffects;
        protected BeatEngine beatEngine;

        public MusicFactory(AudioManager manager)
        {
            notifier = manager;
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);

            beatEngine = new BeatEngine(manager.Game);

            float on = Loudness.Normal;
            float off = Loudness.Quiet;

            SelectSong = new Song(notifier.Game, selectMenuSounds, selectMenuWaves,
              new List<string> { selectMenuCue });

            string[] BEcueOrder = { Intensity1, Intensity2, Intensity3, 
                                      Intensity4, Intensity5 };

            float[,] BEvolumes = {
                                  {on, off, off, off, off},
                                  {on, on, off, off, off},
                                  {on, on, on, off, off},
                                  {on, on, on, on, off},
                                  {on, on, on, on, on}
                              };

            BeatEffects = new IntensitySong(notifier.Game, beatEffectsSounds,
                beatEffectsWaves, BEcueOrder, BEvolumes);

            string[] cueOrder = {LoSBassDrum, LoSHiHat, LoSBassSynth, LoSStabs, 
                                  LoSMelody, LoSAccent, LoSToms};

            float[,] volumes = {
                                  {on, on, off, off, off, off, off},
                                  {on, on, on, off, off, off, off},
                                  {on, on, on, on, off, off, off},
                                  {on, on, off, on, on, off, off},
                                  {on, on, off, on, on, on, off},
                                  {on, off, off, on, on, on, on},
                              };
            LandOfSand = new IntensitySong(notifier.Game, landOfSandSounds,
                landOfSandWaves, cueOrder, volumes);

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