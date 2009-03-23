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

        //Credits Song constants
        protected const string creditsWaves = "Content\\Audio\\CreditsMenu.xwb";
        protected const string creditsSounds = "Content\\Audio\\CreditsMenu.xsb";
        protected const string creditsCue = "credits";

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

        //Land of Sand Song 16ths constants
        protected const string LoS16Waves = "Content\\Audio\\LandOfSand16ths.xwb";
        protected const string LoS16Sounds = "Content\\Audio\\LandOfSand16ths.xsb";
        protected const string LoSBD01 = "LoSBassDrum-01";
        protected const string LoSBD02 = "LoSBassDrum-02";
        protected const string LoSBD03 = "LoSBassDrum-03";
        protected const string LoSBD04 = "LoSBassDrum-04";
        protected const string LoSBD05 = "LoSBassDrum-05";
        protected const string LoSBD06 = "LoSBassDrum-06";
        protected const string LoSBD07 = "LoSBassDrum-07";
        protected const string LoSBD08 = "LoSBassDrum-08";
        protected const string LoSBD09 = "LoSBassDrum-09";
        protected const string LoSBD10 = "LoSBassDrum-10";
        protected const string LoSBD11 = "LoSBassDrum-11";
        protected const string LoSBD12 = "LoSBassDrum-12";
        protected const string LoSBD13 = "LoSBassDrum-13";
        protected const string LoSBD14 = "LoSBassDrum-14";
        protected const string LoSBD15 = "LoSBassDrum-15";
        protected const string LoSBD16 = "LoSBassDrum-16";
        protected const string LoSHH01 = "LoSHiHat-01";
        protected const string LoSHH02 = "LoSHiHat-02";
        protected const string LoSHH03 = "LoSHiHat-03";
        protected const string LoSHH04 = "LoSHiHat-04";
        protected const string LoSHH05 = "LoSHiHat-05";
        protected const string LoSHH06 = "LoSHiHat-06";
        protected const string LoSHH07 = "LoSHiHat-07";
        protected const string LoSHH08 = "LoSHiHat-08";
        protected const string LoSHH09 = "LoSHiHat-09";
        protected const string LoSHH10 = "LoSHiHat-10";
        protected const string LoSHH11 = "LoSHiHat-11";
        protected const string LoSHH12 = "LoSHiHat-12";
        protected const string LoSHH13 = "LoSHiHat-13";
        protected const string LoSHH14 = "LoSHiHat-14";
        protected const string LoSHH15 = "LoSHiHat-15";
        protected const string LoSHH16 = "LoSHiHat-16";
        protected const string LoSBS01 = "LoSBassSynth-01";
        protected const string LoSBS02 = "LoSBassSynth-02";
        protected const string LoSBS03 = "LoSBassSynth-03";
        protected const string LoSBS04 = "LoSBassSynth-04";
        protected const string LoSBS05 = "LoSBassSynth-05";
        protected const string LoSBS06 = "LoSBassSynth-06";
        protected const string LoSBS07 = "LoSBassSynth-07";
        protected const string LoSBS08 = "LoSBassSynth-08";
        protected const string LoSBS09 = "LoSBassSynth-09";
        protected const string LoSBS10 = "LoSBassSynth-10";
        protected const string LoSBS11 = "LoSBassSynth-11";
        protected const string LoSBS12 = "LoSBassSynth-12";
        protected const string LoSBS13 = "LoSBassSynth-13";
        protected const string LoSBS14 = "LoSBassSynth-14";
        protected const string LoSBS15 = "LoSBassSynth-15";
        protected const string LoSBS16 = "LoSBassSynth-16";
        protected const string LoSSt01 = "LoSStabs-01";
        protected const string LoSSt02 = "LoSStabs-02";
        protected const string LoSSt03 = "LoSStabs-03";
        protected const string LoSSt04 = "LoSStabs-04";
        protected const string LoSSt05 = "LoSStabs-05";
        protected const string LoSSt06 = "LoSStabs-06";
        protected const string LoSSt07 = "LoSStabs-07";
        protected const string LoSSt08 = "LoSStabs-08";
        protected const string LoSSt09 = "LoSStabs-09";
        protected const string LoSSt10 = "LoSStabs-10";
        protected const string LoSSt11 = "LoSStabs-11";
        protected const string LoSSt12 = "LoSStabs-12";
        protected const string LoSSt13 = "LoSStabs-13";
        protected const string LoSSt14 = "LoSStabs-14";
        protected const string LoSSt15 = "LoSStabs-15";
        protected const string LoSSt16 = "LoSStabs-16";
        protected const string LoSMel01 = "LoSMelody-01";
        protected const string LoSMel02 = "LoSMelody-02";
        protected const string LoSMel03 = "LoSMelody-03";
        protected const string LoSMel04 = "LoSMelody-04";
        protected const string LoSMel05 = "LoSMelody-05";
        protected const string LoSMel06 = "LoSMelody-06";
        protected const string LoSMel07 = "LoSMelody-07";
        protected const string LoSMel08 = "LoSMelody-08";
        protected const string LoSMel09 = "LoSMelody-09";
        protected const string LoSMel10 = "LoSMelody-10";
        protected const string LoSMel11 = "LoSMelody-11";
        protected const string LoSMel12 = "LoSMelody-12";
        protected const string LoSMel13 = "LoSMelody-13";
        protected const string LoSMel14 = "LoSMelody-14";
        protected const string LoSMel15 = "LoSMelody-15";
        protected const string LoSMel16 = "LoSMelody-16";
        protected const string LoSAcc01 = "LoSAccent-01";
        protected const string LoSAcc02 = "LoSAccent-02";
        protected const string LoSAcc03 = "LoSAccent-03";
        protected const string LoSAcc04 = "LoSAccent-04";
        protected const string LoSAcc05 = "LoSAccent-05";
        protected const string LoSAcc06 = "LoSAccent-06";
        protected const string LoSAcc07 = "LoSAccent-07";
        protected const string LoSAcc08 = "LoSAccent-08";
        protected const string LoSAcc09 = "LoSAccent-09";
        protected const string LoSAcc10 = "LoSAccent-10";
        protected const string LoSAcc11 = "LoSAccent-11";
        protected const string LoSAcc12 = "LoSAccent-12";
        protected const string LoSAcc13 = "LoSAccent-13";
        protected const string LoSAcc14 = "LoSAccent-14";
        protected const string LoSAcc15 = "LoSAccent-15";
        protected const string LoSAcc16 = "LoSAccent-16";
        protected const string LoST01 = "LoSToms-01";
        protected const string LoST02 = "LoSToms-02";
        protected const string LoST03 = "LoSToms-03";
        protected const string LoST04 = "LoSToms-04";
        protected const string LoST05 = "LoSToms-05";
        protected const string LoST06 = "LoSToms-06";
        protected const string LoST07 = "LoSToms-07";
        protected const string LoST08 = "LoSToms-08";
        protected const string LoST09 = "LoSToms-09";
        protected const string LoST10 = "LoSToms-10";
        protected const string LoST11 = "LoSToms-11";
        protected const string LoST12 = "LoSToms-12";
        protected const string LoST13 = "LoSToms-13";
        protected const string LoST14 = "LoSToms-14";
        protected const string LoST15 = "LoSToms-15";
        protected const string LoST16 = "LoSToms-16";

        //ultrafix song constants
        protected const string UFWaves = "Content\\Audio\\ultrafix.xwb";
        protected const string UFSounds = "Content\\Audio\\ultrafix.xsb";
        protected const string UFB1 = "bass_line-1";
        protected const string UFB2 = "bass_line-2";
        protected const string UFB3 = "bass_line-3";
        protected const string UFB4 = "bass_line-4";
        protected const string UFB5 = "bass_line-5";
        protected const string UFB6 = "bass_line-6";
        protected const string UFB7 = "bass_line-7";
        protected const string UFB8 = "bass_line-8";
        protected const string UFD1 = "drum-1";
        protected const string UFD2 = "drum-2";
        protected const string UFD3 = "drum-3";
        protected const string UFD4 = "drum-4";
        protected const string UFD5 = "drum-5";
        protected const string UFD6 = "drum-6";
        protected const string UFD7 = "drum-7";
        protected const string UFD8 = "drum-8";
        protected const string UFFX1 = "fx-1";
        protected const string UFFX2 = "fx-2";
        protected const string UFFX3 = "fx-3";
        protected const string UFFX4 = "fx-4";
        protected const string UFFX5 = "fx-5";
        protected const string UFFX6 = "fx-6";
        protected const string UFFX7 = "fx-7";
        protected const string UFFX8 = "fx-8";
        protected const string UFS1 = "synth-1";
        protected const string UFS2 = "synth-2";
        protected const string UFS3 = "synth-3";
        protected const string UFS4 = "synth-4";
        protected const string UFS5 = "synth-5";
        protected const string UFS6 = "synth-6";
        protected const string UFS7 = "synth-7";
        protected const string UFS8 = "synth-8";

        protected static Dictionary<SongID, Song> songMap = new Dictionary<SongID, Song>();
        public Song SelectSong;
        public Song Dance8ths;
        public Song LandOfSand16ths;
        public Song Credits;
        public Song Ultrafix;

        public MusicFactory(AudioManager manager)
        {
            bool on = true;
            bool off = false;

            //SelectSong
            string[] selectArr = { selectMenuCue };
            SelectSong = new Song(manager.Game, selectMenuSounds, selectMenuWaves, selectArr);

            //CreditsSong
            string[] creditsArr = { creditsCue };
            Credits = new Song(manager.Game, creditsSounds, creditsWaves, creditsArr);

            //Dance8ths
            string[,] D8arr = { 
                {D8Bass8th, D8Bass8th, D8Bass8th, D8Bass8th, D8Bass8th, D8Bass8th, D8Bass8th, D8Bass8th},
                {D8bassplus1, D8bassplus2, D8bassplus3, D8bassplus4, D8bassplus5, D8bassplus6, D8bassplus7, D8bassplus8},
                {D8organ1, D8organ2, D8organ3, D8organ4, D8organ5, D8organ6, D8organ7, D8organ8},
                {D8guitar1, D8guitar2, D8guitar3, D8guitar4, D8guitar5, D8guitar6, D8guitar7, D8guitar8}
                        };

            //this intensity volume map is per track, not per Q
            bool[,] D8volumes = {
                                  {on, off, off, off, off}, // One row per intensity
                                  {on, on, off, off, off},  // with a switch for each track
                                  {on, on, on, off, off},
                                  {on, on, on, on, off},
                                  {on, on, on, on, on}
                              };
            Dance8ths = new Song(manager.Game, dance8thsSounds, dance8thsWaves,
                D8arr, D8volumes);

            //LandOfSand16ths
            string[,] LoS16arr = { 
            { LoSBD01, LoSBD01,LoSBD01,LoSBD01,LoSBD01,LoSBD01,LoSBD01,LoSBD01,
                    LoSBD01,LoSBD01,LoSBD01,LoSBD01,LoSBD01,LoSBD01,LoSBD01,LoSBD01},
            { LoSHH01, LoSHH02, LoSHH03, LoSHH04, LoSHH01, LoSHH02, LoSHH03, LoSHH04, 
                    LoSHH01, LoSHH02, LoSHH03, LoSHH04, LoSHH01, LoSHH02, LoSHH03, LoSHH04},
            { LoSBS01, LoSBS02, LoSBS03, LoSBS04, LoSBS05, LoSBS06, LoSBS07, LoSBS08, 
                    LoSBS01, LoSBS02, LoSBS03, LoSBS04, LoSBS05, LoSBS06, LoSBS07, LoSBS08},
            { LoSSt01, LoSSt02, LoSSt03, LoSSt04, LoSSt05, LoSSt06, LoSSt07, LoSSt08, 
                    LoSSt01, LoSSt02, LoSSt03, LoSSt04, LoSSt05, LoSSt06, LoSSt07, LoSSt08},
            {LoSMel01, LoSMel02, LoSMel03, LoSMel04, LoSMel05, LoSMel06, LoSMel07, LoSMel08,
                    LoSMel01, LoSMel02, LoSMel03, LoSMel04, LoSMel05, LoSMel06, LoSMel07, LoSMel08},
            {LoSAcc01, LoSAcc02, LoSAcc03, LoSAcc04, LoSAcc05, LoSAcc06, LoSAcc07, LoSAcc08,
                    LoSAcc01, LoSAcc02, LoSAcc03, LoSAcc04, LoSAcc05, LoSAcc06, LoSAcc07, LoSAcc08,},                                 
                                 };

            //this intensity volume map is per track, not per Q
            bool[,] LoS16volumes = {
                                  {on, on, off, off, off, off, off},
                                  {on, on, on, off, off, off, off},
                                  {on, on, on, on, off, off, off},
                                  {on, on, off, on, on, off, off},
                                  {on, on, off, on, on, on, off},
                                  {on, off, off, on, on, on, on},
                              };
            LandOfSand16ths = new Song(manager.Game, LoS16Sounds, LoS16Waves,
                LoS16arr, LoS16volumes);

            //Ultrafix
            string[,] UFarr = {
              {UFB1, UFB2, UFB3, UFB4, UFB5, UFB6, UFB7, UFB8},
              {UFD1, UFD2, UFD3, UFD4, UFD5, UFD6, UFD7, UFD8},
              {UFFX1, UFFX2, UFFX3, UFFX4, UFFX5, UFFX6, UFFX7, UFFX8},
              {UFS1, UFS2, UFS3, UFS4, UFS5, UFS6, UFS7, UFS8}
                              };

            bool[,] UFvolumes = {
                      {on, off, off, off}, // One row per intensity
                      {on, on, off, off},  // with a switch for each track
                      {on, on, on, off},
                      {on, on, on, on},
                                };

            Ultrafix = new Song(manager.Game, UFSounds, UFWaves, UFarr, UFvolumes);
            Ultrafix.beater.lengthOfBeat = 500f;

            //Addition of songs to Game Components
            manager.Game.Components.Add(SelectSong);
            manager.Game.Components.Add(Dance8ths);
            manager.Game.Components.Add(LandOfSand16ths);
            manager.Game.Components.Add(Credits);
            manager.Game.Components.Add(Ultrafix);
        }

    }
}