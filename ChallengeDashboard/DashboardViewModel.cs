using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class DashboardViewModel : ViewModel
    {
        public override string Title => "Dashboard";

        public override IDictionary<uint, string> CourseNames
            => _courses.ToDictionary(c => c.ID, c => c.Name);

        public DashboardViewModel(IEnumerable<Course> courses) => _courses = courses;

        public List<DashboardResultsTable> OverallResults
            => new List<DashboardResultsTable>
            {
                new DashboardResultsTable
                {
                    Title = "Most Points (F)",
                    Rows = _courses.SelectMany(c => c.Fastest(Category.F))
                        .GroupBy(r => r.Athlete)
                        .ToDictionary(g => g.Key, g => g.Sum(r => r.Points.Value))
                        .OrderByDescending(r => r.Value).Take(3)
                        .Select(r => new DashboardResultRow {Name = r.Key.Name, Value = new Points(r.Value).Display})
                },
                new DashboardResultsTable
                {
                    Title = "Most Points (M)",
                    Rows = _courses.SelectMany(c => c.Fastest(Category.M))
                        .GroupBy(r => r.Athlete)
                        .ToDictionary(g => g.Key, g => g.Sum(r => r.Points.Value))
                        .OrderByDescending(r => r.Value).Take(3)
                        .Select(r => new DashboardResultRow {Name = r.Key.Name, Value = new Points(r.Value).Display})
                },
                new DashboardResultsTable
                {
                    Title = "Most Runs",
                    Rows = _courses.SelectMany(c => c.MostRuns())
                        .GroupBy(r => r.Athlete)
                        .ToDictionary(g => g.Key, g => g.Sum(r => r.Value))
                        .OrderByDescending(r => r.Value).Take(3)
                        .Select(r => new DashboardResultRow {Name = r.Key.Name, Value = r.Value.ToString()})
                },
                new DashboardResultsTable
                {
                    Title = "Top Teams",
                    Rows = _courses.SelectMany(c => c.TeamPoints())
                        .GroupBy(r => r.Team)
                        .ToDictionary(g => g.Key, g => g.Sum(r => r.TotalPoints) + 14 * (10 - g.Count()))
                        .OrderBy(g => g.Value).Take(3)
                        .Select(t => new DashboardResultRow {Name = t.Key.Display, Value = t.Value.ToString()})
                }
            };

        public IDictionary<Course, List<DashboardResultsTable>> CourseResults => _courses.ToDictionary(c => c, c =>
            new List<DashboardResultsTable>
            {
                new()
                {
                    Title = "Fastest (F)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Fastest),
                    Category = Category.F,
                    Link = $"/Course/{c.ID}/{ResultType.Fastest}/F",
                    Rows = c.Fastest(Category.F).Take(3).Select(r => new DashboardResultRow
                        {Name = r.Athlete.Name, Value = r.Value.Display})
                },
                new DashboardResultsTable
                {
                    Title = "Fastest (M)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Fastest),
                    Category = Category.M,
                    Link = $"/Course/{c.ID}/{ResultType.Fastest}/M",
                    Rows = c.Fastest(Category.M).Take(3).Select(r => new DashboardResultRow
                        {Name = r.Athlete.Name, Value = r.Value.Display})
                },
                new DashboardResultsTable
                {
                    Title = "Best Average (F)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.BestAverage),
                    Category = Category.F,
                    Link = $"/Course/{c.ID}/{ResultType.BestAverage}/F",
                    Rows = c.BestAverage(Category.F).Take(3).Select(r => new DashboardResultRow
                        {Name = r.Athlete.Name, Value = r.Value.Display})
                },
                new DashboardResultsTable
                {
                    Title = "Best Average (M)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.BestAverage),
                    Category = Category.M,
                    Link = $"/Course/{c.ID}/{ResultType.BestAverage}/M",
                    Rows = c.BestAverage(Category.M).Take(3).Select(r => new DashboardResultRow
                        {Name = r.Athlete.Name, Value = r.Value.Display})
                },
                new DashboardResultsTable
                {
                    Title = "Fastest (Team)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Team),
                    Link = $"/Course/{c.ID}/{ResultType.Team}",
                    Rows = c.TeamPoints().OrderByDescending(p => p.AverageAgeGrade).Take(3).Select(r =>
                        new DashboardResultRow {Name = r.Team.Display, Value = r.AverageAgeGrade.Display})
                },
                new DashboardResultsTable
                {
                    Title = "Most Runs (Team)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Team),
                    Link = $"/Course/{c.ID}/{ResultType.Team}",
                    Rows = c.TeamPoints().OrderByDescending(p => p.TotalRuns).Take(3).Select(r =>
                        new DashboardResultRow {Name = r.Team.Display, Value = r.TotalRuns.ToString()})
                }
            });

        private readonly IEnumerable<Course> _courses;
    }
}