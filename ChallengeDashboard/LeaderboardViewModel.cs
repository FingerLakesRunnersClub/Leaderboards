using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class LeaderboardViewModel : ViewModel
    {
        public override string Title => "Leaderboard";

        public override IDictionary<uint, string> CourseNames
            => _courses.ToDictionary(c => c.ID, c => c.Name);

        public LeaderboardViewModel(IEnumerable<Course> courses) => _courses = courses;

        public List<LeaderboardTable> OverallResults
        {
            get
            {
                var vm = new OverallViewModel(_courses);
                return new List<LeaderboardTable>
                {
                    new LeaderboardTable
                    {
                        Title = "Most Points (F)",
                        Link = "/Overall/Points/F",
                        Rows = vm.MostPoints(Category.F).Take(3)
                            .Select(r => new LeaderboardRow {Name = r.Athlete.Name, Value = r.Value.Display })
                    },
                    new LeaderboardTable
                    {
                        Title = "Most Points (M)",
                        Link = "/Overall/Points/M",
                        Rows = vm.MostPoints(Category.M).Take(3)
                            .Select(r => new LeaderboardRow {Name = r.Athlete.Name, Value = r.Value.Display})
                    },
                    new LeaderboardTable
                    {
                        Title = "Most Miles",
                        Link = "/Overall/Miles",
                        Rows = vm.MostMiles().Take(3)
                            .Select(r => new LeaderboardRow {Name = r.Athlete.Name, Value = r.Value.ToString()})
                    },
                    new LeaderboardTable
                    {
                        Title = "Top Teams",
                        Link = "/Overall/Team",
                        Rows = vm.TeamPoints().Take(3)
                            .Select(t => new LeaderboardRow {Name = t.Team.Display, Value = t.TotalPoints.ToString() })
                    }
                };
            }
        }

        public IDictionary<Course, List<LeaderboardTable>> CourseResults => _courses.ToDictionary(c => c, c =>
            new List<LeaderboardTable>
            {
                new()
                {
                    Title = "Fastest (F)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Fastest),
                    Category = Category.F,
                    Link = $"/Course/{c.ID}/{ResultType.Fastest}/F",
                    Rows = c.Fastest(Category.F).Take(3).Select(r => new LeaderboardRow
                        {Name = r.Athlete.Name, Value = r.Value.Display})
                },
                new LeaderboardTable
                {
                    Title = "Fastest (M)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Fastest),
                    Category = Category.M,
                    Link = $"/Course/{c.ID}/{ResultType.Fastest}/M",
                    Rows = c.Fastest(Category.M).Take(3).Select(r => new LeaderboardRow
                        {Name = r.Athlete.Name, Value = r.Value.Display})
                },
                new LeaderboardTable
                {
                    Title = "Best Average (F)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.BestAverage),
                    Category = Category.F,
                    Link = $"/Course/{c.ID}/{ResultType.BestAverage}/F",
                    Rows = c.BestAverage(Category.F).Take(3).Select(r => new LeaderboardRow
                        {Name = r.Athlete.Name, Value = r.Value.Display})
                },
                new LeaderboardTable
                {
                    Title = "Best Average (M)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.BestAverage),
                    Category = Category.M,
                    Link = $"/Course/{c.ID}/{ResultType.BestAverage}/M",
                    Rows = c.BestAverage(Category.M).Take(3).Select(r => new LeaderboardRow
                        {Name = r.Athlete.Name, Value = r.Value.Display})
                },
                new LeaderboardTable
                {
                    Title = "Most Runs",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.MostRuns),
                    Link = $"/Course/{c.ID}/{ResultType.MostRuns}",
                    Rows = c.MostRuns().Take(3).Select(r => new LeaderboardRow
                        {Name = r.Athlete.Name, Value = r.Value.ToString()})
                },
                new LeaderboardTable
                {
                    Title = "Fastest (Team)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Team),
                    Link = $"/Course/{c.ID}/{ResultType.Team}",
                    Rows = c.TeamPoints().OrderByDescending(p => p.AverageAgeGrade).Take(3).Select(r =>
                        new LeaderboardRow {Name = r.Team.Display, Value = r.AverageAgeGrade.Display})
                },
                new LeaderboardTable
                {
                    Title = "Most Runs (Team)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Team),
                    Link = $"/Course/{c.ID}/{ResultType.Team}",
                    Rows = c.TeamPoints().OrderByDescending(p => p.TotalRuns).Take(3).Select(r =>
                        new LeaderboardRow {Name = r.Team.Display, Value = r.TotalRuns.ToString()})
                }
            });

        private readonly IEnumerable<Course> _courses;
    }
}