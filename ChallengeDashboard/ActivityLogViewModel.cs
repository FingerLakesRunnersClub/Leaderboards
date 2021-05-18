using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class ActivityLogViewModel : ViewModel
    {
        public override string Title => "Activity Log";
        
        public ActivityLogType Type { get; init; }
        public IEnumerable<IGrouping<string, Result>> Results { get; init; }

        public readonly IDictionary<ActivityLogType, string> LogTypes = new Dictionary<ActivityLogType, string>
        {
            {ActivityLogType.Recent, ""},
            {ActivityLogType.Archive, "All"},
        };
    }
}