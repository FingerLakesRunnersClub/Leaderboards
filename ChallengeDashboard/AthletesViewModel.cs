using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class AthletesViewModel : DataTableViewModel
    {
        public override string Title => "All Athletes";
        public override string ResponsiveBreakpoint => "md";
        public IDictionary<uint, Athlete> Athletes { get; init; }
    }
}