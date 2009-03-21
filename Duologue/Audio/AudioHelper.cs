using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;


namespace Duologue.Audio
{

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

        protected const string volumeName = "Volume";

        protected static string engineFileName;
        protected static AudioEngine engine;
        protected static AudioCategory musicCategory;

        // These SoundBanks are where we pull new copies of Cues. We don't play them there.
        protected static Dictionary<string, SoundBank> soundBanks = new Dictionary<string, SoundBank>();

        // These WaveBanks are where the audio data actually resides
        // Although we don't interact with the WaveBank objects, we have to load them or nothing works
        protected static Dictionary<string, WaveBank> waveBanks = new Dictionary<string, WaveBank>();

        // These Cues are the ones we will actually call Play on.
        // They stay in this structure until they either:
        // - stop playing
        // - are bumped by a new instance
        //                        <soundbank name, <cue name, Cue instance>>
        protected static Dictionary<string, Dictionary<string, Cue>> cues =
            new Dictionary<string, Dictionary<string, Cue>>();

        // This List of Cues is where Cues go after they leave the "cues" structure.
        // During updates, this List is checked for Cues that can be disposed.
        protected static List<Cue> usedCues = new List<Cue>();

        public AudioHelper(Game game, string engineName) : base(game)
        {
            long seconds = 1;
            long ticks = seconds * 10000000000;
            TimeSpan lookahead = new TimeSpan(ticks); // tick is ten to the -10 seconds
            engineFileName = engineName;
            engine = new AudioEngine(engineFileName, lookahead, Guid.Empty);
            musicCategory = engine.GetCategory("Music");
            musicCategory.SetVolume(0.1f);
        }

        protected static void ProcessPlayedCues()
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

        protected static void RecycleCue(string sbname, string cueName)
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

        protected static void Preload(string soundbank, string cue)
        {
            if (!cues[soundbank].Keys.Contains(cue))
            {
                cues[soundbank].Add(cue, soundBanks[soundbank].GetCue(cue));
            }
        }

        protected static void Preload(Q q)
        {
            Preload(q.SoundBankName, q.CueName);
        }

        protected static void Preload(Track track)
        {
            for (int q = 0; q < track.QCount; q++)
            {
                Preload(track.Cues[q]);
            }
        }

        public static void Preload(Song song)
        {
            AddBank(song.SoundBankName, song.WaveBankName);
            SoundBank sb = soundBanks[song.SoundBankName];
            if (song.Managed)
            {
                for (int t = 0; t < song.TrackCount; t++)
                {
                    Preload(song.Tracks[t]);
                }
            }
        }

        protected static void AddBank(string soundBankName, string waveBankName)
        {
            if (!soundBanks.Keys.Contains(soundBankName))
            {
                SoundBank tmpSB = new SoundBank(engine, soundBankName);
                soundBanks.Add(soundBankName, tmpSB);
                waveBanks.Add(waveBankName, new WaveBank(engine, waveBankName));
            }

            if (!cues.Keys.Contains(soundBankName))
            {
                cues.Add(soundBankName, new Dictionary<string, Cue>());
            }
        }

        public static void Preload(string soundBankName, string waveBankName, List<string> cueNames)
        {
            AddBank(soundBankName, waveBankName);
            cueNames.ForEach(delegate(string cueName)
            {
                Preload(soundBankName, cueName);
            });
        }

/*
        public static bool CueIsPlaying(string sbname, string cuename)
        {
            return cues[sbname][cuename].IsPlaying;
        }

        public static bool CueIsStopping(string sbname, string cuename)
        {
            return cues[sbname][cuename].IsStopping;
        }
*/
        public static void PlayCue(string sbname, string cueName)
        {
            Q q = new Q(sbname, cueName);
            Play(q, false);
        }

        public static void Play(Q q, bool retainReference)
        {
            if (retainReference)
            {
                RecycleCue(q.SoundBankName, q.CueName);
                cues[q.SoundBankName][q.CueName].Play();
            }
            else
            {
                soundBanks[q.SoundBankName].PlayCue(q.CueName);
            }
        }

        public static void Play(Q q, float volume)
        {
            Play(q, true);
            cues[q.SoundBankName][q.CueName].SetVariable(volumeName, volume);
            //we *could* range limit the volume before making that call
        }

        public static void Play(Song song)
        {
            for (int t = 0; t < song.TrackCount; t++)
            {
                for (int q = 0; q < song.Tracks[t].QCount; q++)
                {
                    Play(song.Tracks[t].Cues[q], song.Tracks[t].Volume);
                }
            }
            UpdateCues(song);
        }

        public static void Pause(Q q)
        {
            cues[q.SoundBankName][q.CueName].Pause();
        }

        public static void Resume(Q q)
        {
            if (cues[q.SoundBankName][q.CueName].IsPaused)
            {
                cues[q.SoundBankName][q.CueName].Resume();
            }
        }

        public static void Stop(Q q)
        {
            cues[q.SoundBankName][q.CueName].Stop(AudioStopOptions.AsAuthored);
            RecycleCue(q.SoundBankName, q.CueName);
        }

        public static void StopCue(string sbname, string cueName)
        {
            cues[sbname][cueName].Stop(AudioStopOptions.AsAuthored);
            RecycleCue(sbname, cueName);
        }

        public static void Pause(Song song)
        {
            for (int t = 0; t < song.TrackCount; t++)
            {
                for (int q = 0; q < song.Tracks[t].QCount; q++)
                {
                    Pause(song.Tracks[t].Cues[q]);
                }
            }
            UpdateCues(song);
        }

        public static void Resume(Song song)
        {
            for (int t = 0; t < song.TrackCount; t++)
            {
                for (int q = 0; q < song.Tracks[t].QCount; q++)
                {
                    Resume(song.Tracks[t].Cues[q]);
                }
            }
            UpdateCues(song);
        }

        /*
        protected static void Stop(Track track)
        {
            if (cues[track.SoundbankName][track.CueName].IsPlaying)
            {
                StopCue(track.SoundbankName, track.CueName);
            }
        }

        public static void Stop(Song song)
        {
            song.Tracks.ForEach(track =>
                {
                    StopCue(song.SoundBankName, track.CueName);
                });
        }
        */

        public static void UpdateCues(Song song)
        {
            for (int t = 0; t < song.TrackCount; t++)
            {
                for (int q = 0; q < song.Tracks[t].QCount; q++)
                {
                    cues[song.SoundBankName][song.Tracks[t].Cues[q].CueName].SetVariable(
                        volumeName, song.Tracks[t].Volume);
                }
            }
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