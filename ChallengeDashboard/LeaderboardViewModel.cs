using System;
using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class LeaderboardViewModel : ViewModel
    {
        public override string Title => "Leaderboard";


        public override IDictionary<uint, string> CourseNames
            => _courses.ToDictionary(c => c.ID, c => c.Name);

        public LeaderboardResultType LeaderboardResultType { get; }

        private readonly IEnumerable<Course> _courses;
        private readonly Func<LeaderboardTable, bool> _filter;

        public LeaderboardViewModel(IEnumerable<Course> courses, LeaderboardResultType type)
        {
            _courses = courses;
            LeaderboardResultType = type;
            _filter = GetFilter(type);
        }

        public List<LeaderboardTable> OverallResults
        {
            get
            {
                var vm = new OverallViewModel(_courses);
                return new List<LeaderboardTable>
                {

                    new LeaderboardTable
                    {
                        Title = "Top Teams",
                        Link = "/Overall/Team",
                        Rows = vm.TeamPoints().Take(3)
                            .Select(t => new LeaderboardRow { Rank = t.Rank, Name = t.Team.Display, Link = $"/Team/Index/{t.Team.Value}", Value = t.TotalPoints.ToString() })
                    },
                    new LeaderboardTable
                    {
                        Title = "Most Miles",
                        Link = "/Overall/Miles",
                        Rows = vm.MostMiles().Take(3)
                            .Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString("F1")})
                    },
                    new LeaderboardTable
                    {
                        Title = "Most Points (F)",
                        Link = "/Overall/Points/F",
                        Rows = vm.MostPoints(Category.F).Take(3)
                            .Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
                    },
                    new LeaderboardTable
                    {
                        Title = "Most Points (M)",
                        Link = "/Overall/Points/M",
                        Rows = vm.MostPoints(Category.M).Take(3)
                            .Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display})
                    }
                };
            }
        }

        public IDictionary<Course, IEnumerable<LeaderboardTable>> CourseResults => _courses.ToDictionary(c => c, c =>
            new List<LeaderboardTable>
            {
                new()
                {
                    Title = "Fastest (F)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Fastest),
                    Category = Category.F,
                    Link = $"/Course/{c.ID}/{ResultType.Fastest}/F",
                    Rows = c.Fastest(Category.F).Take(3)
                        .Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display})
                },
                new LeaderboardTable
                {
                    Title = "Fastest (M)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Fastest),
                    Category = Category.M,
                    Link = $"/Course/{c.ID}/{ResultType.Fastest}/M",
                    Rows = c.Fastest(Category.M).Take(3)
                            .Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display})
                },
                new LeaderboardTable
                {
                    Title = "Best Average (F)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.BestAverage),
                    Category = Category.F,
                    Link = $"/Course/{c.ID}/{ResultType.BestAverage}/F",
                    Rows = c.BestAverage(Category.F).Take(3)
                    .Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display})
                },
                new LeaderboardTable
                {
                    Title = "Best Average (M)",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.BestAverage),
                    Category = Category.M,
                    Link = $"/Course/{c.ID}/{ResultType.BestAverage}/M",
                    Rows = c.BestAverage(Category.M).Take(3)
                        .Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display})
                },
                new LeaderboardTable
                {
                    Title = "Most Runs",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.MostRuns),
                    Link = $"/Course/{c.ID}/{ResultType.MostRuns}",
                    Rows = c.MostRuns().Take(3)
                      .Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString()})
                },
                new LeaderboardTable
                {
                    Title = "Age Grade",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Team),
                    Link = $"/Course/{c.ID}/{ResultType.Team}",
                    Rows = c.TeamPoints().OrderByDescending(p => p.AverageAgeGrade).Take(3)
                        .Select(r => new LeaderboardRow { Rank = new Rank(r.AgeGradePoints), Name = r.Team.Display, Link = $"/Team/Index/{r.Team.Value}", Value = r.AverageAgeGrade.Display })
                },
                new LeaderboardTable
                {
                    Title = "Most Runs",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Team),
                    Link = $"/Course/{c.ID}/{ResultType.Team}",
                    Rows = c.TeamPoints().OrderByDescending(p => p.TotalRuns).Take(3)
                        .Select(r => new LeaderboardRow { Rank = new Rank(r.MostRunsPoints), Name = r.Team.Display, Link = $"/Team/Index/{r.Team.Value}", Value = r.TotalRuns.ToString() })
                },
                new LeaderboardTable
                {
                    Title = "Total Points",
                    Course = c,
                    ResultType = new FormattedResultType(ResultType.Team),
                    Link = $"/Course/{c.ID}/{ResultType.Team}",
                    Rows = c.TeamPoints().Take(3)
                        .Select(r => new LeaderboardRow { Rank = r.Rank, Name = r.Team.Display, Link = $"/Team/Index/{r.Team.Value}", Value = r.TotalPoints.ToString() })
                }
            }.Where(_filter));

        private static Func<LeaderboardTable, bool> GetFilter(LeaderboardResultType type)
        {
            switch (type)
            {
                case LeaderboardResultType.F:
                    return t => t.Category?.Equals(Category.F) ?? t.ResultType.Value != ResultType.Team;
                case LeaderboardResultType.M:
                    return t => t.Category?.Equals(Category.M) ?? t.ResultType.Value != ResultType.Team;
                default:
                    return t => t.ResultType.Value == ResultType.Team;
            }
        }
    }
}