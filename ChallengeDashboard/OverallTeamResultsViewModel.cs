using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class OverallTeamResultsViewModel : OverallResultsViewModel
    {
        public IEnumerable<TeamResults> Results { get; init; }
    }
}