using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class TeamResultsViewModel : ResultsViewModel
    {
        public IEnumerable<TeamResults> Results { get; init; }
    }
}