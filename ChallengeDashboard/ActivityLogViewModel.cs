using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class ActivityLogViewModel : ViewModel
    {
        public override string Title => "Activity Log";
        public IEnumerable<Result> Results { get; init; }
    }
}