using System;
using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class Course
    {
        public const double MetersPerMile = 1609.344;

        public uint ID { get; init; }
        public string Name { get; init; }
        public string Type { get; init; }
        public string Distance { get; init; }
        public double Meters { get; set; }
        public string URL { get; init; }

        public IEnumerable<Result> Results { get; set; }

        public RankedList<Time> Fastest(Category category = null)
            => Rank(category, rs => true, rs => rs.OrderBy(r => r.Duration).First(), rs => rs.Min(r => r.Duration));

        public RankedList<Time> BestAverage(Category category = null)
            => Rank(category, rs => rs.Count() >= AverageThreshold(category), rs => rs.Average(AverageThreshold(category)), rs => rs.Average(AverageThreshold(category)).Duration);

        public RankedList<ushort> MostRuns(Category category = null)
            => RankDescending(category, rs => true, r => r.Average(), r => (ushort)r.Count());

        private IEnumerable<GroupedResult> GroupedResults(Category category = null)
            => Results.Where(r => category == null || (r.Athlete.Category?.Equals(category) ?? false))
                .GroupBy(r => r.Athlete).Select(g => new GroupedResult(g));

        public ushort AverageThreshold(Category category = null)
            => GroupedResults(category).Any()
                ? (ushort)Math.Ceiling(GroupedResults(category).Average(r => r.Count()))
                : (ushort)0;

        public IEnumerable<TeamResults> TeamPoints()
        {
            var teamResults = GroupedResults()
                .GroupBy(g => g.Key.Team)
                .Select(t => new TeamResults
                {
                    Team = t.Key,
                    AverageAgeGrade = new AgeGrade(t.OrderBy(rs => rs.Min(r => r.Duration)).Take(10).Select(r => AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(r.First().Athlete.Category?.Value ?? Category.M.Value ?? throw null, r.First().Athlete.Age, Meters, r.First().Duration.Value)).Average()),
                    TotalRuns = (ushort)t.Sum(rs => rs.Count())
                }).ToList();

            var fastestTeams = teamResults.OrderByDescending(t => t.AverageAgeGrade).ToArray();
            for (var x = 0; x < fastestTeams.Length; x++)
                fastestTeams[x].AgeGradePoints = x > 0 && fastestTeams[x].AverageAgeGrade.Equals(fastestTeams[x-1].AverageAgeGrade)
                    ? fastestTeams[x-1].AgeGradePoints
                    : (byte)(7 - x);

            var mostRunTeams = teamResults.OrderByDescending(t => t.TotalRuns).ToArray();
            for (var x = 0; x < mostRunTeams.Length; x++)
                mostRunTeams[x].MostRunsPoints = x > 0 && mostRunTeams[x].TotalRuns.Equals(mostRunTeams[x - 1].TotalRuns)
                        ? mostRunTeams[x - 1].MostRunsPoints
                        : (byte)(7 - x);

            var topTeams = teamResults.OrderByDescending(t => t.TotalPoints).ToArray();
            for (var x = 0; x < topTeams.Length; x++)
                topTeams[x].Rank = x > 0 && topTeams[x].TotalPoints == topTeams[x - 1].TotalPoints
                    ? topTeams[x - 1].Rank
                    : (byte)(x + 1);

            return topTeams.ToList();
        }

        private RankedList<T> Rank<T>(Category category, Func<GroupedResult, bool> filter, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
            => RankedList(GroupedResults(category).Where(filter).OrderBy(sort), getResult, sort);

        private RankedList<T> RankDescending<T>(Category category, Func<GroupedResult, bool> filter, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
            => RankedList(GroupedResults(category).Where(filter).OrderByDescending(sort), getResult, sort);

        private RankedList<T> RankedList<T>(IOrderedEnumerable<GroupedResult> sorted, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> getValue)
        {
            var ranks = new RankedList<T>();

            var list = sorted.ThenBy(rs => getResult(rs).Duration).ToList();
            for (ushort rank = 1; rank <= list.Count; rank++)
            {
                var results = list[rank - 1];
                var athlete = results.Key;
                var result = getResult(results);
                var value = getValue(results);
                ranks.Add(new Ranked<T>
                {
                    Rank = ranks.Any() && ranks.Last().Value.Equals(value) ? ranks.Last().Rank : rank,
                    Athlete = athlete,
                    Result = result,
                    Value = value,
                    Count = (uint)results.Count(),
                    BehindLeader = rank == 1 ? new Time(TimeSpan.Zero) : result.Behind(ranks.First().Result),
                    Points = new Points(rank == 1 ? 100 : ranks.First().Result.Duration.Value.TotalSeconds / result.Duration.Value.TotalSeconds * 100), 
                    AgeGrade = new AgeGrade(athlete.Category != null
                        ? AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(athlete.Category.Value ?? throw null, athlete.Age, Meters, result.Duration.Value)
                        : 0)
                });
            }

            return ranks;
        }
    }
}