using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio
{
    class SelectMenuSong : Song, ISong
    {

        private const string waveBank = "Content\\Audio\\SelectMenu.xwb";
        private const string soundBank = "Content\\Audio\\SelectMenu.xsb";
        
        private const string SelectMenuCue = "nicStage_gso";

        private const int NUMBER_OF_TRACKS = 1;

        public SelectMenuSong() : base()
        {
            this.WaveBankName = waveBank;
            this.SoundBankName = soundBank;
            this.Tracks = new List<Track> { new Track(SelectMenuCue, Loudness.Full) };
            List <string> cues = new List<string>();
            Tracks.ForEach(delegate(Track track)
                {
                    cues.Add(track.CueName);
                });
            AudioHelper.AddBank(SoundBankName, WaveBankName, cues);
        }

        public override void Play()
        {
            AudioHelper.PlayCues(SoundBankName, PlayType.Nonstop);
        }

        public override void Stop()
        {
            AudioHelper.StopCues(SoundBankName);
        }

    }

}
