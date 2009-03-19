using System;
using System.Collections.Generic;
using System.Text;

namespace Duologue.State
{
    /// <summary>
    /// The enumeration of possible game states
    /// </summary>
    public enum GameState
    {
        None,
        MainMenuSystem,
        PlayerSelect,
        CampaignGame,
        InfinityGame,
        ColorStateTest,
        EndCinematics,
        Exit,
        CompanyIntro,
    }
}
