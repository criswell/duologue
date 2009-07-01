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
        SurvivalGame,
        InfiniteGame,
        ColorStateTest,
        EndCinematics,
        Credits,
        Exit,
        CompanyIntro,
        MedalCase,
        BuyScreen,
    }
}
