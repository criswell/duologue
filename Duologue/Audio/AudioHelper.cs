using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace Duologue.Audio
{

    public enum PlayType
    {
        Single,
        Nonstop
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AudioHelper : Microsoft.Xna.Framework.GameComponent
    {
        // A game-specific configurator will pass the appropriate string values
        // to the contructor, so that initialize can load the audio files
        // and populate the static data structures.
        // That keeps this code generic and useable in future games.
        // However, the IIntensitySong is an immediate example of a specialized need
        // of a game for songs to have a particular type of behavior.
        // Those songs would be able to be put in the songs dictionary,
        // and this AudioHelper could still handle the ISong update tasks (fade in, out)
        // on them.
        // But something may need to handle ISpecializedSong update tasks on an ad hoc
        // basis in the future, without continually adding specialized dictionaries and
        // specialized update code to this class. How should we do that?

        private const string volumeName = "Volume";

        private static string engineFileName;
        private static AudioEngine engine;

        // These SoundBanks are where we pull new copies of Cues. We don't play them there.
        private static Dictionary<string, SoundBank> soundBanks = new Dictionary<string, SoundBank>();

        // These WaveBanks are where the audio data actually resides
        // Although we don't interact with the WaveBank objects, we have to load them or nothing works
        private static Dictionary<string, WaveBank> waveBanks = new Dictionary<string, WaveBank>();

        // These Cues are the ones we will actually call Play on.
        // They stay in this structure until they either:
        // - stop playing
        // - are bumped by a new instance
        //                        <soundbank name, <cue name, Cue instance>>
        private static Dictionary<string, Dictionary<string, Cue>> cues =
            new Dictionary<string, Dictionary<string, Cue>>();

        // This List of Cues is where Cues go after they leave the "cues" structure.
        // During updates, this List is checked for Cues that can be disposed.
        private static List<Cue> usedCues = new List<Cue>();

        public AudioHelper(Game game, string engineName) : base(game)
        {
            engineFileName = engineName;
            engine = new AudioEngine(engineFileName);
        }

        private static void ProcessPlayedCues()
        {
            //FIXME watch for this introducing process latency
            usedCues.ForEach(delegate(Cue cue)
            {
                if (cue.IsStopped)
                {
                    cue.Dispose();
                    usedCues.Remove(cue);
                }
            });
        }

        private static void RecycleCue(string sbname, string cueName)
        {
            if (null != cues[sbname][cueName])
            {
                //don't want to end up with an ever growing list of cues that never played!
                if (cues[sbname][cueName].IsPlaying ||
                    cues[sbname][cueName].IsStopping)
                {
                    usedCues.Add(cues[sbname][cueName]);
                }
                else if (!cues[sbname][cueName].IsDisposed)
                {
                    cues[sbname][cueName].Dispose();
                }
            }
            cues[sbname][cueName] = soundBanks[sbname].GetCue(cueName);
        }

        public static void AddBank(string soundBankName, string waveBankName, List<string> cueNames)
        {
            SoundBank tmpSB = new SoundBank(engine, soundBankName);
            soundBanks.Add(soundBankName, tmpSB);
            waveBanks.Add(waveBankName, new WaveBank(engine, waveBankName));

            cues.Add(soundBankName, new Dictionary<string, Cue>());

            cueNames.ForEach(delegate(string cueName)
            {
                cues[soundBankName].Add(cueName, tmpSB.GetCue(cueName));
            });
        }

        public static void PreloadSong(Song song)
        {
            SoundBank tmpSB = new SoundBank(engine, song.SoundBankName);
            soundBanks.Add(song.SoundBankName, tmpSB);
            waveBanks.Add(song.WaveBankName,
                new WaveBank(engine, song.WaveBankName));

            cues.Add(song.SoundBankName, new Dictionary<string, Cue>());

            song.Tracks.Values.ToList().ForEach(delegate(Track track)
            {
                cues[song.SoundBankName].Add(track.CueName, 
                    tmpSB.GetCue(track.CueName));
            });
        }

        public static bool CueIsPlaying(string sbname, string cuename)
        {
            return cues[sbname][cuename].IsPlaying;
        }

        public static void Play(Song song)
        {
            song.Tracks.Values.ToList().ForEach(track =>
                {
                    RecycleCue(song.SoundBankName, track.CueName);
                    cues[song.SoundBankName][track.CueName].Play();
                    cues[song.SoundBankName][track.CueName].SetVariable(
                        volumeName, track.Volume);
                });
            UpdateCues(song);
        }

        public static void Stop(Song song)
        {
            song.Tracks.Values.ToList().ForEach(track =>
                {
                    StopCue(song.SoundBankName, track.CueName);
                });
        }

        public static void PlayCue(string sbname, string cueName, PlayType type)
        {
            RecycleCue(sbname, cueName);
            cues[sbname][cueName].SetVariable(volumeName, Loudness.Normal);
            cues[sbname][cueName].Play();
        }

        public static void UpdateCues(Song song)
        {
            song.Tracks.Values.ToList().ForEach(track =>
            {
                cues[song.SoundBankName][track.CueName].SetVariable(volumeName, track.Volume);
            });
        }

        public static void StopCue(string sbname, string cueName)
        {
            cues[sbname][cueName].Stop(AudioStopOptions.AsAuthored);
            RecycleCue(sbname, cueName);
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            engine.Update();
            ProcessPlayedCues();
            base.Update(gameTime);
        }
    }
}