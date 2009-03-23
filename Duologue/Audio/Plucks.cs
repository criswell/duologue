using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio
{

    public enum PluckNote { A, A3, C, C3, E }

    class Plucks
    {
        public const string PlucksWB = "Content\\Audio\\Plucks.xwb";
        public const string PlucksSB = "Content\\Audio\\Plucks.xsb";

        public const string A = "A";
        public const string A3 = "A3";
        public const string C = "C";
        public const string C3 = "C3";
        public const string E = "E";

        protected AudioManager audio;

        private static Dictionary<PluckNote, string> PluckMap =
            new Dictionary<PluckNote, string>
            {
                {PluckNote.A, A}, {PluckNote.A3, A3}, {PluckNote.C, C},
                {PluckNote.C3, C3}, {PluckNote.E, E}
            };

        public Plucks()
        {
            audio = ServiceLocator.GetService<AudioManager>();

            List<string> plucksNames = new List<string>();
            //plucksNames = PluckMap.Values.ToList<string>();
            foreach (string name in PluckMap.Values)
            {
                plucksNames.Add(name);
            }
            AudioHelper.Preload(PlucksSB, PlucksWB, plucksNames);
        }

        public void PlayPluckNote(PluckNote note)
        {
            AudioHelper.PlayCue(PlucksSB, PluckMap[note]);
        }

        public void A_()
        {
            PlayPluckNote(PluckNote.A);
        }

        public void A3_()
        {
            PlayPluckNote(PluckNote.A3);
        }

        public void C_()
        {
            PlayPluckNote(PluckNote.C);
        }

        public void C3_()
        {
            PlayPluckNote(PluckNote.C3);
        }

        public void E_()
        {
            PlayPluckNote(PluckNote.E);
        }

    }
}
