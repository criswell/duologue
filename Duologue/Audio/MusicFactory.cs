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

        //Superbowl Intro constants
        protected const string sbiWaves = "Content\\Audio\\Superintro.xwb";
        protected const string sbiSounds = "Content\\Audio\\Superintro.xsb";
        protected const string sbIntro = "intro";

        //Superbowl constants
        protected const string sbWaves = "Content\\Audio\\Superbowl.xwb";
        protected const string sbSounds = "Content\\Audio\\Superbowl.xsb";
        protected const string sbSD1 = "solodrums-1";
        protected const string sbSD2 = "solodrums-2";
        protected const string sbSD3 = "solodrums-3";
        protected const string sbSD4 = "solodrums-4";
        protected const string sbSD5 = "solodrums-5";
        protected const string sbSD6 = "solodrums-6";
        protected const string sbSD7 = "solodrums-7";
        protected const string sbSD8 = "solodrums-8";
        protected const string sbBD1 = "backdrums-1";
        protected const string sbBD2 = "backdrums-2";
        protected const string sbBD3 = "backdrums-3";
        protected const string sbBD4 = "backdrums-4";
        protected const string sbBD5 = "backdrums-5";
        protected const string sbBD6 = "backdrums-6";
        protected const string sbBD7 = "backdrums-7";
        protected const string sbBD8 = "backdrums-8";
        protected const string sbB01 = "bass-01";
        protected const string sbB02 = "bass-02";
        protected const string sbB03 = "bass-03";
        protected const string sbB04 = "bass-04";
        protected const string sbB05 = "bass-05";
        protected const string sbB06 = "bass-06";
        protected const string sbB07 = "bass-07";
        protected const string sbB08 = "bass-08";
        protected const string sbB09 = "bass-09";
        protected const string sbB10 = "bass-10";
        protected const string sbB11 = "bass-11";
        protected const string sbB12 = "bass-12";
        protected const string sbB13 = "bass-13";
        protected const string sbB14 = "bass-14";
        protected const string sbB15 = "bass-15";
        protected const string sbB16 = "bass-16";
        protected const string sbB17 = "bass-17";
        protected const string sbB18 = "bass-18";
        protected const string sbB19 = "bass-19";
        protected const string sbB20 = "bass-20";
        protected const string sbB21 = "bass-21";
        protected const string sbB22 = "bass-22";
        protected const string sbB23 = "bass-23";
        protected const string sbB24 = "bass-24";
        protected const string sbB25 = "bass-25";
        protected const string sbB26 = "bass-26";
        protected const string sbB27 = "bass-27";
        protected const string sbB28 = "bass-28";
        protected const string sbB29 = "bass-29";
        protected const string sbB30 = "bass-30";
        protected const string sbB31 = "bass-31";
        protected const string sbB32 = "bass-32";
        protected const string sbB33 = "bass-33";
        protected const string sbB34 = "bass-34";
        protected const string sbB35 = "bass-35";
        protected const string sbB36 = "bass-36";
        protected const string sbB37 = "bass-37";
        protected const string sbB38 = "bass-38";
        protected const string sbB39 = "bass-39";
        protected const string sbB40 = "bass-40";
        protected const string sbB41 = "bass-41";
        protected const string sbB42 = "bass-42";
        protected const string sbB43 = "bass-43";
        protected const string sbB44 = "bass-44";
        protected const string sbB45 = "bass-45";
        protected const string sbB46 = "bass-46";
        protected const string sbB47 = "bass-47";
        protected const string sbB48 = "bass-48";
        protected const string sbB49 = "bass-49";
        protected const string sbB50 = "bass-50";
        protected const string sbB51 = "bass-51";
        protected const string sbB52 = "bass-52";
        protected const string sbB53 = "bass-53";
        protected const string sbB54 = "bass-54";
        protected const string sbB55 = "bass-55";
        protected const string sbB56 = "bass-56";
        protected const string sbB57 = "bass-57";
        protected const string sbB58 = "bass-58";
        protected const string sbB59 = "bass-59";
        protected const string sbB60 = "bass-60";
        protected const string sbB61 = "bass-61";
        protected const string sbB62 = "bass-62";
        protected const string sbB63 = "bass-63";
        protected const string sbB64 = "bass-64";
        protected const string sbBO01 = "buzzorgan-01";
        protected const string sbBO02 = "buzzorgan-02";
        protected const string sbBO03 = "buzzorgan-03";
        protected const string sbBO04 = "buzzorgan-04";
        protected const string sbBO05 = "buzzorgan-05";
        protected const string sbBO06 = "buzzorgan-06";
        protected const string sbBO07 = "buzzorgan-07";
        protected const string sbBO08 = "buzzorgan-08";
        protected const string sbBO09 = "buzzorgan-09";
        protected const string sbBO10 = "buzzorgan-10";
        protected const string sbBO11 = "buzzorgan-11";
        protected const string sbBO12 = "buzzorgan-12";
        protected const string sbBO13 = "buzzorgan-13";
        protected const string sbBO14 = "buzzorgan-14";
        protected const string sbBO15 = "buzzorgan-15";
        protected const string sbBO16 = "buzzorgan-16";
        protected const string sbBO17 = "buzzorgan-17";
        protected const string sbBO18 = "buzzorgan-18";
        protected const string sbBO19 = "buzzorgan-19";
        protected const string sbBO20 = "buzzorgan-20";
        protected const string sbBO21 = "buzzorgan-21";
        protected const string sbBO22 = "buzzorgan-22";
        protected const string sbBO23 = "buzzorgan-23";
        protected const string sbBO24 = "buzzorgan-24";
        protected const string sbBO25 = "buzzorgan-25";
        protected const string sbBO26 = "buzzorgan-26";
        protected const string sbBO27 = "buzzorgan-27";
        protected const string sbBO28 = "buzzorgan-28";
        protected const string sbBO29 = "buzzorgan-29";
        protected const string sbBO30 = "buzzorgan-30";
        protected const string sbBO31 = "buzzorgan-31";
        protected const string sbBO32 = "buzzorgan-32";
        protected const string sbBO33 = "buzzorgan-33";
        protected const string sbBO34 = "buzzorgan-34";
        protected const string sbBO35 = "buzzorgan-35";
        protected const string sbBO36 = "buzzorgan-36";
        protected const string sbBO37 = "buzzorgan-37";
        protected const string sbBO38 = "buzzorgan-38";
        protected const string sbBO39 = "buzzorgan-39";
        protected const string sbBO40 = "buzzorgan-40";
        protected const string sbBO41 = "buzzorgan-41";
        protected const string sbBO42 = "buzzorgan-42";
        protected const string sbBO43 = "buzzorgan-43";
        protected const string sbBO44 = "buzzorgan-44";
        protected const string sbBO45 = "buzzorgan-45";
        protected const string sbBO46 = "buzzorgan-46";
        protected const string sbBO47 = "buzzorgan-47";
        protected const string sbBO48 = "buzzorgan-48";
        protected const string sbBO49 = "buzzorgan-49";
        protected const string sbBO50 = "buzzorgan-50";
        protected const string sbBO51 = "buzzorgan-51";
        protected const string sbBO52 = "buzzorgan-52";
        protected const string sbBO53 = "buzzorgan-53";
        protected const string sbBO54 = "buzzorgan-54";
        protected const string sbBO55 = "buzzorgan-55";
        protected const string sbBO56 = "buzzorgan-56";
        protected const string sbBO57 = "buzzorgan-57";
        protected const string sbBO58 = "buzzorgan-58";
        protected const string sbBO59 = "buzzorgan-59";
        protected const string sbBO60 = "buzzorgan-60";
        protected const string sbBO61 = "buzzorgan-61";
        protected const string sbBO62 = "buzzorgan-62";
        protected const string sbBO63 = "buzzorgan-63";
        protected const string sbBO64 = "buzzorgan-64";        
        
        //Beat Effects Song constants
        protected const string beatEffectsWaves = "Content\\Audio\\Intensity.xwb";
        protected const string beatEffectsSounds = "Content\\Audio\\Intensity.xsb";
        protected const string Intensity1 = "beat";
        protected const string Intensity2 = "bass";
        protected const string Intensity3 = "bassplus";
        protected const string Intensity4 = "organ";
        protected const string Intensity5 = "guitar";

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

        //WinOne Song constants
        protected const string WinOneWaves = "Content\\Audio\\WinOne.xwb";
        protected const string WinOneSounds = "Content\\Audio\\WinOne.xsb";
        protected const string W1B1 = "beat-1";
        protected const string W1B2 = "beat-2";
        protected const string W1B3 = "beat-3";
        protected const string W1B4 = "beat-4";
        protected const string W1sbA01 = "synth_beatA-01";
        protected const string W1sbA02 = "synth_beatA-02";
        protected const string W1sbA03 = "synth_beatA-03";
        protected const string W1sbA04 = "synth_beatA-04";
        protected const string W1sbA05 = "synth_beatA-05";
        protected const string W1sbA06 = "synth_beatA-06";
        protected const string W1sbA07 = "synth_beatA-07";
        protected const string W1sbA08 = "synth_beatA-08";
        protected const string W1sbA09 = "synth_beatA-09";
        protected const string W1sbA10 = "synth_beatA-10";
        protected const string W1sbA11 = "synth_beatA-11";
        protected const string W1sbA12 = "synth_beatA-12";
        protected const string W1sbA13 = "synth_beatA-13";
        protected const string W1sbA14 = "synth_beatA-14";
        protected const string W1sbA15 = "synth_beatA-15";
        protected const string W1sbA16 = "synth_beatA-16";
        protected const string W1sbB01 = "synth_beatB-01";
        protected const string W1sbB02 = "synth_beatB-02";
        protected const string W1sbB03 = "synth_beatB-03";
        protected const string W1sbB04 = "synth_beatB-04";
        protected const string W1sbB05 = "synth_beatB-05";
        protected const string W1sbB06 = "synth_beatB-06";
        protected const string W1sbB07 = "synth_beatB-07";
        protected const string W1sbB08 = "synth_beatB-08";
        protected const string W1sbB09 = "synth_beatB-09";
        protected const string W1sbB10 = "synth_beatB-10";
        protected const string W1sbB11 = "synth_beatB-11";
        protected const string W1sbB12 = "synth_beatB-12";
        protected const string W1sbB13 = "synth_beatB-13";
        protected const string W1sbB14 = "synth_beatB-14";
        protected const string W1sbB15 = "synth_beatB-15";
        protected const string W1sbB16 = "synth_beatB-16";

        protected const string W1breath = "breath";

        protected const string W1rA1 = "raveA-1";
        protected const string W1rA2 = "raveA-2";
        protected const string W1rA3 = "raveA-3";
        protected const string W1rA4 = "raveA-4";
        protected const string W1rA5 = "raveA-5";
        protected const string W1rB01 = "raveB-01";
        protected const string W1rB02 = "raveB-02";
        protected const string W1rB03 = "raveB-03";
        protected const string W1rB04 = "raveB-04";
        protected const string W1rB05 = "raveB-05";
        protected const string W1rB06 = "raveB-06";
        protected const string W1rB07 = "raveB-07";
        protected const string W1rB08 = "raveB-08";
        protected const string W1rB09 = "raveB-09";
        protected const string W1rB10 = "raveB-10";
        protected const string W1rB11 = "raveB-11";
        protected const string W1rB12 = "raveB-12";
        protected const string W1rB13 = "raveB-13";
        protected const string W1rB14 = "raveB-14";
        protected const string W1rB15 = "raveB-15";
        protected const string W1rB16 = "raveB-16";
        protected const string W1rB17 = "raveB-17";
        protected const string W1oA01 = "orionA-1";
        protected const string W1oA02 = "orionA-2";
        protected const string W1oA03 = "orionA-3";
        protected const string W1oA04 = "orionA-4";
        protected const string W1oA05 = "orionA-5";
        protected const string W1oA06 = "orionA-6";
        protected const string W1oA07 = "orionA-7";
        protected const string W1oA08 = "orionA-8";
        protected const string W1oA09 = "orionA-9";
        protected const string W1oB01 = "orionB-01";
        protected const string W1oB02 = "orionB-02";
        protected const string W1oB03 = "orionB-03";
        protected const string W1oB04 = "orionB-04";
        protected const string W1oB05 = "orionB-05";
        protected const string W1oB06 = "orionB-06";
        protected const string W1oB07 = "orionB-07";
        protected const string W1oB08 = "orionB-08";
        protected const string W1oB09 = "orionB-09";
        protected const string W1oB10 = "orionB-10";
        protected const string W1oB11 = "orionB-11";
        protected const string W1oB12 = "orionB-12";
        protected const string W1oB13 = "orionB-13";
        protected const string W1oB14 = "orionB-14";
        protected const string W1oB15 = "orionB-15";
        protected const string W1oB16 = "orionB-16";
        protected const string W1oB17 = "orionB-17";
        protected const string W1jA01 = "jamA-01";
        protected const string W1jA02 = "jamA-02";
        protected const string W1jA03 = "jamA-03";
        protected const string W1jA04 = "jamA-04";
        protected const string W1jA05 = "jamA-05";
        protected const string W1jA06 = "jamA-06";
        protected const string W1jA07 = "jamA-07";
        protected const string W1jA08 = "jamA-08";
        protected const string W1jA09 = "jamA-09";
        protected const string W1jA10 = "jamA-10";
        protected const string W1jA11 = "jamA-11";
        protected const string W1jA12 = "jamA-12";
        protected const string W1jA13 = "jamA-13";
        protected const string W1jA14 = "jamA-14";
        protected const string W1jA15 = "jamA-15";
        protected const string W1jA16 = "jamA-16";
        protected const string W1jA17 = "jamA-17";
        protected const string W1jB01 = "jamB-01";
        protected const string W1jB02 = "jamB-02";
        protected const string W1jB03 = "jamB-03";
        protected const string W1jB04 = "jamB-04";
        protected const string W1jB05 = "jamB-05";
        protected const string W1jB06 = "jamB-06";
        protected const string W1jB07 = "jamB-07";
        protected const string W1jB08 = "jamB-08";
        protected const string W1jB09 = "jamB-09";
        protected const string W1jB10 = "jamB-10";
        protected const string W1jB11 = "jamB-11";
        protected const string W1jB12 = "jamB-12";
        protected const string W1jB13 = "jamB-13";
        protected const string W1jB14 = "jamB-14";
        protected const string W1jB15 = "jamB-15";
        protected const string W1jB16 = "jamB-16";
        protected const string W1jB17 = "jamB-17";

        //SecondChance Song constants
        protected const string SecondChanceWaves = "Content\\Audio\\SecondChance.xwb";
        protected const string SecondChanceSounds = "Content\\Audio\\SecondChance.xsb";
        protected const string SCb01 = "beat-01";
        protected const string SCb02 = "beat-02";
        protected const string SCb03 = "beat-03";
        protected const string SCb04 = "beat-04";
        protected const string SCb05 = "beat-05";
        protected const string SCb06 = "beat-06";
        protected const string SCb07 = "beat-07";
        protected const string SCb08 = "beat-08";
        protected const string SCb09 = "beat-09";
        protected const string SCb10 = "beat-10";
        protected const string SCb11 = "beat-11";
        protected const string SCb12 = "beat-12";
        protected const string SCb13 = "beat-13";
        protected const string SCb14 = "beat-14";
        protected const string SCb15 = "beat-15";
        protected const string SCb16 = "beat-16";
        protected const string SCn01 = "nernt-01";
        protected const string SCn02 = "nernt-02";
        protected const string SCn03 = "nernt-03";
        protected const string SCn04 = "nernt-04";
        protected const string SCn05 = "nernt-05";
        protected const string SCn06 = "nernt-06";
        protected const string SCn07 = "nernt-07";
        protected const string SCn08 = "nernt-08";
        protected const string SCn09 = "nernt-09";
        protected const string SCn10 = "nernt-10";
        protected const string SCn11 = "nernt-11";
        protected const string SCn12 = "nernt-12";
        protected const string SCn13 = "nernt-13";
        protected const string SCn14 = "nernt-14";
        protected const string SCn15 = "nernt-15";
        protected const string SCn16 = "nernt-16";
        protected const string SCc01 = "cartoon-01";
        protected const string SCc02 = "cartoon-02";
        protected const string SCc03 = "cartoon-03";
        protected const string SCc04 = "cartoon-04";
        protected const string SCc05 = "cartoon-05";
        protected const string SCc06 = "cartoon-06";
        protected const string SCc07 = "cartoon-07";
        protected const string SCc08 = "cartoon-08";
        protected const string SCc09 = "cartoon-09";
        protected const string SCc10 = "cartoon-10";
        protected const string SCc11 = "cartoon-11";
        protected const string SCc12 = "cartoon-12";
        protected const string SCc13 = "cartoon-13";
        protected const string SCc14 = "cartoon-14";
        protected const string SCc15 = "cartoon-15";
        protected const string SCc16 = "cartoon-16";


        protected static Dictionary<SongID, Song> songMap = new Dictionary<SongID, Song>();
        public Song SelectSong;
        public Song Dance8ths;
        public Song LandOfSand16ths;
        public Song Credits;
        public Song Ultrafix;
        public Song WinOne;
        public Song SecondChance;
        public Song SuperbowlIntro;
        public Song Superbowl;

        public MusicFactory(AudioManager manager)
        {
            bool on = true;
            bool off = false;

            //SelectSong
            string[] selectArr = { selectMenuCue };
            SelectSong = new Song(manager.Game, selectMenuSounds, selectMenuWaves, selectArr);

            //Credits
            string[] creditsArr = { creditsCue };
            Credits = new Song(manager.Game, creditsSounds, creditsWaves, creditsArr);

            //Superbowl Intro
            string[] sbIntroArr = { sbIntro };
            SuperbowlIntro = new Song(manager.Game, sbiSounds, sbiWaves, sbIntroArr);

            //Superbowl (the loop)
            string[,] SBarr = { 
                                {
                                    sbSD1, sbSD2, sbSD3, sbSD4, sbSD5, sbSD6, sbSD7, sbSD8, 
                                    sbSD1, sbSD2, sbSD3, sbSD4, sbSD5, sbSD6, sbSD7, sbSD8, 
                                    sbSD1, sbSD2, sbSD3, sbSD4, sbSD5, sbSD6, sbSD7, sbSD8, 
                                    sbSD1, sbSD2, sbSD3, sbSD4, sbSD5, sbSD6, sbSD7, sbSD8, 
                                    sbSD1, sbSD2, sbSD3, sbSD4, sbSD5, sbSD6, sbSD7, sbSD8, 
                                    sbSD1, sbSD2, sbSD3, sbSD4, sbSD5, sbSD6, sbSD7, sbSD8, 
                                    sbSD1, sbSD2, sbSD3, sbSD4, sbSD5, sbSD6, sbSD7, sbSD8, 
                                    sbSD1, sbSD2, sbSD3, sbSD4, sbSD5, sbSD6, sbSD7, sbSD8
                                },
                                {
                                    sbBD1, sbBD2, sbBD3, sbBD4, sbBD5, sbBD6, sbBD7, sbBD8, 
                                    sbBD1, sbBD2, sbBD3, sbBD4, sbBD5, sbBD6, sbBD7, sbBD8, 
                                    sbBD1, sbBD2, sbBD3, sbBD4, sbBD5, sbBD6, sbBD7, sbBD8, 
                                    sbBD1, sbBD2, sbBD3, sbBD4, sbBD5, sbBD6, sbBD7, sbBD8, 
                                    sbBD1, sbBD2, sbBD3, sbBD4, sbBD5, sbBD6, sbBD7, sbBD8, 
                                    sbBD1, sbBD2, sbBD3, sbBD4, sbBD5, sbBD6, sbBD7, sbBD8, 
                                    sbBD1, sbBD2, sbBD3, sbBD4, sbBD5, sbBD6, sbBD7, sbBD8, 
                                    sbBD1, sbBD2, sbBD3, sbBD4, sbBD5, sbBD6, sbBD7, sbBD8
                                },
                                {
                                    sbB01, sbB02, sbB03, sbB04, sbB05, sbB06, sbB07, sbB08, 
                                    sbB09, sbB10, sbB11, sbB12, sbB13, sbB14, sbB15, sbB16, 
                                    sbB17, sbB18, sbB19, sbB20, sbB21, sbB22, sbB23, sbB24, 
                                    sbB25, sbB26, sbB27, sbB28, sbB29, sbB30, sbB31, sbB32, 
                                    sbB33, sbB34, sbB35, sbB36, sbB37, sbB38, sbB39, sbB40, 
                                    sbB41, sbB42, sbB43, sbB44, sbB45, sbB46, sbB47, sbB48, 
                                    sbB49, sbB50, sbB51, sbB52, sbB53, sbB54, sbB55, sbB56, 
                                    sbB57, sbB58, sbB59, sbB60, sbB61, sbB62, sbB63, sbB64 
                                },
                                {
                                    sbBO01, sbBO02, sbBO03, sbBO04, sbBO05, sbBO06, sbBO07, sbBO08, 
                                    sbBO09, sbBO10, sbBO11, sbBO12, sbBO13, sbBO14, sbBO15, sbBO16, 
                                    sbBO17, sbBO18, sbBO19, sbBO20, sbBO21, sbBO22, sbBO23, sbBO24, 
                                    sbBO25, sbBO26, sbBO27, sbBO28, sbBO29, sbBO30, sbBO31, sbBO32, 
                                    sbBO33, sbBO34, sbBO35, sbBO36, sbBO37, sbBO38, sbBO39, sbBO40, 
                                    sbBO41, sbBO42, sbBO43, sbBO44, sbBO45, sbBO46, sbBO47, sbBO48, 
                                    sbBO49, sbBO50, sbBO51, sbBO52, sbBO53, sbBO54, sbBO55, sbBO56, 
                                    sbBO57, sbBO58, sbBO59, sbBO60, sbBO61, sbBO62, sbBO63, sbBO64 
                                }
                              };
            bool[,] SBVolumes = { 
                                    { on, off, off, off },
                                    { off, on, on, off },
                                    { off, on, on, on }
                                };
            Superbowl = new Song(manager.Game, sbSounds, sbWaves, SBarr, SBVolumes);
            Superbowl.beater.lengthOfBeat = 355f;

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

            //WinOne
            string[,] W1arr = {
                      {W1B1, W1B2, W1B3, W1B4, W1B1, W1B2, W1B3, W1B4,
                       W1B1, W1B2, W1B3, W1B4, W1B1, W1B2, W1B3, W1B4,
                       W1B1, W1B2, W1B3, W1B4, W1B1, W1B2, W1B3, W1B4,
                       W1B1, W1B2, W1B3, W1B4, W1B1, W1B2, W1B3, W1B4},

                      {W1oA01, W1oA02, W1oA03, W1oA04, W1oA05, W1oA06, W1oA07, W1oA08,
                       W1oA01, W1oA02, W1oA03, W1oA04, W1oA05, W1oA06, W1oA07, W1oA08,
                       W1oB01, W1oB02, W1oB03, W1oB04, W1oB05, W1oB06, W1oB07, W1oB08, 
                       W1oB09, W1oB10, W1oB11, W1oB12, W1oB13, W1oB14, W1oB15, W1oB16 },

                      {W1sbA01, W1sbA02, W1sbA03, W1sbA04, W1sbA05, W1sbA06, W1sbA07, W1sbA08, 
                       W1sbA09, W1sbA10, W1sbA11, W1sbA12, W1sbA13, W1sbA14, W1sbA15, W1sbA16,
                       W1sbB01, W1sbB02, W1sbB03, W1sbB04, W1sbB05, W1sbB06, W1sbB07, W1sbB08, 
                       W1sbB09, W1sbB10, W1sbB11, W1sbB12, W1sbB13, W1sbB14, W1sbB15, W1sbB16},

                      {W1rA1, W1rA2, W1rA3, W1rA4, W1rA1, W1rA2, W1rA3, W1rA4, 
                       W1rA1, W1rA2, W1rA3, W1rA4, W1rA1, W1rA2, W1rA3, W1rA4, 
                       W1rB01, W1rB02, W1rB03, W1rB04, W1rB05, W1rB06, W1rB07, W1rB08, 
                       W1rB09, W1rB10, W1rB11, W1rB12, W1rB13, W1rB14, W1rB15, W1rB16},
                                                            
                      {W1jA01, W1jA02, W1jA03, W1jA04, W1jA05, W1jA06, W1jA07, W1jA08, 
                       W1jA09, W1jA10, W1jA11, W1jA12, W1jA13, W1jA14, W1jA15, W1jA16,
                       W1jB01, W1jB02, W1jB03, W1jB04, W1jB05, W1jB06, W1jB07, W1jB08, 
                       W1jB09, W1jB10, W1jB11, W1jB12, W1jB13, W1jB14, W1jB15, W1jB16},

                       {"","","","","","","","","","","","","","",W1breath,"",
                           "","","","","","","","","","","","","","","",""},

                              };

            bool[,] W1volumes = {
                                    {on, off, off, off, off, off},
                                    {on, on, off, off, off, on},
                                    {on, on, on, off, off, on},
                                    {on, on, on, on, off, off},
                                    {on, on, on, off, on, off}
                                };

            WinOne = new Song(manager.Game, WinOneSounds, WinOneWaves, W1arr, W1volumes);
            WinOne.beater.lengthOfBeat = 450f;

            //SecondChance
            string[,] SCarr = {
              {SCb01, SCb02, SCb03, SCb04, SCb05, SCb06, SCb07, SCb08, 
                  SCb09, SCb10, SCb11, SCb12, SCb13, SCb14, SCb15, SCb16 },
              {SCn01, SCn02, SCn03, SCn04, SCn05, SCn06, SCn07, SCn08, 
                  SCn09, SCn10, SCn11, SCn12, SCn13, SCn14, SCn15, SCn16 },
              {SCc01, SCc02, SCc03, SCc04, SCc05, SCc06, SCc07, SCc08, 
                  SCc09, SCc10, SCc11, SCc12, SCc13, SCc14, SCc15, SCc16 },
                              };

            bool[,] SCvolumes = {
                      {on, off, off}, // One row per intensity
                      {on, on, off},  // with a switch for each track
                      {on, on, on},
                                };

            SecondChance = new Song(manager.Game, SecondChanceSounds, SecondChanceWaves, SCarr, SCvolumes);
            SecondChance.beater.lengthOfBeat = 450f;


            //Addition of songs to Game Components
            manager.Game.Components.Add(SelectSong);
            manager.Game.Components.Add(Dance8ths);
            manager.Game.Components.Add(LandOfSand16ths);
            manager.Game.Components.Add(Credits);
            manager.Game.Components.Add(Ultrafix);
            manager.Game.Components.Add(WinOne);
            manager.Game.Components.Add(SecondChance);
            manager.Game.Components.Add(SuperbowlIntro);
            manager.Game.Components.Add(Superbowl);
        }

    }
}