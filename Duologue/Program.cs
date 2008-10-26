using System;

namespace Duologue
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (DuologueGame game = new DuologueGame())
            {
                game.Run();
            }
        }
    }
}

