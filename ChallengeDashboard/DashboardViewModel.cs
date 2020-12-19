using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class DashboardViewModel : ViewModel
    {
        public override string Title => "Dashboard";

        public override IDictionary<uint, string> CourseNames
            => CourseResults.Keys.ToDictionary(c => c.ID, c => c.Name);

        public DashboardViewModel(IEnumerable<Course> courses) => _courses = courses;


        public List<DashboardResultsTable> OverallResults => new List<DashboardResultsTable>
        {
            new DashboardResultsTable
            {
                Title = "Top Teams",
                ResultType = new FormattedResultType(ResultType.Team),
                Rows = Athlete.Teams.ToDictionary(t => t.Value, t => (byte)CourseResults.Sum(r => r.Key.TeamPoints().FirstOrDefault(p => p.Team == t.Value)?.TotalPoints ?? 0)).OrderBy(t => t.Value).Take(3).Select(t => new DashboardResultRow { Name = t.Key.Display, Value = t.Value.ToString() })
            }
        };


        public IDictionary<Course, List<DashboardResultsTable>> CourseResults => _courses.ToDictionary(c => c, Course =>
            new List<DashboardResultsTable>
            {
                new DashboardResultsTable
                {
                    Title = "Fastest (F)",
                    Course = Course,
                    ResultType = new FormattedResultType(ResultType.Fastest),
                    Category = Category.F,
                    Rows = Course.Fastest(Category.F).Take(3).Select(r => new DashboardResultRow
                        {Name = r.Athlete.Name, Value = r.Value.Display})
                },
                new DashboardResultsTable
                {
                    Title = "Fastest (M)",
                    Course = Course,
                    ResultType = new FormattedResultType(ResultType.Fastest),
                    Category = Category.M,
                    Rows = Course.Fastest(Category.M).Take(3).Select(r => new DashboardResultRow
                        {Name = r.Athlete.Name, Value = r.Value.Display})
                },
                new DashboardResultsTable
                {
                    Title = "Best Average (F)",
                    Course = Course,
                    ResultType = new FormattedResultType(ResultType.BestAverage),
                    Category = Category.F,
                    Rows = Course.BestAverage(Category.F).Take(3).Select(r => new DashboardResultRow
                        {Name = r.Athlete.Name, Value = r.Value.Display})
                },
                new DashboardResultsTable
                {
                    Title = "Best Average (M)",
                    Course = Course,
                    ResultType = new FormattedResultType(ResultType.BestAverage),
                    Category = Category.M,
                    Rows = Course.BestAverage(Category.M).Take(3).Select(r => new DashboardResultRow
                        {Name = r.Athlete.Name, Value = r.Value.Display})
                },
                new DashboardResultsTable
                {
                    Title = "Fastest (Team)",
                    Course = Course,
                    ResultType = new FormattedResultType(ResultType.Team),
                    Rows = Course.TeamPoints().OrderByDescending(p => p.AverageAgeGrade).Take(3).Select(r =>
                        new DashboardResultRow {Name = r.Team.Display, Value = r.AverageAgeGrade.Display})
                },
                new DashboardResultsTable
                {
                    Title = "Most Runs (Team)",
                    Course = Course,
                    ResultType = new FormattedResultType(ResultType.Team),
                    Rows = Course.TeamPoints().OrderByDescending(p => p.TotalRuns).Take(3).Select(r =>
                        new DashboardResultRow {Name = r.Team.Display, Value = r.TotalRuns.ToString()})
                }
            });

        private readonly IEnumerable<Course> _courses;
    }
}
