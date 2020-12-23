using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class AthleteViewModel : ViewModel
    {
        public override string Title => Athlete.Name;

        public Athlete Athlete { get; init; }
        public TeamResults TeamResults { get; init; }
        public Ranked<Points> OverallPoints { get; init; }
        public Ranked<double> OverallMiles { get; init; }
        public IDictionary<Course, Ranked<Time>> Fastest { get; init; }
        public IDictionary<Course, Ranked<Time>> Average { get; init; }
        public IDictionary<Course, Ranked<ushort>> Runs { get; init; }
        public IDictionary<Course, IEnumerable<Result>> AllResults { get; init; }
    }
}