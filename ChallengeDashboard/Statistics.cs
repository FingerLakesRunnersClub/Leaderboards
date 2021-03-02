using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class Statistics
    {
        public IDictionary<string, int> Participants { get; init; }
        public Dictionary<string, int> Runs { get; init; }
        public Dictionary<string, double> Miles { get; init; }
        public Dictionary<string, double> Average { get; init; }
    }
}