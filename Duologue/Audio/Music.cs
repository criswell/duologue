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

        private const string selectMenuWaves = "Content\\Audio\\SelectMenu.xwb";
        private const string selectMenuSounds = "Content\\Audio\\SelectMenu.xsb";
        private const string selectMenuCue = "nicStage_gso";

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
        //private BeatEffectsSong beatSong;
        //private LandOfSandSong landOfSandSong;
        public Song SelectSong;
        public IntensitySong LandOfSand;
        private BeatEngine beatEngine;

        public Music(AudioManager manager)
        {
            notifier = manager;
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);

            //beatSong = new BeatEffectsSong();
            //landOfSandSong = new LandOfSandSong();
            beatEngine = new BeatEngine(manager.Game);

            BuildSelectSong();
            BuildLandOfSand();

            manager.Game.Components.Add(SelectSong);
            manager.Game.Components.Add(LandOfSand);
        }

        private void BuildSelectSong()
        {
            SelectSong = new Song(notifier.Game, selectMenuSounds, selectMenuWaves,
              new List<string> { selectMenuCue });
        }

        private void BuildLandOfSand()
        {
            List<string> cues = new List<string> {LoSBassDrum, LoSHiHat,
                LoSBassSynth, LoSStabs, LoSMelody, LoSAccent, LoSToms};

            Track bassOn = new Track(LoSBassDrum, Loudness.Full);
            Track bassOff = new Track(LoSBassDrum, Loudness.Silent);
            Track hatOn = new Track(LoSHiHat, Loudness.Full);
            Track hatOff = new Track(LoSHiHat, Loudness.Silent);
            Track bsOn = new Track(LoSBassSynth, Loudness.Full);
            Track bsOff = new Track(LoSBassSynth, Loudness.Silent);
            Track stabsOn = new Track(LoSStabs, Loudness.Full);
            Track stabsOff = new Track(LoSStabs, Loudness.Silent);
            Track melodyOn = new Track(LoSMelody, Loudness.Full);
            Track melodyOff = new Track(LoSMelody, Loudness.Silent);
            Track accentOn = new Track(LoSAccent, Loudness.Full);
            Track accentOff = new Track(LoSAccent, Loudness.Silent);
            Track tomsOn = new Track(LoSToms, Loudness.Full);
            Track tomsOff = new Track(LoSToms, Loudness.Silent);

            List<Track> one = new List<Track> { bassOn, hatOn, bsOff, stabsOff,
                melodyOff, accentOff, tomsOff };

            List<Track> two = new List<Track> { bassOn, hatOn, bsOn, stabsOff,
                melodyOff, accentOff, tomsOff };

            List<Track> three = new List<Track> { bassOn, hatOn, bsOn, stabsOn,
                melodyOff, accentOff, tomsOff };

            List<Track> four = new List<Track> { bassOn, hatOn, bsOff, stabsOn,
                melodyOn, accentOff, tomsOff };

            List<Track> five = new List<Track> { bassOn, hatOn, bsOff, stabsOn,
                melodyOn, accentOn, tomsOff };

            List<Track> six = new List<Track> { bassOn, hatOff, bsOff, stabsOn,
                melodyOn, accentOn, tomsOn };

            float on = Loudness.Full;
            float off = Loudness.Silent;
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
                landOfSandWaves, cues, volumes);

        }

        public void UpdateIntensity(IntensityEventArgs e)
        {
            //beatSong.ChangeIntensity(e.ChangeAmount);
            //landOfSandSong.ChangeIntensity(e.ChangeAmount);
        }

        public void Detach()
        {
            notifier.Changed -= new IntensityEventHandler(UpdateIntensity);
            notifier = null;
        }

    }
}