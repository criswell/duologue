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
    //the rule is: one sound bank = one song = one SongID
    public enum SongID { SelectMenu, Intensity}

    //keep from having to tweak floats and add levels in many places
    public struct Loudness
    {
        public const float Silent = 0.0f;
        public const float Full = 999.0f;
    }


    public class Music
    {
        private AudioManager notifier;
        private static Dictionary<SongID, Song> songMap = new Dictionary<SongID, Song>();
        private BeatEffectsSong beatSong;
        private SelectMenuSong selectSong;
        private BeatEngine beatEngine;

        public Music(AudioManager manager)
        {
            notifier = manager;
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);

            beatSong = new BeatEffectsSong();
            selectSong = new SelectMenuSong();
            beatEngine = new BeatEngine(manager.Game);
            manager.Game.Components.Add(beatEngine);

            songMap.Add(SongID.Intensity, beatSong);
            songMap.Add(SongID.SelectMenu, selectSong);
        }

        public void PlaySong(SongID ID)
        {
            songMap[ID].Play();
            if (ID == SongID.Intensity)
                beatEngine.Enabled = true;
        }

        public void StopSong(SongID ID)
        {
            if (ID == SongID.Intensity)
                beatEngine.Enabled = false;
            songMap[ID].Stop();
        }

        public bool SongIsPlaying(SongID ID)
        {
            return songMap[ID].IsPlaying;
        }

        public void UpdateIntensity(IntensityEventArgs e)
        {
            beatSong.ChangeIntensity(e.ChangeAmount);
            //songMap.Keys.ToList().ForEach(delegate(SongID ID)
            //{
            //    if (songMap[ID].GetType() == IIntensitySong)
            //    {
            //        ((IIntensitySong)songMap[ID]).SetIntensity(notifier.Intensity);
            //    }
            //});
        }

        public void Detach()
        {
            notifier.Changed -= new IntensityEventHandler(UpdateIntensity);
            notifier = null;
        }

    }
}