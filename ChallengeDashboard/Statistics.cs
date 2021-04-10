using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class Statistics
    {
        public IDictionary<string, int> Participants { get; init; }
        public IDictionary<string, int> Runs { get; init; }
        public IDictionary<string, double> Miles { get; init; }
        public IDictionary<string, double> Average { get; init; }
    }
}