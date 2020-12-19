using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class DashboardResultsTable
    {
        public string Title { get; set; }
        public Course Course { get; set; }
        public FormattedResultType ResultType { get; set; }
        public Category Category { get; set; }
        public IEnumerable<DashboardResultRow> Rows { get; set; }
    }
}