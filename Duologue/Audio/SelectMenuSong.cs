using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio
{
    class SelectMenuSong : Song, ISong
    {

        private const int NUMBER_OF_TRACKS = 1;

        public SelectMenuSong() : base()
        {
            WaveBankName = Music.SelectMenuWB;
            SoundBankName = Music.SelectMenuSB;
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
