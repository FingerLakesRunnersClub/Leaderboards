using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class StatisticsViewModel : ViewModel
    {
        public override string Title => "Statistics";
        public Dictionary<Course, Statistics> Courses { get; init; }
        public Statistics Total { get; init; }
    }
}