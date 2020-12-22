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
        {
            get
            {
                var vm = new OverallViewModel(_courses);
                return new List<DashboardResultsTable>
                {
                    new DashboardResultsTable
                    {
                        Title = "Most Points (F)",
                        Link = "/Overall/Points/F",
                        Rows = vm.MostPoints(Category.F).Take(3)
                            .Select(r => new DashboardResultRow {Name = r.Athlete.Name, Value = r.Value.Display })
                    },
                    new DashboardResultsTable
                    {
                        Title = "Most Points (M)",
                        Link = "/Overall/Points/M",
                        Rows = vm.MostPoints(Category.M).Take(3)
                            .Select(r => new DashboardResultRow {Name = r.Athlete.Name, Value = r.Value.Display})
                    },
                    new DashboardResultsTable
                    {
                        Title = "Most Miles",
                        Link = "/Overall/Miles",
                        Rows = vm.MostMiles().Take(3)
                            .Select(r => new DashboardResultRow {Name = r.Athlete.Name, Value = r.Value.ToString()})
                    },
                    new DashboardResultsTable
                    {
                        Title = "Top Teams",
                        Link = "/Overall/Team",
                        Rows = vm.TeamPoints().Take(3)
                            .Select(t => new DashboardResultRow {Name = t.Team.Display, Value = t.TotalPoints.ToString() })
                    }
                };
            }
        }

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