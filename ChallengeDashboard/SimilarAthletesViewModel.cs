using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class SimilarAthletesViewModel : DataTableViewModel
    {
        public override string Title => "Similar Athletes";
        public override string ResponsiveBreakpoint => "lg";
        public Athlete Athlete { get; init; }
        public IEnumerable<SimilarAthlete> Matches { get; init; }
    }
}