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

        private AudioManager notifier;
        private static Dictionary<SongID, Song> songMap = new Dictionary<SongID, Song>();
        //private BeatEffectsSong beatSong;
        //private LandOfSandSong landOfSandSong;
        public Song SelectSong;
        private BeatEngine beatEngine;

        public Music(AudioManager manager)
        {
            notifier = manager;
            notifier.Changed += new IntensityEventHandler(UpdateIntensity);

            //beatSong = new BeatEffectsSong();
            //landOfSandSong = new LandOfSandSong();
            beatEngine = new BeatEngine(manager.Game);

            SelectSong = new Song(manager.Game, selectMenuSounds, selectMenuWaves,
              new List<string> {selectMenuCue});

            manager.Game.Components.Add(SelectSong);
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