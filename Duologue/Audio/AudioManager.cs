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

    //keep from having to tweak floats and add levels in many places
    public struct Loudness
    {
        public const float Silent = 0.0f;
        public const float Full = 1.0f;
        public const string param = "Volume";
    }

    public partial class Track
    {
        public Cue CueObj;
    }

    public partial class Song
    {
        public SoundBank SoundBankObj;
        public WaveBank WaveBankObj;
    }

    public partial class SoundEffect
    {
        public Cue CueObj;
    }

    public partial class SoundEffectsGroup
    {
        public SoundBank SoundBankObj;
        public WaveBank WaveBankObj;
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AudioManager : Microsoft.Xna.Framework.GameComponent
    {
        // A game-specific configurator will pass the appropriate string values
        // to the contructor, so that initialize can load the audio files
        // and populate the static data structures.
        // That keeps this code generic and useable in future games.
        // However, the IIntensitySong is an immediate example of a specialized need
        // of a game for songs to have a particular type of behavior.
        // Those songs would be able to be put in the songs dictionary,
        // and this AudioManager could still handle the ISong update tasks (fade in, out)
        // on them.
        // But something may need to handle ISpecializedSong update tasks on an ad hoc
        // basis in the future, without continually adding specialized dictionaries and
        // specialized update code to this class. How should we do that?

        private static string engineFileName;
        private static AudioEngine engine;

        // Dictionary mapping a string name to a song object
        private static Dictionary<string, Song> songDict = 
            new Dictionary<string, Song>();

        private static Dictionary<string, SoundEffectsGroup> effectsDict =
            new Dictionary<string, SoundEffectsGroup>();

        private static Dictionary<string, SoundBank> soundBanks =
            new Dictionary<string, SoundBank>();

        public static Dictionary<string, Dictionary<string, Cue>> soundCueMap =
            new Dictionary<string, Dictionary<string, Cue>>();

        public AudioManager(Game game, string engineName) : base(game)
        {
            game.Components.Add(this);//FIXME I hope this is right...
            engineFileName = engineName;
            engine = new AudioEngine(engineFileName);
        }

        /// <summary>
        /// Add a song to the collection
        /// </summary>
        /// <param name="newSong"></param>
        public static void AddSong(SongID ID, Song newSong)
        {
            // load the sound bank for this song into memory
            newSong.SoundBankObj = new SoundBank(engine, newSong.SoundBankName);
            newSong.WaveBankObj = new WaveBank(engine, newSong.WaveBankName);
            // and each of the cues
            newSong.Tracks.ForEach(delegate(Track track)
            {
                track.CueObj = newSong.SoundBankObj.GetCue(track.CueName);
            });
            //stick it in the list
            songDict.Add(ID.ToString(), newSong);
        }

        /// <summary>
        /// Add a group of sound effects, which share a single sound bank,
        /// to the collection of such groups
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="newGroup"></param>
        public static void AddSoundEffectsGroup(EffectsGroupID ID,
            SoundEffectsGroup newGroup)
        {
            newGroup.SoundBankObj = new SoundBank(engine, newGroup.SoundBankName);
            newGroup.WaveBankObj = new WaveBank(engine, newGroup.WaveBankName);
            newGroup.Effects.Values.ToList().ForEach(delegate(SoundEffect effect)
            {
                effect.CueObj = newGroup.SoundBankObj.GetCue(effect.CueName);
            });
            effectsDict.Add(ID.ToString(), newGroup);
        }

        /// <summary>
        /// PlayTrack is for single-play sounds, like effects
        /// </summary>
        /// <param name="soundBankName">soundBankName</param>
        /// <param name="cueName">cueName</param>
        public static void PlayTrack(string soundBankName, string cueName)
        {
            RefreshTrack(soundBankName, cueName);
            soundCueMap[soundBankName][cueName].Play();
        }

        public static void PlaySoundEffect(EffectsGroupID ID, string effectName)
        {
            string groupName = effectsDict[ID.ToString()].SoundBankName;
            effectsDict[ID.ToString()].Effects[effectName].CueObj.Dispose();
            effectsDict[ID.ToString()].Effects[effectName].CueObj =
                effectsDict[ID.ToString()].SoundBankObj.GetCue(effectName);
            effectsDict[ID.ToString()].Effects[effectName].CueObj.Play();
        }

        private static void RefreshTrack(string soundBank, string cue)
        {
            soundCueMap[soundBank][cue].Dispose();
            soundCueMap[soundBank][cue] = soundBanks[soundBank].GetCue(cue);
        }

        public static void PlaySong(SongID ID)
        {
            //FIXME at some point, will need to allow for this manager
            //to control repeating loop play, with different cues being
            //different lengths.
            songDict[ID.ToString()].Tracks.ForEach(delegate(Track track)
            {
                track.CueObj.Play();
                track.CueObj.SetVariable(Loudness.param, track.Volume);
            });
        }

        public static void StopSong(SongID ID)
        {
            Song song = songDict[ID.ToString()];
            song.Tracks.ForEach(delegate(Track track)
            {
                track.CueObj.Stop(AudioStopOptions.AsAuthored);
                track.CueObj.Dispose();
                track.CueObj = song.SoundBankObj.GetCue(track.CueName);
            });
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
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}