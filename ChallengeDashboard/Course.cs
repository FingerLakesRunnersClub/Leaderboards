using System;
using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class Course
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Distance { get; set; }
        public double Meters { get; set; }
        public string URL { get; set; }

        public IEnumerable<Result> Results { get; set; }

        public RankedList<Time> Fastest(Category category = null)
            => Rank(category, rs => true, rs => rs.OrderBy(r => r.Duration).First(), rs => rs.Min(r => r.Duration));

        public RankedList<Time> BestAverage(Category category = null)
            => Rank(category, rs => rs.Count() >= AverageThreshold(category), rs => rs.Average(AverageThreshold(category)), rs => rs.Average(AverageThreshold(category)).Duration);

        public RankedList<ushort> MostRuns(Category category = null)
            => RankDescending(category, rs => true, r => r.Average(), r => (ushort)r.Count());

        private IEnumerable<GroupedResult> GroupedResults(Category category = null)
            => Results.Where(r => category == null || r.Athlete.Category.Equals(category))
                .GroupBy(r => r.Athlete).Select(g => new GroupedResult(g));

        public ushort AverageThreshold(Category category = null)
            => GroupedResults(category).Any()
                ? (ushort)Math.Ceiling(GroupedResults(category).Average(r => r.Count()))
                : (ushort)0;

        public IEnumerable<TeamResults> TeamPoints()
        {
            var teamResults = GroupedResults()
                .GroupBy(g => g.Key.Team)
                .Where(t => t.Count() >= 10)
                .Select(t => new TeamResults
                {
                    Team = t.Key,
                    AverageAgeGrade = new AgeGrade(t.OrderBy(rs => rs.Min(r => r.Duration)).Take(10).Select(r => AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(r.First().Athlete.Category.Value ?? Category.M.Value ?? throw null, r.First().Athlete.Age, Meters, r.First().Duration.Value)).Average()),
                    TotalRuns = (ushort)t.Sum(rs => rs.Count())
                }).ToList();

            var fastestTeams = teamResults.OrderByDescending(t => t.AverageAgeGrade).ToArray();
            for (var x = 0; x < fastestTeams.Length; x++)
                fastestTeams[x].AgeGradePoints = (byte)(x + 1);

            var mostRunTeams = teamResults.OrderByDescending(t => t.TotalRuns).ToArray();
            for (var x = 0; x < mostRunTeams.Length; x++)
                mostRunTeams[x].MostRunsPoints = (byte)(x + 1);

            var topTeams = teamResults.OrderBy(t => t.TotalPoints).ToArray();
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
                    AgeGrade = new AgeGrade(athlete.Category != null
                        ? AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(athlete.Category.Value ?? throw null, athlete.Age, Meters, result.Duration.Value)
                        : 0)
                });
            }

            return ranks;
        }
    }
}