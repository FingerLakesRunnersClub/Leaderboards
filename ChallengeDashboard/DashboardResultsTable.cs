using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class DashboardResultsTable
    {
        public string Title { get; init; }
        public Course Course { get; init; }
        public FormattedResultType ResultType { get; init; }
        public Category Category { get; init; }
        public string Link { get; set; }
        public IEnumerable<DashboardResultRow> Rows { get; init; }
    }
}