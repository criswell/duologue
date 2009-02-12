using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace Duologue.Audio
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SelectMenuSong : Song
    {

        private const string wave = "Content\\Audio\\SelectMenu.xwb";
        private const string sound = "Content\\Audio\\SelectMenu.xsb";
        private const string SelectMenuCue = "nicStage_gso";

        public SelectMenuSong(Game game)
            : base(game, sound, wave)
        {
            this.Tracks = new List<Track> { new Track(SelectMenuCue, Loudness.Full) };
            List<string> cues = new List<string>();
            Tracks.ForEach(delegate(Track track)
            {
                cues.Add(track.CueName);
            });
            AudioHelper.AddBank(sound, wave, cues);
            // TODO: Construct any child components here
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        public override bool IsPlaying
        {
            get
            {
                isPlaying = AudioHelper.CueIsPlaying(SoundBankName, SelectMenuCue);
                return isPlaying;
            }
            set
            {
                isPlaying = value;
            }
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